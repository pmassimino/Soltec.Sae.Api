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
        // Lee una columna del reader como decimal; si la columna no existe, es nula, o el driver
        // tira una excepción al leerla/convertirla (pasa con algunos registros corruptos en la BD), devuelve 0.
        private static decimal ParseDecimal(OleDbDataReader reader, string column)
        {
            try
            {
                return Convert.ToDecimal(reader[column]);
            }
            catch
            {
                return 0;
            }
        }

        // Lee una columna del reader como int, protegida igual que ParseDecimal.
        private static int ParseInt(OleDbDataReader reader, string column)
        {
            try
            {
                return Convert.ToInt32(reader[column]);
            }
            catch
            {
                return 0;
            }
        }

        private Articulo Parse(OleDbDataReader reader)
        {
            Articulo item = new Articulo();
            item.Id = reader["cod"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            item.PrecioCosto = ParseDecimal(reader, "pco");
            item.MargenVenta = ParseDecimal(reader, "pove");
            item.ImpuestoInterno = ParseDecimal(reader, "imi");
            item.PrecioVenta = ParseDecimal(reader, "pve1");
            item.PrecioVentaFinal = ParseDecimal(reader, "pfin");
            item.AlicuotaIva = ParseDecimal(reader, "aiva");
            item.IdFamilia = reader["agru"].ToString().Trim();
            item.IdSeccionOp = reader["sect"].ToString().Trim();
            item.IdDivisa = ParseInt(reader, "div");
            item.Stock = ParseDecimal(reader, "sact");
            item.PendRemitir = ParseDecimal(reader, "spen");

            var listaPrecio = new List<PrecioArticulo>
            {
                new PrecioArticulo { Tipo = "Publico", Valor = item.PrecioVentaFinal }
            };

            var precioVenta2 = ParseDecimal(reader, "PVE2F");
            var precioVenta3 = ParseDecimal(reader, "PVE3F");

            if (precioVenta2 > 0)
            {
                listaPrecio.Add(new PrecioArticulo { Tipo = "Especial", Valor = precioVenta2 });
            }

            if (precioVenta3 > 0)
            {
                listaPrecio.Add(new PrecioArticulo { Tipo = "Mayorista", Valor = precioVenta3 });
            }
            item.Precios = listaPrecio;

            return item;
        }
    }
}
