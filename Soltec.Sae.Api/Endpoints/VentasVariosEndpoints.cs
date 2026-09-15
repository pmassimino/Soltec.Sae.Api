using Soltec.Sae.Api;

public static class VentasVariosEndpoints
{
    public static void MapVentasVariosEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];

        app.MapGet("/api/ventas/Seccion", (HttpRequest request, HttpResponse response) =>
        {
            SeccionService service = new SeccionService(connectionStringBase);
            List<Seccion> result = null;
            result = service.List();
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/ClaseVenta", (HttpRequest request, HttpResponse response) =>
        {
            ClaseVentaService service = new ClaseVentaService(connectionStringBase);
            List<EntityGeneric> result = null;
            result = service.List();
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/Campania", (HttpRequest request, HttpResponse response) =>
        {
            CampaniaService service = new CampaniaService(connectionStringBase);
            List<EntityGeneric> result = null;
            result = service.List();
            return Results.Ok(result);
        });
    }
}
