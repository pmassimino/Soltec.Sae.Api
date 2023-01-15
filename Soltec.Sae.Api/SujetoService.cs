using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class SujetoService
    {
        public SujetoService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public List<Sujeto> List()
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT clipro.cod,clipro.nom,dir,alt,loc,pos,provin,email,cuit,piva,ibru,pibru,coniva.nom as CondicionIva,"
              + "conibru.nom as CondicionIB,provin.nom as NombreProvincia " + 
                "FROM clipro " + 
                "Left Join ConIva on piva = conIva.cod " +
                "Left Join ConIbru on piva = conIBru.cod " +
                "Left Join provin on pro = provin.cod ";
            OleDbDataReader reader = command.ExecuteReader();
            List<Sujeto> result = new List<Sujeto>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }
        public Sujeto FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT clipro.cod,clipro.nom,dir,alt,loc,pos,provin,email,cuit,piva,ibru,pibru,coniva.nom as CondicionIva,"
              + "conibru.nom as CondicionIB,provin.nom as NombreProvincia " +
                "FROM clipro " +
                "Left Join ConIva on piva = conIva.cod " +
                "Left Join ConIbru on piva = conIBru.cod " +
                "Left Join provin on pro = provin.cod " + 
                "where clipro.cod = '" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Sujeto result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
                OleDbCommand commandSujeto = cnn.CreateCommand();
                commandSujeto.CommandText = "SELECT sub,mg,nom FROM resubmg where sub='" + id + "'";
                OleDbDataReader readerSujeto = commandSujeto.ExecuteReader();
                while (readerSujeto.Read())
                {
                    Subdiario item = new Subdiario
                    {
                        Id = readerSujeto["mg"].ToString().Trim(),
                        Nombre = readerSujeto["nom"].ToString().Trim(),
                        IdDivisa = 0
                    };
                    result.Subdiarios.Add(item);
                }
            }
            cnn.Close();
            return result;
        }
        private Sujeto Parse(OleDbDataReader reader)
        {
            var result = new Sujeto();
            result.Id = reader["cod"].ToString().Trim();
            result.Nombre = reader["nom"].ToString().Trim();
            result.Localidad = reader["loc"].ToString().Trim();
            result.Provincia = reader["NombreProvincia"].ToString().Trim();
            result.CodigoPostal = reader["pos"].ToString().Trim();
            result.NumeroDocumento = reader["cuit"].ToString().Trim();
            result.NumeroIngBruto = reader["ibru"].ToString().Trim();
            result.Domicilio = reader["dir"].ToString().Trim();
            result.CondicionIva = reader["CondicionIva"].ToString().Trim();
            result.CondicionIB = reader["CondicionIB"].ToString().Trim();
            if (string.IsNullOrEmpty(result.NumeroIngBruto)) result.NumeroIngBruto = "0";
            if (string.IsNullOrEmpty(result.NumeroDocumento)) result.NumeroDocumento = "0";
            if (string.IsNullOrEmpty(result.CondicionIva)) result.CondicionIva = "0";
            if (string.IsNullOrEmpty(result.CondicionIB)) result.CondicionIB = "0";
            return result;
        }
    }
}
