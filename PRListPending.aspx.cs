using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;
using LIMS.DAL;

namespace LIMS
{
    public partial class PRListPending : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        private string currentDepartment;
        private string currentUserID;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Assuming UserID is stored in session after login
            currentUserID = Session["UserID"]?.ToString();

            if (!IsPostBack && !string.IsNullOrEmpty(currentUserID))
            {
                DataTable userDetails = _cm.GetEmpDetails(currentUserID);
                if (userDetails != null && userDetails.Rows.Count > 0)
                {

                    if (Session["RoleID"] != null)
                    {
                        BindDivisions();
                        BindPR();
                    }

                }
                else
                {
                    // Handle case where user details cannot be retrieved
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Detail Not Found');", true);
                }
            }
        }
        private void BindDivisions()
        {
            DataTable dt = _cm.GetAllDivisions("LIMS");
            if (dt.Rows.Count > 0)
            {
                ddlDivision.DataSource = dt;
                ddlDivision.DataTextField = "DivisionName";
                ddlDivision.DataValueField = "DivisionID";
                ddlDivision.DataBind();
                ddlDivision.Items.Insert(0, new ListItem("Select Department", ""));
            }

        }
        private void BindPR()
        {
            int Tier = Convert.ToInt32(radChoice.SelectedValue.ToString());
            // Hide GM GridView and show Requests GridView
            gvRequests.Visible = true;
            DataTable dt = _cm.GetPendingPRByTier(Tier);

            if (dt.Rows.Count > 0)
            {
                gvRequests.DataSource = dt;
                gvRequests.DataBind();
            }
            else
            {
                gvRequests.DataSource = null;
                gvRequests.DataBind();
            }
            lblRecords.Text = dt.Rows.Count.ToString();

        }
        protected void gvRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvRequests.PageIndex = e.NewPageIndex;
                BindPR();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Paging Error: {ex.Message}');", true);
            }
        }

        protected void radChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPR();
        }
        protected void BindFilteredData()
        {
            int tier = Convert.ToInt32(radChoice.SelectedValue);

            string division = ddlDivision.SelectedValue == "0"
                ? null
                : ddlDivision.SelectedValue;

            DateTime? fromDate = string.IsNullOrWhiteSpace(txtFromDate.Text)
                ? (DateTime?)null
                : Convert.ToDateTime(txtFromDate.Text);

            DateTime? toDate = string.IsNullOrWhiteSpace(txtToDate.Text)
                ? (DateTime?)null
                : Convert.ToDateTime(txtToDate.Text);

            string prNo = string.IsNullOrWhiteSpace(txtPPNo.Text)
                ? null
                : txtPPNo.Text.Trim();

            DataTable dt = _cm.GetPendingPRByTier(
                tier,
                division,
                fromDate,
                toDate,
                prNo);
            if (dt.Rows.Count > 0)
            {
                gvRequests.DataSource = dt;
                gvRequests.DataBind();
            }
            else
            {
                gvRequests.DataSource = null;
                gvRequests.DataBind();
            }
        }
        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFilteredData();
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            BindFilteredData();
        }

        protected void txtPPNo_TextChanged(object sender, EventArgs e)
        {
            BindFilteredData();
        }

        protected void txtToDate_TextChanged(object sender, EventArgs e)
        {
            BindFilteredData();
        }
    }
}