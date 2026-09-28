using ClosedXML.Excel;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ReportPurchaseRequisitionList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime today = DateTime.Today;

                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

                txtFromDate.Text = firstDay.ToString("yyyy-MM-dd");
                txtToDate.Text = lastDay.ToString("yyyy-MM-dd");
                BindList(0);
            }
        }
        protected void BindList(int index)
        {
            int PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvList.PageSize = PageSize;

            int StartIndex = index * PageSize;   // important for OFFSET 
            DataTable pagedDT = new DataTable();
            if ((txtFromDate.Text != "") && (txtToDate.Text != ""))
            {
                _cm.FromDate = Convert.ToDateTime(txtFromDate.Text);
                _cm.ToDate = Convert.ToDateTime(txtToDate.Text);
                DataTable dt = _cm.GetPurchaseRequisitionList(_cm);
                if (dt.Rows.Count > 0)
                {
                    pagedDT = dt.AsEnumerable()
                                .Skip(StartIndex)
                                .Take(PageSize)
                                .CopyToDataTable();
                    ViewState["dirState"] = pagedDT;
                    ViewState["sortdr"] = "Asc";
                    gvList.DataSource = pagedDT;
                    gvList.DataBind();
                }
                else
                {
                    pagedDT = dt.Clone();
                }
                spanCount.InnerHtml = dt.Rows.Count.ToString();
                ViewState["PRData"] = dt;// pagedDT;

                BindTier(pagedDT);
                BindPO(pagedDT);
                BindGRN(pagedDT);
            }
        }
        private void BindTier(DataTable dt)
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = "CurrentTier IS NOT NULL";

            DataTable dtTier = dv.ToTable(true, "CurrentTier");

            ddlTier.DataSource = dtTier;
            ddlTier.DataTextField = "CurrentTier";
            ddlTier.DataValueField = "CurrentTier";
            ddlTier.DataBind();

            ddlTier.Items.Insert(0, new ListItem("--All--", ""));
        }
        private void BindPO(DataTable dt)
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = "PONumber IS NOT NULL AND PONumber <> ''";

            DataTable dtPO = dv.ToTable(true, "PONumber");

            ddlPO.DataSource = dtPO;
            ddlPO.DataTextField = "PONumber";
            ddlPO.DataValueField = "PONumber";
            ddlPO.DataBind();

            ddlPO.Items.Insert(0, new ListItem("--All--", ""));
        }
        private void BindGRN(DataTable dt)
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = "GRNNumber IS NOT NULL AND GRNNumber <> ''";

            DataTable dtGRN = dv.ToTable(true, "GRNNumber");

            ddlGRN.DataSource = dtGRN;
            ddlGRN.DataTextField = "GRNNumber";
            ddlGRN.DataValueField = "GRNNumber";
            ddlGRN.DataBind();

            ddlGRN.Items.Insert(0, new ListItem("--All--", ""));
        }
        protected void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();            
        }
        private void ApplyFilters()
        {
            DataTable dt = (DataTable)ViewState["PRData"];

            DataView dv = dt.DefaultView;

            List<string> filter = new List<string>();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string s = txtSearch.Text.Replace("'", "''");

                filter.Add(string.Format(
                "(PRNumber LIKE '%{0}%' " +
                "OR PONumber LIKE '%{0}%' " +
                "OR GRNNumber LIKE '%{0}%' " +
                "OR RequestBy LIKE '%{0}%') " , s));
            }

            if (!string.IsNullOrEmpty(ddlTier.SelectedValue))
                filter.Add($"CurrentTier='{ddlTier.SelectedValue}'");

            if (!string.IsNullOrEmpty(ddlPO.SelectedValue))
                filter.Add($"PONumber='{ddlPO.SelectedValue}'");

            if (!string.IsNullOrEmpty(ddlGRN.SelectedValue))
                filter.Add($"GRNNumber='{ddlGRN.SelectedValue}'");

            if (!string.IsNullOrEmpty(ddlProcessStatus.SelectedValue))
            {
                filter.Add($"ProcessStatus = '{ddlProcessStatus.SelectedValue}'");
            }

            dv.RowFilter = string.Join(" AND ", filter);

            gvList.DataSource = dv;
            gvList.DataBind();

            spanCount.InnerText = dv.Count.ToString();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindList(0);
            ApplyFilters();
        }

        protected void gvList_Sorting(object sender, GridViewSortEventArgs e)
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
                gvList.DataSource = dtrslt;
                gvList.DataBind();
                spanCount.InnerHtml = dtrslt.Rows.ToString();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if ((txtFromDate.Text != "") && (txtToDate.Text != ""))
            {
                _cm.FromDate = Convert.ToDateTime(txtFromDate.Text);
                _cm.ToDate = Convert.ToDateTime(txtToDate.Text);
                DataTable dt = _cm.GetPurchaseRequisitionList(_cm);
                if (dt.Rows.Count > 0)
                {

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "PR Report");

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=PR_Report.xlsx");

                        using (MemoryStream ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            ms.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlProcessStatus.ClearSelection();
            BindList(0);
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
        protected void lnkProcessStatus_Command(object sender, CommandEventArgs e)
        {
            string url = "";
            switch (e.CommandName)
            {
                case "PR":
                    url = "GeneratePurchaseRequisitionReport.aspx?ID=" + e.CommandArgument;
                    break;

                case "PO":
                    url = "GeneratePO.aspx?POID=" + e.CommandArgument;
                    break;

                case "GRN":
                    string grnID = e.CommandArgument.ToString();
                    string encodedID = _cm.Encode(grnID);
                    url = "GenerateGRN.aspx?ID=" + encodedID;
                    break;
            }

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                Guid.NewGuid().ToString(),
                $"window.open('{ResolveUrl(url)}', '_blank');",
                true);
        }
        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            LinkButton lnk = (LinkButton)e.Row.FindControl("lnkProcessStatus");

            string status = DataBinder.Eval(e.Row.DataItem, "ProcessStatus").ToString();

            switch (status)
            {
                case "PR Pending":
                    lnk.Text = "<span class='status-dot dot-red'></span> PR Pending";
                    lnk.CommandName = "PR";
                    lnk.CommandArgument = DataBinder.Eval(e.Row.DataItem, "PRID").ToString();
                    break;

                case "PO Created":
                    lnk.Text = "<span class='status-dot dot-yellow'></span> PO Created";
                    lnk.CommandName = "PO";
                    lnk.CommandArgument = DataBinder.Eval(e.Row.DataItem, "POID").ToString();
                    break;

                case "GRN Completed":
                    lnk.Text = "<span class='status-dot dot-green'></span> GRN Completed";
                    lnk.CommandName = "GRN";
                    lnk.CommandArgument = DataBinder.Eval(e.Row.DataItem, "GRNID").ToString();
                    break;
                case "Quotation Received":
                    lnk.Text = "<span class='status-dot dot-green'></span> Quotation Received";                    
                    break;
            }
        }

        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvList.PageIndex = e.NewPageIndex;
            BindList(e.NewPageIndex);
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvList.PageIndex = 0;
            BindList(0);
        }
    }
}