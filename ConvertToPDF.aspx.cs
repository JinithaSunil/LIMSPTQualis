using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace LIMS
{
    public partial class ConvertToPDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void ExportToPDF(object sender, EventArgs e)
        {
            try

            {

                string strHtml = string.Empty;

                string htmlFileName = Server.MapPath("~") + "\\TestReport\\" + "TestReport-11478769206.htm";

                string pdfFileName = Request.PhysicalApplicationPath + "\\TestReport\\" + "TestReport-11478769206.pdf";

                FileStream fsHTMLDocument = new FileStream(htmlFileName, FileMode.Open, FileAccess.Read);

                StreamReader srHTMLDocument = new StreamReader(fsHTMLDocument);

                strHtml = srHTMLDocument.ReadToEnd();

                srHTMLDocument.Close();

                strHtml = strHtml.Replace("\r\n", "");

                strHtml = strHtml.Replace("\0", "");

                using (Stream fs = new FileStream(Request.PhysicalApplicationPath + "\\TestReport\\" + "TestReport-11478769206.pdf", FileMode.Create))

                {

                    using (Document doc = new Document(PageSize.A4))

                    {

                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                        doc.Open();

                        using (StringReader sr = new StringReader(strHtml))

                        {

                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

                        }

                        doc.Close();

                        Response.ContentType = "application/pdf";

                        Response.AddHeader("content-disposition", "attachment;filename=sample.pdf");

                        Response.Cache.SetCacheability(HttpCacheability.NoCache);

                        Response.Write(doc);

                    }

                }

            }

            catch (Exception ex)

            {

                Response.Write(ex.Message);

            }
        }
    }
}