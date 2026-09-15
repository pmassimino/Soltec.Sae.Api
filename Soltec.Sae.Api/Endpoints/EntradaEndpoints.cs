using Soltec.Sae.Api;

public static class EntradaEndpoints
{
    public static void MapEntradaEndpoints(this WebApplication app)
    {
        string connectionStringCerealesBase = app.Configuration["ConnectionStringsCereales"];
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();
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

        //Entradas
        app.MapGet("/api/cereales/entrada/{id}", (string id, HttpRequest request, HttpResponse response) =>
        {
            EntradaService service = new EntradaService(connectionStringCerealesBase);
            Entrada result = service.FindOne(id);
            return result;
        });

        //Entradas
        app.MapGet("/api/cereales/entrada/trx/{ntra}", (string ntra, HttpRequest request, HttpResponse response) =>
        {
            EntradaService service = new EntradaService(connectionStringCerealesBase);
            Entrada result = service.FindBynTra(ntra);
            return result;
        });

        app.MapGet("/api/cereales/entrada/trx/{ntra}/pdf", async (string ntra, HttpRequest request, HttpResponse response) =>
        {
            EntradaService service = new EntradaService(connectionStringCerealesBase);
            Entrada entity = null;
            entity = service.FindBynTra(ntra);
            if (entity == null)
            {
                return Results.NotFound("Registro no encontrado");
            }
            EntradaTemplate template = new EntradaTemplate();
            template.Entity = entity;
            template.Empresa = empresa;
            template.Path = webHostEnvironment.ContentRootPath;
            MemoryStream stream = await template.ToPDF();
            stream.Position = 0;
            string filename = "Entrada_" + entity.Ctg.ToString() + ".pdf";
            return Results.File(stream.ToArray(), "application/pdf", filename);
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
                var tmpResult = service.List(idCuenta, idCosecha, fecha, fechaHasta);
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
            Int64 result = service.Total(idCuenta, idCosecha, fecha);
            return result;
        });
    }
}
