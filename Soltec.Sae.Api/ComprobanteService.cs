using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class ComprobanteService
    {
        public ComprobanteService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Comprobante> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod, nom, detalle, rubro, prop, cotip, pe, numera, report, fa, tipo, letra, genc, canp, usuario, diahora FROM comprob";
            OleDbDataReader reader = command.ExecuteReader();
            List<Comprobante> result = new List<Comprobante>();
            while (reader.Read())
            {
                result.Add(new Comprobante
                {
                    Id = reader["cod"].ToString().Trim(),
                    Nombre = reader["nom"].ToString().Trim(),
                    IdTipo = reader["genc"].ToString().Trim(),
                    Report = reader["report"].ToString().Trim()
                });
            }
            cnn.Close();
            return result;
        }
        public Comprobante FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod, nom, detalle, rubro, prop, cotip, pe, numera, report, fa, tipo, letra, genc, canp, usuario, diahora FROM comprob where cod = '" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Comprobante result = null;
            while (reader.Read())
            {
                result = new Comprobante
                {
                    Id = reader["cod"].ToString().Trim(),
                    Nombre = reader["nom"].ToString().Trim(),
                    IdTipo = reader["genc"].ToString().Trim(),
                    Report = reader["report"].ToString().Trim()
                };
            }
            cnn.Close();
            return result;
        }
     }
}

