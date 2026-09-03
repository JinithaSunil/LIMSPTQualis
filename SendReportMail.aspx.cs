using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class SendReportMail : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                Label lbl = (Label)Master.FindControl("lblTitle");
                if (lbl != null)
                {
                    lbl.Text = "Send Report - Mail";
                }
                LoadReportData();
            }
        }

        private void LoadReportData()
        {
            string sampleID = Session["SampleID"]?.ToString();

            if (string.IsNullOrEmpty(sampleID))
            {
                ShowMessage("No report selected.");
                return;
            }

            DataTable dt = _cm.GetReportDetailsForSending(Convert.ToInt32(sampleID));

            if (dt == null || dt.Rows.Count == 0)
            {
                ShowMessage("No report details found.");
                return;
            }

            DataRow row = dt.Rows[0];

            string customerEmail = row["CustomerEmail"].ToString();
            string customerName = row["CustomerName"].ToString();
            string testReportNo = row["TestReportNo"].ToString();
            int sampleIDVal = Convert.ToInt32(row["SampleID"]);
            int cloneCRFSampleID = Convert.ToInt32(row["CloneCRFSampleID"]);
            string sampleUID = row["SampleUID"].ToString();
            int Sample_ID = Convert.ToInt32(row["Sample_ID"]);

            // Find all attachments (latest revision only)
            var attachments = FindAllAttachments(sampleUID, sampleIDVal, Sample_ID);

            // Store the FULL original list in Session — this is the source of truth
            // that indexes will be matched against on send.
            Session["AttachmentPaths"] = string.Join("|", attachments);

            // Store for sending
            Session["SampleID"] = sampleID;
            Session["CustomerName"] = customerName;
            Session["TestReportNo"] = testReportNo;

            // Set email fields
            txtTo.Text = customerEmail;
            txtSubject.Text = "Test Report# - " + testReportNo;
            txtBCC.Text = "support@rnisolutions.in";
            txtBCC.ReadOnly = true;

            if (attachments.Count > 0)
            {
                var sb = new System.Text.StringBuilder();
                var allIndexes = new List<string>();

                for (int i = 0; i < attachments.Count; i++)
                {
                    string path = attachments[i];
                    string fileName = Path.GetFileName(path);
                    string fileUrl = path.Contains("DraftFiles") ? "/DraftFiles/" + fileName : "/TestReport/" + fileName;

                    sb.Append($"<div style='margin:5px 0;' data-idx='{i}'>");
                    sb.Append($"<a href='{fileUrl}' target='_blank'>{fileName}</a>");
                    sb.Append($" <span style='cursor:pointer; color:red; margin-left:10px;' onclick=\"removeAttachment(this, {i})\">&#10006;</span>");
                    sb.Append($"</div>");

                    allIndexes.Add(i.ToString());
                }

                litAttachments.Text = sb.ToString();

                // seed hidden field with ALL indexes present at load time; JS trims it on removal
                hdRemainingIndexes.Value = string.Join(",", allIndexes);
            }
            else
            {
                hdRemainingIndexes.Value = "";
            }

            // Prepare email body
            string body = objEmail.PrepareMailBodyWith("EmailReportSent.html",
                "{contactName}", customerName,
                "{ContactNumber}", "");

            ltMessage.Text = body;
            divMessage.InnerHtml = body;
            divMessage.Visible = true;
        }

        private List<string> FindAllAttachments(string sampleUID, int sampleIDVal, int Sample_ID)
        {
            var attachments = new List<string>();

            int latestRevision = 0;
            DataTable dtRev = _cm.GetRevisionCountSampleIdWise(Sample_ID);

            if (dtRev != null && dtRev.Rows.Count > 0 && dtRev.Rows[0]["RevisionCount"] != DBNull.Value)
                latestRevision = Convert.ToInt32(dtRev.Rows[0]["RevisionCount"]);


            if (latestRevision == 0)
            {
                string nabl = Server.MapPath($"~/TestReport/TestReport-{sampleUID}.pdf");
                if (File.Exists(nabl)) attachments.Add(nabl);

                string nonNabl = Server.MapPath($"~/TestReport/TestReport-{sampleUID}_BH.pdf");
                if (File.Exists(nonNabl)) attachments.Add(nonNabl);

                string draftNabl = Server.MapPath($"~/DraftFiles/TestReport-{sampleUID}.pdf");
                if (File.Exists(draftNabl)) attachments.Add(draftNabl);

                string draftNonNabl = Server.MapPath($"~/DraftFiles/TestReport-{sampleUID}_BH.pdf");
                if (File.Exists(draftNonNabl)) attachments.Add(draftNonNabl);
            }
            else
            {
                string nabl = Server.MapPath($"~/TestReport/TestReport-{sampleUID}_Rev{latestRevision}.pdf");
                if (File.Exists(nabl)) attachments.Add(nabl);

                string nonNabl = Server.MapPath($"~/TestReport/TestReport-{sampleUID}_Rev{latestRevision}_BH.pdf");
                if (File.Exists(nonNabl)) attachments.Add(nonNabl);

                string draftNabl = Server.MapPath($"~/DraftFiles/TestReport-{sampleUID}_Rev{latestRevision}_Draft.pdf");
                if (File.Exists(draftNabl)) attachments.Add(draftNabl);

                string draftNonNabl = Server.MapPath($"~/DraftFiles/TestReport-{sampleUID}_Rev{latestRevision}_BH_Draft.pdf");
                if (File.Exists(draftNonNabl)) attachments.Add(draftNonNabl);
            }

            return attachments;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string sampleID = Session["SampleID"].ToString();

                // Full original list from Session (this is the "master" list, indexed 0..n-1)
                string[] allPaths = (Session["AttachmentPaths"]?.ToString() ?? "")
                    .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                // Indexes still remaining after user removed some rows client-side
                string[] remainingIndexStrings = (hdRemainingIndexes.Value ?? "")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                List<string> validPaths = new List<string>();
                foreach (string idxStr in remainingIndexStrings)
                {
                    if (int.TryParse(idxStr, out int idx) && idx >= 0 && idx < allPaths.Length)
                    {
                        string path = allPaths[idx];
                        if (File.Exists(path))
                            validPaths.Add(path);
                    }
                }

                // Create email object
                DAL.E_MailCommunication eml = new DAL.E_MailCommunication();
                eml.To = txtTo.Text;
                eml.Subject = txtSubject.Text;
                eml.CC = txtCC.Text;
                eml.BCC = txtBCC.Text;
                eml.Message = hdMessage.Value;

                // Send email with attachments
                if (validPaths.Count > 0)
                {
                    // Pass the List<string> directly
                    eml.ReportSentWithMultipleAttachments(eml, validPaths);
                }
                else
                {
                    // Send without attachments
                    eml.ReportSent(eml);
                }

                // Mark as sent
                _cm.GetDataUsingQuery("UPDATE SampleDetails SET IsReportSent = 1 WHERE ID = " + sampleID);

                int SampleId = Convert.ToInt32(sampleID);

                DataTable dtSID = _cm.GetRevisionCountSampleIdWise(SampleId);
                if (dtSID != null && dtSID.Rows.Count > 0)
                {
                    DataRow row = dtSID.Rows[0];
                    int IsEmailSent = Convert.ToInt32(row["IsEmailSent"].ToString());
                    int TotalEamilCount = IsEmailSent + 1;

                    bool updateSuccess = _cm.SendReportViaEmail(SampleId, TotalEamilCount);
                    if (!updateSuccess)
                    {
                        Debug.WriteLine($"Failed to update database for SampleId: {sampleID}");
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Report Sent Successfully!'); window.location='SendReport.aspx';", true);
            }
            catch (Exception ex)
            {
                // Mark as failed
                _cm.GetDataUsingQuery(
                    "UPDATE SampleDetails SET IsReportSent = 2 WHERE ID = " + Session["SampleID"]);

                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Failed: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("SendReport.aspx");
        }

        private void ShowMessage(string message)
        {
            divMessage.InnerHtml = message;
            divMessage.Visible = true;
            btnSend.Enabled = false;
        }

    }
}