using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ReportRevisionDelete : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!Page.IsPostBack)
            {
                Label lbl = (Label)Master.FindControl("lblTitle");
                if (lbl != null)
                {
                    lbl.Text = "Reports Status";
                }

                BindYear();
                string year = DateTime.Now.Year.ToString();
                ddlYear.Items.FindByValue(year).Selected = true;
                string month = DateTime.Now.Month.ToString();
                ddlMonth.Items.FindByValue(month).Selected = true;

                BindReports();
            }
        }

        private void BindYear()
        {
            System.Collections.Generic.List<int> years = new System.Collections.Generic.List<int>();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                years.Add(year);
            }

            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("Select Year", "0"));
        }

        private void BindReports()
        {
            string DepartmentID = "";
            try
            {
                if (ddlYear.SelectedValue == "0") return;
                if (ddlMonth.SelectedIndex == 0) return;

                int year = Convert.ToInt32(ddlYear.SelectedValue);
                int month = Convert.ToInt32(ddlMonth.SelectedValue);

                DepartmentID = Session["DepartmentID"].ToString();

                DataTable dtNew = _cm.getAllSamplesForEmailReport(year, month, DepartmentID);
                gvReportNewList.DataSource = dtNew;
                gvReportNewList.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Error: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvReportNewList.PageIndex = 0;
            BindReports();
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvReportNewList.PageIndex = 0;
            BindReports();
        }

        protected void gvReportNewList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReportNewList.PageIndex = e.NewPageIndex;

            DataTable dt = ViewState["ReportData"] as DataTable;

            if (dt != null)
            {
                gvReportNewList.DataSource = dt;
                gvReportNewList.DataBind();
            }
            else
            {
                BindReports();
            }
        }

        protected void gvReportNewList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView row = (DataRowView)e.Row.DataItem;
                string sampleID = row["SampleID"].ToString();
                int isSent = Convert.ToInt32(row["IsEmailSent"].ToString());
                int Sample_ID = Convert.ToInt32(row["ID"]);

                // Get latest revision number for this sample ////
                int latestRevision = 0;
                if (int.TryParse(sampleID, out int sampleIdInt))
                {
                    DataTable dtRev = _cm.GetRevisionCountSampleIdWise(Sample_ID);

                    if (dtRev != null && dtRev.Rows.Count > 0 && dtRev.Rows[0]["RevisionCount"] != DBNull.Value)
                        latestRevision = Convert.ToInt32(dtRev.Rows[0]["RevisionCount"]);
                }
                string revSuffix = latestRevision > 0 ? $"_Rev{latestRevision}" : "";

                
                // Status Badge //
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                if (lblStatus != null)
                {
                    if (isSent == 1)
                    {
                        lblStatus.Text = "Email Sent";
                    }
                    else if (isSent > 1)
                    {
                        lblStatus.Text = "Email Sent " + isSent + " times";
                    }
                    else
                    {
                        lblStatus.Text = "New";
                    }
                }

                // Delete button now opens the selection page in a new tab instead of deleting directly //
                LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    string deleteUrl = $"ReportRevisionDeleteSelect.aspx?ID={Sample_ID}&SampleID={HttpUtility.UrlEncode(sampleID)}";
                    btnDelete.OnClientClick = $"window.open('{deleteUrl}', '_blank'); return false;";

                    if (isSent >= 1)
                    {
                        btnDelete.Enabled = true;
                        btnDelete.CssClass = "btn btn-primary btn-sm";
                        btnDelete.Text = "Delete";
                        btnDelete.ToolTip = "Report Already Sent";
                    }
                    else
                    {
                        btnDelete.Enabled = true;
                        btnDelete.CssClass = "btn btn-danger btn-sm";
                        btnDelete.Text = "Delete";
                        btnDelete.ToolTip = "Report Not Sent";
                    }
                }
            }
        }

        // Kept for backward compatibility - no longer triggered since btnDelete now opens
        // ReportRevisionDeleteSelect.aspx via OnClientClick instead of posting back.
        protected void gvReportList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }
    }
}