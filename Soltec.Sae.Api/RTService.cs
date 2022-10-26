using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class RTService
    {
        public RTService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
       
        public List<Rt> List(string idCuenta , string idCuentaDestino, string idCosecha,DateTime fecha) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id_rt,fecha_venc,fecha_emi,n1116rt,n1116a,id_deposi,id_tipo_rt,id_receptor,id_cosecha,kilosnetos,origen,coe,pe,numorden," + "" +
                         "estado,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor " + 
                         "FROM rettransf " +
                         "LEFT JOIN Produmae on produmae.codigo = rettransf.id_deposi " +
                         "LEFT JOIN Cosechas on cosechas.cod = rettransf.id_cosecha " +
                         "WHERE (id_deposi = '" + idCuenta + "' OR empty('" + idCuenta + "')) AND (id_receptor = '" + idCuentaDestino + "' OR empty('" + idCuentaDestino + "')) AND (id_cosecha = '" + idCosecha +
                         "' OR empty('" + idCosecha + "')) and id_tipo_rt = 1 and " + 
                         "fecha_emi <= ctod('" + fecha.ToString("MM-dd-yyy") + "')";
            OleDbDataReader reader = command.ExecuteReader();
            List<Rt> result = new List<Rt>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            reader.Close();
            cnn.Close();
            return result;
        }
        public Rt FindOne(string id) 
        {
            Rt result = null;
            return result;
        }
        public Int64 TotalTransferido(string idCuenta, string idCosecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(kilosnetos) as Total " +
                                  "FROM rettransf " +
                                  "WHERE (id_deposi = '" + idCuenta + "') and " +
                                  " (id_cosecha = '" + idCosecha + "' and fecha_emi <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and " +
                                  "(id_tipo_rt = 1)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }
        public Int64 TotalRecibido(string idCuenta, string idCosecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(kilosnetos) as Total " +
                                  "FROM rettransf " +
                                  "WHERE (id_receptor = '" + idCuenta + "') and " +
                                  " (id_cosecha = '" + idCosecha + "' and fecha_emi <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and " +
                                  "(id_tipo_rt = 1)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }
        public List<Rt> ListTraOri(string idCuenta = "", string idCuentaDestino = "", string idCosecha = "")
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT nro, fec, cos, pro, kgs, usuario, diahora, n_comp, transfe, obse, id_clasif " + 
                         "FROM traori " +                         
                         "WHERE (pro = '" + idCuenta + "' OR empty('" + idCuenta + "')) AND (cos = '" + idCosecha +
                         "' OR empty('" + idCosecha + "'))";
            OleDbDataReader reader = command.ExecuteReader();
            List<Rt> result = new List<Rt>();
            while (reader.Read())
            {
                result.Add(this.ParseTraori(reader));
            }
            reader.Close();
            //Trades
            command.CommandText = "SELECT nro, fec, cos, pro, kgs, usuario, diahora, transfe " +
                         "FROM trades " +
                         "WHERE (pro = '" + idCuentaDestino + "' OR empty('" + idCuentaDestino + "')) AND (cos = '" + idCosecha +
                         "' OR empty('" + idCosecha + "'))";
            reader = command.ExecuteReader();            
            while (reader.Read())
            {
                result.Add(this.ParseTrades(reader));
            }
            cnn.Close();
            return result;
        }


        private Rt Parse(OleDbDataReader reader)
        {
            Rt item = new Rt();
            item.Id = reader["id_rt"].ToString().Trim();
            item.IdSucursal = this.IdSucursal;
            item.IdTransaccion = "RT;" + this.IdSucursal + ";" + item.Id; 
            item.Fecha = (DateTime)reader["fecha_emi"];
            item.IdCosecha = reader["id_cosecha"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["id_cosecha"].ToString().Trim();
            cosecha.Nombre = reader["NombreCosecha"].ToString().Trim();
            item.Cosecha = cosecha;
            item.IdCuenta = reader["id_deposi"].ToString().Trim();
            Sujeto cuenta = new Sujeto();
            cuenta.Id = reader["id_deposi"].ToString().Trim();
            cuenta.Nombre = reader["NombreProductor"].ToString().Trim();
            cuenta.NumeroDocumento = reader["CuitProductor"].ToString().Trim();
            item.Cuenta = cuenta;
            item.IdCuentaDestino = reader["id_receptor"].ToString().Trim();
            item.PesoNeto = Convert.ToInt64(reader["kilosnetos"].ToString().Trim());            
            item.Numero = reader["n1116rt"].ToString().Trim();            
            return item;
        }
        private Rt ParseTraori(OleDbDataReader reader)
        {
            Rt item = new Rt();
            item.Id = reader["nro"].ToString().Trim();
            item.IdSucursal = this.IdSucursal;            
            item.Fecha = (DateTime)reader["fec"];
            item.IdCosecha = reader["cos"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["cos"].ToString().Trim();            
            item.IdCuentaDestino = reader["pro"].ToString().Trim();                     
            item.PesoNeto = Convert.ToInt64(reader["kgs"].ToString().Trim());
            item.Numero = reader["n_comp"].ToString().Trim();
            return item;
        }
        private Rt ParseTrades(OleDbDataReader reader)
        {
            Rt item = new Rt();
            item.Id = reader["nro"].ToString().Trim();
            item.IdSucursal = this.IdSucursal;
            item.Fecha = (DateTime)reader["fec"];
            item.IdCosecha = reader["cos"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["cos"].ToString().Trim();
            item.IdCuenta = reader["pro"].ToString().Trim();
            item.PesoNeto = Convert.ToInt64(reader["kgs"].ToString().Trim());
            item.Numero = reader["nro"].ToString().Trim();
            return item;
        }
    }
}
