using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class LiquidacionTools
{
    [McpServerTool, Description(
        "Devuelve el listado de liquidaciones de cereal (liquidación de granos a productores/clientes) " +
        "para una cuenta y cosecha, en una fecha determinada. Usar cuando el usuario pregunte por " +
        "liquidaciones de cereal. Si no se especifica fecha, usa la fecha de hoy."
    )]
    public static List<Liquidacion> Liquidacion(
        List<Sucursal> sucursales, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de cuenta del productor/cliente en el sistema SAE. Dejar en blanco para todas las cuentas.")] string idCuenta = "",
        [Description("Id de cosecha del productor/cliente en el sistema SAE. Dejar en blanco para todas las cosechas.")] string idCosecha = "",
        [Description("Id de la sucursal. Vacío = todas las sucursales")] string idSucursal = "",
        [Description("Fecha, formato MM-dd-yyyy. Vacío = hoy")] string? fecha = null)
    {
        var f = string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.ParseExact(fecha, "MM-dd-yyyy", null);

        var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
        var result = new List<Liquidacion>();
        foreach (var suc in sucFilter)
        {
            var service = new LiquidacionService(suc.ConnectionStrings) { IdSucursal = suc.Id };
            result.AddRange(service.List(idCuenta, idCosecha, f));
        }
        return result;
    }
}
