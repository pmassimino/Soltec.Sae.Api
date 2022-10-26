using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class SujetoService
    {
        public SujetoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Sujeto> List() 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom,dir,alt,loc,pos,provin,email,cuit,piva,ibru FROM clipro";
            OleDbDataReader reader = command.ExecuteReader();
            List<Sujeto> result = new List<Sujeto>();
            while (reader.Read())
            {
                result.Add(new Sujeto
                {
                    Id = reader["cod"].ToString().Trim(),
                    Nombre = reader["nom"].ToString().Trim(),
                    Localidad = reader["loc"].ToString().Trim(),
                    Provincia = reader["provin"].ToString().Trim(),
                    CodigoPostal = reader["pos"].ToString().Trim(),
                    NumeroDocumento = reader["cuit"].ToString().Trim(),
                    NumeroIngBruto = reader["ibru"].ToString().Trim(),
                    Domicilio = reader["dir"].ToString().Trim(),
                    CondicionIva = reader["piva"].ToString().Trim(),
                    CondicionIB = reader["ibru"].ToString().Trim()
                });
            }            
            cnn.Close();
            return result;
        }
        public Sujeto FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom,dir,alt,loc,pos,provin,email,cuit,piva,ibru FROM clipro where cod = '" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();          
            Sujeto result = null;
            while (reader.Read())
            {
                result = new Sujeto
                {
                    Id = reader["cod"].ToString().Trim(),
                    Nombre = reader["nom"].ToString().Trim(),
                    Localidad = reader["loc"].ToString().Trim(),
                    Provincia = reader["provin"].ToString().Trim(),
                    CodigoPostal = reader["pos"].ToString().Trim(),
                    NumeroDocumento = reader["cuit"].ToString().Trim(),
                    NumeroIngBruto = reader["ibru"].ToString().Trim(),
                    Domicilio = reader["dir"].ToString().Trim(),
                    CondicionIva = reader["piva"].ToString().Trim(),
                    CondicionIB = reader["ibru"].ToString().Trim()
                };
                OleDbCommand commandSujeto = cnn.CreateCommand();
                commandSujeto.CommandText = "SELECT sub,mg,nom FROM resubmg where sub='" + id + "'";
                OleDbDataReader readerSujeto = commandSujeto.ExecuteReader();
                while (readerSujeto.Read())
                {
                    Subdiario item = new Subdiario
                    {
                        Id = readerSujeto["mg"].ToString().Trim(),
                        Nombre = readerSujeto["nom"].ToString().Trim(),
                        IdDivisa = 0
                    };
                    result.Subdiarios.Add(item);
                }
            }
            cnn.Close();
            return result;
        }
    }
}
