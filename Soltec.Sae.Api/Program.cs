using Soltec.Sae.Api;
using System.Data.OleDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;

using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Key Auth", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "ApiKey must appear in header",
        Type = SecuritySchemeType.ApiKey,
        Name = "ApiKey",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
    c.AddSecurityRequirement(requirement);
});
// Configura la compresión para el tipo de contenido "application/json"
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

//builder.Services.AddDbContext<DatabaseContext>();

var app = builder.Build();

// Habilita la compresión de respuesta
app.UseResponseCompression();

var message = app.Configuration["ConnectionStrings"];
IWebHostEnvironment webHostEnvironment = app.Services.GetService<IWebHostEnvironment>();

string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
string connectionStringCerealesBase = app.Configuration["ConnectionStringsCereales"];
string tipoSaldo = app.Configuration["TipoSaldo"];

var sucursales = app.Configuration.GetSection("Sucursales").GetChildren().ToList().Select(x => new Sucursal {
    Id = x.GetValue<string>("Id"),
    Nombre = x.GetValue<string>("Nombre"),
    ConnectionStrings = x.GetValue<string>("ConnectionStrings")
}).ToList();
var seccionDolar = app.Configuration.GetSection("SeccionDolar").GetChildren().ToList().Select(x => new Seccion
{
    Id = x.GetValue<string>("Id"),
    Nombre = x.GetValue<string>("Nombre"),    
}).ToList();
var empresa = new Empresa
{
    Nombre = app.Configuration["Empresa:Nombre"],
    Cuit = app.Configuration["Empresa:CUIT"],
    NumeroIB = app.Configuration["Empresa:NumeroIB"],
    Direccion = app.Configuration["Empresa:Direccion"],
    Localidad = app.Configuration["Empresa:Localidad"],
    Provincia = app.Configuration["Empresa:Provincia"],
    CondIva = app.Configuration["Empresa:CondIva"],
    Telefono = app.Configuration["Empresa:Telefono"],
    Email = app.Configuration["Empresa:Email"]

};

SujetoService sujetoService = new SujetoService(connectionStringBase);
//app.Services.AddTransient<SujetoService,sujetoService>();
//Configure Security
app.UseMiddleware<ApiKeyMiddleware>();

//Errors Manage
app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI() ;
}

app.UseHttpsRedirection();





app.MapGet("/api/contabilidad/sujeto",() =>
{
   
    SujetoService sujetoService = new SujetoService(connectionStringBase);
    List<Sujeto> result = sujetoService.List();     
    return result;
});
app.MapGet("/api/comun/provincia", () =>
{

    ProvinciaService service = new ProvinciaService(connectionStringBase);
    List<EntityGeneric> result = service.List();
    return result;
});
app.MapGet("/api/comun/categoria", () =>
{

    CategoriaService service = new CategoriaService(connectionStringBase);
    List<EntityGeneric> result = service.List();
    return result;
});
app.MapGet("/api/comun/zona", () =>
{

    ZonaService service = new ZonaService(connectionStringBase);
    List<EntityGeneric> result = service.List();
    return result;
});

app.MapGet("/api/comun/isRuning", [AllowAnonymous] () =>
{
    return Results.Ok(true);
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
        dt.Rows.Add(item.Id, item.Nombre, item.NumeroDocumento, item.Domicilio,item.CodigoPostal, item.Localidad,item.Provincia,item.CondicionIva,item.CondicionIB);
    }

    using (XLWorkbook wb = new XLWorkbook())
    {

        wb.Worksheets.Add(dt);
        using (MemoryStream stream = new MemoryStream())
        {
            wb.SaveAs(stream);          
            return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Sujeto.xlsx");
        }
    }
    
});

