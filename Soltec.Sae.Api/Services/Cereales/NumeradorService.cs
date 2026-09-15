using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class NumeradorService
    {
        public NumeradorService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Numerador> List()
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id,nombre,pe,numero FROM numeradores";
            OleDbDataReader reader = command.ExecuteReader();
            List<Numerador> result = new List<Numerador>();            
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public Numerador FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id,nombre,pe,numero FROM numeradores where id='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Numerador result = new Numerador();
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }

        public Numerador Incrementar(string id)
        {
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            Int64 nuevoNumero = 0;

            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();

            
            Numerador numerador = this.FindOne(id);
            object resultado = numerador.Numero;
            if (resultado != null)
            {
                Int64 numeroActual = Convert.ToInt64(resultado);
                nuevoNumero = numeroActual + 1;

                // Actualizar el número
                OleDbCommand updateCmd = cnn.CreateCommand();
                updateCmd.CommandText = "UPDATE numeradores SET numero = " + nuevoNumero + " WHERE id = '" + id + "'";

                int filasAfectadas = updateCmd.ExecuteNonQuery();
                if (filasAfectadas == 0)
                {
                    cnn.Close();
                    throw new Exception("No se pudo actualizar el numerador");
                }
            }
            else
            {
                cnn.Close();
                throw new Exception("No se encontró el numerador con ID: " + id);
            }

            cnn.Close();
            numerador.Numero = nuevoNumero;
            return numerador;
        }

        private Numerador Parse(OleDbDataReader reader)
        {
            Numerador item = new Numerador();
            item.Id = reader["id"].ToString().Trim();
            item.Nombre = reader["nombre"].ToString().Trim();
            item.Pe = Convert.ToInt32(reader["pe"]);
            item.Numero = Convert.ToInt64(reader["numero"]);
            return item;
        }
    }
}
