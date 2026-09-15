using Soltec.Sae.Api;

public static class PlantaEndpoints
{
    public static void MapPlantaEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
        string connectionStringCerealesBase = app.Configuration["ConnectionStringsCereales"];
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //Planta
        app.MapGet("/api/cereales/planta/", (HttpRequest request, HttpResponse response) =>
        {
            List<Planta> result = new List<Planta>();
            PlantaService service = new PlantaService(connectionStringCerealesBase);
            result = service.List();
            return result;
        });

        app.MapGet("/api/cereales/planta/c14", (HttpRequest request, HttpResponse response) =>
        {
            string idPlanta = request.Query["IdPlanta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-30) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<C14View> result = new List<C14View>();
            foreach (var suc in sucFilter)
            {
                MovPlantaCerealService service = new MovPlantaCerealService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.C14(idPlanta, idCosecha, fecha, fechaHasta);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/planta/saldo", (HttpRequest request, HttpResponse response) =>
        {
            string idPlanta = request.Query["IdPlanta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-30) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<SaldoPlantaView> result = new List<SaldoPlantaView>();
            foreach (var suc in sucFilter)
            {
                MovPlantaCerealService service = new MovPlantaCerealService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.Saldo(idPlanta, idCosecha, fecha, fechaHasta);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/planta/posicioncomercial", (HttpRequest request, HttpResponse response) =>
        {
            string idPlanta = request.Query["IdPlanta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-365) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<ItemPosicionFisica> result = new List<ItemPosicionFisica>();

            CerealesService service = new CerealesService(connectionStringCerealesBase);
            service.Sucursales = sucFilter.ToList();
            service.SaeConnectionStringBase = connectionStringBase;
            result = service.PosicionFisica(fecha, fechaHasta);
            return result;
        });
    }
}
