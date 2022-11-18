using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;
using System.Reflection.Metadata;
using Document = iTextSharp.text.Document;

namespace ReportePdfApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        public WeatherForecastController()
        {

        }
        /*Pdf buscar por nombre*/
        [HttpGet("BusquedaDelPdf/{pdfBuscar}")]
        public ActionResult Get(string pdfBuscar)
        { 
            try
            {
                var stream = new FileStream(@"C:\Users\VIAMATICA\Desktop\"+pdfBuscar+".pdf", FileMode.Open);
                return File(stream, "application/pdf", "FileDownloadName.ext");
            }
            catch
            {
                return NotFound("No se encontro el pdf");
            }
        }
        /*Pdf buscar por nombre y presentar imagen*/
        [HttpGet("BusquedaDelPdfConImagen/{pdfBuscar}")]
        public ActionResult GetPdfConImagen(string pdfBuscar)
        {
            try
            {
                string pdfUser = "C:\\Users\\VIAMATICA\\Desktop\\"+ pdfBuscar + ".pdf";
                string tempPdf = "C:\\Users\\VIAMATICA\\Desktop\\TemporalPdf"+ pdfBuscar + ".pdf";

                using (Stream inputPdf = new FileStream(pdfUser, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Stream outputPdf = new FileStream(tempPdf, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var reader = new PdfReader(inputPdf);
                        var stamper = new PdfStamper(reader, outputPdf);
                        iTextSharp.text.Rectangle rect =
                         new iTextSharp.text.Rectangle(100, 100, 500, 500);
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance("C:\\Users\\VIAMATICA\\Desktop\\QrImagen.png");
                        img.ScaleAbsolute(rect.Width, rect.Height);
                        img.SetAbsolutePosition(rect.Left, rect.Bottom);
                        int numeroDePaginas = stamper.Reader.NumberOfPages;
                        stamper.InsertPage(numeroDePaginas+1, new iTextSharp.text.Rectangle(20,28,20,28));
                        stamper.GetOverContent(numeroDePaginas+1).AddImage(img);
                        stamper.Close();
                    }
                }
                using (Stream outputPdf = new FileStream(tempPdf, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Stream inputPdf = new FileStream(pdfUser, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var reader = new PdfReader(outputPdf);
                        var stamper = new PdfStamper(reader, inputPdf);
                        stamper.Close();
                    }
                }
                System.IO.File.Delete(tempPdf);
                FileStream stream = new FileStream(@"C:\Users\VIAMATICA\Desktop\" + pdfBuscar + ".pdf", FileMode.Open);
                return File(stream, "application/pdf", "FileDownloadName.ext");
            }
            catch(Exception ex)
            {
                return NotFound("No se encontro el pdf");
            }
        }
        /*Pdf crearlo con html*/
        [HttpPost("CrarElPdfConHtml")]
        public ActionResult CrearPdfConHtml(Html html)
        {
            try
            {

                    StringWriter sw = new StringWriter();
                    sw.WriteLine(html.codigoHtml.ToString());
                    StringReader sr = new StringReader(sw.ToString());
                iTextSharp.text.Document pdfDoc = new Document();
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, new FileStream("C:\\Users\\VIAMATICA\\Desktop\\HtmlPdfiu.pdf", FileMode.Create));
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance("C:\\Users\\VIAMATICA\\Desktop\\QrImagen.png");
                    image1.ScaleAbsoluteWidth(100);
                    image1.ScaleAbsoluteHeight(100);
                    image1.Alignment = Element.ALIGN_RIGHT;
                    //image1.SetAbsolutePosition(50, 50);
               // float positionx = image1.AbsoluteX;
              //  float positiony = image1.AbsoluteY;
              //  positionx = positionx + 10;
              //  Console.WriteLine("Position x:" + positionx + " Position Y : " + positiony);
                pdfDoc.Add(image1);
                    pdfDoc.Close();
                    FileStream stream = new FileStream("C:\\Users\\VIAMATICA\\Desktop\\HtmlPdfiu.pdf", FileMode.Open);
                    return File(stream, "application/pdf", "FileDownloadName.ext");
            }
            catch(Exception ex)
            {
                return NotFound("No se encontro el pdf => "+ex.Message);
            }
        }
        /*Pdf crearlo y codigo Qr*/
        [HttpPost("CrarElPdfConHtmlYPersonaQr")]
        public ActionResult CrearPdfConHtmlYPersonaQr(Html html)
        {
            try
            {
                
                StringWriter sw = new StringWriter();
                sw.WriteLine(html.codigoHtml.ToString());
                StringReader sr = new StringReader(sw.ToString());
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document();
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream("C:\\Users\\VIAMATICA\\Desktop\\HtmlPdfiu.pdf", FileMode.Create));
                pdfDoc.Open();
                htmlparser.Parse(sr);
                
                iTextSharp.text.Image image1 = imagenPersona().GetImage();
                image1.ScaleAbsoluteWidth(100);
                image1.ScaleAbsoluteHeight(100);
                image1.Alignment = Element.ALIGN_RIGHT;
                image1.IndentationRight = 20;
                pdfDoc.Add(image1);
                float posiscionImagenX = image1.Right;
                float posiscionImagenY = image1.Top;
                
                Console.WriteLine("Posicicon X: "+posiscionImagenX+" Posicion Y : "+posiscionImagenY);
                pdfDoc.Close();
                FileStream stream = new FileStream("C:\\Users\\VIAMATICA\\Desktop\\HtmlPdfiu.pdf", FileMode.Open);
                return File(stream, "application/pdf", "FileDownloadName.ext");
            }
            catch (Exception ex)
            {
                return NotFound("No se encontro el pdf => " + ex.Message);
            }
        }
        /*No funciona aun*/
        [HttpGet("BusquedaDeTodosLosPdfs")]
        public ActionResult GetTodos()
        {
            try
            {
                List<String> listaPdf = new List<String>();
                string[] files = Directory.GetFiles(@"C:\Users\VIAMATICA\Desktop", "*.pdf");
                foreach (string file in files)
                {
                    listaPdf.Add(file);
                }
                if(listaPdf.Count > 0)
                {
                    //FileInfo[] file = new FileInfo[]();
                    return Ok(files);
                }
                else{
                    return NotFound("No se encontro el pdf");
                }
                
            }
            catch
            {
                return NotFound("No se encontro el pdf");
            }
        }/*
        public HttpResponseMessage Get(string docId)
        {
            byte[] response = FileProxy.GetDocumentStream(docId);

            if (response == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            MemoryStream ms = new MemoryStream(response);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(ms.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return result;
        }*/
        private BarcodeQRCode imagenPersona()
        {
            Persona persona = new Persona
            {
                nombre = "Kevin",
                apellido = "Arevalo",
                cedula = "0302527742"
            };
            var paramQR = new Dictionary<EncodeHintType, object>();
            paramQR.Add(EncodeHintType.CHARACTER_SET, CharacterSetECI.GetCharacterSetECIByName("UTF-8"));
            BarcodeQRCode qrCodigo = new BarcodeQRCode("Nombre: " + persona.nombre + " Apellido: " + persona.apellido + " Cedula: " + persona.cedula,
                150, 150, paramQR);
            return qrCodigo;
        }
    }
}