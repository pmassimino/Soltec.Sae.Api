using System.Text.Json.Serialization;

namespace Soltec.Sae.Api
{
    public class Empresa
    {        
        public string Nombre { get; set; }
        public string Cuit { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CondIva { get; set; }
    }
    public class Sucursal
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string ConnectionStrings { get; set; }
    }
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
    public class EntityGeneric 
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
    public class Subdiario
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int IdDivisa { get; set; }
    }
    public class Articulo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdFamilia { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal MargenVenta { get; set; }
        public decimal AlicuotaIva { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioVentaFinal { get; set; }

    }
    public class Familia
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        
    }
    public class Seccion
    {
        public string Id { get; set; }
        public string Nombre { get; set; }

    }

    public class MovStock
    {
        public string Id { get; set; }
        public string IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string Concepto { get; set; }
        public DateTime Fecha { get; set; }
        public string IdDeposito {get;set;}
        public string IdDepositoDestino { get; set; }
        public double Cantidad { get; set; }
        public int Pe { get; set; }
        public int Numero { get; set; }
        public string Lote { get; set; }

    }
    public class Stock    {
        public string IdArticulo { get; set; }        
        public string NombreArticulo { get; set; }
        public double Cantidad { get; set; }

    }
    public class StockDeposito 
    { 
        public string Id { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get;set;}

    }
    public class MovCtaCte 
    {
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }   
        public  string Concepto { get; set; }
        public string IdCuenta { get; set; }
        public string  NumeroComprobante { get; set; }
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
        public string IdCuenta { get; set;}
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
    public class Factura
    {
        public Factura()
            {
            this.Detalle = new List<DetalleFactura>();
            }
    public string Sec { get; set; }
    public   string Orden { get;set; }
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
    public decimal PrecepcionIB { get; set; }
    public decimal Total { get; set; }    
    public Int64 Cae { get; set; }
    public string Remito { get; set; }
    public string  CondVenta { get; set; }    
    public string TipoComp { get; set; }    
    public List<DetalleFactura> Detalle { get; set; }
    public string IdClaseVenta { get; set; }
    public string IdCampania { get; set; }
    }
    public class DetalleFactura 
    {      
    public string IdArticulo { get; set; }
    public string Concepto {get;set;}
    public decimal Precio {get; set;}
    public decimal AlicuotaIva {get; set;}
    public decimal Iva { get; set; }
    public decimal ImpInterno { get; set; }
    public decimal Cantidad { get; set;}
    public string UnidadMedida { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Descuento {get; set;}
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
        public decimal PrecepcionIva { get; set; }
        public decimal PrecepcionIB { get; set; }
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
        public DetalleMayor AddDetalle(string IdCuentaMayor, string Concepto, string IdTipo, decimal Debe,decimal Haber, DateTime FechaVenc, string IdCuenta = null)
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
        public string Pe{ get; set; }
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
    public class Remito
    {
        public  Remito()
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
        public string  Id { get; set; }
        public string IdCosecha { get; set; }
        public string NombreCosecha { get; set; }
        public string NombreCereal { get; set; }
        public string  IdCuenta { get; set; }
        public string  Nombre { get; set; }
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
        public Sujeto Transporte { get;set; }
        public Sujeto Chofer { get; set; }    
        public string Procedencia { get; set; }
        public  decimal Distancia { get; set; }
        public string IdPlanta { get; set; }   
        public string IdLocalidadProcedencia { get; set; }
        public string LocalidadProcedencia { get;set;}
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
        public string  Nombre { get; set; }
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
        public  decimal Precio { get; set; }
        public Int64 PesoNeto { get; set; }
        public decimal TarifaComision { get; set; }
        public  decimal TarifaFlete { get; set; }
        public decimal ImporteComision { get; set; }
        public  decimal ImporteFlete { get; set; }
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
        public string  Puerto { get; set; }
        public string NumeroCertificado { get; set; }
        public string Procedencia { get; set; }
        public string Coe  { get; set; }
        public string CoeAjustado { get; set; }
        public string Actividad { get; set; }
        public string  TipoOperacion { get; set; }
        public string Tipo { get; set; }
        public decimal ImporteBruto { get; set; }
        public List<Deduccion> Deducciones { get; set; }
        public List<Retencion> Retenciones { get; set; }
        public List<CertificadoLiquidacion> Certificados { get; set; }
        public  Liquidacion() 
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
        public Int64 PesoNeto {get;set;}
        public Int64 PesoAplicado { get; set; }
        public Int64 PesoPendienteAplicar { get; set; }
        public Int64 PesoFijado { get; set; }
        public Int64 PesoPendienteFijar { get; set; }
        public Int64 PesoLiquidado { get; set; }
        public Int64 PesoPendienteLiquidar { get; set; }

    }



}
