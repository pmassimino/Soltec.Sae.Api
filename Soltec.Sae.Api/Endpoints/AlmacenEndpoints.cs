using Soltec.Sae.Api;

public static class AlmacenEndpoints
{
    public static void MapAlmacenEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];

        app.MapGet("/api/almacen/familia", () =>
        {
            FamiliaService service = new FamiliaService(connectionStringBase);
            List<Familia> result = service.List();
            return result;
        });

        app.MapGet("/api/almacen/linea", () =>
        {
            LineaService service = new LineaService(connectionStringBase);
            List<Linea> result = service.List();
            return result;
        });

        app.MapGet("/api/almacen/seccionoperativa", () =>
        {
            SeccionOperativaService service = new SeccionOperativaService(connectionStringBase);
            List<SeccionOperativa> result = service.List();
            return result;
        });

        //MovStock
        app.MapPost("/api/almacen/MovStock", (MovStock entity) =>
        {   //Validar datos
            if (string.IsNullOrEmpty(entity.IdArticulo))
                return Results.BadRequest("IdArticulo requerido");
            if (string.IsNullOrEmpty(entity.IdDeposito))
                return Results.BadRequest("IdDeposito requerido");
            if (string.IsNullOrEmpty(entity.IdDepositoDestino))
                return Results.BadRequest("IdDepositoDestino requerido");
            if (entity.IdDeposito == entity.IdDepositoDestino)
                return Results.BadRequest("Deposito origen y destino no pueden ser iguales");
            if (string.IsNullOrEmpty(entity.Concepto))
                return Results.BadRequest("Concepto requerido");
            if (entity.Serie == null)
                return Results.BadRequest("Serie requerido");
            //if (entity.Fecha.Date < DateTime.Now.Date)
            //    return Results.BadRequest("Fecha no puede ser menor a la actual");
            if (entity.Cantidad < 0)
                return Results.BadRequest("Cantidad debe ser mayor a cero");
            //Verificar que exista el articulo
            ArticuloService articuloService = new ArticuloService(connectionStringBase);
            var existeArt = articuloService.FindOne(entity.IdArticulo) != null;
            if (!existeArt)
                return Results.BadRequest("Articulo no existe");

            MovStockService service = new MovStockService(connectionStringBase);
            service.add(entity);
            return Results.Ok();
        });
    }
}
