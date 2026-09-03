using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ReportRevisionDeleteSelect : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();

        private string sampleID;
        private int sampleRecordId;
        private int revisionCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            sampleID = Request.QueryString["SampleID"];

            if (string.IsNullOrEmpty(sampleID) || !int.TryParse(Request.QueryString["ID"], out sampleRecordId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Invalid sample reference.');", true);
                return;
            }

            revisionCount = GetRevisionCount();

            if (!Page.IsPostBack)
            {
                LoadFiles();
            }
        }

        private int GetRevisionCount()
        {
            int count = 0;
            DataTable dtRev = _cm.GetRevisionCountSampleIdWise(sampleRecordId);
            if (dtRev != null && dtRev.Rows.Count > 0 && dtRev.Rows[0]["RevisionCount"] != DBNull.Value)
                count = Convert.ToInt32(dtRev.Rows[0]["RevisionCount"]);
            return count;
        }

        private void LoadFiles()
        {
            List<FileItem> files = new List<FileItem>();

            for (int i = 0; i <= revisionCount; i++)
            {
                string suffix = i == 0 ? "" : $"_Rev{i}";
                bool isFinal = (i == revisionCount);
                string revisionLabel = i == 0 ? "Original" : $"Revision {i}";

                // TestReport files
                AddFileIfExists(files, i, isFinal, revisionLabel, "TestReport",
                    $"TestReport-{sampleID}{suffix}.pdf",
                    new[] { $"TestReport-{sampleID}{suffix}.docx", $"{sampleID}{suffix}.png" });

                AddFileIfExists(files, i, isFinal, revisionLabel, "TestReport",
                    $"TestReport-{sampleID}{suffix}_BH.pdf",
                    new[] { $"TestReport-{sampleID}{suffix}_BH.docx", $"{sampleID}{suffix}_BH.png" });

                // DraftFiles
                AddFileIfExists(files, i, isFinal, revisionLabel, "DraftFiles",
                    $"TestReport-{sampleID}{suffix}_Draft.pdf",
                    new[] { $"TestReport-{sampleID}{suffix}_Draft.docx" });

                AddFileIfExists(files, i, isFinal, revisionLabel, "DraftFiles",
                    $"TestReport-{sampleID}{suffix}_BH_Draft.pdf",
                    new[] { $"TestReport-{sampleID}{suffix}_BH_Draft.docx" });
            }

            // Sort by revision number descending (newest first)
            files = files.OrderByDescending(f => f.RevisionNumber).ThenBy(f => f.Folder).ThenBy(f => f.PdfFileName).ToList();

            gvFiles.DataSource = files;
            gvFiles.DataBind();

            int totalFiles = files.Count;
            int totalRevisions = files.Select(f => f.RevisionNumber).Distinct().Count();
            string SampleIDSeries = "";
            DataTable dtSID = _cm.GetRevisionCountSampleUIDWise(sampleID);
            if (dtSID != null || dtSID.Rows.Count > 0)
            {
                DataRow row = dtSID.Rows[0];
                revisionCount = row["RevisionCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["RevisionCount"].ToString());
                SampleIDSeries = row["sampleIDSeries"].ToString();
            }

            lblSampleInfo.Text = $"SampleIDSeries: <b>{HttpUtility.HtmlEncode(SampleIDSeries)}</b> &nbsp;|&nbsp; Total Files: <b>{totalFiles}</b> &nbsp;|&nbsp; Revisions: <b>{totalRevisions}</b>";
        }

        private void AddFileIfExists(List<FileItem> files, int revisionNumber, bool isFinal, string revisionLabel,
            string folder, string pdfFile, string[] relatedFiles)
        {
            string pdfPath = Server.MapPath($"~/{folder}/{pdfFile}");

            if (File.Exists(pdfPath))
            {
                files.Add(new FileItem
                {
                    RevisionNumber = revisionNumber,
                    IsFinal = isFinal,
                    RevisionLabel = revisionLabel,
                    Folder = folder,
                    PdfFileName = pdfFile,
                    RelativeUrl = $"/{folder}/{pdfFile}",
                    RelatedFileNames = relatedFiles
                });
            }
        }

        protected void gvFiles_RowDataBound1(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileItem item = e.Row.DataItem as FileItem;
                if (item == null) return;

                // Store this row's file info on the checkbox itself,
                // so we know exactly what to delete when it's checked.
                CheckBox chkSelect = e.Row.FindControl("chkSelect") as CheckBox;
                if (chkSelect != null)
                {
                    chkSelect.Attributes["data-revision"] = item.RevisionNumber.ToString();
                    chkSelect.Attributes["data-folder"] = item.Folder;
                    chkSelect.Attributes["data-filename"] = item.PdfFileName;
                    chkSelect.Attributes["data-related-files"] = string.Join(",", item.RelatedFileNames ?? new string[0]);
                }
            }
        }

        protected string GetRelatedFilesDisplay(object relatedFilesObj)
        {
            string[] relatedFiles = relatedFilesObj as string[];
            if (relatedFiles == null || relatedFiles.Length == 0)
                return "None";

            List<string> fileNames = new List<string>();
            foreach (string file in relatedFiles)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    fileNames.Add(Path.GetFileName(file));
                }
            }

            return string.Join(", ", fileNames);
        }

        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            int deletedFiles = 0;
            List<string> deletedFilesList = new List<string>();

            try
            {
                foreach (GridViewRow row in gvFiles.Rows)
                {
                    if (row.RowType != DataControlRowType.DataRow) continue;

                    CheckBox chkSelect = row.FindControl("chkSelect") as CheckBox;
                    if (chkSelect == null || !chkSelect.Checked) continue;

                    string folder = chkSelect.Attributes["data-folder"];
                    string pdfFileName = chkSelect.Attributes["data-filename"];
                    string relatedFilesAttr = chkSelect.Attributes["data-related-files"];

                    if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(pdfFileName)) continue;

                    string pdfPath = Server.MapPath($"~/{folder}/{pdfFileName}");

                    // Delete the PDF for this row
                    if (DeleteFileIfExists(pdfPath))
                    {
                        deletedFiles++;
                        deletedFilesList.Add($"PDF: {folder}/{pdfFileName}");
                    }

                    // Delete the matching DOCX / PNG for this same row
                    if (!string.IsNullOrEmpty(relatedFilesAttr))
                    {
                        string[] relatedFiles = relatedFilesAttr.Split(',');
                        foreach (string relatedFile in relatedFiles)
                        {
                            if (!string.IsNullOrEmpty(relatedFile))
                            {
                                string relatedPath = Server.MapPath($"~/{folder}/{relatedFile.Trim()}");
                                if (DeleteFileIfExists(relatedPath))
                                {
                                    deletedFiles++;
                                    deletedFilesList.Add($"{Path.GetExtension(relatedFile).ToUpper().Replace(".", "")}: {folder}/{relatedFile}");
                                }
                            }
                        }
                    }
                }

                if (deletedFiles > 0)
                {
                    lblMessage.Text = $"Successfully deleted {deletedFiles} file(s) (PDF + matching DOCX/PNG).";
                    lblMessage.CssClass = "message-success";
                }
                else
                {
                    lblMessage.Text = "No files were selected for deletion.";
                    lblMessage.CssClass = "message-warning";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblMessage.CssClass = "message-error";
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Error deleting files: " + ex.Message.Replace("'", "\\'") + "');", true);
            }

            // Reload the grid to reflect the deleted files
            LoadFiles();
        }

        private bool DeleteFileIfExists(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting {filePath}: {ex.Message}");
            }
            return false;
        }

        protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileItem item = e.Row.DataItem as FileItem;
                if (item == null) return;

                CheckBox chkSelect = e.Row.FindControl("chkSelect") as CheckBox;
                if (chkSelect != null)
                {
                    chkSelect.Attributes["data-revision"] = item.RevisionNumber.ToString();
                    chkSelect.Attributes["data-folder"] = item.Folder;
                    chkSelect.Attributes["data-filename"] = item.PdfFileName;
                    chkSelect.Attributes["data-related-files"] = string.Join(",", item.RelatedFileNames ?? new string[0]);
                }

                // Visually mark current/final revision rows as locked (not deletable)
                if (item.IsFinal)
                {
                    e.Row.CssClass += " row-locked";
                }
            }
        }

    }

    public class FileItem
    {
        public int RevisionNumber { get; set; }
        public bool IsFinal { get; set; }
        public string RevisionLabel { get; set; }
        public string Folder { get; set; }
        public string PdfFileName { get; set; }
        public string RelativeUrl { get; set; }
        public string[] RelatedFileNames { get; set; }
    }
}

