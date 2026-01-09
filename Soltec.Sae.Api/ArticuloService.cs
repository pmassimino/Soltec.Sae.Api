using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class ArticuloService
    {
        public ArticuloService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<Articulo> List() 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom,ccbar,pco,VAL(str(pove,10,3)) as pove,imi,pve1,aiva,pfin,agru,sect,linea,sact,spen,div FROM artgen where !empty(cod) and !empty(nom)";
            OleDbDataReader reader = command.ExecuteReader();
            List<Articulo> result = new List<Articulo>();
            while (reader.Read())
            {                
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        public Articulo FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT cod,nom,ccbar,pco,VAL(str(pove,10,3)) as pove,imi,pve1,aiva,pfin,agru,sect,sact,spen,div FROM artgen WHERE cod ='" + id + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Articulo result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        private Articulo Parse(OleDbDataReader reader)
        {
            Articulo item = new Articulo();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            try
            {
             item.PrecioCosto = (decimal)reader["pco"];
            }
            catch (Exception ex)
            { }
            item.MargenVenta = (decimal)reader["pove"];
            item.ImpuestoInterno = (decimal)reader["imi"];
            try
            {
                item.PrecioVenta = (decimal)reader["pve1"];
            } catch (Exception ex) { }
            try
            {
                item.PrecioVentaFinal = (decimal)reader["pfin"];
            }catch (Exception ex) { }

            item.AlicuotaIva = (decimal)reader["aiva"];
            item.IdFamilia = reader["agru"].ToString().Trim();
            item.IdSeccionOp = reader["sect"].ToString().Trim();
            int.TryParse(reader["div"]?.ToString(), out int idDivisa);
            item.IdDivisa = idDivisa;
            item.Stock = Convert.ToDecimal(reader["sact"]);
            try 
            {
                item.PendRemitir = Convert.ToDecimal(reader["spen"]);
            }catch (Exception ex) 
            {
            }
            
            return item;
        }
    }
}
