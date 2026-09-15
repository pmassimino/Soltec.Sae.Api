namespace Soltec.Sae.Api
{
    public class Seccion
    {
        public string Id { get; set; }
        public string Nombre { get; set; }

    }
    public class Factura
    {
        public Factura()
        {
            this.Detalle = new List<DetalleFactura>();
        }
        public string Sec { get; set; }
        public string SeccionFacturacion { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Tipo { get; set; }
        public string Letra { get; set; }
        public decimal Pe { get; set; }
        public decimal Numero { get; set; }
        public string Comprobante { get; set; }
        public int IdDivisa { get; set; }
        public string Divisa { get; set; }
        public decimal Cotizacion { get; set; }
        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string Obs { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IvaGeneral { get; set; }
        public decimal IvaOtro { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal PrecepcionIva { get; set; }
        public decimal PrecepcionIB { get; set; }
        public decimal Total { get; set; }
        public Int64 Cae { get; set; }
        public string Remito { get; set; }
        public string CondVenta { get; set; }
        public string TipoComp { get; set; }
        public List<DetalleFactura> Detalle { get; set; }
        public string IdClaseVenta { get; set; }
        public string IdCampania { get; set; }
        public string NTra { get; set; }
    }
    public class DetalleFactura
    {
        public string IdArticulo { get; set; }
        public string Concepto { get; set; }
        public decimal Precio { get; set; }
        public decimal AlicuotaIva { get; set; }
        public decimal Iva { get; set; }
        public decimal ImpInterno { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Bonificacion { get; set; }
        public string IdRemito { get; set; }

    }
    public class FacturaView
    {
        public string Sec { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Tipo { get; set; }
        public string Letra { get; set; }
        public decimal Pe { get; set; }
        public decimal Numero { get; set; }
        public string Comprobante { get; set; }
        public int IdDivisa { get; set; }
        public decimal Cotizacion { get; set; }
        public string IdCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string Obs { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IvaGeneral { get; set; }
        public decimal IvaOtro { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal PercepcionIva { get; set; }
        public decimal PercepcionIB { get; set; }
        public decimal Total { get; set; }
        public Int64 Cae { get; set; }
        public string Remito { get; set; }
        public string CondVenta { get; set; }
        public string TipoComp { get; set; }
        public string IdClaseVenta { get; set; }
        public string IdCampania { get; set; }
        public int Item { get; set; }
        public string IdArticulo { get; set; }
        public string Concepto { get; set; }
        public decimal Precio { get; set; }
        public decimal AlicuotaIva { get; set; }
        public decimal Iva { get; set; }
        public decimal ImpuestoInternoItem { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal SubTotalItem { get; set; }
        public decimal Bonificacion { get; set; }
        public string IdRemito { get; set; }

    }
    public class DocumentoPendienteView
    {
        public string Sec { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Tipo { get; set; }
        public string Comprobante { get; set; }
        public string Letra { get; set; }
        public decimal Pe { get; set; }
        public decimal Numero { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadPendiente { get; set; }

    }
    public class Remito
    {
        public Remito()
        {
            this.Detalle = new List<DetalleRemito>();
        }
        public string Sec { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Tipo { get; set; }
        public string Letra { get; set; }
        public decimal Pe { get; set; }
        public decimal Numero { get; set; }
        public string Comprobante { get; set; }
        public int IdDivisa { get; set; }
        public decimal Cotizacion { get; set; }
        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string Obs { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IvaGeneral { get; set; }
        public decimal IvaOtro { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal PrecepcionIva { get; set; }
        public decimal Total { get; set; }
        public Int64 Cae { get; set; }
        public string CondVenta { get; set; }
        public string TipoComp { get; set; }
        public List<DetalleRemito> Detalle { get; set; }

    }
    public class DetalleRemito
    {
        public string IdArticulo { get; set; }
        public string Concepto { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadPendiente { get; set; }
        public string Estado { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public string IdRemito { get; set; }

    }
    public class Pedido
    {
        public Pedido()
        {
            this.Detalle = new List<DetallePedido>();
        }
        public string Sec { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Tipo { get; set; }
        public string Letra { get; set; }
        public decimal Pe { get; set; }
        public decimal Numero { get; set; }
        public string Comprobante { get; set; }
        public int IdDivisa { get; set; }
        public decimal Cotizacion { get; set; }
        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string Obs { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IvaGeneral { get; set; }
        public decimal IvaOtro { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal PrecepcionIva { get; set; }
        public decimal Total { get; set; }
        public Int64 Cae { get; set; }
        public string CondVenta { get; set; }
        public string TipoComp { get; set; }
        public List<DetallePedido> Detalle { get; set; }

    }
    public class DetallePedido
    {
        public string IdArticulo { get; set; }
        public string Concepto { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadPendiente { get; set; }
        public string Estado { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public string IdRemito { get; set; }

    }
}
