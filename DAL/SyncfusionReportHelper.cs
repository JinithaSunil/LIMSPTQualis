using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using ClosedXML.Excel;
using Syncfusion.DocIO.DLS;

namespace LIMS.DAL
{
    public class SyncfusionReportHelper
    {
        public void ReplaceText(WordDocument doc, string findText, string replaceText)
        {
            doc.Replace(
                findText,
                replaceText ?? string.Empty,
                false,  // case sensitive
                true    // whole word
            );
        }

        public void ReplaceHeaderFooter(
            WordDocument doc,
            string placeholder,
            string replacement)
        {
            doc.Replace(
                placeholder,
                replacement ?? string.Empty,
                false,
                true);
        }
        public void InsertResultTable(
            WordDocument doc,
            string placeholder,
            string result)
        {
            TextSelection selection =
                doc.Find(placeholder, false, true);

            if (selection == null)
                return;

            WTextRange textRange =
                selection.GetAsOneRange();

            WParagraph paragraph =
                textRange.OwnerParagraph;

            textRange.Text = "";

            WTable table =
                paragraph.OwnerTextBody.AddTable() as WTable;

            table.ResetCells(2, 1);

            bool isPass =
                result.Trim().ToUpper() == "PASS";

            table[0, 0]
                .AddParagraph()
                .AppendText(
                    (isPass ? "☑ " : "☐ ") + "PASS");

            table[1, 0]
                .AddParagraph()
                .AppendText(
                    (!isPass ? "☑ " : "☐ ") + "FAIL");
        }
        public void InsertImages(
            WordDocument doc,
            string placeholder,
            List<string> imagePaths)
        {
            try
            {
                TextSelection selection =
                    doc.Find(placeholder, false, true);

                if (selection == null ||
                    imagePaths == null ||
                    imagePaths.Count == 0)
                    return;

                WTextRange textRange =
                    selection.GetAsOneRange();

                WParagraph paragraph =
                    textRange.OwnerParagraph;

                // Remove placeholder
                textRange.Text = "";

                IWParagraph imagePara =
                    paragraph.OwnerTextBody.AddParagraph();

                foreach (string imagePath in imagePaths)
                {
                    if (!File.Exists(imagePath))
                        continue;

                    byte[] imageBytes =
                        File.ReadAllBytes(imagePath);

                    IWPicture picture =
                        imagePara.AppendPicture(imageBytes);

                    picture.Width = 150;
                    picture.Height = 150;

                    imagePara.AppendBreak(BreakType.LineBreak);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }
        }

        public void CreateSimpleTable(
            WordDocument doc,
            string placeholder,
            DataTable dt)
        {
            try
            {
                TextSelection selection =
                    doc.Find(placeholder, false, true);

                if (selection == null)
                    return;

                WTextRange textRange =
                    selection.GetAsOneRange();

                if (dt == null || dt.Rows.Count == 0)
                {
                    textRange.Text = "No data available";
                    return;
                }

                // Remove placeholder
                textRange.Text = "";

                WParagraph paragraph =
                    textRange.OwnerParagraph;

                WTable table =
                    paragraph.OwnerTextBody.AddTable() as WTable;

                table.ResetCells(
                    dt.Rows.Count + 1,
                    dt.Columns.Count);

                // Header row
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    WTableCell cell = table[0, col];

                    IWParagraph para =
                        cell.AddParagraph();

                    WTextRange txt =
                        para.AppendText(
                            dt.Columns[col].ColumnName)
                            as WTextRange;

                    txt.CharacterFormat.Bold = true;
                    txt.CharacterFormat.FontSize = 10;

                    para.ParagraphFormat.HorizontalAlignment =
                        HorizontalAlignment.Center;
                }

                // Data rows
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        IWParagraph para =
                            table[row + 1, col]
                            .AddParagraph();

                        para.AppendText(
                            Convert.ToString(
                                dt.Rows[row][col]));

                        para.ParagraphFormat.HorizontalAlignment =
                            HorizontalAlignment.Center;
                    }
                }

