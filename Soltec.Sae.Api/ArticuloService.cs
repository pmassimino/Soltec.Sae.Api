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
       
        public List<Articulo> List(ArticuloFilterOptions filtro) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();            
            // Base de la consulta
            string sql = "SELECT artgen.cod, artgen.nom, artgen.ccbar, artgen.pco, VAL(STR(artgen.pove,10,3)) as pove, " +
                         "artgen.imi, artgen.pve1, artgen.aiva, artgen.pfin, artgen.agru, artgen.sect, artgen.linea, " +
                         "artgen.sact, artgen.spen, artgen.div, ARTEXT.PVE2F, ARTEXT.PVE3F " +
                         "FROM artgen " +
                         "LEFT JOIN ARTEXT ON ARTGEN.COD == ARTEXT.COD " +
                         "WHERE !EMPTY(artgen.cod) AND !EMPTY(artgen.nom)";

            // Lógica dinámica: Solo agrega el filtro si es true
            if (filtro.FiltrarActivos)
            {
                sql += " AND artgen.activo = .T.";
            }

            OleDbCommand command = new OleDbCommand(sql, cnn);
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
            try
            {
                var listaPrecio = new List<PrecioArticulo>();
                listaPrecio.Add(new PrecioArticulo { Tipo = "Publico", Valor = item.PrecioVentaFinal });
                // Usamos Convert.ToDecimal para evitar errores si el valor es nulo en BD
                var precioVenta2 = Convert.ToDecimal(reader["PVE2F"]);
                var precioVenta3 = Convert.ToDecimal(reader["PVE3F"]); // Corregida la columna
                
                if (precioVenta2 > 0)
                {
                    listaPrecio.Add(new PrecioArticulo { Tipo = "Especial", Valor = precioVenta2 });
                }

                if (precioVenta3 > 0)
                {
                    // Corregido: Usamos precioVenta3
                    listaPrecio.Add(new PrecioArticulo { Tipo = "Mayorista", Valor = precioVenta3 });
                }
                item.Precios = listaPrecio;
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }

            return item;
        }
    }
}
