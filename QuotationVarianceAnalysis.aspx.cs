using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
    public partial class QuotationVarianceAnalysis : System.Web.UI.Page
    {
        Purchase _pur = new Purchase();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEnquiry();
                LoadVendor();
                LoadRevision();

                Search();
            }
        }

        #region Load Dropdowns

        private void LoadEnquiry()
        {
            ddlEnquiry.DataSource = _pur.GetEnquiryList();
            ddlEnquiry.DataTextField = "EnquiryNumber";
            ddlEnquiry.DataValueField = "EnquiryID";
            ddlEnquiry.DataBind();

            ddlEnquiry.Items.Insert(0, new ListItem("--Select--", "0"));
        }

        private void LoadVendor()
        {
            ddlVendor.DataSource = _pur.GetVendorList();
            ddlVendor.DataTextField = "VendorName";
            ddlVendor.DataValueField = "VendorID";
            ddlVendor.DataBind();

            ddlVendor.Items.Insert(0, new ListItem("All Vendors", "0"));
        }

        private void LoadRevision()
        {
            ddlRevision.Items.Clear();

            ddlRevision.Items.Add(new ListItem("Latest", "0"));
            ddlRevision.Items.Add(new ListItem("All", "-1"));
        }

        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            int enquiryID = Convert.ToInt32(ddlEnquiry.SelectedValue);
            int vendorID = Convert.ToInt32(ddlVendor.SelectedValue);
            int revision = Convert.ToInt32(ddlRevision.SelectedValue);

            bool varianceOnly = chkVarianceOnly.Checked;

            DataTable dt = _pur.GetQuotationVarianceAnalysis(
                                enquiryID,
                                vendorID,
                                revision,
                                varianceOnly);

            gvQuotationVariance.DataSource = dt;
            gvQuotationVariance.DataBind();

            LoadSummary(dt);
        }

        private void LoadSummary(DataTable dt)
        {
            lblQuotationCount.Text = dt.Rows.Count.ToString();

            DataView dvVendor = new DataView(dt);
            DataTable vendors = dvVendor.ToTable(true, "VendorID");

            lblVendorCount.Text = vendors.Rows.Count.ToString();

            int varianceCount = 0;
            decimal highestVariance = 0;

            foreach (DataRow dr in dt.Rows)
            {
                decimal variance = Convert.ToDecimal(dr["VariancePercent"]);

                if (variance != 0)
                    varianceCount++;

                if (Math.Abs(variance) > highestVariance)
                    highestVariance = Math.Abs(variance);
            }

            lblVariance.Text = varianceCount.ToString();
            lblHighestVariance.Text = highestVariance.ToString("N2") + " %";
        }

        protected void gvQuotationVariance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            // Alternate colour by Item
            string currentItem = DataBinder.Eval(e.Row.DataItem, "ItemName").ToString();

            if (currentItem != Convert.ToString(ViewState["LastItem"]))
            {
                bool alt = ViewState["Alternate"] == null ? false : (bool)ViewState["Alternate"];
                ViewState["Alternate"] = !alt;
            }

            bool alternate = (bool)ViewState["Alternate"];

            if (alternate)
                e.Row.BackColor = Color.FromArgb(248, 248, 255);   // Lavender
            else
                e.Row.BackColor = Color.White;

            ViewState["LastItem"] = currentItem;

            decimal variance = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "VariancePercent"));
            string status = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Status"));

            // Change these indexes to match your GridView
            int varianceCellIndex = 6;   // Variance %
            int statusCellIndex = 8;     // Status

            TableCell varianceCell = e.Row.Cells[varianceCellIndex];
            TableCell statusCell = e.Row.Cells[statusCellIndex];

            varianceCell.Font.Bold = true;
            statusCell.Font.Bold = true;

            if (variance == 0)
            {
                varianceCell.BackColor = Color.FromArgb(223, 255, 223);
                statusCell.BackColor = Color.FromArgb(223, 255, 223);
                statusCell.ForeColor = Color.DarkGreen;
            }
            else if (Math.Abs(variance) <= 5)
            {
                varianceCell.BackColor = Color.FromArgb(255, 247, 204);
                statusCell.BackColor = Color.FromArgb(255, 247, 204);
                statusCell.ForeColor = Color.DarkOrange;
            }
            else
            {
                varianceCell.BackColor = Color.FromArgb(255, 220, 220);
                statusCell.BackColor = Color.FromArgb(255, 220, 220);
                statusCell.ForeColor = Color.DarkRed;
            }

            // Optional: colour negative variance red, positive green
            if (variance < 0)
                varianceCell.ForeColor = Color.Red;
            else if (variance > 0)
                varianceCell.ForeColor = Color.DarkGreen;
            string currentGroup =
    DataBinder.Eval(e.Row.DataItem, "VendorName") + "|" +
    DataBinder.Eval(e.Row.DataItem, "ItemName");

            string lastGroup = Convert.ToString(ViewState["LastGroup"]);

            if (currentGroup != lastGroup)
            {
                e.Row.CssClass += " group-start";

                ViewState["LastGroup"] = currentGroup;
            }
        }

        protected void gvQuotationVariance_RowCommand(object sender,
                                        GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetail")
            {
                int detailID =
                    Convert.ToInt32(e.CommandArgument);

                LoadDetail(detailID);
            }
        }

        private void LoadDetail(int vendorQuotationDetailsID)
        {
            DataTable dt =
                _pur.GetQuotationVarianceDetail(vendorQuotationDetailsID);

            dvVariance.DataSource = dt;
            dvVariance.DataBind();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (gvQuotationVariance.SelectedIndex < 0)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please select a quotation.');",
                    true);

                return;
            }

            int quotationDetailID =
                Convert.ToInt32(gvQuotationVariance.DataKeys[
                gvQuotationVariance.SelectedIndex].Value);

            _pur.ApproveQuotationVariance(
                quotationDetailID,
                txtRemarks.Text,
                Session["UserID"].ToString());

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('Variance Approved Successfully.');",
                true);

            Search();
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (gvQuotationVariance.SelectedIndex < 0)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please select a quotation.');",
                    true);

                return;
            }

            int quotationDetailID =
                Convert.ToInt32(gvQuotationVariance.DataKeys[
                gvQuotationVariance.SelectedIndex].Value);

            _pur.RejectQuotationVariance(
                quotationDetailID,
                txtRemarks.Text,
                Session["UserID"].ToString());

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('Quotation Rejected.');",
                true);

            Search();
        }
    }
}