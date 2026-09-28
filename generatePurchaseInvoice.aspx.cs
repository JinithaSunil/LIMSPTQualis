using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit.HtmlEditor.ToolbarButtons;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using iTextSharp.tool.xml.html;
using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using static LIMS.DAL.GLModule;

namespace LIMS
{
    public partial class generatePurchaseInvoice : System.Web.UI.Page
    {
        DAL.CM _cm = new CM();
        mailer objEmail = new mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPI();
            }
        }

        private void BindPI()
        {
            int PIInvoiceID = 0; string PurchaseOrderID = ""; string PRNo = "";
            DataTable dt = new DataTable();
            DataTable dtDetails = new DataTable();
            DataTable dtGRN = new DataTable();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            string DeliveryOrder = "", VendorInvoiceNumber="";
            decimal Rate = 0, Amount = 0;
            string strPrice = "", strAmount = "", TIN="", CreatedBy="", DesignationName="";

            if (Request.QueryString["i"] != null)//Purchase InvoiceID
            {
                PIInvoiceID = Convert.ToInt32(Request.QueryString["i"]);

                sb.Append(@"<table style='width:100%' class='tab1'>");
                string CurrencyCode = "", CurrencyName = "";
                dt = _cm.GetPIHeader(PIInvoiceID);
                if (dt.Rows.Count > 0)
                {
                    CreatedBy = dt.Rows[0]["EmployeeName"].ToString();
                    DesignationName = dt.Rows[0]["DesignationName"].ToString();
                    PurchaseOrderID = dt.Rows[0]["PurchaseOrderID"].ToString();
                    VendorInvoiceNumber = dt.Rows[0]["InvoiceNumber"].ToString();
                    TIN = dt.Rows[0]["PANNumber"].ToString();
                    DataTable dtPR = _cm.GetPOPR(PurchaseOrderID);
                    if (dtPR.Rows.Count > 0)
                    {
                        string PRNo1 = dtPR.Rows[0]["PR"].ToString();
                        PRNo = string.Join(", ",
                                            PRNo1.Split(',')
                                                 .Select(x => x.Trim())
                                                 .Distinct());
                    }
                    CurrencyCode = dt.Rows[0]["CurrencyCode"].ToString();
                    CurrencyName = dt.Rows[0]["CurrencyName"].ToString();

                    StringBuilder sbApproval = new StringBuilder();
                    StringBuilder sbAttachments = new StringBuilder();
                    dtDetails = _cm.GetPIDetails(PIInvoiceID);
                    if (dtDetails.Rows.Count > 0)
                    {
                        if (dtDetails.Rows[0]["InwardID"].ToString() != "")
                        {
                            dtGRN = _cm.GetGRNDO(Convert.ToInt32(dtDetails.Rows[0]["InwardID"].ToString()));
                            if (dtGRN.Rows.Count > 0)
                            {
                                DeliveryOrder = dtGRN.Rows[0]["DocumentName"].ToString();                                
                                foreach (DataRow dr in dtGRN.Rows)
                                {
                                    string documentName = dr["DocumentName"].ToString();
                                    string fileName = dr["GRNFileName"].ToString();

                                    sbAttachments.Append($@"
                                        <div>
                                            <a href='UploadedFiles/GRNDocs/{fileName}' target='_blank'>
                                                📎 DOFile
                                            </a>
                                        </div>");
                                }
                            }
                            sb.Append("<thead><tr><td>No.</td><td>GR Date</td><td>GR No.</td>");
                            sb.Append("<td>Item Code</td><td>Item</td><td>Qty</td><td>Measr</td>");
                            sb.Append("<td>Price</td><td>Disc.(%)</td><td>Amount</td><td>Currency</td></tr></thead><tbody>");

                            int counter = 0;

                            decimal subTotal = 0; decimal ppnRate = 0.11m;
                            foreach (DataRow dr in dtDetails.Rows)
                            {
                                Rate = Convert.ToDecimal(dr["Rate"].ToString());
                                Amount = Convert.ToDecimal(dr["Amount"].ToString());
                                subTotal += Amount;
                                strPrice = _cm.ToAnyCurrency(CurrencyCode,Rate);
                                strAmount = _cm.ToAnyCurrency(CurrencyCode, Amount);

                                sb.Append("<tr>");
                                sb.Append("<td>" + ++counter + "</td>");
                                sb.Append("<td>" + Convert.ToDateTime(dr["ReceivedDate"]).ToString("dd-MM-yyyy") + "</td>");
                                sb.Append("<td>" + dr["GRNNumber"].ToString() + "</td>");
                                sb.Append("<td>" + dr["ItemCode"].ToString() + "</td>");
                                sb.Append("<td>" + dr["ItemDescription"].ToString() + "</td>");
                                sb.Append("<td>" + Convert.ToInt32(dr["Quantity"]).ToString() + "</td>");
                                sb.Append("<td>" + dr["UOMName"].ToString() + "</td>");
                                sb.Append("<td>" + strPrice + "</td>");
                                sb.Append(
                                     dr["Discount"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["Discount"].ToString())
                                     ? $"<td>{Convert.ToInt32(dr["Discount"])}%</td>"
                                     : "<td></td>"
                                 );
                                sb.Append("<td>" + strAmount + "</td>");
                                sb.Append("<td style='text-align:center'>" + CurrencyCode + "</td>");
                                sb.Append("</tr>");
                            }
                            
                            decimal ppn = Math.Round(subTotal * ppnRate, 2);
                            decimal total = subTotal + ppn;

                            string strsubTotal = "", strppn = "", strtotal = "";
                            strsubTotal = _cm.ToAnyCurrency(CurrencyCode,subTotal);
                            strppn = _cm.ToAnyCurrency(CurrencyCode,ppn);
                            strtotal = _cm.ToAnyCurrency(CurrencyCode,total);

                            sb2.Append("<table class=footer-table'>");
                            sb2.Append("<tr><td width=80%' style='text-align:right'><b>SubTotal : </b></td><td colspan='2' style='text-align:right'><b>" + strsubTotal + "</b></td></tr>");
                            sb2.Append("<tr><td width=80%' style='text-align:right'><b>PPN : </b></td><td colspan='2' style='text-align:right'><b>" + strppn + "</b></td></tr>");
                            sb2.Append("<tr><td width=80%' style='text-align:right'><b>Total : </b></td><td colspan='2' style='text-align:right'><b>" + strtotal + "</b></td></tr>");
                            sb2.Append("</table>");
                        }

                        sb.Append("</tbody></table>");
                        //Approval
                        string Tier1 = "", Tier1Dept = "", Tier2 = "", Tier2Dept = "", Tier3 = "", Tier3Dept = "", Tier4 = "", Tier4Dept = "";
                        StringBuilder SignUser = new StringBuilder(), SignTier1 = new StringBuilder(), SignTier2 = new StringBuilder(), SignTier3 = new StringBuilder(), SignTier4 = new StringBuilder();

                        Tier1 = CreatedBy;
                        Tier1Dept = DesignationName;
                        SignTier1.Append("<div style='width:160px;text-align:center;font-family:Times New Roman;'>");
                        // SignTier1.Append("<div> <img src='assets/img/Signature/" + dtApproval.Rows[0]["ID"].ToString() + ".png' width='auto' height='40'></div>");
                        SignTier1.Append("<div style='font-family:SignatureFont,cursive;font-size:20px;line-height:40px;height:40px;font-weight:bold'>");
                        SignTier1.Append(Tier1);
                        SignTier1.Append("</div>");
                        SignTier1.Append("<div style='position:relative;display:inline-block;'><img src='assets/img/Signature/ValidSign.png' width='auto' height='100'>");
                        SignTier1.Append("<div style='position:absolute; top:85px; font-size:10px; font-weight:bold;'> Date/Time: " + Convert.ToDateTime(dt.Rows[0]["InvoiceDate"]).ToString("dd-MM-yyyy") + "</div>");
                        SignTier1.Append("</div></div>");

                        sbApproval.Append(@"<table class='tab1' width='100%' style='font-size:10px;'>
                    <tr>
                    <td style='width:25%; text-align:center'>Dibuat Oleh</td>
                    <td style='width:25%; text-align:center'>Mengetahui</td>
                    <td style='width:25%; text-align:center'>Disetujui Oleh</td>
                    <td style='width:25%; text-align:center'>Disetujui Oleh</td>
                    </tr>
                    <tr><td height='100px'>" + SignTier1.ToString() + "</td><td>" + SignTier2.ToString() + "</td><td>" + SignTier3.ToString() + "</td><td>" + SignTier4.ToString() + "</td></tr><tr>" +

                       @"<td style='width:25%; text-align:center'><b>" + Tier1 + "</b> / " + Tier1Dept + "</td>" +
                       @"<td style='width:25%; text-align:center'><b>" + Tier2 + "</b> / " + Tier2Dept + "</td> " +
                       @"<td style='width:25%; text-align:center'><b>" + Tier3 + "</b> / " + Tier3Dept + "</td> " +
                       @"<td style='width:25%; text-align:center'><b>" + Tier4 + "</b> / " + Tier4Dept + "</td> " +
                       @"</tr>
                    </table>");
                        string body = objEmail.PrepareMailBodyWith("PurchaseInvoice.html",
                            "{pgNo}", "1",
                            "{VendorName}", dt.Rows[0]["VendorName"].ToString(),
                            "{TIN}", TIN,
                            "{PINo}", dt.Rows[0]["PINumber"].ToString(),
                            "{VendorInvoiceNumber}", VendorInvoiceNumber,
                            "{PONo}", dt.Rows[0]["PONumber"].ToString(),
                            "{PRNo}", PRNo,
                            "{DeliveryOrder}", DeliveryOrder,
                            "{Documents}", sbAttachments.ToString(),
                            "{Date}", Convert.ToDateTime(dt.Rows[0]["InvoiceDate"]).ToString("dd-MM-yyyy"),
                            "{PurchaseInvoiceDetails}", sb.ToString(),
                            "{PIFooter}",sb2.ToString(),
                            "{CreatedBy}",CreatedBy,
                            "{Approval}", sbApproval.ToString());

                        string filename = Server.MapPath("//PurchaseInvoice/PI-" + PIInvoiceID + ".html");
                        System.IO.File.WriteAllText(@filename, body.ToString());
                        string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//PurchaseInvoice/PI-" + PIInvoiceID + ".html"));
                        divVoucher.InnerHtml = html;
                    }

                   
                }

            }
        }

    }

}
