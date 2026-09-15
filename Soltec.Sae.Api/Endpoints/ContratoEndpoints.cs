using Soltec.Sae.Api;

public static class ContratoEndpoints
{
    public static void MapContratoEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //Contrato
        app.MapGet("/api/cereales/Contrato", (HttpRequest request, HttpResponse response) =>
        {
            string numero = request.Query["numero"].ToString();
            string tipo = request.Query["tipo"].ToString();
            string estado = request.Query["estado"].ToString();
            string id = request.Query["id"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-355) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Contrato> result = new List<Contrato>();
            foreach (var suc in sucFilter)
            {
                ContratoService service = new ContratoService(suc.ConnectionStrings);
                var tmpresult = service.List(id, numero, fecha, fechaHasta, tipo, estado);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/Contrato/estado", (HttpRequest request, HttpResponse response) =>
        {
            string numero = request.Query["numero"].ToString();
            string tipo = request.Query["tipo"].ToString();
            string estado = request.Query["estado"].ToString();
            string id = request.Query["id"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-355) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<EstadoContratoView> result = new List<EstadoContratoView>();
            foreach (var suc in sucFilter)
            {
                ContratoService service = new ContratoService(suc.ConnectionStrings);
                var tmpresult = service.ListEstado(id, numero, fecha, fechaHasta, estado, tipo);
                result.AddRange(tmpresult);
            }
            return result;
        });
    }
}