app.MapGet("/api/contabilidad/sujeto/{id}", (string id) =>
{
    SujetoService sujetoService = new SujetoService(connectionStringBase);
    Sujeto result = sujetoService.FindOne(id);
    return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapGet("/api/almacen/articulo", () =>
{
    ArticuloService service = new ArticuloService(connectionStringBase);
    List<Articulo> result = service.List();
    return result;
});
app.MapGet("/api/almacen/articulo/{id}", (string id) =>
{
    ArticuloService service = new ArticuloService(connectionStringBase);
    Articulo result = service.FindOne(id);
    return result == null ? Results.NotFound() : Results.Ok(result);
});
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

app.MapGet("/api/almacen/articulo/stock", ( HttpRequest request, HttpResponse response) =>
{
    string idArticulo = request.Query["IdArticulo"].ToString();
    string idArticuloHasta = request.Query["IdArticuloHasta"].ToString();
    string idSeccion = request.Query["IdSeccion"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();    
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);

    if (string.IsNullOrEmpty(idArticuloHasta)) idArticuloHasta = idArticulo;
    MovStockService service = new MovStockService(connectionStringBase);
    List<Stock> result = service.ListStock(fecha,idArticulo,idArticuloHasta,idSeccion);
    return result == null ? Results.NotFound() : Results.Ok(result);
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

//Cta Cte
app.MapGet("/api/contabilidad/CtaCte/{id}/saldo", (string id, HttpRequest request, HttpResponse response) =>
{
    string idCuentaMayor = request.Query["IdCuentaMayor"];
    var fechaStr = request.Query["Fecha"].ToString() ;
    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString() ;
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    CtaCteService service = new CtaCteService(connectionStringBase);
    decimal result = 0;    
    result = service.Saldo(id, idCuentaMayor, fecha,idDivisa,vencido);
    return Results.Ok(result);
});
app.MapGet("/api/contabilidad/CtaCte/{id}", (string id, HttpRequest request, HttpResponse response) =>
{
    string idCuentaMayor = request.Query["IdCuentaMayor"];
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-60) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr==""? DateTime.Now:DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);

    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString();
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
    
    CtaCteService service = new CtaCteService(connectionStringBase);
    List<MovCtaCte> result = null;
    result = service.List(id, idCuentaMayor, fecha, fechaHasta, idDivisa);

    return Results.Ok(result);
});
app.MapGet("/api/contabilidad/CtaCte/{id}/pdf", async (string id, HttpRequest request, HttpResponse response) =>
{
    string idCuentaMayor = request.Query["IdCuentaMayor"];
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-60) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);

    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString();
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);

    SujetoService sujetoService = new SujetoService(connectionStringBase);
    Sujeto sujeto = sujetoService.FindOne(id);
    CtaCteService service = new CtaCteService(connectionStringBase);
    List<MovCtaCte> movCtaCte = null;
    movCtaCte = service.List(id, idCuentaMayor, fecha, fechaHasta, idDivisa);
    CtaCteReportTemplate template = new CtaCteReportTemplate();
    template.FechaDesde = fecha;
    template.FechaHasta = fechaHasta;
    template.Sujeto = sujeto;
    template.MovCtaCte = movCtaCte;
    template.Empresa = empresa;
    template.Path = webHostEnvironment.ContentRootPath;
    MemoryStream stream = await template.ListPDF();
    stream.Position = 0;
    return Results.File(stream.ToArray(), "application/pdf", "ResumenCtaCte.pdf");    
    
});

