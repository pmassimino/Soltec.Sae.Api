﻿using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class MovStockService
    {         
        public MovStockService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        
       
      
        public List<Stock> List(DateTime fecha,string idArticulo,string idArticuloHasta,string idSeccion)
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