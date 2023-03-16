using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class ContratoService
    {
        public ContratoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Contrato> List(string idContrato,string numero,DateTime fecha,DateTime fechaHasta, string estado = "PENDIENTE", string tipo = "") 
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            string fin = estado == "FINALIZADO" ? ".t." : ".f.";
            string fijado = tipo =="A FIJAR"? ".t.":".f.";
            command.CommandText = "SELECT ctocod,ctonro,ctodesti,ctocose,ctokgs,ctofope,fijar,ctofin,cosechas.descri as NombreCosecha,cosechas.cereal,cermae.descri as NombreCereal,Destina.des_den as NombreComprador " +
                "FROM contrato " +
                "LEFT JOIN cosechas ON cosechas.cod = ctocose " +
                "LEFT JOIN cermae ON cermae.cod_cer = cosechas.cereal " +
                "LEFT JOIN destina On Destina.des_cod = ctodesti " +
                "WHERE (ctocod = '" + idContrato + "' OR empty('" + idContrato + "')) " +
                "AND  (ctoNRO = '" + numero + "' OR empty('" + numero + "'))" +
                "and ctofope >= ctod('" + fecha.ToString("MM-dd-yyy") + "') and ctofope <= ctod('" + fechaHasta.ToString("MM-dd-yyy") + "') " + 
                "and (ctofin = " + fin + " or empty('" + estado.Trim() + "')) " + 
                "and (fijar = " + fijado + " or empty('" + tipo.Trim() + "')) ";


            OleDbDataReader reader = command.ExecuteReader();
            List<Contrato> result = new List<Contrato>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
       
        public List<EstadoContratoView> ListEstado(string idContrato,string numero,DateTime fecha,DateTime fechaHasta,string estado  = "PENDIENTE",string tipo = "") 
        {
            var tmpContratos = this.List(idContrato,numero,fecha,fechaHasta,estado,tipo);

            List<EstadoContratoView> result = new List<EstadoContratoView>();
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            
            foreach (var contrato in tmpContratos) 
            {                
                //Aplicado
                command.CommandText = "SELECT PesoNeto as PesoNeto from Aplicacion where idContrato='" + contrato.Id.Trim() + "'";
                OleDbDataReader reader = command.ExecuteReader();
                Int64 pesoAplicado = 0;
                while (reader.Read()) 
                {
                    pesoAplicado += Convert.ToInt64(reader["PesoNeto"].ToString().Trim());
                }
                reader.Close();
                //Fijado
                Int64 pesoFijado = 0;
                if (contrato.Tipo.Trim() == "A FIJAR")
                {
                    //Fijado
                    command.CommandText = "SELECT Kilos as PesoNeto from cttokgs where ctto='" + contrato.Id.Trim() + "'";
                    reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        pesoFijado += Convert.ToInt64(reader["PesoNeto"].ToString().Trim());
                    }
                    reader.Close();
                }
                //Liquidado
                command.CommandText = "SELECT kgs as PesoNeto from cttoliq where ctto='" + contrato.Id.Trim() + "'";
                reader = command.ExecuteReader();
                Int64 pesoLiquidado = 0;
                while (reader.Read())
                {
                    pesoLiquidado += Convert.ToInt64(reader["PesoNeto"].ToString().Trim());
                }
                reader.Close();

                EstadoContratoView item = new EstadoContratoView();
                item.Fecha = contrato.Fecha;
                item.NombreComprador = contrato.NombreComprador;
                item.NombreCosecha = contrato.Cosecha.Nombre;
                item.NombreCereal = contrato.NombreCereal;
                item.Numero = contrato.Numero;
                item.PesoNeto = contrato.PesoNeto;
                item.PesoAplicado = pesoAplicado;
                item.PesoPendienteAplicar = item.PesoNeto - pesoAplicado;
                item.PesoFijado = pesoFijado;
                item.PesoPendienteFijar = contrato.PesoNeto - pesoFijado;
                item.PesoLiquidado = pesoLiquidado;
                item.PesoPendienteLiquidar = contrato.PesoNeto - pesoLiquidado;
                item.Estado = contrato.Estado;
                item.Tipo = contrato.Tipo;
                
                result.Add(item);
                //Mercaderia en transito

            }
            return result;
        }
        //Entregado pendiente de Asignar
        private Contrato Parse(OleDbDataReader reader)
        {
            Contrato item = new Contrato();
            item.Id = reader["ctocod"].ToString().Trim();
            item.Numero = reader["ctonro"].ToString().Trim();
            item.IdCosecha = reader["ctocose"].ToString();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["ctocose"].ToString();
            cosecha.Nombre = reader["nombrecosecha"].ToString();
            cosecha.IdCereal = reader["cereal"].ToString();
            bool aFijar = (bool)reader["fijar"];
            string tipo = aFijar ? "A FIJAR" : "NORMAL";
            bool finalizado = (bool)reader["ctofin"];
            item.Estado = finalizado ? "FINALIZADO" : "PENDIENTE";
            item.Tipo = tipo;
            item.Cosecha = cosecha;
            item.NombreCereal = reader["NombreCereal"].ToString();
            item.NombreComprador = reader["NombreComprador"].ToString();
            item.Fecha = (DateTime)reader["ctofope"];
            item.PesoNeto = Convert.ToInt64(reader["ctokgs"].ToString().Trim());
            return item;
        }
        private EstadoContratoView ParseEstadoContratoView(OleDbDataReader reader)
        {
            EstadoContratoView item = new EstadoContratoView();
            
            return item;
        }
    }
    public class CartaPorte
    {
        public string Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaDescarga {get;set;}

    }
}
