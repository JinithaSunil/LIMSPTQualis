using LIMS.DAL;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Word;
using System.Text;

namespace LIMS
{
    public partial class GenerateQuotationCalib : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int QuotationID = 0;
        int Revision = 0;
        int InquiryId = 0;
        mailer objEmail = new mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                QuotationID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                Revision = Convert.ToInt32(Request.QueryString["r"].ToString());
                Session["QuotationID"] = QuotationID;
                Session["Revision"] = Revision;

                GenerateCalibHTML();
                GenerateCalibQuotation();
            }
        }
        protected void GenerateCalibHTML()
        {

            string QuotationNo = "";
            string Date = "";
            string CompanyName = "";
            string ContactPerson = "";
            string Address = "";
            string Dept = "";
            string Fax = "";
            string FromTelephone = "";
            string FromEmail = "";
            string To = "";
            string ToTelephone = "";
            string ToEmail = "";
            string TestItems = "";
            string PaymentTerm = "";
            string CurrencyName = "";
            string Remarks = "";
            int QuotationValidity = 0;
            string PostPODays = "";
            string CalibrationDays = "";
            string CertificateDays = "";
            decimal Quantity = 0;
            decimal Price = 0;
            decimal Amount = 0;
            decimal SubTotal = 0;
            decimal Discount = 0;
            decimal Totalbeforetax = 0;
            decimal VAT = 0;
            decimal Grandtotal = 0;
            StringBuilder sbQuotationDetails = new StringBuilder();

            ContactPerson = "Penny Wijaya";
            Dept = "Food Laboratory";
            Fax = "021 4585 0661";
            FromTelephone = "021 4585 0885";
            FromEmail = "penny.wijaya@qualis-indonesia.com";


            QuotationID = Convert.ToInt32(Request.QueryString["ID"].ToString());


            System.Data.DataTable dtQM = _cm.getquotationmaster(QuotationID);
            if (dtQM.Rows.Count > 0)
            {
                InquiryId = Convert.ToInt32(dtQM.Rows[0]["EnquiryId"].ToString());
                Date = dtQM.Rows[0]["QuotationDate"].ToString();
                QuotationNo = dtQM.Rows[0]["QuotationNumber"].ToString();
                CompanyName = dtQM.Rows[0]["CompanyName"].ToString();
                Address = dtQM.Rows[0]["AddressLine1"] + " " +
                            dtQM.Rows[0]["AddressLine2"] + " " +
                            dtQM.Rows[0]["AddressLine3"] + " <br/>" +
                            dtQM.Rows[0]["District"] + " " +
                            dtQM.Rows[0]["State"] + " " +
                            dtQM.Rows[0]["Pincode"] + " " +
                            dtQM.Rows[0]["Country"].ToString();
                To = dtQM.Rows[0]["Title"] + " " + dtQM.Rows[0]["FirstName"] + "  " + dtQM.Rows[0]["LastName"].ToString();
                ToTelephone = dtQM.Rows[0]["Phone"].ToString();
                ToEmail = dtQM.Rows[0]["Email"].ToString();
                PaymentTerm = dtQM.Rows[0]["PaymentTermName"].ToString();
                CurrencyName = dtQM.Rows[0]["CurrencyName"].ToString();
                Remarks = dtQM.Rows[0]["Remarks"].ToString();
                QuotationValidity = Convert.ToInt32(dtQM.Rows[0]["QuotationValidityInDays"].ToString());
                PostPODays = "2-3";


                Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());
                VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());
                Totalbeforetax = Grandtotal - VAT;
                SubTotal = Totalbeforetax + Discount;

            }
            System.Data.DataTable dtQD = _cm.getquotationdetailsNew(QuotationID);

            if (dtQD.Rows.Count > 0)
            {
                int count = 0;
                sbQuotationDetails.Append(@"
                    <table style='border: 1px solid'>
                        <tr>
                            <td style='border: 1px solid !important'>No.</td>
                            <td style='border: 1px solid !important; height: 30px; width: 60%; text-align: center;'>Nama Alat</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Kapasitas</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Harga Satuan</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Kuantitas</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Harga Kalibrasi</td>
                        </tr>");
                foreach (DataRow dr in dtQD.Rows)
                {
                    count++;
                    sbQuotationDetails.Append(
                          @"<tr><td style='border: 1px solid !important'>" + count + "</td>" +
                          @"<td style='border: 1px solid !important; height: 30px; width: 60%; text-align: left;'>" + dr["TestItems"].ToString() + "</td>" +
                          @"<td style='border: 1px solid !important; width: 10%; text-align: center;'>" + dr["SampleDescription"].ToString() + "</td>" +
                          @"<td style='border: 1px solid !important; width: 10%; text-align: center;'>" + dr["Price"].ToString() + "</td> " +
                          @"<td style='border: 1px solid !important; width: 10%; text-align: center;'>" + dr["Quantity"].ToString() + "</td> " +
                          @"<td style='border: 1px solid !important; width: 10%; text-align: center;'>" + dr["TotalAmount"].ToString() + "</td></tr>");
                }
                sbQuotationDetails.Append("<tr><td style='border-left:none !important;border-bottom:none !important;' colspan='4'>&nbsp;</td><td style='border-left:1px solid black !important;'>Sub Total<br/>PPN 10%<br/>Total<br/></td><td>" + SubTotal + "<br>" + VAT + "<br>" + Grandtotal + "</td></tr>");
                sbQuotationDetails.Append("</table>");
            }
            string body = objEmail.PrepareMailBodyWith("QuotationCalib.html",
                   "{Date}", Date,
                   "{QuotationNo}", QuotationNo,
                   "{CompanyName}", CompanyName,
                   "{ContactPerson}", ContactPerson,
                   "{Address}", Address,
                   "{Dept}", Dept,
                   "{Fax}", Fax,
                   "{FromTelephone}", FromTelephone,
                   "{FromEmail}", FromEmail,
                   "{To}", To,
                   "{ToTelephone}", ToTelephone,
                   "{ToEmail}", ToEmail,
                   "{PaymentTerm}", PaymentTerm,
                   "{CurrencyName}", CurrencyName,
                   "{Remarks}", Remarks,
                   "{QuotationDetails}", sbQuotationDetails.ToString(),  // Insert the dynamic table here
                   "{SubTotal}", SubTotal.ToString(),
                   "{Discount}", Discount.ToString(),
                   "{Totalbeforetax}", Totalbeforetax.ToString(),
                   "{VAT}", VAT.ToString(),
                   "{Grandtotal}", Grandtotal.ToString()
               );

            // Convert the body to string and save it to an HTML file
            string str = body.ToString();
            string filename = Server.MapPath("//Quotation/Quotation-" + QuotationID + "_" + Revision + ".htm");
            System.IO.File.WriteAllText(@filename, str);

            // Change customer inquiry Status to CRF Received
            _cm.inquiryStatusId = 6;
            _cm.UpdateInquiryStatus(_cm);
            GC.Collect();

            // Read the generated HTML file and display it in the page
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Quotation/Quotation-" + QuotationID + "_" + Revision + ".htm"));
            divRequest.InnerHtml = html;
        }

        static void FindAndReplace(Microsoft.Office.Interop.Word.Application wordApp, string findText, string replaceText)
        {
            Document doc = wordApp.ActiveDocument;
            Microsoft.Office.Interop.Word.Range range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = findText;
            range.Find.Forward = true;
            // Find each occurrence of the placeholder and replace manually
            while (range.Find.Execute())
            {
                range.Text = replaceText;
                range.Start = range.End;
                range.End = doc.Content.End;
            }
        }
        protected void GenerateCalibQuotation()
        {
            if (Request.QueryString["ID"] != null)
            {
                string QuotationNo = "";
                string Date = "";
                string CompanyName = "";
                string ContactPerson = "";
                string Address = "";
                string Dept = "";
                string Fax = "";
                string FromTelephone = "";
                string FromEmail = "";
                string To = "";
                string ToTelephone = "";
                string ToEmail = "";
                string TestItems = "";
                string PaymentTerm = "";
                string CurrencyName = "";
                string Remarks = "";
                int QuotationValidity = 0;
                string PostPODays = "";
                string CalibrationDays = "";
                string CertificateDays = "";
                decimal Quantity = 0;
                decimal Price = 0;
                decimal Amount = 0;
                decimal SubTotal = 0;
                decimal Discount = 0;
                decimal Totalbeforetax = 0;
                decimal VAT = 0;
                decimal Grandtotal = 0;

                ContactPerson = "Penny Wijaya";
                Dept = "Food Laboratory";
                Fax = "021 4585 0661";
                FromTelephone = "021 4585 0885";
                FromEmail = "penny.wijaya@qualis-indonesia.com";


                QuotationID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                Application wordApp = new Application();
                Microsoft.Office.Interop.Word.Document doc = null;

                string path = Server.MapPath("~/Inputfiles/");
                string templatePath = path + "TSQ-CAL-FM-027-QUOTATION.doc";
                doc = wordApp.Documents.Add(templatePath);


                System.Data.DataTable dtQM = _cm.getquotationmaster(QuotationID);
                if (dtQM.Rows.Count > 0)
                {
                    InquiryId = Convert.ToInt32(dtQM.Rows[0]["EnquiryId"].ToString());
                    Date = dtQM.Rows[0]["QuotationDate"].ToString();
                    QuotationNo = dtQM.Rows[0]["QuotationNumber"].ToString();
                    CompanyName = dtQM.Rows[0]["CompanyName"].ToString();
                    Address = dtQM.Rows[0]["AddressLine1"] + " " +
                                dtQM.Rows[0]["AddressLine2"] + " " +
                                dtQM.Rows[0]["AddressLine3"] + " <br/>" +
                                dtQM.Rows[0]["District"] + " " +
                                dtQM.Rows[0]["State"] + " " +
                                dtQM.Rows[0]["Pincode"] + " " +
                                dtQM.Rows[0]["Country"].ToString();
                    To = dtQM.Rows[0]["Title"] + " " + dtQM.Rows[0]["FirstName"] + "  " + dtQM.Rows[0]["LastName"].ToString();
                    ToTelephone = dtQM.Rows[0]["Phone"].ToString();
                    ToEmail = dtQM.Rows[0]["Email"].ToString();
                    PaymentTerm = dtQM.Rows[0]["PaymentTermName"].ToString();
                    CurrencyName = dtQM.Rows[0]["CurrencyName"].ToString();
                    Remarks = dtQM.Rows[0]["Remarks"].ToString();
                    QuotationValidity = Convert.ToInt32(dtQM.Rows[0]["QuotationValidityInDays"].ToString());
                    PostPODays = "2-3";


                    Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());
                    VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                    Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());
                    Totalbeforetax = Grandtotal - VAT;
                    SubTotal = Totalbeforetax + Discount;


                    // Replace placeholders with database values
                    FindAndReplace(wordApp, "{QuotationNo}", QuotationNo);
                    FindAndReplace(wordApp, "{Date}", Date);
                    FindAndReplace(wordApp, "{To}", To);
                    FindAndReplace(wordApp, "{CompanyName}", CompanyName);
                    FindAndReplace(wordApp, "{ToTelephone}", ToTelephone);
                    FindAndReplace(wordApp, "{FromEmail}", FromEmail);
                    FindAndReplace(wordApp, "{CertificateDays}", CertificateDays);
                    FindAndReplace(wordApp, "{PaymentTerms}", PaymentTerm);
                    FindAndReplace(wordApp, "{PostPODays}", PostPODays);
                    FindAndReplace(wordApp, "{QuotationValidity}", QuotationValidity.ToString());
                }
                System.Data.DataTable dtQD = _cm.getquotationdetailsDoc(QuotationID);
                if (dtQD.Rows.Count > 0)
                {
                    CreateInstrumentTable(wordApp, doc, "{QuotationDetails}", dtQD, SubTotal, VAT, Grandtotal);
                    try
                    {
                        string pathout = Server.MapPath("~/Quotation/");
                        string outputDocxPath = pathout + "Quotation-" + QuotationID + "_1" + ".docx";
                        string outputPdfPath = pathout + "Quotation-" + QuotationID + "_1" + ".pdf";
                        doc.SaveAs2(outputDocxPath);
                        doc.SaveAs2(outputPdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                        Response.AddHeader("content-disposition", "inline;filename=" + outputPdfPath);
                        string script = "<script>window.open('/Quotation/Quotation-" + QuotationID + "_1" + ".pdf" + "', '_blank');</script>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenPDF", script);
                        // To save changes before closing
                        doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges);
                        wordApp.Quit();

                    }
                    catch (Exception ex)
                    {
                        string s1 = ex.Message;
                        Response.Write(s1);
                    }
                    finally
                    {
                        if (doc != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                            doc = null;
                            wordApp = null;
                        }

                        //change customer inquiry Status to CRF Received
                        _cm.inquiryId = InquiryId;
                        _cm.inquiryStatusId = 1010;//Quotation New
                        _cm.UpdateInquiryStatus(_cm);
                        GC.Collect();
                    }
                }

            }
        }

        public void CreateInstrumentTable(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, System.Data.DataTable dt, decimal subTotal, decimal VAT, decimal grandTotal)
        {
            var range = doc.Content;
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range != null)
            {
                // 4. Create a table at placeholder
                int rows = dt.Rows.Count + 2; // +1 for header +1 for sub total
                int cols = dt.Columns.Count;

                Microsoft.Office.Interop.Word.Table wordTable = doc.Tables.Add(range, rows, cols);
                wordTable.Borders.Enable = 1;
                // 5. Insert column headers
                for (int c = 0; c < cols; c++)
                {
                    wordTable.Cell(1, c + 1).Range.Text = dt.Columns[c].ColumnName;
                    wordTable.Cell(1, c + 1).Range.Bold = 1;
                }


                // 6. Insert data rows
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        wordTable.Cell(r + 2, c + 1).Range.Text = dt.Rows[r][c].ToString();
                    }
                }
                int r1 = dt.Rows.Count + 2;
                for (int c = 0; c < cols; c++)
                {
                    if (c == 4)
                    {
                        wordTable.Cell(r1, c + 1).Range.Text = "Sub Total\r\nPPN 10%\r\nTotal\r\n";
                    }
                    if (c == 5)
                    {
                        wordTable.Cell(r1, c + 1).Range.Text = subTotal + "\r\n" + VAT + "\r\n" + grandTotal;
                    }

                }

            }
        }


    }
}
