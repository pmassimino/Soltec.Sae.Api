using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CampaniaService
    {
        public CampaniaService(string connectionStringBase)
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
            command.CommandText = "SELECT cod,nom FROM periasig where !empty(cod) and !empty(nom)";
            OleDbDataReader reader = command.ExecuteReader();
            List<EntityGeneric> result = new List<EntityGeneric>();            
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public EntityGeneric FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom FROM periasig where !empty(cod) and !empty(nom) and cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            EntityGeneric result = new EntityGeneric();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }

        private EntityGeneric Parse(OleDbDataReader reader)
        {
            EntityGeneric item = new EntityGeneric();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            return item;
        }
    }
}
