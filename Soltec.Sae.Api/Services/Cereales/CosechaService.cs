using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CosechaService
    {
        public CosechaService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Cosecha> List() 
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cosechas.cod,cosechas.descri,cosechas.cereal,cermae.descri as NombreCereal FROM cosechas LEFT JOIN cermae ON cermae.cod_cer = cosechas.cereal";
            OleDbDataReader reader = command.ExecuteReader();
            List<Cosecha> result = new List<Cosecha>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        public Cosecha FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cosechas.cod,cosechas.descri,cosechas.cereal,cermae.descri as NombreCereal FROM cosechas LEFT JOIN cermae ON cermae.cod_cer = cosechas.cereal " +
                                  "WHERE COD = '" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Cosecha result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        
        private Cosecha Parse(OleDbDataReader reader)
        {
            Cosecha item = new Cosecha();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["descri"].ToString();
            item.IdCereal = reader["cereal"].ToString();
            item.NombreCereal = reader["NombreCereal"].ToString();
            return item;
        }
    }
}
