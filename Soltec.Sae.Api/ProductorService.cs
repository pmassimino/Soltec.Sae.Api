using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class ProductorService
    {
        public ProductorService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Sujeto> List() 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT codigo,rsocial,domicilio,cpostal,localidad,pcia,n_cuit FROM produmae";
            OleDbDataReader reader = command.ExecuteReader();
            List<Sujeto> result = new List<Sujeto>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        public List<Cosecha> CosechasDisponible(string idCuenta)         
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT en_cosec as IdCosecha  FROM entrada " + 
                                   "WHERE(en_produ ='" + idCuenta + "') " + 
                                   "GROUP BY en_cosec " + 
                                   "UNION " +
                                   "SELECT id_cosecha as IdCosecha " + 
                                   "FROM rettransf " + 
                                   "WHERE(id_receptor ='" +idCuenta + "') " + 
                                   "GROUP BY id_cosecha " + 
                                   "UNION " +
                                   "SELECT bol_cosec as IdCosecha " + 
                                   "FROM boletos " + 
                                   "WHERE(bol_produ ='" + idCuenta +"') " + 
                                   "GROUP BY bol_cosec " ;
            OleDbDataReader reader = command.ExecuteReader();
            List<Cosecha> result = new List<Cosecha>();
            while (reader.Read())
            {
                result.Add(this.ParseCosecha(reader));
            }
            cnn.Close();
            return result; 
        }
        public List<CosechaDisponible> CosechaDisponible(string idCuenta, string idCosecha) 
        {
            List<CosechaDisponible> result = new List<CosechaDisponible>();
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT en_produ as idProdu,en_cosec as idCosecha " +
                "FROM entrada " +
                "WHERE((en_produ ='" + idCuenta + "' OR EMPTY('" + idCuenta + "')) AND(en_cosec = '" + idCosecha + "' OR EMPTY('" + idCosecha + "'))) " +
                "GROUP BY en_produ,en_cosec  " +
                "UNION " +
                "SELECT id_receptor as idProdu,id_cosecha as idCosecha " +
                "FROM rettransf " +
                "WHERE((id_receptor = '" + idCuenta + "' OR EMPTY('" + idCuenta + "')) AND(id_cosecha = '" + idCosecha + "' OR EMPTY('" + idCosecha + "'))) " +
                "GROUP BY id_receptor,id_cosecha " +
                "UNION " +
                "SELECT bol_produ as idProdu,bol_cosec as idCosecha " +
                "FROM boletos " +
                "WHERE((bol_produ = '" + idCuenta + "' OR EMPTY('" + idCuenta + "')) AND(bol_cosec = '" + idCosecha + "' OR EMPTY('" + idCosecha + "'))) " +
                "GROUP BY bol_produ,bol_cosec " +
                "order by 1,2 ";
            OleDbDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                result.Add(this.ParseCosechaDisponible(reader));
            }
            cnn.Close();
            return result;
        }

        private Sujeto Parse(OleDbDataReader reader)
        {
            Sujeto item = new Sujeto();
            item.Id = reader["codigo"].ToString().Trim();
            item.Nombre = reader["rsocial"].ToString();
            item.Domicilio = reader["domicilio"].ToString();
            item.Localidad = reader["localidad"].ToString();
            item.NumeroDocumento = reader["n_cuit"].ToString();
            return item;
        }
        private Cosecha ParseCosecha(OleDbDataReader reader)
        {
            Cosecha item = new Cosecha();
            item.Id = reader["IdCosecha"].ToString().Trim();            
            return item;
        }
        private CosechaDisponible ParseCosechaDisponible(OleDbDataReader reader)
        {
            CosechaDisponible item = new CosechaDisponible();
            item.IdCosecha = reader["IdCosecha"].ToString().Trim();
            item.IdProductor = reader["IdProdu"].ToString().Trim();
            return item;
        }
    }
    public class CosechaDisponible 
    {
        public string IdProductor { get; set; }
        public string IdCosecha { get; set; }  
    }
    
   
}
