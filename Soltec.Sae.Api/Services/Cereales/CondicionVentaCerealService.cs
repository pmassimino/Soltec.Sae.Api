using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CondicionVentaCerealService
    {
        public CondicionVentaCerealService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<EntityGeneric> List()
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id, nombre " +
                "FROM condicionventa ";
               
            OleDbDataReader reader = command.ExecuteReader();
            List<EntityGeneric> result = new List<EntityGeneric>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
       
        private EntityGeneric Parse(OleDbDataReader reader)
        {
            var result = new EntityGeneric();
            result.Id = reader["id"].ToString().Trim();
            result.Nombre = reader["nombre"].ToString().Trim();
            return result;
        }
    }
}
