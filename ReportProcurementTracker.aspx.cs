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
    public partial class ReportProcurementTracker : System.Web.UI.Page
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
                hdType.Value = radOption.SelectedItem.ToString();
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
                string PendingType = hdType.Value;
                DataTable dt = _cm.GetPREnquiryQuotePOPending(PendingType);
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
                    gvList.DataSource = null;
                    gvList.DataBind();
                }
                spanCount.InnerHtml = dt.Rows.Count.ToString();
                ViewState["PRData"] = dt;// pagedDT;

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvList.PageIndex = 0;
            BindList(0);
        }

        protected void lnkProcessStatus_Command(object sender, CommandEventArgs e)
        {

        }

        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvList_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvList.PageIndex = e.NewPageIndex;
            BindList(e.NewPageIndex);
        }

        protected void radOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdType.Value = radOption.SelectedItem.ToString();
            BindList(0);
        }
    }
}