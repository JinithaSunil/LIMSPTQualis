using LIMS.Models;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;


namespace LIMS
{
    public partial class HTMLToPDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PO/PO-80.html"));
            divPO.InnerHtml = html;
        }
        protected async void btnGenerate_Click(object sender, EventArgs e)
        {
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PO/PO-80.html"));
            divPO.InnerHtml = html;

            string pdfPath = Server.MapPath("~/Reports/TestReport.pdf");

            await GeneratePdf(html, pdfPath);

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "inline; filename=TestReport.pdf");
            Response.WriteFile(pdfPath);
            Response.End();
        }


        public static async Task GeneratePdf(string html, string outputFile)
        {
            await DownloadBrowser();

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
            });

            var page = await browser.NewPageAsync();

            await page.SetContentAsync(html);

            await page.PdfAsync(outputFile, new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                //MarginOptions = new MarginOptions
                //{
                //    Top = "15mm",
                //    Bottom = "15mm",
                //    Left = "10mm",
                //    Right = "10mm"
                //}
            });

            await browser.CloseAsync();
        }

        private static async Task DownloadBrowser()
        {
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
            });
        }
    }
}