using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
// ADD THESE USING STATEMENTS FOR SYNC FUSION
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using ZXing;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using A = DocumentFormat.OpenXml.Drawing;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using DataTable = System.Data.DataTable;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace LIMS
{
    public partial class CreateReportFromTemplateFHB : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            string templatePath = ""; string template = ""; string oldTemplate = ""; string ReportType = "";

            try
            {
                /* Register Syncfusion License Key */
                //string licenseKey = "Ngo9BigBOggjHTQxAR8/V1JAaF5cX2pCd1p/TH5YfUNzdUVEY1ZUTXxaS1ZhSXxVdkJgWX1dcnJWRGFVUER9XEY=";

                // Get sampleID from query string
                int sampleID = 0;
                string CategoryName = "";
                string path = Server.MapPath("~/Inputfiles/");
                string pathout = Server.MapPath("~/TestReport/");

                if (Request.QueryString["s"] != null && Request.QueryString["c"] != null)
                {
                    sampleID = Convert.ToInt32(Request.QueryString["s"]);
                    CategoryName = Request.QueryString["c"].ToString();
                }
                string s = "", c = "", t = "";

                if (Request.QueryString["s"] != null)
                    s = Request.QueryString["s"].ToString();
                _cm.sampleId = Convert.ToInt32(s);
                if (Request.QueryString["c"] != null)
                    c = Request.QueryString["c"].ToString();
                if (Request.QueryString["t"] != null)
                    t = Request.QueryString["t"].ToString();

                int ID = Convert.ToInt32(s); string SampleUID = "";
                DataTable dt = _cm.getSampleUID(ID);
                if (dt.Rows.Count > 0)
                {
                    SampleUID = dt.Rows[0]["SampleID"].ToString();
                    GenerateQR(SampleUID);
                }

                // Get common data for TestReport from database//
                DataTable foodDetails = _cm.getFoodDetails(sampleID);



                DataTable dt1 = _cm.getReportQuery(c);
                oldTemplate = dt1.Rows[0]["Template"].ToString();
                DataTable dt9 = _cm.getTestReportID(ID);
                if (dt9.Rows.Count > 0)
                {
                    ReportType = dt9.Rows[0]["ReportType"].ToString();

                    if (ReportType.Trim() == "ACCREDITATION")
                    {
                        template = oldTemplate;
                        templatePath = path + template;
                    }
                    else
                    {
                        template = oldTemplate;
                        string name = Path.GetFileNameWithoutExtension(template);
                        string ext = Path.GetExtension(template);
                        string newName = $"{name}_NA{ext}";
                        templatePath = path + newName;
                    }


                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Report Type not found'); setTimeout(function() { window.location = 'GenerateReport.aspx'; }, 10);</script>");
                }

                using (FileStream fileStream = new FileStream(
                        templatePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                {
                    WordDocument document = new WordDocument(fileStream, FormatType.Docx);

                    string imageUrl = Server.MapPath("~/TestReport/" + SampleUID + ".png");
                    List<string> imagePaths = new List<string> { imageUrl };
                    InsertImagesIntoDocument(document, "{QR_Code}", imagePaths);

                    string certUrl = Server.MapPath("~/assets/img/cert.png");

                    string noncertUrl = Server.MapPath("~/assets/img/cert1.png");

                    string cert = "";
                    DataTable dt2 = new DataTable();
                    try
                    {
                        if (foodDetails != null && foodDetails.Rows.Count > 0)
                        {
                            // Replace placeholders with database values
                            document.Replace("{TestReportNo}", foodDetails.Rows[0]["testreportno"].ToString(), true, true);
                            document.Replace("{CustomerName}", foodDetails.Rows[0]["CustomerName"].ToString(), true, true);
                            document.Replace("{Address}", foodDetails.Rows[0]["Address"].ToString(), true, true);
                            document.Replace("{SampleDescription}", foodDetails.Rows[0]["SampleDescription"].ToString(), true, true);
                            document.Replace("{SampleNo}", foodDetails.Rows[0]["SampleNo"].ToString(), true, true);
                            document.Replace("{SampleCondition}", foodDetails.Rows[0]["SampleCondition"].ToString(), true, true);
                            document.Replace("{IssueDate}", Convert.ToDateTime(foodDetails.Rows[0]["issuedate"]).ToString("yyyy MMM dd"), true, true);
                            document.Replace("{ReceivedDate}", Convert.ToDateTime(foodDetails.Rows[0]["Receiveddate"]).ToString("yyyy MMM dd"), true, true);
                            document.Replace("{AnalysisCompletionDate}", Convert.ToDateTime(foodDetails.Rows[0]["AnalysisCompletionDate"]).ToString("yyyy MMM dd"), true, true);

                            document.Replace("{Buyer}", foodDetails.Rows[0]["Buyer"].ToString(), true, true);
                            document.Replace("{SampleCode}", foodDetails.Rows[0]["BatchNo"].ToString(), true, true);
                            document.Replace("{BatchNo}", foodDetails.Rows[0]["BatchNo"].ToString(), true, true);
                            document.Replace("{ServiceConfirmationNo}", foodDetails.Rows[0]["Service Confirmation No."].ToString(), true, true);
                            document.Replace("{LabHeadName}", foodDetails.Rows[0]["LabHeadName"].ToString(), true, true);

                            document.Replace("{DocNo}", "TSQ-LAB-QAF-089", true, true);

                            //ReplaceInHeaderFooter(document, "{DocNo}", foodDetails.Rows[0]["testreportno"].ToString());
                            //ReplaceInHeaderFooter(document, "{TestReportNo}", foodDetails.Rows[0]["testreportno"].ToString());

                            ViewState["SampleDescription"] = foodDetails.Rows[0]["SampleDescription"].ToString();
                        }

                        if (ViewState["SampleDescription"] != null)
                        {
                            // Generate unique file name based on current date and time
                            string uniqueFileName = SampleUID;// ViewState["SampleDescription"].ToString().Substring(0, 5) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                            string outputDocxPath = pathout + "TestReport-" + uniqueFileName + ".docx";
                            string outputPdfPath = pathout + "TestReport-" + uniqueFileName + ".pdf";
                            FileStream docStream = new FileStream(outputDocxPath, FileMode.Create, FileAccess.Write);
                            using (docStream)
                            {
                                document.Save(docStream, FormatType.Docx);
                            }
                            FileStream docStreampdf = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
                            using (docStreampdf)
                            {
                                document.Save(docStreampdf, FormatType.Docx);
                            }
                            int counter = 0;



                            if (dt1.Rows.Count > 0)
                            {
                                string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                                string[] orderby = dt1.Rows[0]["orderby"].ToString().Split('|');
                                string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');

                                for (int i = 0; i < query.Length; i++)
                                {
                                    counter = i;
                                    if (table[i] == "1")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        MultipleRequirementsCreateTable1(document, "{ResultTable1}", dt2);
                                    }
                                    if (table[i] == "2")
                                    {
                                        //dt2 = _cm.getReportQuery(1, query[i] + s);
                                        //CreateTable2(document, "{ResultTable1}", dt2);

                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        CreateNutritionalTable(document, "{ResultTable3}");
                                    }
                                    if (table[i] == "3")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        WithoutRequirementCreateTable1(document, "{ResultTable1}", dt2);
                                    }
                                    if (table[i] == "4")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        CreateTable4(document, "{ResultTable1}", dt2);
                                    }
                                    if (table[i] == "5")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        RequirementCreateTable1(document, "{ResultTable1}", dt2);
                                    }
                                    if (table[i] == "6")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        WithoutRequirementCreateTable1(document, "{ResultTable2}", dt2);
                                    }
                                    if (table[i] == "7")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s);

                                        List<string> imagePaths1 = new List<string>();
                                        foreach (DataRow row in dt2.Rows)
                                        {
                                            string imageFileName = row["filename"].ToString().Trim();

                                            if (!string.IsNullOrEmpty(imageFileName))
                                            {
                                                string fullImagePath = Server.MapPath("~/SampleImages/") + imageFileName;

                                                if (File.Exists(fullImagePath))
                                                {
                                                    imagePaths1.Add(fullImagePath);
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"Image not found: {fullImagePath}");
                                                }
                                            }
                                        }

                                        InsertImagesIntoDocument1(document, "{ images }", imagePaths1);

                                    }

                                    if (table[i] == "8")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        CreateTableLastRaisins(document, "{ResultTable2}", dt2);
                                    }
                                    if (table[i] == "9")
                                    {
                                        dt2 = _cm.getReportQuery(1, query[i] + s + orderby[i]);
                                        RequirementCreateTable2(document, "{ResultTable3}", dt2);
                                    }
                                }

                                try
                                {
                                    if (counter == query.Length - 1)
                                    {
                                        // Save DOCX
                                        using (FileStream wordStream = new FileStream(outputDocxPath,
                                                                                      FileMode.Create,
                                                                                      FileAccess.Write))
                                        {
                                            document.Save(wordStream, FormatType.Docx);
                                        }

                                        // Open the saved DOCX and convert to PDF
                                        using (FileStream docxStream = new FileStream(outputDocxPath,
                                                                                      FileMode.Open,
                                                                                      FileAccess.Read))
                                        using (WordDocument wordDocument = new WordDocument(docxStream, FormatType.Docx))
                                        using (DocIORenderer renderer = new DocIORenderer())
                                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                                        {
                                            using (FileStream pdfStream = new FileStream(outputPdfPath,
                                                                                         FileMode.Create,
                                                                                         FileAccess.Write))
                                            {
                                                pdfDocument.Save(pdfStream);
                                            }
                                        }
                                        string script =
                                            $"window.open('/TestReport/TestReport-{uniqueFileName}.pdf', '_blank');";

                                        ClientScript.RegisterStartupScript(
                                            this.GetType(),
                                            "OpenPDF",
                                            script,
                                            true);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    string s1 = ex.Message;
                                    Response.Write(s1);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error appropriately
                        string msg = ex.Message;
                    }
                    finally
                    {
                        if (document != null)
                        {
                            document.Close();
                            document.Dispose();   // If your version implements IDisposable
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

        }

        private void FindAndReplace(WordDocument document, string placeholder, string replacement)
        {
            document.Replace(placeholder, replacement ?? string.Empty, true, true);
        }


        private void MultipleRequirementsCreateTable1(WordDocument document, string placeholder, DataTable dt)
        {
            TextSelection selection = document.Find(placeholder, false, true);

            if (selection == null || dt == null || dt.Rows.Count == 0)
                return;

            dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

            WTextBody body = selection.GetAsOneRange().OwnerParagraph.OwnerTextBody;
            IWParagraph paragraph = selection.GetAsOneRange().OwnerParagraph;

            List<WTable> allTables = new List<WTable>();

            var requirementGroups = dt.AsEnumerable()
                                      .GroupBy(r => r.Field<string>("Requirement"));

            float[] maxWidths = new float[6];

            foreach (var reqGroup in requirementGroups)
            {
                DataTable currentDt = new DataTable();

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "Requirement" &&
                        col.ColumnName != "TestSubParameter")
                    {
                        currentDt.Columns.Add(col.ColumnName, col.DataType);
                    }
                }

                var mainGroups = reqGroup.GroupBy(r => r.Field<string>("Test Item(s)"));

                foreach (var mainGroup in mainGroups)
                {
                    if (mainGroup.Any(x => string.IsNullOrEmpty(x["TestSubParameter"].ToString())))
                    {
                        foreach (DataRow row in mainGroup.Where(r => string.IsNullOrEmpty(r["TestSubParameter"].ToString())))
                        {
                            DataRow newRow = currentDt.NewRow();

                            newRow["Test Item(s)"] = "- " + mainGroup.Key;

                            foreach (DataColumn col in currentDt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)")
                                    newRow[col.ColumnName] = row[col.ColumnName];
                            }

                            currentDt.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        DataRow header = currentDt.NewRow();
                        header["Test Item(s)"] = mainGroup.Key + " :";
                        currentDt.Rows.Add(header);

                        foreach (DataRow row in mainGroup)
                        {
                            DataRow newRow = currentDt.NewRow();

                            newRow["Test Item(s)"] = "- " + row["TestSubParameter"];

                            foreach (DataColumn col in currentDt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)" &&
                                    !string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                {
                                    newRow[col.ColumnName] = row[col.ColumnName];
                                }
                            }

                            currentDt.Rows.Add(newRow);
                        }
                    }
                }

                WTable table = body.AddTable() as WTable;

                table.ResetCells(currentDt.Rows.Count + 1, currentDt.Columns.Count);

                table.TableFormat.Borders.BorderType = BorderStyle.Single;

                // Header
                table[0, 0].AddParagraph().AppendText("Test Item(s)");

                for (int c = 1; c < currentDt.Columns.Count; c++)
                {
                    string header = (c == 4)
                        ? reqGroup.Key
                        : currentDt.Columns[c].ColumnName;

                    table[0, c].AddParagraph().AppendText(header);
                }

                table.Rows[0].IsHeader = true;

                for (int c = 0; c < currentDt.Columns.Count; c++)
                {
                    table[0, c].CellFormat.BackColor = Syncfusion.Drawing.Color.LightGray;

                    foreach (WParagraph p in table[0, c].Paragraphs)
                    {
                        foreach (WTextRange t in p.ChildEntities.OfType<WTextRange>())
                            t.CharacterFormat.Bold = true;
                    }
                }

                // Data
                for (int r = 0; r < currentDt.Rows.Count; r++)
                {
                    for (int c = 0; c < currentDt.Columns.Count; c++)
                    {
                        string text = currentDt.Rows[r][c]?.ToString() ?? "";

                        if (text.Contains("^"))
                            text = _cm.ReplaceSuperScript(text);

                        IWParagraph para = table[r + 1, c].AddParagraph();

                        WTextRange txt = para.AppendText(text) as WTextRange;

                        if (c == 0 && text.EndsWith(" :"))
                            txt.CharacterFormat.Bold = true;
                    }
                }

                table.ApplyStyle(BuiltinTableStyle.LightGrid);

                // Store max widths
                for (int c = 0; c < currentDt.Columns.Count; c++)
                {
                    maxWidths[c] = Math.Max(maxWidths[c], table.Rows[0].Cells[c].Width);
                }

                allTables.Add(table);

                body.ChildEntities.Insert(body.ChildEntities.IndexOf(paragraph) + 1, table);

                body.AddParagraph();
            }

            // Apply equal widths
            foreach (WTable table in allTables)
            {
                for (int c = 0; c < table.Rows[0].Cells.Count; c++)
                {
                    table.Rows[0].Cells[c].Width = maxWidths[c];
                }
            }

            paragraph.Text = paragraph.Text.Replace(placeholder, "");
        }


        /* without requirements (for all p/s/ss)*/
        private void FillRow(IWTable table, int rowIndex, DataRow dataRow,
                     DataColumnCollection columns, int startColumnIndex = 0)
        {
            int colIndex = startColumnIndex;

            foreach (DataColumn col in columns)
            {
                if (col.ColumnName != "Test Items(s)" &&
                    col.ColumnName != "Test SubItems" &&
                    col.ColumnName != "Test SubSubItems")
                {
                    string cellValue = dataRow[col.ColumnName]?.ToString() ?? "";

                    if (cellValue.Contains("^"))
                        cellValue = _cm.ReplaceSuperScript(cellValue);

                    WTableCell cell = (WTableCell)table.Rows[rowIndex].Cells[colIndex];
                    cell = table.Rows[rowIndex].Cells[colIndex];
                    cell.ChildEntities.Clear();
                    cell.AddParagraph().AppendText(cellValue);

                    colIndex++;
                }
            }
        }

        void CreateTable4(WordDocument document, string placeholder, DataTable dt)
        {
            TextSelection selection = document.Find(placeholder, false, true);

            if (selection == null || dt == null || dt.Rows.Count == 0)
                return;

            WTextRange textRange = selection.GetAsOneRange();
            WParagraph paragraph = textRange.OwnerParagraph;

            // Remove placeholder
            paragraph.ChildEntities.Remove(textRange);

            // Rename columns
            dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

            if (dt.Columns.Contains("Requirement"))
            {
                string requirement = dt.Rows[0]["Requirement"].ToString();

                dt.Columns.Remove("Requirement");

                if (dt.Columns.Contains("Specification"))
                    dt.Columns["Specification"].ColumnName = requirement;
            }

            // Create new table structure
            DataTable newDt = new DataTable();

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName != "TestSubParameter")
                    newDt.Columns.Add(col.ColumnName, col.DataType);
            }

            // Build grouped table
            var allGroups = dt.AsEnumerable()
                              .GroupBy(r => r.Field<string>("Test Item(s)"));

            foreach (var mainGroup in allGroups)
            {
                var subGroups = mainGroup.GroupBy(r => r.Field<string>("TestSubParameter"));

                // No sub items
                if (subGroups.Any(g => string.IsNullOrEmpty(g.Key)))
                {
                    foreach (DataRow row in mainGroup.Where(r =>
                             string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                    {
                        DataRow dr = newDt.NewRow();

                        dr["Test Item(s)"] = "- " + mainGroup.Key;

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName != "Test Item(s)" &&
                                col.ColumnName != "TestSubParameter")
                            {
                                dr[col.ColumnName] = row[col];
                            }
                        }

                        newDt.Rows.Add(dr);
                    }
                }
                else
                {
                    // Header row
                    DataRow header = newDt.NewRow();
                    header["Test Item(s)"] = mainGroup.Key + " :";
                    newDt.Rows.Add(header);

                    foreach (DataRow row in mainGroup.Where(r =>
                             !string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                    {
                        DataRow dr = newDt.NewRow();

                        dr["Test Item(s)"] = "- " + row["TestSubParameter"].ToString();

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName != "Test Item(s)" &&
                                col.ColumnName != "TestSubParameter")
                            {
                                if (!string.IsNullOrWhiteSpace(row[col].ToString()))
                                    dr[col.ColumnName] = row[col];
                            }
                        }

                        newDt.Rows.Add(dr);
                    }
                }
            }

            // Create Syncfusion table
            WTable table = new WTable(document);
            table.ResetCells(newDt.Rows.Count + 1, newDt.Columns.Count);

            paragraph.OwnerTextBody.ChildEntities.Insert(
                paragraph.OwnerTextBody.ChildEntities.IndexOf(paragraph) + 1,
                table);

            table.TableFormat.IsAutoResized = true;

            // Header row
            for (int c = 0; c < newDt.Columns.Count; c++)
            {
                WTableCell cell = table[0, c];

                IWParagraph p = cell.AddParagraph();
                IWTextRange tr = p.AppendText(newDt.Columns[c].ColumnName);

                tr.CharacterFormat.Bold = true;

                cell.CellFormat.BackColor = Syncfusion.Drawing.Color.LightGray;
            }

            // Data rows
            for (int r = 0; r < newDt.Rows.Count; r++)
            {
                for (int c = 0; c < newDt.Columns.Count; c++)
                {
                    string value = newDt.Rows[r][c]?.ToString() ?? "";

                    WTableCell cell = table[r + 1, c];

                    IWParagraph p = cell.AddParagraph();

                    IWTextRange tr = p.AppendText(value);

                    if (c == 0 && value.EndsWith(" :"))
                    {
                        tr.CharacterFormat.Bold = true;
                    }
                }
            }

            // Repeat header row
            table.Rows[0].IsHeader = true;

            // Borders
            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    cell.CellFormat.Borders.Left.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Right.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Top.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Bottom.BorderType = BorderStyle.Single;
                }
            }

            table.ApplyStyle(BuiltinTableStyle.LightGrid);
        }

        /* Without Requirement for P,S,SS */
        private void WithoutRequirementCreateTable1(WordDocument document, string placeholder, DataTable dt)
        {
            TextSelection selection = document.Find(placeholder, false, true);

            if (selection == null)
                throw new Exception($"Placeholder {placeholder} not found.");

            WTextRange textRange = selection.GetAsOneRange();
            WParagraph paragraph = textRange.OwnerParagraph;

            // Remove placeholder
            paragraph.ChildEntities.Remove(textRange);

            var groups = dt.AsEnumerable()
                           .GroupBy(r => r.Field<string>("Test Items(s)"))
                           .ToList();

            // Calculate total rows
            int totalRows = 1; // Header

            foreach (var group in groups)
            {
                totalRows += 1;

                totalRows += group.Count(r =>
                    !string.IsNullOrEmpty(r["Test SubItems"].ToString()) ||
                    !string.IsNullOrEmpty(r["Test SubSubItems"].ToString()));
            }

            int totalColumns = dt.Columns.Count - 2; // Exclude SubItems & SubSubItems
            
            WTable table = new WTable(document);
            table.ResetCells(totalRows, totalColumns);
            
            paragraph.OwnerTextBody.ChildEntities.Insert(
                paragraph.OwnerTextBody.ChildEntities.IndexOf(paragraph) + 1,
                table);

            // =======================
            // Header
            // =======================
            int colIndex = 0;

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "Test SubItems" ||
                    col.ColumnName == "Test SubSubItems")
                    continue;

                WTableCell cell = table[0, colIndex];

                cell.AddParagraph().AppendText(col.ColumnName);

                cell.CellFormat.BackColor = Syncfusion.Drawing.Color.LightGray;

                foreach (WParagraph p in cell.Paragraphs)
                {
                    foreach (ParagraphItem item in p.Items)
                    {
                        if (item is WTextRange tr)
                            tr.CharacterFormat.Bold = true;
                    }
                }

                colIndex++;
            }

            int currentRow = 1;
            System.Diagnostics.Debug.WriteLine("Current Row: " + currentRow);
            foreach (var group in groups)
            {
                string mainItem = group.Key;

                bool hasSubItems = group.Any(r =>
                    !string.IsNullOrEmpty(r["Test SubItems"].ToString()));

                bool hasSubSubItems = group.Any(r =>
                    !string.IsNullOrEmpty(r["Test SubSubItems"].ToString()));

                //------------------------------------------
                // Scenario 1
                //------------------------------------------
                if (!hasSubItems && !hasSubSubItems)
                {
                    table[currentRow, 0].AddParagraph().AppendText(mainItem);

                    FillRow(table, currentRow, group.First(), dt.Columns, 1);

                    currentRow++;
                }

                //------------------------------------------
                // Scenario 2
                //------------------------------------------
                else if (hasSubItems && !hasSubSubItems)
                {
                    IWTextRange tr = table[currentRow, 0]
                                        .AddParagraph()
                                        .AppendText(mainItem);

                    tr.CharacterFormat.Bold = true;

                    currentRow++;

                    foreach (DataRow row in group)
                    {
                        string subItem = row["Test SubItems"].ToString();

                        if (!string.IsNullOrEmpty(subItem))
                        {
                            IWParagraph p = table[currentRow, 0].AddParagraph();
                            p.ParagraphFormat.LeftIndent = 10;

                            p.AppendText("- " + subItem);

                            FillRow(table, currentRow, row, dt.Columns, 1);

                            currentRow++;
                        }
                    }
                }

                //------------------------------------------
                // Scenario 3
                //------------------------------------------
                else
                {
                    IWTextRange tr = table[currentRow, 0]
                                        .AddParagraph()
                                        .AppendText(mainItem);

                    tr.CharacterFormat.Bold = true;

                    currentRow++;

                    var subItems = group
                        .Select(r => r["Test SubItems"].ToString())
                        .Distinct();

                    foreach (string subItem in subItems)
                    {
                        IWParagraph p1 = table[currentRow, 0].AddParagraph();

                        p1.ParagraphFormat.LeftIndent = 10;
                        p1.AppendText("• " + subItem);

                        currentRow++;

                        var subSubRows = group.Where(r =>
                            r["Test SubItems"].ToString() == subItem);

                        foreach (DataRow row in subSubRows)
                        {
                            string subSub = row["Test SubSubItems"].ToString();

                            if (!string.IsNullOrEmpty(subSub))
                            {
                                IWParagraph p2 = table[currentRow, 0].AddParagraph();

                                p2.ParagraphFormat.LeftIndent = 20;
                                p2.AppendText("- " + subSub);

                                FillRow(table, currentRow, row, dt.Columns, 1);

                                currentRow++;
                            }
                        }
                    }
                }
            }

            //===========================
            // Formatting
            //===========================

            table.TableFormat.IsAutoResized = true;

            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    cell.CellFormat.Borders.BorderType =
                        Syncfusion.DocIO.DLS.BorderStyle.Single;
                }
            }
            System.Diagnostics.Debug.WriteLine("Filled Until Row : " + currentRow);
        }
        private void RequirementCreateTable1(WordDocument document, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;

            TextSelection selection = document.Find(placeholder, false, true);

            if (selection == null)
                throw new Exception($"Placeholder {placeholder} not found.");

            WTextRange textRange = selection.GetAsOneRange();
            WParagraph paragraph = textRange.OwnerParagraph;

            // Remove placeholder
            paragraph.ChildEntities.Remove(textRange);

            // Rename columns
            dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

            if (dt.Columns.Contains("Requirement"))
            {
                string requirement = dt.Rows[0]["Requirement"].ToString();

                dt.Columns.Remove("Requirement");

                if (dt.Columns.Contains("Specification"))
                    dt.Columns["Specification"].ColumnName = requirement;
            }

            DataTable newDt = new DataTable();

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName != "TestSubParameter")
                    newDt.Columns.Add(col.ColumnName, col.DataType);
            }

            var allGroups = dt.AsEnumerable()
                              .GroupBy(r => r.Field<string>("Test Item(s)"));

            foreach (var mainGroup in allGroups)
            {
                var subGroups = mainGroup.GroupBy(r => r.Field<string>("TestSubParameter"));

                if (subGroups.Any(g => string.IsNullOrEmpty(g.Key)))
                {
                    foreach (DataRow row in mainGroup.Where(r => string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                    {
                        DataRow dr = newDt.NewRow();
                        dr["Test Item(s)"] = "- " + mainGroup.Key;

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName != "Test Item(s)" &&
                                col.ColumnName != "TestSubParameter")
                            {
                                dr[col.ColumnName] = row[col];
                            }
                        }

                        newDt.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow header = newDt.NewRow();
                    header["Test Item(s)"] = mainGroup.Key + " :";
                    newDt.Rows.Add(header);

                    foreach (DataRow row in mainGroup.Where(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                    {
                        DataRow dr = newDt.NewRow();

                        dr["Test Item(s)"] = "- " + row["TestSubParameter"].ToString();

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName != "Test Item(s)" &&
                                col.ColumnName != "TestSubParameter" &&
                                !string.IsNullOrWhiteSpace(row[col].ToString()))
                            {
                                dr[col.ColumnName] = row[col];
                            }
                        }

                        newDt.Rows.Add(dr);
                    }
                }
            }

            // Create table
            WTable table = new WTable(document);
            table.ResetCells(newDt.Rows.Count + 1, newDt.Columns.Count);

            paragraph.OwnerTextBody.ChildEntities.Insert(
                paragraph.OwnerTextBody.ChildEntities.IndexOf(paragraph) + 1,
                table);

            table.TableFormat.IsAutoResized = true;

            // Header
            for (int c = 0; c < newDt.Columns.Count; c++)
            {
                WTableCell cell = table[0, c];

                cell.CellFormat.BackColor = Syncfusion.Drawing.Color.LightGray;

                IWParagraph p = cell.AddParagraph();
                IWTextRange tr = p.AppendText(newDt.Columns[c].ColumnName);
                tr.CharacterFormat.Bold = true;
            }

            // Data
            for (int r = 0; r < newDt.Rows.Count; r++)
            {
                for (int c = 0; c < newDt.Columns.Count; c++)
                {
                    string value = newDt.Rows[r][c]?.ToString() ?? "";

                    IWParagraph p = table[r + 1, c].AddParagraph();
                    IWTextRange tr = p.AppendText(value);

                    if (c == 0 && value.EndsWith(" :"))
                        tr.CharacterFormat.Bold = true;
                }
            }

            // Borders
            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    cell.CellFormat.Borders.Left.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Right.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Top.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Bottom.BorderType = BorderStyle.Single;
                }
            }

            table.ApplyStyle(BuiltinTableStyle.LightGrid);
        }


        /* for QR Code */
        private void InsertImagesIntoDocument(WordDocument document, string placeholder, List<string> imagePaths)
        {
            try
            {
                TextSelection selection = document.Find(placeholder, false, true);

                if (selection == null)
                    return;

                selection = document.Find(placeholder, false, true);
                WTextRange textRange = selection.GetAsOneRange();
                WParagraph paragraph = textRange.OwnerParagraph;

                if (selection != null)
                {

                    // Remove the placeholder text
                    textRange.Text = "";

                    // Insert image
                    byte[] imageBytes = File.ReadAllBytes(imagePaths[0]);

                    IWPicture picture = paragraph.AppendPicture(imageBytes);

                    picture.Width = 70;
                    picture.Height = 70;
                }              
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "Scripts",
                    "<script>alert('" + ex.Message.Replace("'", "\\'") + "');</script>");
            }

        }

        /* for oil (food)*/
        private void InsertImagesIntoDocument1(WordDocument document, string placeholder, List<string> imagePaths)
        {
            try
            {
                TextSelection selection = document.Find(placeholder, false, true);

                if (selection == null)
                    return;

                WTextRange textRange = selection.GetAsOneRange();
                WParagraph paragraph = textRange.OwnerParagraph;

                // Remove placeholder
                textRange.Text = "";

                if (imagePaths == null)
                    return;

                var existingImages = imagePaths
                    .Where(File.Exists)
                    .ToList();

                if (existingImages.Count == 0)
                    return;

                // Attachment heading
                paragraph.AppendText("Attachment:");
                paragraph.AppendBreak(BreakType.LineBreak);

                // Create 3-column table
                WTable table = paragraph.OwnerTextBody.AddTable() as WTable;

                int imagesPerRow = 3;
                int rows = (int)Math.Ceiling((double)existingImages.Count / imagesPerRow);

                table.ResetCells(rows, imagesPerRow);

                int imageIndex = 0;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < imagesPerRow; c++)
                    {
                        if (imageIndex >= existingImages.Count)
                            break;

                        IWParagraph cellPara = table[r, c].AddParagraph();

                        byte[] imageBytes = File.ReadAllBytes(existingImages[imageIndex]);

                        IWPicture picture = cellPara.AppendPicture(imageBytes);

                        picture.Width = 120;
                        picture.Height = 90;

                        imageIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "Scripts",
                    "<script>alert('" + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }


        private void CreateTableLastRaisins(WordDocument document, string placeholder, DataTable dt)
        {
            try
            {
                TextSelection selection = document.Find(placeholder, false, true);

                if (selection == null || dt == null || dt.Rows.Count == 0)
                    return;

                WTextRange textRange = selection.GetAsOneRange();
                WParagraph paragraph = textRange.OwnerParagraph;

                // Remove placeholder
                textRange.Text = "";

                if (dt.Columns.Contains("TestParameter"))
                    dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

                HashSet<string> requirements = new HashSet<string>();

                if (dt.Columns.Contains("Requirement"))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Requirement"] != DBNull.Value)
                            requirements.Add(row["Requirement"].ToString());
                    }
                }

                DataTable newDt = new DataTable();

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "TestSubParameter" &&
                        col.ColumnName != "Requirement")
                    {
                        newDt.Columns.Add(col.ColumnName, col.DataType);
                    }
                }

                var groups = dt.AsEnumerable()
                    .GroupBy(r => r.Field<string>("Test Item(s)"))
                    .OrderBy(g => g.Key);

                foreach (var mainGroup in groups)
                {
                    DataRow header = newDt.NewRow();
                    header["Test Item(s)"] = mainGroup.Key + ":";
                    newDt.Rows.Add(header);

                    foreach (var subGroup in mainGroup.GroupBy(r => r.Field<string>("TestSubParameter"))
                                                      .Where(g => !string.IsNullOrEmpty(g.Key)))
                    {
                        var first = subGroup.First();

                        DataRow merged = newDt.NewRow();
                        merged["Test Item(s)"] = "- " + subGroup.Key;

                        foreach (DataColumn col in newDt.Columns)
                        {
                            if (col.ColumnName != "Test Item(s)" &&
                                col.ColumnName != "Unit(s)" &&
                                col.ColumnName != "Result(s)")
                            {
                                merged[col.ColumnName] = first[col.ColumnName];
                            }
                        }

                        bool firstRow = true;

                        foreach (DataRow row in subGroup)
                        {
                            if (firstRow)
                            {
                                merged["Unit(s)"] = row["Unit(s)"];
                                merged["Result(s)"] = row["Result(s)"];
                                newDt.Rows.Add(merged);
                                firstRow = false;
                            }
                            else
                            {
                                DataRow extra = newDt.NewRow();
                                extra["Unit(s)"] = row["Unit(s)"];
                                extra["Result(s)"] = row["Result(s)"];
                                newDt.Rows.Add(extra);
                            }
                        }
                    }
                }

                WTable table = paragraph.OwnerTextBody.AddTable() as WTable;
                table.ResetCells(newDt.Rows.Count + 1, newDt.Columns.Count);

                // Header
                for (int c = 0; c < newDt.Columns.Count; c++)
                {
                    WTableCell cell = table[0, c];
                    IWParagraph p = cell.AddParagraph();

                    WTextRange tr = p.AppendText(newDt.Columns[c].ColumnName) as WTextRange;
                    tr.CharacterFormat.Bold = true;
                }

                // Data
                for (int r = 0; r < newDt.Rows.Count; r++)
                {
                    for (int c = 0; c < newDt.Columns.Count; c++)
                    {
                        string value = newDt.Rows[r][c]?.ToString() ?? "";

                        WTableCell cell = table[r + 1, c];
                        IWParagraph p = cell.AddParagraph();

                        WTextRange tr = p.AppendText(value) as WTextRange;

                        if (c == 0 && !value.StartsWith("-"))
                            tr.CharacterFormat.Bold = true;
                    }
                }

                // Replace {Requirement}
                if (requirements.Count > 0)
                {
                    TextSelection reqSel = document.Find("{Requirement}", false, true);

                    if (reqSel != null)
                    {
                        WTextRange reqRange = reqSel.GetAsOneRange();
                        reqRange.Text = string.Join(Environment.NewLine, requirements);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void RequirementCreateTable2(WordDocument document, string placeholder, DataTable dt)
        {
            try
            {
                TextSelection selection = document.Find(placeholder, false, true);

                if (selection == null || dt == null || dt.Rows.Count == 0)
                    return;

                WTextRange textRange = selection.GetAsOneRange();
                WParagraph paragraph = textRange.OwnerParagraph;

                // Remove placeholder
                textRange.Text = "";

                var groups = dt.AsEnumerable()
                               .GroupBy(r => r.Field<string>("TestParameter"));

                int totalRows = 1;

                foreach (var group in groups)
                {
                    if (group.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                        totalRows += group.Count() + 1;
                    else
                        totalRows++;
                }

                int numColumns = dt.Columns.Count - 3;

                WTable table = paragraph.OwnerTextBody.AddTable() as WTable;
                table.ResetCells(totalRows, numColumns);

                //-----------------------------------------
                // Header Row
                //-----------------------------------------

                string requirementHeader = dt.Rows[0]["Requirement"].ToString();

                int colIndex = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == "TestSubParameter" ||
                        col.ColumnName == "RequirementID" ||
                        col.ColumnName == "Requirement")
                        continue;

                    string header = col.ColumnName;

                    if (col.ColumnName == "Specification")
                        header = requirementHeader;

                    IWParagraph p = table[0, colIndex].AddParagraph();
                    WTextRange tr = p.AppendText(header) as WTextRange;
                    tr.CharacterFormat.Bold = true;

                    colIndex++;
                }

                //-----------------------------------------
                // Data Rows
                //-----------------------------------------

                int currentRow = 1;

                foreach (var group in groups)
                {
                    string mainItem = group.Key;

                    bool hasSubItems = group.Any(r =>
                        !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));

                    if (!hasSubItems)
                    {
                        DataRow firstRow = group.First();

                        table[currentRow, 0].AddParagraph().AppendText(mainItem);

                        colIndex = 1;

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName == "TestSubParameter" ||
                                col.ColumnName == "RequirementID" ||
                                col.ColumnName == "Requirement")
                                continue;

                            if (col.ColumnName == "TestParameter")
                                continue;

                            string value = firstRow[col] == DBNull.Value
                                ? ""
                                : firstRow[col].ToString();

                            if (value.Contains("^"))
                                value = _cm.ReplaceSuperScript(value);

                            table[currentRow, colIndex]
                                .AddParagraph()
                                .AppendText(value);

                            colIndex++;
                        }

                        currentRow++;
                    }
                    else
                    {
                        //-------------------------------------
                        // Main Group
                        //-------------------------------------

                        WTextRange title =
                            table[currentRow, 0]
                            .AddParagraph()
                            .AppendText(mainItem + " :") as WTextRange;

                        title.CharacterFormat.Bold = true;

                        currentRow++;

                        //-------------------------------------
                        // Sub Items
                        //-------------------------------------

                        foreach (DataRow row in group)
                        {
                            IWParagraph para =
                                table[currentRow, 0].AddParagraph();

                            para.ParagraphFormat.LeftIndent = 10;

                            para.AppendText("- " + row["TestSubParameter"]);

                            colIndex = 1;

                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName == "TestParameter" ||
                                    col.ColumnName == "TestSubParameter" ||
                                    col.ColumnName == "RequirementID" ||
                                    col.ColumnName == "Requirement")
                                    continue;

                                string value = row[col] == DBNull.Value
                                    ? ""
                                    : row[col].ToString();

                                if (value.Contains("^"))
                                    value = _cm.ReplaceSuperScript(value);

                                table[currentRow, colIndex]
                                    .AddParagraph()
                                    .AppendText(value);

                                colIndex++;
                            }

                            currentRow++;
                        }
                    }
                }

                //-----------------------------------------
                // Formatting
                //-----------------------------------------

                table.TableFormat.IsAutoResized = true;

                foreach (WTableRow row in table.Rows)
                {
                    foreach (WTableCell cell in row.Cells)
                    {
                        cell.CellFormat.Borders.BorderType = BorderStyle.Single;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /* Static Table data */
        public void CreateNutritionalTable(WordDocument document, string placeholder)
        {
            TextSelection selection = document.Find(placeholder, false, true);

            if (selection == null)
                return;

            WTextRange textRange = selection.GetAsOneRange();
            WParagraph paragraph = textRange.OwnerParagraph;

            // Remove placeholder
            textRange.Text = "";

            // Create table
            WTable table = paragraph.OwnerTextBody.AddTable() as WTable;
            table.ResetCells(10, 2);

            //-----------------------------------------
            // Header Row
            //-----------------------------------------

            table.ApplyStyle(BuiltinTableStyle.LightGrid);

            // Merge first row
            table[0, 0].CellFormat.HorizontalMerge = Syncfusion.DocIO.DLS.CellMerge.Start;
            table[0, 1].CellFormat.HorizontalMerge = Syncfusion.DocIO.DLS.CellMerge.Continue;

            IWParagraph p = table[0, 0].AddParagraph();

            WTextRange txt = p.AppendText("INFORMASI NILAI GIZI") as WTextRange;
            txt.CharacterFormat.Bold = true;
            txt.CharacterFormat.FontSize = 14;

            p.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

            //-----------------------------------------
            // Row 2
            //-----------------------------------------

            table[1, 0].AddParagraph().AppendText("JUMLAH Per 100 mL");

            IWParagraph qty = table[1, 1].AddParagraph();
            qty.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;

            //-----------------------------------------
            // Data
            //-----------------------------------------

            string[,] data =
            {
        { "Energi Total", "900 kkal" },
        { "Lemak Total", "100 g" },
        { "Lemak Jenuh", "42 g" },
        { "Protein", "0 g" },
        { "Karbohidrat", "0 g" },
        { "Gula", "0 g" },
        { "Garam (Natrium)", "0 mg" },
        { "Vitamin A", "2906 mcg" }
    };

            for (int i = 0; i < data.GetLength(0); i++)
            {
                table[i + 2, 0]
                    .AddParagraph()
                    .AppendText(data[i, 0]);

                IWParagraph value = table[i + 2, 1].AddParagraph();
                value.AppendText(data[i, 1]);
                value.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            }

            //-----------------------------------------
            // Borders
            //-----------------------------------------

            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    cell.CellFormat.Borders.Left.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Right.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Top.BorderType = BorderStyle.Single;
                    cell.CellFormat.Borders.Bottom.BorderType = BorderStyle.Single;
                }
            }

            // Remove inside horizontal borders
            for (int r = 2; r < table.Rows.Count; r++)
            {
                table.Rows[r].Cells[0].CellFormat.Borders.Bottom.BorderType = BorderStyle.None;
                table.Rows[r].Cells[1].CellFormat.Borders.Bottom.BorderType = BorderStyle.None;
            }
        }

        // ========== GENERATE QR CODE ==========
        protected void GenerateQR(string SampleID)
        {
            try
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string QRCodeFilePath = baseUrl + "/TestReport/TestReport-" + SampleID + ".pdf";
                string QRCodeFileName = SampleID + ".png";

                string barcodeFolderPath = Server.MapPath("~/TestReport/");
                if (!Directory.Exists(barcodeFolderPath))
                {
                    Directory.CreateDirectory(barcodeFolderPath);
                }

                BarcodeWriter barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;

                var options = new ZXing.Common.EncodingOptions
                {
                    Width = 3000,
                    Height = 3000,
                    Margin = 2
                };
                barcodeWriter.Options = options;

                Bitmap barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);

                string barcodeFilePath = Path.Combine(barcodeFolderPath, QRCodeFileName);
                barcodeBitmap.Save(barcodeFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error generating QR: " + ex.Message);
            }
        }



        /* placeholder replace for header and footer all pages |Start| */

        private void ReplaceInHeaderFooter(WordDocument document, string placeholder, string replacement)
        {
            foreach (WSection section in document.Sections)
            {
                ReplaceInBody(section.HeadersFooters.Header, placeholder, replacement);
                ReplaceInBody(section.HeadersFooters.Footer, placeholder, replacement);

                ReplaceInBody(section.HeadersFooters.FirstPageHeader, placeholder, replacement);
                ReplaceInBody(section.HeadersFooters.FirstPageFooter, placeholder, replacement);

                ReplaceInBody(section.HeadersFooters.EvenHeader, placeholder, replacement);
                ReplaceInBody(section.HeadersFooters.EvenFooter, placeholder, replacement);
            }
        }
        private void ReplaceInBody(WTextBody body, string placeholder, string replacement)
        {
            for (int p = 0; p < body.ChildEntities.Count; p++)
            {
                if (body.ChildEntities[p] is WParagraph para)
                {
                    for (int i = 0; i < para.ChildEntities.Count; i++)
                    {
                        if (para.ChildEntities[i] is WTextRange text)
                        {
                            if (!string.IsNullOrEmpty(text.Text) && text.Text.Contains(placeholder))
                            {
                                text.Text = text.Text.Replace(placeholder, replacement);
                            }
                        }
                    }
                }
            }
        }
        private void ReplaceInTextBody(WTextBody body, string placeholder, string replacement)
        {
            foreach (WParagraph paragraph in body.Paragraphs)
            {
                foreach (ParagraphItem item in paragraph.ChildEntities)
                {
                    if (item is WTextRange textRange)
                    {
                        textRange.Text = textRange.Text.Replace(placeholder, replacement);
                    }
                }
            }
        }
        private void FindAndReplaceInRange(Microsoft.Office.Interop.Word.Range range, string placeholder, string replacement)
        {
            Microsoft.Office.Interop.Word.Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;
            find.Replacement.ClearFormatting();
            find.Replacement.Text = replacement;
            find.Execute(Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
        }
        private void ReplaceInHeader(Microsoft.Office.Interop.Word.HeaderFooter header, string placeholder, string imagePath)
        {
            if (header == null) return;
            Microsoft.Office.Interop.Word.Range range = header.Range;

            // Find placeholder
            Microsoft.Office.Interop.Word.Find find = range.Find;
            find.Text = placeholder;
            find.Replacement.Text = "";

            if (find.Execute())
            {
                // Delete placeholder text
                range.Text = "";

                // Insert image at the placeholder position
                range.InlineShapes.AddPicture(
                    FileName: imagePath,
                    LinkToFile: false,
                    SaveWithDocument: true
                );
            }
        }
        public void ReplaceHeaderPlaceholderWithImage(string filePath, string placeholder, string imagePath)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
                wordApp.Visible = false;
                doc = wordApp.Documents.Open(filePath);

                foreach (Microsoft.Office.Interop.Word.Section section in doc.Sections)
                {
                    // Check all 3 header types (some templates use only one)
                    //ReplaceInHeader(section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary],
                    //placeholder, imagePath);

                    //ReplaceInHeader(section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage],
                    //placeholder, imagePath);

                    //ReplaceInHeader(section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterEvenPages],
                    //                placeholder, imagePath);

                    foreach (Microsoft.Office.Interop.Word.HeaderFooter headerFooter in section.Headers)
                    {
                        ReplaceInHeader(headerFooter, placeholder, imagePath);
                    }

                }

                doc.Save();
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();
                    wordApp.Quit();
                }
            }
        }
    }
}
