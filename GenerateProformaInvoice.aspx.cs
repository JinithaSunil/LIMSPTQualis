using LIMS.DAL;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateProformaInvoice : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int QuotationID = 0;
        int Revision = 0;
        mailer objEmail = new mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                QuotationID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                //Revision = Convert.ToInt32(Request.QueryString["r"].ToString());
                Session["QuotationID"] = QuotationID;
                //Session["Revision"] = Revision;

                string InvoiceNo = "";
                string InvoiceDate = "";
                int Status = 1;
                string CompanyName = "";
                string CompanyID = "";
                string ContactPerson = "";
                string Address1 = "";
                string Address2 = "";
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
                decimal Quantity = 0;
                decimal Price = 0;
                decimal Amount = 0;
                decimal SubTotal = 0;
                decimal Discount = 0;
                decimal Totalbeforetax = 0;
                decimal VAT = 0;
                decimal Grandtotal = 0;
                string PreparedBy = "";
                string Designation = "";
                string PIC = "";
                string Currency = "IDR";

                int UserID = Convert.ToInt32(Session["UserID"].ToString());
                DataTable dtEmpDetails = _cm.getLoginUserDetails(UserID);
                if (dtEmpDetails.Rows.Count > 0)
                {
                    ContactPerson = dtEmpDetails.Rows[0]["EmployeeName"].ToString();
                    Dept = dtEmpDetails.Rows[0]["EmployeeDepartmentName"].ToString();
                    Fax = dtEmpDetails.Rows[0]["ContactNumber"].ToString();
                    FromTelephone = dtEmpDetails.Rows[0]["ContactNumber"].ToString();
                    FromEmail = dtEmpDetails.Rows[0]["Email"].ToString();
                }


                DataTable dtQM = _cm.getquotationmaster(QuotationID);
                if (dtQM.Rows.Count > 0)
                {
                    InvoiceDate = DateTime.Now.ToString("dd-MM-yyyy");
                    InvoiceNo = dtQM.Rows[0]["InvoiceNumber"].ToString();
                    CompanyID = dtQM.Rows[0]["CompanyID"].ToString();
                    CompanyName = dtQM.Rows[0]["CompanyName"].ToString();
                    Address1 = dtQM.Rows[0]["AddressLine1"].ToString();
                    Address2 = dtQM.Rows[0]["AddressLine2"] + " " +
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
                                        
                    Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());                    
                    VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                    Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());
                    Totalbeforetax = Grandtotal - VAT;
                    SubTotal = Totalbeforetax + Discount;
                    PIC = dtQM.Rows[0]["EmployeeName"].ToString();
                }


                DataTable dtQMEmpName = _cm.getquotationmasterEmpName(QuotationID);
                if (dtQMEmpName.Rows.Count > 0)
                {
                    PreparedBy = dtQMEmpName.Rows[0]["EmployeeName"].ToString();
                    Designation = dtQMEmpName.Rows[0]["DesignationName"].ToString();
                }


                // Get the quotation details
                DataTable dtQD = _cm.getquotationdetailsNew(QuotationID);

                // Prepare the table header for the HTML
                string tableHtml = @"
                    <table style='height:200px;'>
                        <tr>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important; height: 30px; width: 25%; text-align:center'>Description</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important; width: 10%; text-align: center;'>Quantity</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important; width: 10%; text-align: center;'>Price</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important; width: 10%; text-align: center;'>Amount(IDR)</td>
                        </tr>";

                int rowNumber = 1;

                // Loop through the DataTable rows and dynamically create table rows
                foreach (DataRow row in dtQD.Rows)
                {
                    string sampleName = row["TestItems"].ToString();

                    string testItems = row["TestItemDetails"].ToString();
                    testItems = testItems.Replace(":", ":<br>").Replace(";", ";<br>");

                    string sampleDescription = row["SampleDescription"].ToString();
                    string formattedsampleDescription = Regex.Replace(sampleDescription, @"(?<!^)(?=[A-Z]\.)", "<br>");

                    decimal quantity = Convert.ToDecimal(row["Quantity"]);
                    decimal price = Convert.ToDecimal(row["Price"]);
                    decimal amount = Convert.ToDecimal(row["TotalAmount"]);

                    // Add each row to the table
                    tableHtml += $@"
                        <tr>
                            <td> <b>{sampleName}</b> <br>{formattedsampleDescription}<br>{testItems}</td>
                            <td style='text-align:center'>{quantity}</td>
                            <td style='text-align:right'>{price}</td>
                            <td style='text-align: right'>{amount}</td>
                        </tr>";

                    rowNumber++;  // Increment the row number
                }

                // Close the table tag
                tableHtml += "</table>";

                // Prepare the email body with the dynamic table
                string body = objEmail.PrepareMailBodyWith("ProformaInvoice.html",
                    "{Date}", InvoiceDate,
                    "{InvoiceNo}", InvoiceNo,
                    "{CompanyName}", CompanyName,
                    "{ContactPerson}", ContactPerson,
                    "{AddressLine1}", Address1,
                    "{AddressLine2}", Address2,
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
                    "{TestItems}", tableHtml,  // Insert the dynamic table here
                    "{SubTotal}", SubTotal.ToString(),
                    "{Discount}", Discount.ToString(),
                    "{Totalbeforetax}", Totalbeforetax.ToString(),
                    "{VAT}", VAT.ToString(),
                    "{Grandtotal}", Grandtotal.ToString(),
                    "{BalanceDue}", Grandtotal.ToString(),
                    "{PreparedBy}", PreparedBy.ToString(),
                    "{Designation}", Designation.ToString(),
                    "{PIC}",PIC,
                    "{Curr}",Currency
                );

                string folderPath = Server.MapPath("/ProformaInvoice");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Write the file to the server
                string filename = Server.MapPath("/ProformaInvoice/PI-" + QuotationID + ".htm");
                System.IO.File.WriteAllText(filename, body);

                // Read the generated HTML file and display it on the page
                string html = File.ReadAllText(filename);
                divRequest.InnerHtml = html;

            }
        }
    }
}
