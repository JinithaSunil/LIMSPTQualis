using Microsoft.Office.Interop.Word;
using System;
using System.Runtime.InteropServices;
using DataTable = System.Data.DataTable;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace LIMS
{
    public partial class ChemicalReports : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            Application wordApp = new Application();
            wordApp.Visible = false;
            Microsoft.Office.Interop.Word.Document doc = null;
            try
            {
                // Get sampleID from query string and convert it to int
                int sampleID = 0;
                if (Request.QueryString["sampleID"] != null && int.TryParse(Request.QueryString["sampleID"], out sampleID))
                {
                    // Get data from database
                    DataTable ChemicalDetails = _cm.getChemicalDetails(sampleID);

                    // Open the Word template
                    string path = Server.MapPath("~/Inputfiles/");
                    string templatePath = path + @"A01-Test Report-EN.docx";
                    doc = wordApp.Documents.Add(templatePath);

                    if (ChemicalDetails != null && ChemicalDetails.Rows.Count > 0)
                    {
                        FindAndReplace(wordApp, "{TestReportNo.}", ChemicalDetails.Rows[0]["TestReportNo"].ToString());
                        FindAndReplace(wordApp, "{Date}", ChemicalDetails.Rows[0]["Date"].ToString());
                        FindAndReplace(wordApp, "{Applicant}", ChemicalDetails.Rows[0]["Applicant"].ToString());
                        FindAndReplace(wordApp, "{Attention}", ChemicalDetails.Rows[0]["Attention"].ToString());
                        FindAndReplace(wordApp, "{Email}", ChemicalDetails.Rows[0]["Email"].ToString());
                        FindAndReplace(wordApp, "{TestSample}", ChemicalDetails.Rows[0]["Test Sample"].ToString());
                        FindAndReplace(wordApp, "{TestPeriod}", ChemicalDetails.Rows[0]["Test Period"].ToString());
                        FindAndReplace(wordApp, "{SampleDescription}", ChemicalDetails.Rows[0]["Sample Description"].ToString());
                        FindAndReplace(wordApp, "{ColorName}", ChemicalDetails.Rows[0]["Color Name/Code"].ToString());
                        FindAndReplace(wordApp, "{Buyer'sName}", ChemicalDetails.Rows[0]["Buyer's Name"].ToString());
                        FindAndReplace(wordApp, "{AgeGroup}", ChemicalDetails.Rows[0]["Age Group"].ToString());
                        FindAndReplace(wordApp, "{MaterialName}", ChemicalDetails.Rows[0]["Material Name/Code"].ToString());
                        FindAndReplace(wordApp, "{P.O.Order}", ChemicalDetails.Rows[0]["PO No."].ToString());
                        FindAndReplace(wordApp, "{ArticleNumber}", ChemicalDetails.Rows[0]["ArticleNumber"].ToString());
                        FindAndReplace(wordApp, "{LotNo.}", ChemicalDetails.Rows[0]["Lot No."].ToString());
                        FindAndReplace(wordApp, "{SupplierName}", ChemicalDetails.Rows[0]["Suppliername"].ToString());
                        FindAndReplace(wordApp, "{CountryOfOrigin}", ChemicalDetails.Rows[0]["Country of Origin"].ToString());
                        FindAndReplace(wordApp, "{CountryOfDestination}", ChemicalDetails.Rows[0]["Country of Destination"].ToString());
                        FindAndReplace(wordApp, "{TestSpecification}", ChemicalDetails.Rows[0]["Test Specification"].ToString());
                        FindAndReplace(wordApp, "{TestPackage}", ChemicalDetails.Rows[0]["Test Package"].ToString());
                        FindAndReplace(wordApp, "{Sample1}", ChemicalDetails.Rows[0]["Test Package"].ToString());
                    }




                    // Handle additional logic for tables
                    string s = "";
                    if (Request.QueryString["s"] != null) s = Request.QueryString["s"].ToString();
                 

                    DataTable Chemical1 = _cm.GetChemicalResultSummaryTable(s);
                    DataTable Chemical2 = _cm.GetChemicalResultTable1(s);
                    DataTable Chemical3 = _cm.GetChemicalResultTable2(s);

                    // Create tables
                    Chem1(wordApp, doc, "{ResultSummaryTable}", Chemical1);
                    Chemresulttable1(wordApp, doc, "{ResultTable}", Chemical2);
                    Chemresulttable2(wordApp, doc, "{ResultTable2}", Chemical3);

                    // Save the document
                    string pathout = Server.MapPath("~/TestReport/");
                    string outputPath = pathout + @"A01_2.docx";
                    doc.SaveAs2(outputPath);
                }
            }
            catch (Exception ex)
            {
                // Log error
                string errorMessage = $"Error in Page_Load: {ex.Message}\nStack Trace: {ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                throw;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }
                if (wordApp != null)
                {
                    wordApp.Quit();
                    Marshal.ReleaseComObject(wordApp);
                }
            }
        }

        static void FindAndReplace(Application wordApp, string findText, string replaceText)
        {
            wordApp.Selection.Find.ClearFormatting();
            wordApp.Selection.Find.Text = findText;
            wordApp.Selection.Find.Replacement.ClearFormatting();
            wordApp.Selection.Find.Replacement.Text = replaceText;
            wordApp.Selection.Find.Execute(Replace: WdReplace.wdReplaceAll);
        }
        private void Chem1(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                // Create a table in Word based on the DataTable
                Table table = doc.Tables.Add(range, dt.Rows.Count + 1, dt.Columns.Count);

                // Set overall table properties
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;

                // Set preferred width for the entire table (100% of page width)
                table.PreferredWidth = 100;
                table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Set column headers
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = dt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                    // Set equal width for each column
                    float columnWidth = 100f / dt.Columns.Count;
                    table.Columns[col + 1].PreferredWidth = columnWidth;
                    table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                }

                // Fill the table with data
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        string cellText = dt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;
                    }
                }

                // Set table formatting
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

               

                // Ensure text wrapping is enabled
                table.AllowPageBreaks = true;

                // Optional: Set minimum row height if needed
                foreach (Row row in table.Rows)
                {
                    row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                    row.Height = 20; // Minimum height in points
                }
            }
        }
        private void Chemresulttable1(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // First, handle the placeholders if there's data
            if (dt.Rows.Count > 0)
            {
                // Replace placeholders with values from the first row
                if (dt.Columns.Contains("Test Parameter"))
                    ReplaceTextInDocument(doc, "{TestParameter}", dt.Rows[0]["Test Parameter"].ToString());
                if (dt.Columns.Contains("Test Method"))
                    ReplaceTextInDocument(doc, "{Testmethod}", dt.Rows[0]["Test Method"].ToString());
                if (dt.Columns.Contains("Equipment"))
                    ReplaceTextInDocument(doc, "{Equipment}", dt.Rows[0]["Equipment"].ToString());
            }

            // Create new DataTable without the extracted columns
            DataTable modifiedDt = new DataTable();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName != "Test Parameter" &&
                    col.ColumnName != "Test Method" &&
                    col.ColumnName != "Equipment")
                {
                    modifiedDt.Columns.Add(col.ColumnName);
                }
            }

            // Copy the data to new DataTable
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = modifiedDt.NewRow();
                foreach (DataColumn col in modifiedDt.Columns)
                {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                modifiedDt.Rows.Add(newRow);
            }

            // Find the placeholder for the table
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found)
            {
                // Create table with modified DataTable
                Table table = doc.Tables.Add(range, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                // Set overall table properties
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;
                table.PreferredWidth = 100;
                table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Set column headers
                for (int col = 0; col < modifiedDt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                    float columnWidth = 100f / modifiedDt.Columns.Count;
                    table.Columns[col + 1].PreferredWidth = columnWidth;
                    table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                }

                // Fill the table with data
                for (int row = 0; row < modifiedDt.Rows.Count; row++)
                {
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;
                    }
                }

                // Set table formatting
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.AllowPageBreaks = true;

                foreach (Row row in table.Rows)
                {
                    row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                    row.Height = 20;
                }
            }
        } 
        private void Chemresulttable2(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            // First, handle the placeholders if there's data
            if (dt.Rows.Count > 0)
            {
                // Replace placeholders with values from the first row
                if (dt.Columns.Contains("Test Parameter"))
                    ReplaceTextInDocument(doc, "{TestParameter2}", dt.Rows[0]["Test Parameter"].ToString());
                if (dt.Columns.Contains("Test Method"))
                    ReplaceTextInDocument(doc, "{Testmethod2}", dt.Rows[0]["Test Method"].ToString());
                if (dt.Columns.Contains("Equipment"))
                    ReplaceTextInDocument(doc, "{Equipment2}", dt.Rows[0]["Equipment"].ToString());
            }

            // Create new DataTable without the extracted columns
            DataTable modifiedDt = new DataTable();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName != "Test Parameter" &&
                    col.ColumnName != "Test Method" &&
                    col.ColumnName != "Equipment")
                {
                    modifiedDt.Columns.Add(col.ColumnName);
                }
            }

            // Copy the data to new DataTable
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = modifiedDt.NewRow();
                foreach (DataColumn col in modifiedDt.Columns)
                {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                modifiedDt.Rows.Add(newRow);
            }

            // Find the placeholder for the table
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found)
            {
                // Create table with modified DataTable
                Table table = doc.Tables.Add(range, modifiedDt.Rows.Count + 1, modifiedDt.Columns.Count);

                // Set overall table properties
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;
                table.PreferredWidth = 100;
                table.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;

                // Set column headers
                for (int col = 0; col < modifiedDt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = modifiedDt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;

                    float columnWidth = 100f / modifiedDt.Columns.Count;
                    table.Columns[col + 1].PreferredWidth = columnWidth;
                    table.Columns[col + 1].PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                }

                // Fill the table with data
                for (int row = 0; row < modifiedDt.Rows.Count; row++)
                {
                    for (int col = 0; col < modifiedDt.Columns.Count; col++)
                    {
                        string cellText = modifiedDt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;
                    }
                }

                // Set table formatting
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.AllowPageBreaks = true;

                foreach (Row row in table.Rows)
                {
                    row.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
                    row.Height = 20;
                }
            }
        }
        private void ReplaceTextInDocument(Document doc, string findText, string replaceText)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = findText;
            range.Find.Replacement.Text = replaceText;
            range.Find.Execute(Replace: WdReplace.wdReplaceAll);
        }
    }
}