app.MapGet("/api/contabilidad/CtaCte/saldos", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCuentaHasta = request.Query["IdCuentaHasta"].ToString();
    string idCuentaMayor = request.Query["IdCuentaMayor"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString();
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
    //Validar
    if (idCuentaMayor.Trim() == "") 
    {
        return Results.BadRequest("IdCuentaMayor requerido");
    }
    if (idCuentaHasta.Trim() == "")
    {
        idCuentaHasta = "9999999999";
    }
    CtaCteService service = new CtaCteService(connectionStringBase);
    List<SaldoCtaCte> result = null;
    result = service.Saldos(idCuenta,idCuentaHasta, idCuentaMayor, fecha,idDivisa);

    return Results.Ok(result);
});
app.MapGet("/api/contabilidad/CtaCte/saldos/xls", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCuentaHasta = request.Query["IdCuentaHasta"].ToString();
    string idCuentaMayor = request.Query["IdCuentaMayor"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString();
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
    //Validar
    if (idCuentaMayor.Trim() == "")
    {
        return Results.BadRequest("IdCuentaMayor requerido");
    }
    if (idCuentaHasta.Trim() == "")
    {
        idCuentaHasta = "9999999999";
    }
    CtaCteService service = new CtaCteService(connectionStringBase);
    List<SaldoCtaCte> result = null;
    result = service.Saldos(idCuenta, idCuentaHasta, idCuentaMayor, fecha, idDivisa);
    //Convertir a Excel
    DataTable dt = new DataTable("Grid");

    dt.Columns.AddRange(new DataColumn[6] { new DataColumn("IdCuenta"),
                                            new DataColumn("IdCuentaMayor"),
                                            new DataColumn("Nombre"),
                                            new DataColumn("Saldo Vencido"),
                                            new DataColumn("Saldo"),
                                            new DataColumn("idDivisa")});


    foreach (var item in result)
    {
        dt.Rows.Add(item.IdCuenta,item.IdCuentaMayor ,item.Nombre, item.SaldoVencido,item.SaldoVencido,item.IdDivisa);
    }

    using (XLWorkbook wb = new XLWorkbook())
    {

        wb.Worksheets.Add(dt);
        using (MemoryStream stream = new MemoryStream())
        {
            wb.SaveAs(stream);
            return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SaldosCtaCte.xlsx");
        }
    }

    return Results.Ok(result);
});
//Dias Deuda
app.MapGet("/api/contabilidad/CtaCte/{id}/diasdeuda", (string id, HttpRequest request, HttpResponse response) =>
{
    string idCuentaMayor = request.Query["IdCuentaMayor"];
    var fechaStr = request.Query["Fecha"].ToString();
    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString();
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    CtaCteService service = new CtaCteService(connectionStringBase);
    Int32 result = 0;
    result = service.DiasDeuda(id, idCuentaMayor);
    return Results.Ok(result);
});

//Recbos de Cta. Cte.
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

app.MapGet("/api/ventas/Seccion", (HttpRequest request, HttpResponse response) =>
{   
    SeccionService service = new SeccionService(connectionStringBase);
    List<Seccion> result = null;
    result = service.List();
    return Results.Ok(result);
});
app.MapGet("/api/ventas/ClaseVenta", (HttpRequest request, HttpResponse response) =>
{
    ClaseVentaService service = new ClaseVentaService(connectionStringBase);
    List<EntityGeneric> result = null;
    result = service.List();
    return Results.Ok(result);
});
app.MapGet("/api/ventas/Campania", (HttpRequest request, HttpResponse response) =>
{
    CampaniaService service = new CampaniaService(connectionStringBase);
    List<EntityGeneric> result = null;
    result = service.List();
    return Results.Ok(result);
});

