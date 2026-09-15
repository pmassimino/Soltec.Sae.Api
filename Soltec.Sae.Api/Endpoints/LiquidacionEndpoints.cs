using Soltec.Sae.Api;

public static class LiquidacionEndpoints
{
    public static void MapLiquidacionEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //Liquidaciones
        app.MapGet("/api/cereales/liquidacion", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Liquidacion> result = new List<Liquidacion>();
            foreach (var suc in sucFilter)
            {
                LiquidacionService service = new LiquidacionService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.List(idCuenta, idCosecha, fecha);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/liquidacion/{id}", (string id, HttpRequest request, HttpResponse response) =>
        {
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Liquidacion result = new Liquidacion();
            foreach (var suc in sucFilter)
            {
                LiquidacionService service = new LiquidacionService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpResult = service.FindOne(id);
                if (tmpResult != null) result = tmpResult;
            }
            return result;
        });

        app.MapGet("/api/cereales/liquidacion/total", (HttpRequest request, HttpResponse response) =>
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
                LiquidacionService service = new LiquidacionService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.Total(idCuenta, idCosecha, fecha);
                result += tmpresult;
            }
            return result;
        });

        //Liquidacion Secundaria
        app.MapGet("/api/cereales/liquidacionsec", (HttpRequest request, HttpResponse response) =>
        {
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<LiquidacionSec> result = new List<LiquidacionSec>();
            foreach (var suc in sucFilter)
            {
                LiquidacionSecService service = new LiquidacionSecService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.List(idCosecha, DateTime.Now);
                result.AddRange(tmpresult);
            }
            return result;
        });
    }
}
