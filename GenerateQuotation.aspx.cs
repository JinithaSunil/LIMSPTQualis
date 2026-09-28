using LIMS.DAL;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateQuotation : System.Web.UI.Page
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
                Revision = Convert.ToInt32(Request.QueryString["r"].ToString());
                Session["QuotationID"] = QuotationID;
                Session["Revision"] = Revision;

                int UserID = 0;
                string RequestNo = "";
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
                string CurrencyCode = "";
                string Remarks = "";
                decimal Quantity = 0;
                decimal Price = 0;
                decimal Amount = 0;
                decimal SubTotal = 0;
                decimal Discount = 0;
                decimal Totalbeforetax = 0;
                decimal VAT = 0;
                decimal Grandtotal = 0;
                string EnquiryID = "";

                string PreparedBy = "";
                string Designation = "";


                //ContactPerson = "Penny Wijaya";
                //Dept = "Sale/Marketing";
                //Fax = "021 4585 0661";
                //FromTelephone = "021 4585 0885";
                //FromEmail = "penny.wijaya@qualis-indonesia.com";
                DataTable dtRequest = _cm.getQuoatationRequest(QuotationID);
                if (dtRequest.Rows.Count > 0)
                {
                    RequestNo = "R/"+dtRequest.Rows[0]["RequestNo"].ToString();
                }
                DataTable dtQM = _cm.getquotationmaster(QuotationID);
                if (dtQM.Rows.Count > 0)
                {
                    UserID = Convert.ToInt32(dtQM.Rows[0]["UserID"].ToString());
                    EnquiryID = dtQM.Rows[0]["EnquiryID"].ToString();
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
                    CurrencyCode = dtQM.Rows[0]["CurrencyCode"].ToString();
                    Remarks = dtQM.Rows[0]["Remarks"].ToString();
                    //Dept = dtQM.Rows[0][""].ToString();
                    Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());
                    VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                    Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());
                    Totalbeforetax = Grandtotal - VAT;
                    SubTotal = Totalbeforetax + Discount;
                }
                DataTable dtEmpDetails = _cm.getLoginUserDetails(UserID);
                if (dtEmpDetails.Rows.Count > 0)
                {
                    ContactPerson = dtEmpDetails.Rows[0]["EmployeeName"].ToString();
                    Dept = dtEmpDetails.Rows[0]["EmployeeDepartmentName"].ToString();
                    Fax = dtEmpDetails.Rows[0]["ContactNumber"].ToString();
                    FromTelephone = dtEmpDetails.Rows[0]["ContactNumber"].ToString();
                    FromEmail = dtEmpDetails.Rows[0]["Email"].ToString();
                }

                DataTable dtQMEmpName = _cm.getquotationmasterEmpName(QuotationID);
                if (dtQMEmpName.Rows.Count > 0)
                {
                    PreparedBy = dtQMEmpName.Rows[0]["EmployeeName"].ToString();
                    Designation = dtQMEmpName.Rows[0]["DesignationName"].ToString();
                }


                // Get the quotation details
                DataTable dtQD = _cm.getquotationdetailsNew(QuotationID);
                //string SampleDescription = dtQD.Rows[0]["TestItems"].ToString();
                // Prepare the table header for the HTML
                string tableHtml = @"
                    <table style='height:200px; border: 1px solid'>
                        <tr>
                            <td style='border: 1px solid !important'>No.</td>
                            <td style='border: 1px solid !important; height: 30px; width: 35%; text-align:center'>Test Items</td>
                            <td style='border: 1px solid !important; height: 30px; width: 25%; text-align:center'>Metoda</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Quantity</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Price</td>
                            <td style='border: 1px solid !important; width: 10%; text-align: center;'>Amount("+CurrencyCode+")</td>" +
                      @"  </tr>";
                int rowNumber = 1;
                // Loop through the DataTable rows and dynamically create table rows
                foreach (DataRow row in dtQD.Rows)
                {
                    string sampleName = row["TAFSampleName"].ToString();
                    string testItems = row["TestItems"].ToString();
                    string testItemDetails = row["TestItemDetails"].ToString();
                    testItemDetails = testItemDetails.Replace(":", ":<br>").Replace(";", ";<br>");

                    string sampleDescription = row["SampleDescription"].ToString();

                    string testMethods = row["TestMethods"].ToString();
                    string formattedsampleDescription = Regex.Replace(sampleDescription, @"(?<!^)(?=[A-Z]\.)", "<br>");

                    int quantity = Convert.ToInt32(row["Quantity"]);
                    decimal price = Convert.ToDecimal(row["Price"]);
                    decimal amount = Convert.ToDecimal(row["TotalAmount"]);

                    string strPrice = "", strAmount = "";
                    strPrice = _cm.ToAnyCurrency(CurrencyCode,price);
                    strAmount = _cm.ToAnyCurrency(CurrencyCode,amount);
                    
                    // Add each row to the table
                    tableHtml += $@"
                        <tr>
                            <td style='border: 1px solid !important;'>{rowNumber}</td>
                            <td style='border: 1px solid !important;'><b><u>{testItems}</u></b> <b>{sampleDescription}</b><br/>{testItemDetails}</td>
                            <td style='border: 1px solid !important;'>{testMethods}</td>
                            <td style='border: 1px solid !important; text-align:right'>{quantity}</td>
                            <td style='border: 1px solid !important; text-align:right'>{strPrice}</td>
                            <td style='border: 1px solid !important; text-align: right'>{strAmount}</td>
                        </tr>";

                    rowNumber++;  // Increment the row number
                }
                //Prepared By signature
                
                StringBuilder SignPreparedBy = new StringBuilder();
                DataTable dtPreparedBy = new DataTable();
                dtPreparedBy = _cm.getQuotationPreparedBy(QuotationID);
                if (dtPreparedBy.Rows.Count > 0)
                {
                    string PreparedDate = Convert.ToDateTime(dtPreparedBy.Rows[0]["QuotationDate"]).ToString("dd-MM-yyyy HH:mm");
                    if (dtPreparedBy.Rows[0]["DigiSign"].ToString() != "")
                    {
                        SignPreparedBy.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                        SignPreparedBy.Append("<div> <img src='assets/img/Signature/" + dtPreparedBy.Rows[0]["ID"].ToString() + ".png' width='auto' height='50'></div>");
                        SignPreparedBy.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                        SignPreparedBy.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + PreparedDate + "</div>");
                        SignPreparedBy.Append("</div></div>");
                    }
                }
               
                // Close the table tag
                tableHtml += "</table>";

                string strSubTotal = "", strDiscount = "", strDiscountPercent = "", strTotalbeforetax = "", strVAT = "", strGrandtotal = "", strBalanceDue = "";
                strSubTotal = _cm.ToAnyCurrency(CurrencyCode, SubTotal);
                strDiscount = _cm.ToAnyCurrency(CurrencyCode, Discount);
                strTotalbeforetax = _cm.ToAnyCurrency(CurrencyCode,Totalbeforetax);
                strVAT = _cm.ToAnyCurrency(CurrencyCode,VAT);
                strGrandtotal = _cm.ToAnyCurrency(CurrencyCode,Grandtotal);
                strBalanceDue = _cm.ToAnyCurrency(CurrencyCode,(Grandtotal - Discount)).ToString();
                
                strDiscountPercent = Math.Round(((Discount / SubTotal) * 100),0).ToString() + "%";
                // Prepare the email body with the dynamic table
                string body = objEmail.PrepareMailBodyWith("Quotation.html",
                    "{Date}", Date,
                    "{RequestNo}", RequestNo,
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
                    //"{SampleDescription}", SampleDescription,
                    "{TestItems}", tableHtml,
                    "{SubTotal}", strSubTotal,
                    "{DiscountPercent}", "(" + strDiscountPercent + ")",
                    "{Discount}", strDiscount,
                    "{Totalbeforetax}", strTotalbeforetax,
                    "{VAT}", strVAT,
                    "{Grandtotal}", strGrandtotal,
                    "{PreparedBy}", PreparedBy.ToString(),
                    "{Designation}", Designation.ToString(),
                    "{SignPreparedBy}", SignPreparedBy.ToString()
                );

                // Convert the body to string and save it to an HTML file
                string str = body.ToString();
                string filename = Server.MapPath("//Quotation/Quotation-" + QuotationID + "_" + Revision + ".htm");
                File.WriteAllText(@filename, str);

                // Change customer inquiry Status to CRF Received
                _cm.inquiryStatusId = 1010;
                _cm.UpdateInquiryStatus(_cm);
                _cm.inquiryId = Convert.ToInt32(EnquiryID);
                _cm.UpdateInquiryQuotationStatus(_cm);
                GC.Collect();

                // Read the generated HTML file and display it in the page
                string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Quotation/Quotation-" + QuotationID + "_" + Revision + ".htm"));
                divRequest.InnerHtml = html;
            }
        }
    }
}
