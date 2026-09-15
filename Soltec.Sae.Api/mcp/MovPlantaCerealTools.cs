using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class MovPlantaCerealTools
{
    [McpServerTool, Description(
        "Devuelve el saldo de cereal en planta (stock de granos) para una planta y cosecha, " +
        "dentro de un rango de fechas. Usar cuando el usuario pregunte por saldo o stock de cereal en planta. " +
        "Si no se especifica rango de fechas, devuelve el saldo desde hace 30 días hasta hoy."
    )]
    public static List<SaldoPlantaView> SaldoPlanta(
        List<Sucursal> sucursales, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de planta en el sistema SAE. Dejar en blanco para todas las plantas.")] string idPlanta = "",
        [Description("Id de cosecha. Dejar en blanco para todas las cosechas.")] string idCosecha = "",
        [Description("Id de la sucursal. Vacío = todas las sucursales")] string idSucursal = "",
        [Description("Fecha desde, formato MM-dd-yyyy. Vacío = hoy - 30 días")] string? fecha = null,
        [Description("Fecha hasta, formato MM-dd-yyyy. Vacío = hoy")] string? fechaHasta = null)
    {
        var f  = string.IsNullOrEmpty(fecha)      ? DateTime.Now.AddDays(-30) : DateTime.ParseExact(fecha, "MM-dd-yyyy", null);
        var fh = string.IsNullOrEmpty(fechaHasta) ? DateTime.Now               : DateTime.ParseExact(fechaHasta, "MM-dd-yyyy", null);

        var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
        var result = new List<SaldoPlantaView>();
        foreach (var suc in sucFilter)
        {
            var service = new MovPlantaCerealService(suc.ConnectionStrings) { IdSucursal = suc.Id };
            result.AddRange(service.Saldo(idPlanta, idCosecha, f, fh));
        }
        return result;
    }
}
