using Soltec.Sae.Api;

public static class RemitoEndpoints
{
    public static void MapRemitoEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];

        //Remito
        app.MapGet("/api/ventas/Remito", (HttpRequest request, HttpResponse response) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var incluyeDetalleStr = request.Query["IncluyeDetalle"].ToString();
            var incluyeDetalle = incluyeDetalleStr == "" ? false : Convert.ToBoolean(incluyeDetalleStr);
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            RemitoService service = new RemitoService(connectionStringBase);
            List<Remito> result = null;
            result = service.List(fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/Remito/pendiente", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            RemitoService service = new RemitoService(connectionStringBase);
            List<DocumentoPendienteView> result = null;
            result = service.ListPendiente(idCuenta, fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/remito/informe", (HttpRequest request, HttpResponse response) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-365) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            RemitoService service = new RemitoService(connectionStringBase);
            List<RemitoView> result = null;
            result = service.ListInforme(fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/Remito/{orden}", (string orden, HttpRequest request, HttpResponse response) =>
        {
            string sec = request.Query["sec"];
            if (sec.Trim() == "")
            {
                return Results.BadRequest("Parametro Sec requerido");
            }
            if (orden.Trim() == "")
            {
                return Results.BadRequest("Parametro Orden requerido");
            }

            RemitoService service = new RemitoService(connectionStringBase);
            Remito result = null;
            result = service.FindOne(sec, orden);
            return Results.Ok(result);
        });
    }
}