app.MapGet("/api/ventas/Factura", (HttpRequest request, HttpResponse response) =>
{
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
    FacturaService service = new FacturaService(connectionStringBase);
    service.SeccionDolar = seccionDolar;
    List<Factura> result = null;
    result = service.List(fecha,fechaHasta);
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

    //FacturaService service = new FacturaService(connectionStringBase);    
    //List<FacturaView> result = null;
    // Define una clave única para el caché, basada en los parámetros de la solicitud
    string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

    // Intenta obtener los datos desde el caché
    if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
    {
        // Si no se encuentra en caché, realiza la consulta y almacena el resultado en caché durante un tiempo específico
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300) // Ajusta el tiempo de expiración según tus necesidades
        };

        FacturaService service = new FacturaService(connectionStringBase);
        service.SeccionDolar = seccionDolar;
        result = service.ListView(fecha, fechaHasta);

        // Almacena los resultados en caché
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

    // Define una clave única para el caché, basada en los parámetros de la solicitud
    string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

    // Intenta obtener los datos desde el caché
    if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
    {
        // Si no se encuentra en caché, realiza la consulta y almacena el resultado en caché durante un tiempo específico
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300) // Ajusta el tiempo de expiración según tus necesidades
        };

        FacturaService service = new FacturaService(connectionStringBase);
        service.SeccionDolar = seccionDolar;
        result = service.ListView(fecha, fechaHasta);

        // Almacena los resultados en caché
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
                     item.Tipo,item.Letra,item.Pe,item.Numero,item.Comprobante,item.IdDivisa,item.Cotizacion,
                     item.IdCuenta,item.NombreCuenta,item.Obs,item.SubTotal,item.Descuento,item.IvaGeneral,
                     item.IvaOtro,item.ImpuestoInterno,item.PercepcionIva,item.PercepcionIB,item.Total,
                     item.Cae,item.Remito,item.CondVenta,item.TipoComp,item.IdClaseVenta,item.IdCampania,
                     item.Item,item.IdArticulo,item.Concepto,item.Precio,item.AlicuotaIva,item.Iva,item.ImpuestoInternoItem,
                     item.Cantidad,item.UnidadMedida,item.SubTotalItem,item.Bonificacion,item.IdRemito);
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

    // Define una clave única para el caché, basada en los parámetros de la solicitud
    string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

    // Intenta obtener los datos desde el caché
    if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
    {
        // Si no se encuentra en caché, realiza la consulta y almacena el resultado en caché durante un tiempo específico
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300) // Ajusta el tiempo de expiración según tus necesidades
        };

        FacturaService service = new FacturaService(connectionStringBase);
        service.SeccionDolar = seccionDolar;
        result = service.ListView(fecha, fechaHasta);

        // Almacena los resultados en caché
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

    // Define una clave única para el caché, basada en los parámetros de la solicitud
    string cacheKey = $"FacturaView_{fecha.ToShortDateString()}_{fechaHasta.ToShortDateString()}";

    // Intenta obtener los datos desde el caché
    if (!cache.TryGetValue(cacheKey, out List<FacturaView> result))
    {
        // Si no se encuentra en caché, realiza la consulta y almacena el resultado en caché durante un tiempo específico
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300) // Ajusta el tiempo de expiración según tus necesidades
        };

        FacturaService service = new FacturaService(connectionStringBase);
        service.SeccionDolar = seccionDolar;
        result = service.ListView(fecha, fechaHasta);

        // Almacena los resultados en caché
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
    result = service.FindOne(sec,orden);
    return Results.Ok(result);
});
//Remito
app.MapGet("/api/ventas/Remito", (HttpRequest request, HttpResponse response) =>
{
    var fechaStr = request.Query["Fecha"].ToString();
    var incluyeDetalleStr = request.Query["IncluyeDetalle"].ToString();
    var incluyeDetalle = incluyeDetalleStr == "" ? false : Convert.ToBoolean(incluyeDetalleStr);
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
    RemitoService service = new RemitoService(connectionStringBase);
    List<Remito> result = null;
    result = service.List(fecha, fechaHasta);
    return Results.Ok(result);
});
app.MapGet("/api/ventas/Remito/pendiente", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();    
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
    RemitoService service = new RemitoService(connectionStringBase);
    List<DocumentoPendienteView> result = null;
    result = service.ListPendiente(idCuenta,fecha, fechaHasta);
    return Results.Ok(result);
});

app.MapGet("/api/ventas/remito/informe", (HttpRequest request, HttpResponse response) =>
{
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-365) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
    RemitoService service = new RemitoService(connectionStringBase);
    List<RemitoView> result = null;
    result = service.ListInforme(fecha, fechaHasta);
    return Results.Ok(result);
});
app.MapGet("/api/ventas/Remito/{orden}", (string orden, HttpRequest request, HttpResponse response) =>
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

    RemitoService service = new RemitoService(connectionStringBase);
    Remito result = null;
    result = service.FindOne(sec, orden);
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
                    item.IdComprobante, item.Pe, item.Numero,item.Origen,item.IdCuentaMayor, item.NombreCuentaMayor, item.IdCuenta,item.NombreSujeto,item.IdTipo,item.Debe,item.Haber,item.Cantidad);
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
    result = service.List(idCuenta,fecha, fechaHasta);
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




