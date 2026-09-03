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
    public partial class SendReport : System.Web.UI.Page
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
                // Call DAL method with year and month filter
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

        protected void gvReportList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SendReport")
            {
                string sampleID = e.CommandArgument.ToString();
                Session["SampleID"] = sampleID;
                Response.Redirect("SendReportMail.aspx");
            }
        }

        protected void gvReportNewList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReportNewList.PageIndex = e.NewPageIndex;

            // Retrieve the data from ViewState
            DataTable dt = ViewState["ReportData"] as DataTable;

            if (dt != null)
            {
                gvReportNewList.DataSource = dt;
                gvReportNewList.DataBind();
            }
            else
            {
                // If data is not in ViewState, reload it
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
                    //DataTable dtRev = _cm.GetMaxRevisionNo(sampleIdInt);
                    DataTable dtRev = _cm.GetRevisionCountSampleIdWise(Sample_ID);

                    if (dtRev != null && dtRev.Rows.Count > 0 && dtRev.Rows[0]["RevisionCount"] != DBNull.Value)
                        latestRevision = Convert.ToInt32(dtRev.Rows[0]["RevisionCount"]);
                }
                string revSuffix = latestRevision > 0 ? $"_Rev{latestRevision}" : "";

                Button btnViewReport = (Button)e.Row.FindControl("btnViewReport");
                if (btnViewReport != null)
                {
                    string pdfPath = Server.MapPath($"~/TestReport/TestReport-{sampleID}.pdf");
                    string pdfPathNA = Server.MapPath($"~/TestReport/TestReport-{sampleID}_BH.pdf");
                    string DraftpdfPath = Server.MapPath($"~/DraftFiles/TestReport-{sampleID}_Draft.pdf");
                    string DraftpdfPathNA = Server.MapPath($"~/DraftFiles/TestReport-{sampleID}_BH_Draft.pdf");

                    bool nablExists = File.Exists(pdfPath);
                    bool nonNablExists = File.Exists(pdfPathNA);
                    bool DarftNablExists = File.Exists(DraftpdfPath);
                    bool DraftNonNablExists = File.Exists(DraftpdfPathNA);

                    string pdfUrl = "";
                    string pdfUrlNA = "";
                    string DraftpdfUrl = "";
                    string DraftpdfUrlNA = "";

                    if (latestRevision == 0)
                    {
                        pdfUrl = $"/TestReport/TestReport-{sampleID}.pdf";
                        pdfUrlNA = $"/TestReport/TestReport-{sampleID}_BH.pdf";
                        DraftpdfUrl = $"/DraftFiles/TestReport-{sampleID}_Draft.pdf";
                        DraftpdfUrlNA = $"/DraftFiles/TestReport-{sampleID}_BH_Draft.pdf";
                    }
                    else
                    {
                        pdfUrl = $"/TestReport/TestReport-{sampleID}_Rev{latestRevision}.pdf";
                        pdfUrlNA = $"/TestReport/TestReport-{sampleID}_Rev{latestRevision}_BH.pdf";
                        DraftpdfUrl = $"/DraftFiles/TestReport-{sampleID}_Rev{latestRevision}_Draft.pdf";
                        DraftpdfUrlNA = $"/DraftFiles/TestReport-{sampleID}_Rev{latestRevision}_BH_Draft.pdf";
                    }

                    if (nablExists && nonNablExists && DarftNablExists && DraftNonNablExists)
                    {
                        btnViewReport.OnClientClick = $"window.open('{pdfUrl}', '_blank'); window.open('{DraftpdfUrl}', '_blank'); " +
                                                      $"window.open('{pdfUrlNA}', '_blank'); window.open('{DraftpdfUrlNA}', '_blank'); return false;";
                        btnViewReport.Enabled = true;
                        btnViewReport.CssClass = "btn btn-info btn-sm";
                        btnViewReport.Text = "View Report";
                    }
                    else if (nablExists && nonNablExists)
                    {
                        btnViewReport.OnClientClick = $"window.open('{pdfUrl}', '_blank'); window.open('{pdfUrlNA}', '_blank'); return false;";
                        btnViewReport.Enabled = true;
                        btnViewReport.CssClass = "btn btn-info btn-sm";
                        btnViewReport.Text = "View Report";
                    }
                    else if (nablExists && DarftNablExists)
                    {
                        btnViewReport.OnClientClick = $"window.open('{pdfUrl}', '_blank'); window.open('{DraftpdfUrl}', '_blank'); return false;";
                        btnViewReport.Enabled = true;
                        btnViewReport.CssClass = "btn btn-info btn-sm";
                        btnViewReport.Text = "View Report";
                    }
                    else if (nonNablExists && DraftNonNablExists)
                    {
                        btnViewReport.OnClientClick = $"window.open('{nonNablExists}', '_blank'); window.open('{DraftNonNablExists}', '_blank'); return false;";
                        btnViewReport.Enabled = true;
                        btnViewReport.CssClass = "btn btn-info btn-sm";
                        btnViewReport.Text = "View Report";
                    }
                    else if (nablExists)
                    {
                        btnViewReport.OnClientClick = $"window.open('{pdfUrl}', '_blank'); return false;";
                        btnViewReport.Enabled = true;
                        btnViewReport.CssClass = "btn btn-info btn-sm";
                        btnViewReport.Text = "View Report";
                    }
                    else if (nonNablExists)
                    {
                        btnViewReport.OnClientClick = $"window.open('{pdfUrlNA}', '_blank'); return false;";
                        btnViewReport.Enabled = true;
                        btnViewReport.CssClass = "btn btn-info btn-sm";
                        btnViewReport.Text = "View Report";
                    }
                    else
                    {
                        btnViewReport.Enabled = false;
                        btnViewReport.OnClientClick = "";
                        btnViewReport.CssClass = "btn btn-secondary btn-sm";
                        btnViewReport.Text = "Not Generated";
                    }
                }

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

                // Send button //
                LinkButton btnSend = (LinkButton)e.Row.FindControl("btnSend");
                if (btnSend != null)
                {
                    if (isSent >= 1)
                    {
                        btnSend.Enabled = true;
                        btnSend.CssClass = "btn btn-primary btn-sm";
                        btnSend.Text = "Send";
                    }
                    else
                    {
                        btnSend.Enabled = true;
                        btnSend.CssClass = "btn btn-danger btn-sm";
                        btnSend.Text = "Send";
                    }
                }

            }
        }
    }
}