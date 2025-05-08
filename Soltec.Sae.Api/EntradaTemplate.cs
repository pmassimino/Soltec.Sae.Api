using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using NPOI.SS.Formula.Functions;

namespace Soltec.Sae.Api
{
    public class EntradaTemplate
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }        
        public Entrada Entity { get; set; }
        public Empresa Empresa { get; set; }
        public string Path { get; set; }
        public async Task<MemoryStream> ToPDF()
        {
            var doc = new Document(PageSize.A4, 10f, 10f, 135f, 100f);
            var strFilePath = this.Path + @"\ReportsTemplate";
            var pdfTemplate = strFilePath + @"\TemplateRom.pdf";

            PdfReader pdfReader = new PdfReader(pdfTemplate);
            MemoryStream stream = new MemoryStream();
            PdfStamper pdfStamper = new PdfStamper(pdfReader, stream);
            AcroFields Form = pdfStamper.AcroFields;
            // add a image            
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(strFilePath + @"\logo.jpg");
            PushbuttonField ad = Form.GetNewPushbuttonFromField("logo");
            if (ad != null)
            {
                ad.Layout = PushbuttonField.LAYOUT_ICON_ONLY;
                ad.ProportionalIcon = true;
                ad.Image = image;
                Form.ReplacePushbuttonField("logo", ad.Field);
            }


            Form.SetField("nombreEmpresa", Empresa.Nombre);
            Form.SetField("direccionEmpresa", Empresa.Direccion + "- Tel:" + Empresa.Telefono + " - " );
            Form.SetField("emailEmpresa", "email:" + Empresa.Email);
            Form.SetField("condIvaEmpresa", "IVA RESPONSABLE INSCRIPTO");
            Form.SetField("cuitEmpresa", Empresa.Cuit);
            Form.SetField("ingBEmpresa", "");
            Form.SetField("fecha", Entity.Fecha.ToShortDateString());
            Form.SetField("tipoCopia", "ORIGINAL");

            string numero = Entity.Numero.ToString();
            Form.SetField("numero", numero);

            string nombreProductor = Entity.Nombre;
            string cuitProductor = Entity.NumeroDocumento;
            Form.SetField("nombreProductor", nombreProductor);
            Form.SetField("cuitProductor", cuitProductor);
            string nombreTransporte = Entity.Transporte.Nombre;
            string cuitTransporte = Entity.Transporte.NumeroDocumento;

            Form.SetField("nombreCosecha", Entity.NombreCosecha);
            Form.SetField("nombreTransporte", nombreTransporte);
            Form.SetField("cuitTransporte", cuitTransporte);


            string nombreChofer = Entity.Chofer.Nombre;
            string cuitChofer = Entity.Chofer.NumeroDocumento;
            string patenteC = Entity.PatenteC;
            string patenteA = Entity.PatenteA;
            Form.SetField("nombreChofer", nombreChofer);
            Form.SetField("cuitChofer", cuitChofer);
            Form.SetField("patenteC", patenteC);
            Form.SetField("patenteA", patenteA);
            string nombreDestinatario = Empresa.Nombre;
            string nombreDestino = Empresa.Nombre;
            Form.SetField("nombreDestinatario", nombreDestinatario);
            Form.SetField("nombreDestino", nombreDestino);
            string lugarEntrega = "";
            Form.SetField("plantaOncca", Entity.IdPlanta);
            string numeroComp = Entity.NumeroCartaPorte;
            Form.SetField("numeroComp", numeroComp);
            Form.SetField("ctg", Entity.Ctg.ToString());
            Form.SetField("cosecha", "cosecha");
            Form.SetField("procedencia", Entity.Procedencia);
            Form.SetField("pesoBruto", Entity.PesoBruto.ToString());
            Form.SetField("pesoTara", Entity.PesoTara.ToString());
            Form.SetField("pesoNetoParcial", Entity.PesoNeto.ToString());
            Form.SetField("porHum", Entity.PorHumedad.ToString());
            Form.SetField("mermaHum", Entity.MermaHumedad.ToString());
            Form.SetField("porZar", Entity.PorZaranda.ToString());
            Form.SetField("mermaZar", Entity.MermaZaranda.ToString());
            Form.SetField("porVol", Entity.PorVolatil.ToString());
            Form.SetField("mermaVol", Entity.MermaVolatil.ToString());
            Form.SetField("porCal", Entity.PorCalidad.ToString());
            Form.SetField("mermaCal", Entity.MermaCalidad.ToString());
            decimal totalMerma = Entity.MermaCalidad + Entity.MermaHumedad + Entity.MermaVolatil + Entity.MermaZaranda;
            Form.SetField("totalMermas", totalMerma.ToString());
            Form.SetField("pesoNeto", Entity.PesoNetoFinal.ToString());
            Form.SetField("obs", Entity.Observacion);
            Form.SetField("distancia", Entity.Distancia.ToString());


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


