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
using iTextSharp.tool.xml.html;
using LIMS.DAL;

namespace LIMS
{
    public partial class generateVoucher : System.Web.UI.Page
    {
        DAL.APModule _ap = new DAL.APModule();
        DAL.ARModule _ar = new DAL.ARModule();
        mailer objEmail = new mailer();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindVoucher();
            }
        }

        private void BindVoucher()
        {
            int paymentid = 0; int receiptid = 0; string voucherID = "";
            DataTable dt = new DataTable(); StringBuilder sb = new StringBuilder();
            if (Request.QueryString["o"] != null)//payments
            {
                string PaymentType = Request.QueryString["o"];

                if (Request.QueryString["x"] != null)
                {
                    paymentid = Convert.ToInt32(Request.QueryString["x"]);


                    sb.Append(@"<table style='width:100%' class='tab1'>");

                    if (PaymentType == "1") // PO Payments
                    {
                        dt = _ap.GetPOPayments(paymentid);
                        voucherID = dt.Rows[0]["VoucherCount"].ToString();
                        sb.Append("<tr><td>Supplier Inv</td><td>Purchase No.</td><td>Invoice Date</td>");
                        sb.Append("<td>Invoice Amount</td><td>Discounts</td><td>Previous Amounts</td>");
                        sb.Append("<td>Current Payment</td></tr>");

                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td>" + dr["InvoiceNumber"].ToString() + "</td>");
                            sb.Append("<td>" + dr["PONumber"].ToString() + "</td>");
                            sb.Append("<td>" + Convert.ToDateTime(dr["InvoiceDate"]).ToString("dd-MM-yyyy") + "</td>");
                            sb.Append("<td>" + dr["TotalAmount"].ToString() + "</td>");
                            sb.Append("<td>" + dr["Discounts"].ToString() + "</td>");
                            sb.Append("<td>" + dr["PreviousPayments"].ToString() + "</td>");
                            sb.Append("<td>" + dr["PaymentAmount"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                    }
                    if (PaymentType == "2")// Payment Requisition Payments
                    {
                        dt = _ap.GetPRPayments(paymentid);
                        voucherID = dt.Rows[0]["VoucherCount"].ToString();
                        sb.Append("<tr><td>Bank Account</td><td>Acc. No. </td><td>Acc. Name</td>");
                        sb.Append("<td>Amount</td><td>Description</td>");
                        sb.Append("<td>Current Payment</td></tr>");
                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append("<tr>");
                            sb.Append("<td>" + dr["BankAccountCode"].ToString() + "</td>");
                            sb.Append("<td>" + dr["COAAccountCode"].ToString() + "</td>");
                            sb.Append("<td>" + dr["AccountName"].ToString() + "</td>");
                            sb.Append("<td>" + dr["TotalPrice"].ToString() + "</td>");
                            sb.Append("<td>" + dr["ItemDescription"].ToString() + "</td>");
                            sb.Append("<td>" + dr["Amount"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                    }
                   
                    sb.Append("</table>");
                    string body = objEmail.PrepareMailBodyWith("Voucher.html",
                        "{pgNo}", "1",
                        "{PaymentType}", "Payment To:",
                        "{PaymentFromTo}", dt.Rows[0]["PayeeName"].ToString(),
                        "{No}", dt.Rows[0]["VoucherNumber"].ToString(),
                        "{Date}", Convert.ToDateTime(dt.Rows[0]["PaymentDate"]).ToString("dd-MM-yyyy"),
                        "{Amount}", dt.Rows[0]["PaymentAmount"].ToString(),
                        "{PaymentDetails}", sb.ToString());

                    string filename = Server.MapPath("//Voucher/BK-" + voucherID + ".html");
                    System.IO.File.WriteAllText(@filename, body.ToString());
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//Voucher/BK-" + voucherID + ".html"));
                    divVoucher.InnerHtml = html;

                }
            }
            if (Request.QueryString["i"] != null)//receipts
            {

                if (Request.QueryString["x"] != null)
                {
                    receiptid = Convert.ToInt32(Request.QueryString["x"]);
                    dt = _ar.GetReceipts(receiptid);
                    if (dt.Rows.Count > 0)
                    {

                    }
                }
            }
        }

    }
}