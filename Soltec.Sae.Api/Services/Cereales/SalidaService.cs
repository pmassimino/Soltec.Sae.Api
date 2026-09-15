using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class SalidaService
    {
        public SalidaService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
       
        public List<Salida> List(string idCosecha , string idPlanta ,DateTime fecha,DateTime fechaHasta) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();

            //new OleDbCommand("set enginebehavior 80", oleDbCon).ExecuteNonQuery();
            command.CommandText = "SET TABLEVALIDATE TO 0";
            command.ExecuteNonQuery();
            command.CommandText = "SELECT  suc,sec,sa_nro,sa_fecha,sa_produ,sa_cerea,sa_tipo,sa_cosec,sa_proce,sa_comp,sa_pes_bru,sa_tara,sa_pes_net," +
                         "sa_c_res,sa_n_cre,planta,venta_dir,venta_planta,sust,noincstock,ntra," +
                         "Cosechas.descri as NombreCosecha ," +
                         "destina.des_den as NombreDestino,destina.des_doc as CuitDestino " +
                         "FROM salida " +
                         "LEFT JOIN Destina on destina.des_cod = salida.sa_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = salida.sa_cosec " +
                         "WHERE (sa_cosec = '" + idCosecha + "' OR empty('" + idCosecha + "'))  AND(planta ='" + idPlanta + "') " + " OR empty('" + idPlanta + "')" +
                         "and (sa_fecha BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')" + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and " +
                         "(noincstock = .F.)";            
            OleDbDataReader reader = command.ExecuteReader(); 
            List<Salida> result = new List<Salida>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        public Salida FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  suc,sec,sa_nro,sa_fecha,sa_produ,sa_cerea,sa_tipo,sa_cosec,sa_proce,sa_comp,sa_pes_bru,sa_tara,sa_pes_net," +
                          "sa_c_res,sa_n_cre,planta,venta_dir,venta_planta,sust,noincstock,ntra," +
                          "Cosechas.descri as NombreCosecha ," +
                          "destina.des_den as NombreDestino,destina.des_doc as CuitDestino " +
                          "FROM salida " +
                          "LEFT JOIN Destina on destina.des_cod = salida.sa_produ " +
                          "LEFT JOIN Cosechas on cosechas.cod = salida.sa_cosec " +
                          "WHERE (sa_nro = '" + id + "')";
            OleDbDataReader reader = command.ExecuteReader();
            Salida result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        public Int64 TotalPlanta(string idPlanta, string idCosecha, DateTime fecha,DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(sa_pes_net) as Total " +
                                  "FROM Salida " +
                                  "WHERE (planta = '" + idPlanta + "') and " +
                                  " (sa_cosec = '" + idCosecha + "') and (sa_fecha BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                                  + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "'))  and " +
                                    "(noincstock = .F.)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }


        private Salida Parse(OleDbDataReader reader)
        {
            Salida item = new Salida();
            item.Id = reader["sa_nro"].ToString().Trim();
            item.IdSucursal = this.IdSucursal;
            item.IdTransaccion = "SALIDA;" + this.IdSucursal + ";" + item.Id; 
            item.Fecha = (DateTime)reader["sa_fecha"];
            item.IdCosecha = reader["sa_cosec"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["sa_cosec"].ToString().Trim();
            cosecha.Nombre = reader["NombreCosecha"].ToString().Trim();
            item.Cosecha = cosecha;
            item.IdCuenta = reader["sa_produ"].ToString().Trim();
            item.IdPlanta = reader["planta"].ToString().Trim();
            Sujeto cuenta = new Sujeto();
            cuenta.Id = reader["sa_produ"].ToString().Trim();
            cuenta.Nombre = reader["NombreDestino"].ToString().Trim();
            cuenta.NumeroDocumento = reader["CuitDestino"].ToString().Trim();
            item.Cuenta = cuenta;            
            item.PesoNeto = Convert.ToInt64(reader["sa_pes_net"].ToString().Trim());            
            item.Numero = reader["sa_comp"].ToString().Trim();
            item.NumeroCPorte = reader["sa_n_cre"].ToString().Trim();
            return item;
        }
    }
}
