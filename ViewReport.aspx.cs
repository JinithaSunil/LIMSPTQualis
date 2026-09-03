using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.DataVisualization.Charting;

namespace LIMS
{
    public partial class ViewReport : System.Web.UI.Page
    {
        private LIMS.DAL.CM _cm = new LIMS.DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Prevent caching of the page //
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            if (Request.QueryString["ID"] != null)
            {
                string id = Request.QueryString["ID"].ToString();
                string basePath = "~/TestReport/TestReport-" + id;
                string baseDraftPath = "~/DraftFiles/TestReport-" + id;
                int revisionCount = 0;
                string SampleIDSeries = "";
                DataTable dtSID = _cm.GetRevisionCountSampleUIDWise(id);
                if (dtSID != null || dtSID.Rows.Count > 0)
                {
                    DataRow row = dtSID.Rows[0];
                    revisionCount = row["RevisionCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["RevisionCount"].ToString());
                    SampleIDSeries = row["sampleIDSeries"].ToString();
                }

                Dictionary<string, string> files;
                if (revisionCount > 0)
                {
                    files = new Dictionary<string, string>
                    {
                        { $"{basePath}_Rev{revisionCount}.pdf", "Final Report PDF" },
                        { $"{basePath}_Rev{revisionCount}_BH.pdf", "Final Report BH PDF" },
                        { $"{baseDraftPath}_Rev{revisionCount}_Draft.pdf", "Draft PDF" },
                        { $"{baseDraftPath}_Rev{revisionCount}_BH_Draft.pdf", "Draft BH PDF" },
                        { $"{basePath}_Rev{revisionCount}.docx", "Word" },
                        { $"{basePath}_Rev{revisionCount}_BH.docx", "BH Word" }
                    };
                }
                else
                {
                    files = new Dictionary<string, string>
                    {
                        { $"{basePath}.pdf", "Final Report PDF" },
                        { $"{basePath}_BH.pdf", "Final Report BH PDF" },
                        { $"{baseDraftPath}_Draft.pdf", "Draft PDF" },
                        { $"{baseDraftPath}_BH_Draft.pdf", "Draft BH PDF" },
                        { $"{basePath}.docx", "Word" },
                        { $"{basePath}_BH.docx", "BH Word" }
                    };
                }

                StringBuilder html = new StringBuilder();
                html.Append(@"<style>
                                .report-container {
                                    max-width: 1400px;
                                    margin: 0 auto;
                                    padding: 30px 20px;
                                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                    background: #f8f9fa;
                                    min-height: 100vh;
                                }
                                .report-header {
                                    background: linear-gradient(135deg, #6e2e61 0%, #803369 100%);
                                    padding: 5px 5px;
                                    border-radius: 16px;
                                    margin-bottom: 35px;
                                    box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
                                    color: white;
                                }
                                .report-header h2 {
                                    margin: 0;
                                    font-size: 28px;
                                    font-weight: 600;
                                }
                                .report-header .subtitle {
                                    opacity: 0.9;
                                    margin-top: 8px;
                                    font-size: 14px;
                                }
                                .report-grid {
                                    display: grid;
                                    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
                                    gap: 24px;
                                }
                                .report-card {
                                    background: white;
                                    border-radius: 16px;
                                    padding: 0;
                                    box-shadow: 0 2px 12px rgba(0,0,0,0.06);
                                    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
                                    text-decoration: none;
                                    color: inherit;
                                    overflow: hidden;
                                    display: block;
                                    border: 1px solid rgba(0,0,0,0.04);
                                }
                                .report-card:hover {
                                    transform: translateY(-6px);
                                    box-shadow: 0 20px 40px rgba(0,0,0,0.12);
                                    border-color: transparent;
                                }
                                .report-card-content {
                                    padding: 24px 20px 20px 20px;
                                    text-align: center;
                                }
                                .report-icon-wrapper {
                                    width: 72px;
                                    height: 72px;
                                    margin: 0 auto 16px;
                                    background: #f0f4ff;
                                    border-radius: 50%;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    transition: all 0.3s ease;
                                }
                                .report-card:hover .report-icon-wrapper {
                                    background: #e8edff;
                                    transform: scale(1.05);
                                }
                                .report-icon-wrapper img {
                                    width: 40px;
                                    height: 40px;
                                    object-fit: contain;
                                }
                                .report-name {
                                    font-weight: 600;
                                    font-size: 15px;
                                    color: #1a1a2e;
                                    margin-bottom: 6px;
                                    letter-spacing: 0.3px;
                                }
                                .report-size {
                                    font-size: 13px;
                                    color: #6c757d;
                                    margin-bottom: 16px;
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    gap: 6px;
                                }
                                .report-action {
                                    display: inline-block;
                                    padding: 8px 24px;
                                    border-radius: 20px;
                                    font-size: 13px;
                                    font-weight: 500;
                                    background: #f0f4ff;
                                    color: #667eea;
                                    transition: all 0.3s ease;
                                }
                                .report-card:hover .report-action {
                                    background: #667eea;
                                    color: white;
                                    transform: scale(1.02);
                                }
                                .report-card.pdf .report-action {
                                    background: #e8f0fe;
                                    color: #1a73e8;
                                }
                                .report-card.pdf:hover .report-action {
                                    background: #1a73e8;
                                    color: white;
                                }
                                .report-card.word .report-action {
                                    background: #e8f5e9;
                                    color: #2e7d32;
                                }
                                .report-card.word:hover .report-action {
                                    background: #2e7d32;
                                    color: white;
                                }
                                .report-empty {
                                    background: #f8f9fa;
                                    border: 2px dashed #dee2e6;
                                    border-radius: 16px;
                                    padding: 30px;
                                    text-align: center;
                                    color: #adb5bd;
                                    display: flex;
                                    flex-direction: column;
                                    align-items: center;
                                    justify-content: center;
                                    min-height: 200px;
                                }
                                .report-empty .empty-icon {
                                    font-size: 48px;
                                    margin-bottom: 10px;
                                }
                                .report-empty .empty-text {
                                    font-size: 14px;
                                }
                                .report-badge {
                                    position: absolute;
                                    top: 12px;
                                    right: 12px;
                                    background: rgba(102, 126, 234, 0.1);
                                    color: #667eea;
                                    font-size: 10px;
                                    padding: 2px 10px;
                                    border-radius: 12px;
                                    font-weight: 600;
                                    text-transform: uppercase;
                                    letter-spacing: 0.5px;
                                }
                                .report-card {
                                    position: relative;
                                }
                                @media (max-width: 768px) {
                                    .report-grid {
                                        grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
                                        gap: 16px;
                                    }
                                    .report-header {
                                        padding: 20px;
                                    }
                                    .report-header h2 {
                                        font-size: 22px;
                                    }
                                }
                            </style>");

                html.Append("<div class='report-container'>");

                html.Append($@"<div class='report-header'>
                                    <h4>📄 Reports for ID: {SampleIDSeries}</h4>
                                    <div class='subtitle'>Click any card to view or download the report</div>
                                </div>");

                html.Append("<div class='report-grid'>");

                foreach (var file in files)
                {
                    string fullPath = HttpContext.Current.Server.MapPath(file.Key);

                    if (File.Exists(fullPath))
                    {
                        string ext = Path.GetExtension(file.Key).ToLower();
                        string displayName = file.Value;
                        bool isPdf = ext == ".pdf";
                        FileInfo fileInfo = new FileInfo(fullPath);
                        string fileSize = (fileInfo.Length / 1024.0 / 1024.0).ToString("F2") + " MB";
                        string fileUrl = VirtualPathUtility.ToAbsolute(file.Key) + "?v=" + fileInfo.LastWriteTimeUtc.Ticks;
                        string imagePath = isPdf ? "~/assets/img/pdf-icon.png" : "~/assets/img/word-icon.png";
                        string imageUrl = VirtualPathUtility.ToAbsolute(imagePath);
                        // For PDF: open in new tab, for Word: download //
                        string target = isPdf ? "_blank" : "_self";
                        string downloadAttr = isPdf ? "" : "download";
                        string cardClass = isPdf ? "pdf" : "word";
                        string actionText = isPdf ? "📖 View PDF" : "⬇ Download Word";
                        string badge = isPdf ? "PDF" : "DOCX";

                        html.Append($@"<a href='{fileUrl}' target='{target}' {downloadAttr} class='report-card {cardClass}'>
                                        <div class='report-card-content'>
                                            <div class='report-icon-wrapper'>
                                                <img src='{imageUrl}' alt='{(isPdf ? "PDF" : "Word")} Icon' />
                                            </div>
                                            <div class='report-name'>{displayName}</div>
                                            <div class='report-size'>
                                                <span>📊</span> {fileSize}
                                            </div>
                                            <div class='report-action'>{actionText}</div>
                                        </div>
                                        <div class='report-badge'>{badge}</div>
                                    </a>");
                    }
                    else
                    {
                        // Empty placeholder
                        html.Append($@"<div class='report-empty'>
                                            <div class='empty-icon'>📄</div>
                                            <div class='empty-text'>No File Available</div>
                                        </div>");
                    }
                }

                html.Append("</div>"); // Close grid
                html.Append("</div>"); // Close container
                divTestResult.InnerHtml = html.ToString();
            }
        }

    }
}