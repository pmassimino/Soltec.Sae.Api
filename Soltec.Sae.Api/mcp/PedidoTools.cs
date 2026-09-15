using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class PedidoTools
{
    [McpServerTool, Description(
        "Devuelve el listado de pedidos de venta, opcionalmente filtrados por cuenta y rango de fechas. " +
        "Usar cuando el usuario pregunte por pedidos de venta de un cliente. " +
        "Si no se especifica rango de fechas, devuelve los pedidos desde hace 530 días hasta hoy."
    )]
    public static List<Pedido> Pedido(
        IConfiguration configuration, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de cuenta del cliente en el sistema SAE. Dejar en blanco para todas las cuentas.")] string idCuenta = "",
        [Description("Fecha desde, formato MM-dd-yyyy. Vacío = hoy - 530 días")] string? fecha = null,
        [Description("Fecha hasta, formato MM-dd-yyyy. Vacío = hoy")] string? fechaHasta = null)
    {
        var connectionStringBase = configuration["ConnectionStringsSAE"];

        var f  = string.IsNullOrEmpty(fecha)      ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fecha, "MM-dd-yyyy", null);
        var fh = string.IsNullOrEmpty(fechaHasta) ? DateTime.Now               : DateTime.ParseExact(fechaHasta, "MM-dd-yyyy", null);

        var service = new PedidoService(connectionStringBase);
        return service.List(f, fh, idCuenta);
    }
}
