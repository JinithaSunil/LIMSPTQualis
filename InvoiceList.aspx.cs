using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static LIMS.DAL.ARModule;

namespace LIMS
{
    public partial class InvoiceList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["od"] != null)
                {
                    BindInvoicesOD();
                }
                else
                {
                    BindInvoices();
                }

            }
        }
        protected void BindInvoicesOD()
        {
            DataTable dt = _cm.GetInvoicesOD();
            if (dt.Rows.Count > 0)
            {
                gvInvoices.DataSource = dt;
                gvInvoices.DataBind();
                spanCount.InnerHtml = dt.Rows.Count.ToString();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";
            }
            else
            {
                gvInvoices.DataSource = null;
                gvInvoices.DataBind();
            }
        }
        protected void BindInvoices()
        {
            DataTable dt = _cm.GetInvoices();
            if (dt.Rows.Count > 0)
            {
                gvInvoices.DataSource = dt;
                gvInvoices.DataBind();
                spanCount.InnerHtml = dt.Rows.Count.ToString();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";
            }
            else
            {
                gvInvoices.DataSource = null;
                gvInvoices.DataBind();
            }
        }

        protected void Filter(object sender, EventArgs e)
        {

        }
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetCustomers(string prefixText, int count)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    //com.CommandText = "Select distinct TestParameter from TestParameter where TestParameter like " + " @Search + '%' and isRequirement=0";
                    com.CommandText = "Select distinct CompanyName from CompanyDetails where CompanyName like " + " @Search + '%' ";
                    com.Parameters.AddWithValue("@Search", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> CompanyName = new List<string>();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            CompanyName.Add(sdr["CompanyName"].ToString());
                        }
                    }
                    con.Close();
                    return CompanyName;


                }

            }

        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            List<string> conditions = new List<string>();
            SqlCommand cmd = new SqlCommand();

            // INVOICE NUMBER
            if (!string.IsNullOrWhiteSpace(txtInvoiceNumber.Text))
            {
                conditions.Add("i.InvoiceNumber LIKE @InvoiceNumber");
                cmd.Parameters.AddWithValue("@InvoiceNumber", "%" + txtInvoiceNumber.Text.Trim() + "%");
            }

            // CUSTOMER (CompanyName)
            if (!string.IsNullOrWhiteSpace(txtCustomer.Text))
            {
                conditions.Add("CompanyName LIKE @Customer");
                cmd.Parameters.AddWithValue("@Customer", "%" + txtCustomer.Text.Trim() + "%");
            }

            // REQUEST NO
            if (!string.IsNullOrWhiteSpace(txtRequestNo.Text))
            {
                conditions.Add("RequestNo LIKE @RequestNo");
                cmd.Parameters.AddWithValue("@RequestNo", "%" + txtRequestNo.Text.Trim() + "%");
            }

            // PAID DROPDOWN
            if (ddlPaid.SelectedValue != "0" && ddlPaid.SelectedValue != "")
            {
                if (ddlPaid.SelectedItem.ToString() == "Yes")
                {
                    conditions.Add("i.AmountPaid > @Paid");
                }
                else
                if (ddlPaid.SelectedItem.ToString() == "No")
                {
                    conditions.Add("i.AmountPaid = @Paid");
                }
                else
                if (ddlPaid.SelectedItem.ToString() == "All")
                {
                    conditions.Add("i.AmountPaid >= @Paid");
                }
                cmd.Parameters.AddWithValue("@Paid", 0);
            }

            // INVOICE DATE RANGE
            DateTime dt1, dt2;

            if (DateTime.TryParse(txtInvDate1.Text, out dt1))
            {
                conditions.Add("i.InvoiceDate >= @InvDate1");
                cmd.Parameters.AddWithValue("@InvDate1", txtInvDate1.Text.Trim());
            }

            if (DateTime.TryParse(txtInvDate2.Text, out dt2))
            {
                conditions.Add("i.InvoiceDate <= @InvDate2");
                cmd.Parameters.AddWithValue("@InvDate2", txtInvDate2.Text.Trim());
            }

            // TOTAL AMOUNT RANGE
            decimal total1, total2;

            if (decimal.TryParse(txtTotal1.Text, out total1))
            {
                conditions.Add("TotalAmount >= @Total1");
                cmd.Parameters.AddWithValue("@Total1", total1);
            }

            if (decimal.TryParse(txtTotal2.Text, out total2))
            {
                conditions.Add("TotalAmount <= @Total2");
                cmd.Parameters.AddWithValue("@Total2", total2);
            }

            // FINAL QUERY
            string query = @"Select i.InvoiceID, i.InvoiceNumber,i.InvoiceDate,i.DueDate,i.TotalAmount,
