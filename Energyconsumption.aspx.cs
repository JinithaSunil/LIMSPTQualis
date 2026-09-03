using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Word;

using DataTable = System.Data.DataTable;         
using Table = Microsoft.Office.Interop.Word.Table;

namespace LIMS
{
    public partial class Energyconsumption : System.Web.UI.Page
    {
       string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)

        {
            if (!IsPostBack)
            {
                if (Request.QueryString["s"] != null)
                {
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                    lblSampleID.Text = Request.QueryString["s"].ToLower();
                }
                if (Request.QueryString["r"] != null)
                {
                    _cm.ReportID = Request.QueryString["r"];
                    lblReportID.Text = _cm.ReportID;
                }


                LoadPrefilledData();
                // LoadExistingMeasurementData();
                //CreateHeatingTestGrid();

                DataTable existingData = _cm.GetExistingTestResults_Home_RP(lblSampleID.Text, lblReportID.Text);
                
                if (existingData != null && existingData.Rows.Count > 0)
                {
                    //PrefillTestResultsGrid(existingData);
                    //PrefillTestResultsGrid2(existingData);
                    //PrefillAllData(existingData);
                }

                txtbrand.Enabled = false;
                txtstandard.Enabled = false;
                txtproduct.Enabled = false;
                txttestMethodNo.Enabled = false;
                txtworkorder.Enabled = false;
                txtmodel.Enabled = false;

            }
        }
        protected void btnCalculateEC_Click(object sender, EventArgs e)
        {
            try
            {
                double screenSize = Convert.ToDouble(txtScreenSize.Text.Trim());
                double lratio = Convert.ToDouble(txtLratio.Text.Trim());
                double pstandby = Convert.ToDouble(txtPstandby.Text.Trim());

                // Formula: EC = (Screen size × Lratio × 0.43) + Pstandby_passive
                double ec = (screenSize * lratio * 0.43) + pstandby;

                txtEC.Text = ec.ToString("0.00");

                CalculateConclusion();

                lblMessage.Text = "EC calculated successfully!";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error calculating EC. Please enter valid numbers.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        // CALCULATE STAR CONCLUSION BASED ON EC VALUE
        private void CalculateConclusion()
        {
            try
            {
                double ec = Convert.ToDouble(txtEC.Text.Trim());
                string conclusion = "";

                if (ec <= 10.59)
                {
                    conclusion = "Six Stars (******)";
                }
                else if (ec <= 15.24)
                {
                    conclusion = "Five Stars (*****)";
                }
                else if (ec <= 20.33)
                {
                    conclusion = "Four Stars (****)";
                }
                else if (ec <= 25.27)
                {
                    conclusion = "Three Stars (***)";
                }
                else if (ec <= 29.78)
                {
                    conclusion = "Two Stars (**)";
                }
                else
                {
                    conclusion = "Does not meet star rating requirements";
                }

                lblConclusion.Text = conclusion;
            }
            catch
            {
                lblConclusion.Text = "N/A";
            }
        }
        private void LoadPrefilledData()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);

            // Get clone CRF details
            DataTable dtcl = _cm.GetCRFCloneList_HA(sampleId);
            if (dtcl == null || dtcl.Rows.Count == 0)
                return;

            int cloneCRFSampleID = Convert.ToInt32(dtcl.Rows[0]["CloneCRFSampleID"]);
            ViewState["CloneCRFSampleID"] = cloneCRFSampleID.ToString();

            // Get sample + standard info
            DataTable dts = _cm.GetCRFSampleDetails_HA(sampleId, reportId, cloneCRFSampleID);
            if (dts != null && dts.Rows.Count > 0)
            {
                // Fill Standard IDs and Names
                var standardIds = dts.AsEnumerable()
                    .Where(r => !string.IsNullOrEmpty(r["TestStandardID"]?.ToString()))
                    .Select(r => r["TestStandardID"].ToString().Trim())
                    .Distinct()
                    .ToList();

                var standardNames = dts.AsEnumerable()
                    .Where(r => !string.IsNullOrEmpty(r["TestStandard"]?.ToString()))
                    .Select(r => r["TestStandard"].ToString().Trim())
                    .Distinct()
                    .ToList();

                lblstandardID.Text = string.Join(", ", standardIds);
                txtstandard.Text = string.Join(" ; ", standardNames);
            }

            // Load previously saved values (Humidity, Temperature, Dates)
            DataTable saved = GetSavedResults();
            if (saved != null && saved.Rows.Count > 0)
            {
                txthumidity.Text = saved.Rows[0]["Humidity"]?.ToString();
                txttemp.Text = saved.Rows[0]["Temperature"]?.ToString();

                if (DateTime.TryParse(saved.Rows[0]["TestDate"]?.ToString(), out DateTime testDate))
                    txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");

                if (DateTime.TryParse(saved.Rows[0]["TestCompletationDate"]?.ToString(), out DateTime completionDate))
                    txtcompletiondate.Text = completionDate.ToString("yyyy-MM-dd");
            }

            // Get customer + product details
            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_HA(sampleId);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                txtproduct.Text = dt1.Rows[0]["ProductName"]?.ToString();
                txtbrand.Text = dt1.Rows[0]["BrandProduct"]?.ToString();
                txtmodel.Text = dt1.Rows[0]["ProductSerialNo"]?.ToString();
            }

