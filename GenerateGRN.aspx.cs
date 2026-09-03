using LIMS.DAL;
using OpenXmlPowerTools;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
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
    public partial class GenerateGRN : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        mailer objEmail = new mailer();
        string InwardID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                
                InwardID = Request.QueryString["ID"].ToString();
                //string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(InwardID));
                //InwardID = _cm.Decode(encodedEventID);
                InwardID = _cm.Decode(InwardID);
                Session["InwardID"] = InwardID;

                string InwardDate = "";
                string GRNNumber = "";
                string vendorID = "";
                string VendorName = "";
                string AddressLine1 = "";
                string AddressLine2 = "";
                string PurchaseOrderID = "";
                string PONumber = "";
                string PODate = "";
                string SignatureReception = "";
                string SignaturePRRequestedBy = "";
                string PRList = "";

                // Get the quotation details
                DataTable dtQD = _cm.GetGRNDetails(Convert.ToInt32(InwardID));
                if (dtQD.Rows.Count > 0)
                {                                
                    InwardDate = Convert.ToDateTime(dtQD.Rows[0]["ReceivedDate"]).ToString("dd-MM-yyyy");
                    GRNNumber = dtQD.Rows[0]["GRNNumber"].ToString();
                    PurchaseOrderID = dtQD.Rows[0]["PurchaseOrderID"].ToString();
                    PONumber = dtQD.Rows[0]["PONumber"].ToString();
                    PODate = Convert.ToDateTime(dtQD.Rows[0]["PODate"]).ToString("dd-MM-yyyy");
                    vendorID = dtQD.Rows[0]["VendorID"].ToString();                   

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
                    DataTable dtPRList = _cm.GetPRList(PurchaseOrderID);
                    if(dtPRList.Rows.Count > 0)
                    {
                        foreach(DataRow dr in dtPRList.Rows)
                        {
                            PRList += "PR-" + dr["PRNumber"].ToString() + ",";
                        }
                    }
                    StringBuilder sbAttachments = new StringBuilder();
                    string DeliveryOrder = "", GRNFileName="";
                    DataTable dtGRN = _cm.GetGRNDO(Convert.ToInt32(InwardID));

                    if (dtGRN.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtGRN.Rows)
                        {
                            string documentName = dr["DocumentName"].ToString();
                            string fileName = dr["GRNFileName"].ToString();

                            sbAttachments.Append($@"
                                        <div>
                                            <a href='UploadedFiles/GRNDocs/{fileName}' target='_blank'>
                                                📎 {documentName}
                                            </a>
                                        </div>");
                        }
                    }
                    string tableHtml = @"
                    <table>
                        <tr>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 30%; text-align: center;'>DESCRIPTION</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>ORDER QTY</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 10%; text-align: center;'>RECEIVED QTY</td>
                            <td style='border-top:1px solid !important;border-bottom:1px solid !important;width: 15%; text-align: center;'>PR REQUESTED BY</td>
                        </tr>";

                    // Loop through the DataTable rows and dynamically create table rows
                    foreach (DataRow row in dtQD.Rows)
                    {
                         // Add each row to the table
                        tableHtml += $@"
                        <tr><td>" + row["ItemName"].ToString() + "</td>" +
                              @"<td style='text-align:center'>" + Convert.ToInt32(row["PO_OrderedQty"]).ToString() + "</td>" +
                              @"<td style='text-align:center'>" + Convert.ToInt32(row["ReceivedQty"]).ToString() + "</td>" +
                              @"<td style='text-align:center'>" + row["EmployeeName"].ToString() + "</td></tr>";
                    }
                    tableHtml += $@"<tr><td colspan='4'><hr/></td></tr>";
                    if (dtQD.Rows.Count < 10)
                    {
                        for (int i = dtQD.Rows.Count; i < 10; i++)
                        {
                            tableHtml += $@"<tr><td colspan='4'>&nbsp;</td></tr>";
                        }
                    }
                    // Close the table tag
                    tableHtml += "</table>";

                    // Prepare the email body with the dynamic table
                    string body = objEmail.PrepareMailBodyWith("GRN.html",
                    "{InwardDate}", InwardDate,
                        "{GRNNumber}", GRNNumber,
                        "{VendorName}", VendorName,
                        "{AddressLine1}", AddressLine1,
                        "{AddressLine2}", AddressLine2,
                        "{PONumber}", PONumber,
                        "{PODate}", PODate,
                        "{PRList}", PRList,
                        "{SignatureReception}", SignatureReception,
                        "{SignaturePRRequestedBy}", SignaturePRRequestedBy,
                        "{GRN}", tableHtml,
                        "{Documents}", sbAttachments.ToString()
                    );

                    string folderPath = Server.MapPath("/GRN");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Write the file to the server
                    string filename = Server.MapPath("/GRN/" + InwardID + ".htm");
                    System.IO.File.WriteAllText(filename, body);

                    // Read the generated HTML file and display it on the page
                    string html = File.ReadAllText(filename);
                    divRequest.InnerHtml = html;
                }
            }
        }
    }
}