i.AmountPaid,(i.TotalAmount-i.AmountPaid) as BalanceDue,cd.CompanyName,c.CurrencyID,c.CurrencyCode,ci.RequestNo,i.QuotationID,i.CompanyID from Invoices i
                            inner join CompanyDetails cd on cd.CustomerMasterId = i.CompanyID
                            inner join QuotationMaster q on q.QuotationMasterID = i.QuotationID
                            inner join Currency c on c.CurrencyID = q.CurrencyID
                            inner join CustomerInquiry ci on ci.InquiryId = q.EnquiryId";

            if (conditions.Count > 0)
            {
                query += " WHERE " + string.Join(" AND ", conditions);
            }
            cmd.CommandText = query;
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt != null)
            {
                spanCount.InnerHtml = dt.Rows.Count.ToString();
                gvInvoices.DataSource = dt;
                gvInvoices.DataBind();
                ViewState["dirState"] = dt;
            }
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtInvoiceNumber.Text = "";
            txtCustomer.Text = "";
            txtInvDate1.Text = "";
            txtInvDate2.Text = "";
            txtRequestNo.Text = "";
            txtTotal1.Text = "";
            txtTotal2.Text = "";
            BindInvoices();
        }

        protected void gvInvoices_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvInvoices.DataSource = dtrslt;
                gvInvoices.DataBind();
            }
        }
        protected void btnMerge_Click(object sender, EventArgs e)
        {
            List<string> selectedFiles = new List<string>();
            string Inv = "";
            foreach (GridViewRow row in gvInvoices.Rows)
            {
                CheckBox chk = row.FindControl("chkSelect") as CheckBox;

                if (chk != null && chk.Checked)
                {
                    //string filePath = gvInvoices.DataKeys[row.RowIndex]["FilePath"].ToString();
                    HiddenField hdQuotationID = (HiddenField)row.Cells[0].FindControl("hdQuotationID"); // FilePath column
                    HiddenField hdInvoiceID = (HiddenField)row.Cells[0].FindControl("hdInvoiceID"); // FilePath column
                    HiddenField hdCompanyID = (HiddenField)row.Cells[0].FindControl("hdCompanyID"); // FilePath column
                    string filePath = "~/Invoice/INV-" + hdQuotationID.Value + ".htm";
                    Inv += hdInvoiceID.Value + ",";
                    selectedFiles.Add(Server.MapPath(filePath));
                }
            }
            Session["Inv"] = Inv;

            if (selectedFiles.Count > 0)
            {
                string mergedHtml = MergeHtmlFiles(selectedFiles);

                string outputPath = Server.MapPath("~/Invoice/MergedOutput.html");
                File.WriteAllText(outputPath, mergedHtml);
                string url = "ViewDocuments.aspx?Inv=1";
                Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                string url2 = "CustomerLabel.aspx";
                Response.Write("<script> window.open( '" + url2 + "','_blank' ); </script>");
                // DownloadMergedFile(outputPath);

            }
        }
        private string MergeHtmlFiles(List<string> htmlFiles)
        {
            StringBuilder sb = new StringBuilder();


            foreach (var file in htmlFiles)
            {
                string html = File.ReadAllText(file);

                // Extract only body content (optional)
                string body = ExtractBody(html);

                sb.Append(body);
                sb.Append("<hr/>");   // separator between files (optional)
            }


            return sb.ToString();
        }
        private string ExtractBody(string html)
        {
            int start = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
            if (start < 0) return html;

            start = html.IndexOf(">", start) + 1;
            int end = html.IndexOf("</body>", start, StringComparison.OrdinalIgnoreCase);

            if (end < 0) end = html.Length;

            return html.Substring(start, end - start);
        }
        private void DownloadMergedFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);

            Response.Clear();
            Response.ContentType = "text/html";
            Response.AddHeader("Content-Disposition", "attachment; filename=MergedOutput.html");
            Response.WriteFile(file.FullName);


            Response.End();
        }

        protected void bnCoretax_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)ViewState["dirState"];
            List<int> invoiceIds = dt.AsEnumerable()
                         .Select(r => r.Field<int>("InvoiceID"))
                         .ToList();
            DataTable dtInvoice = _cm.GetInvoicesByIds(invoiceIds);
            DataTable dtDetails = _cm.GetInvoiceDetailsByIds(invoiceIds);

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Sheet 1 – Faktur
                var wsInvoice = wb.Worksheets.Add("Faktur");
                wsInvoice.Row(1).Style.Font.Bold = true;
                wsInvoice.Row(1).Style.Font.FontSize = 10;
                wsInvoice.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                wsInvoice.Cell(1, 1).Value = "NPWP Penjual";
                wsInvoice.Range(1, 1, 1, 2).Merge();   // adjust column count


                wsInvoice.Cell(1, 3).Value = "'0311995997402000";
                wsInvoice.Range(1, 3, 1, 4).Merge();   // adjust column count

                wsInvoice.Row(3).Style.Font.Bold = true;
                wsInvoice.Row(3).Style.Font.FontSize = 10;

                wsInvoice.Cell(3, 1).Value = "Baris";
                wsInvoice.Cell(3, 2).Value = "Tanggal Faktur";
                wsInvoice.Cell(3, 3).Value = "Jenis Faktur";
                wsInvoice.Cell(3, 4).Value = "Kode Transaksi";
                wsInvoice.Cell(3, 5).Value = "Keterangan Tambahan";
                wsInvoice.Cell(3, 6).Value = "Dokumen Pendukung";
                wsInvoice.Cell(3, 7).Value = "Period Dok Pendukung";
                wsInvoice.Cell(3, 8).Value = "Referensi";
                wsInvoice.Cell(3, 9).Value = "Cap Fasilitas";
                wsInvoice.Cell(3, 10).Value = "ID TKU Penjual";
                wsInvoice.Cell(3, 11).Value = "NPWP/NIK Pembeli";
                wsInvoice.Cell(3, 12).Value = "Jenis ID Pembeli";
                wsInvoice.Cell(3, 13).Value = "Negara Pembeli";
                wsInvoice.Cell(3, 14).Value = "Nomor Dokumen Pembeli";
                wsInvoice.Cell(3, 15).Value = "Nama Pembeli";
                wsInvoice.Cell(3, 16).Value = "Alamat Pembeli";
                wsInvoice.Cell(3, 17).Value = "Email Pembeli";
                wsInvoice.Cell(3, 18).Value = "ID TKU Pembeli";



                wsInvoice.Cell(4, 1).InsertData(dtInvoice.AsEnumerable());

                wsInvoice.Columns().AdjustToContents();
                wsInvoice.Row(1).Style.Font.Bold = true;

                // Sheet 2 – DetailFaktur
                var wsDetails = wb.Worksheets.Add("DetailFaktur");
                wsDetails.Columns().AdjustToContents();
                wsDetails.Row(1).Style.Font.Bold = true;
                wsDetails.Row(1).Style.Font.FontSize = 10;

                wsDetails.Cell(1, 1).Value = "Baris";
                wsDetails.Cell(1, 2).Value = "Barang/Jasa";
                wsDetails.Cell(1, 3).Value = "Kode Barang Jasa";
                wsDetails.Cell(1, 4).Value = "Nama Barang/Jasa";
                wsDetails.Cell(1, 5).Value = "Nama Satuan Ukur";
                wsDetails.Cell(1, 6).Value = "Harga Satuan";
                wsDetails.Cell(1, 7).Value = "Jumlah Barang Jasa";
                wsDetails.Cell(1, 8).Value = "Total Diskon";
                wsDetails.Cell(1, 9).Value = "DPP";
                wsDetails.Cell(1, 10).Value = "DPP Nilai Lain";
                wsDetails.Cell(1, 11).Value = "Tarif PPN";
                wsDetails.Cell(1, 12).Value = "PPN";
                wsDetails.Cell(1, 13).Value = "Tarif PPnBM";
                wsDetails.Cell(1, 14).Value = "PPnBM";

                wsDetails.Cell(2, 1).InsertData(dtDetails.AsEnumerable());
                wsDetails.Columns().AdjustToContents();

                string folderPath = Server.MapPath("~/Invoice/");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string filePath = Path.Combine(
                    folderPath,
                    "CoreTaxFile.xlsx"
                );
                wb.SaveAs(filePath);
                if (File.Exists(filePath))
                {
                    Response.Clear();
                    Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    Response.AddHeader(
                        "Content-Disposition",
                        "attachment; filename=" + Path.GetFileName(filePath)
                    );

                    Response.TransmitFile(filePath);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            List<InvoicePayment> invoices = new List<InvoicePayment>();

            foreach (GridViewRow row in gvInvoices.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow)
                    continue;

                CheckBox chk = row.FindControl("chkSelect") as CheckBox;

                if (chk != null && chk.Checked)
                {
                    int invoiceId = Convert.ToInt32(gvInvoices.DataKeys[row.RowIndex]["InvoiceID"]);
                    decimal balance = Convert.ToDecimal(gvInvoices.DataKeys[row.RowIndex]["BalanceDue"]);
                    DateTime invoiceDate = Convert.ToDateTime(gvInvoices.DataKeys[row.RowIndex]["InvoiceDate"]);

                    invoices.Add(new InvoicePayment
                    {
                        InvoiceID = invoiceId,
                        BalanceAmount = balance,
                        InvoiceDate = invoiceDate
                    });
                }
            }


        }

    }
}