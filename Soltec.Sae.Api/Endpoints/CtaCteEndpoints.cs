using System.Data;
using ClosedXML.Excel;
using Soltec.Sae.Api;

public static class CtaCteEndpoints
{
    public static void MapCtaCteEndpoints(this WebApplication app)
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

        app.MapGet("/api/contabilidad/CtaCte/{id}/saldo", (string id, HttpRequest request, HttpResponse response) =>
        {
            string idCuentaMayor = request.Query["IdCuentaMayor"];
            var fechaStr = request.Query["Fecha"].ToString();
            string vencidoStr = request.Query["vencido"].ToString();
            bool vencido = vencidoStr != "" ? Convert.ToBoolean(vencidoStr.ToString()) : false;
            string idDivisaStr = request.Query["idDivisa"].ToString();
            int idDivisa = idDivisaStr == "" ? 0 : Convert.ToInt32(idDivisaStr);
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            CtaCteService service = new CtaCteService(connectionStringBase);
            decimal result = 0;
            result = service.Saldo(id, idCuentaMayor, fecha, idDivisa, vencido);
            return Results.Ok(result);
        });

        app.MapGet("/api/contabilidad/CtaCte/{id}", (string id, HttpRequest request, HttpResponse response) =>
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

            var fechaVencStr = request.Query["FechaVenc"].ToString();
            var fechaVenc = fechaVencStr == "" ? DateTime.Now : DateTime.ParseExact(fechaVencStr, "MM-dd-yyyy", null);

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
            result = service.Saldos(idCuenta, idCuentaHasta, idCuentaMayor, fecha, fechaVenc, idDivisa);

            return Results.Ok(result);
        });

        app.MapGet("/api/contabilidad/CtaCte/saldos/xls", (HttpRequest request, HttpResponse response) =>
        {
            string idCuenta = request.Query["IdCuenta"].ToString();
            string idCuentaHasta = request.Query["IdCuentaHasta"].ToString();
            string idCuentaMayor = request.Query["IdCuentaMayor"].ToString();
            var fechaStr = request.Query["Fecha"].ToString();
            var fecha = fechaStr == "" ? DateTime.Now : DateTime.ParseExact(fechaStr, "MM-dd-yyyy", null);
            var fechaVencStr = request.Query["FechaVenc"].ToString();
            var fechaVenc = fechaVencStr == "" ? DateTime.Now : DateTime.ParseExact(fechaVencStr, "MM-dd-yyyy", null);
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
            result = service.Saldos(idCuenta, idCuentaHasta, idCuentaMayor, fecha, fechaVenc, idDivisa);
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
                dt.Rows.Add(item.IdCuenta, item.IdCuentaMayor, item.Nombre, item.SaldoVencido, item.SaldoVencido, item.IdDivisa);
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
    }
}
