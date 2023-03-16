using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CategoriaService
    {
        public CategoriaService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<EntityGeneric> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod, nom " +
                "FROM catesub ";
               
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
            result.Id = reader["cod"].ToString().Trim();
            result.Nombre = reader["nom"].ToString().Trim();
            return result;
        }
    }
}
