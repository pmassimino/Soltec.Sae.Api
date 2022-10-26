using Soltec.Sae.Api;
using System.Data.OleDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var message = app.Configuration["ConnectionStrings"];

string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
string connectionStringCerealesBase = app.Configuration["ConnectionStringsCereales"];
string tipoSaldo = app.Configuration["TipoSaldo"];

var sucursales = app.Configuration.GetSection("Sucursales").GetChildren().ToList().Select(x => new {
    Id = x.GetValue<string>("Id"),
    Nombre = x.GetValue<string>("Nombre"),
    ConnectionStrings = x.GetValue<string>("ConnectionStrings")
}).ToList();


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


app.MapGet("/api/contabilidad/sujeto", () =>
{
   
    SujetoService sujetoService = new SujetoService(connectionStringBase);
    List<Sujeto> result = sujetoService.List();     
    return result;
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
//Cta Cte
app.MapGet("/api/contabilidad/CtaCte/{id}/saldo", (string id, HttpRequest request, HttpResponse response) =>
{
    string idCuentaMayor = request.Query["IdCuentaMayor"];
    var fechaStr = request.Query["Fecha"] ;
    string vencidoStr = request.Query["vencido"].ToString();
    bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
    string idDivisaStr = request.Query["idDivisa"].ToString() ;
    int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
    var fecha = DateTime.ParseExact(fechaStr,"MM-dd-yyyy",null);
    CtaCteService service = new CtaCteService(connectionStringBase);
    decimal result = 0;    
    result = service.Saldo(id, idCuentaMayor, fecha,idDivisa,vencido);
    return Results.Ok(result);
});
app.MapGet("/api/contabilidad/CtaCte/{id}", (string id, HttpRequest request, HttpResponse response) =>
{
    string idCuentaMayor = request.Query["IdCuentaMayor"];
    var fechaStr = request.Query["Fecha"];
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
app.MapGet("/api/contabilidad/CtaCte/saldos", (HttpRequest request, HttpResponse response) =>
{
    string idCuenta = request.Query["IdCuenta"].ToString();
    string idCuentaHasta = request.Query["IdCuentaHasta"].ToString();
    string idCuentaMayor = request.Query["IdCuentaMayor"].ToString();
    var fechaStr = request.Query["Fecha"];
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
        return Results.BadRequest("IdCuentaHasta requerido");
    }
    CtaCteService service = new CtaCteService(connectionStringBase);
    List<SaldoCtaCte> result = null;
    result = service.Saldos(idCuenta,idCuentaHasta, idCuentaMayor, fecha,idDivisa,vencido);

    return Results.Ok(result);
});

app.MapGet("/api/ventas/Factura", (HttpRequest request, HttpResponse response) =>
{
    var fechaStr = request.Query["Fecha"].ToString();
    var fecha = fechaStr == "" ? DateTime.Now.AddDays(-530) : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
    var fechaHastaStr = request.Query["FechaHasta"].ToString();
    var fechaHasta = fechaHastaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaHastaStr, "MM-dd-yyyy", null);
    FacturaService service = new FacturaService(connectionStringBase);
    List<Factura> result = null;
    result = service.List(fecha,fechaHasta);
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
    var sucFilter = sucursales.Where(w => w.Id == idSucursal || idSucursal == "");
    List<Entrada> result = new List<Entrada>();
    foreach (var suc in sucFilter) 
    {
        EntradaService service = new EntradaService(suc.ConnectionStrings);
        service.IdSucursal = suc.Id;
        var tmpResult = service.List(idCuenta, idCosecha,fecha);
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
        var tmpresult = service.Saldos(idCuenta, idCosecha, fecha);
        result.AddRange(tmpresult);
    }
    return result;
});
//Planta
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






app.Run();

