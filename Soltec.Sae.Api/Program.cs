using Soltec.Sae.Api;
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
using DocumentFormat.OpenXml.InkML;
using Soltec.Sae.Api.Soltec.Sae.Api;
using System.Text.Json;

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
//Cors
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()              
              .WithExposedHeaders("Content-Disposition", "Content-Length"); // si devuelves archivos
    });
});


// Configura la compresi�n para el tipo de contenido "application/json"
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

//builder.Services.AddDbContext<DatabaseContext>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<CerealesTools>()
    .WithTools<SujetosTools>()
    .WithTools<FacturaTools>()
    .WithTools<PedidoTools>()
    .WithTools<ArticuloTools>()
    .WithTools<LiquidacionTools>()
    .WithTools<CertificadoTools>()
    .WithTools<MovPlantaCerealTools>()
    .WithTools<CosechaTools>();

var sucursales = builder.Configuration.GetSection("Sucursales").GetChildren().ToList().Select(x => new Sucursal {
    Id = x.GetValue<string>("Id"),
    Nombre = x.GetValue<string>("Nombre"),
    ConnectionStrings = x.GetValue<string>("ConnectionStrings")
}).ToList();
builder.Services.AddSingleton(sucursales);


var app = builder.Build();
app.MapMcp("/mcp"); 
// Habilita la compresi�n de respuesta
//app.UseResponseCompression();

var message = app.Configuration["ConnectionStrings"];
IWebHostEnvironment webHostEnvironment = app.Services.GetService<IWebHostEnvironment>();

string connectionStringBase = app.Configuration["ConnectionStringsSAE"];
string connectionStringCerealesBase = app.Configuration["ConnectionStringsCereales"];
string tipoSaldo = app.Configuration["TipoSaldo"];

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

SujetoService sujetoService = new SujetoService(connectionStringBase);
//app.Services.AddTransient<SujetoService,sujetoService>();

//Errors Manage
//app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI() ;
}
// 2. ORDEN CRUCIAL DE MIDDLEWARES
app.UseHttpsRedirection();
//app.UseMiddleware<HeaderLoggingMiddleware>();
app.UseCors("AppPolicy"); // Debe estar ANTES de UseAuthorization y MapControllers
                 
app.UseMiddleware<ApiKeyMiddleware>(); // Tu middleware de API Key
app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapArticuloEndpoints();

app.MapSujetoEndpoints();

app.MapComunEndpoints();

app.MapAlmacenEndpoints();

//Cta Cte
app.MapCtaCteEndpoints();

//Ventas
app.MapVentasVariosEndpoints();
app.MapFacturaEndpoints();
app.MapRemitoEndpoints();
app.MapPedidoEndpoints();

//Contabilidad
app.MapContabilidadEndpoints();

//Cereal
app.MapCosechaEndpoints();
app.MapEntradaEndpoints();
app.MapCertificadoEndpoints();
app.MapLiquidacionEndpoints();
app.MapRetiroSalidaEndpoints();
app.MapBoletoEndpoints();
app.MapRtEndpoints();
app.MapCtaCteCerealEndpoints();
app.MapPlantaEndpoints();
app.MapContratoEndpoints();
app.MapVariosCerealEndpoints();

app.Run();





