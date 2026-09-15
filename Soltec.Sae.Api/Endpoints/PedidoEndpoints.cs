using Soltec.Sae.Api;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];

        //Pedido
        app.MapGet("/api/ventas/pedido", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var incluyeDetalleStr = request.Query["IncluyeDetalle"].ToString();
            var incluyeDetalle = incluyeDetalleStr == "" ? false : Convert.ToBoolean(incluyeDetalleStr);
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            PedidoService service = new PedidoService(connectionStringBase);
            List<Pedido> result = null;
            result = service.List(fecha, fechaHasta, idCuenta);
            return Results.Ok(result);
        });
    }
}