//Cereal
//Cosechas
app.MapGet("/api/cereales/cosecha", (HttpRequest request, HttpResponse response) =>
{    
    string idSucursal = request.Query["IdSucursal"].ToString();
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    List<Cosecha> result = new List<Cosecha>();
    foreach (var suc in sucFilter)
    {
        CosechaService service = new CosechaService(suc.ConnectionStrings);        
        result= service.List();        
    }
    return result;
});
app.MapGet("/api/cereales/cosecha/{id}", (string id,HttpRequest request, HttpResponse response) =>
{
    string idSucursal = request.Query["IdSucursal"].ToString();
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    Cosecha result = new Cosecha();
    foreach (var suc in sucFilter)
    {
        CosechaService service = new CosechaService(suc.ConnectionStrings);
        result = service.FindOne(id);
    }
    return result;
});

//Entradas
app.MapGet("/api/cereales/entrada/{id}", (string id, HttpRequest request, HttpResponse response) =>
{    
    EntradaService service = new EntradaService(connectionStringCerealesBase);
    Entrada result = service.FindOne(id);
    return result;
});
app.MapGet("/api/cereales/entrada", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCosecha = request.Query["IdCosecha"].ToString();
    string idSucursal = request.Query["IdSucursal"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    List<Entrada> result = new List<Entrada>();
    foreach (var suc in sucFilter) 
    {
        EntradaService service = new EntradaService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpResult = service.List(idCuenta, idCosecha,fecha,fechaHasta);
        result.AddRange(tmpResult);
    }   
    return result;
});
app.MapGet("/api/cereales/entrada/total", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCosecha = request.Query["IdCosecha"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    EntradaService service = new EntradaService(connectionStringCerealesBase);
    Int64 result = service.Total(idCuenta, idCosecha,fecha);
    return result;
});
//Certificado
app.MapGet("/api/cereales/certificado", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCosecha = request.Query["IdCosecha"].ToString();
    string idSucursal = request.Query["IdSucursal"].ToString();
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    List<Certificado> result = new List<Certificado>();
    foreach (var suc in sucFilter)
    {
        CertificadoService service = new CertificadoService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpresult = service.List(idCuenta, idCosecha,fecha);
        result.AddRange(tmpresult);
    }
    return result;
});
app.MapGet("/api/cereales/certificado/{id}", (string id,HttpRequest request, HttpResponse response) =>
{
    string idSucursal = request.Query["IdSucursal"].ToString();
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    Certificado result = new Certificado();
    foreach (var suc in sucFilter)
    {
        CertificadoService service = new CertificadoService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpResult = service.FindOne(id);
        if (tmpResult != null) result = tmpResult;
    }
    return result;
});
app.MapGet("/api/cereales/certificado/total", (HttpRequest request, HttpResponse response) =>
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
        CertificadoService service = new CertificadoService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpresult = service.Total(idCuenta, idCosecha, fecha);
        result += tmpresult;
    }
    return result;
});

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
        var tmpresult = service.List(idCuenta, idCosecha,fecha);
        result.AddRange(tmpresult);
    }
    return result;
});
app.MapGet("/api/cereales/liquidacion/{id}", (string id,HttpRequest request, HttpResponse response) =>
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
        var tmpresult = service.Total(idCuenta, idCosecha,fecha);
        result += tmpresult;
    }
    return result;
});
//Liquidacion Secundaria
//Liquidaciones
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
        var tmpresult = service.List(idCosecha,DateTime.Now);
        result.AddRange(tmpresult);
    }
    return result;
});


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
        var tmpresult = service.List(idCuenta, idCosecha,fecha);
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
        var tmpresult = service.List(idCosecha,idPlanta, fecha,fechaHasta);
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
        var tmpresult = service.List(idCuenta, idCosecha,fecha);
        result.AddRange(tmpresult);
    }
    return result;
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
        var tmpresult = service.ListPendiente(idCuenta, idCosecha, fecha,fechaHasta);
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
        dt.Rows.Add(item.IdSucursal,item.Id, item.IdCuenta, item.NombreCuenta,item.IdCosecha, item.NombreCosecha,item.Precio,item.PesoNeto,item.PesoLiquidado,item.PesoPendienteLiquidar);
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

