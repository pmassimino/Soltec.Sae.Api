using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class PlantaService
    {
        public PlantaService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Planta> List() 
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id_planta,descripcion from plantaoncca";
            OleDbDataReader reader = command.ExecuteReader();
            List<Planta> result = new List<Planta>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        
        
        private Planta Parse(OleDbDataReader reader)
        {
            Planta item = new Planta();
            item.Id = reader["id_planta"].ToString().Trim();
            item.Nombre = reader["descripcion"].ToString();            
            return item;
        }
    }
}
