using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class CerealesTools
{
    [McpServerTool, Description(
        "Devuelve el listado de entradas de cereales (recepción de granos en planta) registradas " +
        "para una cuenta y cosecha, dentro de un rango de fechas. Usar cuando el usuario pregunte " +
        "por entradas, recepciones o ingresos de cereal de un cliente. Si no se especifica rango de " +
        "fechas, devuelve solo las entradas un año atras hasta  hoy, NO el historial completo — para consultar " +
        "el histórico hay que pasar 'fecha' explícitamente."
    )]
    public static List<Entrada> EntradaCereales(
        List<Sucursal> sucursales, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de cuenta del productor/cliente en el sistema SAE. Dejar en blanco para todas las cuentas.")] string idCuenta,
        [Description("Id de cosecha del productor/cliente en el sistema SAE. Dejar en blanco para todas las cosechas.")] string idCosecha,
        [Description("Id de la sucursal. Vacío = todas las sucursales")] string idSucursal = "",
        [Description("Fecha desde, formato MM-dd-yyyy. Vacío = hoy - 1 año")] string? fecha = null,
        [Description("Fecha hasta, formato MM-dd-yyyy. Vacío = hoy")] string? fechaHasta = null)
    {
        var f  = string.IsNullOrEmpty(fecha)      ? DateTime.Now : DateTime.ParseExact(fecha, "MM-dd-yyyy", null);
        var fh = string.IsNullOrEmpty(fechaHasta) ? DateTime.Now : DateTime.ParseExact(fechaHasta, "MM-dd-yyyy", null);

        var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
        var result = new List<Entrada>();
        foreach (var suc in sucFilter)
        {
            var service = new EntradaService(suc.ConnectionStrings) { IdSucursal = suc.Id };
            result.AddRange(service.List(idCuenta, idCosecha, f, fh));
        }
        return result;
    }
}