namespace Soltec.Sae.Api
{
    public class Numerador
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int Pe { get; set; }
        public Int64 Numero { get; set; }
    }
    public class Cosecha
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdCereal { get; set; }
        public string NombreCereal { get; set; }

    }
    public class MovCtaCteCereal
    {
        public string IdSucursal { get; set; } = "01";
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Concepto { get; set; }
        public string IdCuenta { get; set; }
        public string IdCosecha { get; set; }
        public string NumeroComprobante { get; set; }
        public string IdTransaccion { get; set; }
        public Int64 PesoNeto { get; set; }
        public Int64 Ingreso { get; set; }
        public Int64 Egreso { get; set; }
        public Int64 Saldo { get; set; }
        public Int64 SaldoACertificar { get; set; }
        public string Tipo { get; set; }
        public bool TieneComp { get; set; }
        public int Orden { get; set; }
    }
    public class SaldoCtaCteCereal
    {
        public string IdSucursal { get; set; } = "01";
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        public Int64 Entregado { get; set; }
        public Int64 Recibido { get; set; }
        public Int64 Transferido { get; set; }
        public Int64 Retirado { get; set; }
        public Int64 Certificado { get; set; }
        public Int64 Liquidado { get; set; }
        public Int64 Autorizado { get; set; }
        public Int64 Disponible { get; set; }
        public Int64 Saldo { get; set; }

    }
    public class Entrada
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string NumeroDocumento { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public string NumeroCartaPorte { get; set; }
        public Int64 PesoBruto { get; set; }
        public Int64 PesoTara { get; set; }
        public Int64 PesoNeto { get; set; }
        public Decimal PorHumedad { get; set; }
        public int MermaHumedad { get; set; }
        public decimal PorZaranda { get; set; }
        public int MermaZaranda { get; set; }
        public decimal PorCalidad { get; set; }
        public int MermaCalidad { get; set; }
        public decimal PorVolatil { get; set; }
        public int MermaVolatil { get; set; }
        public Int64 PesoNetoFinal { get; set; }
        public string Observacion { get; set; }
        public string PatenteC { get; set; }
        public string PatenteA { get; set; }
        public string IdTransporte { get; set; }
        public Sujeto Transporte { get; set; }
        public Sujeto Chofer { get; set; }
        public string Procedencia { get; set; }
        public decimal Distancia { get; set; }
        public string IdPlanta { get; set; }
        public string IdLocalidadProcedencia { get; set; }
        public string LocalidadProcedencia { get; set; }
        public string IdLocalidadDestino { get; set; }
        public string LocalidadDestino { get; set; }
        public bool Directo { get; set; }
        public Int64 Ctg { get; set; }
    }
    public class Planta
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
    public class Localidad
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdProvincia { get; set; }
    }
    public class ItemRomaneo
    {
        public DateTime Fecha { get; set; }
        public string NumeroCPorte { get; set; }
        public Int64 PesoBruto { get; set; }
        public int MermaZaranda { get; set; }
        public decimal MermaHumedad { get; set; }
        public decimal TarifaZaranda { get; set; }
        public decimal TarifaSecado { get; set; }
        public decimal PorHumedad { get; set; }
        public decimal ImporteSecado { get; set; }
        public decimal ImporteZaranda { get; set; }
        public string IdTransaccion { get; set; }
    }
    public class Analisis
    {
        public string Nombre { get; set; }
        public decimal Valor { get; set; }
        public decimal Bonificacion { get; set; }
        public decimal Rebaja { get; set; }
    }
    public class Certificado
    {
        public Certificado()
        {
            this.DetalleAnalisis = new List<Analisis>();
            this.DetalleRomaneo = new List<ItemRomaneo>();
        }
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Numero { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public decimal TarifaAlmacenaje { get; set; }
        public decimal TarifaAcarreo { get; set; }
        public decimal TarifaGG { get; set; }
        public decimal TarifaZarandeo { get; set; }
        public decimal SecadoDesde { get; set; }
        public decimal SecadoHasta { get; set; }
        public decimal TarifaSecado { get; set; }
        public decimal TarifaPtoExceso { get; set; }
        public decimal Otros { get; set; }
        public decimal Sellado { get; set; }
        public int NumeroMuestra { get; set; }
        public int NumeroBoletin { get; set; }
        public Int64 PesoBruto { get; set; }
        public int MermaVolatil { get; set; }
        public int MermaSecado { get; set; }
        public int MermaZaranda { get; set; }
        public Int32 MermaCalidad { get; set; }
        public Int64 PesoNeto { get; set; }
        public decimal Factor { get; set; }
        public int Grado { get; set; }
        public decimal ContProteico { get; set; }
        public decimal ImporteGG { get; set; }
        public decimal ImporteSecado { get; set; }
        public decimal ImporteZaranda { get; set; }
        public decimal ImporteAcarreo { get; set; }
        public decimal ImporteFlete { get; set; }
        public decimal ImporteFumigada { get; set; }
        public decimal ImporteOtros { get; set; }
        public decimal ImporteSellado { get; set; }
        public decimal ImporteAnalisis { get; set; }
        public decimal ImporteImp1 { get; set; }
        public decimal ImporteImp2 { get; set; }
        public decimal SubTotal { get; set; }
        public decimal AlicuotaIva { get; set; }
        public decimal ImporteIva { get; set; }
        public decimal Total { get; set; }
        public string FormaPago { get; set; }
        public List<Analisis> DetalleAnalisis { get; set; }
        public List<ItemRomaneo> DetalleRomaneo { get; set; }

        public string Coe { get; set; }
    }
    public class Retencion
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Alicuota { get; set; }
        public decimal ImporteBase { get; set; }
        public decimal Importe { get; set; }
    }
    public class Deduccion
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Alicuota { get; set; }
        public decimal ImporteBase { get; set; }
        public decimal AlicuotaIva { get; set; }
        public decimal ImporteIva { get; set; }
        public decimal Importe { get; set; }
    }
    public class CertificadoLiquidacion
    {
        public DateTime Fecha { get; set; }
        public long Numero { get; set; }
        public Int64 PesoNeto { get; set; }
        public string Tipo { get; set; }
        public int Grado { get; set; }
        public decimal Factor { get; set; }
    }
    public class Liquidacion
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdTransaccion { get; set; }
        public int IdTipo { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Numero { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public Int64 PesoNeto { get; set; }
        public decimal TarifaComision { get; set; }
        public decimal TarifaFlete { get; set; }
        public decimal ImporteComision { get; set; }
        public decimal ImporteFlete { get; set; }
        public int Grado { get; set; }
        public decimal Factor { get; set; }
        public decimal ContProteico { get; set; }
        public decimal ImporteIva { get; set; }
        public decimal RetIva { get; set; }
        public decimal RetGan { get; set; }
        public decimal RetIb { get; set; }
        public decimal ImporteFinal { get; set; }
        public decimal ImporteNeto { get; set; }
        public decimal ImporteRg2300 { get; set; }
        public decimal PrecioOperacion { get; set; }
        public string Puerto { get; set; }
        public string NumeroCertificado { get; set; }
        public string Procedencia { get; set; }
        public string Coe { get; set; }
        public string CoeAjustado { get; set; }
        public string Actividad { get; set; }
        public string TipoOperacion { get; set; }
        public string Tipo { get; set; }
        public decimal ImporteBruto { get; set; }
        public List<Deduccion> Deducciones { get; set; }
        public List<Retencion> Retenciones { get; set; }
        public List<CertificadoLiquidacion> Certificados { get; set; }
        public Liquidacion()
        {
            this.Deducciones = new List<Deduccion>();
            this.Retenciones = new List<Retencion>();
            this.Certificados = new List<CertificadoLiquidacion>();
        }
    }
    public class Salida
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdCosecha { get; set; }
        public Cosecha Cosecha { get; set; }
        public string IdCuenta { get; set; }
        public string IdPlanta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public string NumeroCPorte { get; set; }
        public Int64 PesoNeto { get; set; }
    }
    public class Retiro
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdCosecha { get; set; }
        public Cosecha Cosecha { get; set; }
        public string IdCuenta { get; set; }
        public string IdPlanta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public string NumeroCPorte { get; set; }
        public Int64 PesoNeto { get; set; }
    }
    public class LiquidacionSec
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdTransaccion { get; set; }
        public int IdTipo { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Numero { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public Int64 PesoNeto { get; set; }
        public decimal ImporteComision { get; set; }
        public decimal ImporteSellado { get; set; }
        public decimal ImporteDeduccion { get; set; }
        public decimal ImporteFlete { get; set; }
        public decimal ImporteIva { get; set; }
        public decimal RetIva { get; set; }
        public decimal RetGan { get; set; }
        public decimal RetIb { get; set; }
        public decimal PrecioOperacion { get; set; }
        public string Coe { get; set; }
        public string CoeAjustado { get; set; }
        public decimal ImporteBruto { get; set; }
        public decimal ImporteFinal { get; set; }

    }

    public class MovPlantaCereal
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdCosecha { get; set; }
        public Cosecha Cosecha { get; set; }
        public string IdCuenta { get; set; }
        public string IdPlanta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string Tipo { get; set; }
        public string IdTransaccion { get; set; }
        public string Numero { get; set; }
        public string NumeroCPorte { get; set; }
        public Int64 PesoBruto { get; set; }
        public Int64 PesoTara { get; set; }
        public Int64 PesoNeto { get; set; }
    }
    public class Boleto
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdCosecha { get; set; }
        public Cosecha Cosecha { get; set; }
        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string IdTransaccion { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Numero { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
        public string IdMoneda { get; set; } = "0001";
        public string IdCondicionVenta { get; set; }
        public string CondicionVenta { get; set; }
        public Int64 PesoNeto { get; set; }
        public string Obs { get; set; }
        public string Estado { get; set; }
        public bool AFijar { get; set; }
        public Int64 PendienteFijar { get; set; }
    }
    public class BoletoPendienteLiquidar
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string IdCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Precio { get; set; }
        public Int64 PesoNeto { get; set; }
        public string Moneda { get; set; }
        public string IdCondicionVenta { get; set; }
        public string CondicionVenta { get; set; }
        public Int64 PesoLiquidado { get; set; }
        public Int64 PesoPendienteLiquidar { get; set; }
    }

    public class Rt
    {
        public string IdSucursal { get; set; } = "01";
        public string Id { get; set; }
        public string IdTransaccion { get; set; }
        public string IdCosecha { get; set; }
        public Cosecha Cosecha { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public Int64 PesoNeto { get; set; }
        public string IdCuenta { get; set; }
        public Sujeto Cuenta { get; set; }
        public string IdCuentaDestino { get; set; }
        public Sujeto CuentaDestino { get; set; }

    }
    //Contrato
    public class ItemPosicionFisica
    {
        public string Concepto { get; set; } = "";
        public Int64 Trigo { get; set; } = 0;
        public Int64 Maiz { get; set; } = 0;
        public Int64 Soja { get; set; } = 0;
        public Int64 Sorgo { get; set; } = 0;
        public Int64 Girasol { get; set; } = 0;

    }
    public class Contrato
    {
        public string Id { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public string IdCosecha { get; set; }
        public Cosecha Cosecha { get; set; }
        public string NombreCereal { get; set; }
        public string NombreComprador { get; set; }
        public Int64 PesoNeto { get; set; }
        public string Estado { get; set; } = "PENDIENTE";
    }

    public class EstadoContratoView
    {
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public string NombreComprador { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        //A Fijar - Normal
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public Int64 PesoNeto { get; set; }
        public Int64 PesoAplicado { get; set; }
        public Int64 PesoPendienteAplicar { get; set; }
        public Int64 PesoFijado { get; set; }
        public Int64 PesoPendienteFijar { get; set; }
        public Int64 PesoLiquidado { get; set; }
        public Int64 PesoPendienteLiquidar { get; set; }

    }
}
