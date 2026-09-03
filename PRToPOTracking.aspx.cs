using LIMS.BLL;
using LIMS.DAL;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class PRToPOTracking : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        PurchaseBLL _purchaseBLL = new PurchaseBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadVendors();

                BindGrid(0);
            }
        }
        private void LoadDepartments()
        {
            try
            {
                DataTable dt = _purchaseBLL.GetDepartmentList();
                if (dt.Rows.Count > 0)
                {
                    ddlDepartment.DataSource = dt;
                    ddlDepartment.DataTextField = "EmployeeDepartmentName";
                    ddlDepartment.DataValueField = "EmployeeDepartmentID";
                    ddlDepartment.DataBind();
                    ddlDepartment.Items.Insert(0, "-- Select Department --");
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void ShowSuccess(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "alert alert-success alert-dismissible fade show";
            lblMessage.Text = message;
        }
        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "alert alert-danger alert-dismissible fade show";
            lblMessage.Text = message;
        }
        private void LoadVendors()
        {
            try
            {
                DataTable dt = _purchaseBLL.GetVendorList();

                ddlVendor.DataSource = dt;
                ddlVendor.DataTextField = "VendorName";
                ddlVendor.DataValueField = "VendorID";
                ddlVendor.DataBind();

                ddlVendor.Items.Insert(0, new ListItem("-- All Vendors --", "0"));

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        private void BindGrid(int index)
        {
            try
            {
                string PRNo = txtPRNo.Text.Trim();
                int DepartmentId = ddlDepartment.SelectedIndex > 0 ? Convert.ToInt32(ddlDepartment.SelectedValue) : 0;
                int VendorId = ddlVendor.SelectedIndex > 0 ? Convert.ToInt32(ddlVendor.SelectedValue) : 0;
                string Status = ddlStatus.SelectedValue;
                string FromDate = txtFrom.Text.Trim();
                string ToDate = txtTo.Text.Trim();

                int PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                gvTracking.PageSize = PageSize;

                int StartIndex = index * PageSize;   // important for OFFSET

                DataTable dt = _purchaseBLL.GetPRTracking(
                                    PRNo,
                                    DepartmentId,
                                    VendorId,
                                    Status,
                                    FromDate,
                                    ToDate,
                                    StartIndex,
                                    PageSize);
                if (dt.Rows.Count > 0)
                {                   
                    gvTracking.VirtualItemCount = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                    gvTracking.DataSource = dt;
                    gvTracking.DataBind();
                }
                else
                {
                    gvTracking.DataSource = null;
                    gvTracking.DataBind();
                }
                spanCount.InnerHtml = dt.Rows.Count.ToString();
                //BindDashboard(dt);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.CssClass = "text-danger";
            }
        }
        //private void BindDashboard(DataTable dt)
        //{
        //    if (dt.Rows.Count == 0)
        //    {
        //        lblPR.Text = "0";
        //        lblEnquiry.Text = "0";
        //        lblPO.Text = "0";
        //        lblGRN.Text = "0";
        //        lblOutstanding.Text = "0";
        //        lblReceipt.Text = "0";
        //        return;
        //    }

        //    lblPR.Text = dt.AsEnumerable()
        //                   .Select(r => r["PRNo"].ToString())
        //                   .Distinct()
        //                   .Count()
        //                   .ToString();

        //    lblEnquiry.Text = dt.AsEnumerable()
        //                        .Select(r => r["EnquiryNo"].ToString())
        //                        .Distinct()
        //                        .Count()
        //                        .ToString();

        //    lblPO.Text = dt.AsEnumerable()
        //                   .Select(r => r["PONo"].ToString())
        //                   .Distinct()
        //                   .Count()
        //                   .ToString();

        //    lblGRN.Text = dt.AsEnumerable()
        //                    .Select(r => r["GRNNo"].ToString())
        //                    .Distinct()
        //                    .Count()
        //                    .ToString();

        //    lblOutstanding.Text = dt.AsEnumerable()
        //                            .Sum(r => Convert.ToDecimal(r["PendingPOQty"]))
        //                            .ToString("N2");

        //    lblReceipt.Text = dt.AsEnumerable()
        //                        .Sum(r => Convert.ToDecimal(r["PendingReceiptQty"]))
        //                        .ToString("N2");
        //}
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(0);

            updMain.Update();
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtPRNo.Text = "";

            ddlDepartment.SelectedIndex = 0;

            ddlVendor.SelectedIndex = 0;

            ddlStatus.SelectedIndex = 0;

            BindGrid(0);

            updMain.Update();
        }
        protected void gvTracking_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                int row = Convert.ToInt32(e.CommandArgument);

                GridViewRow gr = gvTracking.Rows[row];

            }
            if (e.CommandName == "PRDetails")
            {
                int PurchaseRequisitionID = Convert.ToInt32(e.CommandArgument);

                LoadPRDetails(PurchaseRequisitionID);

                pnlPRDetails.Visible = true;

                updMain.Update();
            }
        }
        protected void btnCreatePO_Click(object sender, EventArgs e)
        {
            //  SavePO();

            BindGrid(0);

            updMain.Update();
        }
        //protected void tmRefresh_Tick(object sender, EventArgs e)
        //{
        //    BindGrid(0);

        //    updGrid.Update();
        //}



        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvTracking.PageIndex = 0;
            BindGrid(0);
        }
        private void LoadPRDetails(int PurchaseRequisitionID)
        {
            DataSet ds = _purchaseBLL.GetPRCompleteDetails(PurchaseRequisitionID);

            DataRow dr = ds.Tables[0].Rows[0];


            DataTable dt = ds.Tables[1];

            if (dt.Rows.Count > 0)
            {
                rptPO.DataSource = dt;
                rptPO.DataBind();

                lblNoPO.Visible = false;
            }
            else
            {
                rptPO.DataSource = null;
                rptPO.DataBind();

                lblNoPO.Visible = true;
            }

            lblPRHeader.Text = "PR-" + dr["PRNo"].ToString();
            lblItem.Text = dr["ItemDescription"].ToString();

            lblReqQty.Text = dr["RequestedQty"].ToString();

            lblOrderedQty.Text = dr["OrderedQty"].ToString();

            lblReceivedQty.Text = dr["ReceivedQty"].ToString();

            lblPendingPO.Text = dr["PendingPOQty"].ToString();

            lblPendingReceipt.Text = dr["PendingReceiptQty"].ToString();
        }
        
        protected void rptPO_ItemDataBound(object sender,RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int poDetailsID = Convert.ToInt32(
                    DataBinder.Eval(e.Item.DataItem, "PODetailsID"));

                Repeater rptGRN = (Repeater)e.Item.FindControl("rptGRN");
                Label lblNoGRN = (Label)e.Item.FindControl("lblNoGRN");

                DataTable dt = _purchaseBLL.GetGRNDetailsByPODetails(poDetailsID);

                if (dt.Rows.Count > 0)
                {
                    rptGRN.DataSource = dt;
                    rptGRN.DataBind();

                    lblNoGRN.Visible = false;
                }
                else
                {
                    rptGRN.DataSource = null;
                    rptGRN.DataBind();

                    lblNoGRN.Visible = true;
                }
            }
        }

        protected string GetStatusCss(string status)
        {
            switch (status.ToLower())
            {
                case "completed":
                case "fully received":
                    return "status-pill completed";

                case "pending":
                    return "status-pill pending";

                case "partial":
                case "partial po":
                case "Partial Receipt":
                    return "status-pill partial";

                case "Partial PO":
                    return "status-pill pendingreceipt";

                case "ordered":
                    return "status-pill ordered";

                default:
                    return "status-pill";
            }
        }

        protected void rptGRN_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemShow")
            {
                string grnID = e.CommandArgument.ToString();

                string encodedID = _cm.Encode(grnID);

                string url = "GenerateGRN.aspx?ID=" + encodedID;

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "OpenGRN",
                    "window.open('" + url + "','_blank');",
                    true);
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string PRNo = txtPRNo.Text.Trim();
            int DepartmentId = ddlDepartment.SelectedIndex > 0 ? Convert.ToInt32(ddlDepartment.SelectedValue) : 0;
            int VendorId = ddlVendor.SelectedIndex > 0 ? Convert.ToInt32(ddlVendor.SelectedValue) : 0;
            string Status = ddlStatus.SelectedValue;
            string FromDate = txtFrom.Text.Trim();
            string ToDate = txtTo.Text.Trim();

            DataTable dt = _purchaseBLL.GetPRTracking(
                              txtPRNo.Text.Trim(),
                              DepartmentId,
                              VendorId,
                              Status,
                              txtFrom.Text,
                              txtTo.Text,
                              0,
                              100000);     // Export all records

            DataTable dtExport = new DataTable();

            dtExport.Columns.Add("PRNo");
            dtExport.Columns.Add("RequestDate");
            dtExport.Columns.Add("Department");
            dtExport.Columns.Add("RequestBy");
            dtExport.Columns.Add("ItemName");
            dtExport.Columns.Add("RequestedQty");
            dtExport.Columns.Add("EnquiryNo");
            dtExport.Columns.Add("PONo");
            dtExport.Columns.Add("GRNNo");
            dtExport.Columns.Add("OrderedQty");
            dtExport.Columns.Add("ReceivedQty");
            dtExport.Columns.Add("PendingPOQty");
            dtExport.Columns.Add("PendingReceiptQty");
            dtExport.Columns.Add("Status");
            dtExport.Columns.Add("Progress");

            foreach (DataRow dr in dt.Rows)
            {
                dtExport.Rows.Add(
                    dr["PRNo"],
                    Convert.ToDateTime(dr["RequestDate"]).ToString("dd/MM/yyyy"),
                    dr["Department"],
                    dr["RequestBy"],
                    dr["ItemName"],
                    dr["RequestedQty"],
                    dr["EnquiryNo"],
                    dr["PONo"],
                    dr["GRNNo"],
                    dr["OrderedQty"],
                    dr["ReceivedQty"],
                    dr["PendingPOQty"],
                    dr["PendingReceiptQty"],
                    dr["Status"],
                    dr["Progress"] + "%"
                );
            }

            ExportToExcel(dtExport, "PRToPOTracking");
        }
        private void ExportToExcel(DataTable dt, string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
                "attachment;filename=" + fileName + ".xls");

            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    GridView gv = new GridView();

                    gv.DataSource = dt;
                    gv.DataBind();

                    gv.HeaderStyle.Font.Bold = true;
                    gv.HeaderStyle.BackColor = System.Drawing.Color.LightGray;

                    gv.RenderControl(hw);

                    Response.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            pnlFilters.Visible = !pnlFilters.Visible;
        }

        protected void gvTracking_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTracking.PageIndex = e.NewPageIndex;
            BindGrid(e.NewPageIndex);
        }
    }
}