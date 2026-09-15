using System.Data;
using System.Text;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Caching.Memory;
using Soltec.Sae.Api;

public static class FacturaEndpoints
{
    public static void MapFacturaEndpoints(this WebApplication app)
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
        var seccionDolar = app.Configuration.GetSection("SeccionDolar").GetChildren().ToList().Select(x => new Seccion
        {
            Id = x.GetValue<string>("Id"),
            Nombre = x.GetValue<string>("Nombre"),
        }).ToList();
        var seccionPendiente = app.Configuration.GetSection("SeccionPendiente").GetChildren().ToList().Select(x => new Seccion
        {
            Id = x.GetValue<string>("Id"),
            Nombre = x.GetValue<string>("Nombre"),
        }).ToList();

        app.MapGet("/api/ventas/Factura", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            FacturaService service = new FacturaService(connectionStringBase);
            service.SeccionDolar = seccionDolar;
            List<Factura> result = null;
            result = service.List(fecha, fechaHasta, idCuenta);
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/Factura/view", (HttpRequest request, HttpResponse response, IMemoryCache cache) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var diasStr = request.Query["Dias"].ToString();
            int dias = 730;
            if (!string.IsNullOrEmpty(diasStr))
                dias = Convert.ToInt32(diasStr);

            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-dias) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);

            string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

            if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300)
                };

                FacturaService service = new FacturaService(connectionStringBase);
                service.SeccionDolar = seccionDolar;
                result = service.ListView(fecha, fechaHasta);

                cache.Set(cacheKey, result, cacheEntryOptions);
            }
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/Factura/view/xls", (HttpRequest request, HttpResponse response, IMemoryCache cache) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var diasStr = request.Query["Dias"].ToString();
            int dias = 730;
            if (!string.IsNullOrEmpty(diasStr))
                dias = Convert.ToInt32(diasStr);

            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-dias) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);

            string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

            if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300)
                };

                FacturaService service = new FacturaService(connectionStringBase);
                service.SeccionDolar = seccionDolar;
                result = service.ListView(fecha, fechaHasta);

                cache.Set(cacheKey, result, cacheEntryOptions);
            }
            //Convertir a Excel
            DataTable dt = new DataTable("Grid");

            dt.Columns.AddRange(new DataColumn[41] { new DataColumn("Sec"),
                                                    new DataColumn("Orden"),
                                                    new DataColumn("FechaPase"),
                                                    new DataColumn("FechaComprobante"),
                                                    new DataColumn("FechaVencimiento"),
                                                    new DataColumn("Tipo"),
                                                    new DataColumn("Letra"),
                                                    new DataColumn("Pe"),
                                                    new DataColumn("Numero"),
                                                    new DataColumn("Comprobante"),
                                                    new DataColumn("IdDivisa"),
                                                    new DataColumn("Cotizacion"),
                                                    new DataColumn("IdCuenta"),
                                                    new DataColumn("NombreCuenta"),
                                                    new DataColumn("Obs"),
                                                    new DataColumn("SubTotal"),
                                                    new DataColumn("Descuento"),
                                                    new DataColumn("IvaGeneral"),
                                                    new DataColumn("IvaOtro"),
                                                    new DataColumn("ImpuestoInterno"),
                                                    new DataColumn("PercepcionIva"),
                                                    new DataColumn("PercepcionIB"),
                                                    new DataColumn("Total"),
                                                    new DataColumn("Cae"),
                                                    new DataColumn("Remito"),
                                                    new DataColumn("CondVenta"),
                                                    new DataColumn("TipoComp"),
                                                    new DataColumn("IdClaseVenta"),
                                                    new DataColumn("IdCampania"),
                                                    new DataColumn("Item"),
                                                    new DataColumn("IdArticulo"),
                                                    new DataColumn("Concepto"),
                                                    new DataColumn("Precio"),
                                                    new DataColumn("AlicuotaIva"),
                                                    new DataColumn("Iva"),
                                                    new DataColumn("ImpuestoInternoItem"),
                                                    new DataColumn("Cantidad"),
                                                    new DataColumn("UnidadMedida"),
                                                    new DataColumn("SubTotalItem"),
                                                    new DataColumn("Bonificacion"),
                                                    new DataColumn("IdRemito")
                                                    });

            int i = 0;
            foreach (var item in result)
            {
                i += 1;
                dt.Rows.Add(item.Sec, item.Orden, item.FechaPase, item.FechaComprobante, item.FechaVencimiento,
                             item.Tipo, item.Letra, item.Pe, item.Numero, item.Comprobante, item.IdDivisa, item.Cotizacion,
                             item.IdCuenta, item.NombreCuenta, item.Obs, item.SubTotal, item.Descuento, item.IvaGeneral,
                             item.IvaOtro, item.ImpuestoInterno, item.PercepcionIva, item.PercepcionIB, item.Total,
                             item.Cae, item.Remito, item.CondVenta, item.TipoComp, item.IdClaseVenta, item.IdCampania,
                             item.Item, item.IdArticulo, item.Concepto, item.Precio, item.AlicuotaIva, item.Iva, item.ImpuestoInternoItem,
                             item.Cantidad, item.UnidadMedida, item.SubTotalItem, item.Bonificacion, item.IdRemito);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {

                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FacturasVenta.xlsx");
                }
            }
        });

        app.MapGet("/api/ventas/Factura/view/xls1", (HttpRequest request, HttpResponse response, IMemoryCache cache) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var diasStr = request.Query["Dias"].ToString();
            int dias = 730;
            if (!string.IsNullOrEmpty(diasStr))
                dias = Convert.ToInt32(diasStr);

            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-dias) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);

            string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

            if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300)
                };

                FacturaService service = new FacturaService(connectionStringBase);
                service.SeccionDolar = seccionDolar;
                result = service.ListView(fecha, fechaHasta);

                cache.Set(cacheKey, result, cacheEntryOptions);
            }

            // Crear un nuevo archivo Excel
            using (MemoryStream stream = new MemoryStream())
            {
                // Crear el documento Excel
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    // Crear una hoja de cálculo en el libro de trabajo
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                    sheets.Append(sheet);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Encabezados de columna
                    Row headerRow = new Row();
                    foreach (var prop in typeof(FacturaView).GetProperties())
                    {
                        Cell headerCell = new Cell(new CellValue(prop.Name))
                        {
                            DataType = CellValues.String
                        };
                        headerRow.AppendChild(headerCell);
                    }
                    sheetData.AppendChild(headerRow);

                    // Datos de la factura
                    foreach (var item in result)
                    {
                        Row excelRow = new Row();
                        foreach (var prop in typeof(FacturaView).GetProperties())
                        {
                            Cell cell = new Cell(new CellValue(prop.GetValue(item)?.ToString()))
                            {
                                DataType = CellValues.String
                            };
                            excelRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(excelRow);
                    }
                }

                // Configurar la respuesta HTTP para el archivo Excel
                response.Headers.Add("Content-Disposition", "attachment; filename=FacturasVenta.xlsx");
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Escribir el archivo Excel en la respuesta
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(response.Body);
            }

        });

        // Tu endpoint API modificado para devolver un archivo CSV
        app.MapGet("/api/ventas/Factura/view/csv", (HttpRequest request, HttpResponse response, IMemoryCache cache) =>
        {
            var fechaStr = request.Query["Fecha"].ToString();
            var diasStr = request.Query["Dias"].ToString();
            int dias = 730;
            if (!string.IsNullOrEmpty(diasStr))
                dias = Convert.ToInt32(diasStr);

            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-dias) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);

            string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

            if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300)
                };

                FacturaService service = new FacturaService(connectionStringBase);
                service.SeccionDolar = seccionDolar;
                result = service.ListView(fecha, fechaHasta);

                cache.Set(cacheKey, result, cacheEntryOptions);
            }

            // Configurar la respuesta HTTP para devolver un archivo CSV
            response.Headers.Add("Content-Type", "text/csv");
            response.Headers.Add("Content-Disposition", "attachment; filename=FacturasVenta.csv");

            // Crear el contenido CSV manualmente
            var csvContent = new StringBuilder();

            // Agregar encabezados de columna
            csvContent.AppendLine("Sec,Orden,FechaPase,FechaComprobante,FechaVencimiento,Tipo,Letra,Pe,Numero,Comprobante,IdDivisa,Cotizacion,IdCuenta,NombreCuenta,Obs,SubTotal,Descuento,IvaGeneral,IvaOtro,ImpuestoInterno,PercepcionIva,PercepcionIB,Total,Cae,Remito,CondVenta,TipoComp,IdClaseVenta,IdCampania,Item,IdArticulo,Concepto,Precio,AlicuotaIva,Iva,ImpuestoInternoItem,Cantidad,UnidadMedida,SubTotalItem,Bonificacion,IdRemito");

            // Agregar datos de las filas
            foreach (var item in result)
            {
                csvContent.AppendLine($"{item.Sec},{item.Orden},{item.FechaPase},{item.FechaComprobante},{item.FechaVencimiento},{item.Tipo},{item.Letra},{item.Pe},{item.Numero},{item.Comprobante},{item.IdDivisa},{item.Cotizacion},{item.IdCuenta},{item.NombreCuenta},{item.Obs},{item.SubTotal},{item.Descuento},{item.IvaGeneral},{item.IvaOtro},{item.ImpuestoInterno},{item.PercepcionIva},{item.PercepcionIB},{item.Total},{item.Cae},{item.Remito},{item.CondVenta},{item.TipoComp},{item.IdClaseVenta},{item.IdCampania},{item.Item},{item.IdArticulo},{item.Concepto},{item.Precio},{item.AlicuotaIva},{item.Iva},{item.ImpuestoInternoItem},{item.Cantidad},{item.UnidadMedida},{item.SubTotalItem},{item.Bonificacion},{item.IdRemito}");
            }

            // Escribir el contenido CSV en el cuerpo de la respuesta
            byte[] csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
            response.Body.Write(csvBytes, 0, csvBytes.Length);

            return Results.Ok();
        });

        app.MapGet("/api/ventas/Factura/pendiente", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaHastaStr = request.Query["FechaHasta"].ToString();
            var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
            FacturaService service = new FacturaService(connectionStringBase);
            List<DocumentoPendienteView> result = null;
            service.SeccionPendiente = seccionPendiente;
            result = service.ListPendiente(idCuenta, fecha, fechaHasta);
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/Factura/{orden}", (string orden, HttpRequest request, HttpResponse response) =>
        {
            string sec = request.Query["sec"];
            if (sec.Trim() == "")
            {
                return Results.BadRequest("Parametro Sec requerido");
            }
            if (orden.Trim() == "")
            {
                return Results.BadRequest("Parametro Orden requerido");
            }

            FacturaService service = new FacturaService(connectionStringBase);
            Factura result = null;
            result = service.FindOne(sec, orden);
            return Results.Ok(result);
        });

        app.MapGet("/api/ventas/factura/trx/{ntra}", async (string ntra, HttpRequest request, HttpResponse response) =>
        {
            FacturaService service = new FacturaService(connectionStringBase);
            service.SeccionDolar = seccionDolar;
            Factura entity = null;

            entity = service.FindByNTra(ntra);
            if (entity == null)
            {
                return Results.NotFound("Registro no encontrado");
            }
            return Results.Ok(entity);
        });

        app.MapGet("/api/ventas/factura/trx/{ntra}/pdf", async (string ntra, HttpRequest request, HttpResponse response) =>
        {
            FacturaService service = new FacturaService(connectionStringBase);
            service.SeccionDolar = seccionDolar;
            Factura entity = null;

            entity = service.FindByNTra(ntra);
            if (entity == null)
            {
                return Results.NotFound("Registro no encontrado");
            }
            SujetoService sujetoService = new SujetoService(connectionStringBase);
            var sujeto = sujetoService.FindOne(entity.IdCuenta);
            FacturaTemplate template = new FacturaTemplate();
            template.Entity = entity;
            template.Empresa = empresa;
            template.Sujeto = sujeto;
            template.Path = webHostEnvironment.ContentRootPath;
            template.SeccionDolar = seccionDolar;
            MemoryStream stream = await template.ToPDF();
            stream.Position = 0;
            string filename = "Factura_" + entity.Cae.ToString() + ".pdf";
            return Results.File(stream.ToArray(), "application/pdf", filename);
        });
    }
}
