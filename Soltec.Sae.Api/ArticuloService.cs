using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class ArticuloService
    {
        public ArticuloService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Articulo> List() 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom,ccbar,pco,VAL(str(pove,10,3)) as pove,imi,pve1,aiva,pfin FROM artgen";
            OleDbDataReader reader = command.ExecuteReader();
            List<Articulo> result = new List<Articulo>();
            while (reader.Read())
            {
                result.Add(new Articulo
                {
                    Id = reader["cod"].ToString().Trim(),
                    Nombre = reader["nom"].ToString().Trim(),
                    PrecioCosto = (decimal)reader["pco"],
                    MargenVenta = (decimal)reader["pove"],
                    ImpuestoInterno = (decimal)reader["imi"],
                    PrecioVenta = (decimal)reader["pve1"],
                    PrecioVentaFinal = (decimal)reader["pfin"],
                    AlicuotaIva = (decimal)reader["aiva"]
                });
            }            
            cnn.Close();
            return result;
        }
        public Articulo FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom,ccbar,pco,VAL(str(pove,10,3)) as pove,imi,pve1,aiva,pfin FROM artgen WHERE cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Articulo result = null;
            while (reader.Read())
            {
                result = new Articulo
                {
                    Id = reader["cod"].ToString().Trim(),
                    Nombre = reader["nom"].ToString().Trim(),
                    PrecioCosto = (decimal)reader["pco"],
                    MargenVenta = (decimal)reader["pove"],
                    ImpuestoInterno = (decimal)reader["imi"],
                    PrecioVenta = (decimal)reader["pve1"],
                    PrecioVentaFinal = (decimal)reader["pfin"],
                    AlicuotaIva = (decimal)reader["aiva"]
                };
            }
            cnn.Close();
            return result;
        }
    }
}
