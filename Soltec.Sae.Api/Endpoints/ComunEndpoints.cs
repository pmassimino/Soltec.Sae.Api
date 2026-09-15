using Microsoft.AspNetCore.Authorization;
using Soltec.Sae.Api;

public static class ComunEndpoints
{
    public static void MapComunEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];

        app.MapGet("/api/comun/provincia", () =>
        {
            ProvinciaService service = new ProvinciaService(connectionStringBase);
            List<EntityGeneric> result = service.List();
            return result;
        });

        app.MapGet("/api/comun/categoria", () =>
        {
            CategoriaService service = new CategoriaService(connectionStringBase);
            List<EntityGeneric> result = service.List();
            return result;
        });

        app.MapGet("/api/comun/zona", () =>
        {
            ZonaService service = new ZonaService(connectionStringBase);
            List<EntityGeneric> result = service.List();
            return result;
        });

        app.MapGet("/api/comun/isRuning", [AllowAnonymous] () =>
        {
            return Results.Ok(true);
        });
    }
}
