using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class ArticuloTools
{
    [McpServerTool, Description(
        "Devuelve el listado de artículos del almacén (precios, stock, familia, sección, etc.). " +
        "Usar cuando el usuario pregunte por artículos, productos, precios o stock."
    )]
    public static List<Articulo> Articulo(
        IConfiguration configuration, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Si es true, devuelve solo los artículos activos. Si no se especifica, devuelve todos.")] bool? soloActivos = null)
    {
        var connectionStringBase = configuration["ConnectionStringsSAE"];

        var service = new ArticuloService(connectionStringBase);
        var filtros = new ArticuloFilterOptions
        {
            FiltrarActivos = soloActivos ?? false
        };

        return service.List(filtros);
    }
}
