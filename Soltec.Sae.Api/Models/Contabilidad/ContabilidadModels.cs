using System.Text.Json.Serialization;

namespace Soltec.Sae.Api
{
    public class Sujeto
    {
        public Sujeto()
        {
            this.Subdiarios = new List<Subdiario>();
        }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroIngBruto { get; set; } = "0";
        public string Domicilio { get; set; }
        public string Localidad { get; set; }
        public string IdProvincia { get; set; }
        public string Provincia { get; set; }
        public string IdCategoria { get; set; }
        public string Categoria { get; set; }
        public string IdZona { get; set; }
        public string Zona { get; set; }
        public string CodigoPostal { get; set; }
        public string CondicionIva { get; set; } = "";
        public string CondicionIB { get; set; } = "";
        public List<Subdiario> Subdiarios { get; set; }
    }
    public class Subdiario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int IdDivisa { get; set; }
    }
    public class MovCtaCte
    {
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Concepto { get; set; }
        public string IdCuenta { get; set; }
        public string NumeroComprobante { get; set; }
        public string IdTransaccion { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal ImpD { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoVencido { get; set; }
        public decimal SaldoAVencer { get; set; }
        public bool Vencido { get; set; }
        public string Tipo { get; set; }
        public bool TieneComp { get; set; }
        public decimal Cotizacion { get; set; }
        public int IdDivisa { get; set; }
        public int Orden { get; set; }

    }
    public class SaldoCtaCte
    {
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string IdCuentaMayor { get; set; }
        public int IdDivisa { get; set; }
        public decimal SaldoVencido { get; set; }
        public decimal Saldo { get; set; }
    }
    public class Comprobante
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdTipo { get; set; }
        public string Report { get; set; }
    }

    //Contabilidad
    //Mayor
    public class Mayor
    {
        public Mayor()
        {
            List<DetalleMayor> Detalle = new List<DetalleMayor>();
            this.Detalle = Detalle;
        }
        public string IdSucursal { get; set; }
        public string IdSeccion { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public string Concepto { get; set; }

        public string IdComprobante { get; set; }
        public int Pe { get; set; }
        public long Numero { get; set; }

        public string Origen { get; set; }
        public virtual IList<DetalleMayor> Detalle { get; set; }
        public DetalleMayor AddDetalle(string IdCuentaMayor, string Concepto, string IdTipo, decimal Debe, decimal Haber, DateTime FechaVenc, string IdCuenta = null)
        {
            DetalleMayor item = new DetalleMayor();
            item.Item = Detalle.Count();
            item.IdCuentaMayor = IdCuentaMayor;
            item.Concepto = Concepto;
            item.IdCuenta = IdCuenta;
            item.IdTipo = IdTipo;
            item.Debe = Debe;
            item.Haber = Haber;
            item.FechaVenc = FechaVenc;
            this.Detalle.Add(item);
            return item;
        }

    }
    public class Diario
    {
        public string IdSucursal { get; set; }
        public string IdSeccion { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Concepto { get; set; }
        public string IdComprobante { get; set; }
        public int Item { get; set; }
        public int Pe { get; set; }
        public long Numero { get; set; }
        public string Origen { get; set; }
        public string IdCuentaMayor { get; set; }
        public string NombreCuentaMayor { get; set; }
        public string IdCuenta { get; set; }
        public string NombreSujeto { get; set; }
        public string IdTipo { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Cantidad { get; set; }
    }
    public class DetalleMayor
    {
        //Estructura
        public string IdSucursal { get; set; }
        public string IdSeccion { get; set; }
        public string IdTransaccion { get; set; }

        public int Item { get; set; }
        public DateTime FechaVenc { get; set; }

        public string IdCuentaMayor { get; set; }
        public string NombreCuentaMayor { get; set; }

        public string Concepto { get; set; }

        public string IdCuenta { get; set; }
        public string NombreSujeto { get; set; }

        public string IdTipo { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Cantidad { get; set; }
        //public virtual CuentaMayor CuentaMayor { get; set; }
        //


    }
    public class ReciboCtaCte
    {
        //Estructura

        public string Sec { get; set; }
        public string Id { get; set; }

        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string IdCuentaMayor { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Pe { get; set; }
        public string Numero { get; set; }
        public string IdTipo { get; set; }
        public decimal Importe { get; set; }
        public int IdDivisa { get; set; }
        public decimal Cotizacion { get; set; }
        public string Obs { get; set; }
        //Detalles
        public virtual IList<DetalleComprobante> DetalleComprobante { get; set; }
        public virtual IList<DetalleValores> DetalleValores { get; set; }

        //inicializar
        public ReciboCtaCte()
        {
            this.DetalleComprobante = new List<DetalleComprobante>();
            this.DetalleValores = new List<DetalleValores>();
        }
    }
    public class DetalleComprobante
    {
        //Estructura
        [JsonIgnore]
        public string Pe { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        public int Item { get; set; }
        public string IdTipo { get; set; } //1 - debito 2 - crédito

        public string IdComprobante { get; set; }
        public DateTime Fecha { get; set; }
        public int PeComprobante { get; set; }
        public Int64 NumeroComprobante { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
    }
    public class DetalleValores
    {
        [JsonIgnore]
        public string Pe { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }

        public int Item { get; set; }

        public string IdTipo { get; set; } //1 - debito 2 - crédito

        public string IdCuentaMayor { get; set; }

        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string NumeroComprobante { get; set; }
        public string Concepto { get; set; }
        public decimal Importe { get; set; }

        public string Banco { get; set; }
        public string Sucursal { get; set; }

    }
    public class LibroBanco
    {
        public LibroBanco()
        {

        }
        public string IdSucursal { get; set; }
        public string IdSeccion { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaConciliado { get; set; }
        public string IdCuentaMayor { get; set; }
        public string NombreCuentaMayor { get; set; }
        public string Concepto { get; set; }
        public string IdComprobante { get; set; }
        public int Pe { get; set; }
        public long Numero { get; set; }
        public string Origen { get; set; }
        public string Tipo { get; set; }
        public decimal Importe { get; set; }
        public bool Conciliado { get; set; }

    }
    public class RetencionBase
    {
        public string Sec { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Id { get; set; }
        public int Pe { get; set; }
        public string Numero { get; set; }
        public int PeComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public string Tipo { get; set; }
        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string Impuesto { get; set; }
        public string Regimen { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal Alicuota { get; set; }
        public decimal Importe { get; set; }
        public string Obs { get; set; }
    }

    public class ResumenView
    {
        public ResumenView()
        {
            this.Cosechas = new List<CosechaView>();
            this.CtaCte = new List<CtaCteView>();
        }
        public Sujeto Sujeto { get; set; }
        public List<CtaCteView> CtaCte { get; set; }
        public List<CosechaView> Cosechas { get; set; }
        public List<DocumentoPendienteView> RemitosPendientes { get; set; }
        public List<DocumentoPendienteView> FacturasPendientes { get; set; }
    }
    public class CtaCteView
    {
        public string IdCuenta { get; set; }
        public string IdSubdiario { get; set; }
        public string Nombre { get; set; }
        public int IdDivisa { get; set; }
        public decimal SaldoVencido { get; set; }
        public decimal Saldo { get; set; }
    }
    public class CosechaView
    {
        public string IdCosecha { get; set; }
        public string Nombre { get; set; }
        public long Saldo { get; set; }
        public string IdSucursal { get; set; }

    }
}
