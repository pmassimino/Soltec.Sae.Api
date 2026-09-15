using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class LineaService
    {
        public LineaService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Linea> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM linea where !empty(cod) and !empty(nom)";
            OleDbDataReader reader = command.ExecuteReader();
            List<Linea> result = new List<Linea>();            
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public Linea FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM linea where !empty(cod) and !empty(nom) and cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Linea result = new Linea();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }

        private Linea Parse(OleDbDataReader reader)
        {
            Linea item = new Linea();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            return item;
        }
    }
}
