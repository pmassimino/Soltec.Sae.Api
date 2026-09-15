using Soltec.Sae.Api;

public static class ArticuloEndpoints
{
    public static void MapArticuloEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];

        app.MapGet("/api/almacen/articulo", (bool? soloActivos) =>
        {
            ArticuloService service = new ArticuloService(connectionStringBase);

            // Instanciamos la clase de opciones que creamos
            var filtros = new ArticuloFilterOptions
            {
                FiltrarActivos = soloActivos ?? false // Si no se envía, por defecto es false
            };

            List<Articulo> result = service.List(filtros);
            return Results.Ok(result);
        });

        app.MapGet("/api/almacen/articulo/{id}", (string id) =>
        {
            ArticuloService service = new ArticuloService(connectionStringBase);
            Articulo result = service.FindOne(id);
            return result == null ? Results.NotFound() : Results.Ok(result);
        });

        app.MapGet("/api/almacen/articulo/stock", (HttpRequest request, HttpResponse response) =>
        {
            string idArticulo = request.Query["IdArticulo"].ToString();
            string idArticuloHasta = request.Query["IdArticuloHasta"].ToString();
            string idSeccion = request.Query["IdSeccion"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);

            if (string.IsNullOrEmpty(idArticuloHasta)) idArticuloHasta = idArticulo;
            MovStockService service = new MovStockService(connectionStringBase);
            List<Stock> result = service.ListStock(fecha, idArticulo, idArticuloHasta, idSeccion);
            return result == null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
