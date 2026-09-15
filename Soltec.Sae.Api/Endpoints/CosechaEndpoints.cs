using Soltec.Sae.Api;

public static class CosechaEndpoints
{
    public static void MapCosechaEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        app.MapGet("/api/cereales/cosecha", (HttpRequest request, HttpResponse response) =>
        {
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Cosecha> result = new List<Cosecha>();
            foreach (var suc in sucFilter)
            {
                CosechaService service = new CosechaService(suc.ConnectionStrings);
                result = service.List();
            }
            return result;
        });

        app.MapGet("/api/cereales/cosecha/{id}", (string id, HttpRequest request, HttpResponse response) =>
        {
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Cosecha result = new Cosecha();
            foreach (var suc in sucFilter)
            {
                CosechaService service = new CosechaService(suc.ConnectionStrings);
                result = service.FindOne(id);
            }
            return result;
        });
    }
}
