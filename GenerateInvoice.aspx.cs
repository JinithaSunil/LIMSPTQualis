using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateInvoice : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int InvoiceID = 0; int QuotationID = 0;
        int Revision = 0;
        mailer objEmail = new mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                InvoiceID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                //Revision = Convert.ToInt32(Request.QueryString["r"].ToString());
                Session["InvoiceID"] = InvoiceID;
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
                string CurrencyCode = "";
                string Remarks = "";
                decimal Quantity = 0;
                decimal Price = 0;
                decimal Amount = 0;
                decimal SubTotal = 0;
                decimal DPP = 0;
                decimal Discount = 0;
                decimal DownPayment = 0;
                decimal Totalbeforetax = 0;
                decimal VAT = 0;
                decimal Grandtotal = 0;
                string PreparedBy = "";
                string Designation = "";
                string PIC = "";
                string PONumber = "";
                string DueDate = "";
                string QuotationNo = "";
                if (Session["UserID"] != null)
                {
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


                    DataTable dtQM = _cm.GetInvoice(InvoiceID);
                    if (dtQM.Rows.Count > 0)
                    {
                        QuotationID = Convert.ToInt32(dtQM.Rows[0]["QuotationID"].ToString());
                        QuotationNo = dtQM.Rows[0]["QuotationNumber"].ToString();
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
                        CurrencyCode = dtQM.Rows[0]["CurrencyCode"].ToString();
                        Remarks = dtQM.Rows[0]["Remarks"].ToString();
                        if (dtQM.Rows[0]["Discount"].ToString() != "")
                        {
                            Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());
                        }
                        if (dtQM.Rows[0]["DownPayment"].ToString() != "")
                        {
                            DownPayment = Convert.ToDecimal(dtQM.Rows[0]["DownPayment"].ToString());
                        }
                        else
                        {
                            DownPayment = 0;
                        }
                        if (dtQM.Rows[0]["VATAmount"].ToString() != "")
                        {
                            VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                        }
                        else
                        {
                            VAT = 0;
                        }
                        if (dtQM.Rows[0]["TotalAmount"].ToString() != "")
                        {
                            Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["TotalAmount"].ToString());
                        }
                        else
                        {
                            Grandtotal = 0;
                        }
                        Totalbeforetax = Grandtotal - VAT;
                        DPP = Totalbeforetax * (11 / 12);
                        if (dtQM.Rows[0]["SubTotal"].ToString() != "")
                        {
                            SubTotal = Convert.ToDecimal(dtQM.Rows[0]["SubTotal"].ToString());
                        }
                        
                        PIC = dtQM.Rows[0]["PIC"].ToString();
                        PONumber = dtQM.Rows[0]["PONumber"].ToString();
                        DueDate = Convert.ToDateTime(dtQM.Rows[0]["DueDate"]).ToString("dd-MM-yyyy");
                    }


                    DataTable dtQMEmpName = _cm.getquotationmasterEmpName(QuotationID);
                    if (dtQMEmpName.Rows.Count > 0)
                    {
                        PreparedBy = dtQMEmpName.Rows[0]["EmployeeName"].ToString();
                        Designation = dtQMEmpName.Rows[0]["DesignationName"].ToString();
                    }


                    // Get the quotation details
                    DataTable dtQD = _cm.GetInvoiceDetails(InvoiceID);

                    // Prepare the table header for the HTML
                    string tableHtml = @"
                    <table style='height:200px;'>
                        <tr>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;height: 30px; width: 10%; text-align:center'>REQUEST NO.</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 30%; text-align: center;'>DESCRIPTION</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>QTY</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 15%; text-align: center;'>UNIT PRICE</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 15%; text-align: center;'>TOTAL(" + CurrencyCode + ")</td>" +
                          @"  </tr>";

                    int rowNumber = 1;

                    // Loop through the DataTable rows and dynamically create table rows
                    foreach (DataRow row in dtQD.Rows)
                    {
                        string requestNo = row["RequestNo"].ToString();
                        string WONo = row["WorkOrderNumber"].ToString();
                        string sampleName = row["TestItems"].ToString();

                        string testItems = row["TestItemDetails"].ToString();
                        testItems = testItems.Replace(":", ":<br>").Replace(";", ";<br>");

                        string sampleDescription = row["ItemDescription"].ToString();//row["SampleDescription"].ToString();
                        string formattedsampleDescription = Regex.Replace(sampleDescription, @"(?<!^)(?=[A-Z]\.)", "<br>");

                        int quantity = Convert.ToInt32(row["Quantity"]);
                        decimal price = Convert.ToDecimal(row["Price"]);
                        decimal amount = Convert.ToDecimal(row["TotalAmount"]);

                        string strPrice = "", strAmount = "";
                        switch (CurrencyName)
                        {
                            case "Indonesian Rupiah":
                                strPrice = _cm.ToIDR(price);
                                strAmount = _cm.ToIDR(amount);
                                break;
                            case "US Dollar":
                                strPrice = _cm.ToUSD(SubTotal);
                                strAmount = _cm.ToUSD(Discount);
                                break;
                            default:
                                break;
                        }

                        // Add each row to the table
                        tableHtml += $@"
                        <tr>
                            <td style='text-align:center'>{requestNo}</td>
                            <td style='text-align:left'> <b>{sampleName}</b> <br>{formattedsampleDescription}<br>{testItems}</td>
                            <td style='text-align:center'>{quantity}</td>
                            <td style='text-align:right'>{strPrice}</td>
                            <td style='text-align:right'>{strAmount}</td>
                        </tr>";

                        rowNumber++;  // Increment the row number
                    }

                    // Close the table tag
                    tableHtml += "</table>";
                    string strSubTotal = "", strDownPayment = "", strTotalbeforetax = "", strDPP="", strVAT = "", strGrandtotal = "", strBalanceDue = "";
                    switch (CurrencyName)
                    {
                        case "Indonesian Rupiah":
                            strSubTotal = _cm.ToIDR(SubTotal);
                            strDownPayment = _cm.ToIDR(DownPayment);
                            strTotalbeforetax = _cm.ToIDR(Totalbeforetax);
                            strDPP = _cm.ToIDR(DPP);
                            strVAT = _cm.ToIDR(VAT);                           
                            strGrandtotal = _cm.ToIDR(Grandtotal);
                            strBalanceDue = _cm.ToIDR((Grandtotal - DownPayment)).ToString();
                            break;
                        case "US Dollar":
                            strSubTotal = _cm.ToUSD(SubTotal);
                            strDownPayment = _cm.ToUSD(DownPayment);
                            strTotalbeforetax = _cm.ToUSD(Totalbeforetax);
                            strDPP = _cm.ToUSD(DPP);
                            strVAT = _cm.ToUSD(VAT);
                            strGrandtotal = _cm.ToUSD(Grandtotal);
                            strBalanceDue = _cm.ToUSD((Grandtotal - DownPayment)).ToString();

                            break;
                        default:
                            break;
                    }
                    // Prepare the email body with the dynamic table
                    string body = objEmail.PrepareMailBodyWith("Invoice.html",
                        "{Date}", InvoiceDate,
                        "{InvoiceNo}", InvoiceNo,
                        "{QuotationNo}", QuotationNo,
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
                        "{SubTotal}", strSubTotal,
                        "{DownPayment}", strDownPayment,
                        "{BalanceDue}", strBalanceDue,
                        "{Discount}", Discount.ToString(),
                        "{Totalbeforetax}", strTotalbeforetax,
                        "{DPP}", strDPP,
                        "{VAT}", strVAT,
                        "{Grandtotal}", strGrandtotal,
                        "{PreparedBy}", PreparedBy.ToString(),
                        "{Designation}", Designation.ToString(),
                        "{PIC}", PIC,
                        "{PONumber}", PONumber,
                        "{DueDate}", DueDate
                    );

                    string folderPath = Server.MapPath("/Invoice");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Write the file to the server
                    string filename = Server.MapPath("/Invoice/INV-" + QuotationID + ".htm");
                    System.IO.File.WriteAllText(filename, body);

                    // Read the generated HTML file and display it on the page
                    string html = File.ReadAllText(filename);
                    divRequest.InnerHtml = html;

                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}
