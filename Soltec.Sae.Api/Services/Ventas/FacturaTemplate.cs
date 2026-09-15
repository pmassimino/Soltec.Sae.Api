using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using NPOI.SS.Formula.Functions;

namespace Soltec.Sae.Api
{
    public class FacturaTemplate
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }        
        public Factura Entity { get; set; }
        public Sujeto Sujeto { get; set; }
        public Empresa Empresa { get; set; }
        public string Path { get; set; }
        public List<Seccion> SeccionDolar { get; set; }
        public async Task<MemoryStream> ToPDF()
        {
            var doc = new Document(PageSize.A4, 10f, 10f, 135f, 100f);
            var strFilePath = this.Path + @"\ReportsTemplate";
            var pdfTemplate = strFilePath + @"\TemplateRom.pdf";
            if (Entity.Cae == 0)
                pdfTemplate = this.Path + @"\ReportsTemplate\TemplateFacturaCFiscal.pdf";
            else
                pdfTemplate = this.Path + @"\ReportsTemplate\TemplateFactura.pdf";

            if (this.Entity.IdDivisa == 1 || (this.Entity.IdDivisa == 0 && this.SeccionDolar?.Where(w => w.Id == this.Entity.Sec).Count() > 0)) 
                pdfTemplate = this.Path + @"\ReportsTemplate\TemplateFacturaDol.pdf";

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


            Form.SetField("domicilioEmpresa", Empresa.Direccion + "-Tel.: " + Empresa.Telefono);
            Form.SetField("domicilioEmpresa1", Empresa.Cpostal + "-" + Empresa.Localidad + "-" + Empresa.Provincia);
            Form.SetField("emailEmpresa", Empresa.Email);
            Form.SetField("condIvaEmpresa", "IVA RESPONSABLE INSCRIPTO");
            Form.SetField("cuitEmpresa", Empresa.Cuit);
            Form.SetField("numeroIBEmpresa", Empresa.NumeroIB);
            Form.SetField("fechaInicioAct", Empresa.FechaIniAct);
            Form.SetField("fecha", Entity.FechaComprobante.ToShortDateString());
            Form.SetField("fechaVenc", Entity.FechaVencimiento.ToShortDateString());
            string numero = Entity.Pe.ToString().PadLeft(4, '0') + "-" + Entity.Numero.ToString().PadLeft(8, '0'); ;
            Form.SetField("numero", numero);
            Form.SetField("nombre", Entity.Cuenta.Nombre);
            Form.SetField("domicilio", Entity.Cuenta.Domicilio);
            Form.SetField("localidadProvincia", this.Sujeto.CodigoPostal + "-" + this.Sujeto.Localidad + "-" + this.Sujeto.Provincia);
            Form.SetField("condIva", this.Sujeto.CondicionIva);
            Form.SetField("cuit", this.Sujeto.NumeroDocumento);
            Form.SetField("cuentaNumero", Entity.Cuenta.Id);
            Form.SetField("condVenta", Entity.CondVenta);
            Form.SetField("letra", Entity.Letra);
            Form.SetField("cotiz", Entity.Cotizacion.ToString());

            string tipoComp = "FACTURA";
            if (Entity.Tipo == 2)
                tipoComp = "NOTA DE CREDITO";
            else if (Entity.Tipo == 3)
                tipoComp = "NOTA DE DEBITO";
            else if (Entity.Tipo == 4)
                tipoComp = "TICKET";
            Form.SetField("tipoComprobante", tipoComp);

            // Detalle productos
            var i = 1;
            foreach (var item in Entity.Detalle)
            {
                Form.SetField("codigo" + i.ToString().Trim(), item.IdArticulo);
                Form.SetField("cantidad" + i.ToString().Trim(), item.Cantidad.ToString());
                Form.SetField("detalle" + i.ToString().Trim(), item.Concepto);
                Form.SetField("precioUnitario" + i.ToString().Trim(), item.Precio.ToString("N2"));
                Form.SetField("importe" + i.ToString().Trim(), item.SubTotal.ToString("N2"));
                i += 1;
            }
            Form.SetField("obs", Entity.Obs);
            Form.SetField("importeSubTotal", Entity.SubTotal.ToString("N2"));
            Form.SetField("importeDescuento", Entity.Descuento.ToString("N2"));
            Form.SetField("importePercepcionIB", Entity.PrecepcionIB.ToString("N2"));
            Form.SetField("importePercepcion", Entity.PrecepcionIva.ToString("N2"));
            Form.SetField("importeSubTotal2", (Entity.SubTotal - Entity.Descuento).ToString("N2"));
            Form.SetField("importeImp", Entity.ImpuestoInterno.ToString("N2"));
            Form.SetField("importeIvaOtro", Entity.IvaOtro.ToString("N2"));
            Form.SetField("importeIvaG", Entity.IvaGeneral.ToString("N2"));
            Form.SetField("importeTotal", Entity.Total.ToString("N2"));
            decimal totalDol = Entity.Cotizacion == 0 ? 0 : Convert.ToDecimal(Entity.Total) / Entity.Cotizacion;
            Form.SetField("importeTotalDol", totalDol.ToString("N2"));

            Form.SetField("cae", Entity.Cae.ToString());
            Form.SetField("fechaVencCae", Entity.FechaComprobante.AddDays(10).ToShortDateString());
            //Form.SetField("codBarra", Entity.);
            //Form.SetField("codBarraNumero", Entity.codigoBarra);
            Form.SetField("remito", Entity.Remito);

            //QR
            var year = Entity.FechaComprobante.Year.ToString();
            var month = Entity.FechaComprobante.Month.ToString().PadLeft(2, '0');
            var day = Entity.FechaComprobante.Day.ToString().PadLeft(2,'0');
            var fecha = year + "-" + month + "-" + day;
            var cuit = Empresa.Cuit.Replace("-", "");
            var ptoVa = Entity.Pe.ToString();
            var letra = Entity.Letra;
            var nroCmp = Entity.Numero.ToString();
            var tipoCmp = Entity.TipoComp;
            var importe = Entity.Total.ToString().Replace(",", ".");
            var moneda = Entity.IdDivisa == 0 ? "PES" : "DOL";
            var ctz = Entity.IdDivisa == 0 ? "1" : Entity.Cotizacion.ToString();
            var tipoDocRec = Entity.Cuenta.NumeroDocumento.Trim().Length == 11 ? "80" : "96";
            var nroDocRec = Entity.Cuenta.NumeroDocumento.Trim();
            var tipoCodAut = 'E';
            var codAut = Entity.Cae;
            var qrStr = "{'ver':1,'fecha':'" + fecha + "','cuit':" + cuit + ",'ptoVta':" + ptoVa + ",'tipoCmp':" + tipoCmp + ",'nroCmp':" + nroCmp + ",'importe':" + importe + ",'moneda':'" + moneda + "','ctz':" + ctz + ",'tipoDocRec':" + tipoDocRec + ",'nroDocRec':" + nroDocRec + ",'tipoCodAut':'" + "E" + "','codAut':" + codAut + "}";

            qrStr = qrStr.Replace(@"'", @"""");

            byte[] byt = System.Text.Encoding.UTF8.GetBytes(qrStr);
            var qrBase64 = Convert.ToBase64String(byt);

            // Dim qrBase64 = Convert.ToBase64String(qrStr)

            var url = "https://www.afip.gob.ar/fe/qr/?p=" + qrBase64;
            // Insertar qr
            iTextSharp.text.pdf.BarcodeQRCode qrcode = new BarcodeQRCode(url, 50, 50, null);
            iTextSharp.text.Image img = qrcode.GetImage();

            img.SetAbsolutePosition(50, 85);
            if (Entity.Cae != 0)
                pdfStamper.GetOverContent(1).AddImage(img);


            pdfStamper.FormFlattening = true;
            pdfStamper.Close();

            var file = stream.ToArray();
            var output = new MemoryStream();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return output;
        }
    }
    
    }


