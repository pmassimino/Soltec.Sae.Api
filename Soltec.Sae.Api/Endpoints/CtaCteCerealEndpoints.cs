using Soltec.Sae.Api;

public static class CtaCteCerealEndpoints
{
    public static void MapCtaCteCerealEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
        string tipoSaldo = app.Configuration["TipoSaldo"];
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //Cuenta Corriente Cereales
        app.MapGet("/api/cereales/CtaCte", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<MovCtaCteCereal> result = new List<MovCtaCteCereal>();
            foreach (var suc in sucFilter)
            {
                CtaCteCerealService service = new CtaCteCerealService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                service.TipoSaldo = tipoSaldo;
                var tmpresult = service.List(idCuenta, idCosecha, fecha);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/CtaCteCereal/saldo", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<SaldoCtaCteCereal> result = new List<SaldoCtaCteCereal>();
            foreach (var suc in sucFilter)
            {
                CtaCteCerealService service = new CtaCteCerealService(suc.ConnectionStrings);
                service.SaeConnectionStringBase = connectionStringBase;
                service.IdSucursal = suc.Id;
                service.TipoSaldo = tipoSaldo;
                var tmpresult = service.Saldo(idCuenta, idCosecha, fecha);
                result.Add(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/CtaCteCereal/saldos", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            string FiltraSaldoCero = request.Query["FiltraSaldoCero"].ToString().ToLower();
            var fechaStr = request.Query["Fecha"].ToString();

            var fecha = string.IsNullOrWhiteSpace(fechaStr)
                ? DateTime.Now
                : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);

            var sucFilter = sucursales.Where(w => w.Id == idSucursal || string.IsNullOrEmpty(idSucursal));

            List<SaldoCtaCteCereal> result = new List<SaldoCtaCteCereal>();

            foreach (var suc in sucFilter)
            {
                var service = new CtaCteCerealService(suc.ConnectionStrings)
                {
                    SaeConnectionStringBase = connectionStringBase,
                    IdSucursal = suc.Id,
                    TipoSaldo = tipoSaldo
                };

                var tmpresult = service.Saldos2(idCuenta, idCosecha, fecha);

                if (FiltraSaldoCero == "si")
                {
                    tmpresult = tmpresult.Where(item => item.Saldo != 0).ToList();
                }

                result.AddRange(tmpresult);
            }

            return result;
        });

        //Obtiene las cosechas disponibles para una cuenta
        app.MapGet("/api/cereales/cosechasdisponibles", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idSucursal = request.Query["IdSucursal"].ToString();
            string FiltraSaldoCero = request.Query["FiltraSaldoCero"].ToString().ToLower();
            var fechaStr = request.Query["Fecha"].ToString();

            var fecha = string.IsNullOrWhiteSpace(fechaStr)
                ? DateTime.Now
                : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);

            var sucFilter = sucursales.Where(w => w.Id == idSucursal || string.IsNullOrEmpty(idSucursal));

            List<string> result = new List<string>();

            foreach (var suc in sucFilter)
            {
                var service = new CtaCteCerealService(suc.ConnectionStrings)
                {
                    SaeConnectionStringBase = connectionStringBase,
                    IdSucursal = suc.Id,
                    TipoSaldo = tipoSaldo
                };

                var tmpresult = service.ObtenerCosechasDisponibles(idCuenta);
                result.AddRange(tmpresult);
            }

            return result;
        });
    }
}
