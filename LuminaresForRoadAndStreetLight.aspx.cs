using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using DataTable = System.Data.DataTable;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using ListItem = System.Web.UI.WebControls.ListItem;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace LIMS
{
    public partial class LuminaresForRoadAndStreetLight : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DAL.CM _cm = new DAL.CM();
        // Constants for A4 page layout //
        private const int A4_PAGE_WIDTH_TWIPS = 12240;
        private const int LEFT_MARGIN_TWIPS = 1440;
        private const int RIGHT_MARGIN_TWIPS = 1440;
        private const int USABLE_WIDTH_TWIPS = A4_PAGE_WIDTH_TWIPS - LEFT_MARGIN_TWIPS - RIGHT_MARGIN_TWIPS;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Register Syncfusion License Key
                string licenseKey = "Ngo9BigBOggjGyl/VkV+XU9AclRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3hTc0RnWX5cd3FcTmBdV091XA==";
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

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

                    BindEquipmentCheckboxes();
                    LoadClauseTable();

                    CreateGridTable();

                    DataTable existingData = _cm.GetExistingTestResults_LED_RP(lblSampleID.Text, lblReportID.Text);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        PrefillTestResultsGrid(existingData);
                        PrefillAllData(existingData);
                    }

                    txtbrand.Enabled = false;
                    txtstandard.Enabled = false;
                    txtproduct.Enabled = false;
                    txttestMethodNo.Enabled = false;
                    txtworkorder.Enabled = false;
                    txtmodel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        private void LoadClauseTable()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);

            DataTable dtcl = _cm.GetCRFCloneList_LED(sampleId);
            int cloneCRFSampleID = Convert.ToInt32(dtcl.Rows[0]["CloneCRFSampleID"].ToString());
            ViewState["CloneCRFSampleID"] = dtcl.Rows[0]["CloneCRFSampleID"].ToString();

            DataTable dts = _cm.GetCRFSampleDetails_LED(sampleId, reportId, cloneCRFSampleID);
            if (dts != null && dts.Rows.Count > 0)
            {
                var standardIds = dts.AsEnumerable()
                                    .Where(row => row["TestStandardID"] != null && !string.IsNullOrEmpty(row["TestStandardID"].ToString()))
                                    .Select(row => row["TestStandardID"].ToString().Trim()).Distinct().ToList();

                var standardNames = dts.AsEnumerable()
                                      .Where(row => row["TestStandard"] != null && !string.IsNullOrEmpty(row["TestStandard"].ToString()))
                                      .Select(row => row["TestStandard"].ToString().Trim()).Distinct().ToList();

                lblstandardID.Text = string.Join(", ", standardIds);
                txtstandard.Text = string.Join(" ; ", standardNames);
            }

            DataTable dt = GetClauseData();
            DataTable saved = GetSavedResults();
            if (saved != null && saved.Rows.Count > 0)
            {
                txthumidity.Text = saved.Rows[0]["Humidity"].ToString();
                txttemp.Text = saved.Rows[0]["Temperature"].ToString();

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestDate"].ToString()))
                {
                    DateTime testDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestDate"].ToString(), out testDate))
                    {
                        txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");
                    }
                }

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestCompletationDate"].ToString()))
                {
                    DateTime completionDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestCompletationDate"].ToString(), out completionDate))
                    {
                        txtcompletiondate.Text = completionDate.ToString("yyyy-MM-dd");
                    }
                }
            }

            DataTable DisResget = _cm.GetTestResultLED_DISTINCT(lblSampleID.Text, lblReportID.Text);
            if (DisResget != null && DisResget.Rows.Count > 0)
            {
                string equipementIDGet = DisResget.Rows[0]["EquipmentIds"].ToString();

                if (!string.IsNullOrEmpty(equipementIDGet))
                {
                    string[] selectedEquipmentIds = equipementIDGet.Split(',').Select(id => id.Trim()).ToArray();
                    foreach (ListItem item in chkEquipmentList.Items)
                    {
                        if (selectedEquipmentIds.Contains(item.Value))
                        {
                            item.Selected = true;
                        }
                    }
                }
            }

            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_LED(sampleId);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                int departmentID = Convert.ToInt32(dt1.Rows[0]["DepartmentID"]);
                int productCategoryID = Convert.ToInt32(dt1.Rows[0]["ProductCategoryID"]);
                string StandardID = dt1.Rows[0]["StandardID"].ToString();
                int reportID = Convert.ToInt32(Request.QueryString["r"]);

                string TestStandardGet = dts.Rows[0]["TestStandard"].ToString();
                txtproduct.Text = dt1.Rows[0]["ProductName"].ToString();
                txtbrand.Text = dt1.Rows[0]["BrandProduct"].ToString();
                txtmodel.Text = dt1.Rows[0]["ProductSerialNo"].ToString();
               
                txttestMethodNo.Text = dts.Rows[0]["TestMethod"].ToString();
                ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"].ToString();

                string CloneCRFSampleID = dt1.Rows[0]["CloneCRFSampleID"].ToString();

                DataTable dtst = _cm.CheckAllAllocated_LED(Convert.ToInt32(CloneCRFSampleID));
                if (dtst.Rows.Count > 0)
                {
                    txtworkorder.Text = dtst.Rows[0]["WorkorderNumber"].ToString();
                }
                else
                {
                    txtworkorder.Text = " ";
                }

                    string[] ids = StandardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> standardIdList = ids.Select(id => id.Trim()).ToList();
                DataTable filteredClauseTable = null;

                if (standardIdList.Count > 0)
                {
                    var matchingRows = dt.AsEnumerable().Where(r =>
                    {
                        string clauseStandardIds = r["Standard_ID"].ToString();
                        string[] clauseIds = clauseStandardIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                        return clauseIds.Any(cid => standardIdList.Contains(cid));
                    });

                    if (matchingRows.Any())
                        filteredClauseTable = matchingRows.CopyToDataTable();
                }

                if (filteredClauseTable == null || filteredClauseTable.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No matching clauses found!');</script>");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                bool tableStarted = false;

                foreach (DataRow row in filteredClauseTable.Rows)
                {
                    bool isHeader = row["isHeader"].ToString() == "1";

                    if (isHeader)
                    {
                        if (tableStarted) sb.Append("</table>");

                        sb.Append("<table class='clause-table'>");

                        sb.Append("<tr class='sub-header-row'>");
                        sb.Append("<th class='clause-number'>Klausul<br/>Clause</th>");
                        sb.Append("<th class='requirement'>Persyaratan<br/>Requirements</th>");
                        sb.Append("<th class='result'>Hasil uji<br/>Test Result</th>");
                        sb.Append("<th class='remark'>*Keterangan<br/>Remark</th>");
                        sb.Append("</tr>");

                        sb.Append("<tr class='header-row'>");
                        sb.Append("<td>");
                        sb.Append($"<b>{row["ClauseNumber"]}</b> <br/>");
                        sb.Append("</td>");
                        sb.Append("<td colspan='2'>");
                        sb.Append($"<u>{row["ClauseTitle_BH"]}</u><br/>");
                        sb.Append($"<i>{row["ClauseTitle"]}</i>");
                        sb.Append("</td>");

                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string savedRemark = "";
                        var savedRowForRemark = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForRemark != null)
                            savedRemark = savedRowForRemark["Remark"]?.ToString() ?? "";

                        sb.Append($"<td class='remark'><textarea name='remark_{clauseID}' rows='2' style='width:96%; resize:vertical;'>{HttpUtility.HtmlEncode(savedRemark)}</textarea></td>");

                        sb.Append("</tr>");

                        tableStarted = true;
                    }
                    else
                    {
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        string inputType = GetInputType(row);

                        sb.Append("<tr>");
                        sb.Append($"<td class='clause-number'>{row["ClauseNumber"]}</td>");

                        sb.Append("<td class='requirement'>");
                        sb.Append($"<u>{row["Requirement_BH"]}</u><br/>");
                        sb.Append($"<i>{row["Requirement"]}</i>");
                        sb.Append("</td>");

                        sb.Append("<td class='result'>");
                        sb.Append($"<input type='hidden' name='inputtype_{clauseID}' value='{inputType}' />");
                        GenerateResultInput(sb, row, saved, clauseID);
                        sb.Append("</td>");

                        string savedRemark = "";
                        var savedRowForRemark = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                        if (savedRowForRemark != null)
                            savedRemark = savedRowForRemark["Remark"]?.ToString() ?? "";

                        sb.Append($"<td class='remark'><textarea name='remark_{clauseID}' rows='2' style='width:96%; resize:vertical;'>{HttpUtility.HtmlEncode(savedRemark)}</textarea></td>");
                        sb.Append("</tr>");
                    }
                }

                if (tableStarted) sb.Append("</table>");
                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
            }
        }

        private void BindEquipmentCheckboxes()
        {
            int DepartmentId = Convert.ToInt32(Session["DeptID"].ToString());
            string query = "SELECT * FROM Calibration_Equipment WHERE DepartmentId = 4009";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", DepartmentId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    chkEquipmentList.DataSource = reader;
                    chkEquipmentList.DataTextField = "Equipment";
                    chkEquipmentList.DataValueField = "EpID";
                    chkEquipmentList.DataBind();

                    reader.Close();
                }
            }
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
                        sb.Append("<div class='checkbox-row'>");
                    }

                    sb.Append($"<label class='checkbox-item'><input type='checkbox' name='chk_{clauseID}_{optValue}' value='{optValue}' {isChecked}/> {optDisplayBH}</label>");

                    if ((i + 1) % 3 == 0 || i == options.Length - 1)
                    {
                        sb.Append("</div>");
                    }
                }

                sb.Append("</div>");
            }
            else
            {
                var savedRow = saved.Select($"ClauseID={clauseID}").FirstOrDefault();
                string savedResult = savedRow != null ? savedRow["Result"]?.ToString() ?? "" : "";
                sb.Append($"<input type='text' name='textresult_{clauseID}' value='{HttpUtility.HtmlEncode(savedResult)}' style='width:94%;' />");
            }
        }

        private DataTable GetClauseData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string[] standardIds = lblstandardID.Text.Split(',').Select(id => id.Trim()).Where(id => !string.IsNullOrEmpty(id)).ToArray();

                if (standardIds.Length > 0)
                {
                    string whereClause = string.Join(" OR ", standardIds.Select(id => $"(',' + Standard_ID + ',') LIKE '%,{id},%'"));
                    string query = $@"SELECT * FROM Clauses WHERE {whereClause} ORDER BY ClauseID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                }
            }
            return dt;
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool b1 = false; bool b2 = false;

                string cloneCFRsampleID = ViewState["CloneCRFSampleID"].ToString();
                string equipmentIdsCSV = "";
                string equipmentNamesCSV = "";

                List<string> selectedEquipmentIds = new List<string>();
                List<string> selectedEquipmentNames = new List<string>();

                foreach (ListItem item in chkEquipmentList.Items)
                {
                    if (item.Selected)
                    {
                        selectedEquipmentIds.Add(item.Value);
                        selectedEquipmentNames.Add(item.Text);
                    }
                }

                if (selectedEquipmentIds.Count > 0)
                {
                    equipmentIdsCSV = string.Join(",", selectedEquipmentIds);
                    equipmentNamesCSV = string.Join(",", selectedEquipmentNames);
                    int sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                    int ReportID = Convert.ToInt32(Request.QueryString["r"]);
                    string sampleIdSeries = ViewState["SampleIDSeriesVS"].ToString();
                    string language = "";
                    int reportParamMapID = 0;

                    ClearExistingResults();
                    DataTable clauses = GetClauseData();

                    foreach (DataRow row in clauses.Rows)
                    {
                        int clauseID = Convert.ToInt32(row["ClauseID"]);
                        bool isHeader = row["isHeader"].ToString() == "1";
                        string inputType = Request.Form[$"inputtype_{clauseID}"] ?? "";
                        string remark = Request.Form[$"remark_{clauseID}"] ?? "";

                        if (isHeader)
                        {
                            int result0 = _cm.TestResultsAdd_LED(clauseID, txtstartingdate.Text, txtcompletiondate.Text, "", remark, sampleId, sampleIdSeries,
                                 "header", language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                 txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text);
                            if (result0 > 0)
                            {
                                b1 = true;
                            }
                        }
                        else
                        {
                            string result = "";

                            if (inputType == "checkbox")
                            {
                                string[] options = row["ResultOptions"].ToString().Split(',');
                                List<string> selectedOptions = new List<string>();
                                foreach (var option in options)
                                {
                                    if (Request.Form[$"chk_{clauseID}_{option.Trim()}"] != null)
                                        selectedOptions.Add(option.Trim());
                                }
                                result = string.Join(",", selectedOptions);
                            }
                            else
                            {
                                result = Request.Form[$"textresult_{clauseID}"] ?? "";
                            }

                            int result0 = _cm.TestResultsAdd_LED(clauseID, txtstartingdate.Text, txtcompletiondate.Text, result, remark, sampleId, sampleIdSeries,
                                inputType, language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text);

                            if (result0 > 0)
                            {
                                b1 = true;
                            }
                        }
                    }

                    if (b1 == true)
                    {
                        DataTable dtRepoParam = ViewState["TestData"] as DataTable;

                        if (dtRepoParam == null || dtRepoParam.Rows.Count == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No test data to save!');", true);
                            return;
                        }

                        List<string> objectNames = new List<string>();
                        List<string> manufacturers = new List<string>();
                        List<string> typeOrModels = new List<string>();
                        List<string> technicalDatas = new List<string>();
                        List<string> standards = new List<string>();
                        List<string> signOfConformities = new List<string>();

                        foreach (DataRow row in dtRepoParam.Rows)
                        {
                            objectNames.Add(row["ObjectName"].ToString());
                            manufacturers.Add(row["Manufacturer"].ToString());
                            typeOrModels.Add(row["TypeOrModel"].ToString());
                            technicalDatas.Add(row["TechnicalData"].ToString());
                            standards.Add(row["Standard"].ToString());
                            signOfConformities.Add(row["SignOfConformity"].ToString());
                        }

                        string objectNamesCsv = string.Join(",", objectNames);
                        string manufacturersCsv = string.Join(",", manufacturers);
                        string typeOrModelsCsv = string.Join(",", typeOrModels);
                        string technicalDatasCsv = string.Join(",", technicalDatas);
                        string standardsCsv = string.Join(",", standards);
                        string signOfConformitiesCsv = string.Join(",", signOfConformities);

                        List<string> reportParamMapList = new List<string>();
                        List<string> resultList = new List<string>();

                        reportParamMapList.Add("17813"); resultList.Add(objectNamesCsv);
                        reportParamMapList.Add("17814"); resultList.Add(manufacturersCsv);
                        reportParamMapList.Add("17815"); resultList.Add(typeOrModelsCsv);
                        reportParamMapList.Add("17816"); resultList.Add(technicalDatasCsv);
                        reportParamMapList.Add("17817"); resultList.Add(standardsCsv);
                        reportParamMapList.Add("17818"); resultList.Add(signOfConformitiesCsv);

                        reportParamMapList.Add("17819"); resultList.Add(txtActinicUV1.Text);
                        reportParamMapList.Add("17820"); resultList.Add(txtActinicUV2.Text);
                        reportParamMapList.Add("17821"); resultList.Add(txtActinicUV3.Text);

                        reportParamMapList.Add("17822"); resultList.Add(txtNearUV1.Text);
                        reportParamMapList.Add("17823"); resultList.Add(txtNearUV2.Text);
                        reportParamMapList.Add("17824"); resultList.Add(txtNearUV3.Text);

                        reportParamMapList.Add("17825"); resultList.Add(txtBlueLight1.Text);
                        reportParamMapList.Add("17826"); resultList.Add(txtBlueLight2.Text);
                        reportParamMapList.Add("17827"); resultList.Add(txtBlueLight3.Text);

                        reportParamMapList.Add("17828"); resultList.Add(txtBlueLightHazardRadiance.Text);
                        reportParamMapList.Add("17829"); resultList.Add(txtLuminance.Text);

                        reportParamMapList.Add("17830");
                        resultList.Add($"{txtProductTypeResult.Text.Trim()}|{txtProductTypeRemark.Text.Trim()}");

                        reportParamMapList.Add("17831");
                        resultList.Add($"{txtTheLightsUsedResult.Text.Trim()}|{txtTheLightsUsedRemark.Text.Trim()}");

                        reportParamMapList.Add("17832");
                        resultList.Add($"{txtTypeOfLEDDriverUsedResult.Text.Trim()}|{txtTypeOfLEDDriverUsedRemark.Text.Trim()}");

                        reportParamMapList.Add("17833");
                        resultList.Add($"{txtMountingPositionDuringTestingResult.Text.Trim()}|{txttxtMountingPositionDuringTestingRemark.Text.Trim()}");

                        reportParamMapList.Add("17834");
                        resultList.Add($"{txtPowerWResult.Text.Trim()}|{txtPowerWRemark.Text.Trim()}");

                        reportParamMapList.Add("17835");
                        resultList.Add($"{txtCurrentAResult.Text.Trim()}|{txtCurrentARemark.Text.Trim()}");

                        reportParamMapList.Add("17836");
                        resultList.Add($"{txtTest1RatedVoltageResult.Text.Trim()}|{txtTest1RatedVoltageRemark.Text.Trim()}");

                        reportParamMapList.Add("17837");
                        resultList.Add($"{txtTest2Result.Text.Trim()}|{txtTest2Remark.Text.Trim()}");

                        reportParamMapList.Add("17838");
                        resultList.Add($"{txtTest3Result.Text.Trim()}|{txtTest3Remark.Text.Trim()}");

                        reportParamMapList.Add("17839");
                        resultList.Add($"{txtTest4Result.Text.Trim()}|{txtTest4Remark.Text.Trim()}");

                        reportParamMapList.Add("17840");
                        resultList.Add($"{txtTestChamberTemperatureTest1.Text.Trim()}|{txtTestChamberTemperatureTest2.Text.Trim()}|{txtTestChamberTemperatureTest3.Text.Trim()}|{txtTestChamberTemperatureLimitsMax1.Text.Trim()}|{txtTestChamberTemperatureTest4.Text.Trim()}|{txtTestChamberTemperatureLimitsMax2.Text.Trim()}");

                        reportParamMapList.Add("17841");
                        resultList.Add($"{txtOuterSurfaceOfTheLuminaireTest1.Text.Trim()}|{txtOuterSurfaceOfTheLuminaireTest2.Text.Trim()}|{txtOuterSurfaceOfTheLuminaireTest3.Text.Trim()}|{txtOuterSurfaceOfTheLuminaireLimitMax1.Text.Trim()}|{txtOuterSurfaceOfTheLuminaireTest4.Text.Trim()}|{txtOuterSurfaceOfTheLuminaireLimitMax2.Text.Trim()}");

                        reportParamMapList.Add("17842");
                        resultList.Add($"{txtSurfaceLEDDriverTest1.Text.Trim()}|{txtSurfaceLEDDriverTest2.Text.Trim()}|{txtSurfaceLEDDriverTest3.Text.Trim()}|{txtSurfaceLEDDriverLimitMax1.Text.Trim()}|{txtSurfaceLEDDriverTest4.Text.Trim()}|{txtSurfaceLEDDriverLimitMax2.Text.Trim()}");

                        reportParamMapList.Add("17843");
                        resultList.Add($"{txtLEDModulePCBTest1.Text.Trim()}|{txtLEDModulePCBTest2.Text.Trim()}|{txtLEDModulePCBTest3.Text.Trim()}|{txtLEDModulePCBLimitMax1.Text.Trim()}|{txtLEDModulePCBTest4.Text.Trim()}|{txtLEDModulePCBLimitMax2.Text.Trim()}");

                        reportParamMapList.Add("17844");
                        resultList.Add($"{txtLEDModuleEnclosurePlasticTest1.Text.Trim()}|{txtLEDModuleEnclosurePlasticTest2.Text.Trim()}|{txtLEDModuleEnclosurePlasticTest3.Text.Trim()}|{txtLEDModuleEnclosurePlasticLimitMax1.Text.Trim()}|{txtLEDModuleEnclosurePlasticTest4.Text.Trim()}|{txtLEDModuleEnclosurePlasticLimitMax2.Text.Trim()}");

                        reportParamMapList.Add("17845");
                        resultList.Add($"{txtExternalCableTest1.Text.Trim()}|{txtExternalCableTest2.Text.Trim()}|{txtExternalCableTest3.Text.Trim()}|{txtExternalCableLimitMax1.Text.Trim()}|{txtExternalCableTest4.Text.Trim()}|{txtExternalCableLimitMax2.Text.Trim()}");

                        reportParamMapList.Add("17846");
                        resultList.Add($"{txtInternalWiringTest1.Text.Trim()}|{txtInternalWiringTest2.Text.Trim()}|{txtInternalWiringTest3.Text.Trim()}|{txtInternalWiringLimitMax1.Text.Trim()}|{txtInternalWiringTest4.Text.Trim()}|{txtInternalWiringLimitMax2.Text.Trim()}");

                        string[] reportParamMap = reportParamMapList.ToArray();
                        string[] resultNew = resultList.ToArray();

                        int modified = _cm.TestResultsAdd_LED_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "", sampleId, sampleIdSeries,
                                    "", language, ReportID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                    txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text, reportParamMap, resultNew);

                        if (modified > 0)
                        {
                            b2 = true;
                        }

                        if (b2 == true)
                        {
                            GenerateAndExportReport();
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save data.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select at least one equipment.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
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

        private void GenerateAndExportReport()
        {
            string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
            string pathout = Server.MapPath("~/Reports/");
            string templatePath = Server.MapPath("~/InputFiles/LEDTestRecord_1.docx");

            if (!Directory.Exists(pathout))
                Directory.CreateDirectory(pathout);

            string outputDocxPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".docx");
            string outputPdfPath = Path.Combine(pathout, "TestReport-" + SampleUID + ".pdf");

            try
            {
                // Load template using OpenXML
                byte[] templateBytes = File.ReadAllBytes(templatePath);

                // Process document using OpenXML
                byte[] processedBytes = ProcessDocument(templateBytes);

                // Save DOCX
                File.WriteAllBytes(outputDocxPath, processedBytes);

                // Convert to PDF using Syncfusion
                byte[] pdfBytes = ConvertDocxToPdfSyncfusion(processedBytes);
                File.WriteAllBytes(outputPdfPath, pdfBytes);

                // Update status
                string cloneCFRsampleID = ViewState["CloneCRFSampleID"].ToString();
                int sampleStatusID = 27;
                int res1 = _cm.UpdateCRFSampleDetailsStatus_LED(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
                if (res1 > 0)
                {
                    int res2 = _cm.UpdateCRFCloneStatus_LED(sampleStatusID, cloneCFRsampleID);
                    if (res2 > 0)
                    {
                        int res3 = _cm.updateSampleDetailStatus_LED(sampleStatusID, lblSampleID.Text);
                    }
                }

                // Stream PDF to client
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
                    $"<script> alert('Error: {ex.Message.Replace("'", "\\'")}'); </script>");
            }
        }
                
        private byte[] ProcessDocument(byte[] docBytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(docBytes, 0, docBytes.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
                {
                    // Replace template variables
                    ReplaceTemplateVariablesOpenXML(wordDoc);

                    // Create report tables
                    DataTable dtReport = GetReportData();
                    CreateReportTableOpenXML(wordDoc, dtReport);

                    DataTable dtReportT2 = GetReportDataForT2();
                    CreateReportTableT2OpenXML(wordDoc, dtReportT2);

                    DataTable dtReportT3 = GetReportDataForT2();
                    CreateReportTableT3OpenXML(wordDoc, dtReportT3);

                    DataTable dtReportT4 = GetReportDataForT2();
                    CreateReportTableT4OpenXML(wordDoc, dtReportT4);


                    DataTable dtReportTC = GetReportDataRefer();
                    CreateReportTableOpenXML_ReferTC(wordDoc, dtReportTC);


                    // Clean up unresolved placeholders
                    CleanupUnresolvedPlaceholders(wordDoc);

                    wordDoc.MainDocumentPart.Document.Save();
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Convert DOCX to PDF using Syncfusion
        /// </summary>
        private byte[] ConvertDocxToPdfSyncfusion(byte[] docxBytes)
        {
            try
            {
                using (MemoryStream docxStream = new MemoryStream(docxBytes))
                using (WordDocument wordDocument = new WordDocument(docxStream, FormatType.Automatic))
                using (DocIORenderer renderer = new DocIORenderer())
                using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                using (MemoryStream pdfStream = new MemoryStream())
                {
                    pdfDocument.Save(pdfStream);
                    return pdfStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Syncfusion PDF conversion error: {ex.Message}");
            }
        }

        #region OpenXML Helper Methods

        /// <summary>
        /// Find and replace text using OpenXML
        /// </summary>
        private void FindAndReplaceOpenXML(WordprocessingDocument doc, string findText, string replaceText)
        {
            if (string.IsNullOrEmpty(findText)) return;
            if (replaceText == null) replaceText = "";

            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            ReplaceAllInElementOpenXML(mainPart.Document.Body, findText, replaceText);

            foreach (var headerPart in mainPart.HeaderParts)
            {
                if (headerPart.Header != null)
                    ReplaceAllInElementOpenXML(headerPart.Header, findText, replaceText);
            }

            foreach (var footerPart in mainPart.FooterParts)
            {
                if (footerPart.Footer != null)
                    ReplaceAllInElementOpenXML(footerPart.Footer, findText, replaceText);
            }
        }

        /// <summary>
        /// Replace all occurrences in an OpenXML element
        /// </summary>
        private void ReplaceAllInElementOpenXML(OpenXmlElement element, string findText, string replaceText)
        {
            foreach (var paragraph in element.Descendants<Paragraph>().ToList())
            {
                ReplaceInParagraphOpenXML(paragraph, findText, replaceText);
            }
        }

        /// <summary>
        /// Replace text in a paragraph using OpenXML
        /// </summary>
        private bool ReplaceInParagraphOpenXML(Paragraph paragraph, string findText, string replaceText)
        {
            var runs = paragraph.Descendants<Run>().ToList();
            if (runs.Count == 0) return false;

            foreach (var run in runs)
            {
                var textElement = run.Descendants<WordText>().FirstOrDefault();
                if (textElement != null && textElement.Text.Contains(findText))
                {
                    textElement.Text = textElement.Text.Replace(findText, replaceText);
                    textElement.Space = SpaceProcessingModeValues.Preserve;
                    return true;
                }
            }

            string fullText = string.Join("", runs.Select(r => r.Descendants<WordText>().FirstOrDefault()?.Text ?? ""));
            if (!fullText.Contains(findText)) return false;

            string newFullText = fullText.Replace(findText, replaceText);

            WordText firstTextEl = null;
            foreach (var run in runs)
            {
                var textEl = run.Descendants<WordText>().FirstOrDefault();
                if (textEl == null) continue;
                if (firstTextEl == null)
                {
                    firstTextEl = textEl;
                    textEl.Text = newFullText;
                    textEl.Space = SpaceProcessingModeValues.Preserve;
                }
                else
                {
                    textEl.Text = "";
                }
            }

            return firstTextEl != null;
        }

        /// <summary>
        /// Get paragraph full text
        /// </summary>
        private string GetParagraphFullText(Paragraph paragraph)
        {
            return string.Join("", paragraph.Descendants<WordText>().Select(t => t.Text));
        }

        /// <summary>
        /// Find paragraph containing placeholder
        /// </summary>
        private Paragraph FindParagraphContaining(OpenXmlElement root, string placeholder)
        {
            foreach (var para in root.Descendants<Paragraph>())
            {
                if (GetParagraphFullText(para).Contains(placeholder))
                    return para;
            }
            return null;
        }

        /// <summary>
        /// Clean up unresolved placeholders
        /// </summary>
        private void CleanupUnresolvedPlaceholders(WordprocessingDocument doc)
        {
            var mainPart = doc.MainDocumentPart;
            if (mainPart == null) return;

            var regex = new Regex(@"\{[A-Za-z][A-Za-z0-9_.\*:'/ ]{0,60}\}");

            void CleanElement(OpenXmlElement element)
            {
                foreach (var paragraph in element.Descendants<Paragraph>().ToList())
                {
                    string fullText = GetParagraphFullText(paragraph);
                    if (string.IsNullOrEmpty(fullText)) continue;

                    var matches = regex.Matches(fullText);
                    foreach (Match m in matches)
                    {
                        if (GetParagraphFullText(paragraph).Contains(m.Value))
                        {
                            ReplaceInParagraphOpenXML(paragraph, m.Value, "");
                        }
                    }
                }
            }

            CleanElement(mainPart.Document.Body);
            foreach (var headerPart in mainPart.HeaderParts)
                if (headerPart.Header != null) CleanElement(headerPart.Header);
            foreach (var footerPart in mainPart.FooterParts)
                if (footerPart.Footer != null) CleanElement(footerPart.Footer);
        }
        #endregion

        #region Template Variable Replacement
        private void ReplaceTemplateVariablesOpenXML(WordprocessingDocument doc)
        {
            DataTable DisResget = _cm.GetTestResultLED_DISTINCT(lblSampleID.Text, lblReportID.Text);

            if (DisResget != null && DisResget.Rows.Count > 0)
            {
                string equipmentIds = DisResget.Rows[0]["EquipmentIds"]?.ToString() ?? "";
                List<string> equipmentNames = GetEquipmentNamesAsList(equipmentIds);
                DataRow row = DisResget.Rows[0];

                var replacements = new Dictionary<string, string>
                {
                    {"{WorkorderNumber}", GetSafeString(row, "WorkOrderNo") },
                    {"{Product}", GetSafeString(row, "ProductName")},
                    {"{Brand}", GetSafeString(row, "ProductBrand")},
                    {"{TestStartDate}", FormatDate(GetSafeString(row, "TestDate"))},
                    {"{TestCompletiondate}", FormatDate(GetSafeString(row, "TestCompletationDate"))},
                    {"{Temperature}", GetSafeString(row, "Temperature")},
                    {"{Humidity}", GetSafeString(row, "Humidity") },
                    {"{Method}", GetSafeString(row, "TestMethod")},
                    {"{Standard}", GetSafeString(row, "StandardName")},
                    {"{Model}", GetSafeString(row, "ProductModel")},
                };

                foreach (var replacement in replacements)
                {
                    if (!string.IsNullOrEmpty(replacement.Key) && !string.IsNullOrEmpty(replacement.Value))
                    {
                        FindAndReplaceOpenXML(doc, replacement.Key, replacement.Value);
                    }
                }

                if (equipmentNames != null && equipmentNames.Count > 0)
                {
                    StringBuilder equipmentText = new StringBuilder();
                    for (int i = 0; i < equipmentNames.Count; i++)
                    {
                        equipmentText.AppendLine($"{i + 1}. {equipmentNames[i]}");
                    }
                    FindAndReplaceOpenXML(doc, "{Equipment}", equipmentText.ToString());
                }
            }
        }
        #endregion

        #region Main Report Table
        private void CreateReportTableOpenXML(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{TestingAndMeasurement}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{TestingAndMeasurement}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{TestingAndMeasurement}", "");

            int dataRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() != "1");
            int headerRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() == "1");
            int totalRows = 1 + dataRows + headerRows;

            Table table = new Table();

            TableProperties tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            table.AppendChild(tableProps);

            // Column widths
            float totalOriginalWidth = 0.6f + 3.4f + 2.0f + 1.0f;
            float scaleFactor = USABLE_WIDTH_TWIPS / (totalOriginalWidth * 1440f);
            int col1Width = (int)(0.7f * 1440f * scaleFactor);
            int col2Width = (int)(3.3f * 1440f * scaleFactor);
            int col3Width = (int)(2.0f * 1440f * scaleFactor);
            int col4Width = (int)(1.0f * 1440f * scaleFactor);

            TableGrid grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = col1Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col2Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col3Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col4Width.ToString() });
            table.AppendChild(grid);

            // Header row
            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            string[] headers = { "Klausul\nClause", "Persyaratan\nRequirements", "Hasil\nResult", "*Keterangan\nRemark" };
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            int currentRow = 1;
            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                tableRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 300, HeightType = HeightRuleValues.AtLeast }
                ));

                if (row["isHeader"].ToString() == "1")
                {
                    // Section header row
                    TableCell clauseCell = new TableCell();
                    clauseCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph clausePara = new Paragraph();
                    clausePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run clauseRun = new Run();
                    clauseRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    clauseRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clausePara.AppendChild(clauseRun);
                    clauseCell.AppendChild(clausePara);
                    tableRow.AppendChild(clauseCell);

                    TableCell titleCell = new TableCell();
                    titleCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                        new GridSpan() { Val = 2 }
                    ));
                    string clauseTitleBH = row["ClauseTitle_BH"]?.ToString() ?? "";
                    string clauseTitle = row["ClauseTitle"]?.ToString() ?? "";
                    // First line underlined, second line plain (matches corrected design in image 2)
                    AppendTwoLineParagraph(titleCell, clauseTitleBH, clauseTitle, true, JustificationValues.Left, 18);
                    tableRow.AppendChild(titleCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }
                else
                {
                    // Data row
                    TableCell clauseNumCell = new TableCell();
                    clauseNumCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph clauseNumPara = new Paragraph();
                    clauseNumPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run clauseNumRun = new Run();
                    clauseNumRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    clauseNumRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clauseNumPara.AppendChild(clauseNumRun);
                    clauseNumCell.AppendChild(clauseNumPara);
                    tableRow.AppendChild(clauseNumCell);

                    TableCell reqCell = new TableCell();
                    reqCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    string reqBH = row["Requirement_BH"]?.ToString() ?? "";
                    string reqEN = row["Requirement"]?.ToString() ?? "";
                    // First line underlined, second line plain (matches corrected design in image 2)
                    AppendTwoLineParagraph(reqCell, reqBH, reqEN, false, JustificationValues.Left, 18);
                    tableRow.AppendChild(reqCell);

                    TableCell resultCell = new TableCell();
                    resultCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string inputType = GetInputTypeFromClause(row);
                    Paragraph resultPara = new Paragraph();
                    resultPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    // Builds checkbox glyph runs with a symbol-capable font so ☐ / ☒ actually render (fixes image 1 -> image 2)
                    AppendCheckboxResultParagraph(resultPara, row, inputType);
                    resultCell.AppendChild(resultPara);
                    tableRow.AppendChild(resultCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }

                table.AppendChild(tableRow);
                currentRow++;
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        private void CreateReportTableOpenXML_ReferTC(WordprocessingDocument doc, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_C-TERMINAL}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_C-TERMINAL}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_C-TERMINAL}", "APPENDIX C – TERMINAL");

            int dataRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() != "1");
            int headerRows = dt.Rows.Cast<DataRow>().Count(r => r["isHeader"].ToString() == "1");
            int totalRows = 1 + dataRows + headerRows;

            Table table = new Table();

            TableProperties tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            table.AppendChild(tableProps);

            // Column widths
            float totalOriginalWidth = 0.6f + 3.4f + 2.0f + 1.0f;
            float scaleFactor = USABLE_WIDTH_TWIPS / (totalOriginalWidth * 1440f);
            int col1Width = (int)(0.7f * 1440f * scaleFactor);
            int col2Width = (int)(3.3f * 1440f * scaleFactor);
            int col3Width = (int)(2.0f * 1440f * scaleFactor);
            int col4Width = (int)(1.0f * 1440f * scaleFactor);

            TableGrid grid = new TableGrid();
            grid.AppendChild(new GridColumn() { Width = col1Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col2Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col3Width.ToString() });
            grid.AppendChild(new GridColumn() { Width = col4Width.ToString() });
            table.AppendChild(grid);

            // Header row
            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            string[] headers = { "Klausul\nClause", "Persyaratan\nRequirements", "Hasil\nResult", "*Keterangan\nRemark" };
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            int currentRow = 1;
            foreach (DataRow row in dt.Rows)
            {
                TableRow tableRow = new TableRow();
                tableRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 300, HeightType = HeightRuleValues.AtLeast }
                ));

                if (row["isHeader"].ToString() == "1")
                {
                    // Section header row
                    TableCell clauseCell = new TableCell();
                    clauseCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph clausePara = new Paragraph();
                    clausePara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run clauseRun = new Run();
                    clauseRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    clauseRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clausePara.AppendChild(clauseRun);
                    clauseCell.AppendChild(clausePara);
                    tableRow.AppendChild(clauseCell);

                    TableCell titleCell = new TableCell();
                    titleCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                        new GridSpan() { Val = 2 }
                    ));
                    string clauseTitleBH = row["ClauseTitle_BH"]?.ToString() ?? "";
                    string clauseTitle = row["ClauseTitle"]?.ToString() ?? "";
                    // First line underlined, second line plain (matches corrected design in image 2)
                    AppendTwoLineParagraph(titleCell, clauseTitleBH, clauseTitle, true, JustificationValues.Left, 18);
                    tableRow.AppendChild(titleCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }
                else
                {
                    // Data row
                    TableCell clauseNumCell = new TableCell();
                    clauseNumCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph clauseNumPara = new Paragraph();
                    clauseNumPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run clauseNumRun = new Run();
                    clauseNumRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    clauseNumRun.AppendChild(new WordText() { Text = row["ClauseNumber"]?.ToString() ?? "", Space = SpaceProcessingModeValues.Preserve });
                    clauseNumPara.AppendChild(clauseNumRun);
                    clauseNumCell.AppendChild(clauseNumPara);
                    tableRow.AppendChild(clauseNumCell);

                    TableCell reqCell = new TableCell();
                    reqCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    string reqBH = row["Requirement_BH"]?.ToString() ?? "";
                    string reqEN = row["Requirement"]?.ToString() ?? "";
                    // First line underlined, second line plain (matches corrected design in image 2)
                    AppendTwoLineParagraph(reqCell, reqBH, reqEN, false, JustificationValues.Left, 18);
                    tableRow.AppendChild(reqCell);

                    TableCell resultCell = new TableCell();
                    resultCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string inputType = GetInputTypeFromClause(row);
                    Paragraph resultPara = new Paragraph();
                    resultPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    // Builds checkbox glyph runs with a symbol-capable font so ☐ / ☒ actually render (fixes image 1 -> image 2)
                    AppendCheckboxResultParagraph(resultPara, row, inputType);
                    resultCell.AppendChild(resultPara);
                    tableRow.AppendChild(resultCell);

                    TableCell remarkCell = new TableCell();
                    remarkCell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
                    ));
                    string remarkText = GetRemarkText(row);
                    Paragraph remarkPara = new Paragraph();
                    remarkPara.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left }
                    );
                    Run remarkRun = new Run();
                    remarkRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    remarkRun.AppendChild(new WordText() { Text = remarkText, Space = SpaceProcessingModeValues.Preserve });
                    remarkPara.AppendChild(remarkRun);
                    remarkCell.AppendChild(remarkPara);
                    tableRow.AppendChild(remarkCell);
                }

                table.AppendChild(tableRow);
                currentRow++;
            }

            targetParagraph.Parent.InsertAfter(table, targetParagraph);
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

        private string GetRemarkText(DataRow row)
        {
            if (row.Table.Columns.Contains("Remark"))
                return row["Remark"]?.ToString() ?? "";
            else if (row.Table.Columns.Contains("Remarks"))
                return row["Remarks"]?.ToString() ?? "";
            else if (row.Table.Columns.Contains("Keterangan"))
                return row["Keterangan"]?.ToString() ?? "";
            return "";
        }

        /// <summary>
        /// Appends a two-line paragraph to a cell where only the first line is underlined
        /// and the second line is plain. Uses a real line break (not a literal "\n" inside
        /// one Run) so the two lines can carry different formatting.
        /// </summary>
        private void AppendTwoLineParagraph(TableCell cell, string firstLine, string secondLine, bool bold, JustificationValues alignment, int fontSize)
        {
            Paragraph para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = alignment });

            if (!string.IsNullOrEmpty(firstLine))
            {
                Run run1 = new Run();
                RunProperties rp1 = new RunProperties();
                if (bold) rp1.AppendChild(new Bold());
                rp1.AppendChild(new Underline() { Val = UnderlineValues.Single });
                rp1.AppendChild(new FontSize() { Val = fontSize.ToString() });
                run1.AppendChild(rp1);
                run1.AppendChild(new WordText() { Text = firstLine, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run1);
            }

            if (!string.IsNullOrEmpty(firstLine) && !string.IsNullOrEmpty(secondLine))
            {
                Run brRun = new Run();
                brRun.AppendChild(new Break());
                para.AppendChild(brRun);
            }

            if (!string.IsNullOrEmpty(secondLine))
            {
                Run run2 = new Run();
                RunProperties rp2 = new RunProperties();
                if (bold) rp2.AppendChild(new Bold());
                rp2.AppendChild(new FontSize() { Val = fontSize.ToString() });
                run2.AppendChild(rp2);
                run2.AppendChild(new WordText() { Text = secondLine, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run2);
            }

            cell.AppendChild(para);
        }

        /// <summary>
        /// Appends a checkbox symbol run + label run for a single option (e.g. "☒ P").
        /// The checkbox glyph gets its own font run (Segoe UI Symbol) so it always renders,
        /// regardless of the document's default body font.
        /// </summary>
        private void AppendCheckboxRun(Paragraph para, string optValue, string optDisplay, string[] selected)
        {
            bool isChecked = selected.Any(s => s.Trim().Equals(optValue, StringComparison.OrdinalIgnoreCase));
            string symbol = isChecked ? "☒" : "☐";

            Run symbolRun = new Run();
            symbolRun.AppendChild(new RunProperties(
                new RunFonts() { Ascii = "Segoe UI Symbol", HighAnsi = "Segoe UI Symbol", ComplexScript = "Segoe UI Symbol", EastAsia = "Segoe UI Symbol" },
                new FontSize() { Val = "20" }
            ));
            symbolRun.AppendChild(new WordText() { Text = symbol + " ", Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(symbolRun);

            Run labelRun = new Run();
            labelRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
            labelRun.AppendChild(new WordText() { Text = optDisplay, Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(labelRun);
        }

        /// <summary>
        /// Builds the full checkbox/result paragraph directly into an existing Paragraph.
        /// Replaces the old string-based FormatResultsForExport approach so each checkbox
        /// symbol can carry its own font (fixes missing ☐ / ☒ glyphs seen in image 1).
        /// </summary>
        private void AppendCheckboxResultParagraph(Paragraph resultPara, DataRow row, string inputType)
        {
            string resultOptions = row["ResultOptions"]?.ToString();
            string resultOptionsBH = row["ResultOptions_BH"]?.ToString();
            string selectedResults = row["Results"]?.ToString();

            if (inputType == "checkbox" && !string.IsNullOrEmpty(resultOptions))
            {
                string[] options = resultOptions.Split(',');
                string[] optionsBH = !string.IsNullOrEmpty(resultOptionsBH) ? resultOptionsBH.Split(',') : options;
                string[] selected = !string.IsNullOrEmpty(selectedResults) ? selectedResults.Split(',') : new string[0];

                for (int i = 0; i < options.Length; i += 2)
                {
                    string opt1 = options[i].Trim();
                    string disp1 = i < optionsBH.Length ? optionsBH[i].Trim() : opt1;
                    AppendCheckboxRun(resultPara, opt1, disp1, selected);

                    if (i + 1 < options.Length)
                    {
                        Run tabRun = new Run();
                        tabRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                        tabRun.AppendChild(new TabChar());
                        resultPara.AppendChild(tabRun);

                        string opt2 = options[i + 1].Trim();
                        string disp2 = (i + 1) < optionsBH.Length ? optionsBH[i + 1].Trim() : opt2;
                        AppendCheckboxRun(resultPara, opt2, disp2, selected);
                    }

                    if (i + 2 < options.Length)
                    {
                        Run brRun = new Run();
                        brRun.AppendChild(new Break());
                        resultPara.AppendChild(brRun);
                    }
                }
            }
            else
            {
                Run run = new Run();
                run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                run.AppendChild(new WordText() { Text = selectedResults ?? "", Space = SpaceProcessingModeValues.Preserve });
                resultPara.AppendChild(run);
            }
        }

        #endregion

        #region Table T2 - Appendix A (Components List)
        private void CreateReportTableT2OpenXML(WordprocessingDocument doc, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}' not found in the document template.");

            // Replace placeholder with heading
            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}",
                "LAMPIRAN A – DAFTAR KOMPONEN LUMINER\nAPPENDIX A – LIST OF LUMINARY COMPONENTS");

            // Define column mapping
            Dictionary<int, int> columnMapping = new Dictionary<int, int>
            {
                { 17813, 0 }, // Nama Objek
                { 17814, 1 }, // Pabrikan
                { 17815, 2 }, // Tipe / Model
                { 17816, 3 }, // Data Teknis
                { 17817, 4 }, // Standard
                { 17818, 5 }  // Tanda Kesesuaian
            };

            List<string[]> parsedColumns = new List<string[]>();
            for (int i = 0; i < 6; i++)
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

            int numRows = parsedColumns.Max(col => col.Length);
            int numCols = 6;

            // Create table
            Table table = new Table();
            TableProperties tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            table.AppendChild(tableProps);

            // Column widths
            float[] colWidths = { 1.2f, 1.0f, 1.0f, 1.0f, 0.8f, 1.5f };
            TableGrid grid = new TableGrid();
            foreach (float width in colWidths)
            {
                int widthTwips = (int)(width * 1440f);
                grid.AppendChild(new GridColumn() { Width = widthTwips.ToString() });
            }
            table.AppendChild(grid);

            // Header row
            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            string[] headers = {
                "Nama Objek\nObject Name",
                "Pabrikan\nManufacturer",
                "Tipe/Model\nType/Model",
                "Data Teknis\nTechnical Data",
                "Standard",
                "Tanda Kesesuaian\nConformity Mark"
            };

            for (int col = 0; col < numCols; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = headers[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            table.AppendChild(headerRow);

            // Data rows
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                TableRow dataRow = new TableRow();
                dataRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
                ));

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    TableCell cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    string text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    Paragraph para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run run = new Run();
                    run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    run.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    dataRow.AppendChild(cell);
                }
                table.AppendChild(dataRow);
            }

            // Insert table after the target paragraph
            targetParagraph.Parent.InsertAfter(table, targetParagraph);
        }

        #endregion

        #region Helper Methods
        private string GetSafeString(DataRow row, string columnName)
        {
            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                {
                    return row[columnName].ToString();
                }
                return "";
            }
            catch
            {
                return "";
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
                return dateString;
            }
            catch
            {
                return dateString;
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

        private DataTable GetReportData()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT 
                                c.ClauseID,
                                c.ClauseNumber,
                                c.ClauseTitle,
                                c.ClauseTitle_BH,
                                c.Requirement,
                                c.Requirement_BH,
                                c.ResultOptions,
                                c.ResultOptions_BH,
                                c.isHeader, c.standard_ID,
                                ISNULL(t.Result, '') as Results,
                                ISNULL(t.Remark, '') as Remark,
                                ISNULL(t.InputType, '') as SavedInputType
                            FROM Clauses c
                            inner JOIN TestResultsLED t ON c.ClauseID = t.ClauseID 
                                AND t.SampleID = @SampleID
                                AND t.ReportID = @ReportID 
                                AND (c.isRefre is null or c.isRefre = 0 or c.isRefre = '')
                            ORDER BY c.ClauseID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private DataTable GetReportDataRefer()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT 
                                c.ClauseID,
                                c.ClauseNumber,
                                c.ClauseTitle,
                                c.ClauseTitle_BH,
                                c.Requirement,
                                c.Requirement_BH,
                                c.ResultOptions,
                                c.ResultOptions_BH,
                                c.isHeader, c.standard_ID,
                                ISNULL(t.Result, '') as Results,
                                ISNULL(t.Remark, '') as Remark,
                                ISNULL(t.InputType, '') as SavedInputType
                            FROM Clauses c
                            inner JOIN TestResultsLED t ON c.ClauseID = t.ClauseID 
                                AND t.SampleID = @SampleID
                                AND t.ReportID = @ReportID 
                                AND c.isRefre = 1
                            ORDER BY c.ClauseID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SampleID", lblSampleID.Text);
                cmd.Parameters.AddWithValue("@ReportID", lblReportID.Text);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            return dt;
        }


        private DataTable GetReportDataForT2()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * FROM TestResultsLED trl
                             INNER JOIN ReportParamMap rpm ON rpm.ReportParamMapID = trl.ReportParamMapID 
                             INNER JOIN ReportParameters rp ON rp.ReportParameterID = rpm.ReportParameterID
                             WHERE SampleID = @SampleID AND trl.ReportID = @ReportID AND ClauseID = @ClauseID";

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

        #endregion

        #region Grid Table Methods
        private void CreateGridTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ObjectName");
            dt.Columns.Add("Manufacturer");
            dt.Columns.Add("TypeOrModel");
            dt.Columns.Add("TechnicalData");
            dt.Columns.Add("Standard");
            dt.Columns.Add("SignOfConformity");

            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;

            DataRow dr = dt.NewRow();
            dr["ObjectName"] = txtObjectName.Text.Trim();
            dr["Manufacturer"] = txtManufacturer.Text.Trim();
            dr["TypeOrModel"] = txtTypeOrModel.Text.Trim();
            dr["TechnicalData"] = txtTechnicalData.Text.Trim();
            dr["Standard"] = txtStandard1.Text.Trim();
            dr["SignOfConformity"] = txtSignOfConformity.Text.Trim();

            dt.Rows.Add(dr);

            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();

            txtObjectName.Text = "";
            txtManufacturer.Text = "";
            txtTypeOrModel.Text = "";
            txtTechnicalData.Text = "";
            txtStandard1.Text = "";
            txtSignOfConformity.Text = "";
        }

        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;
            dt.Rows.RemoveAt(e.RowIndex);
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        private string GetResultValue(DataTable dt, string parameterId)
        {
            DataRow[] rows = dt.Select($"ParameterID = '{parameterId}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
        }

        private void PrefillTestResultsGrid(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("ObjectName");
            gridDt.Columns.Add("Manufacturer");
            gridDt.Columns.Add("TypeOrModel");
            gridDt.Columns.Add("TechnicalData");
            gridDt.Columns.Add("Standard");
            gridDt.Columns.Add("SignOfConformity");

            string objectNamesCsv = GetResultValue(dt, "17813");
            string manufacturersCsv = GetResultValue(dt, "17814");
            string modelsCsv = GetResultValue(dt, "17815");
            string techDataCsv = GetResultValue(dt, "17816");
            string standardsCsv = GetResultValue(dt, "17817");
            string conformityCsv = GetResultValue(dt, "17818");

            if (!string.IsNullOrEmpty(objectNamesCsv))
            {
                string[] objectNames = objectNamesCsv.Split(',');
                string[] manufacturers = manufacturersCsv?.Split(',') ?? new string[0];
                string[] models = modelsCsv?.Split(',') ?? new string[0];
                string[] techData = techDataCsv?.Split(',') ?? new string[0];
                string[] standards = standardsCsv?.Split(',') ?? new string[0];
                string[] conformity = conformityCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < objectNames.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["ObjectName"] = objectNames[i].Trim();
                    dr["Manufacturer"] = (i < manufacturers.Length) ? manufacturers[i].Trim() : "";
                    dr["TypeOrModel"] = (i < models.Length) ? models[i].Trim() : "";
                    dr["TechnicalData"] = (i < techData.Length) ? techData[i].Trim() : "";
                    dr["Standard"] = (i < standards.Length) ? standards[i].Trim() : "";
                    dr["SignOfConformity"] = (i < conformity.Length) ? conformity[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData"] = gridDt;
            gvTests.DataSource = gridDt;
            gvTests.DataBind();
        }

        private void PrefillAllData(DataTable dt)
        {
            PrefillSingleValues(dt);
            PrefillTwoValues(dt);
            PrefillSixValues(dt);
        }

        private void PrefillSingleValues(DataTable dt)
        {
            Dictionary<string, TextBox> singleValueMap = new Dictionary<string, TextBox>
            {
                { "17819", txtActinicUV1 },
                { "17820", txtActinicUV2 },
                { "17821", txtActinicUV3 },
                { "17822", txtNearUV1 },
                { "17823", txtNearUV2 },
                { "17824", txtNearUV3 },
                { "17825", txtBlueLight1 },
                { "17826", txtBlueLight2 },
                { "17827", txtBlueLight3 },
                { "17828", txtBlueLightHazardRadiance },
                { "17829", txtLuminance }
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (singleValueMap.ContainsKey(paramId))
                {
                    singleValueMap[paramId].Text = rawValue;
                }
            }
        }

        private void PrefillTwoValues(DataTable dt)
        {
            Dictionary<string, (TextBox result, TextBox remark)> twoValueMap = new Dictionary<string, (TextBox, TextBox)>
            {
                { "17830", (txtProductTypeResult, txtProductTypeRemark) },
                { "17831", (txtTheLightsUsedResult, txtTheLightsUsedRemark) },
                { "17832", (txtTypeOfLEDDriverUsedResult, txtTypeOfLEDDriverUsedRemark) },
                { "17833", (txtMountingPositionDuringTestingResult, txttxtMountingPositionDuringTestingRemark) },
                { "17834", (txtPowerWResult, txtPowerWRemark) },
                { "17835", (txtCurrentAResult, txtCurrentARemark) },
                { "17836", (txtTest1RatedVoltageResult, txtTest1RatedVoltageRemark) },
                { "17837", (txtTest2Result, txtTest2Remark) },
                { "17838", (txtTest3Result, txtTest3Remark) },
                { "17839", (txtTest4Result, txtTest4Remark) }
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (twoValueMap.ContainsKey(paramId))
                {
                    var controls = twoValueMap[paramId];
                    string[] parts = rawValue.Split('|');

                    if (parts.Length >= 2)
                    {
                        controls.result.Text = parts[0];
                        controls.remark.Text = parts[1];
                    }
                }
            }
        }

        private void PrefillSixValues(DataTable dt)
        {
            Dictionary<string, (TextBox test1, TextBox test2, TextBox test3, TextBox limitMax1, TextBox test4, TextBox limitMax2)> sixValueMap = new Dictionary<string, (TextBox, TextBox, TextBox, TextBox, TextBox, TextBox)>
            {
                { "17840", (txtTestChamberTemperatureTest1, txtTestChamberTemperatureTest2, txtTestChamberTemperatureTest3, txtTestChamberTemperatureLimitsMax1, txtTestChamberTemperatureTest4, txtTestChamberTemperatureLimitsMax2) },
                { "17841", (txtOuterSurfaceOfTheLuminaireTest1, txtOuterSurfaceOfTheLuminaireTest2, txtOuterSurfaceOfTheLuminaireTest3, txtOuterSurfaceOfTheLuminaireLimitMax1, txtOuterSurfaceOfTheLuminaireTest4, txtOuterSurfaceOfTheLuminaireLimitMax2) },
                { "17842", (txtSurfaceLEDDriverTest1, txtSurfaceLEDDriverTest2, txtSurfaceLEDDriverTest3, txtSurfaceLEDDriverLimitMax1, txtSurfaceLEDDriverTest4, txtSurfaceLEDDriverLimitMax2) },
                { "17843", (txtLEDModulePCBTest1, txtLEDModulePCBTest2, txtLEDModulePCBTest3, txtLEDModulePCBLimitMax1, txtLEDModulePCBTest4, txtLEDModulePCBLimitMax2) },
                { "17844", (txtLEDModuleEnclosurePlasticTest1, txtLEDModuleEnclosurePlasticTest2, txtLEDModuleEnclosurePlasticTest3, txtLEDModuleEnclosurePlasticLimitMax1, txtLEDModuleEnclosurePlasticTest4, txtLEDModuleEnclosurePlasticLimitMax2) },
                { "17845", (txtExternalCableTest1, txtExternalCableTest2, txtExternalCableTest3, txtExternalCableLimitMax1, txtExternalCableTest4, txtExternalCableLimitMax2) },
                { "17846", (txtInternalWiringTest1, txtInternalWiringTest2, txtInternalWiringTest3, txtInternalWiringLimitMax1, txtInternalWiringTest4, txtInternalWiringLimitMax2) }
            };

            foreach (DataRow row in dt.Rows)
            {
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (sixValueMap.ContainsKey(paramId))
                {
                    var controls = sixValueMap[paramId];
                    string[] parts = rawValue.Split('|');

                    if (parts.Length >= 6)
                    {
                        controls.test1.Text = parts[0];
                        controls.test2.Text = parts[1];
                        controls.test3.Text = parts[2];
                        controls.limitMax1.Text = parts[3];
                        controls.test4.Text = parts[4];
                        controls.limitMax2.Text = parts[5];
                    }
                }
            }
        }

        #endregion

        #region Table T3 - Photobiological Hazards (FIXED - with proper merging for columns 1, 2, 3 AND 6)
        private void CreateReportTableT3OpenXML(WordprocessingDocument doc, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}",
                "LAMPIRAN B – BAHAYA FOTOBIOLOGIS\nAPPENDIX B – PHOTOBIOLOGICAL HAZARDS");

            // Parse data
            Dictionary<int, string> measurementData = new Dictionary<int, string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["ReportParamMapID"] != DBNull.Value && row["Result"] != DBNull.Value)
                {
                    int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                    string result = row["Result"].ToString();
                    measurementData[reportParamMapID] = result;
                }
            }

            // Create main table
            Table mainTable = new Table();
            TableProperties mainTableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            mainTable.AppendChild(mainTableProps);

            // Column widths
            float[] colWidths = { 1.0f, 0.7f, 0.8f, 1.0f, 1.2f, 1.5f };
            TableGrid mainGrid = new TableGrid();
            foreach (float width in colWidths)
            {
                int widthTwips = (int)(width * 1440f);
                mainGrid.AppendChild(new GridColumn() { Width = widthTwips.ToString() });
            }
            mainTable.AppendChild(mainGrid);

            // Header row
            TableRow headerRow = new TableRow();
            headerRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            // Create header with proper line breaks using Break element
            string[][] headerParts = {
        new string[] { "Risk", "Bahaya Optik" },
        new string[] { "Symbol", "Simbol" },
        new string[] { "Unit", "Satuan" },
        new string[] { "Measurement Result", "Hasil Pengukuran" },
        new string[] { "Limit", "Batas" },
        new string[] { "Remark", "Keterangan" }
    };

            for (int col = 0; col < 6; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));

                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );

                // First line - Bold
                Run run1 = new Run();
                run1.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run1.AppendChild(new WordText() { Text = headerParts[col][0], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run1);

                // Line break
                Run breakRun = new Run();
                breakRun.AppendChild(new Break());
                para.AppendChild(breakRun);

                // Second line - Bold
                Run run2 = new Run();
                run2.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run2.AppendChild(new WordText() { Text = headerParts[col][1], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run2);

                cell.AppendChild(para);
                headerRow.AppendChild(cell);
            }
            mainTable.AppendChild(headerRow);

            int currentRow = 1;

            // Actinic UV Section - 3 rows
            AddT3SectionRow(mainTable, ref currentRow, "Actinic UV", "E_s", "W.m⁻²",
                GetMeasurementResultT3(17819, measurementData), "0,001\n(Exempt/RG0)",
                GetMeasurementResultT3(17820, measurementData), "0,003\n(Low Risk/RG1)",
                GetMeasurementResultT3(17821, measurementData), "0,03\n(Mod Risk/RG2)",
                "Risk group classification:\nRG0 (Exempt) /\nRG1 (Low Risk) /\nRG2 (Mod Risk) /\nRG3 (High Risk) /\nTest Result:\n(Pass / Fail)");

            // Near UV Section - 3 rows
            AddT3SectionRow(mainTable, ref currentRow, "Near UV", "E_UVA", "W.m⁻²",
                GetMeasurementResultT3(17822, measurementData), "10\n(Exempt/RG0)",
                GetMeasurementResultT3(17823, measurementData), "33\n(Low Risk/RG1)",
                GetMeasurementResultT3(17824, measurementData), "100\n(Mod Risk/RG2)",
                "Risk group classification:\nRG0 (Exempt) /\nRG1 (Low Risk) /\nRG2 (Mod Risk) /\nRG3 (High Risk) /\nTest Result:\n(Pass / Fail)");

            // Blue Light Section - 3 rows
            AddT3SectionRow(mainTable, ref currentRow, "Blue light", "L_B", "W.m⁻².sr⁻¹",
                GetMeasurementResultT3(17825, measurementData), "100\n(Exempt/RG0)",
                GetMeasurementResultT3(17826, measurementData), "10000\n(Low Risk/RG1)",
                GetMeasurementResultT3(17827, measurementData), "4000000\n(Mod Risk/RG2)",
                "Risk group classification:\nRG0 (Exempt) /\nRG1 (Low Risk) /\nRG2 (Mod Risk) /\nRG3 (High Risk) /\nTest Result:\n(Pass / Fail)");

            // Notes rows
            string[] notes = {
        "Note:",
        "RG: Risk Group",
        "RG0 (Exempt group): The lamp does not pose any photobiological hazard.",
        "RG1 (Low Risk): The lamp does not pose a hazard due to normal behavioral limitations on exposure.",
        "RG2 (Moderate Risk): The lamp does not pose a hazard due to the aversion response to very bright light sources or due to thermal discomfort.",
        "RG3 (High Risk): The lamp may pose a hazard even for momentary or brief exposure.",
        "The lamps which exceed the limits for RG2 (Moderate Risk) are in RG3 (High Risk)."
    };

            foreach (string note in notes)
            {
                TableRow noteRow = new TableRow();
                noteRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
                ));

                TableCell noteCell = new TableCell();
                noteCell.AppendChild(new TableCellProperties(
                    new GridSpan() { Val = 6 },
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                ));
                Paragraph notePara = new Paragraph();
                notePara.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Left }
                );
                Run noteRun = new Run();
                RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                if (note == "Note:")
                    runProps.AppendChild(new Bold());
                if (note != "Note:" && note != "RG: Risk Group")
                    runProps.AppendChild(new Italic());
                noteRun.AppendChild(runProps);
                noteRun.AppendChild(new WordText() { Text = note, Space = SpaceProcessingModeValues.Preserve });
                notePara.AppendChild(noteRun);
                noteCell.AppendChild(notePara);
                noteRow.AppendChild(noteCell);
                mainTable.AppendChild(noteRow);
                currentRow++;
            }

            // Insert main table
            targetParagraph.Parent.InsertAfter(mainTable, targetParagraph);

            // Create second table - Blue light hazard radiance and Luminance
            Paragraph spacingPara = new Paragraph(new Run(new Break()));
            targetParagraph.Parent.InsertAfter(spacingPara, mainTable);

            Table secondTable = new Table();
            TableProperties secondTableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            secondTable.AppendChild(secondTableProps);

            float[] secondColWidths = { 1.8f, 0.8f, 0.8f, 2.6f };
            TableGrid secondGrid = new TableGrid();
            foreach (float width in secondColWidths)
            {
                int widthTwips = (int)(width * 1440f);
                secondGrid.AppendChild(new GridColumn() { Width = widthTwips.ToString() });
            }
            secondTable.AppendChild(secondGrid);

            // Second table header
            TableRow secondHeaderRow = new TableRow();
            secondHeaderRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            string[][] secondHeaderParts = {
        new string[] { "Item Test", "Item uji" },
        new string[] { "Symbol", "Simbol" },
        new string[] { "Unit", "Satuan" },
        new string[] { "Results", "Hasil uji" }
    };

            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));

                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );

                Run run1 = new Run();
                run1.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run1.AppendChild(new WordText() { Text = secondHeaderParts[col][0], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run1);

                Run breakRun2 = new Run();
                breakRun2.AppendChild(new Break());
                para.AppendChild(breakRun2);

                Run run2 = new Run();
                run2.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run2.AppendChild(new WordText() { Text = secondHeaderParts[col][1], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run2);

                cell.AppendChild(para);
                secondHeaderRow.AppendChild(cell);
            }
            secondTable.AppendChild(secondHeaderRow);

            // Data rows for second table
            string[] secondData = {
        "Blue light hazard radiance",
        "L_B",
        "W/m².sr",
        GetMeasurementResultT3(17828, measurementData)
    };

            TableRow dataRow1 = new TableRow();
            dataRow1.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
            ));
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = col == 0 ? JustificationValues.Left : JustificationValues.Center }
                );
                Run run = new Run();
                run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                run.AppendChild(new WordText() { Text = secondData[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                dataRow1.AppendChild(cell);
            }
            secondTable.AppendChild(dataRow1);

            string[] secondData2 = {
        "Luminance",
        "L_v",
        "cd/m²",
        GetMeasurementResultT3(17829, measurementData)
    };

            TableRow dataRow2 = new TableRow();
            dataRow2.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
            ));
            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = col == 0 ? JustificationValues.Left : JustificationValues.Center }
                );
                Run run = new Run();
                run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                run.AppendChild(new WordText() { Text = secondData2[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                dataRow2.AppendChild(cell);
            }
            secondTable.AppendChild(dataRow2);

            // Insert second table
            Paragraph spacingPara2 = new Paragraph(new Run(new Break()));
            targetParagraph.Parent.InsertAfter(spacingPara2, mainTable);
            targetParagraph.Parent.InsertAfter(secondTable, spacingPara2);
        }

        /// <summary>
        /// Adds a section row with proper vertical merging for columns 1, 2, 3, and 6
        /// Remark column (column 6) IS merged vertically across 3 rows
        /// </summary>
        private void AddT3SectionRow(Table table, ref int currentRow, string riskText, string symbolText, string unitText,
            string result1, string limit1, string result2, string limit2, string result3, string limit3, string remarkText)
        {
            // Row 1 - has all data, starts vertical merge for columns 1, 2, 3, and 6
            TableRow row1 = new TableRow();
            row1.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
            ));
            row1.AppendChild(CreateMergedCell(riskText, JustificationValues.Center, true, MergedCellValues.Restart));     // Column 1: Risk (merged)
            row1.AppendChild(CreateMergedCell(symbolText, JustificationValues.Center, true, MergedCellValues.Restart));   // Column 2: Symbol (merged)
            row1.AppendChild(CreateMergedCell(unitText, JustificationValues.Center, true, MergedCellValues.Restart));     // Column 3: Unit (merged)
            row1.AppendChild(CreateRegularCell(result1, JustificationValues.Center, false));                              // Column 4: Result
            row1.AppendChild(CreateRegularCell(limit1, JustificationValues.Center, false));                              // Column 5: Limit
            row1.AppendChild(CreateMergedCell(remarkText, JustificationValues.Left, false, MergedCellValues.Restart));   // Column 6: Remark (merged)
            table.AppendChild(row1);

            // Row 2 - continues vertical merge for columns 1, 2, 3, and 6
            TableRow row2 = new TableRow();
            row2.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
            ));
            row2.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));   // Column 1: Risk (merged)
            row2.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));   // Column 2: Symbol (merged)
            row2.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));   // Column 3: Unit (merged)
            row2.AppendChild(CreateRegularCell(result2, JustificationValues.Center, false));                       // Column 4: Result
            row2.AppendChild(CreateRegularCell(limit2, JustificationValues.Center, false));                       // Column 5: Limit
            row2.AppendChild(CreateMergedCell("", JustificationValues.Left, false, MergedCellValues.Continue));   // Column 6: Remark (merged, empty)
            table.AppendChild(row2);

            // Row 3 - continues vertical merge for columns 1, 2, 3, and 6
            TableRow row3 = new TableRow();
            row3.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
            ));
            row3.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));   // Column 1: Risk (merged)
            row3.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));   // Column 2: Symbol (merged)
            row3.AppendChild(CreateMergedCell("", JustificationValues.Center, true, MergedCellValues.Continue));   // Column 3: Unit (merged)
            row3.AppendChild(CreateRegularCell(result3, JustificationValues.Center, false));                       // Column 4: Result
            row3.AppendChild(CreateRegularCell(limit3, JustificationValues.Center, false));                       // Column 5: Limit
            row3.AppendChild(CreateMergedCell("", JustificationValues.Left, false, MergedCellValues.Continue));   // Column 6: Remark (merged, empty)
            table.AppendChild(row3);

            currentRow += 3;
        }

        /// <summary>
        /// Creates a cell with vertical merge properties (for columns 1, 2, 3, and 6)
        /// </summary>
        private TableCell CreateMergedCell(string text, JustificationValues alignment, bool isBold, MergedCellValues mergeValue)
        {
            TableCell cell = new TableCell();
            TableCellProperties cellProps = new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            );
            cellProps.AppendChild(new VerticalMerge() { Val = mergeValue });
            cell.AppendChild(cellProps);

            Paragraph para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = alignment }
            );

            // Handle multi-line text with \n
            if (!string.IsNullOrEmpty(text) && text.Contains("\n"))
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    Run run = new Run();
                    RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                    if (isBold)
                        runProps.AppendChild(new Bold());
                    run.AppendChild(runProps);
                    run.AppendChild(new WordText() { Text = lines[i], Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);

                    if (i < lines.Length - 1)
                    {
                        Run breakRun = new Run();
                        breakRun.AppendChild(new Break());
                        para.AppendChild(breakRun);
                    }
                }
            }
            else
            {
                Run run = new Run();
                RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                if (isBold)
                    runProps.AppendChild(new Bold());
                run.AppendChild(runProps);
                run.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
            }

            cell.AppendChild(para);
            return cell;
        }

        /// <summary>
        /// Creates a regular cell (for columns 4 and 5 - Result, Limit)
        /// </summary>
        private TableCell CreateRegularCell(string text, JustificationValues alignment, bool isBold)
        {
            TableCell cell = new TableCell();
            cell.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
            ));

            Paragraph para = new Paragraph();
            para.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = alignment }
            );

            // Handle multi-line text with \n
            if (!string.IsNullOrEmpty(text) && text.Contains("\n"))
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    Run run = new Run();
                    RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                    if (isBold)
                        runProps.AppendChild(new Bold());
                    run.AppendChild(runProps);
                    run.AppendChild(new WordText() { Text = lines[i], Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);

                    if (i < lines.Length - 1)
                    {
                        Run breakRun = new Run();
                        breakRun.AppendChild(new Break());
                        para.AppendChild(breakRun);
                    }
                }
            }
            else
            {
                Run run = new Run();
                RunProperties runProps = new RunProperties(new FontSize() { Val = "18" });
                if (isBold)
                    runProps.AppendChild(new Bold());
                run.AppendChild(runProps);
                run.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
            }

            cell.AppendChild(para);
            return cell;
        }

        private string GetMeasurementResultT3(int reportParamMapID, Dictionary<int, string> measurementData)
        {
            if (measurementData.ContainsKey(reportParamMapID))
            {
                string result = measurementData[reportParamMapID];
                return string.IsNullOrEmpty(result) ? "" : result;
            }
            return "";
        }

        #endregion

        #region Table T4 - Thermal Testing (FIXED)
        private void CreateReportTableT4OpenXML(WordprocessingDocument doc, DataTable dt)
        {
            var mainPart = doc.MainDocumentPart;
            var body = mainPart.Document.Body;

            Paragraph targetParagraph = FindParagraphContaining(body, "{LAMPIRAN_D-UJI_TERMAL}");
            if (targetParagraph == null)
                throw new Exception("Placeholder '{LAMPIRAN_D-UJI_TERMAL}' not found in the document template.");

            ReplaceInParagraphOpenXML(targetParagraph, "{LAMPIRAN_D-UJI_TERMAL}",
                "LAMPIRAN D – UJI TERMAL\nAPPENDIX D – THERMAL TESTING");

            // Parse data
            Dictionary<int, string[]> measurementData = new Dictionary<int, string[]>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["ReportParamMapID"] != DBNull.Value && row["Result"] != DBNull.Value)
                {
                    int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
                    string result = row["Result"].ToString();
                    string[] values = result.Split('|');
                    measurementData[reportParamMapID] = values;
                }
            }

            // Create first table (Klausul table) - 10 rows
            Table firstTable = new Table();
            TableProperties firstTableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            firstTable.AppendChild(firstTableProps);

            float[] firstColWidths = { 0.6f, 3.5f, 1.2f, 1.2f };
            TableGrid firstGrid = new TableGrid();
            foreach (float width in firstColWidths)
            {
                int widthTwips = (int)(width * 1440f);
                firstGrid.AppendChild(new GridColumn() { Width = widthTwips.ToString() });
            }
            firstTable.AppendChild(firstGrid);

            // First table header
            TableRow firstHeaderRow = new TableRow();
            firstHeaderRow.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            string[] firstHeaders = {
        "Klausul\nclause",
        "Persyaratan\nRequirement",
        "Hasil\nResult",
        "*Keterangan\nRemark*"
    };

            for (int col = 0; col < 4; col++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph para = new Paragraph();
                para.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
                Run run = new Run();
                run.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                run.AppendChild(new WordText() { Text = firstHeaders[col], Space = SpaceProcessingModeValues.Preserve });
                para.AppendChild(run);
                cell.AppendChild(para);
                firstHeaderRow.AppendChild(cell);
            }
            firstTable.AppendChild(firstHeaderRow);

            // First table data - 10 rows
            string[,] firstTableData = {
        { "1.", "Tipe produk.", GetMeasurementValue(17830, 0, measurementData), GetMeasurementValue(17830, 1, measurementData) },
        { "2.", "Lampu yang digunakan.", GetMeasurementValue(17831, 0, measurementData), GetMeasurementValue(17831, 1, measurementData) },
        { "3.", "Tipe LED driver yang digunakan.", GetMeasurementValue(17832, 0, measurementData), GetMeasurementValue(17832, 1, measurementData) },
        { "4.", "Posisi pemasangan saat pengujian.", GetMeasurementValue(17833, 0, measurementData), GetMeasurementValue(17833, 1, measurementData) },
        { "5.", "Daya (W).", GetMeasurementValue(17834, 0, measurementData), GetMeasurementValue(17834, 1, measurementData) },
        { "6.", "Arus (A).", GetMeasurementValue(17835, 0, measurementData), GetMeasurementValue(17835, 1, measurementData) },
        { "7.", "Uji 1:\nTegangan pengenal (V).", GetMeasurementValue(17836, 0, measurementData), GetMeasurementValue(17836, 1, measurementData) },
        { "8.", "Uji 2:\n1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17837, 0, measurementData), GetMeasurementValue(17837, 1, measurementData) },
        { "9.", "Uji 3:\nBakan dengan dilengkapi kotak kontak 1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17838, 0, measurementData), GetMeasurementValue(17838, 1, measurementData) },
        { "10.", "Uji 4:\n1,1 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17839, 0, measurementData), GetMeasurementValue(17839, 1, measurementData) }
    };

            for (int row = 0; row < 10; row++)
            {
                TableRow dataRow = new TableRow();
                dataRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
                ));

                for (int col = 0; col < 4; col++)
                {
                    TableCell cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = (col == 0 || col == 2) ? JustificationValues.Center : JustificationValues.Left }
                    );
                    Run run = new Run();
                    run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    run.AppendChild(new WordText() { Text = firstTableData[row, col] ?? "", Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    dataRow.AppendChild(cell);
                }
                firstTable.AppendChild(dataRow);
            }

            targetParagraph.Parent.InsertAfter(firstTable, targetParagraph);

            // Spacing between tables
            Paragraph spacingPara = new Paragraph(new Run(new Break()));
            targetParagraph.Parent.InsertAfter(spacingPara, firstTable);

            // Create second table - Thermal testing results (7 rows with proper header merging)
            Table secondTable = new Table();
            TableProperties secondTableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Fixed },
                new TableWidth() { Width = USABLE_WIDTH_TWIPS.ToString(), Type = TableWidthUnitValues.Dxa }
            );
            secondTable.AppendChild(secondTableProps);

            float[] secondColWidths = { 1.5f, 0.8f, 0.8f, 0.8f, 1.0f, 0.8f, 1.0f };
            TableGrid secondGrid = new TableGrid();
            foreach (float width in secondColWidths)
            {
                int widthTwips = (int)(width * 1440f);
                secondGrid.AppendChild(new GridColumn() { Width = widthTwips.ToString() });
            }
            secondTable.AppendChild(secondGrid);

            // ========================
            // SECOND TABLE - HEADER ROW 1
            // ========================
            TableRow headerRow1 = new TableRow();
            headerRow1.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            // Cell 1: "Bagian yang diuji" - rowspan=2
            TableCell cell1 = new TableCell();
            cell1.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
            ));
            cell1.AppendChild(new VerticalMerge() { Val = MergedCellValues.Restart });
            Paragraph p1 = new Paragraph();
            p1.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            Run r1 = new Run();
            r1.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
            r1.AppendChild(new WordText() { Text = "Bagian yang diuji", Space = SpaceProcessingModeValues.Preserve });
            p1.AppendChild(r1);
            cell1.AppendChild(p1);
            headerRow1.AppendChild(cell1);

            // Cell 2: "Klausul 12.4 – Uji termal (Operasi normal)" - colspan=4
            TableCell cell2 = new TableCell();
            cell2.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new GridSpan() { Val = 4 }
            ));
            Paragraph p2 = new Paragraph();
            p2.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            Run r2 = new Run();
            r2.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
            r2.AppendChild(new WordText() { Text = "Klausul 12.4 – Uji termal\n(Operasi normal)", Space = SpaceProcessingModeValues.Preserve });
            p2.AppendChild(r2);
            cell2.AppendChild(p2);
            headerRow1.AppendChild(cell2);

            // Cell 3: "Klausul 12.5 – Uji termal (Operasi abnormal)" - colspan=2
            TableCell cell3 = new TableCell();
            cell3.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" },
                new GridSpan() { Val = 2 }
            ));
            Paragraph p3 = new Paragraph();
            p3.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            Run r3 = new Run();
            r3.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
            r3.AppendChild(new WordText() { Text = "Klausul 12.5 – Uji termal\n(Operasi abnormal)", Space = SpaceProcessingModeValues.Preserve });
            p3.AppendChild(r3);
            cell3.AppendChild(p3);
            headerRow1.AppendChild(cell3);

            secondTable.AppendChild(headerRow1);

            // ========================
            // SECOND TABLE - HEADER ROW 2
            // ========================
            TableRow headerRow2 = new TableRow();
            headerRow2.AppendChild(new TableRowProperties(
                new TableRowHeight() { Val = 400, HeightType = HeightRuleValues.AtLeast }
            ));

            // Cell 1: Empty (merged with cell1 from row 1 via VerticalMerge.Continue)
            TableCell cell1Row2 = new TableCell();
            cell1Row2.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
            ));
            cell1Row2.AppendChild(new VerticalMerge() { Val = MergedCellValues.Continue });
            headerRow2.AppendChild(cell1Row2);

            // Cells 2-7: Sub-headers (Uji 1, Uji 2, Uji 3, Limit Maks., Uji 4, Limit Maks.)
            string[] subHeaders = { "Uji 1", "Uji 2", "Uji 3", "Limit Maks.", "Uji 4", "Limit Maks." };
            for (int i = 0; i < subHeaders.Length; i++)
            {
                TableCell cell = new TableCell();
                cell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "D3D3D3" }
                ));
                Paragraph p = new Paragraph();
                p.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                Run r = new Run();
                r.AppendChild(new RunProperties(new Bold(), new FontSize() { Val = "20" }));
                r.AppendChild(new WordText() { Text = subHeaders[i], Space = SpaceProcessingModeValues.Preserve });
                p.AppendChild(r);
                cell.AppendChild(p);
                headerRow2.AppendChild(cell);
            }

            secondTable.AppendChild(headerRow2);

            // ========================
            // SECOND TABLE - DATA ROWS (7 rows)
            // ========================
            string[,] secondTableData = {
        { "Suhu ruang uji.",
          GetMeasurementValue(17840, 0, measurementData), GetMeasurementValue(17840, 1, measurementData),
          GetMeasurementValue(17840, 2, measurementData), GetMeasurementValue(17840, 3, measurementData),
          GetMeasurementValue(17840, 4, measurementData), GetMeasurementValue(17840, 5, measurementData) },
        { "Permukaan luar luminer",
          GetMeasurementValue(17841, 0, measurementData), GetMeasurementValue(17841, 1, measurementData),
          GetMeasurementValue(17841, 2, measurementData), GetMeasurementValue(17841, 3, measurementData),
          GetMeasurementValue(17841, 4, measurementData), GetMeasurementValue(17841, 5, measurementData) },
        { "Permukaan LED Driver.",
          GetMeasurementValue(17842, 0, measurementData), GetMeasurementValue(17842, 1, measurementData),
          GetMeasurementValue(17842, 2, measurementData), GetMeasurementValue(17842, 3, measurementData),
          GetMeasurementValue(17842, 4, measurementData), GetMeasurementValue(17842, 5, measurementData) },
        { "LED module / PCB.",
          GetMeasurementValue(17843, 0, measurementData), GetMeasurementValue(17843, 1, measurementData),
          GetMeasurementValue(17843, 2, measurementData), GetMeasurementValue(17843, 3, measurementData),
          GetMeasurementValue(17843, 4, measurementData), GetMeasurementValue(17843, 5, measurementData) },
        { "Selungkup LED module\n(Plastik)",
          GetMeasurementValue(17844, 0, measurementData), GetMeasurementValue(17844, 1, measurementData),
          GetMeasurementValue(17844, 2, measurementData), GetMeasurementValue(17844, 3, measurementData),
          GetMeasurementValue(17844, 4, measurementData), GetMeasurementValue(17844, 5, measurementData) },
        { "Kabel eksternal.",
          GetMeasurementValue(17845, 0, measurementData), GetMeasurementValue(17845, 1, measurementData),
          GetMeasurementValue(17845, 2, measurementData), GetMeasurementValue(17845, 3, measurementData),
          GetMeasurementValue(17845, 4, measurementData), GetMeasurementValue(17845, 5, measurementData) },
        { "Kabel internal.",
          GetMeasurementValue(17846, 0, measurementData), GetMeasurementValue(17846, 1, measurementData),
          GetMeasurementValue(17846, 2, measurementData), GetMeasurementValue(17846, 3, measurementData),
          GetMeasurementValue(17846, 4, measurementData), GetMeasurementValue(17846, 5, measurementData) }
    };

            for (int row = 0; row < 7; row++)
            {
                TableRow dataRow = new TableRow();
                dataRow.AppendChild(new TableRowProperties(
                    new TableRowHeight() { Val = 250, HeightType = HeightRuleValues.AtLeast }
                ));

                // First column: left-aligned
                TableCell firstCell = new TableCell();
                firstCell.AppendChild(new TableCellProperties(
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                ));
                Paragraph firstPara = new Paragraph();
                firstPara.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Left }
                );
                Run firstRun = new Run();
                firstRun.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                firstRun.AppendChild(new WordText() { Text = secondTableData[row, 0] ?? "", Space = SpaceProcessingModeValues.Preserve });
                firstPara.AppendChild(firstRun);
                firstCell.AppendChild(firstPara);
                dataRow.AppendChild(firstCell);

                // Columns 2-7: center-aligned
                for (int col = 1; col < 7; col++)
                {
                    TableCell cell = new TableCell();
                    cell.AppendChild(new TableCellProperties(
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center }
                    ));
                    Paragraph para = new Paragraph();
                    para.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    );
                    Run run = new Run();
                    run.AppendChild(new RunProperties(new FontSize() { Val = "18" }));
                    string text = secondTableData[row, col] ?? "";
                    run.AppendChild(new WordText() { Text = text, Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(run);
                    cell.AppendChild(para);
                    dataRow.AppendChild(cell);
                }
                secondTable.AppendChild(dataRow);
            }

            targetParagraph.Parent.InsertAfter(secondTable, spacingPara);
        }

        private string GetMeasurementValue(int reportParamMapID, int index, Dictionary<int, string[]> measurementData)
        {
            if (measurementData.ContainsKey(reportParamMapID))
            {
                string[] values = measurementData[reportParamMapID];
                if (index < values.Length)
                {
                    return values[index];
                }
            }
            return string.Empty;
        }

        #endregion

        protected void btnExport_Click(object sender, EventArgs e)
        {
            GenerateAndExportReport();
        }

    }
}
