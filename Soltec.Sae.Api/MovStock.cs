using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class MovStockService
    {         
        public MovStockService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

        public void add(MovStock entity) 
        {
            Random random = new Random();
            int ntra = random.Next(10000, 99999);

            string sql = "INSERT INTO ALMO (suc,usu,tra,femi,art,depo,depd,can,fcom,pe,ncom,con,obn) " +
                "VALUES ('01','001'," + ntra.ToString() + ",ctod('" + entity.Fecha.Date.ToString("MM-dd-yyyy") + "'),'"
                + entity.IdArticulo.Trim() + "','" + entity.IdDeposito.Trim() + "','" + entity.IdDepositoDestino.Trim() + "',"
                + entity.Cantidad.ToString() + ",CTOD('" + entity.Fecha.Date.ToString("MM-dd-yyyy") + "')," + entity.Pe.ToString() + "," + entity.Numero + ",'" + entity.Concepto.Trim() + "','API')";
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SET NULL OFF" ;//Anular valores nulos
            command.ExecuteNonQuery();
            //command.CommandText = "SET DATE TO DMY";//Anular valores nulos
            //command.ExecuteNonQuery();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
        
        public List<Stock> ListStock(DateTime fecha,string idArticulo,string idArticuloHasta,string idSeccion)
        {
            ArticuloService service = new ArticuloService(this.ConnectionStringBase);
            var tmpArticulos = service.List().Where(w => string.Compare(w.Id, idArticulo, StringComparison.Ordinal) >= 0 && string.Compare(w.Id, idArticuloHasta, StringComparison.Ordinal) <= 0);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();            
            command.CommandText = "SELECT tra,femi,art,depo,depd,imp,can,artgen.nom as NombreArticulo " + 
                "FROM almo "  +                 
                "LEFT JOIN Artgen on artgen.cod = almo.art " + 
                "WHERE  (femi <= ctod('" + fecha.ToString("MM-dd-yyy") + "')) " +                
                "order by femi,art";
            OleDbDataReader reader = command.ExecuteReader();
            List<MovStock> tmpMovStock = new List<MovStock>();
            while (reader.Read())
            {
                tmpMovStock.Add(this.Parse(reader));
            }
            List<Stock> result = new List<Stock>();
            foreach (var item in tmpArticulos) 
            {
                Stock newItem = new Stock();
                decimal cant = tmpMovStock.Where(w=>w.IdArticulo==item.Id).Sum(x => x.Cantidad);
                newItem.Cantidad = cant;
                newItem.IdArticulo = idArticulo;
                newItem.NombreArticulo = item.Nombre;
                result.Add(newItem);
            }            
            return result;            
        }
       

        private MovStock Parse(OleDbDataReader reader)
        {
            MovStock item = new MovStock();
            item.Id = reader["tra"].ToString().Trim(); ;
            item.Fecha = (DateTime)reader["femi"];
            item.IdArticulo = reader["art"].ToString().Trim();
            item.NombreArticulo = reader["NombreArticulo"].ToString().Trim();
            item.IdDeposito = reader["depo"].ToString().Trim();
            item.IdDepositoDestino = reader["depd"].ToString().Trim();
            item.Cantidad = (decimal)reader["can"];            
            return item;
        }
        
      
    }
}
