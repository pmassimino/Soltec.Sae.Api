using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using NPOI.SS.Formula.Functions;

namespace Soltec.Sae.Api
{
    public class RetencionDGRTemplate
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }        
        public RetencionBase Entity { get; set; }
        public Empresa Empresa { get; set; }
        public string Path { get; set; }
        public async Task<MemoryStream> ToPDF()
        {
            var doc = new Document(PageSize.A4, 10f, 10f, 135f, 100f);
            var strFilePath = this.Path + @"\ReportsTemplate";
            var pdfTemplate = strFilePath + @"\TemplateRetencionDGR.pdf";

            PdfReader pdfReader = new PdfReader(pdfTemplate);
            MemoryStream stream = new MemoryStream();
            PdfStamper pdfStamper = new PdfStamper(pdfReader, stream);
            AcroFields Form = pdfStamper.AcroFields;
            

            Form.SetField("NombreEmpresa", Empresa.Nombre);
            Form.SetField("DomicilioEmpresa", Empresa.Direccion + "-" + Empresa.Localidad );
            Form.SetField("EmailEmpresa", Empresa.Email);
            Form.SetField("TelefonoEmpresa", Empresa.Telefono  );
            Form.SetField("LugarEmision", Empresa.Localidad);


            Form.SetField("CuitEmpresa", Empresa.Cuit);
            Form.SetField("NumeroIBEmpresa", Empresa.NumeroIB);
            Form.SetField("Fecha", Entity.FechaComprobante.ToShortDateString());
            

            string numero = Entity.Numero.ToString();
            Form.SetField("Numero", numero);            
            string nombre = Entity.Cuenta.Nombre;
            string cuit = Entity.Cuenta.NumeroDocumento;
            Form.SetField("Nombre", nombre);
            Form.SetField("Cuenta", Entity.IdCuenta);
            Form.SetField("Cuit", cuit);
            Form.SetField("NumeroIB", Entity.Cuenta.NumeroIngBruto);
            Form.SetField("Domicilio", Entity.Cuenta.Domicilio);
            Form.SetField("Localidad", Entity.Cuenta.Localidad);
            Form.SetField("CodigoPostal", Entity.Cuenta.CodigoPostal);
            Form.SetField("Provincia", Entity.Cuenta.Provincia);
            

            Form.SetField("Impuesto", Entity.Impuesto);
            Form.SetField("Regimen", Entity.Regimen);
            Form.SetField("NumeroComprobante", Entity.NumeroComprobante);
            Form.SetField("ImposibilidadRetener", "NO");
            Form.SetField("BaseImponible", Entity.BaseImponible.ToString("N"));
            Form.SetField("Alicuota", Entity.Alicuota.ToString("N"));
            Form.SetField("Importe", Entity.Importe.ToString("N"));
            Form.SetField("ImporteEnLetras", Entity.Obs);
            Form.SetField("Periodo", Entity.FechaPase.Month.ToString() + " de " + Entity.FechaPase.Year.ToString());


            pdfStamper.FormFlattening = true;
            pdfStamper.Close();
            //PdfContentByte contentByte = pdfStamper.GetOverContent(1);
            //contentByte.AddImage(image);

            var file = stream.ToArray();
            var output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return output;
        }
    }
    
    }


