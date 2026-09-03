using LIMS.DAL;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ZXing;
using DataTable = System.Data.DataTable;
using Table = Microsoft.Office.Interop.Word.Table;

namespace LIMS
{
    public partial class CreateReportFromTemplate : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            Application wordApp = new Application();
            Microsoft.Office.Interop.Word.Document doc = null;

            try
            {
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

                int ID = Convert.ToInt32(s);
                DataTable dt = _cm.getSampleUID(ID);
                if (dt.Rows.Count > 0)
                {
                    GenerateQR(dt.Rows[0]["SampleID"].ToString());
                }

                // Get common data for TestReport from database//
                DataTable foodDetails = _cm.getFoodDetails(sampleID);

                string templatePath = "";
                string ReportName = "";

                DataTable dt1 = _cm.getReportQuery(c);
                templatePath = path + dt1.Rows[0]["Template"].ToString();
                doc = wordApp.Documents.Add(templatePath);

                DataTable dt2 = new DataTable();

                if (foodDetails != null && foodDetails.Rows.Count > 0)
                {
                    ReportName = "TestReport-" + foodDetails.Rows[0]["SID"].ToString();
                    // Replace placeholders with database values
                    FindAndReplace(wordApp, "{TestReportNo}", foodDetails.Rows[0]["testreportno"].ToString());
                    FindAndReplace(wordApp, "{CustomerName}", foodDetails.Rows[0]["CustomerName"].ToString());
                    FindAndReplace(wordApp, "{Address}", foodDetails.Rows[0]["Address"].ToString());
                    FindAndReplace(wordApp, "{SampleDescription}", foodDetails.Rows[0]["SampleDescription"].ToString());
                    FindAndReplace(wordApp, "{SampleNo}", foodDetails.Rows[0]["SampleNo"].ToString());
                    FindAndReplace(wordApp, "{SampleCondition}", foodDetails.Rows[0]["SampleCondition"].ToString());
                    FindAndReplace(wordApp, "{IssueDate}", Convert.ToDateTime(foodDetails.Rows[0]["issuedate"]).ToString("yyyy MMM dd"));
                    FindAndReplace(wordApp, "{ReceivedDate}", Convert.ToDateTime(foodDetails.Rows[0]["Receiveddate"]).ToString("yyyy MMM dd"));
                    FindAndReplace(wordApp, "{AnalysisCompletionDate}", Convert.ToDateTime(foodDetails.Rows[0]["AnalysisCompletionDate"]).ToString("yyyy MMM dd"));
                    FindAndReplace(wordApp, "{Buyer}", foodDetails.Rows[0]["Buyer"].ToString());
                    FindAndReplace(wordApp, "{ServiceConfirmationNo}", foodDetails.Rows[0]["Service Confirmation No."].ToString());
                    FindAndReplace(wordApp, "{BatchNo}", foodDetails.Rows[0]["BatchNo"].ToString());
                    FindAndReplace(wordApp, "{SampleCode}", foodDetails.Rows[0]["BatchNo"].ToString());

                    ReplaceBookmarkText(doc, "TestReportNo", foodDetails.Rows[0]["SampleDescription"].ToString());

                    //foreach (Microsoft.Office.Interop.Word.Section section in doc.Sections)
                    //{

                    //    doc.TrackRevisions = false;
                    //    //Get all Footers
                    //    Microsoft.Office.Interop.Word.HeadersFooters footers = section.Footers;
                    //    FindAndReplaceFooter(wordApp, "{TestReportNo}", footers, foodDetails.Rows[0]["testreportno"].ToString());
                    //}
                    ViewState["SampleDescription"] = foodDetails.Rows[0]["SampleDescription"].ToString();
                }




                if (ViewState["SampleDescription"] != null)
                {
                    string reportNameVS = ViewState["SampleDescription"].ToString();
                    //string ReportName = Regex.Replace(reportNameVS, @"\s*\(.*\)", "").Trim();

                    // Generate unique file name based on current date and time
                    string uniqueFileName = ReportName + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                    string outputDocxPath = pathout + ReportName + ".docx";
                    string outputPdfPath = pathout + ReportName + ".pdf";

                    int counter = 0;



                    if (dt1.Rows.Count > 0)
                    {
                        string[] query = dt1.Rows[0]["query"].ToString().Split('|');
                        string[] table = dt1.Rows[0]["docTableName"].ToString().Split('|');
                        string imgPath = "~/TestReport/" + dt.Rows[0]["SampleID"].ToString() + ".jpg";
                        InsertQR(wordApp, doc, "{QR}", imgPath);

                        for (int i = 0; i < query.Length; i++)
                        {
                            counter = i;
                            if (table[i] == "1")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);

                                createtable1(wordApp, doc, "{ResultTable1}", dt2);
                            }
                            if (table[i] == "2")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable2(wordApp, doc, "{ResultTable1}", dt2);
                            }
                            if (table[i] == "3")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable3(wordApp, doc, "{ResultTable1}", dt2);
                            }
                            if (table[i] == "4")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTable4(wordApp, doc, "{ResultTable1}", dt2);
                            }
                            if (table[i] == "5")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                RequirementCreateTable1(wordApp, doc, "{ResultTable1}", dt2);
                            }
                            if (table[i] == "6")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                WithoutRequirementCreateTable1(wordApp, doc, "{ResultTable2}", dt2);
                            }
                            if (table[i] == "7")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                if (dt2.Rows.Count > 0)
                                {
                                    List<string> list = dt2.AsEnumerable()
                                                       .Select(r => r.Field<string>("Filename"))
                                                       .ToList();
                                    InsertImagesIntoDocument(wordApp, doc, "{ images }", list);
                                }
                            }
                            if (table[i] == "8")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                CreateTableLastRaisins(wordApp, doc, "{ResultTable2}", dt2);
                            }
                            if (table[i] == "9")
                            {
                                dt2 = _cm.getReportQuery(1, query[i] + s);
                                RequirementCreateTable2(wordApp, doc, "{ResultTable3}", dt2);
                            }
                        }

                        try
                        {
                            if (counter == query.Length - 1)
                            {
                                doc.SaveAs2(outputDocxPath);
                                doc.SaveAs2(outputPdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                                Response.AddHeader("content-disposition", "inline;filename=" + outputPdfPath);
                                string script = "<script>window.open('/TestReport/" + ReportName + ".pdf" + "', '_blank');</script>";
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenPDF", script);
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
                if (doc != null)
                {
                    doc.Close();

                    Marshal.ReleaseComObject(doc);
                }
                wordApp.Quit();
                Marshal.ReleaseComObject(wordApp);
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

        private void createtable(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        range.Text = "";

                        // Group rows by Test Parameter
                        var groups = dt.AsEnumerable()
                            .GroupBy(row => row.Field<string>("Test Items(s)"));

                        // Count total rows needed (header + main categories + subitems)
                        int totalRows = 1; // Header row
                        foreach (var group in groups)
                        {
                            totalRows += 1; // Main category row
                            totalRows += group.Count(); // Subitem rows
                        }

                        // Create table with correct number of columns (one less than DataTable)
                        int numColumns = dt.Columns.Count - 1; // Remove one column
                        Table table = doc.Tables.Add(range, totalRows, numColumns);
                        table.Borders.Enable = 1;

                        // Add column headers dynamically from DataTable, shifting Method(s) and subsequent columns left
                        int headerRow = 1;
                        int colIndex = 1;

                        // Add first column header (Test Items(s))
                        table.Cell(headerRow, colIndex).Range.Text = dt.Columns[0].ColumnName; // "Test Items(s)"
                        table.Cell(headerRow, colIndex).Range.Bold = 1;
                        table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        colIndex++;

                        // Skip "Test SubItems" column and start from Method(s)
                        for (int i = 2; i < dt.Columns.Count; i++)
                        {
                            table.Cell(headerRow, colIndex).Range.Text = dt.Columns[i].ColumnName;
                            table.Cell(headerRow, colIndex).Range.Bold = 1;
                            table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            colIndex++;
                        }

                        // Fill data
                        int currentRow = 2;
                        foreach (var group in groups)
                        {
                            string mainItem = group.Key;

                            // Add main category row
                            table.Cell(currentRow, 1).Range.Text = mainItem + " :";
                            table.Cell(currentRow, 1).Range.Bold = 1;

                            // Make other cells in the main category row empty
                            for (int col = 2; col <= numColumns; col++)
                            {
                                table.Cell(currentRow, col).Range.Text = "";
                            }
                            currentRow++;

                            // Add subitem rows
                            foreach (DataRow row in group)
                            {
                                // First column: subitem name with indent
                                table.Cell(currentRow, 1).Range.Text = "- " + row["Test SubItems"].ToString();
                                table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;

                                // Fill remaining columns, skipping "Test SubItems" in the data mapping
                                colIndex = 2;
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (col.ColumnName != "Test Items(s)" &&
                                        col.ColumnName != "Test SubItems")
                                    {
                                        string s = row[col.ColumnName].ToString();
                                        s = _cm.ReplaceSuperScript(s);
                                        table.Cell(currentRow, colIndex).Range.Text = s;
                                        colIndex++;
                                    }
                                }
                                currentRow++;
                            }
                        }

                        // Format table
                        table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                        table.Rows[1].HeadingFormat = -1;

                        // Add borders
                        table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void createtable1(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            Table table = null;
            Range range = null;

            try
            {
                // Get columns, excluding TestParameter and TestSubParameter (since we'll merge it)
                var tableColumns = dt.Columns.Cast<DataColumn>()
                    .Where(col => col.ColumnName != "TestParameter" && col.ColumnName != "TestSubParameter")
                    .Select(col => col.ColumnName)
                    .ToArray();

                var uniqueSpecs = dt.AsEnumerable()
                    .Select(row => row.Field<string>("TestParameter"))
                    .Distinct()
                    .ToList();

                range = doc.Content;
                if (range == null) throw new Exception("Could not get document content range");

                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Forward = true;

                if (range.Find.Execute())
                {
                    Range insertionPoint = range.Duplicate;

                    foreach (string specValue in uniqueSpecs)
                    {
                        if (string.IsNullOrEmpty(specValue)) continue;

                        try
                        {
                            var filteredRows = dt.AsEnumerable()
                                .Where(row => row.Field<string>("TestParameter") == specValue)
                                .CopyToDataTable();

                            if (filteredRows.Rows.Count > 0)
                            {
                                if (insertionPoint != range.Duplicate)
                                {
                                    insertionPoint.InsertParagraphAfter();
                                    insertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
                                    insertionPoint.InsertParagraphAfter();
                                    insertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
                                }

                                // Create table with merged columns
                                table = doc.Tables.Add(insertionPoint, filteredRows.Rows.Count + 1, tableColumns.Length);
                                table.Borders.Enable = 1;

                                // Add headers
                                int colIndex = 1;
                                foreach (string columnName in tableColumns)
                                {
                                    string headerText = columnName == "Specification" ?
                                        "Requirement " + specValue : columnName;

                                    table.Cell(1, colIndex).Range.Text = headerText;
                                    table.Cell(1, colIndex).Range.Bold = 1;
                                    table.Cell(1, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                                    colIndex++;
                                }

                                // Group by TestSubParameter
                                var groupedRows = filteredRows.AsEnumerable()
                                    .GroupBy(row => row.Field<string>("TestSubParameter"))
                                    .ToList();

                                int currentRow = 2;
                                foreach (var group in groupedRows)
                                {
                                    string parameterName = group.Key + " :";

                                    // Add parameter header row
                                    table.Cell(currentRow, 1).Range.Text = parameterName;
                                    table.Cell(currentRow, 1).Range.Bold = 1;
                                    currentRow++;

                                    // Add detail rows
                                    foreach (var row in group)
                                    {
                                        int dataColIndex = 1;

                                        // For Test Items column, combine with hyphen
                                        table.Cell(currentRow, dataColIndex).Range.Text =
                                            "- " + row["Test Items(s)"].ToString();
                                        table.Cell(currentRow, dataColIndex).Range.ParagraphFormat.LeftIndent = 10;
                                        dataColIndex++;

                                        // Fill remaining columns
                                        foreach (string columnName in tableColumns.Skip(1))
                                        {
                                            table.Cell(currentRow, dataColIndex).Range.Text =
                                                row[columnName].ToString();
                                            table.Cell(currentRow, dataColIndex).Range.ParagraphFormat.Alignment =
                                                WdParagraphAlignment.wdAlignParagraphCenter;
                                            dataColIndex++;
                                        }
                                        currentRow++;
                                    }
                                }

                                // Format table
                                if (table != null)
                                {
                                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                                    // Adjust column widths for new format
                                    float[] columnWidths = new float[tableColumns.Length];

                                    for (int i = 0; i < tableColumns.Length; i++)
                                    {
                                        if (i == 0) // Test Items column
                                            columnWidths[i] = 5; // Wider for combined content
                                        else if (tableColumns[i].Contains("Method"))
                                            columnWidths[i] = 4;
                                        else if (tableColumns[i].Contains("Unit") ||
                                                 tableColumns[i].Contains("LOD") ||
                                                 tableColumns[i].Contains("Result"))
                                            columnWidths[i] = 2;
                                        else
                                            columnWidths[i] = 3;
                                    }

                                    for (int i = 0; i < columnWidths.Length; i++)
                                    {
                                        table.Columns[i + 1].PreferredWidth =
                                            wordApp.CentimetersToPoints(columnWidths[i]);
                                    }

                                    table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

                                    insertionPoint = table.Range.Duplicate;
                                    insertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
                                }

                                if (table != null)
                                {
                                    Marshal.ReleaseComObject(table);
                                    table = null;
                                }
                            }
                        }
                        catch (Exception tableEx)
                        {
                            throw new Exception($"Error creating table for {specValue}: {tableEx.Message}");
                        }
                    }
                }
                else
                {
                    throw new Exception($"Placeholder {placeholder} not found in document");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error in createtable: {ex.Message}\nStack Trace: {ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                throw;
            }
            finally
            {
                if (range != null)
                    Marshal.ReleaseComObject(range);
                if (table != null)
                    Marshal.ReleaseComObject(table);
            }
        }


        //private void createtable1(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        //{
        //    Table table = null;
        //    Range range = null;

        //    try
        //    {
        //        // Get columns, excluding SpecCols and TestParameter (since we'll merge it)
        //        var tableColumns = dt.Columns.Cast<DataColumn>()
        //            .Where(col => col.ColumnName != "SpecCols" && col.ColumnName != "TestParameter")
        //            .Select(col => col.ColumnName)
        //            .ToArray();

        //        var uniqueSpecs = dt.AsEnumerable()
        //            .Select(row => row.Field<string>("SpecCols"))
        //            .Distinct()
        //            .ToList();

        //        range = doc.Content;

        //        if (range == null) throw new Exception("Could not get document content range");

        //        range.Find.ClearFormatting();
        //        range.Find.Text = placeholder;
        //        range.Find.Forward = true;

        //        if (range.Find.Execute())
        //        {
        //            Range insertionPoint = range.Duplicate;

        //            foreach (string specValue in uniqueSpecs)
        //            {
        //                if (string.IsNullOrEmpty(specValue)) continue;

        //                try
        //                {
        //                    var filteredRows = dt.AsEnumerable()
        //                        .Where(row => row.Field<string>("SpecCols") == specValue)
        //                        .CopyToDataTable();

        //                    if (filteredRows.Rows.Count > 0)
        //                    {
        //                        if (insertionPoint != range.Duplicate)
        //                        {
        //                            insertionPoint.InsertParagraphAfter();
        //                            insertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
        //                            insertionPoint.InsertParagraphAfter();
        //                            insertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
        //                        }

        //                        // Create table with merged columns
        //                        table = doc.Tables.Add(insertionPoint, filteredRows.Rows.Count + 1, tableColumns.Length);
        //                        table.Borders.Enable = 1;

        //                        // Add headers
        //                        int colIndex = 1;
        //                        foreach (string columnName in tableColumns)
        //                        {
        //                            string headerText = columnName == "Specification" ?
        //                                "Requirement " + specValue : columnName;

        //                            table.Cell(1, colIndex).Range.Text = headerText;
        //                            table.Cell(1, colIndex).Range.Bold = 1;
        //                            table.Cell(1, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
        //                            colIndex++;
        //                        }

        //                        // Group by TestParameter
        //                        var groupedRows = filteredRows.AsEnumerable()
        //                            .GroupBy(row => row.Field<string>("TestParameter"))
        //                            .ToList();

        //                        int currentRow = 2;
        //                        foreach (var group in groupedRows)
        //                        {
        //                            string parameterName = group.Key + " :";

        //                            // Add parameter header row
        //                            table.Cell(currentRow, 1).Range.Text = parameterName;
        //                            table.Cell(currentRow, 1).Range.Bold = 1;
        //                            currentRow++;

        //                            // Add detail rows
        //                            foreach (var row in group)
        //                            {
        //                                int dataColIndex = 1;

        //                                // For Test Items column, combine with hyphen
        //                                table.Cell(currentRow, dataColIndex).Range.Text =
        //                                    "- " + row["Test Items(s)"].ToString();
        //                                table.Cell(currentRow, dataColIndex).Range.ParagraphFormat.LeftIndent = 10;
        //                                dataColIndex++;

        //                                // Fill remaining columns
        //                                foreach (string columnName in tableColumns.Skip(1))
        //                                {
        //                                    table.Cell(currentRow, dataColIndex).Range.Text =
        //                                        row[columnName].ToString();
        //                                    table.Cell(currentRow, dataColIndex).Range.ParagraphFormat.Alignment =
        //                                        WdParagraphAlignment.wdAlignParagraphCenter;
        //                                    dataColIndex++;
        //                                }
        //                                currentRow++;
        //                            }
        //                        }

        //                        // Format table
        //                        if (table != null)
        //                        {
        //                            table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
        //                            table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

        //                            // Adjust column widths for new format
        //                            float[] columnWidths = new float[tableColumns.Length];

        //                            for (int i = 0; i < tableColumns.Length; i++)
        //                            {
        //                                if (i == 0) // Test Items column
        //                                    columnWidths[i] = 5; // Wider for combined content
        //                                else if (tableColumns[i].Contains("Method"))
        //                                    columnWidths[i] = 4;
        //                                else if (tableColumns[i].Contains("Unit") ||
        //                                         tableColumns[i].Contains("LOD") ||
        //                                         tableColumns[i].Contains("Result"))
        //                                    columnWidths[i] = 2;
        //                                else
        //                                    columnWidths[i] = 3;
        //                            }

        //                            for (int i = 0; i < columnWidths.Length; i++)
        //                            {
        //                                table.Columns[i + 1].PreferredWidth =
        //                                    wordApp.CentimetersToPoints(columnWidths[i]);
        //                            }

        //                            table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);

        //                            insertionPoint = table.Range.Duplicate;
        //                            insertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
        //                        }

        //                        if (table != null)
        //                        {
        //                            Marshal.ReleaseComObject(table);
        //                            table = null;
        //                        }
        //                    }
        //                }
        //                catch (Exception tableEx)
        //                {
        //                    throw new Exception($"Error creating table for {specValue}: {tableEx.Message}");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"Placeholder {placeholder} not found in document");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = $"Error in createtable: {ex.Message}\nStack Trace: {ex.StackTrace}";
        //        System.Diagnostics.Debug.WriteLine(errorMessage);
        //        throw;
        //    }
        //    finally
        //    {
        //        if (range != null)
        //            Marshal.ReleaseComObject(range);
        //        if (table != null)
        //            Marshal.ReleaseComObject(table);
        //    }
        //}
        /* */
        private void CreateTable2(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            try
            {
                // Find the placeholder in the Word document
                Range range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Clear the placeholder text
                        range.Text = "";

                        // Group rows by the "Test Items(s)" column
                        var groups = dt.AsEnumerable()
                            .GroupBy(row => row.Field<string>("Test Items(s)"));

                        // Calculate the total number of rows needed
                        int totalRows = 1; // Header row
                        foreach (var group in groups)
                        {
                            // If no subitems, add one row; otherwise, add main category row + subitem rows
                            totalRows += group.Count() > 0 ?
                                (group.Any(row => !string.IsNullOrEmpty(row.Field<string>("Test SubItems"))) ?
                                    group.Count() + 1 : 1) : 1;
                        }

                        // Create a table with the correct number of columns (one less than the DataTable)
                        int numColumns = dt.Columns.Count - 1; // Remove one column for "Test SubItems"
                        Table table = doc.Tables.Add(range, totalRows, numColumns);
                        table.Borders.Enable = 1;

                        // Add column headers dynamically from the DataTable
                        int headerRow = 1;
                        int colIndex = 1;

                        // Add the first column header ("Test Items(s)")
                        table.Cell(headerRow, colIndex).Range.Text = dt.Columns[0].ColumnName;
                        table.Cell(headerRow, colIndex).Range.Bold = 1;
                        table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                        colIndex++;

                        // Skip the "Test SubItems" column and add the remaining column headers
                        for (int i = 2; i < dt.Columns.Count; i++)
                        {
                            table.Cell(headerRow, colIndex).Range.Text = dt.Columns[i].ColumnName;
                            table.Cell(headerRow, colIndex).Range.Bold = 1;
                            table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            colIndex++;
                        }

                        // Populate the table with data
                        int currentRow = 2;
                        foreach (var group in groups)
                        {
                            string mainItem = group.Key;

                            // Check if the group has any non-empty subitems
                            bool hasSubItems = group.Any(row => !string.IsNullOrEmpty(row.Field<string>("Test SubItems")));

                            if (!hasSubItems)
                            {
                                // If no subitems, use the first row's data for the entire group
                                DataRow firstRow = group.First();

                                // Main item in first column
                                table.Cell(currentRow, 1).Range.Text = mainItem;

                                // Fill remaining columns
                                colIndex = 2;
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (col.ColumnName != "Test Items(s)" && col.ColumnName != "Test SubItems")
                                    {
                                        // Handle null values in DataRow gracefully
                                        string cellValue = firstRow[col.ColumnName] != DBNull.Value ? firstRow[col.ColumnName].ToString() : "";
                                        table.Cell(currentRow, colIndex).Range.Text = cellValue;
                                        colIndex++;
                                    }
                                }
                                currentRow++;
                            }
                            else
                            {
                                // Add the main category row (Test Items(s) name)
                                table.Cell(currentRow, 1).Range.Text = mainItem + " :";
                                table.Cell(currentRow, 1).Range.Bold = 1;

                                // Clear the other cells in the main category row
                                for (int col = 2; col <= numColumns; col++)
                                {
                                    table.Cell(currentRow, col).Range.Text = "";
                                }
                                currentRow++;

                                // Add the subitem rows (Test SubItems)
                                foreach (DataRow row in group)
                                {
                                    // First column: subitem name with indent
                                    table.Cell(currentRow, 1).Range.Text = "- " + row["Test SubItems"].ToString();
                                    table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;

                                    // Fill the remaining columns, skipping the "Test Items(s)" and "Test SubItems" columns
                                    colIndex = 2;
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        if (col.ColumnName != "Test Items(s)" && col.ColumnName != "Test SubItems")
                                        {
                                            // Handle null values in DataRow gracefully
                                            string cellValue = row[col.ColumnName] != DBNull.Value ? row[col.ColumnName].ToString() : "";
                                            table.Cell(currentRow, colIndex).Range.Text = cellValue;
                                            colIndex++;
                                        }
                                    }
                                    currentRow++;
                                }
                            }
                        }

                        // Format the table
                        table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                        table.Rows[1].HeadingFormat = -1;

                        // Add borders
                        table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exception
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

        /* without requirements (for all p/s/ss)*/

        private void CreateTable3(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            Table table = null;
            Range range = null;
            Range ftr = null;
            try
            {
                //string footers = "";
                //foreach (Section aSection in wordApp.ActiveDocument.Sections)
                //{                   
                //    foreach (HeaderFooter aFooter in aSection.Footers)
                //        footers = aFooter.Range.Text;
                //}

                range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    range.Text = "";

                    var groups = dt.AsEnumerable()
                        .GroupBy(row => row.Field<string>("Test Items(s)"))
                        .ToList();

                    int totalRows = 1; // Header row
                    foreach (var group in groups)
                    {
                        totalRows += 1; // Main category row
                        totalRows += group.Count(); // Subitem rows
                    }

                    int numColumns = dt.Columns.Count - 2; // Exclude Test SubItems and Test SubSubItems
                    table = doc.Tables.Add(range, totalRows, numColumns);
                    table.Borders.Enable = 1;

                    // Add headers dynamically
                    int headerRow = 1;
                    int colIndex = 1;
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName != "Test SubItems" && col.ColumnName != "Test SubSubItems")
                        {
                            table.Cell(headerRow, colIndex).Range.Text = col.ColumnName;
                            table.Cell(headerRow, colIndex).Range.Bold = 1;
                            table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            colIndex++;
                        }
                    }

                    int currentRow = 2;
                    foreach (var group in groups)
                    {
                        string mainItem = group.Key;
                        bool hasSubItems = group.Any(row => !string.IsNullOrEmpty(row["Test SubItems"].ToString()));
                        bool hasSubSubItems = group.Any(row => !string.IsNullOrEmpty(row["Test SubSubItems"].ToString()));

                        if (!hasSubItems && !hasSubSubItems)
                        {
                            // Scenario 1: Only Test Items(s) and its data
                            // Modified to show Test Items(s) and data in one row without bold
                            table.Cell(currentRow, 1).Range.Text = mainItem;
                            FillRow(table, currentRow, group.First(), dt.Columns, 1);
                            currentRow++;
                        }
                        else if (hasSubItems && !hasSubSubItems)
                        {
                            // Scenario 2: Test Items(s) and Test SubItems
                            table.Cell(currentRow, 1).Range.Text = mainItem;
                            table.Cell(currentRow, 1).Range.Bold = 1;
                            currentRow++;

                            foreach (DataRow row in group)
                            {
                                string subItem = row["Test SubItems"].ToString();
                                table.Cell(currentRow, 1).Range.Text = "- " + subItem;
                                table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;
                                FillRow(table, currentRow, row, dt.Columns, 1);
                                currentRow++;
                            }
                        }
                        else if (hasSubItems && hasSubSubItems)
                        {
                            // Scenario 3: Test Items(s), Test SubItems, and Test SubSubItems
                            table.Cell(currentRow, 1).Range.Text = mainItem;
                            table.Cell(currentRow, 1).Range.Bold = 1;
                            currentRow++;

                            var subItems = group.Select(row => row["Test SubItems"].ToString()).Distinct();
                            foreach (var subItem in subItems)
                            {
                                table.Cell(currentRow, 1).Range.Text = "• " + subItem;
                                table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;
                                currentRow++;

                                var subSubItems = group.Where(row => row["Test SubItems"].ToString() == subItem);
                                foreach (var row in subSubItems)
                                {
                                    string subSubItem = row["Test SubSubItems"].ToString();
                                    if (!string.IsNullOrEmpty(subSubItem))
                                    {
                                        table.Cell(currentRow, 1).Range.Text = "- " + subSubItem;
                                        table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 20;
                                        FillRow(table, currentRow, row, dt.Columns, 1);
                                        currentRow++;
                                    }
                                }
                            }
                        }
                    }

                    // Format table
                    table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                    table.Rows[1].HeadingFormat = -1;

                    // Add borders
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                }
                else
                {
                    throw new Exception($"Placeholder {placeholder} not found in document");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating table: {ex.Message}");
            }
        }

        private void WithoutRequirementCreateTable1(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            Table table = null;
            Range range = null;

            try
            {
                range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    range.Text = "";

                    var groups = dt.AsEnumerable()
                        .GroupBy(row => row.Field<string>("Test Items(s)"))
                        .ToList();

                    int totalRows = 1; // Header row
                    foreach (var group in groups)
                    {
                        // Only count rows with actual data
                        totalRows += 1; // Main category row
                        totalRows += group.Count(row => !string.IsNullOrEmpty(row["Test SubItems"].ToString()) || !string.IsNullOrEmpty(row["Test SubSubItems"].ToString())); // Count only rows with data
                    }

                    int numColumns = dt.Columns.Count - 2; // Exclude Test SubItems and Test SubSubItems
                    table = doc.Tables.Add(range, totalRows, numColumns);
                    table.Borders.Enable = 1;

                    // Add headers dynamically
                    int headerRow = 1;
                    int colIndex = 1;
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName != "Test SubItems" && col.ColumnName != "Test SubSubItems")
                        {
                            table.Cell(headerRow, colIndex).Range.Text = col.ColumnName;
                            table.Cell(headerRow, colIndex).Range.Bold = 1;
                            table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                            colIndex++;
                        }
                    }

                    int currentRow = 2;
                    foreach (var group in groups)
                    {
                        string mainItem = group.Key;
                        bool hasSubItems = group.Any(row => !string.IsNullOrEmpty(row["Test SubItems"].ToString()));
                        bool hasSubSubItems = group.Any(row => !string.IsNullOrEmpty(row["Test SubSubItems"].ToString()));

                        if (!hasSubItems && !hasSubSubItems)
                        {
                            // Scenario 1: Only Test Items(s) and its data
                            // Check if the main item has any data
                            table.Cell(currentRow, 1).Range.Text = mainItem;
                            FillRow(table, currentRow, group.First(), dt.Columns, 1);
                            currentRow++;
                        }
                        else if (hasSubItems && !hasSubSubItems)
                        {
                            // Scenario 2: Test Items(s) and Test SubItems
                            table.Cell(currentRow, 1).Range.Text = mainItem;
                            table.Cell(currentRow, 1).Range.Bold = 1;
                            currentRow++;

                            foreach (DataRow row in group)
                            {
                                string subItem = row["Test SubItems"].ToString();
                                if (!string.IsNullOrEmpty(subItem))
                                {
                                    table.Cell(currentRow, 1).Range.Text = "- " + subItem;
                                    table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;
                                    FillRow(table, currentRow, row, dt.Columns, 1);
                                    currentRow++;
                                }
                            }
                        }
                        else if (hasSubItems && hasSubSubItems)
                        {
                            // Scenario 3: Test Items(s), Test SubItems, and Test SubSubItems
                            table.Cell(currentRow, 1).Range.Text = mainItem;
                            table.Cell(currentRow, 1).Range.Bold = 1;
                            currentRow++;

                            var subItems = group.Select(row => row["Test SubItems"].ToString()).Distinct();
                            foreach (var subItem in subItems)
                            {
                                table.Cell(currentRow, 1).Range.Text = "• " + subItem;
                                table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;
                                currentRow++;

                                var subSubItems = group.Where(row => row["Test SubItems"].ToString() == subItem);
                                foreach (var row in subSubItems)
                                {
                                    string subSubItem = row["Test SubSubItems"].ToString();
                                    if (!string.IsNullOrEmpty(subSubItem))
                                    {
                                        table.Cell(currentRow, 1).Range.Text = "- " + subSubItem;
                                        table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 20;
                                        FillRow(table, currentRow, row, dt.Columns, 1);
                                        currentRow++;
                                    }
                                }
                            }
                        }
                    }

                    // Format table
                    table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                    table.Rows[1].HeadingFormat = -1;

                    // Add borders
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                }
                else
                {
                    throw new Exception($"Placeholder {placeholder} not found in document");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating table: {ex.Message}");
            }
        }

        /* Without Requirement for P,S,SS */
        private void FillRow(Table table, int rowIndex, DataRow dataRow, DataColumnCollection columns, int startColumnIndex = 0)
        {
            int colIndex = startColumnIndex + 1;
            foreach (DataColumn col in columns)
            {
                if (col.ColumnName != "Test Items(s)" && col.ColumnName != "Test SubItems" && col.ColumnName != "Test SubSubItems")
                {
                    table.Cell(rowIndex, colIndex).Range.Text = dataRow[col.ColumnName].ToString();
                    colIndex++;
                }
            }
        }

        /* */
        //private void CreateTable4(Application wordApp, Document doc, string placeholder, DataTable dt)
        //{
        //    var range = doc.Content;
        //    range.Find.ClearFormatting();
        //    range.Find.Text = placeholder;
        //    range.Find.Execute();
        //    if (range.Find.Found && dt != null && dt.Rows.Count > 0)
        //    {
        //        dt.Columns["TestSubParameter"].ColumnName = "Test Items";

        //        if (dt.Columns.Contains("TestParameter"))
        //        {
        //            string testParamValue = dt.Rows[0]["TestParameter"].ToString();
        //            dt.Columns.Remove("TestParameter");
        //            dt.Columns["Specification"].ColumnName = testParamValue;
        //        }

        //        DataTable newDt = new DataTable();
        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            if (col.ColumnName != "Test Items(s)")
        //            {
        //                newDt.Columns.Add(col.ColumnName, col.DataType);
        //            }
        //        }

        //        // Modified grouping to put bold header sections at the end
        //        var allGroups = dt.AsEnumerable()
        //            .GroupBy(row => row.Field<string>("Test Items"))
        //            .OrderBy(g =>
        //            {
        //                var firstRow = g.FirstOrDefault();
        //                if (firstRow != null)
        //                {
        //                    // Check if this is a main category (has subitems)
        //                    bool hasSubItems = g.Any(r => !string.IsNullOrEmpty(r.Field<string>("Test Items(s)")));
        //                    return hasSubItems ? 2 : 1;
        //                }
        //                return 1;
        //            })
        //            .ThenBy(g => g.Key);

        //        foreach (var mainGroup in allGroups)
        //        {
        //            var subGroups = mainGroup.GroupBy(row => row.Field<string>("Test Items(s)"));

        //            if (subGroups.Any(g => string.IsNullOrEmpty(g.Key)))
        //            {
        //                foreach (var row in mainGroup.Where(r => string.IsNullOrEmpty(r.Field<string>("Test Items(s)"))))
        //                {
        //                    DataRow directRow = newDt.NewRow();
        //                    directRow["Test Items"] = "- " + mainGroup.Key;
        //                    foreach (DataColumn col in dt.Columns)
        //                    {
        //                        if (col.ColumnName != "Test Items" && col.ColumnName != "Test Items(s)")
        //                        {
        //                            directRow[col.ColumnName] = row[col];
        //                        }
        //                    }
        //                    newDt.Rows.Add(directRow);
        //                }
        //            }
        //            else
        //            {
        //                DataRow headerRow = newDt.NewRow();
        //                headerRow["Test Items"] = mainGroup.Key + " :";
        //                newDt.Rows.Add(headerRow);

        //                foreach (var row in mainGroup.Where(r => !string.IsNullOrEmpty(r.Field<string>("Test Items(s)"))))
        //                {
        //                    DataRow newRow = newDt.NewRow();
        //                    newRow["Test Items"] = "- " + row.Field<string>("Test Items(s)");
        //                    foreach (DataColumn col in dt.Columns)
        //                    {
        //                        if (col.ColumnName != "Test Items" &&
        //                            col.ColumnName != "Test Items(s)" &&
        //                            !string.IsNullOrEmpty(row[col].ToString()))
        //                        {
        //                            newRow[col.ColumnName] = row[col];
        //                        }
        //                    }
        //                    newDt.Rows.Add(newRow);
        //                }
        //            }
        //        }

        //        Table table = doc.Tables.Add(range, newDt.Rows.Count + 1, newDt.Columns.Count);
        //        table.Borders.Enable = 1;
        //        table.AllowAutoFit = true;

        //        for (int col = 0; col < newDt.Columns.Count; col++)
        //        {
        //            table.Cell(1, col + 1).Range.Text = newDt.Columns[col].ColumnName;
        //            table.Cell(1, col + 1).Range.Bold = 1;
        //            table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
        //        }

        //        for (int row = 0; row < newDt.Rows.Count; row++)
        //        {
        //            for (int col = 0; col < newDt.Columns.Count; col++)
        //            {
        //                string cellText = newDt.Rows[row][col]?.ToString() ?? string.Empty;
        //                table.Cell(row + 2, col + 1).Range.Text = cellText;

        //                if (col == 0 && cellText.EndsWith(" :"))
        //                {
        //                    table.Cell(row + 2, col + 1).Range.Bold = 1;
        //                }
        //            }
        //        }

        //        table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
        //        table.Rows[1].HeadingFormat = -1;
        //        table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
        //        table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
        //    }
        //}
        private void CreateTable4(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();
            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

                if (dt.Columns.Contains("Requirement"))
                {
                    string testParamValue = dt.Rows[0]["Requirement"].ToString();
                    dt.Columns.Remove("Requirement");
                    dt.Columns["Specification"].ColumnName = testParamValue;
                }

                DataTable newDt = new DataTable();
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "TestSubParameter")
                    {
                        newDt.Columns.Add(col.ColumnName, col.DataType);
                    }
                }

                // Modified grouping to put bold header sections at the end
                var allGroups = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Item(s)"))
                    .OrderBy(g =>
                    {
                        var firstRow = g.FirstOrDefault();
                        if (firstRow != null)
                        {
                            // Check if this is a main category (has subitems)
                            bool hasSubItems = g.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));
                            return hasSubItems ? 2 : 1;
                        }
                        return 1;
                    })
                    .ThenBy(g => g.Key);

                foreach (var mainGroup in allGroups)
                {
                    var subGroups = mainGroup.GroupBy(row => row.Field<string>("TestSubParameter"));

                    if (subGroups.Any(g => string.IsNullOrEmpty(g.Key)))
                    {
                        foreach (var row in mainGroup.Where(r => string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                        {
                            DataRow directRow = newDt.NewRow();
                            directRow["Test Item(s)"] = "- " + mainGroup.Key;
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)" && col.ColumnName != "TestSubParameter")
                                {
                                    directRow[col.ColumnName] = row[col];
                                }
                            }
                            newDt.Rows.Add(directRow);
                        }
                    }
                    else
                    {
                        DataRow headerRow = newDt.NewRow();
                        headerRow["Test Item(s)"] = mainGroup.Key + " :";
                        newDt.Rows.Add(headerRow);

                        foreach (var row in mainGroup.Where(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                        {
                            DataRow newRow = newDt.NewRow();
                            newRow["Test Item(s)"] = "- " + row.Field<string>("TestSubParameter");
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)" &&
                                    col.ColumnName != "TestSubParameter" &&
                                    !string.IsNullOrEmpty(row[col].ToString()))
                                {
                                    newRow[col.ColumnName] = row[col];
                                }
                            }
                            newDt.Rows.Add(newRow);
                        }
                    }
                }

                Table table = doc.Tables.Add(range, newDt.Rows.Count + 1, newDt.Columns.Count);
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;

                for (int col = 0; col < newDt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = newDt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                }

                for (int row = 0; row < newDt.Rows.Count; row++)
                {
                    for (int col = 0; col < newDt.Columns.Count; col++)
                    {
                        string cellText = newDt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;

                        if (col == 0 && cellText.EndsWith(" :"))
                        {
                            table.Cell(row + 2, col + 1).Range.Bold = 1;
                        }
                    }
                }

                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            }
        }


        private void RequirementCreateTable1(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();
            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

                if (dt.Columns.Contains("Requirement"))
                {
                    string testParamValue = dt.Rows[0]["Requirement"].ToString();
                    dt.Columns.Remove("Requirement");
                    dt.Columns["Specification"].ColumnName = testParamValue;
                }

                DataTable newDt = new DataTable();
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "TestSubParameter")
                    {
                        newDt.Columns.Add(col.ColumnName, col.DataType);
                    }
                }

                // Modified grouping to put bold header sections at the end
                var allGroups = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Test Item(s)"))
                    .OrderBy(g =>
                    {
                        var firstRow = g.FirstOrDefault();
                        if (firstRow != null)
                        {
                            // Check if this is a main category (has subitems)
                            bool hasSubItems = g.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));
                            return hasSubItems ? 2 : 1;
                        }
                        return 1;
                    })
                    .ThenBy(g => g.Key);

                foreach (var mainGroup in allGroups)
                {
                    var subGroups = mainGroup.GroupBy(row => row.Field<string>("TestSubParameter"));

                    if (subGroups.Any(g => string.IsNullOrEmpty(g.Key)))
                    {
                        foreach (var row in mainGroup.Where(r => string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                        {
                            DataRow directRow = newDt.NewRow();
                            directRow["Test Item(s)"] = "- " + mainGroup.Key;
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)" && col.ColumnName != "TestSubParameter")
                                {
                                    directRow[col.ColumnName] = row[col];
                                }
                            }
                            newDt.Rows.Add(directRow);
                        }
                    }
                    else
                    {
                        DataRow headerRow = newDt.NewRow();
                        headerRow["Test Item(s)"] = mainGroup.Key + " :";
                        newDt.Rows.Add(headerRow);

                        foreach (var row in mainGroup.Where(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                        {
                            DataRow newRow = newDt.NewRow();
                            newRow["Test Item(s)"] = "- " + row.Field<string>("TestSubParameter");
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)" &&
                                    col.ColumnName != "TestSubParameter" &&
                                    !string.IsNullOrEmpty(row[col].ToString()))
                                {
                                    newRow[col.ColumnName] = row[col];
                                }
                            }
                            newDt.Rows.Add(newRow);
                        }
                    }
                }

                Table table = doc.Tables.Add(range, newDt.Rows.Count + 1, newDt.Columns.Count);
                table.Borders.Enable = 1;
                table.AllowAutoFit = true;

                for (int col = 0; col < newDt.Columns.Count; col++)
                {
                    table.Cell(1, col + 1).Range.Text = newDt.Columns[col].ColumnName;
                    table.Cell(1, col + 1).Range.Bold = 1;
                    table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                }

                for (int row = 0; row < newDt.Rows.Count; row++)
                {
                    for (int col = 0; col < newDt.Columns.Count; col++)
                    {
                        string cellText = newDt.Rows[row][col]?.ToString() ?? string.Empty;
                        table.Cell(row + 2, col + 1).Range.Text = cellText;

                        if (col == 0 && cellText.EndsWith(" :"))
                        {
                            table.Cell(row + 2, col + 1).Range.Bold = 1;
                        }
                    }
                }

                table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                table.Rows[1].HeadingFormat = -1;
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            }
        }

        /* Requirement with InsertImagesIntoDocument */
        private void InsertImagesIntoDocument(Application wordApp, Document doc, string placeholder, List<string> imagePaths)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && imagePaths != null && imagePaths.Count > 0)
            {
                // Determine the number of rows and columns needed
                int imagesPerRow = 3;
                int totalImages = imagePaths.Count;
                int rows = (int)Math.Ceiling((double)totalImages / imagesPerRow);

                // Create a table with rows and columns
                Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(range, rows, imagesPerRow);

                // Populate the table with images
                int imageIndex = 0;
                for (int row = 1; row <= rows; row++)
                {
                    for (int col = 1; col <= imagesPerRow; col++)
                    {
                        if (imageIndex < totalImages)
                        {
                            string imagePath = Server.MapPath("~/SampleImages/" + imagePaths[imageIndex]);
                            if (File.Exists(imagePath))
                            {
                                Microsoft.Office.Interop.Word.Cell cell = table.Cell(row, col);
                                cell.Range.InlineShapes.AddPicture(imagePath);

                                // Resize the image as needed
                                Microsoft.Office.Interop.Word.InlineShape shape = cell.Range.InlineShapes[1];
                                shape.Width = 100; // Set desired width (adjust as needed)
                                shape.Height = 100; // Maintain aspect ratio

                                imageIndex++;
                            }
                        }
                    }
                }

                // Move the range to the end of the table to continue processing if needed
                range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
            }
        }

        /* Requirement for P,S */
        private void MultipleRequirementsCreateTable1(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["TestParameter"].ColumnName = "Test Item(s)";

                // Create a list to store all tables
                List<Table> allTables = new List<Table>();

                var requirementGroups = dt.AsEnumerable()
                    .GroupBy(row => row.Field<string>("Requirement"))
                    .OrderBy(g => g.Key);

                bool isFirstTable = true;
                float[] maxColumnWidths = new float[6]; // Store maximum width for each column

                foreach (var reqGroup in requirementGroups)
                {
                    if (!isFirstTable)
                    {
                        range.InsertParagraphAfter();
                        range.Collapse(WdCollapseDirection.wdCollapseEnd);
                    }

                    DataTable currentDt = new DataTable();

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName != "Requirement" && col.ColumnName != "TestSubParameter")
                        {
                            currentDt.Columns.Add(col.ColumnName, col.DataType);
                        }
                    }

                    var mainGroups = reqGroup
                        .GroupBy(row => row.Field<string>("Test Item(s)"))
                        .OrderBy(g =>
                        {
                            var firstRow = g.FirstOrDefault();
                            bool hasSubItems = g.Any(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter")));
                            return hasSubItems ? 2 : 1;
                        })
                        .ThenBy(g => g.Key);

                    foreach (var mainGroup in mainGroups)
                    {
                        var subGroups = mainGroup.GroupBy(row => row.Field<string>("TestSubParameter"));

                        if (subGroups.Any(g => string.IsNullOrEmpty(g.Key)))
                        {
                            foreach (var row in mainGroup.Where(r => string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                            {
                                DataRow newRow = currentDt.NewRow();
                                newRow["Test Item(s)"] = "- " + mainGroup.Key;
                                foreach (DataColumn col in currentDt.Columns)
                                {
                                    if (col.ColumnName != "Test Item(s)")
                                    {
                                        newRow[col.ColumnName] = row[col.ColumnName];
                                    }
                                }
                                currentDt.Rows.Add(newRow);
                            }
                        }
                        else
                        {
                            DataRow headerRow = currentDt.NewRow();
                            headerRow["Test Item(s)"] = mainGroup.Key + " :";
                            currentDt.Rows.Add(headerRow);

                            foreach (var row in mainGroup.Where(r => !string.IsNullOrEmpty(r.Field<string>("TestSubParameter"))))
                            {
                                DataRow newRow = currentDt.NewRow();
                                newRow["Test Item(s)"] = "- " + row.Field<string>("TestSubParameter");
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

                    Table table = doc.Tables.Add(range, currentDt.Rows.Count + 1, currentDt.Columns.Count);
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;

                    table.Cell(1, 1).Range.Text = "Test Item(s)";
                    for (int col = 1; col < currentDt.Columns.Count; col++)
                    {
                        if (col == 4)
                        {
                            table.Cell(1, col + 1).Range.Text = reqGroup.Key;
                        }
                        else
                        {
                            table.Cell(1, col + 1).Range.Text = currentDt.Columns[col].ColumnName;
                        }
                    }

                    for (int col = 1; col <= currentDt.Columns.Count; col++)
                    {
                        table.Cell(1, col).Range.Bold = 1;
                        table.Cell(1, col).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    }

                    for (int row = 0; row < currentDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < currentDt.Columns.Count; col++)
                        {
                            string cellText = currentDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;

                            if (col == 0 && cellText.EndsWith(" :"))
                            {
                                table.Cell(row + 2, col + 1).Range.Bold = 1;
                            }
                        }
                    }

                    table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    allTables.Add(table);

                    // Store maximum column widths
                    for (int col = 1; col <= currentDt.Columns.Count; col++)
                    {
                        maxColumnWidths[col - 1] = Math.Max(maxColumnWidths[col - 1], table.Columns[col].Width);
                    }

                    isFirstTable = false;
                    range = table.Range;
                    range.Collapse(WdCollapseDirection.wdCollapseEnd);
                }

                // Apply the maximum width to all columns in all tables
                foreach (Table table in allTables)
                {
                    for (int col = 1; col <= table.Columns.Count; col++)
                    {
                        table.Columns[col].Width = maxColumnWidths[col - 1];
                    }
                }
            }
        }




        private void RequirementCreateTable2(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            try
            {
                // Find the placeholder in the Word document
                Range range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;

                if (range.Find.Execute())
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Clear the placeholder text
                        range.Text = "";

                        // Group rows by the "TestParameter" column (as per the SQL query)
                        var groups = dt.AsEnumerable()
                            .GroupBy(row => row.Field<string>("TestParameter"));

                        // Calculate the total number of rows needed
                        int totalRows = 1; // Header row
                        foreach (var group in groups)
                        {
                            totalRows += group.Count() > 0 ?
                                (group.Any(row => !string.IsNullOrEmpty(row.Field<string>("TestSubParameter"))) ?
                                    group.Count() + 1 : 1) : 1;
                        }

                        // Create a table with the correct number of columns (excluding TestSubParameter, RequirementID, and Requirement)
                        int numColumns = dt.Columns.Count - 3; // Exclude TestSubParameter, RequirementID, and Requirement
                        Table table = doc.Tables.Add(range, totalRows, numColumns);
                        table.Borders.Enable = 1;

                        // Add column headers dynamically from the DataTable
                        int headerRow = 1;
                        int colIndex = 1;

                        // Replace the "Specification" header with the "Requirement" column's data (use first row for Requirement)
                        string requirementHeader = dt.Rows[0]["Requirement"].ToString(); // Get the first value from "Requirement" column

                        // Add headers for all columns excluding TestSubParameter, RequirementID, and Requirement
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName != "TestSubParameter" && col.ColumnName != "RequirementID" && col.ColumnName != "Requirement")
                            {
                                if (col.ColumnName == "Specification")
                                {
                                    table.Cell(headerRow, colIndex).Range.Text = requirementHeader; // Set the Requirement value as header for Specification
                                }
                                else
                                {
                                    table.Cell(headerRow, colIndex).Range.Text = col.ColumnName; // Default header for other columns
                                }
                                table.Cell(headerRow, colIndex).Range.Bold = 1;
                                table.Cell(headerRow, colIndex).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                                colIndex++;
                            }
                        }

                        // Populate the table with data
                        int currentRow = 2;
                        foreach (var group in groups)
                        {
                            string mainItem = group.Key;

                            // Check if the group has any non-empty subitems
                            bool hasSubItems = group.Any(row => !string.IsNullOrEmpty(row.Field<string>("TestSubParameter")));

                            if (!hasSubItems)
                            {
                                // If no subitems, use the first row's data for the entire group
                                DataRow firstRow = group.First();

                                // Main item in first column
                                table.Cell(currentRow, 1).Range.Text = mainItem;

                                // Fill remaining columns
                                colIndex = 2;
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (col.ColumnName != "TestSubParameter" && col.ColumnName != "RequirementID" && col.ColumnName != "Requirement")
                                    {
                                        string cellValue = firstRow[col.ColumnName] != DBNull.Value ? firstRow[col.ColumnName].ToString() : "";
                                        table.Cell(currentRow, colIndex).Range.Text = cellValue;
                                        colIndex++;
                                    }
                                }
                                currentRow++;
                            }
                            else
                            {
                                // Add the main category row (TestParameter name)
                                table.Cell(currentRow, 1).Range.Text = mainItem + " :";
                                table.Cell(currentRow, 1).Range.Bold = 1;

                                // Clear the other cells in the main category row
                                for (int col = 2; col <= numColumns; col++)
                                {
                                    table.Cell(currentRow, col).Range.Text = "";
                                }
                                currentRow++;

                                // Add the subitem rows (TestSubParameter)
                                foreach (DataRow row in group)
                                {
                                    // First column: subitem name with indent
                                    table.Cell(currentRow, 1).Range.Text = "- " + row["TestSubParameter"].ToString();
                                    table.Cell(currentRow, 1).Range.ParagraphFormat.LeftIndent = 10;

                                    // Fill the remaining columns (excluding TestSubParameter, RequirementID, and Requirement)
                                    colIndex = 2;
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        if (col.ColumnName != "TestParameter" && col.ColumnName != "TestSubParameter" && col.ColumnName != "RequirementID" && col.ColumnName != "Requirement")
                                        {
                                            string cellValue = row[col.ColumnName] != DBNull.Value ? row[col.ColumnName].ToString() : "";
                                            table.Cell(currentRow, colIndex).Range.Text = cellValue;
                                            colIndex++;
                                        }
                                    }
                                    currentRow++;
                                }
                            }
                        }

                        // Format the table
                        table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                        table.Rows[1].HeadingFormat = -1;

                        // Add borders
                        table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exception
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }


        public static void CreateNutritionalTable(Microsoft.Office.Interop.Word.Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder)
        {
            var range = doc.Content;
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found)
            {
                var table = doc.Tables.Add(range, 10, 2);

                // First remove all borders
                table.Borders.Enable = 0;

                // Create main header with border
                table.Cell(1, 1).Range.Text = "INFORMASI NILAI GIZI";
                table.Rows[1].Cells.Merge();
                table.Cell(1, 1).Range.Bold = 1;
                table.Cell(1, 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table.Cell(1, 1).Range.Font.Size = 14;

                // Create a separate range for data section (rows 2-10)
                var dataRange = table.Range.Rows.First.Next.Range;
                dataRange.End = table.Range.End;

                // Add outside border to data section
                dataRange.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;

                // Add "JUMLAH Per 100 mL" row
                table.Cell(2, 1).Range.Text = "JUMLAH Per 100 mL";
                table.Cell(2, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;

                // Add data
                string[,] data = {
                      {"Energi Total", "900 kkal"},
                      {"Lemak Total", "100 g"},
                      {"Lemak Jenuh", "42 g"},
                      {"Protein", "0 g"},
                      {"Karbohidrat", "0 g"},
                      {"Gula", "0 g"},
                      {"Garam (Natrium)", "0 mg"},
                      {"Vitamin A", "2906 mcg" }
                   };

                // Fill table
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    table.Cell(i + 3, 1).Range.Text = data[i, 0];
                    table.Cell(i + 3, 2).Range.Text = data[i, 1];
                    // Right align second column
                    table.Cell(i + 3, 2).Range.ParagraphFormat.Alignment =
                        Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                }

                // Remove inside borders for data section
                table.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone;
                // Add border to header
                table.Rows[1].Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            }
        }
        protected void GenerateQR(string SampleID)
        {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            var result = QCwriter.Write("http://ptqualis.rnisolutions.in:4545/TestReport/TestReport-" + SampleID + ".pdf");//QCwriter.Write(lblBatchNumber.Text);
            string path = Server.MapPath("~/QR/MyQR.jpg");
            var barcodeBitmap = new Bitmap(result);

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path,
                   FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
                string sourceFile = Server.MapPath("~/QR/MyQR.jpg");
                string destinationFile = Server.MapPath("~/TestReport/" + SampleID + ".jpg");

                try
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }
        private void CreateTableLastRaisins(Application wordApp, Microsoft.Office.Interop.Word.Document doc, string placeholder, DataTable dt)
        {
            try
            {
                var range = doc.Content;
                range.Find.ClearFormatting();
                range.Find.Text = placeholder;
                range.Find.Execute();

                if (range.Find.Found && dt != null && dt.Rows.Count > 0)
                {
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
                        if (col.ColumnName != "TestSubParameter" && col.ColumnName != "Requirement")
                            newDt.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var testItemGroups = dt.AsEnumerable()
                        .GroupBy(row => row.Field<string>("Test Item(s)"))
                        .OrderBy(g => g.Key);

                    foreach (var mainGroup in testItemGroups)
                    {
                        DataRow headerRow = newDt.NewRow();
                        headerRow["Test Item(s)"] = mainGroup.Key + (mainGroup.Key.EndsWith(":") ? "" : ":");
                        newDt.Rows.Add(headerRow);

                        var subGroups = mainGroup
                            .GroupBy(row => row.Field<string>("TestSubParameter"))
                            .Where(g => !string.IsNullOrEmpty(g.Key));

                        foreach (var subGroup in subGroups)
                        {
                            var firstRow = subGroup.First();
                            DataRow mergedRow = newDt.NewRow();
                            mergedRow["Test Item(s)"] = "- " + subGroup.Key;

                            foreach (DataColumn col in newDt.Columns)
                            {
                                if (col.ColumnName != "Test Item(s)" &&
                                    col.ColumnName != "Unit(s)" &&
                                    col.ColumnName != "Result(s)")
                                {
                                    mergedRow[col.ColumnName] = firstRow[col.ColumnName];
                                }
                            }

                            var unitResults = subGroup.Select(row => new
                            {
                                Unit = row["Unit(s)"].ToString(),
                                Result = row["Result(s)"].ToString()
                            }).Where(ur => !string.IsNullOrEmpty(ur.Unit) || !string.IsNullOrEmpty(ur.Result));

                            bool isFirst = true;
                            foreach (var ur in unitResults)
                            {
                                if (isFirst)
                                {
                                    mergedRow["Unit(s)"] = ur.Unit;
                                    mergedRow["Result(s)"] = ur.Result;
                                    newDt.Rows.Add(mergedRow);
                                    isFirst = false;
                                }
                                else
                                {
                                    DataRow additionalRow = newDt.NewRow();
                                    additionalRow["Unit(s)"] = ur.Unit;
                                    additionalRow["Result(s)"] = ur.Result;
                                    newDt.Rows.Add(additionalRow);
                                }
                            }
                        }
                    }

                    Table table = doc.Tables.Add(range, newDt.Rows.Count + 1, newDt.Columns.Count);
                    table.Borders.Enable = 1;
                    table.AllowAutoFit = true;

                    // Set headers
                    for (int col = 0; col < newDt.Columns.Count; col++)
                    {
                        table.Cell(1, col + 1).Range.Text = newDt.Columns[col].ColumnName;
                        table.Cell(1, col + 1).Range.Bold = 1;
                        table.Cell(1, col + 1).Range.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                    }

                    // Set data rows
                    for (int row = 0; row < newDt.Rows.Count; row++)
                    {
                        for (int col = 0; col < newDt.Columns.Count; col++)
                        {
                            string cellText = newDt.Rows[row][col]?.ToString() ?? string.Empty;
                            table.Cell(row + 2, col + 1).Range.Text = cellText;
                            if (col == 0 && !cellText.StartsWith("-"))
                                table.Cell(row + 2, col + 1).Range.Bold = 1;
                        }
                    }

                    table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitContent);
                    table.Rows[1].HeadingFormat = -1;
                    table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    if (requirements.Count > 0)
                    {
                        Range requirementRange = doc.Content;
                        requirementRange.Find.ClearFormatting();
                        requirementRange.Find.Text = "{Requirement}";
                        if (requirementRange.Find.Execute())
                            requirementRange.Text = string.Join(Environment.NewLine, requirements);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateTableLastRaisins: {ex.Message}");
                throw;
            }
        }
        private void InsertQR(Application wordApp, Document doc, string placeholder, string imagePaths)
        {
            var range = doc.Content;
            range.Find.ClearFormatting();
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range.Find.Found && imagePaths != null)
            {
                // Determine the number of rows and columns needed
                int imagesPerRow = 3;
                int totalImages = 1;
                int rows = 1;

                // Create a table with rows and columns
                Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(range, rows, imagesPerRow);

                // Populate the table with images
                int imageIndex = 0; int row = 1; int col = 1;

                string imagePath = Server.MapPath("~/TestReport/" + imagePaths);
                if (File.Exists(imagePath))
                {
                    Microsoft.Office.Interop.Word.Cell cell = table.Cell(row, col);
                    cell.Range.InlineShapes.AddPicture(imagePath);

                    // Resize the image as needed
                    Microsoft.Office.Interop.Word.InlineShape shape = cell.Range.InlineShapes[1];
                    shape.Width = 100; // Set desired width (adjust as needed)
                    shape.Height = 100; // Maintain aspect ratio
                }
            }

            // Move the range to the end of the table to continue processing if needed
            range.Collapse(Microsoft.Office.Interop.Word.WdCollapseDirection.wdCollapseEnd);
        }
        private void ReplaceBookmarkText( Microsoft.Office.Interop.Word.Document doc,  string bookmarkName, string text)
        {
            if (doc.Bookmarks.Exists(bookmarkName))
            {
                Object name = bookmarkName;
                Microsoft.Office.Interop.Word.Range range = doc.Bookmarks.get_Item(ref name).Range;
                range.Text = text;
                object newRange = range;
                doc.Bookmarks.Add(bookmarkName, ref newRange);

            }

        }
        static void FindAndReplaceFooter(Application wordApp, string findText, Microsoft.Office.Interop.Word.HeadersFooters footers, string replaceText)
        {
            
            wordApp.Selection.Find.ClearFormatting();
            wordApp.Selection.Find.Text = findText;
            wordApp.Selection.Find.Replacement.ClearFormatting();
            wordApp.Selection.Find.Replacement.Text = replaceText;
            wordApp.Selection.Find.Execute(Replace: WdReplace.wdReplaceAll);

                //Section headerfooter loop for all types enum WdHeaderFooterIndex. wdHeaderFooterEvenPages/wdHeaderFooterFirstPage/wdHeaderFooterPrimary; 
                foreach (Microsoft.Office.Interop.Word.HeaderFooter footer in footers)
                {
                    Fields fields = footer.Range.Fields;

                    foreach (Field field in fields)
                    {
                        if (field.Type == WdFieldType.wdFieldDate)
                        {
                            field.Select();
                            field.Delete();
                            wordApp.Selection.TypeText("[DATE]");
                        }
                        else if (field.Type == WdFieldType.wdFieldFileName)
                        {
                            field.Select();
                            field.Delete();
                            wordApp.Selection.TypeText("[FILE NAME]");

                        }
                    }
                }
            
        }
    }

}
