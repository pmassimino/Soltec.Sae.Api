using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class LocalidadService
    {
        public LocalidadService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Localidad> List() 
        {
            string connectionString = this.ConnectionStringBase + "locali.dbf";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id_locali,cdlocalida,ncprovinci from locali";
            OleDbDataReader reader = command.ExecuteReader();
            List<Localidad> result = new List<Localidad>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        
        
        private Localidad Parse(OleDbDataReader reader)
        {
            Localidad item = new Localidad();
            item.Id = reader["id_locali"].ToString().Trim();
            item.Nombre = reader["cdlocalida"].ToString(); 
            item.IdProvincia= reader["ncprovinci"].ToString().PadLeft(2,'0');
            return item;
        }
    }
}