                // Optional row height
                foreach (WTableRow row in table.Rows)
                {
                    row.Height = 20;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }
        }

        public void CreateGroupedTable(
            WordDocument doc,
            string placeholder,
            DataTable dt,
            string groupColumn,
            string[] excludedColumns,
            Func<DataRow, string> headingText,
            Func<DataRow, string> descriptionText,
             ref int lastUsedNumber)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0)
                    return;

                TextSelection selection =
                    doc.Find(placeholder, false, true);

                if (selection == null)
                    return;

                WTextRange placeHolderRange =
                    selection.GetAsOneRange();

                placeHolderRange.Text = "";

                WTextBody body =
                    placeHolderRange.OwnerParagraph.OwnerTextBody;

                var groups = dt.AsEnumerable()
                               .GroupBy(r => Convert.ToString(r[groupColumn]))
                               .ToList();

                foreach (var group in groups)
                {
                    lastUsedNumber++;

                    DataTable groupDt = dt.Clone();

                    foreach (DataRow row in group)
                        groupDt.ImportRow(row);

                    DataTable modifiedDt = groupDt.Copy();

                    foreach (string col in excludedColumns)
                    {
                        if (modifiedDt.Columns.Contains(col))
                            modifiedDt.Columns.Remove(col);
                    }

                    // Heading
                    IWParagraph heading =
                        body.AddParagraph();

                    WTextRange headingRange =
                        heading.AppendText(
                            headingText(groupDt.Rows[0]))
                            as WTextRange;

                    headingRange.CharacterFormat.Bold = true;
                    headingRange.CharacterFormat.FontSize = 11;

                    // Description
                    IWParagraph description =
                        body.AddParagraph();

                    description.AppendText(
                        descriptionText(groupDt.Rows[0]));

                    // Table
                    WTable table =
                        body.AddTable() as WTable;

                    table.ResetCells(
                        modifiedDt.Rows.Count + 1,
                        modifiedDt.Columns.Count);

                    // Headers
                    for (int col = 0;
                         col < modifiedDt.Columns.Count;
                         col++)
                    {
                        WTableCell cell =
                            table[0, col];

                        IWParagraph para =
                            cell.AddParagraph();

                        WTextRange txt =
                            para.AppendText(
                                modifiedDt.Columns[col].ColumnName)
                                as WTextRange;

                        txt.CharacterFormat.Bold = true;

                        para.ParagraphFormat.HorizontalAlignment =
                            HorizontalAlignment.Center;
                    }

                    // Data
                    for (int row = 0;
                         row < modifiedDt.Rows.Count;
                         row++)
                    {
                        for (int col = 0;
                             col < modifiedDt.Columns.Count;
                             col++)
                        {
                            IWParagraph para =
                                table[row + 1, col]
                                .AddParagraph();

                            para.AppendText(
                                Convert.ToString(
                                    modifiedDt.Rows[row][col]));

                            para.ParagraphFormat.HorizontalAlignment =
                                HorizontalAlignment.Center;
                        }
                    }

                    body.AddParagraph();
                    body.AddParagraph();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }
        }

        public void CreateSampleDescriptionTable(
            WordDocument doc,
            string placeholder,
            DataTable dt)
        {
            try
            {
                TextSelection selection =
                    doc.Find(placeholder, false, true);

                if (selection == null)
                    return;

                WTextRange textRange =
                    selection.GetAsOneRange();

                if (dt == null || dt.Rows.Count == 0)
                {
                    textRange.Text = "";
                    return;
                }

                textRange.Text = "";

                WParagraph paragraph =
                    textRange.OwnerParagraph;

                WTable table =
                    paragraph.OwnerTextBody.AddTable() as WTable;

                table.ResetCells(
                    dt.Rows.Count + 1,
                    3);

                // Header
                table[0, 0]
                    .AddParagraph()
                    .AppendText("Sr. No.");

                table[0, 1]
                    .AddParagraph()
                    .AppendText("Sample Description");

                table[0, 2]
                    .AddParagraph()
                    .AppendText("Sample ID");

                // Data
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    table[row + 1, 0]
                        .AddParagraph()
                        .AppendText((row + 1).ToString());

                    table[row + 1, 1]
                        .AddParagraph()
                        .AppendText(
                            Convert.ToString(
                                dt.Rows[row]["Sample Description"]));

                    table[row + 1, 2]
                        .AddParagraph()
                        .AppendText(
                            Convert.ToString(
                                dt.Rows[row]["Sample ID"]));
                }

                // Header formatting
                for (int col = 0; col < 3; col++)
                {
                    WTableCell cell =
                        table[0, col];

                    foreach (WParagraph para in cell.Paragraphs)
                    {
                        foreach (ParagraphItem item in para.Items)
                        {
                            if (item is WTextRange txt)
                            {
                                txt.CharacterFormat.Bold = true;
                            }
                        }

                        para.ParagraphFormat.HorizontalAlignment =
                            HorizontalAlignment.Center;
                    }
                }

                foreach (WTableRow row in table.Rows)
                {
                    row.Height = 20;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error creating Sample Description table: "
                    + ex.Message);
            }
        }

        public void CreateChemicalResultTable(
            WordDocument doc,
            string placeholder,
            DataTable dt,
            string groupColumn,
            ref int lastUsedNumber)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;

            TextSelection selection =
                doc.Find(placeholder, false, true);

            if (selection == null)
                return;

            WTextRange placeHolderRange =
                selection.GetAsOneRange();

            WParagraph currentParagraph =
                placeHolderRange.OwnerParagraph;

            placeHolderRange.Text = "";

            var groupedData = dt.AsEnumerable()
                .GroupBy(r => r.Field<string>("Test Parameter"))
                .ToList();

            foreach (var testParamGroup in groupedData)
            {
                lastUsedNumber++;

                DataRow firstRow =
                    testParamGroup.First();

                // Heading
                IWParagraph heading = currentParagraph.OwnerTextBody.AddParagraph();

                WTextRange headingText =
                    heading.AppendText(
                        $"{lastUsedNumber}. {firstRow["Test Parameter"]}")
                        as WTextRange;

                headingText.CharacterFormat.Bold = true;
                headingText.CharacterFormat.FontSize = 11;

                // Method line
                currentParagraph.OwnerTextBody
                    .AddParagraph()
                    .AppendText(
                        $"Test Method: {firstRow["Method*"]}, analyzed by {firstRow["Equipment"]}");

                var elementGroups =
                    testParamGroup.GroupBy(r => new
                    {
                        Element = r.Field<string>("Element"),
                        CAS = r.Field<string>("CAS Number")
                    }).ToList();

                var colorCodes =
                    testParamGroup
                    .Select(r => r["Colour Code"]?.ToString())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                int resultColumns =
                    colorCodes.Count;

                int totalColumns =
                    3 + resultColumns;

                WTable table =
                    currentParagraph.OwnerTextBody
                        .AddTable() as WTable;

                table.ResetCells(
                    elementGroups.Count + 2,
                    totalColumns);

                // Header row
                table[0, 0]
                    .AddParagraph()
                    .AppendText("Element/Compound");

                table[0, 1]
                    .AddParagraph()
                    .AppendText("CAS Number");

                for (int i = 0; i < resultColumns; i++)
                {
                    table[0, i + 2]
                        .AddParagraph()
                        .AppendText(
                            "Result (mg/kg)\n" +
                            colorCodes[i]);
                }

                table[0, totalColumns - 1]
                    .AddParagraph()
                    .AppendText(
                        "Requirement (mg/kg)");

                bool[] detection =
                    new bool[resultColumns];

                int rowIndex = 1;

                foreach (var elementGroup in elementGroups)
                {
                    var first =
                        elementGroup.First();

                    table[rowIndex, 0]
                        .AddParagraph()
                        .AppendText(
                            first["Element"].ToString());

                    table[rowIndex, 1]
                        .AddParagraph()
                        .AppendText(
                            first["CAS Number"].ToString());

                    for (int i = 0; i < resultColumns; i++)
                    {
                        table[rowIndex, i + 2]
                            .AddParagraph()
                            .AppendText("N.D");
                    }

                    foreach (var row in elementGroup)
                    {
                        string code =
                            row["Colour Code"]?.ToString();

                        int idx =
                            colorCodes.IndexOf(code);

                        if (idx >= 0)
                        {
                            string result =
                                row["Result(mg/kg)"].ToString();

                            table[rowIndex, idx + 2]
                                .Paragraphs[0]
                                .Text = result;

                            if (result.Equals("D",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                detection[idx] = true;
                            }
                        }
                    }

                    table[rowIndex, totalColumns - 1]
                        .AddParagraph()
                        .AppendText(
                            first["Requirement"].ToString());

                    rowIndex++;
                }

                // Remark row
                table[rowIndex, 0]
                    .AddParagraph()
                    .AppendText("Remark");

                for (int i = 0; i < resultColumns; i++)
                {
                    table[rowIndex, i + 2]
                        .AddParagraph()
                        .AppendText(
                            detection[i]
                            ? "FAIL"
                            : "PASS");
                }

                // MDL
                List<string> mdlList =
                    new List<string>();

                foreach (var elementGroup in elementGroups)
                {
                    var first =
                        elementGroup.First();

                    string mdl =
                        first["MDL"].ToString();

                    if (!string.IsNullOrWhiteSpace(mdl))
                    {
                        mdlList.Add(
                            $"{first["Element"]}: {mdl} (mg/kg)");
                    }
                }

                if (mdlList.Count > 0)
                {
                    currentParagraph.OwnerTextBody
                        .AddParagraph()
                        .AppendText(
                            "Method Detection Limit = "
                            + string.Join(", ", mdlList));
                }
            }
        }

        public void AdditionalDataAdded(
            WordDocument doc,
            string placeholder,
            string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                TextSelection selection =
                    doc.Find(placeholder, false, true);

                if (selection == null)
                    return;

                WTextRange textRange =
                    selection.GetAsOneRange();

                WParagraph paragraph =
                    textRange.OwnerParagraph;

                textRange.Text = "";

                using (var workbook =
                    new XLWorkbook(filePath))
                {
                    var worksheet =
                        workbook.Worksheet(1);

                    var usedRange =
                        worksheet.RangeUsed();

                    if (usedRange == null)
                        return;

                    int rows =
                        usedRange.RowCount();

                    int cols =
                        usedRange.ColumnCount();

                    WTable table =
                        paragraph.OwnerTextBody
                            .AddTable() as WTable;

                    table.ResetCells(rows, cols);

                    for (int r = 1; r <= rows; r++)
                    {
                        for (int c = 1; c <= cols; c++)
                        {
                            string value =
                                worksheet.Cell(r, c)
                                    .GetValue<string>();

                            WTableCell cell =
                                table[r - 1, c - 1];

                            cell.AddParagraph()
                                .AppendText(value);
                        }
                    }

                    // Font size
                    foreach (WTableRow row in table.Rows)
                    {
                        foreach (WTableCell cell in row.Cells)
                        {
                            foreach (WParagraph para in cell.Paragraphs)
                            {
                                para.ParagraphFormat.HorizontalAlignment =
                                    HorizontalAlignment.Center;

                                foreach (ParagraphItem item in para.Items)
                                {
                                    if (item is WTextRange txt)
                                    {
                                        txt.CharacterFormat.FontSize = 8;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error creating Additional Data table: "
                    + ex.Message);
            }
        }
    }

}