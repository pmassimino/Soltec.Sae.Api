using Soltec.Sae.Api;

public static class VariosCerealEndpoints
{
    public static void MapVariosCerealEndpoints(this WebApplication app)
    {
        string connectionStringCerealesBase = app.Configuration["ConnectionStringsCereales"];

        app.MapGet("/api/cereales/condicionventa", () =>
        {
            CondicionVentaCerealService service = new CondicionVentaCerealService(connectionStringCerealesBase);
            List<EntityGeneric> result = service.List();
            return result;
        });

        app.MapGet("/api/cereales/localidad", () =>
        {
            LocalidadService service = new LocalidadService(connectionStringCerealesBase);
            List<Localidad> result = service.List();
            return result;
        });
    }
}
