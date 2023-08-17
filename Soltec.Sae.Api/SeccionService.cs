using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class SeccionService
    {
        public SeccionService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Seccion> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM sec where !empty(cod) and !empty(nom)";
            OleDbDataReader reader = command.ExecuteReader();
            List<Seccion> result = new List<Seccion>();            
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public Seccion FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM sec where !empty(cod) and !empty(nom) and cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Seccion result = new Seccion();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }

        private Seccion Parse(OleDbDataReader reader)
        {
            Seccion item = new Seccion();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            return item;
        }
    }
}
