using System.Data.OleDb;
using System.Runtime.CompilerServices;

namespace Soltec.Sae.Api
{
    public class BoletoService
    {
        SujetoService sujetoService;
        CosechaService cosechaService;
        public string IdNumerador { get; set; } = "0001";
        NumeradorService numeradorService;

        public BoletoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
            sujetoService = new SujetoService(connectionStringBase);
            cosechaService= new CosechaService(connectionStringBase);
            numeradorService = new NumeradorService(connectionStringBase);            
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
                
        public string GetSiguienteNumeroBoleto()
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            string queryModulos = "SELECT n_bol as numero FROM modulos";

            using (OleDbConnection cnn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(queryModulos, cnn))
                {
                    cnn.Open();

                    // 1. Obtener el número base desde 'modulos'
                    object resultModulos = command.ExecuteScalar();
                    Int64 numeroReferencia = 1;

                    if (resultModulos != null && resultModulos != DBNull.Value)
                    {
                        numeroReferencia = Convert.ToInt64(resultModulos) + 1;
                    }

                    // 2. Traer a C# todos los boletos ocupados desde el número de referencia hacia arriba
                    // Traemos el campo como texto para no forzar funciones pesadas en el WHERE de FoxPro
                    string queryBoletos = "SELECT bol_nro FROM boletos WHERE VAL(bol_nro) >= " + numeroReferencia.ToString();
                    command.CommandText = queryBoletos;

                    // Usamos un HashSet para búsquedas instantáneas en memoria (.Contains es O(1))
                    HashSet<Int64> boletosOcupados = new HashSet<Int64>();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader[0] != DBNull.Value)
                            {
                                // Convertimos a entero en C# libre de errores de FoxPro
                                Int64 numBoleto = Convert.ToInt64(reader[0]);
                                boletosOcupados.Add(numBoleto);
                            }
                        }
                    }

                    // 3. Buscar el primer número libre en C# empezando desde numeroReferencia
                    Int64 siguienteNumero = numeroReferencia;
                    while (boletosOcupados.Contains(siguienteNumero))
                    {
                        siguienteNumero++; // Si está ocupado, salta al siguiente
                    }

                    // 4. Retornar formateado con 10 ceros a la izquierda
                    return siguienteNumero.ToString().PadLeft(10, '0');
                }
            }
        }
        public List<Boleto> List(string idCuenta , string idCosecha ,DateTime fecha) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  bol_nro, bol_fec, bol_produ, bol_kgs, bol_pre_n, bol_pre_l, bol_obs1, bol_obs2, bol_cosec, bol_confi," +
                         "bol_fpa, bol_loc, bol_tra, bol_ftrans, bol_reg, bol_ctto, pe,numero, estado_liq,fijar,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,id_condvta,CondicionVenta.nombre as CondicionVenta,Divisas.desc as Moneda " +
                         "FROM boletos " +
                         "LEFT JOIN Produmae on produmae.codigo = boletos.bol_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = boletos.bol_cosec " +
                         "LEFT JOIN Divisas on Divisas.id_divisa = boletos.id_divisa " +
                         "LEFT JOIN CondicionVenta on CondicionVenta.id = boletos.id_condvta " +
                         "WHERE(bol_produ = '" + idCuenta + "' OR empty('" + idCuenta + "')) AND (bol_cosec = '" + idCosecha + "' OR empty('" + idCosecha + "')) " +
                         "and bol_fec <= ctod('" + fecha.ToString("MM-dd-yyy") + "')";
           OleDbDataReader reader = command.ExecuteReader();
            List<Boleto> result = new List<Boleto>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        public bool Insert(Boleto boleto)
        {
            // 1. Calculamos y asignamos el nuevo número formateado
            var id = this.GetSiguienteNumeroBoleto();
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            // Escapar valores para evitar problemas con comillas
            string fecha = boleto.Fecha.ToString("MM-dd-yyyy");
            
            string obs = string.IsNullOrEmpty(boleto.Obs) ? "" : boleto.Obs.Replace("'", "''");
            string ntra = Guid.NewGuid().ToString();
            string estadoLiq = "001"; //Pendiente de liquidar


            // Manejar valores nulos
            string pesoNeto = boleto.PesoNeto.ToString().Replace(",", ".") ?? "0";
            string precio = boleto.Precio.ToString().Replace(",", ".") ?? "0";
            string precioEnLetras = Utilities.NumeroALetras(boleto.Precio);
            string idCuenta = boleto.IdCuenta?.ToString() ?? "0";
            string idCosecha = boleto.IdCosecha?.ToString() ?? "0";
            string idCondicionVenta = boleto.IdCondicionVenta?.ToString() ?? "0";
            string aFijar = boleto.AFijar ? ".T." : ".F.";
            string confirmado = ".T.";
            string idDivisa = boleto.IdMoneda;
            string origen = "APP";
            //Numeracion
            var numerador = numeradorService.Incrementar(this.IdNumerador);
            int pe = numerador?.Pe ?? 0;
            Int64 numero = numerador?.Numero ?? 0;
            

            // Construir la consulta SQL con los valores directamente
            string query = $@"INSERT INTO boletos (
        bol_nro, bol_fec, bol_produ, bol_kgs, bol_pre_n, bol_pre_l,
        bol_obs1, bol_obs2, bol_cosec, bol_confi, bol_fpa, bol_loc,         
        estado_liq, fijar, id_condvta, id_divisa,origen,pe,numero
    ) VALUES (
        '{id}', 
        CTOD('{fecha}'), 
        '{idCuenta}', 
        {pesoNeto}, 
        {precio}, 
        '{precioEnLetras}', 
        '', 
        '{obs}', 
        '{idCosecha}', 
        {confirmado}, 
        CTOD('{fecha}'), 
        '', 
        '{estadoLiq}', 
        {aFijar}, 
        '{idCondicionVenta}', 
       '{idDivisa}',
       '{origen}',
        {pe},
        {numero}         
    )";

            using (OleDbConnection cnn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(query, cnn))
                {
                    cnn.Open();
                    int filasAfectadas = command.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }
        public List<BoletoPendienteLiquidar> ListPendiente(string idCuenta, string idCosecha, DateTime fecha,DateTime fechaHasta)
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = " SELECT liqui.bolvta as Numero, SUM(LIQUI.p_neto) as Liquidado from liqui group by liqui.bolvta " +
                         "WHERE liqui.fecha <= ctod('" + fechaHasta.ToString("MM-dd-yyy") + "') ";

            OleDbDataReader reader = command.ExecuteReader();
            //Parse Pendiente Liquidar
            List<BoletoLiquidadoView> resultBoletoLiquidado = new List<BoletoLiquidadoView>();
            while (reader.Read())
            {
                resultBoletoLiquidado.Add(this.ParseBoletoLiquidado(reader));
            }
            reader.Close();
            command.CommandText = "SELECT  bol_nro, bol_fec, bol_produ, bol_kgs, bol_pre_n, bol_pre_l, bol_obs1, bol_obs2, bol_cosec, bol_confi," +
                         "bol_fpa, bol_loc, bol_tra, bol_ftrans, bol_reg, bol_ctto, pe,numero, estado_liq,fijar,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,id_condvta,CondicionVenta.nombre as CondicionVenta,Divisas.desc as Moneda " +
                         "FROM boletos " +
                         "LEFT JOIN Produmae on produmae.codigo = boletos.bol_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = boletos.bol_cosec " +
                          "LEFT JOIN Divisas on Divisas.id_divisa = boletos.id_divisa " +
                         "LEFT JOIN CondicionVenta on CondicionVenta.id = boletos.id_condvta " +
                         "WHERE(bol_produ = '" + idCuenta + "' OR empty('" + idCuenta + "')) AND (bol_cosec = '" + idCosecha + "' OR empty('" + idCosecha + "')) " +
                         "and bol_fec >= ctod('" + fecha.ToString("MM-dd-yyy") + "') and bol_fec <= ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')";

           
            reader = command.ExecuteReader();
            List<Boleto> tmpResult = new List<Boleto>();
            while (reader.Read())
            {
                tmpResult.Add(this.Parse(reader));
            }
            var result = from r in tmpResult                            
                            join l in resultBoletoLiquidado on r.Id equals l.Id into details
                            from d in details.DefaultIfEmpty()
                            where r.PesoNeto - details.Sum(p=>p.Liquidado) > 0
                            select new BoletoPendienteLiquidar { Id = r.Id, Fecha = r.Fecha, IdCosecha=r.IdCosecha,NombreCosecha = r.Cosecha.Nombre,IdCuenta = r.IdCuenta,NombreCuenta =r.Cuenta.Nombre,
                                Precio= r.Precio,PesoNeto = r.PesoNeto,Moneda= r.Moneda,IdCondicionVenta=r.IdCondicionVenta,CondicionVenta = r.CondicionVenta,
                                PesoLiquidado = details.Sum(l=>l.Liquidado), PesoPendienteLiquidar = r.PesoNeto - details.Sum(l => l.Liquidado)                            };
                           
            cnn.Close();
            return result.ToList();
        }
        public Boleto FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  bol_nro, bol_fec, bol_produ, bol_kgs, bol_pre_n, bol_pre_l, bol_obs1, bol_obs2, bol_cosec, bol_confi," +
                         "bol_fpa, bol_loc, bol_tra, bol_ftrans, bol_reg, bol_ctto, pe,numero, estado_liq,fijar,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,id_condvta,CondicionVenta.nombre as CondicionVenta,Divisas.desc as Moneda " +
                         "FROM boletos " +
                         "LEFT JOIN Produmae on produmae.codigo = boletos.bol_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = boletos.bol_cosec " +
                       "LEFT JOIN Divisas on Divisas.id_divisa = boletos.id_divisa " +
                         "LEFT JOIN CondicionVenta on CondicionVenta.id = boletos.id_condvta " +
                         "WHERE (bol_nro = '" + id + "')";
            OleDbDataReader reader = command.ExecuteReader();
            Boleto result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        public Int64 Total(string idCuenta, string idCosecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(bol_kgs) as Total " +
                                  "FROM Boletos " +
                                  "WHERE (bol_produ = '" + idCuenta + "') and " +
                                  " (bol_cosec = '" + idCosecha + "' and bol_fec <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) ";
                                  
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }
        public List<string> Validate(Boleto boleto)
        {
            var errores = new List<string>();

            if (boleto == null)
            {
                errores.Add("El objeto Boleto no puede ser nulo.");
                return errores; // Retornamos de inmediato porque no se puede evaluar el resto.
            }            
            if (string.IsNullOrWhiteSpace(boleto.IdSucursal))
                errores.Add("El Id de Sucursal es requerido.");

            if (string.IsNullOrWhiteSpace(boleto.IdCosecha))
                errores.Add("El Id de Cosecha es requerido.");
            var cosecha = cosechaService.FindOne(boleto.IdCosecha);
            if (cosecha == null)
                errores.Add("El Id de Cosecha no existe en la bd.");

            if (string.IsNullOrWhiteSpace(boleto.IdCuenta))
                errores.Add("El Id de Cuenta (Sujeto) es requerido.");
            
            //var sujeto = sujetoService.FindOne(boleto.IdCuenta);
            //if (sujeto == null)
            //    errores.Add("El Id de Cuenta no existe en la bd.");


            // Validación de Fechas
            if (boleto.Fecha == default)
                errores.Add("La Fecha de emisión no es válida.");

            if (boleto.FechaVencimiento == default)
                errores.Add("La Fecha de vencimiento no es válida.");

            if (boleto.FechaVencimiento < boleto.Fecha)
                errores.Add("La Fecha de vencimiento no puede ser anterior a la Fecha de emisión.");

            // Validación de Valores Numéricos
            if (boleto.Precio < 0)
                errores.Add("El Precio no puede ser un valor negativo.");

            if (string.IsNullOrWhiteSpace(boleto.IdMoneda))
                errores.Add("La Moneda es requerida.");

            string[] monedasValidas = { "0001", "0002" };
            if (!monedasValidas.Contains(boleto.IdMoneda))
                errores.Add("La Moneda no es válida requerida.");

            if (boleto.PesoNeto <= 0)
                errores.Add("El Peso Neto debe ser mayor a cero.");          
            

            return errores;
        }
        private Boleto Parse(OleDbDataReader reader)
        {
            Boleto item = new Boleto();
            item.Id = reader["bol_nro"].ToString().Trim();
            item.IdSucursal = this.IdSucursal;
            item.IdTransaccion = "BOLETO;" + this.IdSucursal + ";" + item.Id; 
            item.Fecha = (DateTime)reader["bol_fec"];
            item.IdCosecha = reader["bol_cosec"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["bol_cosec"].ToString().Trim();
            cosecha.Nombre = reader["NombreCosecha"].ToString().Trim();
            item.Cosecha = cosecha;
            item.IdCuenta = reader["bol_produ"].ToString().Trim();
            Sujeto cuenta = new Sujeto();
            cuenta.Id = reader["bol_produ"].ToString().Trim();
            cuenta.Nombre = reader["NombreProductor"].ToString().Trim();
            cuenta.NumeroDocumento = reader["CuitProductor"].ToString().Trim();
            item.Cuenta = cuenta;            
            item.PesoNeto = Convert.ToInt64(reader["bol_kgs"].ToString().Trim());
            item.Precio = (decimal)reader["bol_pre_n"];
            item.Numero = reader["bol_nro"].ToString().Trim();
            item.Moneda = reader["moneda"].ToString().Trim();
            item.IdCondicionVenta = reader["id_condvta"].ToString().Trim();
            item.CondicionVenta = reader["CondicionVenta"].ToString().Trim();

            return item;
        }
        private BoletoLiquidadoView ParseBoletoLiquidado(OleDbDataReader reader) 
        {
            BoletoLiquidadoView item = new BoletoLiquidadoView();
            item.Id = reader["numero"].ToString().Trim();
            item.Liquidado = Convert.ToInt64(reader["liquidado"].ToString().Trim());
            return item;
        }
    }

    public class BoletoLiquidadoView 
    {
        public string Id { get; set; }
        public Int64 Liquidado { get; set; }
    }
    
}
