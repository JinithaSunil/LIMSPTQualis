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
    public partial class GenerateCoverLetter : System.Web.UI.Page
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
                string InvoiceID = "";
                string InvoiceNo = "";
                string InvoiceDate = "";
                string CompanyName = "";
                string ContactPerson = "";
                string Address1 = "";
                string Address2 = "";
                string Dept = "";
                string Fax = "";
                string FromTelephone = "";
                string FromEmail = "";
                decimal SubTotal = 0;
                decimal Discount = 0;
                decimal Totalbeforetax = 0;
                decimal VAT = 0;
                decimal Grandtotal = 0;
                string PreparedBy = "";
                string Designation = "";
                string DueDate = "";

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
                    InvoiceID = dtQM.Rows[0]["InvoiceID"].ToString();
                    InvoiceDate = DateTime.Now.ToString("dd-MM-yyyy");
                    InvoiceNo = dtQM.Rows[0]["InvoiceNumber"].ToString();
                    CompanyName = dtQM.Rows[0]["CompanyName"].ToString();
                    Address1 = dtQM.Rows[0]["AddressLine1"].ToString();
                    Address2 = dtQM.Rows[0]["AddressLine2"] + " " +
                                dtQM.Rows[0]["AddressLine3"] + " <br/>" +
                                dtQM.Rows[0]["District"] + " " +
                                dtQM.Rows[0]["State"] + " " +
                                dtQM.Rows[0]["Pincode"] + " " +
                                dtQM.Rows[0]["Country"].ToString();   
                                        
                    Discount = Convert.ToDecimal(dtQM.Rows[0]["Discount"].ToString());                    
                    VAT = Convert.ToDecimal(dtQM.Rows[0]["VATAmount"].ToString());
                    Grandtotal = Convert.ToDecimal(dtQM.Rows[0]["GrandTotal"].ToString());
                    Totalbeforetax = Grandtotal - VAT;
                    SubTotal = Totalbeforetax + Discount;                    
                    DueDate = Convert.ToDateTime(dtQM.Rows[0]["DueDate"]).ToString("dd-MM-yyyy");
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
                    <table>
                        <tr><td colspan='6'><hr class='dashed-line'></td></tr>
                        <tr>
                            <td style='height: 30px; width: 10%; text-align:center'>No. Invoice</td>
                            <td style='height: 30px; width: 20%; text-align:center'>Tgl Inv.</td>
							<td style='width: 20%; text-align: center;'>Nilai Penjualan</td>
                            <td style='width: 10%; text-align: center;'>Tgl FP</td>
                            <td style='width: 20%; text-align: center;'>PPN (Rp)</td>
                            <td style='width: 20%; text-align: center;'>Tgl J. Tempo</td>
                        </tr><tr><td colspan='6'><hr class='dashed-line'></td></tr>";


                // Loop through the DataTable rows and dynamically create table rows
               

                    // Add each row to the table
                    tableHtml += $@"
                        <tr>
                            <td style='text-align:center'>{InvoiceNo}</td>
                            <td style='text-align:center'>{InvoiceDate}</td>
                            <td style='text-align:center'> {Grandtotal}</td>
                            <td style='text-align:center'>{InvoiceDate}</td>
                            <td style='text-align:center'>{VAT}</td>
                            <td style='text-align:center'>{DueDate}</td>
                        </tr>";

                // Close the table tag
                tableHtml += "</table>";

                // Prepare the email body with the dynamic table
                string body = objEmail.PrepareMailBodyWith("CoverLetter.html",
                    "{CompanyName}", CompanyName,
                    "{AddressLine1}", Address1,
                    "{AddressLine2}", Address2,
                    "{PreparedBy}", PreparedBy.ToString(),
                    "{Items}", tableHtml
                );

                string folderPath = Server.MapPath("/Invoice");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Write the file to the server
                string filename = Server.MapPath("/Invoice/CoverLetter-" + InvoiceID + ".htm");
                System.IO.File.WriteAllText(filename, body);

                // Read the generated HTML file and display it on the page
                string html = File.ReadAllText(filename);
                divRequest.InnerHtml = html;

            }
        }
    }
}
