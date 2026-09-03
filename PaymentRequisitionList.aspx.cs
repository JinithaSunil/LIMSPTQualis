using LIMS.DAL;
using LIMS.Models;
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

namespace LIMS
{
    public partial class PaymentRequisitionList : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();
        DAL.APModule _ap = new DAL.APModule();

        public int PageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["EmailId"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                return;
            }

            if (!IsPostBack)
            {
                string roleId = Session["RoleId"].ToString();
                string pageName = Path.GetFileName(HttpContext.Current.Request.Url.AbsolutePath);

                ViewState["PageIndex"] = 1;
                ViewState["SearchTerm"] = "";
                BindPagedData();
            }
        }

        private void BindPagedData()
        {
            try
            {
                int DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                int EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                int totalRecords;

                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    if (DateTime.TryParse(txtStartDate.Text, out DateTime parsedStartDate))
                    {
                        startDate = parsedStartDate;
                    }
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    if (DateTime.TryParse(txtEndDate.Text, out DateTime parsedEndDate))
                    {
                        endDate = parsedEndDate.AddDays(1).AddSeconds(-1);
                    }
                }
                int choice = Convert.ToInt32(radChoice.SelectedValue.ToString());
                List<PaymentRequisitionData> filtered = new List<PaymentRequisitionData>();
                if (DepartmentID != 4015)
                {
                    filtered = dal.GetPaymentRequisitionFiltered(pageIndex, PageSize, searchTerm, startDate, endDate, out totalRecords, EmployeeID,choice);
                }
                else
                {
                    //divNew.Visible = false;
                    filtered = dal.GetPaymentRequisitionFiltered(pageIndex, PageSize, searchTerm, startDate, endDate, out totalRecords,choice);
                }
               
                if (filtered != null && filtered.Count > 0)
                {
                    Session["AllData"] = filtered;
                    /*  filtered = filtered
                       .Where(e => e.RequisitionStatus == choice)
                       .ToList();
                    */

                    switch (choice)
                    {
                        case 1:
                            // Existing logic - filter by RequisitionStatus = "1"
                            filtered = filtered
                                .Where(x => x.RequisitionStatus == "1")
                                .ToList();
                            break;

                        case 1015:
                            // StatusID = 1015
                            filtered = filtered
                                .Where(x => x.RequisitionStatus == "1015")
                                .ToList();
                            break;

                        case 1016:
                            // StatusID IN (1010,1012,1016)
                            filtered = filtered
                                .Where(x => x.RequisitionStatus == "1010"
                                         || x.RequisitionStatus == "1012"
                                         || x.RequisitionStatus == "1016")
                                .ToList();
                            break;
                        case 1017:
                            // StatusID IN (1017)
                            filtered = filtered
                                .Where(x => x.RequisitionStatus == "1017")
                                .ToList();
                            break;
                        default: 
                            break;
                    }
                    spanCount.InnerHtml = filtered.Count.ToString();
                    RepDataShow.DataSource = filtered;
                    RepDataShow.DataBind();
                    ViewState["CurrData"] = filtered;

                    GeneratePager(totalRecords, pageIndex);
                }
                else
                {
                    RepDataShow.DataSource = null;
                    RepDataShow.DataBind();
                    ViewState["CurrData"] = null;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script> alert('No records found.'); </script>");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["SearchTerm"] = txtSearch.Text.Trim();
            ViewState["PageIndex"] = 1;
            BindPagedData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            ViewState["SearchTerm"] = "";
            ViewState["PageIndex"] = 1;
            BindPagedData();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
            if (pageIndex > 1)
            {
                ViewState["PageIndex"] = pageIndex - 1;
                BindPagedData();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
            ViewState["PageIndex"] = pageIndex + 1;
            BindPagedData();
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            ViewState["PageIndex"] = 1;
            BindPagedData();
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(txtStartDate.Text))
            {
                if (DateTime.TryParse(txtStartDate.Text, out DateTime parsedStartDate))
                {
                    startDate = parsedStartDate;
                }
            }

            if (!string.IsNullOrEmpty(txtEndDate.Text))
            {
                if (DateTime.TryParse(txtEndDate.Text, out DateTime parsedEndDate))
                {
                    endDate = parsedEndDate.AddDays(1).AddSeconds(-1);
                }
            }

            dal.PaymentRequisitionGetPaged(1, PageSize, ViewState["SearchTerm"]?.ToString() ?? "", startDate, endDate, out int totalRecords);
            int lastPageIndex = (int)Math.Ceiling((double)totalRecords / PageSize);
            ViewState["PageIndex"] = lastPageIndex;
            BindPagedData();
        }

        private void GeneratePager(int totalRecords, int currentPage)
        {
            int pageCount = (int)Math.Ceiling((double)totalRecords / PageSize);
            List<object> pages = new List<object>();
            int maxVisiblePages = 5;
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(pageCount, startPage + maxVisiblePages - 1);

            if (endPage - startPage < maxVisiblePages && startPage > 1)
            {
                startPage = Math.Max(1, endPage - maxVisiblePages + 1);
            }

            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(new { PageIndex = i, PageNumber = i });
            }

            rptPager.DataSource = pages;
            rptPager.DataBind();

            btnFirst.Enabled = currentPage > 1;
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < pageCount;
            btnLast.Enabled = currentPage < pageCount;
        }

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                ViewState["PageIndex"] = Convert.ToInt32(e.CommandArgument);
                BindPagedData();
            }
        }

        protected System.Drawing.Color GetStatusColor(object statusTitle)
        {
            string status = statusTitle.ToString();

            switch (status)
            {
                case "1"://NEW
                    return System.Drawing.Color.Blue;
                case "1015"://ACCEPTED
                    return System.Drawing.Color.Green;
                case "1016"://REJECTED
                    return System.Drawing.Color.Red;
                case "1017"://PAID
                    return System.Drawing.Color.Black;
                default:
                    return System.Drawing.Color.OrangeRed;
            }
        }

        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int PaymentRequisitionID = 0;
            string RequisitionNumber = "", RequisitionBy = "";
            if (e.CommandName == "ItemShow")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                PaymentRequisitionID = Convert.ToInt32(arguments[0]);

                string encodedEventID = _cm.Encode(PaymentRequisitionID.ToString());

                string url = $"PaymentRequisitionApproval.aspx?p={encodedEventID}";
                Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                // Response.Redirect(url);
            }
            if (e.CommandName == "Pay")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                PaymentRequisitionID = Convert.ToInt32(arguments[0]);
                RequisitionNumber = arguments[1];
                RequisitionBy = arguments[2];

                var PaymentRequisition = _ap.GetPaymentRequisition(PaymentRequisitionID);
                if (PaymentRequisition != null)
                {
                    _ap.prVar.EmployeeID = RequisitionBy;
                    _ap.prVar.PaymentRequisitionID = PaymentRequisition.Rows[0]["PaymentRequisitionID"].ToString();
                    _ap.prVar.PaymentRequisitionNumber = PaymentRequisition.Rows[0]["RequisitionNumber"].ToString();
                    _ap.prVar.PayeeType = "Employee";
                    _ap.prVar.PayeeName = PaymentRequisition.Rows[0]["EmployeeName"].ToString();
                    _ap.prVar.RequisitionDate = Convert.ToDateTime(PaymentRequisition.Rows[0]["RequisitionDate"].ToString());
                    _ap.prVar.TotalAmount = Convert.ToDecimal(PaymentRequisition.Rows[0]["TotalAmount"].ToString());
                    if (PaymentRequisition.Rows[0]["AmountPaid"].ToString() == "")
                    {
                        _ap.prVar.PrevAmountPaid = 0;
                    }
                    else
                    {
                        _ap.prVar.PrevAmountPaid = Convert.ToDecimal(PaymentRequisition.Rows[0]["AmountPaid"].ToString());
                    }

                    Session["PRVariables"] = _ap.prVar;
                    Response.Redirect("RequisitionPayments.aspx");
                }
            }


        }


        protected void btnDownloadExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the date range from your controls
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    if (DateTime.TryParse(txtStartDate.Text, out DateTime parsedStartDate))
                    {
                        startDate = parsedStartDate;
                    }
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    if (DateTime.TryParse(txtEndDate.Text, out DateTime parsedEndDate))
                    {
                        endDate = parsedEndDate.AddDays(1).AddSeconds(-1);
                    }
                }
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                var data = GetExportData(searchTerm, startDate, endDate);

                if (data == null || data.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "noData",
                        "alert('No data found to export.');", true);
                    return;
                }
                ExportToExcel(data);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "exportError",
                    $"alert('Error exporting to Excel: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private List<PaymentRequisitionData> GetExportData(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            List<PaymentRequisitionData> items = new List<PaymentRequisitionData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"Select tm.PaymentRequisitionID,RequisitionDate,e.EmployeeName as RequisitionBy,RequisitionStatus,RequisitionNumber,
                            RequisitionType,tm.TotalAmount FROM PaymentRequisitionmaster tm
                            inner join Employees e on e.ID = tm.RequisitionBy   
                                  WHERE (@SearchTerm IS NULL OR 
                                      (tm.RequisitionNumber LIKE '%' + @SearchTerm + '%' ))
                                  AND (@StartDate IS NULL OR CONVERT(date, tm.RequisitionDate) >= @StartDate)
                                  AND (@EndDate IS NULL OR CONVERT(date, tm.RequisitionDate) <= @EndDate)
                                  group by tm.PaymentRequisitionID,RequisitionDate,e.EmployeeName,RequisitionStatus,
								  RequisitionNumber,RequisitionType,RequisitionSubType,TotalAmount
                                  ORDER BY CONVERT(datetime, tm.RequisitionDate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    cmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                    cmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PaymentRequisitionData
                            {
                                PaymentRequisitionID = Convert.ToInt32(sdr["PaymentRequisitionID"]),
                                RequisitionDate = Convert.ToDateTime(sdr["RequisitionDate"].ToString()),
                                RequisitionBy = sdr["RequisitionBy"].ToString(),
                                RequisitionNumber = sdr["RequisitionNumber"].ToString(),
                                RequisitionType = sdr["RequisitionType"].ToString(),
                                RequisitionStatus = sdr["RequisitionStatus"].ToString(),
                                TotalAmount = Convert.ToDecimal(sdr["TotalAmount"].ToString())
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        private void ExportToExcel(List<PaymentRequisitionData> data)
        {
            using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Payment Requisition Report");

                // Add headers
                ws.Cell(1, 1).Value = "Requisition Number";
                ws.Cell(1, 2).Value = "Requisition Date";
                ws.Cell(1, 3).Value = "Requisition By";
                ws.Cell(1, 4).Value = "Requisition Type";
                ws.Cell(1, 5).Value = "Total Amount";

                // Add data
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.RequisitionNumber;
                    ws.Cell(row, 2).Value = item.RequisitionDate;
                    ws.Cell(row, 3).Value = item.RequisitionBy;
                    ws.Cell(row, 4).Value = item.RequisitionType;
                    ws.Cell(row, 5).Value = item.TotalAmount;
                    row++;
                }

                // Formatting
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(1);

                // Prepare response
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0;

                    string fileName = $"PaymentRequisitionReport_{DateTime.Now:yyyyMMdd}.xlsx";
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
        }

        protected void radChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPagedData();
        }

        protected void RepDataShow_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int DepartmentID = 0;
                if (Session["DepartmentID"] != null)
                {
                    DepartmentID = Convert.ToInt32(Session["DepartmentID"]);

                    System.Web.UI.WebControls.HyperLink hypPR =
                    (System.Web.UI.WebControls.HyperLink)e.Item.FindControl("hypPR");

                    int paymentReqId = Convert.ToInt32(
                    DataBinder.Eval(e.Item.DataItem, "PaymentRequisitionID"));

                    string encodedID = _cm.Encode(paymentReqId.ToString());


                    hypPR.NavigateUrl =
                        $"GeneratePaymentRequisition.aspx?ID={encodedID}";

                    LinkButton lnkPay = (LinkButton)e.Item.FindControl("lnkPay");
                    Label lblRequisitionStatus = (Label)e.Item.FindControl("lblRequisitionStatus");
                    lnkPay.Visible = false;


                    if ((lblRequisitionStatus != null)&& (lblRequisitionStatus.Text != ""))
                    {
                        DataTable dtStatus = _cm.GetPRStatus(Convert.ToInt32(lblRequisitionStatus.Text));
                        if (dtStatus.Rows.Count > 0)
                        {
                            lblRequisitionStatus.Text = dtStatus.Rows[0]["StatusType"].ToString();
                            if ((DepartmentID == 4015)&&(lblRequisitionStatus.Text== "Approved by Tier-4"))
                            {
                                lnkPay.Visible = true;
                            }
                        }
                    }
                    
                }

            }
        }

        protected void lnkPayments_Click(object sender, EventArgs e)
        {
            int DepartmentID = 0;
            if (Session["DepartmentID"] != null)
            {
                DepartmentID = Convert.ToInt32(Session["DepartmentID"]);
                if (DepartmentID == 4015)
                {
                    Response.Redirect("RequisitionPayments.aspx?a=1");
                }
                else
                {
                    Response.Redirect("RequisitionPayments.aspx?e=1");
                }
            }
        }
    }
}