            // Fill test method and work order
            if (dts != null && dts.Rows.Count > 0)
            {
                txttestMethodNo.Text = dts.Rows[0]["TestMethod"]?.ToString();
                txtworkorder.Text = dts.Rows[0]["WorkorderNumber"]?.ToString();
                ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"]?.ToString();
            }
        }
        private DataTable GetSavedResults()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM TestResultsLED WHERE SampleID = @SampleID and ReportID = @ReportID", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                    conn.Close();
                }
            }
            return dt;
        }
        private string GetInputType(DataRow row)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
                return "checkbox";
            else
                return "text";
        }     
        private DataTable GetReportData()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT rpm.ReportParamMapID, trl.Result 
                         FROM TestResultsLED trl
                         INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                         INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                         WHERE trl.SampleID = @SampleID 
                         AND trl.ReportID = @ReportID 
                         AND trl.ClauseID = @ClauseID
                         AND rpm.ReportParamMapID IN (17854, 17855, 17856, 17857, 17858, 17859, 17860)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }

            return dt;
        }
        private void GenerateResultInput(StringBuilder sb, DataRow row, DataTable saved, int clauseID)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
            {
                string resultOptionsBH = row["ResultOptions"]?.ToString()?.Trim() ?? "";
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : new string[0];

                var savedRows = saved.Select($"ClauseID={clauseID}");
                List<string> savedValues = new List<string>();
                foreach (var savedRow in savedRows)
                {
                    string savedResult = savedRow["Result"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(savedResult))
                        savedValues.AddRange(savedResult.Split(',').Select(s => s.Trim()));
                }

                sb.Append("<div class='checkbox-group'>");

                for (int i = 0; i < options.Length; i++)
                {
                    string optValue = options[i].Trim();
                    string optDisplayBH = i < optionsBH.Length ? optionsBH[i].Trim() : optValue;
                    string isChecked = savedValues.Contains(optValue) ? "checked='checked'" : "";

                    if (i % 3 == 0)
                    {
                        // Start a new row every 3 checkboxes
                        sb.Append("<div class='checkbox-row'>");
                    }

                    sb.Append($"<label class='checkbox-item'><input type='checkbox' name='chk_{clauseID}_{optValue}' value='{optValue}' {isChecked}/> {optDisplayBH}</label>");

                    if ((i + 1) % 3 == 0 || i == options.Length - 1)
                    {
                        // Close the row
                        sb.Append("</div>");
                    }
                }

                sb.Append("</div>");

                /*
                sb.Append("<div class='checkbox-group'>");
                for (int i = 0; i < options.Length; i++)
                {
                    string optValue = options[i].Trim();
                    string optDisplayBH = i < optionsBH.Length ? optionsBH[i].Trim() : optValue;
                    string isChecked = savedValues.Contains(optValue) ? "checked='checked'" : "";
                    sb.Append($"<label><input type='checkbox' name='chk_{clauseID}_{optValue}' value='{optValue}' {isChecked}/> {optDisplayBH}</label>");
                }
                sb.Append("</div>");
                */
            }
            else
            {
                var savedRow = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                string savedResult = savedRow != null ? savedRow["Result"]?.ToString() ?? "" : "";
                sb.Append($"<input type='text' name='textresult_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='width:94%;' />");
            }
        }
        private void LoadExistingMeasurementData()
        {
            try
            {
                DataTable dtMeasurement = GetReportDataForMeasurement();

                if (dtMeasurement != null && dtMeasurement.Rows.Count > 0)
                {
                    foreach (DataRow row in dtMeasurement.Rows)
                    {
                        int paramId = Convert.ToInt32(row["ReportParamMapID"]);
                        string value = row["Result"].ToString();

                        switch (paramId)
                        {
                            case 17867:
                                txtScreenSize.Text = value;
                                break;
                            case 17868:
                                txtLratio.Text = value;
                                break;
                            case 17869:
                                txtPstandby.Text = value;
                                break;
                            case 17870:
                                txtEC.Text = value;
                                break;
                        }
                    }
                }
            }
            catch { }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool b1 = false;
                bool b2 = false;

                int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                int ReportID = Convert.ToInt32(Request.QueryString["r"]);
                string sampleIdSeries = ViewState["SampleIDSeriesVS"]?.ToString() ?? "";
                string language = "";

                // Calculate EC before saving
                if (!string.IsNullOrEmpty(txtScreenSize.Text) &&
                    !string.IsNullOrEmpty(txtLratio.Text) &&
                    !string.IsNullOrEmpty(txtPstandby.Text))
                {
                    double screenSize = Convert.ToDouble(txtScreenSize.Text.Trim());
                    double lratio = Convert.ToDouble(txtLratio.Text.Trim());
                    double pstandby = Convert.ToDouble(txtPstandby.Text.Trim());
                    double ec = (screenSize * lratio * 0.43) + pstandby;
                    txtEC.Text = ec.ToString("0.00");
                    CalculateConclusion();
                }

                // SAVE MEASUREMENT DATA
                List<string> measurementParamList = new List<string>();
                List<string> measurementResultList = new List<string>();

                measurementParamList.Add("17867"); measurementResultList.Add(txtScreenSize.Text.Trim());
                measurementParamList.Add("17868"); measurementResultList.Add(txtLratio.Text.Trim());
                measurementParamList.Add("17869"); measurementResultList.Add(txtPstandby.Text.Trim());
                measurementParamList.Add("17870"); measurementResultList.Add(txtEC.Text.Trim());
                measurementParamList.Add("17871"); measurementResultList.Add(lblConclusion.Text); // Save conclusion

                string[] measurementParamMap = measurementParamList.ToArray();
                string[] measurementResults = measurementResultList.ToArray();

                int measModified = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "",
                    sampleId, sampleIdSeries, "", language, ReportID, "", "", "", "", "", "", "", "", "", "",
                    measurementParamMap, measurementResults);

                if (measModified > 0) b1 = true;

                // TABLE DATA (TestData)
                DataTable dtRepoParam = ViewState["TestData"] as DataTable;

                if (dtRepoParam != null && dtRepoParam.Rows.Count > 0)
                {
                    List<string> inputDeviations = new List<string>();
                    List<string> pIdentifiers = new List<string>();
                    List<string> pResults = new List<string>();
                    List<string> deltaPs = new List<string>();
                    List<string> conditionDeltaPs = new List<string>();
                    List<string> decisions = new List<string>();
                    List<string> additionalInfos = new List<string>();

                    foreach (DataRow row in dtRepoParam.Rows)
                    {
                        inputDeviations.Add(row["InputDeviation"].ToString());
                        pIdentifiers.Add(row["PIdentifier"].ToString());
                        pResults.Add(row["PResult"].ToString());
                        deltaPs.Add(row["DeltaP"].ToString());
                        conditionDeltaPs.Add(row["ConditionDeltaP"].ToString());
                        decisions.Add(row["Decision"].ToString());
                        additionalInfos.Add(row["AdditionalInfo"].ToString());
                    }

                    string inputDeviationsCsv = string.Join(",", inputDeviations);
                    string pIdentifiersCsv = string.Join(",", pIdentifiers);
                    string pResultsCsv = string.Join(",", pResults);
                    string deltaPsCsv = string.Join(",", deltaPs);
                    string conditionDeltaPsCsv = string.Join(",", conditionDeltaPs);
                    string decisionsCsv = string.Join(",", decisions);
                    string additionalInfosCsv = string.Join(",", additionalInfos);

                    List<string> reportParamMapList = new List<string>();
                    List<string> resultList = new List<string>();

                    reportParamMapList.Add("17847"); resultList.Add(inputDeviationsCsv);
                    reportParamMapList.Add("17848"); resultList.Add(pIdentifiersCsv);
                    reportParamMapList.Add("17849"); resultList.Add(pResultsCsv);
                    reportParamMapList.Add("17850"); resultList.Add(deltaPsCsv);
                    reportParamMapList.Add("17851"); resultList.Add(conditionDeltaPsCsv);
                    reportParamMapList.Add("17852"); resultList.Add(decisionsCsv);
                    reportParamMapList.Add("17853"); resultList.Add(additionalInfosCsv);

                    string[] reportParamMap = reportParamMapList.ToArray();
                    string[] resultNew = resultList.ToArray();

                    int modified = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "",
                        sampleId, sampleIdSeries, "", language, ReportID, "", "", "", "", "", "", "", "", "", "",
                        reportParamMap, resultNew);

                    if (modified > 0)
                    {
                        b2 = true;
                    }
                }

                // TABLE DATA 2 (TestData2)
                DataTable dtRepoParam2 = ViewState["TestData2"] as DataTable;

                if (dtRepoParam2 != null && dtRepoParam2.Rows.Count > 0)
                {
                    List<string> inputDeviations2 = new List<string>();
                    List<string> pIdentifiers2 = new List<string>();
                    List<string> pResults2 = new List<string>();
                    List<string> deltaPs2 = new List<string>();
                    List<string> conditionDeltaPs2 = new List<string>();
                    List<string> decisions2 = new List<string>();
                    List<string> additionalInfos2 = new List<string>();

                    foreach (DataRow row in dtRepoParam2.Rows)
                    {
                        inputDeviations2.Add(row["InputDeviation"].ToString());
                        pIdentifiers2.Add(row["PIdentifier"].ToString());
                        pResults2.Add(row["PResult"].ToString());
                        deltaPs2.Add(row["DeltaP"].ToString());
                        conditionDeltaPs2.Add(row["ConditionDeltaP"].ToString());
                        decisions2.Add(row["Decision"].ToString());
                        additionalInfos2.Add(row["AdditionalInfo"].ToString());
                    }

                    string inputDeviationsCsv2 = string.Join(",", inputDeviations2);
                    string pIdentifiersCsv2 = string.Join(",", pIdentifiers2);
                    string pResultsCsv2 = string.Join(",", pResults2);
                    string deltaPsCsv2 = string.Join(",", deltaPs2);
                    string conditionDeltaPsCsv2 = string.Join(",", conditionDeltaPs2);
                    string decisionsCsv2 = string.Join(",", decisions2);
                    string additionalInfosCsv2 = string.Join(",", additionalInfos2);

                    List<string> reportParamMapList2 = new List<string>();
                    List<string> resultList2 = new List<string>();

                    reportParamMapList2.Add("17854"); resultList2.Add(inputDeviationsCsv2);
                    reportParamMapList2.Add("17855"); resultList2.Add(pIdentifiersCsv2);
                    reportParamMapList2.Add("17856"); resultList2.Add(pResultsCsv2);
                    reportParamMapList2.Add("17857"); resultList2.Add(deltaPsCsv2);
                    reportParamMapList2.Add("17858"); resultList2.Add(conditionDeltaPsCsv2);
                    reportParamMapList2.Add("17859"); resultList2.Add(decisionsCsv2);
                    reportParamMapList2.Add("17860"); resultList2.Add(additionalInfosCsv2);

                    string[] reportParamMap2 = reportParamMapList2.ToArray();
                    string[] resultNew2 = resultList2.ToArray();

                    int modified2 = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "",
                        sampleId, sampleIdSeries, "", language, ReportID, "", "", "", "", "", "", "", "", "", "",
                        reportParamMap2, resultNew2);

                    if (modified2 > 0) b1 = true;
                }

                // GENERATE REPORT
                if (b1 == true || b2 == true)
                {
                    string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
                    string pathout = Server.MapPath("~/Reports/");
                    string templatePath = Server.MapPath("~/InputFiles/HATestRecord_1.docx");

                    if (!Directory.Exists(pathout))
                        Directory.CreateDirectory(pathout);

                    string outputDocxPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".docx");
                    string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

                    Application wordApp = null;
                    Document doc = null;

                    try
                    {
                        wordApp = new Application();
                        wordApp.Visible = false;

                        if (File.Exists(templatePath))
                        {
                            doc = wordApp.Documents.Open(templatePath);
                            ReplaceTemplateVariables(doc);
                        }
                        else
                        {
                            doc = wordApp.Documents.Add();
                        }

                        // CREATE MEASUREMENT RESULTS TABLE
                        DataTable dtMeasurement = GetReportDataForMeasurement();
                        CreateMeasurementResultsTable(wordApp, doc, "{MeasurementResults}", dtMeasurement);

                        // CREATE STAR CATEGORY TABLE
                        CreateStarCategoryTable(wordApp, doc, "{StarCategory}");

                        // CREATE CONCLUSION TABLE
                        CreateConclusionTable(wordApp, doc, "{Conclusion}", lblConclusion.Text);

                        // YOUR EXISTING TABLES
                        DataTable dtReport = GetReportData();
                        CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                        DataTable dtReportT2 = GetReportDataForT2();
                        CreateReportTableT2(wordApp, doc, "{reporttable}", dtReportT2);

                        DataTable dtReportT3 = GetReportDataForT3();
                        CreateReportTableT3(wordApp, doc, "{HeatingTestTable}", dtReportT3);

                        doc.SaveAs2(outputDocxPath);
                        doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);

                        Response.Clear();
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                        Response.TransmitFile(outputPdfPath);
                        Response.Flush();
                        Response.End();
                    }
                    catch (Exception ex)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                            "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    finally
                    {
                        try
                        {
                            if (doc != null)
                            {
                                doc.Close(false);
                                Marshal.ReleaseComObject(doc);
                                doc = null;
                            }
                        }
                        catch { }

                        try
                        {
                            if (wordApp != null)
                            {
                                wordApp.Quit();
                                Marshal.ReleaseComObject(wordApp);
                                wordApp = null;
                            }
                        }
                        catch { }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                        "alert('Failed to save data.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Error: {ex.Message}');", true);
            }
        }
        private void CreateMeasurementResultsTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                return;

            range.Text = "Measurement Results";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            range.ParagraphFormat.SpaceBefore = 0;
            range.ParagraphFormat.SpaceAfter = 0;
            range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
            range.ParagraphFormat.LineSpacing = 14;

            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            Dictionary<int, int> rowMapping = new Dictionary<int, int>
        {
            { 17867, 0 },
            { 17868, 1 },
            { 17869, 2 },
            { 17870, 3 }
        };

            string[] measurementValues = new string[4] { "", "", "", "" };

            foreach (DataRow row in dt.Rows)
            {
                int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                if (rowMapping.ContainsKey(reportParamMapID))
                {
                    int rowIndex = rowMapping[reportParamMapID];
                    measurementValues[rowIndex] = row["Result"].ToString();
                }
            }

            Table table = doc.Tables.Add(tableRange, 6, 2);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;
            table.PreferredWidth = wordApp.InchesToPoints(6.5f);
            table.Columns[1].Width = wordApp.InchesToPoints(4.0f);
            table.Columns[2].Width = wordApp.InchesToPoints(2.5f);

            table.Cell(1, 1).Merge(table.Cell(1, 2));
            Cell headerCell = table.Cell(1, 1);
            headerCell.Range.Text = "Measurement Results\r\nMeasurement Results";
            headerCell.Range.Bold = 1;
            headerCell.Range.Font.Size = 11;
            headerCell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            headerCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Rows[1].Height = wordApp.InchesToPoints(0.5f);

            Cell paramHeader = table.Cell(2, 1);
            paramHeader.Range.Text = "Parameter\r\nParameters";
            paramHeader.Range.Bold = 1;
            paramHeader.Range.Font.Size = 10;
            paramHeader.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            paramHeader.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            paramHeader.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Cell resultHeader = table.Cell(2, 2);
            resultHeader.Range.Text = "Results\r\nResult";
            resultHeader.Range.Bold = 1;
            resultHeader.Range.Font.Size = 10;
            resultHeader.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            resultHeader.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            resultHeader.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Rows[2].Height = wordApp.InchesToPoints(0.4f);

            string[] parameters = {
            "Screen size [cm2]",
            "Lratio [%]",
            "Pstandby_passive [Watt]",
            "EC [Wh/cm2/year]"
        };

            for (int i = 0; i < 4; i++)
            {
                int rowNum = i + 3;
                table.Rows[rowNum].Height = wordApp.InchesToPoints(0.3f);

                Cell paramCell = table.Cell(rowNum, 1);
                paramCell.Range.Text = parameters[i];
                paramCell.Range.Font.Size = 10;
                paramCell.Range.Font.Italic = 1;
                paramCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                paramCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Cell resultCell = table.Cell(rowNum, 2);
                resultCell.Range.Text = measurementValues[i];
                resultCell.Range.Font.Size = 10;
                resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                if (i == 3)
                {
                    resultCell.Shading.BackgroundPatternColor = WdColor.wdColorLightYellow;
                    resultCell.Range.Font.Bold = 1;
                }
            }
        }
        private void CreateStarCategoryTable(Application wordApp, Document doc, string placeholder)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                return;

            range.Text = "Star Category";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            Table table = doc.Tables.Add(tableRange, 7, 2);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(6.5f);

            table.Cell(1, 1).Merge(table.Cell(1, 2));
            Cell headerCell = table.Cell(1, 1);
            headerCell.Range.Text = "Star category\r\nStar category";
            headerCell.Range.Bold = 1;
            headerCell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            string[] stars = { "**", "***", "****", "*****", "******" };
            string[] values = { "≤ 29.78", "≤ 25.27", "≤ 20.33", "≤ 15.24", "≤ 10.59" };

            for (int i = 0; i < 5; i++)
            {
                table.Cell(i + 2, 1).Range.Text = stars[i];
                table.Cell(i + 2, 1).Range.Font.Bold = 1;
                table.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                table.Cell(i + 2, 2).Range.Text = values[i];
                table.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            }
        }
        private void CreateConclusionTable(Application wordApp, Document doc, string placeholder, string conclusionText)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                return;

            range.Text = "Conclusion";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            Table table = doc.Tables.Add(tableRange, 2, 1);
            table.Borders.Enable = 1;
            table.PreferredWidth = wordApp.InchesToPoints(6.5f);

            Cell headerCell = table.Cell(1, 1);
            headerCell.Range.Text = "Conclusion\r\nConclusion";
            headerCell.Range.Bold = 1;
            headerCell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            headerCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            Cell conclusionCell = table.Cell(2, 1);
            conclusionCell.Range.Text = conclusionText;
            conclusionCell.Range.Font.Bold = 1;
            conclusionCell.Range.Font.Italic = 1;
            conclusionCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            conclusionCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Rows[2].Height = wordApp.InchesToPoints(0.6f);
        }
        private DataTable GetReportDataForMeasurement()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT rpm.ReportParamMapID, trl.Result 
                     FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE trl.SampleID = @SampleID 
                     AND trl.ReportID = @ReportID 
                     AND trl.ClauseID = @ClauseID
                     AND rpm.ReportParamMapID IN (17867, 17868, 17869, 17870)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);

                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                }
                catch { }
            }

            return dt;
        }    
        private DataTable GetReportDataForT2()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT rpm.ReportParamMapID, trl.Result 
                     FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE trl.SampleID = @SampleID 
                     AND trl.ReportID = @ReportID 
                     AND trl.ClauseID = @ClauseID
                     AND rpm.ReportParamMapID IN (17847, 17848, 17849, 17850, 17851, 17852, 17853)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }
        private DataTable GetReportDataForT3()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT rpm.ReportParamMapID, trl.Result 
                     FROM TestResultsLED trl
                     INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                     INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                     WHERE trl.SampleID = @SampleID 
                     AND trl.ReportID = @ReportID 
                     AND trl.ClauseID = @ClauseID
                     AND rpm.ReportParamMapID IN (17854, 17855, 17856, 17857, 17858, 17859, 17860)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                cmd.Parameters.AddWithValue("@ClauseID", 0);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }
        private DataTable GetImageData()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);
            return _cm.GetLabAnalysisImages(sampleId, reportId);
        }
        private void InsertLabAnalysisImages(Document doc, string placeholder, string imageNames)
        {
            if (string.IsNullOrEmpty(imageNames)) return;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute()) return;

            range.Text = "";

            string imagePath = Server.MapPath("~/SampleImages/");
            string[] images = imageNames.Split(',');

            foreach (string imageName in images)
            {
                string fullPath = Path.Combine(imagePath, imageName.Trim());
                if (File.Exists(fullPath))
                {
                    var inlineShape = range.InlineShapes.AddPicture(fullPath);
                    inlineShape.Width = 400;
                    inlineShape.Height = 300;
                    range.InsertParagraphAfter();
                    range.Collapse(WdCollapseDirection.wdCollapseEnd);
                }
            }
        }
        protected void btnUploadImages_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuLabAnalysis.HasFiles)
                {
                    string sampleId = lblSampleID.Text;
                    string uploadPath = Server.MapPath("~/SampleImages/");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    int sequence = 1;
                    List<string> uploadedImages = new List<string>();

                    foreach (HttpPostedFile file in fuLabAnalysis.PostedFiles)
                    {
                        string extension = Path.GetExtension(file.FileName).ToLower();
                        string fileName = $"labanalysis_{sampleId}_{sequence}{extension}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        file.SaveAs(filePath);
                        uploadedImages.Add(fileName);
                        sequence++;
                    }

                    ViewState["UploadedImages"] = string.Join(",", uploadedImages);
                    ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Images uploaded successfully!');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }
        }
        private void ClearExistingResults()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM TestResultsLED WHERE SampleID=@SampleID AND ReportID = @ReportID", conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }   
        private void ReplaceTextInDocument(Document doc, string placeholder, string value)
        {
            Find findObject = doc.Content.Find;
            findObject.ClearFormatting();
            findObject.Text = placeholder;
            findObject.Replacement.ClearFormatting();
            findObject.Replacement.Text = value ?? "";

            object replaceAll = WdReplace.wdReplaceAll;
            findObject.Execute(
                FindText: placeholder,
                ReplaceWith: value ?? "",
                Replace: replaceAll
            );
        }
        /* T2 Start */
        private void CreateReportTableT2(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // Set heading
            range.Text = "IEC 62087-1:2015, IEC 61087-2:2023 and IEC 62087-3:2023";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            // Remove all spacing
            range.ParagraphFormat.SpaceBefore = 0;
            range.ParagraphFormat.SpaceAfter = 0;
            range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
            range.ParagraphFormat.LineSpacing = 14;

            // Create table immediately after heading
            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Create a 2-column table for the main content
            Table mainTable = doc.Tables.Add(tableRange, 1, 2);
            mainTable.Borders.Enable = 1;
            mainTable.AllowAutoFit = false;

            float totalWidthInches = 6.5f;
            mainTable.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            // Set column widths - left column smaller, right column larger
            mainTable.Columns[1].Width = wordApp.InchesToPoints(2.0f); // Left column
            mainTable.Columns[2].Width = wordApp.InchesToPoints(4.5f); // Right column

            // Set header row
            Cell leftHeader = mainTable.Cell(1, 1);
            leftHeader.Range.Text = "Needs - Test";
            leftHeader.Range.Bold = 1;
            leftHeader.Range.Font.Size = 10;
            leftHeader.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            leftHeader.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            Cell rightHeader = mainTable.Cell(1, 2);
            rightHeader.Range.Text = "Results - Description";
            rightHeader.Range.Bold = 1;
            rightHeader.Range.Font.Size = 10;
            rightHeader.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
            rightHeader.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            // Remove spacing in header cells
            leftHeader.Range.ParagraphFormat.SpaceBefore = 0;
            leftHeader.Range.ParagraphFormat.SpaceAfter = 0;
            rightHeader.Range.ParagraphFormat.SpaceBefore = 0;
            rightHeader.Range.ParagraphFormat.SpaceAfter = 0;

            // Prepare the data for right column from database
            Dictionary<string, string> resultsData = new Dictionary<string, string>();

            // Map parameter IDs to field names for right column data
            Dictionary<int, string> paramMapping = new Dictionary<int, string>
    {
        { 13721, "Testing Officer Name" },
        { 13722, "Name of Examining Officer" },
        { 13723, "Name of Person in Charge of Testing Laboratory (Head of Lab)" },
        { 17918, "Brand" },
        { 17919, "Model / Type" },
        { 17902, "Version or Serial Number" },
        { 17903, "Voltage" },
        { 17904, "Frequency" },
        { /* Add Rated Power ID here if exists */ 0, "Rated Power" },
        { 17905, "Rated Current" },
        { 17906, "Firmware or Software Version" },
        { 17907, "HDMI Port Features" },
        { 17908, "USB Port Features" },
        { 17909, "AV Input Features" },
        { 17910, "Internet Network Features" },
        { 17911, "MDD (Motion-Based Dimming) Feature" },
        { 17912, "ABC (Automatic Brightness Control) feature" },
        { 17913, "Retail Mode Features" },
        { 17914, "Video Signal Types" },
        { 17915, "Battery Component Availability" },
        { 17916, "Quick Start Feature" }
    };

            // Fill dictionary with data from database
            foreach (DataRow row in dt.Rows)
            {
                int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                string result = row["Result"].ToString();

                if (paramMapping.ContainsKey(reportParamMapID))
                {
                    resultsData[paramMapping[reportParamMapID]] = result;
                }
            }

            // Define the rows for the table based on your image
            List<string> leftColumnItems = new List<string>
    {
        "TEST LABORATORY",
        "Test Date (day/month/year)",
        "Testing Laboratory Name",
        "Testing Laboratory Location",
        "Testing Officer Name",
        "Name of Examining Officer",
        "Name of Person in Charge of Testing Laboratory (Head of Lab)",
        "PRODUCT SPECIFICATIONS",
        "Brand",
        "Model / Type",
        "Version or Serial Number",
        "Voltage",
        "Frequency",
        "Rated Power",
        "Rated Current",
        "Firmware or Software Version",
        "HDMI Port Features",
        "USB Port Features",
        "AV Input Features",
        "Internet Network Features",
        "MDD (Motion-Based Dimming) Feature",
        "ABC (Automatic Brightness Control) feature",
        "Retail Mode Features",
        "Video Signal Types",
        "Battery Component Availability",
        "Quick Start Feature"
    };

            // Add rows to the table
            foreach (string leftItem in leftColumnItems)
            {
                mainTable.Rows.Add();

                int lastRow = mainTable.Rows.Count;

                // Set left cell
                Cell leftCell = mainTable.Cell(lastRow, 1);
                leftCell.Range.Text = leftItem;
                leftCell.Range.Font.Size = 10;

                // Check if this is a section header
                if (leftItem.Contains("TEST LABORATORY") || leftItem.Contains("PRODUCT SPECIFICATIONS"))
                {
                    leftCell.Range.Bold = 1;
                    leftCell.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                }

                // Set right cell with data from database or empty
                Cell rightCell = mainTable.Cell(lastRow, 2);

                // Map the left item to get the corresponding data
                string rightValue = "";
                if (leftItem == "Test Date (day/month/year)")
                {
                    rightValue = "June 11 - 12, 2025"; // Hardcoded or from database
                }
                else if (leftItem == "Testing Laboratory Name")
                {
                    rightValue = "PT Qualis Indonesia"; // Hardcoded or from database
                }
                else if (leftItem == "Testing Laboratory Location")
                {
                    rightValue = "JL Pajajaran No. 17, Gandasari Village, Jatiuwung District, Tangerang, Banten, Indonesia"; // Hardcoded or from database
                }
                else
                {
                    // Get from resultsData dictionary
                    if (resultsData.ContainsKey(leftItem))
                    {
                        rightValue = resultsData[leftItem];
                    }
                }

                rightCell.Range.Text = rightValue;
                rightCell.Range.Font.Size = 10;

                // Apply formatting for section headers
                if (leftItem.Contains("TEST LABORATORY") || leftItem.Contains("PRODUCT SPECIFICATIONS"))
                {
                    rightCell.Range.Bold = 1;
                    rightCell.Shading.BackgroundPatternColor = WdColor.wdColorGray15;
                }

                // Remove spacing
                leftCell.Range.ParagraphFormat.SpaceBefore = 0;
                leftCell.Range.ParagraphFormat.SpaceAfter = 0;
                rightCell.Range.ParagraphFormat.SpaceBefore = 0;
                rightCell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Remove table spacing
            mainTable.Range.ParagraphFormat.SpaceAfter = 0;
            mainTable.Range.ParagraphFormat.SpaceBefore = 0;
        }
        /* T2 Start */
        /* T3 START */
        private void CreateReportTableT3(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute())
                throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // Set heading
            range.Text = "TABLE: Heating Test";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            // Remove all spacing
            range.ParagraphFormat.SpaceBefore = 0;
            range.ParagraphFormat.SpaceAfter = 0;
            range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
            range.ParagraphFormat.LineSpacing = 14;

            // Create table immediately after heading
            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // Define the mapping between ReportParamMapID and column positions
            Dictionary<int, int> columnMapping = new Dictionary<int, int>
        {
            { 17854, 0 }, // Input deviation from/on
            { 17855, 1 }, // P Identifier
            { 17856, 2 }, // P Result
            { 17857, 3 }, // ∆P
            { 17858, 4 }, // Condition∆P
            { 17859, 5 }, // Decision
            { 17860, 6 }  // Additional information
        };

            // Parse data based on ReportParamMapID
            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 7; i++)
            {
                parsedColumns.Add(new string[0]);
            }

            foreach (DataRow row in dt.Rows)
            {
                int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                if (columnMapping.ContainsKey(reportParamMapID))
                {
                    int columnIndex = columnMapping[reportParamMapID];
                    string[] values = row["Result"].ToString().Split(',');
                    parsedColumns[columnIndex] = values;
                }
            }

            // Determine number of rows based on the maximum length of any column
            int numRows = parsedColumns.Max(col => col.Length);
            int numCols = 7;

            // Create table using tableRange
            Table table = doc.Tables.Add(tableRange, numRows + 1, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.5f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            // Adjusted column widths
            float[] colWidths = { 1.1f, 0.8f, 0.8f, 0.8f, 1.0f, 1.0f, 1.0f };
            for (int i = 0; i < numCols; i++)
            {
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
            }

            string[] headers = {
            "Input deviation\r\nfrom/on",
            "P Identifier\r\n(W)",
            "P Result\r\n(W)",
            "∆P\r\n(W ; %)",
            "Condition∆P\r\n(W ; %)",
            "Decision",
            "Additional\r\ninformation"
        };

            // Set the header row height to accommodate two lines
            table.Rows[1].Height = wordApp.InchesToPoints(0.4f);
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Remove all spacing in header cells
                cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                cell.Range.ParagraphFormat.LineSpacing = 10;
                cell.Range.ParagraphFormat.SpaceBefore = 0;
                cell.Range.ParagraphFormat.SpaceAfter = 0;
            }

            // Fill data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                table.Rows[rowIndex + 2].Height = wordApp.InchesToPoints(0.25f);
                table.Rows[rowIndex + 2].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 2, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10;
                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    cell.Shading.BackgroundPatternColor = WdColor.wdColorWhite;

                    // Remove spacing in data cells
                    cell.Range.ParagraphFormat.SpaceBefore = 0;
                    cell.Range.ParagraphFormat.SpaceAfter = 0;
                }
            }

            // Remove table spacing
            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;
        }
       
    
        //protected void gvPowerInput_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "DeleteRow")
        //    {
        //        int rowIndex = Convert.ToInt32(e.CommandArgument);
        //        DataTable dt = ViewState["PowerInputData"] as DataTable;

        //        if (dt != null && rowIndex < dt.Rows.Count)
        //        {
        //            dt.Rows.RemoveAt(rowIndex);
        //            ViewState["PowerInputData"] = dt;

        //            gvPowerInput.DataSource = dt;
        //            gvPowerInput.DataBind();
        //        }
        //    }
        //}

        /*T4 END */
        private void CreateBasicTemplate(Document doc)
        {
            // Add title
            Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = "Test Results Report\n\n";
            title.Range.Font.Size = 16;
            title.Range.Font.Bold = 1;
            title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            // Add sample info
            Paragraph info = doc.Paragraphs.Add();
            info.Range.Text = "Sample ID: SMP/1001-0125\nTest Date: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n\n";

            // Add placeholder for data table
            Paragraph placeholder = doc.Paragraphs.Add();
            placeholder.Range.Text = "{TestingAndMeasurement}";
        }
        private void ReplaceTemplateVariables(Document doc)
        {
            DataTable DisResget = _cm.GetTestResultLED_DISTINCT(lblSampleID.Text, lblReportID.Text);

            if (DisResget.Rows.Count > 0)
            {
                string equipmentIds = DisResget.Rows[0]["EquipmentIds"].ToString();
                List<string> equipmentNames = GetEquipmentNamesAsList(equipmentIds);

                DataRow row = DisResget.Rows[0];

                var replacements = new Dictionary<string, string>
                {
                    {"{WorkorderNumber}", GetSafeString(row, "WorkOrderNo") },
                    {"{Product}", GetSafeString(row, "ProductName")},
                    {"{Brand}", GetSafeString(row, "ProductBrand")},
                    {"{TestStartDate}", FormatDate(GetSafeString(row, "TestDate"))}, // Format date to DD/MM/YYYY
                    {"{TestCompletiondate}", FormatDate(GetSafeString(row, "TestCompletationDate"))}, // Format date to DD/MM/YYYY
                    {"{Temperature}", GetSafeString(row, "Temperature")},
                    {"{Humidity}", GetSafeString(row, "Humidity") },
                    {"{Method}", GetSafeString(row, "TestMethod")},
                    {"{Standard}", GetSafeString(row, "StandardName")},
                    {"{Model}", GetSafeString(row, "ProductModel")},
                };

                // Replace normal placeholders safely
                foreach (var replacement in replacements)
                {
                    if (!string.IsNullOrEmpty(replacement.Key))
                    {
                        Range range = doc.Content;
                        Find find = range.Find;
                        find.ClearFormatting();
                        find.Text = replacement.Key;
                        find.Replacement.ClearFormatting();
                        find.Replacement.Text = replacement.Value ?? string.Empty;
                        find.Execute(Replace: WdReplace.wdReplaceAll);
                    }
                }

                // Replace equipment placeholder separately (to avoid long string error)
                ReplaceEquipmentPlaceholder(doc, equipmentNames);
            }
        }
        private string FormatDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return string.Empty;

            try
            {
                DateTime date;
                if (DateTime.TryParse(dateString, out date))
                {
                    return date.ToString("dd/MM/yyyy");
                }
                return dateString; // Return original if parsing fails
            }
            catch
            {
                return dateString; // Return original if any error
            }
        }
        private void ReplaceEquipmentPlaceholder(Document doc, List<string> equipmentNames)
        {
            if (equipmentNames == null || equipmentNames.Count == 0)
                return;

            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = "{Equipment}";

            if (find.Execute())
            {
                Range equipmentRange = find.Parent as Range;
                if (equipmentRange != null)
                {
                    equipmentRange.Text = "";
                    for (int i = 0; i < equipmentNames.Count; i++)
                    {
                        equipmentRange.InsertAfter($"{i + 1}. {equipmentNames[i]}\r\n");
                        equipmentRange = doc.Range(equipmentRange.End, equipmentRange.End);
                    }
                }
            }
        }
        private List<string> GetEquipmentNamesAsList(string equipmentIds)
        {
            var equipmentList = new List<string>();

            if (string.IsNullOrEmpty(equipmentIds))
                return equipmentList;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string[] ids = equipmentIds.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToArray();

                    if (ids.Length == 0)
                        return equipmentList;

                    string parameterPlaceholders = string.Join(",", ids.Select((_, index) => $"@id{index}"));

                    string query = $@"SELECT * FROM Calibration_Equipment WHERE EpID IN ({parameterPlaceholders}) ORDER BY EpID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        for (int i = 0; i < ids.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
                        }

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                equipmentList.Add(reader["Equipment"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting equipment names: {ex.Message}");
            }

            return equipmentList;
        }
        // Helper method for safe column access
        private string GetSafeString(DataRow row, string columnName)
        {
            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                {
                    return row[columnName].ToString();
                }
                return "[Not Found]"; // or return string.Empty;
            }
            catch
            {
                return "[Error]";
            }
        }  
        private void CreateReportTable(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            Range range = doc.Content;
            Find find = range.Find;
            find.ClearFormatting();
            find.Text = placeholder;

            if (!find.Execute()) throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

            // Count actual rows needed (skip headers for row calculation)
            int dataRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() != "1");
            int headerRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() == "1");
            int totalRows = 1 + dataRows + headerRows; // 1 for main header + data rows + section headers

            Table table = doc.Tables.Add(range, totalRows, 4);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float availableWidthInches = 6.27f;
            table.PreferredWidth = wordApp.InchesToPoints(availableWidthInches);

            // Set column widths proportionally
            float totalOriginalWidth = 0.6f + 3.4f + 2.0f + 1.0f; // 7.0 inches total
            float scaleFactor = availableWidthInches / totalOriginalWidth;

            table.Columns[1].Width = wordApp.InchesToPoints(0.7f * scaleFactor);  // Clause
            table.Columns[2].Width = wordApp.InchesToPoints(3.3f * scaleFactor);  // Requirements
            table.Columns[3].Width = wordApp.InchesToPoints(2.0f * scaleFactor);  // Result
            table.Columns[4].Width = wordApp.InchesToPoints(1.0f * scaleFactor);  // Remark

            // Create main header row
            string[] headers = { "Klausul\nClause", "Persyaratan\nRequirements", "Hasil\nResult", "*Keterangan\nRemark" };
            for (int col = 1; col <= 4; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10;
                cell.Range.Shading.BackgroundPatternColor = WdColor.wdColorGray25;
                cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                cell.Borders.Enable = 1;
            }
            table.Rows[1].HeadingFormat = -1;

            int currentRow = 2;
            foreach (DataRow row in dt.Rows)
            {
                if (row["isHeader"].ToString() == "1")
                {
                    // Section header row - matches original design
                    Cell clauseCell = table.Cell(currentRow, 1);
                    clauseCell.Range.Text = row["ClauseNumber"]?.ToString() ?? "";
                    clauseCell.Range.Bold = 1;
                    clauseCell.Range.Font.Size = 10;
                    clauseCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    clauseCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    clauseCell.Borders.Enable = 1;

                    // Get Remark cell reference BEFORE merging
                    Cell remarkCell = table.Cell(currentRow, 4);

                    // Try multiple possible column names for Remark
                    string remarkText = "";
                    if (dt.Columns.Contains("Remark"))
                        remarkText = row["Remark"]?.ToString() ?? "";
                    else if (dt.Columns.Contains("Remarks"))
                        remarkText = row["Remarks"]?.ToString() ?? "";
                    else if (dt.Columns.Contains("Keterangan"))
                        remarkText = row["Keterangan"]?.ToString() ?? "";

                    // Merge columns 2-3 for the section title (NOT column 4 - keep Remark separate)
                    Cell titleCell = table.Cell(currentRow, 2);
                    titleCell.Merge(table.Cell(currentRow, 3));

                    string clauseTitleBH = row["ClauseTitle_BH"]?.ToString() ?? "";
                    string clauseTitle = row["ClauseTitle"]?.ToString() ?? "";
                    titleCell.Range.Text = $"{clauseTitleBH}\n{clauseTitle}";

                    // Format the merged cell
                    Range titleRange = titleCell.Range;
                    titleRange.Bold = 1;
                    titleRange.Font.Size = 10;
                    titleRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    titleCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    titleCell.Borders.Enable = 1;

                    // Underline the BH title part
                    if (!string.IsNullOrEmpty(clauseTitleBH))
                    {
                        Range bhRange = titleRange.Duplicate;
                        bhRange.End = bhRange.Start + clauseTitleBH.Length;
                        bhRange.Font.Underline = WdUnderline.wdUnderlineSingle;
                    }

                    // Format Remark cell for header rows (reference obtained before merging)
                    remarkCell.Range.Text = remarkText;
                    remarkCell.Range.Bold = 1;
                    remarkCell.Range.Font.Size = 10;
                    remarkCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    remarkCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
                    remarkCell.Borders.Enable = 1;

                    currentRow++;
                }
                else
                {
                    // Data row
                    Cell clauseNumCell = table.Cell(currentRow, 1);
                    clauseNumCell.Range.Text = row["ClauseNumber"]?.ToString() ?? "";
                    clauseNumCell.Range.Font.Size = 10;
                    clauseNumCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    clauseNumCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    clauseNumCell.Borders.Enable = 1;

                    // Requirements cell
                    Cell reqCell = table.Cell(currentRow, 2);
                    string reqBH = row["Requirement_BH"]?.ToString() ?? "";
                    string reqEN = row["Requirement"]?.ToString() ?? "";

                    // Set the full text (reqBH + newline + reqEN)
                    reqCell.Range.Text = $"{reqBH}\n{reqEN}";

                    // Set overall formatting
                    reqCell.Range.Font.Size = 10;
                    reqCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    reqCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    reqCell.Borders.Enable = 1;

                    // Apply underline only to reqBH
                    if (!string.IsNullOrEmpty(reqBH))
                    {
                        Range bhRange = reqCell.Range.Duplicate;
                        bhRange.End = bhRange.Start + reqBH.Length;
                        bhRange.Font.Underline = WdUnderline.wdUnderlineSingle;
                    }

                    // Result cell - Enhanced for checkbox layout
                    Cell resultCell = table.Cell(currentRow, 3);
                    string inputType = GetInputTypeFromClause(row);
                    string resultText = FormatResultsForExport(
                        inputType,
                        row["ResultOptions"]?.ToString(),
                        row["ResultOptions_BH"]?.ToString(),
                        row["Results"]?.ToString()
                    );

                    resultCell.Range.Text = resultText;

                    // Set different font sizes based on input type
                    if (inputType == "checkbox")
                    {
                        resultCell.Range.Font.Size = 11; // 
                    }
                    else
                    {
                        resultCell.Range.Font.Size = 10; // Normal font for other types
                    }

                    resultCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    resultCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
                    resultCell.Borders.Enable = 1;

                    // Set specific paragraph formatting for checkbox layout
                    resultCell.Range.ParagraphFormat.SpaceAfter = 3;
                    resultCell.Range.ParagraphFormat.SpaceBefore = 3;
                    resultCell.Range.ParagraphFormat.LineSpacing = wordApp.LinesToPoints(1.15f);

                    // Set tab stops for better checkbox alignment (if needed)
                    if (inputType == "checkbox")
                    {
                        resultCell.Range.ParagraphFormat.TabStops.ClearAll();
                        resultCell.Range.ParagraphFormat.TabStops.Add(wordApp.InchesToPoints(0.75f));

                        // Increase checkbox size even more (make checkboxes slightly larger than text)
                        Range cellRange = resultCell.Range;
                        string cellText = cellRange.Text;

                        // Find and enlarge each checkbox character
                        for (int charIndex = 0; charIndex < cellText.Length; charIndex++)
                        {
                            char currentChar = cellText[charIndex];
                            if (currentChar == '☐' || currentChar == '☒')
                            {
                                Range charRange = cellRange.Duplicate;
                                charRange.Start = cellRange.Start + charIndex;
                                charRange.End = charRange.Start + 1;
                                charRange.Font.Size = 12; // Checkbox size (slightly larger than text)
                            }
                        }
                    }

                    // Remark cell - FIXED: Ensure remark data is displayed
                    Cell remarkCell = table.Cell(currentRow, 4);

                    // Try multiple possible column names for Remark
                    string remarkText = "";
                    if (dt.Columns.Contains("Remark"))
                        remarkText = row["Remark"]?.ToString() ?? "";
                    else if (dt.Columns.Contains("Remarks"))
                        remarkText = row["Remarks"]?.ToString() ?? "";
                    else if (dt.Columns.Contains("Keterangan"))
                        remarkText = row["Keterangan"]?.ToString() ?? "";

                    // Debug: Log if remark is empty
                    System.Diagnostics.Debug.WriteLine($"Row {currentRow}: Remark = '{remarkText}'");

                    remarkCell.Range.Text = remarkText;
                    remarkCell.Range.Font.Size = 10;
                    remarkCell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    remarkCell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop;
                    remarkCell.Borders.Enable = 1;

                    currentRow++;
                }
            }
        }
        private string GetInputTypeFromClause(DataRow row)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            bool hasResultOptions = !string.IsNullOrEmpty(resultOptions) && resultOptions.Trim() != "";

            if (hasResultOptions)
                return "checkbox";
            else
                return "text";
        }
        private string FormatResultsForExport(string inputType, string resultOptions, string resultOptionsBH, string selectedResults)
        {
            if (inputType == "checkbox" && !string.IsNullOrEmpty(resultOptions))
            {
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : options;
                string[] selected = !string.IsNullOrEmpty(selectedResults) ? selectedResults.Split(',') : new string[0];
                StringBuilder sb = new StringBuilder();

                // Arrange checkboxes in pairs (2 per row)
                for (int i = 0; i < options.Length; i += 2)
                {
                    // First checkbox in the pair
                    string option1 = options[i].Trim();
                    string optionDisplay1 = i < optionsBH.Length ? optionsBH[i].Trim() : option1;
                    bool isSelected1 = selected.Any(s => s.Trim().Equals(option1, StringComparison.OrdinalIgnoreCase));
                    string checkbox1 = isSelected1 ? "☒" : "☐";
                    sb.Append($"{checkbox1} {optionDisplay1}");

                    // Second checkbox in the pair (if exists)
                    if (i + 1 < options.Length)
                    {
                        string option2 = options[i + 1].Trim();
                        string optionDisplay2 = (i + 1) < optionsBH.Length ? optionsBH[i + 1].Trim() : option2;
                        bool isSelected2 = selected.Any(s => s.Trim().Equals(option2, StringComparison.OrdinalIgnoreCase));
                        string checkbox2 = isSelected2 ? "☒" : "☐";

                        // Add spacing between the two checkboxes in the same row
                        sb.Append($"\t\t{checkbox2} {optionDisplay2}");
                    }

                    // Add line break after each pair (except for the last pair)
                    if (i + 2 < options.Length)
                    {
                        sb.Append("\r\n");
                    }
                }
                return sb.ToString();
            }
            return selectedResults ?? "";
        }
        private string GetResultValue(DataTable dt, string parameterId)
        {
            DataRow[] rows = dt.Select($"ParameterID = '{parameterId}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
        }
        /*originalcode*/
        //protected void btnSaveSpecs_Click(object sender, EventArgs e)
        //{
        //    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
        //    int ReportID = Convert.ToInt32(Request.QueryString["r"]);
        //    string sampleIdSeries = ViewState["SampleIDSeriesVS"]?.ToString() ?? "";
        //    string language = "";

        //    List<string> paramList = new List<string>();
        //    List<string> resultList = new List<string>();

        //    // Map textboxes to correct ReportParamMapID
        //    paramList.Add("17918"); resultList.Add(txtbrand.Text.Trim());                    // Brand
        //    paramList.Add("17919"); resultList.Add(txtmodel.Text.Trim());               // Type/Model
        //    paramList.Add("17902"); resultList.Add(txtVersion.Text.Trim());                 // Version or Serial Number
        //    paramList.Add("17903"); resultList.Add(txtVoltage.Text.Trim());                 // Voltage
        //    paramList.Add("17904"); resultList.Add(txtFrequency.Text.Trim());               // Frequency
        //    paramList.Add("17905"); resultList.Add(txtRatedCurrent.Text.Trim());            // Rated Current
        //    paramList.Add("17906"); resultList.Add(txtFirmware.Text.Trim());                // Firmware or Software Version
        //    paramList.Add("17907"); resultList.Add(txtHDMIPort.Text.Trim());                // HDMI Port Features
        //    paramList.Add("17908"); resultList.Add(txtUSBPort.Text.Trim());                 // USB Port Features
        //    paramList.Add("17909"); resultList.Add(txtAVInput.Text.Trim());                 // AV Input Features
        //    paramList.Add("17910"); resultList.Add(txtInternetNetwork.Text.Trim());         // Internet Network Features
        //    paramList.Add("17911"); resultList.Add(txtMDD.Text.Trim());                     // MDD (Motion-Based Dimming) Feature
        //    paramList.Add("17912"); resultList.Add(txtABC.Text.Trim());                     // ABC (Automatic Brightness Control) feature
        //    paramList.Add("17913"); resultList.Add(txtRetailMode.Text.Trim());              // Retail Mode Features
        //    paramList.Add("17914"); resultList.Add(txtVideoSignal.Text.Trim());             // Video Signal Types
        //    paramList.Add("17915"); resultList.Add(txtBattery.Text.Trim());                 // Battery Component Availability
        //    paramList.Add("17916"); resultList.Add(txtQuickStart.Text.Trim());              // Quick Start Feature
        //    paramList.Add("17917"); resultList.Add(txtSatellite.Text.Trim());               // Satellite Features (add this textbox)


        //    string[] paramMap = paramList.ToArray();
        //    string[] results = resultList.ToArray();

        //    int modified = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "",
        //        sampleId, sampleIdSeries, "", language, ReportID, "", "", "", "", "", "", "", "", "", "",
        //        paramMap, results);

        //    if (modified > 0)
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "alert",
        //            "alert('Specifications saved successfully!');", true);
        //    }
        //}
        protected void btnSaveSpecs_Click(object sender, EventArgs e)
        {
            int sampleId = Convert.ToInt32(Request.QueryString["s"]);
            int ReportID = Convert.ToInt32(Request.QueryString["r"]);
            string sampleIdSeries = ViewState["SampleIDSeriesVS"]?.ToString() ?? "";
            string language = "";

            List<string> paramList = new List<string>();
            List<string> resultList = new List<string>();

            // Product Specifications
            paramList.Add("17918"); resultList.Add(txtbrand.Text.Trim());                    // Brand
            paramList.Add("17919"); resultList.Add(txtmodel.Text.Trim());                    // Type/Model
            paramList.Add("17902"); resultList.Add(txtVersion.Text.Trim());                  // Version or Serial Number
            paramList.Add("17903"); resultList.Add(txtVoltage.Text.Trim());                  // Voltage
            paramList.Add("17904"); resultList.Add(txtFrequency.Text.Trim());                // Frequency (NOT 17921)
            paramList.Add("17905"); resultList.Add(txtRatedCurrent.Text.Trim());             // Rated Current
            paramList.Add("17906"); resultList.Add(txtFirmware.Text.Trim());                 // Firmware or Software Version
            paramList.Add("17907"); resultList.Add(txtHDMIPort.Text.Trim());                 // HDMI Port Features
            paramList.Add("17908"); resultList.Add(txtUSBPort.Text.Trim());                  // USB Port Features
            paramList.Add("17909"); resultList.Add(txtAVInput.Text.Trim());                  // AV Input Features
            paramList.Add("17910"); resultList.Add(txtInternetNetwork.Text.Trim());          // Internet Network Features
            paramList.Add("17911"); resultList.Add(txtMDD.Text.Trim());                      // MDD (Motion-Based Dimming) Feature
            paramList.Add("17912"); resultList.Add(txtABC.Text.Trim());                      // ABC (Automatic Brightness Control) feature
            paramList.Add("17913"); resultList.Add(txtRetailMode.Text.Trim());               // Retail Mode Features
            paramList.Add("17914"); resultList.Add(txtVideoSignal.Text.Trim());              // Video Signal Types
            paramList.Add("17915"); resultList.Add(txtBattery.Text.Trim());                  // Battery Component Availability
            paramList.Add("17916"); resultList.Add(txtQuickStart.Text.Trim());               // Quick Start Feature
            paramList.Add("17917"); resultList.Add(txtSatellite.Text.Trim());                // Satellite Features
            paramList.Add("17904"); resultList.Add(txtFrequency.Text.Trim());                // Satellite Features
            //paramList.Add("17920"); resultList.Add(txtTestVoltage.Text.Trim());                // Satellite Features


            // Testing Conditions
            paramList.Add("17920"); resultList.Add(Request.Form["txtVoltage"] ?? "");        // Test voltage
            paramList.Add("17921"); resultList.Add(Request.Form["txtTestFrequency"] ?? "");  // Test frequency (NEW field needed)
            paramList.Add("17922"); resultList.Add(Request.Form["txtHarmonicDistortion"] ?? ""); // Total Harmonic Distortion
            paramList.Add("17923"); resultList.Add(Request.Form["txtSetupDescription"] ?? ""); // Test setup description
            paramList.Add("17924"); resultList.Add(Request.Form["txtInputTerminal"] ?? "");  // Selected input terminal in UUT
            paramList.Add("17925"); resultList.Add(Request.Form["txtVideoSignalPower"] ?? ""); // Video signal for power consumption
            paramList.Add("17926"); resultList.Add(Request.Form["txtResolution"] ?? "");     // Resolution and speed
            paramList.Add("17927"); resultList.Add(Request.Form["txtDisplaySettings"] ?? ""); // Settings changed from default
            paramList.Add("17929"); resultList.Add(Request.Form["txtPeakLuminanceSignal"] ?? ""); // Video signal for peak luminance

            // Test Results - ON MODE (SEPARATE fields as per table)
            paramList.Add("17930"); resultList.Add(txtMDDSDR.Text.Trim());                   // MDD Features SDR
            paramList.Add("17931"); resultList.Add(txtMDDHDR.Text.Trim());                   // MDD Features HDR (NEW parameter)
            paramList.Add("17932"); resultList.Add(txtABCSDR.Text.Trim());                   // ABC Features SDR
            paramList.Add("17933"); resultList.Add(txtABCHDR.Text.Trim());                   // ABC Features HDR (NEW parameter)
            paramList.Add("17934"); resultList.Add(txtLightingSDR.Text.Trim());              // Lighting level applied SDR
            paramList.Add("17935"); resultList.Add(txtLightingHDR.Text.Trim());              // Lighting level applied HDR (NEW parameter)
            paramList.Add("17936"); resultList.Add(txtPowerSDR.Text.Trim());                 // Power consumption on mode SDR
            paramList.Add("17937"); resultList.Add(txtPowerHDR.Text.Trim());                 // Power consumption on mode HDR (NEW parameter)

            // Test Results - PEAK LUMINANCE (SEPARATE fields as per table)
            paramList.Add("17938"); resultList.Add(txtDefaultLuminance.Text.Trim());         // Default image settings Luminance
            paramList.Add("17939"); resultList.Add(txtDefaultMode.Text.Trim());              // Default image settings Mode (NEW parameter)
            paramList.Add("17940"); resultList.Add(txtBrightestLuminance.Text.Trim());       // Brightest preset image settings Luminance
            paramList.Add("17941"); resultList.Add(txtBrightestMode.Text.Trim());            // Brightest preset image settings Mode (NEW parameter)
            paramList.Add("17942"); resultList.Add(txtActualLuminance.Text.Trim());          // Retail image settings Luminance
            paramList.Add("17943"); resultList.Add(txtActualMode.Text.Trim());               // Retail image settings Mode (NEW parameter)
            paramList.Add("17944"); resultList.Add(txtPeakRatio.Text.Trim());                // Peak luminance ratio Luminance
            paramList.Add("17945"); resultList.Add(txtPeakRatio.Text.Trim());                                       // Peak luminance ratio Mode (NEW parameter - might need a field)

            // Test Results - PARTIAL MODE
            paramList.Add("17946"); resultList.Add(txtStandbyDefault.Text.Trim());           // Standby-passive (default)
            paramList.Add("17947"); resultList.Add(txtStandbyQuickStart.Text.Trim());        // Standby-passive (Quick Start)
            paramList.Add("17948"); resultList.Add(txtStandbyLow.Text.Trim());               // Standby-passive, low
            paramList.Add("17949"); resultList.Add(txtOffMode.Text.Trim());                  // Power consumption Off mode

            // Energy Consumption
            paramList.Add("17950"); resultList.Add(TextBox1.Text.Trim());                    // Screen size
            paramList.Add("17951"); resultList.Add(txtEnergyOnMode.Text.Trim());             // Energy Consumption On Mode
            paramList.Add("17952"); resultList.Add(txtStandbyEnergy.Text.Trim());            // Standby-passive Energy Consumption
            paramList.Add("17953"); resultList.Add(txtTotalEnergy.Text.Trim());              // Total Energy Consumption
            string[] paramMap = paramList.ToArray();
            string[] results = resultList.ToArray();

            int modified = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "",
                sampleId, sampleIdSeries, "", language, ReportID, "", "", "", "", "", "", "", "", "", "",
                paramMap, results);

            if (modified > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('All data saved successfully!');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Failed to save data!');", true);
            }
        }



    }
}