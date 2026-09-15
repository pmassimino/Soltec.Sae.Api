using System.Data;
using ClosedXML.Excel;
using Soltec.Sae.Api;

public static class BoletoEndpoints
{
    public static void MapBoletoEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

        //Boletos
        app.MapGet("/api/cereales/boleto", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<Boleto> result = new List<Boleto>();
            foreach (var suc in sucFilter)
            {
                BoletoService service = new BoletoService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.List(idCuenta, idCosecha, fecha);
                result.AddRange(tmpresult);
            }
            return result;
        });

        // Registrar un nuevo boleto
        app.MapPost("/api/cereales/boleto", async (Boleto boletoRequest) =>
        {
            try
            {
                // Validar sucursal
                if (string.IsNullOrEmpty(boletoRequest.IdSucursal))
                    return Results.BadRequest(new { error = "IdSucursal es requerido" });

                var sucursal = sucursales.FirstOrDefault(w => w.Id == boletoRequest.IdSucursal);
                if (sucursal == null)
                    return Results.NotFound(new { error = $"Sucursal {boletoRequest.IdSucursal} no encontrada" });

                // Crear servicio
                BoletoService service = new BoletoService(sucursal.ConnectionStrings);
                service.IdSucursal = sucursal.Id;

                // Crear boleto
                var boleto = new Boleto
                {
                    IdSucursal = boletoRequest.IdSucursal,
                    IdCosecha = boletoRequest.IdCosecha,
                    IdCuenta = boletoRequest.IdCuenta,
                    Fecha = boletoRequest.Fecha,
                    FechaVencimiento = boletoRequest.FechaVencimiento,
                    Precio = boletoRequest.Precio,
                    IdMoneda = boletoRequest.IdMoneda ?? "0",
                    IdCondicionVenta = boletoRequest.IdCondicionVenta ?? "1",
                    PesoNeto = boletoRequest.PesoNeto,
                    Obs = boletoRequest.Obs ?? "",
                    Estado = "ACTIVO",
                    AFijar = boletoRequest.AFijar,
                    PendienteFijar = boletoRequest.PendienteFijar
                };

                // Validar
                var errores = service.Validate(boleto);
                if (errores.Any())
                    return Results.BadRequest(new { error = "Errores de validación", detalles = errores });

                // Insertar
                bool resultado = service.Insert(boleto);

                if (!resultado)
                    return Results.BadRequest(new { error = "Error al registrar el boleto en la base de datos" });

                // Obtener el boleto creado
                var boletoCreado = service.FindOne(boleto.Id);
                return Results.Created($"/api/cereales/boleto/{boleto.Id}", boletoCreado);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = $"Error interno: {ex.Message}" });
            }
        });

        app.MapGet("/api/cereales/boleto/pendiente", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-3650) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<BoletoPendienteLiquidar> result = new List<BoletoPendienteLiquidar>();
            foreach (var suc in sucFilter)
            {
                BoletoService service = new BoletoService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.ListPendiente(idCuenta, idCosecha, fecha, fechaHasta);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/boleto/pendiente/xls", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCosecha = request.Query["IdCosecha"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-365) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            List<BoletoPendienteLiquidar> result = new List<BoletoPendienteLiquidar>();
            foreach (var suc in sucFilter)
            {
                BoletoService service = new BoletoService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.ListPendiente(idCuenta, idCosecha, fecha, fechaHasta);
                result.AddRange(tmpresult);
            }
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("IdSucursal"),
                                                    new DataColumn("Id"),
                                                    new DataColumn("IdCuenta"),
                                                    new DataColumn("NombreCuenta"),
                                                    new DataColumn("IdCosechal"),
                                                    new DataColumn("NombreCosecha"),
                                                    new DataColumn("Precio"),
                                                    new DataColumn("PesoNeto"),
                                                    new DataColumn("PesoLiquidado"),
                                                    new DataColumn("PesoPendienteLiquidar")});

            foreach (var item in result)
            {
                dt.Rows.Add(item.IdSucursal, item.Id, item.IdCuenta, item.NombreCuenta, item.IdCosecha, item.NombreCosecha, item.Precio, item.PesoNeto, item.PesoLiquidado, item.PesoPendienteLiquidar);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {

                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BoletosPendientes.xlsx");
                }
            }
        });

        app.MapGet("/api/cereales/boleto/{id}", (string id, HttpRequest request, HttpResponse response) =>
        {
            string idSucursal = request.Query["IdSucursal"].ToString();
            var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
            Boleto result = new Boleto();
            foreach (var suc in sucFilter)
            {
                BoletoService service = new BoletoService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpResult = service.FindOne(id);
                if (tmpResult != null) result = tmpResult;
            }
            return result;
        });

        app.MapGet("/api/cereales/boleto/total", (HttpRequest request, HttpResponse response) =>
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
                BoletoService service = new BoletoService(suc.ConnectionStrings);
                service.IdSucursal = suc.Id;
                var tmpresult = service.Total(idCuenta, idCosecha, fecha);
                result += tmpresult;
            }
            return result;
        });
    }
}
