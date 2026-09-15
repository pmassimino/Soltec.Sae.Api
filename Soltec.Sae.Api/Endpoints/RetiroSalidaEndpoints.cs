using Soltec.Sae.Api;

public static class RetiroSalidaEndpoints
{
    public static void MapRetiroSalidaEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //Retiros
        app.MapGet("/api/cereales/retiro", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Retiro> result = new List<Retiro>();
            foreach (var suc in sucFilter)
            {
                RetiroService service = new RetiroService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.List(idCuenta, idCosecha, fecha);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/retiro/{id}", (string id, HttpRequest request, HttpResponse response) =>
        {
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Retiro result = new Retiro();
            foreach (var suc in sucFilter)
            {
                RetiroService service = new RetiroService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpResult = service.FindOne(id);
                if (tmpResult != null) result = tmpResult;
            }
            return result;
        });

        //Salidas
        app.MapGet("/api/cereales/salida", (HttpRequest request, HttpResponse response) =>
        {
            string idPlanta = request.Query["IdPlanta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-365) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Salida> result = new List<Salida>();
            foreach (var suc in sucFilter)
            {
                SalidaService service = new SalidaService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.List(idCosecha, idPlanta, fecha, fechaHasta);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/salida/{id}", (string id, HttpRequest request, HttpResponse response) =>
        {
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Salida result = new Salida();
            foreach (var suc in sucFilter)
            {
                SalidaService service = new SalidaService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpResult = service.FindOne(id);
                if (tmpResult != null) result = tmpResult;
            }
            return result;
        });

        app.MapGet("/api/cereales/retiro/total", (HttpRequest request, HttpResponse response) =>
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
                RetiroService service = new RetiroService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.Total(idCuenta, idCosecha, fecha);
                result += tmpresult;
            }
            return result;
        });
    }
}
