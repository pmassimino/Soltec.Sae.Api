using Soltec.Sae.Api;

public static class RtEndpoints
{
    public static void MapRtEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //RT
        app.MapGet("/api/cereales/rt", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCuentaDestino = request.Query["IdCuentaDestino"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Rt> result = new List<Rt>();
            foreach (var suc in sucFilter)
            {
                RTService service = new RTService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.List(idCuenta, idCuentaDestino, idCosecha, fecha);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/rt/totaltransferido", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Int64 result = 0;
            foreach (var suc in sucFilter)
            {
                RTService service = new RTService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.TotalTransferido(idCuenta, idCosecha, fecha);
                result += tmpresult;
            }
            return result;
        });

        app.MapGet("/api/cereales/rt/totalrecibido", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Int64 result = 0;
            foreach (var suc in sucFilter)
            {
                RTService service = new RTService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.TotalRecibido(idCuenta, idCosecha, fecha);
                result += tmpresult;
            }
            return result;
        });
    }
}
