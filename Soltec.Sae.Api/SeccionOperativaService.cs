using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class SeccionOperativaService
    {
        public SeccionOperativaService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<SeccionOperativa> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM division where !empty(cod) and !empty(nom)";
            OleDbDataReader reader = command.ExecuteReader();
            List<SeccionOperativa> result = new List<SeccionOperativa>();            
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public SeccionOperativa FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM division where !empty(cod) and !empty(nom) and cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            SeccionOperativa result = new SeccionOperativa();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }

        private SeccionOperativa Parse(OleDbDataReader reader)
        {
            SeccionOperativa item = new SeccionOperativa();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            return item;
        }
    }
}
