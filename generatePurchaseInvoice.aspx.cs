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
            string DeliveryOrder = "";
            decimal Rate = 0, Amount = 0;
            string strPrice = "", strAmount = "";

            if (Request.QueryString["i"] != null)//Purchase InvoiceID
            {
                PIInvoiceID = Convert.ToInt32(Request.QueryString["i"]);

                sb.Append(@"<table style='width:100%' class='tab1'>");
                string CurrencyCode = "", CurrencyName = "";
                dt = _cm.GetPIHeader(PIInvoiceID);
                if (dt.Rows.Count > 0)
                {
                    PurchaseOrderID = dt.Rows[0]["PurchaseOrderID"].ToString();
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
                    dtDetails = _cm.GetPIDetails(PIInvoiceID);
                    if (dtDetails.Rows.Count > 0)
                    {
                        if (dtDetails.Rows[0]["InwardID"].ToString() != "")
                        {
                            dtGRN = _cm.GetGRNDO(Convert.ToInt32(dtDetails.Rows[0]["InwardID"].ToString()));
                            if (dtGRN.Rows.Count > 0)
                            {
                                DeliveryOrder = dtGRN.Rows[0]["DocumentName"].ToString();
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
                                switch (CurrencyName)
                                {
                                    case "Indonesian Rupiah":
                                        strPrice = _cm.ToIDR(Rate);
                                        strAmount = _cm.ToIDR(Amount);
                                        break;
                                    case "US Dollar":
                                        strPrice = _cm.ToUSD(Rate);
                                        strAmount = _cm.ToUSD(Amount);
                                        break;
                                    default:
                                        break;
                                }
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
                            switch (CurrencyName)
                            {
                                case "Indonesian Rupiah":
                                    strsubTotal = _cm.ToIDR(subTotal);
                                    strppn = _cm.ToIDR(ppn);
                                    strtotal = _cm.ToIDR(total);
                                    break;
                                case "US Dollar":
                                    strsubTotal = _cm.ToUSD(subTotal);
                                    strppn = _cm.ToUSD(ppn);
                                    strtotal = _cm.ToUSD(total);
                                    break;
                                default:
                                    break;
                            }
                            sb2.Append("<table class=footer-table'>");
                            sb2.Append("<tr><td width=80%' style='text-align:right'><b>SubTotal : </b></td><td colspan='2' style='text-align:right'><b>" + strsubTotal + "</b></td></tr>");
                            sb2.Append("<tr><td width=80%' style='text-align:right'><b>PPN : </b></td><td colspan='2' style='text-align:right'><b>" + strppn + "</b></td></tr>");
                            sb2.Append("<tr><td width=80%' style='text-align:right'><b>Total : </b></td><td colspan='2' style='text-align:right'><b>" + strtotal + "</b></td></tr>");
                            sb2.Append("</table>");
                        }

                        sb.Append("</tbody></table>");
                        string body = objEmail.PrepareMailBodyWith("PurchaseInvoice.html",
                            "{pgNo}", "1",
                            "{VendorName}", dt.Rows[0]["VendorName"].ToString(),
                            "{PINo}", dt.Rows[0]["PINumber"].ToString(),
                            "{PONo}", dt.Rows[0]["PONumber"].ToString(),
                            "{PRNo}", PRNo,
                            "{DeliveryOrder}", DeliveryOrder,
                            "{Date}", Convert.ToDateTime(dt.Rows[0]["InvoiceDate"]).ToString("dd-MM-yyyy"),
                            "{PurchaseInvoiceDetails}", sb.ToString(),
                            "{PIFooter}",sb2.ToString());

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
