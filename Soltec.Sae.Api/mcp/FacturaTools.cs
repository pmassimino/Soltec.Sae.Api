using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class FacturaTools
{
    [McpServerTool, Description(
        "Devuelve el listado de facturas de venta, opcionalmente filtradas por cuenta y rango de fechas. " +
        "Usar cuando el usuario pregunte por facturas, ventas o comprobantes de venta de un cliente. " +
        "Si no se especifica rango de fechas, devuelve las facturas desde hace 530 días hasta hoy." + 
        "Si el Comprobante es de tipo 'NOTA DE CREDITO', el campo 'Total'  y todos los importes es negativo para cualquier suma" 
    )]
    public static List<Factura> Factura(
        IConfiguration configuration, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de cuenta del cliente en el sistema SAE. Dejar en blanco para todas las cuentas.")] string idCuenta = "",
        [Description("Fecha desde, formato MM-dd-yyyy. Vacío = hoy - 530 días")] string? fecha = null,
        [Description("Fecha hasta, formato MM-dd-yyyy. Vacío = hoy")] string? fechaHasta = null)
    {
        var connectionStringBase = configuration["ConnectionStringsSAE"];
        var seccionDolar = configuration.GetSection("SeccionDolar").GetChildren().ToList().Select(x => new Seccion
        {
            Id = x.GetValue<string>("Id"),
            Nombre = x.GetValue<string>("Nombre"),
        }).ToList();

        var f  = string.IsNullOrEmpty(fecha)      ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fecha, "MM-dd-yyyy", null);
        var fh = string.IsNullOrEmpty(fechaHasta) ? DateTime.Now               : DateTime.ParseExact(fechaHasta, "MM-dd-yyyy", null);

        var service = new FacturaService(connectionStringBase) { SeccionDolar = seccionDolar };
        return service.List(f, fh, idCuenta);
    }
}
