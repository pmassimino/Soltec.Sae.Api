using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class BoletoService
    {
        public BoletoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
       
        public List<Boleto> List(string idCuenta , string idCosecha ,DateTime fecha) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  bol_nro, bol_fec, bol_produ, bol_kgs, bol_pre_n, bol_pre_l, bol_obs1, bol_obs2, bol_cosec, bol_confi," +
                         "bol_fpa, bol_loc, bol_tra, bol_ftrans, bol_reg, bol_ctto, pe,numero, estado_liq,fijar,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor " +
                         "FROM boletos " +
                         "LEFT JOIN Produmae on produmae.codigo = boletos.bol_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = boletos.bol_cosec " +
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
        public Boleto FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  bol_nro, bol_fec, bol_produ, bol_kgs, bol_pre_n, bol_pre_l, bol_obs1, bol_obs2, bol_cosec, bol_confi," +
                         "bol_fpa, bol_loc, bol_tra, bol_ftrans, bol_reg, bol_ctto, pe,numero, estado_liq,fijar,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor " +
                         "FROM boletos " +
                         "LEFT JOIN Produmae on produmae.codigo = boletos.bol_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = boletos.bol_cosec " +
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
            
            return item;
        }
    }
}
