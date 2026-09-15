using Soltec.Sae.Api;

public static class CertificadoEndpoints
{
    public static void MapCertificadoEndpoints(this WebApplication app)
    {
        var sucursales = app.Services.GetRequiredService<List<Sucursal>>();

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
                var tmpresult = service.List(idCuenta, idCosecha, fecha);
                result.AddRange(tmpresult);
            }
            return result;
        });

        app.MapGet("/api/cereales/certificado/{id}", (string id, HttpRequest request, HttpResponse response) =>
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
    }
}
