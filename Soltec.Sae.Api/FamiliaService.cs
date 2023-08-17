using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class FamiliaService
    {
        public FamiliaService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Familia> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM artrub where !empty(cod) and !empty(nom)";
            OleDbDataReader reader = command.ExecuteReader();
            List<Familia> result = new List<Familia>();            
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public Familia FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM artrub where !empty(cod) and !empty(nom) and cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Familia result = new Familia();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }

        private Familia Parse(OleDbDataReader reader)
        {
            Familia item = new Familia();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            return item;
        }
    }
}
