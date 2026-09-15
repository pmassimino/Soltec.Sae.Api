using System.Data;
using ClosedXML.Excel;
using Microsoft.Extensions.Caching.Memory;
using Soltec.Sae.Api;

public static class SujetoEndpoints
{
    public static void MapSujetoEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
        string tipoSaldo = app.Configuration["TipoSaldo"];
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();
        var seccionPendiente = app.Configuration.GetSection("SeccionPendiente").GetChildren().ToList().Select(x => new Seccion
        {
            Id = x.GetValue<string>("Id"),
            Nombre = x.GetValue<string>("Nombre"),
        }).ToList();

        app.MapGet("/api/contabilidad/sujeto", () =>
        {
            SujetoService sujetoService = new SujetoService(connectionStringBase);
            List<Sujeto> result = sujetoService.List();
            return result;
        });

        app.MapGet("/api/contabilidad/sujeto/xls", () =>
        {
            SujetoService sujetoService = new SujetoService(connectionStringBase);
            List<Sujeto> result = sujetoService.List();
            DataTable dt = new DataTable("Grid");

            dt.Columns.AddRange(new DataColumn[9] { new DataColumn("Codigo"),
                                                    new DataColumn("Nombre"),
                                                    new DataColumn("Numero Documento"),
                                                    new DataColumn("Domicilio"),
                                                    new DataColumn("Codigo Postal"),
                                                    new DataColumn("Localidad"),
                                                    new DataColumn("Provincia"),
                                                    new DataColumn("Condicion Iva"),
                                                    new DataColumn("Condicion I.B.")});

            foreach (var item in result)
            {
                dt.Rows.Add(item.Id, item.Nombre, item.NumeroDocumento, item.Domicilio, item.CodigoPostal, item.Localidad, item.Provincia, item.CondicionIva, item.CondicionIB);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {

                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sujeto.xlsx");
                }
            }
        });

        app.MapGet("/api/contabilidad/sujeto/{id}", (string id) =>
        {
            SujetoService sujetoService = new SujetoService(connectionStringBase);
            Sujeto result = sujetoService.FindOne(id);
            return result == null ? Results.NotFound() : Results.Ok(result);
        });

        app.MapGet("/api/contabilidad/sujeto/documento/{numero}", (string numero) =>
        {
            SujetoService sujetoService = new SujetoService(connectionStringBase);
            var result = sujetoService.FindByDoc(numero);
            return result == null ? Results.NotFound() : Results.Ok(result);
        });

        //Resumen de cuenta sujeto
        app.MapGet("/api/contabilidad/sujeto/{idCuenta}/resumen", (string idCuenta, HttpRequest request, IMemoryCache cache) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = string.IsNullOrEmpty(fechaStr)
                ? DateTime.Now
                : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // 1. Se usa boolResult para no colisionar el nombre de la variable 'result'
            bool filtraSaldoCero = bool.TryParse(request.Query["FiltraSaldoCero"], out var boolResult) && boolResult;

            Dictionary<string, string> error = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(idCuenta))
            {
                error.Add("idCuenta", "Parametro Requerido");
            }
            if (error.Count > 0)
            {
                return Results.BadRequest(error);
            }

            // Define una clave unica para el cache, basada en los parametros que afectan el resultado
            string cacheKey = $"ResumenView_{idCuenta}_{fecha:yyyyMMdd}";
            if (cache.TryGetValue(cacheKey, out ResumenView cachedResult))
            {
                return Results.Ok(cachedResult);
            }

            // Saldos Cta. Cte.
            var result = new ResumenView();

            SujetoService sujetoService = new SujetoService(connectionStringBase);
            var tmpCuenta = sujetoService.FindOne(idCuenta);
            if (tmpCuenta == null)
            {
                return Results.NotFound("Cuenta no encontrada");
            }
            result.Sujeto = tmpCuenta;
            CtaCteService ctaCteService = new CtaCteService(connectionStringBase);

            foreach (var item in tmpCuenta.Subdiarios)
            {
                int idDivisa = Convert.ToInt16(item.IdDivisa);

                var saldoVencido = ctaCteService.Saldo(idCuenta, item.Id, DateTime.Now, idDivisa, true);
                var saldo = ctaCteService.Saldo(idCuenta, item.Id, DateTime.Now, idDivisa, false);

                var itemNew = new CtaCteView
                {
                    IdCuenta = idCuenta,
                    IdSubdiario = item.Id,
                    Nombre = item.Nombre,
                    IdDivisa = idDivisa,
                    Saldo = saldo,
                    SaldoVencido = saldoVencido
                };

                result.CtaCte.Add(itemNew);
            }

            // Saldo Cosechas
            List<SaldoCtaCteCereal> tmpSaldos = new List<SaldoCtaCteCereal>();
            foreach (var suc in sucursales)
            {
                var service = new CtaCteCerealService(suc.ConnectionStrings)
                {
                    SaeConnectionStringBase = connectionStringBase,
                    IdSucursal = suc.Id,
                    TipoSaldo = tipoSaldo
                };
                var tmpresult = service.Saldos(idCuenta, null, fecha);
                tmpSaldos.AddRange(tmpresult);
            }
            foreach (var item in tmpSaldos)
            {
                CosechaView cosechaView = new CosechaView();
                cosechaView.IdSucursal = item.IdSucursal;
                cosechaView.IdCosecha = item.IdCosecha;
                cosechaView.Nombre = item.NombreCosecha;
                cosechaView.Saldo = item.Saldo;
                result.Cosechas.Add(cosechaView);
            }

            // Almacena el resultado en cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            cache.Set(cacheKey, result, cacheEntryOptions);

            //Facturas y Remitos Pendientes
            DateTime fechaDesde = fecha.AddYears(-3);
            FacturaService facturaService = new FacturaService(connectionStringBase);
            facturaService.SeccionPendiente = seccionPendiente;
            var tmpFacturasPendientes = facturaService.ListPendiente(idCuenta, fechaDesde, fecha);

            RemitoService remitoService = new RemitoService(connectionStringBase);
            var tmpRemitosPendientes = remitoService.ListPendiente(idCuenta, fechaDesde, fecha);
            result.RemitosPendientes = tmpRemitosPendientes;
            result.FacturasPendientes = tmpFacturasPendientes;
            return Results.Ok(result);
        });
    }
}
