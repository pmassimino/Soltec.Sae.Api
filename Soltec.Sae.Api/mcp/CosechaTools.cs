using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class CosechaTools
{
    [McpServerTool, Description(
        "Devuelve el listado de cosechas de cereal registradas en el sistema SAE. " +
        "Usar cuando el usuario pregunte por las cosechas disponibles o su información."
    )]
    public static List<Cosecha> Cosecha(
        List<Sucursal> sucursales, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de la sucursal. Vacío = todas las sucursales")] string idSucursal = "")
    {
        var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
        var result = new List<Cosecha>();
        foreach (var suc in sucFilter)
        {
            var service = new CosechaService(suc.ConnectionStrings);
            result.AddRange(service.List());
        }
        return result;
    }
}
