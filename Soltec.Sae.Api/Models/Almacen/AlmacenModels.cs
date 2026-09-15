namespace Soltec.Sae.Api
{
    public class Articulo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdFamilia { get; set; }
        public string IdLinea { get; set; }
        public string IdSeccionOp { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal MargenVenta { get; set; }
        public decimal AlicuotaIva { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioVentaFinal { get; set; }
        public List<PrecioArticulo> Precios { get; set; } = new List<PrecioArticulo>();
        public int IdDivisa { get; set; }
        public decimal Stock { get; set; }
        public decimal PendRemitir { get; set; }

    }
    public class PrecioArticulo
    {
        public string Tipo { get; set; } // 1: Minorista, 2: Mayorista, etc.
        public decimal Valor { get; set; }
    }

    public class ArticuloFilterOptions
    {
        public bool FiltrarActivos { get; set; }
    }

    public class SeccionOperativa : EntityGeneric
    {
    }
    public class Familia
    {
        public string Id { get; set; }
        public string Nombre { get; set; }

    }
    public class Linea : EntityGeneric
    {

    }
    public class MovStock
    {
        public string Id { get; set; }
        public string IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string Concepto { get; set; }
        public DateTime Fecha { get; set; }
        public string IdDeposito { get; set; }
        public string IdDepositoDestino { get; set; }
        public double Cantidad { get; set; }
        public int Pe { get; set; }
        public int Numero { get; set; }
        public string Lote { get; set; }
        public string Serie { get; set; }

    }
    public class Stock
    {
        public string IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public double Cantidad { get; set; }

    }
    public class StockDeposito
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }

    }
}
