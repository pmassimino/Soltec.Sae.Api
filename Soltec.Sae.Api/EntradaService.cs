using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class EntradaService
    {
        LocalidadService localidadService;
        IList<Localidad> localidadList;
        public EntradaService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
            localidadService = new LocalidadService(this.ConnectionStringBase);
            localidadList = localidadService.List();
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
       
        public List<Entrada> List(string idCuenta , string idCosecha ,DateTime fecha , DateTime fechaHasta) 
        {
            if(fecha == null)fecha = DateTime.Now.AddYears(-100);
            if (fechaHasta == null) fechaHasta = DateTime.Now;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT en_n_rom,en_fecha,en_produ,produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,en_cerea,en_tipo,en_cosec,Cosechas.descri as NombreCosecha,en_proce,en_comp,cermae.descri as NombreCereal," +                                   
                                  "en_pes_bru,en_tara,en_pes_net,en_p_hum,en_m_hum,en_p_zar,en_m_zar,en_p_vol,en_m_vol,en_p_cal,en_m_cal,en_p_net,en_obser," +
                                  "en_trans,Transpor.tra_emp as NombreTransporte,Transpor.tra_cui as CuitTransporte,entrada.id_camion,en_pe_cp,en_n_cre,ctg,planta,kms,en_obser, " +
                                  "Camion.chofer as NombreChofer,Camion.patente_a as Patente_A,Camion.Patente_c as Patente_c,str(Camion.cuit_chofer,11,0) as CuitChofer,ntra, " +
                                  "entrada.id_locproc,locali.cdlocalida as LocProcedencia,entrada.id_locdest,entrada.ventadir " + 
                                  "FROM entrada " +
                                  "LEFT JOIN Produmae on produmae.codigo = entrada.en_produ " + 
                                  "LEFT JOIN Cosechas on cosechas.cod = entrada.en_cosec " +
                                  "LEFT JOIN Cermae on cosechas.cereal = cermae.cod_cer " +
                                  "LEFT JOIN Transpor on Transpor.tra_cod = entrada.en_trans " +
                                  "LEFT JOIN Camion on Camion.id_Camion = entrada.id_camion " +
                                  "LEFT JOIN LOCALI on locali.id_locali = entrada.id_locproc " +                                  
                                  "WHERE (en_produ = '" + idCuenta + "' OR empty('" + idCuenta + "')) and " +
                                  " (en_cosec = '" + idCosecha + "' OR empty('" + idCosecha + "')) and " + 
                                   "en_fecha >= ctod('" + fecha.ToString("MM-dd-yyy") + "') and en_fecha <= ctod('" + fechaHasta.ToString("MM-dd-yyy") + "') and " +
                                   "entrada.stock_plan = .f.";

            OleDbDataReader reader = command.ExecuteReader();
            List<Entrada> result = new List<Entrada>();            
            while (reader.Read())
            {                
                result.Add(this.Parse(reader));                
            }            
            cnn.Close();
            return result;
        }
        public Entrada FindOne(string id) 
        {

            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT en_n_rom,en_fecha,en_produ,produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,en_cerea,en_tipo,en_cosec,Cosechas.descri as NombreCosecha,en_proce,en_comp," +
                                  "en_pes_bru,en_tara,en_pes_net,en_p_hum,en_m_hum,en_p_zar,en_m_zar,en_p_vol,en_m_vol,en_p_cal,en_m_cal,en_p_net,en_obser," +
                                  "en_trans,Transpor.tra_emp as NombreTransporte,Transpor.tra_cui as CuitTransporte,entrada.id_camion,en_pe_cp,en_n_cre,ctg,planta,kms,en_obser, " +
                                  "Camion.chofer as NombreChofer,Camion.patente_a as Patente_A,Camion.Patente_c as Patente_c,Camion.cuit_chofer as CuitChofer " +
                                  "FROM entrada " +
                                  "LEFT JOIN Produmae on produmae.codigo = entrada.en_produ " +
                                  "LEFT JOIN Cosechas on cosechas.cod = entrada.en_cosec " +
                                  "LEFT JOIN Transpor on Transpor.tra_cod = entrada.en_trans " +
                                  "LEFT JOIN Camion on Camion.id_Camion = entrada.id_camion " +
                                  "WHERE en_n_rom = '" + id + "'";

            OleDbDataReader reader = command.ExecuteReader();
            Entrada result = new Entrada();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        public Entrada FindBynTra(string ntra)
        {

            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT en_n_rom,en_fecha,en_produ,produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,en_cerea,en_tipo,en_cosec,Cosechas.descri as NombreCosecha,en_proce,en_comp,cermae.descri as NombreCereal," +
                                  "en_pes_bru,en_tara,en_pes_net,en_p_hum,en_m_hum,en_p_zar,en_m_zar,en_p_vol,en_m_vol,en_p_cal,en_m_cal,en_p_net,en_obser," +
                                  "en_trans,Transpor.tra_emp as NombreTransporte,Transpor.tra_cui as CuitTransporte,entrada.id_camion,en_pe_cp,en_n_cre,ctg,planta,kms,en_obser, " +
                                  "Camion.chofer as NombreChofer,Camion.patente_a as PatenteA,Camion.Patente_c as PatenteC,str(Camion.cuit_chofer,11,0) as CuitChofer,ntra, " +
                                  "entrada.id_locproc,locali.cdlocalida as LocProcedencia,entrada.id_locdest,entrada.ventadir " +
                                  "FROM entrada " +
                                  "LEFT JOIN Produmae on produmae.codigo = entrada.en_produ " +
                                  "LEFT JOIN Cosechas on cosechas.cod = entrada.en_cosec " +
                                  "LEFT JOIN Cermae on cosechas.cereal = cermae.cod_cer " +
                                  "LEFT JOIN Transpor on Transpor.tra_cod = entrada.en_trans " +
                                  "LEFT JOIN Camion on Camion.id_Camion = entrada.id_camion " +
                                  "LEFT JOIN LOCALI on locali.id_locali = entrada.id_locproc " +
                                  "WHERE ntra = '" + ntra + "'";

            OleDbDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                cnn.Close();
                return null; // Devuelve null si no hay registros
            }
            Entrada result = new Entrada();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        public Int64 Total(string idCuenta, string idCosecha,DateTime fechaHasta) 
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(en_p_net) as Total " +
                                  "FROM Entrada " + 
                                  "WHERE (en_produ = '" + idCuenta + "') and " +
                                  " (en_cosec = '" + idCosecha + "' and en_fecha <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and " +
                                  "entrada.stock_plan = .F.";
            OleDbDataReader reader = command.ExecuteReader();           
            while (reader.Read())
            {
                result = reader["Total"].ToString()== ""?0:Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }
        public Int64 TotalPlanta(string idPlanta, string idCosecha,DateTime fecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(en_p_net) as Total " +
                                  "FROM Entrada " +
                                  "WHERE (planta = '" + idPlanta + "') and " +
                                  " (en_cosec = '" + idCosecha +"')" +  " and (en_fecha BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                                  + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "'))  and " +
                                  "(noincstock = .f.)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }

        private Entrada Parse(OleDbDataReader reader)
        {
            Entrada item = new Entrada();
            item.IdSucursal = this.IdSucursal;
            item.Id = reader["en_n_rom"].ToString().Trim();
            item.IdTransaccion = reader["ntra"].ToString().Trim();
            item.Fecha = (DateTime)reader["en_fecha"];
            item.IdCosecha = reader["en_cosec"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["en_cosec"].ToString().Trim();
            cosecha.Nombre = reader["NombreCosecha"].ToString().Trim();
            item.NombreCosecha = cosecha.Nombre;
            item.NombreCereal = reader["NombreCereal"].ToString().Trim();
            item.IdCuenta = reader["en_produ"].ToString().Trim();            
            item.Nombre = reader["NombreProductor"].ToString().Trim();
            item.NumeroDocumento = reader["CuitProductor"].ToString().Trim();
            item.PesoBruto = Convert.ToInt64(reader["en_pes_bru"].ToString().Trim());
            item.PesoTara = Convert.ToInt64(reader["en_tara"].ToString().Trim());
            item.PesoNeto = Convert.ToInt64(reader["en_pes_net"].ToString().Trim());
            item.PorHumedad = (decimal)reader["en_p_hum"];
            item.MermaHumedad = Convert.ToInt16(reader["en_m_hum"].ToString().Trim());
            item.PorVolatil = (decimal)reader["en_p_vol"];
            item.MermaVolatil = Convert.ToInt16(reader["en_m_vol"].ToString().Trim());
            item.PorZaranda = (decimal)reader["en_p_zar"];
            item.MermaZaranda = Convert.ToInt16(reader["en_m_zar"].ToString().Trim());
            item.PorCalidad = (decimal)reader["en_p_cal"];
            item.MermaCalidad = Convert.ToInt16(reader["en_m_cal"].ToString().Trim());
            item.PesoNetoFinal = Convert.ToInt64(reader["en_p_net"].ToString().Trim());
            item.Procedencia = reader["en_proce"].ToString().Trim();
            item.Numero = reader["en_comp"].ToString().Trim();
            item.NumeroCartaPorte = reader["en_pe_cp"].ToString().Trim() + "-" + reader["en_n_cre"].ToString().Trim();  
            item.Ctg = Convert.ToInt64(reader["ctg"].ToString().Trim());
            item.IdPlanta = reader["planta"].ToString().Trim();
            Sujeto transporte = new Sujeto();
            transporte.Id = reader["en_trans"].ToString().Trim();
            transporte.Nombre = reader["NombreTransporte"].ToString().Trim();
            transporte.NumeroDocumento = reader["CuitTransporte"].ToString().Trim();
            item.Transporte = transporte;
            item.IdTransporte = reader["en_trans"].ToString().Trim();            
            item.Distancia = Convert.ToDecimal(reader["kms"].ToString().Trim());
            item.Observacion = reader["en_obser"].ToString().Trim();
            Sujeto chofer = new Sujeto();
            chofer.Id = reader["id_camion"].ToString().Trim();
            chofer.Nombre = reader["NombreChofer"].ToString().Trim();            
            chofer.NumeroDocumento = reader["CuitChofer"].ToString().Trim();    
            item.Chofer = chofer;
            item.PatenteA = reader["PatenteA"].ToString().Trim();
            item.PatenteC = reader["PatenteC"].ToString().Trim();
            item.IdLocalidadProcedencia = reader["id_locproc"].ToString().Trim();
            item.LocalidadProcedencia = reader["LocProcedencia"].ToString().Trim();
            item.IdLocalidadDestino = reader["id_locdest"].ToString().Trim();
            if (item.IdLocalidadDestino!=null) 
            {
                string localidadDestino = localidadList.Where(w => w.Id == item.IdLocalidadDestino).FirstOrDefault().Nombre.Trim();
                item.LocalidadDestino=localidadDestino;
            }
            
            item.Directo = Convert.ToBoolean(reader["ventadir"].ToString().Trim());
            return item;
        }
    }
}
