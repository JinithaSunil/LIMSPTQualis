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
    public partial class GenerateDebitNote : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.APModule _ap =  new DAL.APModule();
        mailer objEmail = new mailer();
        string DebitNoteID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                DebitNoteID = Request.QueryString["ID"].ToString();
                Session["DebitNoteID"] = DebitNoteID;

                string DebitNoteDate = "";
                string DebitNoteNo = "";
                string vendorID = "";
                string VendorName = "";
                string AddressLine1 = "";
                string AddressLine2 = "";
                string TotalAmount = "";

                // Get the quotation details
                DataTable dtQD = _ap.GetDebitNoteDetails(DebitNoteID);
                if (dtQD.Rows.Count > 0)
                {
                    DebitNoteDate = Convert.ToDateTime(dtQD.Rows[0]["DebitNoteDate"]).ToString("dd-MM-yyyy");
                    DebitNoteNo = dtQD.Rows[0]["DebitNoteNo"].ToString();
                    vendorID = dtQD.Rows[0]["SupplierID"].ToString();
                    DataTable dt = _cm.GetVendorDetails(vendorID);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            VendorName = dr["VendorName"].ToString();
                            AddressLine1 = dr["AddressLine1"].ToString();
                            AddressLine2 = dr["AddressLine2"].ToString();
                        }
                    }
                    TotalAmount = dtQD.Rows[0]["TotalAmount"].ToString();
                    // Prepare the table header for the HTML
                    string tableHtml = @"
                    <table>
                        <tr>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 30%; text-align: center;'>DESCRIPTION</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>QTY</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 15%; text-align: center;'>PRICE</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 15%; text-align: center;'>AMOUNT(IDR)</td>
                        </tr>";

                    // Loop through the DataTable rows and dynamically create table rows
                    foreach (DataRow row in dtQD.Rows)
                    {
                        // Add each row to the table
                        tableHtml += $@"
                        <tr><td>" + row["ItemName"].ToString() + "</td>" +
                              @"<td>" + Convert.ToInt32(row["Qty"]).ToString() + "</td>" +
                              @"<td>" + row["Rate"].ToString() + "</td>" +
                              @"<td>" + row["Amount"].ToString() + "</td></tr>";
                    }
                    if (dtQD.Rows.Count < 10)
                    {
                        for(int i= dtQD.Rows.Count; i < 10; i++)
                        {
                            tableHtml += $@"<tr><td colspan='4'>&nbsp;</td></tr>";
                        }
                    }
                    tableHtml += $@"<tr><td colspan='4'><hr/></td></tr>";
                    tableHtml += $@" <tr><td colspan='2'>&nbsp;</td><td>Total</td><td>" + TotalAmount + "</td></tr>";
                    tableHtml += $@"<tr><td colspan='4'><hr/></td></tr>";
                    // Close the table tag
                    tableHtml += "</table>";

                    // Prepare the email body with the dynamic table
                    string body = objEmail.PrepareMailBodyWith("DebitNote.html",
                        "{DebitNoteDate}", DebitNoteDate,
                        "{DebitNoteNo}", DebitNoteNo,
                        "{VendorName}", VendorName,
                        "{AddressLine1}", AddressLine1,
                        "{AddressLine2}", AddressLine2,
                        "{DebitNoteItems}", tableHtml
                    );

                    string folderPath = Server.MapPath("/DebitNote");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Write the file to the server
                    string filename = Server.MapPath("/DebitNote/" + DebitNoteID + ".htm");
                    System.IO.File.WriteAllText(filename, body);

                    // Read the generated HTML file and display it on the page
                    string html = File.ReadAllText(filename);
                    divRequest.InnerHtml = html;
                }
            }
        }
    }
}