//RT
app.MapGet("/api/cereales/rt", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCuentaDestino = request.Query["IdCuentaDestino"].ToString();
    string idCosecha = request.Query["IdCosecha"].ToString();
    string idSucursal = request.Query["IdSucursal"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    List<Rt> result = new List<Rt>();
    foreach (var suc in sucFilter)
    {
        RTService service = new RTService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpresult = service.List(idCuenta,idCuentaDestino, idCosecha,fecha);
        result.AddRange(tmpresult);
    }
    return result;
});
app.MapGet("/api/cereales/rt/totaltransferido", (HttpRequest request, HttpResponse response) =>
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
        RTService service = new RTService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpresult = service.TotalTransferido(idCuenta, idCosecha, fecha);
        result += tmpresult;
    }
    return result;
});
app.MapGet("/api/cereales/rt/totalrecibido", (HttpRequest request, HttpResponse response) =>
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
        RTService service = new RTService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpresult = service.TotalRecibido(idCuenta, idCosecha, fecha);
        result += tmpresult;
    }
    return result;
});





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
        var tmpresult = service.List(idCuenta, idCosecha,fecha);
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
        var tmpresult = service.Saldos2(idCuenta, idCosecha, fecha);
        result.AddRange(tmpresult);
    }
    return result;
});
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
        var tmpresult = service.C14(idPlanta, idCosecha, fecha,fechaHasta);
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
//Contrato
app.MapGet("/api/cereales/Contrato", (HttpRequest request, HttpResponse response) =>
{
    string numero = request.Query["numero"].ToString();
    string tipo = request.Query["tipo"].ToString();
    string estado = request.Query["estado"].ToString();
    string id = request.Query["id"].ToString();
    string idCosecha = request.Query["IdCosecha"].ToString();
    string idSucursal = request.Query["IdSucursal"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-355) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    List<Contrato> result = new List<Contrato>();
    foreach (var suc in sucFilter)
    {
        ContratoService service = new ContratoService(suc.ConnectionStrings);        
        var tmpresult = service.List(id,numero,fecha,fechaHasta,tipo,estado);
        result.AddRange(tmpresult);
    }
    return result;
});

app.MapGet("/api/cereales/Contrato/estado", (HttpRequest request, HttpResponse response) =>
{
    string numero = request.Query["numero"].ToString();
    string tipo = request.Query["tipo"].ToString();
    string estado = request.Query["estado"].ToString();
    string id = request.Query["id"].ToString();
    string idCosecha = request.Query["IdCosecha"].ToString();
    string idSucursal = request.Query["IdSucursal"].ToString();
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-355) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    List<EstadoContratoView> result = new List<EstadoContratoView>();
    foreach (var suc in sucFilter)
    {
        ContratoService service = new ContratoService(suc.ConnectionStrings);
        var tmpresult = service.ListEstado(id, numero,fecha,fechaHasta,estado,tipo);
        result.AddRange(tmpresult);
    }
    return result;
});

app.MapGet("/api/cereales/condicionventa", () =>
{

    CondicionVentaCerealService service = new CondicionVentaCerealService(connectionStringCerealesBase);
    List<EntityGeneric> result = service.List();
    return result;
});
app.MapGet("/api/cereales/localidad", () =>
{

    LocalidadService service = new LocalidadService(connectionStringCerealesBase);
    List<Localidad> result = service.List();
    return result;
});



app.Run();





