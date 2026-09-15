using System.Data;
using ClosedXML.Excel;
using Soltec.Sae.Api;

public static class ContabilidadEndpoints
{
    public static void MapContabilidadEndpoints(this WebApplication app)
    {
        string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
        IWebHostEnvironment webHostEnvironment = app.Services.GetService<IWebHostEnvironment>();
        var empresa = new Empresa
        {
            Nombre = app.Configuration["Empresa:Nombre"],
            Cuit = app.Configuration["Empresa:CUIT"],
            NumeroIB = app.Configuration["Empresa:NumeroIB"],
            Direccion = app.Configuration["Empresa:Direccion"],
            Cpostal = app.Configuration["Empresa:CodigoPostal"],
            Localidad = app.Configuration["Empresa:Localidad"],
            Provincia = app.Configuration["Empresa:Provincia"],
            CondIva = app.Configuration["Empresa:CondIva"],
            Telefono = app.Configuration["Empresa:Telefono"],
            Email = app.Configuration["Empresa:Email"],
            FechaIniAct = app.Configuration["Empresa:FechaIniAct"]
        };

        app.MapGet("/api/contabilidad/ReciboCtaCte", (HttpRequest request, HttpResponse response) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-365) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            ReciboCtaCteService service = new ReciboCtaCteService(connectionStringBase);
            List<ReciboCtaCte> result = null;
            result = service.List(fecha, fechaHasta);
            return Results.Ok(result);
        });

        //Contabilidad
        app.MapGet("/api/contabilidad/mayor", (HttpRequest request, HttpResponse response) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            MayorService service = new MayorService(connectionStringBase);
            List<Mayor> result = null;
            result = service.List(fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/contabilidad/diario", (HttpRequest request, HttpResponse response) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            MayorService service = new MayorService(connectionStringBase);
            List<Diario> result = null;
            result = service.ListDiario(fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/contabilidad/diario/xls", (HttpRequest request, HttpResponse response) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            MayorService service = new MayorService(connectionStringBase);
            List<Diario> result = null;
            result = service.ListDiario(fecha, fechaHasta);
            DataTable dt = new DataTable("Diario");
            dt.Columns.AddRange(new DataColumn[19] { new DataColumn("IdSucursal"),
                                                    new DataColumn("IdSeccion"),
                                                    new DataColumn("IdTransaccion"),
                                                    new DataColumn("Fecha"),
                                                    new DataColumn("FechaComprobante"),
                                                    new DataColumn("FechaVencimiento"),
                                                    new DataColumn("Concepto"),
                                                    new DataColumn("IdComprobante"),
                                                    new DataColumn("Pe"),
                                                    new DataColumn("Numero"),
                                                    new DataColumn("Origen"),
                                                    new DataColumn("IdCuentaMayor"),
                                                    new DataColumn("NombreCuentaMayor"),
                                                    new DataColumn("IdCuenta"),
                                                    new DataColumn("NombreSujeto"),
                                                    new DataColumn("IdTipo"),
                                                    new DataColumn("Debe"),
                                                    new DataColumn("Haber"),
                                                    new DataColumn("Cantidad")});

            foreach (var item in result)
            {
                dt.Rows.Add(item.IdSucursal, item.IdSeccion, item.IdTransaccion, item.Fecha, item.FechaComprobante, item.FechaVencimiento, item.Concepto,
                            item.IdComprobante, item.Pe, item.Numero, item.Origen, item.IdCuentaMayor, item.NombreCuentaMayor, item.IdCuenta, item.NombreSujeto, item.IdTipo, item.Debe, item.Haber, item.Cantidad);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {

                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Diario.xlsx");
                }
            }
        });

        //Contabilidad
        app.MapGet("/api/contabilidad/librobanco", (HttpRequest request, HttpResponse response) =>
        {
            string idCuentaMayor = request.Query["IdCuentaMayor"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            // 1. Intentamos obtener el valor de "FiltraConciliado". Si no existe o no es un bool válido, por defecto será 'false'.
            bool filtraConciliado = false;
            if (request.Query.TryGetValue("FiltraConciliado", out var filtraQueryValue))
            {
                bool.TryParse(filtraQueryValue, out filtraConciliado);
            }

            // 2. Intentamos obtener el valor de "Conciliado". Si no existe o no es un bool válido, por defecto será 'false'.
            bool conciliado = false;
            if (request.Query.TryGetValue("Conciliado", out var conciliadoQueryValue))
            {
                bool.TryParse(conciliadoQueryValue, out conciliado);
            }
            LibroBancoService service = new LibroBancoService(connectionStringBase);
            List<LibroBanco> result = null;
            result = service.List(fecha, fechaHasta, idCuentaMayor, filtraConciliado, conciliado);
            return Results.Ok(result);
        });

        //Retenciones
        app.MapGet("/api/contabilidad/retencion/afip", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            RetencionAFIPService service = new RetencionAFIPService(connectionStringBase);
            List<RetencionBase> result = null;
            result = service.List(idCuenta, fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/contabilidad/retencion/afip/{id}/pdf", async (string id, HttpRequest request, HttpResponse response) =>
        {
            RetencionAFIPService service = new RetencionAFIPService(connectionStringBase);
            RetencionBase entity = null;
            entity = service.FindOne(id);
            if (entity == null)
            {
                return Results.NotFound("Registro no encontrado");
            }
            RetencionAFIPTemplate template = new RetencionAFIPTemplate();
            template.Entity = entity;
            template.Empresa = empresa;
            template.Path = webHostEnvironment.ContentRootPath;
            MemoryStream stream = await template.ToPDF();
            stream.Position = 0;
            return Results.File(stream.ToArray(), "application/pdf", "RetencionAFIP.pdf");
        });

        app.MapGet("/api/contabilidad/retencion/dgr/{id}/pdf", async (string id, HttpRequest request, HttpResponse response) =>
        {
            RetencionDGRService service = new RetencionDGRService(connectionStringBase);
            RetencionBase entity = null;
            entity = service.FindOne(id);
            if (entity == null)
            {
                return Results.NotFound("Registro no encontrado");
            }
            RetencionDGRTemplate template = new RetencionDGRTemplate();
            template.Entity = entity;
            template.Empresa = empresa;
            template.Path = webHostEnvironment.ContentRootPath;
            MemoryStream stream = await template.ToPDF();
            stream.Position = 0;
            return Results.File(stream.ToArray(), "application/pdf", "RetencionDGR.pdf");
        });

        //Retenciones
        app.MapGet("/api/contabilidad/retencion/dgr", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            RetencionDGRService service = new RetencionDGRService(connectionStringBase);
            List<RetencionBase> result = null;
            result = service.List(idCuenta, fecha, fechaHasta);
            return Results.Ok(result);
        });
    }
}
