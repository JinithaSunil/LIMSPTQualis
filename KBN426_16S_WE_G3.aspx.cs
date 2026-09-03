using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;
using Table = Microsoft.Office.Interop.Word.Table;


namespace LIMS
{
    public partial class KBN426_16S_WE_G3 : System.Web.UI.Page
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

                BindEquipmentCheckboxes();
                LoadClauseTable();

                CreateGridTable();

                DataTable existingData = _cm.GetExistingTestResults_LED_RP(lblSampleID.Text, lblReportID.Text);

                if (existingData != null && existingData.Rows.Count > 0)
                {
                    PrefillTestResultsGrid(existingData);
                    PrefillAllData(existingData);
                }

                txtRequestNo.Enabled = false;
                txtstandard.Enabled = false;
                txtType.Enabled = false;
                //txttestMethodNo.Enabled = false;
                txtworkorder.Enabled = false;
                //txtmodel.Enabled = false;

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

                // Convert DateTime to proper format for Date TextMode
                if (!string.IsNullOrEmpty(saved.Rows[0]["TestDate"].ToString()))
                {
                    DateTime testDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestDate"].ToString(), out testDate))
                    {
                        txtstartingdate.Text = testDate.ToString("yyyy-MM-dd");
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
            int reportID = Convert.ToInt32(lblReportID.Text);
            string standardID =  lblstandardID.Text;
            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_EA(sampleId, reportID,standardID);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                int departmentID = Convert.ToInt32(dt1.Rows[0]["DepartmentID"]);
                int productCategoryID = Convert.ToInt32(dt1.Rows[0]["ProductCategoryID"]);
                string StandardID = dt1.Rows[0]["StandardID"].ToString();
                string TestStandardGet = dts.Rows[0]["TestStandard"].ToString();

                txtType.Text = dt1.Rows[0]["ReportName"].ToString();
                //txtbrand.Text = dt1.Rows[0]["BrandProduct"].ToString();
                //txtmodel.Text = dt1.Rows[0]["ProductSerialNo"].ToString();
                txtworkorder.Text = dt1.Rows[0]["WorkorderNumber"].ToString();
                //txttestMethodNo.Text = dts.Rows[0]["TestMethod"].ToString();
                string sis = dts.Rows[0]["sampleIDSeries"].ToString();  // e.g. "smp/20251112"
                string requestNo = "";

                if (!string.IsNullOrEmpty(sis) && sis.Contains("/"))
                {
                    string[] parts = sis.Split('/');
                    if (parts.Length > 1)
                    {
                        requestNo = "R/" + parts[1];
                    }
                    
                }

                txtRequestNo.Text = requestNo;
                ViewState["SampleIDSeriesVS"] = dts.Rows[0]["sampleIDSeries"].ToString();

                // Split StandardID by commas
                string[] ids = StandardID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> standardIdList = ids.Select(id => id.Trim()).ToList();
                // Filter clauses based on Standard_ID match
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


                sb.Append("<table class='clause-table'>");

                // Add table header only once
                sb.Append("<tr class='sub-header-row'>");
                sb.Append("<th class='clause-number'>Klausul<br/>Clause</th>");
                sb.Append("<th class='requirement'>Persyaratan<br/>Requirements</th>");
                sb.Append("<th class='result'>Keputusan<br/>Decision</th>");
                sb.Append("<th class='remark'>*Keterangan<br/>Remark</th>");
                sb.Append("</tr>");

                foreach (DataRow row in filteredClauseTable.Rows)
                {
                    bool isHeader = row["isHeader"].ToString() == "1";

                    if (isHeader)
                    {
                        // Just make a header row, no new table
                        sb.Append("<tr class='header-row'>");
                        sb.Append("<td>");
                        sb.Append($"<b>{row["ClauseNumber"]}</b><br/>");
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

                sb.Append("</table>");


                /*
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
                        sb.Append("<th class='result'>Hasil<br/>Result</th>");
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

                        // Add remarks textbox for header
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
                */
                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
            }
        }

        private void BindEquipmentCheckboxes()
        {
            int DepartmentId =Convert.ToInt32(Session["DeptID"].ToString());
            string query = "SELECT * FROM Calibration_Equipment WHERE DepartmentId = @DepartmentId";

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
                    //reader.Close();
                    con.Close();
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
                bool b1 = false; bool b2 = false; bool b3 = false;

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
                            //int result0 = _cm.TestResultsAdd_LED(clauseID, txtstartingdate.Text, txtcompletiondate.Text, "", remark, sampleId, sampleIdSeries,
                            //     "header", language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                            //     txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text);
                            //if (result0 > 0)
                            //{
                            //    b1 = true;
                            //}
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

                            //int result0 = _cm.TestResultsAdd_LED(clauseID, txtstartingdate.Text, txtcompletiondate.Text, result, remark, sampleId, sampleIdSeries,
                            //    inputType, language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                            //    txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text);

                            //if (result0 > 0)
                            //{
                            //    b1 = true;
                            //}
                        }
                    }

                    /*WITHOUT CLAUSES DATA*/

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

                        // Collect data from all rows in the DataTable
                        foreach (DataRow row in dtRepoParam.Rows)
                        {
                            objectNames.Add(row["ObjectName"].ToString());
                            manufacturers.Add(row["Manufacturer"].ToString());
                            typeOrModels.Add(row["TypeOrModel"].ToString());
                            technicalDatas.Add(row["TechnicalData"].ToString());
                            standards.Add(row["Standard"].ToString());
                            signOfConformities.Add(row["SignOfConformity"].ToString());
                        }

                        // Convert lists to comma-separated strings
                        string objectNamesCsv = string.Join(",", objectNames);
                        string manufacturersCsv = string.Join(",", manufacturers);
                        string typeOrModelsCsv = string.Join(",", typeOrModels);
                        string technicalDatasCsv = string.Join(",", technicalDatas);
                        string standardsCsv = string.Join(",", standards);
                        string signOfConformitiesCsv = string.Join(",", signOfConformities);

                        // Prepare data for saving
                        List<string> reportParamMapList = new List<string>();
                        List<string> resultList = new List<string>();

                        // Add the data with corresponding parameter IDs
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


                        // Convert to arrays
                        string[] reportParamMap = reportParamMapList.ToArray();
                        string[] resultNew = resultList.ToArray();

                        //// Save to database
                        int modified = 1;
                            //_cm.TestResultsAdd_LED_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "", sampleId, sampleIdSeries,
                            //        "", language, ReportID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                            //        txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text, txthumidity.Text, reportParamMap, resultNew);

                        if (modified > 0)
                        {
                            //Response.Write("<script>alert('Data saved successfully!'); window.location.href = window.location.href;</script>");
                            int sampleStatusID = 27;
                            int res1 = _cm.UpdateCRFSampleDetailsStatus_LED(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
                            if (res1 > 0)
                            {
                                int res2 = _cm.UpdateCRFCloneStatus_LED(sampleStatusID, cloneCFRsampleID);

                                if (res2 > 0)
                                {
                                    int res3 = _cm.updateSampleDetailStatus_LED(sampleStatusID, lblSampleID.Text);

                                    if (res3 > 0)
                                    {
                                        b2 = true;
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }

                        }


                        if (b2 == true)
                        {
                            /* Report Generate Code Start */
                            string SampleUID = "SID=" + lblSampleID.Text + "_RPID=" + lblReportID.Text;
                            string pathout = Server.MapPath("~/Reports/");
                            //string templatePath = Server.MapPath("~/InputFiles/LED_ReportTemplate.docx");
                            string templatePath = Server.MapPath("~/InputFiles/LEDTestRecord_1.docx");

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
                                    CreateBasicTemplate(doc);
                                }

                                DataTable dtReport = GetReportData();
                                CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                                DataTable dtReportT2 = GetReportDataForT2();
                                CreateReportTableT2(wordApp, doc, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}", dtReportT2);

                                DataTable dtReportT3 = GetReportDataForT2();
                                CreateReportTableT3(wordApp, doc, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}", dtReportT3);

                                DataTable dtReportT4 = GetReportDataForT2();
                                CreateReportTableT4(wordApp, doc, "{LAMPIRAN_D-UJI_TERMAL}", dtReportT4);

                                // Save as Word and then PDF
                                doc.SaveAs2(outputDocxPath);
                                doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);

                                // Return PDF to browser
                                Response.Clear();
                                Response.ContentType = "application/pdf";

                                // 'inline' opens in browser/tab, 'attachment' prompts download
                                Response.AddHeader("Content-Disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                                Response.TransmitFile(outputPdfPath);
                                Response.Flush();
                                Response.End();

                                //doc.SaveAs2(outputDocxPath);
                                //doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);
                                //Response.Clear();
                                //Response.ContentType = "application/pdf";
                                //Response.AddHeader("content-disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                                //Response.WriteFile(outputPdfPath);
                                //Response.End();
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
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

                                /*Force - kill any lingering WINWORD processes started by this user session for locally server */
                                foreach (var process in Process.GetProcessesByName("WINWORD"))
                                {
                                    if (string.IsNullOrEmpty(process.MainWindowTitle))
                                    {
                                        process.Kill();
                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save data.');", true);
                        //return false;
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string SampleUID = Guid.NewGuid().ToString("N").Substring(0, 6);
            string pathout = Server.MapPath("~/Reports/");
            //string templatePath = Server.MapPath("~/InputFiles/LED_ReportTemplate.docx");
            string templatePath = Server.MapPath("~/InputFiles/LEDTestRecord_1.docx");

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
                    CreateBasicTemplate(doc);
                }

                DataTable dtReport = GetReportData();
                CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                DataTable dtReportT2 = GetReportDataForT2();
                CreateReportTableT2(wordApp, doc, "{LAMPIRAN_A-DAFTAR_KOMPONEN_LUMINER}", dtReportT2);

                DataTable dtReportT3 = GetReportDataForT2();
                CreateReportTableT3(wordApp, doc, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}", dtReportT3);

                DataTable dtReportT4 = GetReportDataForT2();
                CreateReportTableT4(wordApp, doc, "{LAMPIRAN_D-UJI_TERMAL}", dtReportT4);

                doc.SaveAs2(outputDocxPath);
                doc.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"inline; filename=TestReport-{SampleUID}.pdf");
                Response.WriteFile(outputPdfPath);
                Response.End();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
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

                /*Force - kill any lingering WINWORD processes started by this user session for locally server */
                foreach (var process in Process.GetProcessesByName("WINWORD"))
                {
                    if (string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        process.Kill();
                    }
                }
            }
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

            // COMPLETELY REMOVE SPACING - Use exact line spacing
            range.Text = "LAMPIRAN A – DAFTAR KOMPONEN LUMINER" + (char)11 + "APPENDIX A – LIST OF LUMINARY COMPONENTS";
            range.Font.Size = 12;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

            // Remove all spacing
            range.ParagraphFormat.SpaceBefore = 0;
            range.ParagraphFormat.SpaceAfter = 0;
            range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
            range.ParagraphFormat.LineSpacing = 14;

            // Create table immediately after heading with NO extra space
            Range tableRange = doc.Range(range.End, range.End);
            tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

            // NO extra paragraph after - create table directly
            // tableRange.InsertParagraphAfter(); // REMOVE THIS LINE

            // Define the mapping between ReportParamMapID and column positions
            Dictionary<int, int> columnMapping = new Dictionary<int, int>
            {
                { 17813, 0 }, // Nama Objek
                { 17814, 1 }, // Pabrikan
                { 17815, 2 }, // Tipe / Model
                { 17816, 3 }, // Data Teknis
                { 17817, 4 }, // Standard
                { 17818, 5 }  // Tanda Kesesuaian
            };

            // Parse data based on ReportParamMapID
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

            // Determine number of rows based on the maximum length of any column
            int numRows = parsedColumns.Max(col => col.Length);
            int numCols = 6;

            // Create table using tableRange instead of range
            Table table = doc.Tables.Add(tableRange, numRows + 1, numCols);
            table.Borders.Enable = 1;
            table.AllowAutoFit = false;

            float totalWidthInches = 6.5f;
            table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

            // Adjusted column widths
            float[] colWidths = { 1.2f, 1.0f, 1.0f, 1.0f, 0.8f, 1.5f };
            for (int i = 0; i < numCols; i++)
            {
                table.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
            }

            // Set header row with proper two-line formatting
            string[] headers = {
                                "Nama Objek\r\nObject Name",
                                "Pabrikan\r\nManufacturer",
                                "Tipe/Model\r\nType/Model",
                                "Data Teknis\r\nTechnical Data",
                                "Standard",
                                "Tanda Kesesuaian\r\nConformity Mark"
                                };

            // Set the header row height to accommodate two lines
            table.Rows[1].Height = wordApp.InchesToPoints(0.4f); // Reduced height
            table.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

            for (int col = 1; col <= numCols; col++)
            {
                Cell cell = table.Cell(1, col);
                cell.Range.Text = headers[col - 1];
                cell.Range.Bold = 1;
                cell.Range.Font.Size = 10; // Smaller font for header
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
                table.Rows[rowIndex + 2].Height = wordApp.InchesToPoints(0.25f); // Reduced data row height
                table.Rows[rowIndex + 2].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                for (int colIndex = 0; colIndex < numCols; colIndex++)
                {
                    Cell cell = table.Cell(rowIndex + 2, colIndex + 1);
                    string text = (rowIndex < parsedColumns[colIndex].Length) ? parsedColumns[colIndex][rowIndex].Trim() : "";
                    cell.Range.Text = text;
                    cell.Range.Font.Size = 10; // Slightly smaller font for data
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
        /* T2 Start */


        /* T3 START */
        private void CreateReportTableT3(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc.Content;
                Find find = range.Find;
                find.ClearFormatting();
                find.Text = placeholder;

                if (!find.Execute())
                    throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

                // COMPLETELY REMOVE SPACING - Use exact line spacing
                range.Text = "LAMPIRAN B – BAHAYA FOTOBIOLOGIS" + (char)11 + "APPENDIX B – PHOTOBIOLOGICAL HAZARDS";
                range.Font.Size = 12;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                // Remove all spacing
                range.ParagraphFormat.SpaceBefore = 0;
                range.ParagraphFormat.SpaceAfter = 0;
                range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                range.ParagraphFormat.LineSpacing = 14;

                // Create table immediately after heading with NO extra space
                Range tableRange = doc.Range(range.End, range.End);
                tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                // Parse data from database based on ParameterID
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

                // Create main table for photobiological hazards - REDUCED to 15 rows since we removed one header row
                Table mainTable = doc.Tables.Add(tableRange, 15, 6);
                mainTable.Borders.Enable = 1;
                mainTable.AllowAutoFit = false;

                float totalWidthInches = 7.0f;
                mainTable.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

                // Adjusted column widths for the new structure
                float[] colWidths = { 1.0f, 0.7f, 0.8f, 1.0f, 1.0f, 1.5f };
                for (int i = 0; i < 6; i++)
                {
                    mainTable.Columns[i + 1].Width = wordApp.InchesToPoints(colWidths[i]);
                }

                // NEW HEADER STYLE - Single row with line breaks
                string[] mainHeaders = {
                    "Risk\r\nBahaya Optik",
                    "Symbol\r\nSimbol",
                    "Unit\r\nSatuan",
                    "Measurement Result\r\nHasil Pengukuran",
                    "Limit\r\nBatas",
                    "Remark\r\nKeterangan"
                };

                for (int col = 1; col <= 6; col++)
                {
                    SafeSetCellTextT3(mainTable, 1, col, mainHeaders[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    // Set gray background for header
                    SafeSetCellShadingT3(mainTable, 1, col, WdColor.wdColorGray25);
                }

                // Set row height to accommodate two lines
                mainTable.Rows[1].Height = wordApp.InchesToPoints(0.4f);
                mainTable.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // Actinic UV Section - ADJUSTED ROW NUMBERS (starting from row 2 now)
                SafeSetCellTextT3(mainTable, 2, 1, "Actinic UV");
                SafeSetCellTextT3(mainTable, 2, 2, "E_s");
                SafeSetCellTextT3(mainTable, 2, 3, "W.m⁻²");

                // Merge cells for Actinic UV section
                SafeMergeCellsT3(mainTable, 2, 1, 4, 1); // Risk merged
                SafeMergeCellsT3(mainTable, 2, 2, 4, 2); // Symbol merged  
                SafeMergeCellsT3(mainTable, 2, 3, 4, 3); // Unit merged
                SafeMergeCellsT3(mainTable, 2, 6, 4, 6); // Remark merged

                // Actinic UV measurement results and limits
                SafeSetCellTextT3(mainTable, 2, 4, GetMeasurementResultT3(17819, measurementData));
                SafeSetCellTextT3(mainTable, 2, 5, "0,001\r\n(Exempt/RG0)");

                SafeSetCellTextT3(mainTable, 3, 4, GetMeasurementResultT3(17820, measurementData));
                SafeSetCellTextT3(mainTable, 3, 5, "0,003\r\n(Low Risk/RG1)");

                SafeSetCellTextT3(mainTable, 4, 4, GetMeasurementResultT3(17821, measurementData));
                SafeSetCellTextT3(mainTable, 4, 5, "0,03\r\n(Mod Risk/RG2)");

                // Remark for Actinic UV
                SafeSetCellTextT3(mainTable, 2, 6, "Risk group classification:\r\nRG0 (Exempt) /\r\nRG1 (Low Risk) /\r\nRG2 (Mod Risk) /\r\nRG3 (High Risk) /\r\nTest Result:\r\n(Pass / Fail)");

                // Near UV Section - ADJUSTED ROW NUMBERS
                SafeSetCellTextT3(mainTable, 5, 1, "Near UV");
                SafeSetCellTextT3(mainTable, 5, 2, "E_UVA");
                SafeSetCellTextT3(mainTable, 5, 3, "W.m⁻²");

                // Merge cells for Near UV section
                SafeMergeCellsT3(mainTable, 5, 1, 7, 1); // Risk merged
                SafeMergeCellsT3(mainTable, 5, 2, 7, 2); // Symbol merged  
                SafeMergeCellsT3(mainTable, 5, 3, 7, 3); // Unit merged
                SafeMergeCellsT3(mainTable, 5, 6, 7, 6); // Remark merged

                // Near UV measurement results and limits
                SafeSetCellTextT3(mainTable, 5, 4, GetMeasurementResultT3(17822, measurementData));
                SafeSetCellTextT3(mainTable, 5, 5, "10\r\n(Exempt/RG0)");

                SafeSetCellTextT3(mainTable, 6, 4, GetMeasurementResultT3(17823, measurementData));
                SafeSetCellTextT3(mainTable, 6, 5, "33\r\n(Low Risk/RG1)");

                SafeSetCellTextT3(mainTable, 7, 4, GetMeasurementResultT3(17824, measurementData));
                SafeSetCellTextT3(mainTable, 7, 5, "100\r\n(Mod Risk/RG2)");

                // Remark for Near UV
                SafeSetCellTextT3(mainTable, 5, 6, "Risk group classification:\r\nRG0 (Exempt) /\r\nRG1 (Low Risk) /\r\nRG2 (Mod Risk) /\r\nRG3 (High Risk) /\r\nTest Result:\r\n(Pass / Fail)");

                // Blue Light Section - ADJUSTED ROW NUMBERS
                SafeSetCellTextT3(mainTable, 8, 1, "Blue light");
                SafeSetCellTextT3(mainTable, 8, 2, "L_B");
                SafeSetCellTextT3(mainTable, 8, 3, "W.m⁻².sr⁻¹");

                // Merge cells for Blue Light section
                SafeMergeCellsT3(mainTable, 8, 1, 10, 1); // Risk merged
                SafeMergeCellsT3(mainTable, 8, 2, 10, 2); // Symbol merged  
                SafeMergeCellsT3(mainTable, 8, 3, 10, 3); // Unit merged
                SafeMergeCellsT3(mainTable, 8, 6, 10, 6); // Remark merged

                // Blue Light measurement results and limits
                SafeSetCellTextT3(mainTable, 8, 4, GetMeasurementResultT3(17825, measurementData));
                SafeSetCellTextT3(mainTable, 8, 5, "100\r\n(Exempt/RG0)");

                SafeSetCellTextT3(mainTable, 9, 4, GetMeasurementResultT3(17826, measurementData));
                SafeSetCellTextT3(mainTable, 9, 5, "10000\r\n(Low Risk/RG1)");

                SafeSetCellTextT3(mainTable, 10, 4, GetMeasurementResultT3(17827, measurementData));
                SafeSetCellTextT3(mainTable, 10, 5, "4000000\r\n(Mod Risk/RG2)");

                // Remark for Blue Light
                SafeSetCellTextT3(mainTable, 8, 6, "Risk group classification:\r\nRG0 (Exempt) /\r\nRG1 (Low Risk) /\r\nRG2 (Mod Risk) /\r\nRG3 (High Risk) /\r\nTest Result:\r\n(Pass / Fail)");

                // Notes section - ADJUSTED ROW NUMBERS
                SafeMergeCellsT3(mainTable, 11, 1, 11, 6);
                SafeSetCellTextT3(mainTable, 11, 1, "Note:", true, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 11, 1, true);

                SafeMergeCellsT3(mainTable, 12, 1, 12, 6);
                SafeSetCellTextT3(mainTable, 12, 1, "RG: Risk Group", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 12, 1, true);

                SafeMergeCellsT3(mainTable, 13, 1, 13, 6);
                SafeSetCellTextT3(mainTable, 13, 1, "RG0 (Exempt group): The lamp does not pose any photobiological hazard.", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 13, 1, true);

                SafeMergeCellsT3(mainTable, 14, 1, 14, 6);
                SafeSetCellTextT3(mainTable, 14, 1, "RG1 (Low Risk): The lamp does not pose a hazard due to normal behavioral limitations on exposure.", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 14, 1, true);

                SafeMergeCellsT3(mainTable, 15, 1, 15, 6);
                SafeSetCellTextT3(mainTable, 15, 1, "RG2 (Moderate Risk): The lamp does not pose a hazard due to the aversion response to very bright light sources or due to thermal discomfort.\r\nRG3 (High Risk): The lamp may pose a hazard even for momentary or brief exposure.\r\nThe lamps which exceed the limits for RG2 (Moderate Risk) are in RG3 (High Risk).", false, WdParagraphAlignment.wdAlignParagraphLeft);
                SafeSetCellItalicT3(mainTable, 15, 1, true);

                // Apply formatting to all cells
                for (int row = 1; row <= 15; row++)
                {
                    for (int col = 1; col <= 6; col++)
                    {
                        if (CellExistsT3(mainTable, row, col))
                        {
                            Cell cell = mainTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Remove spacing
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;

                            // Set white background for data rows (rows 2-15)
                            if (row >= 2)
                            {
                                SafeSetCellShadingT3(mainTable, row, col, WdColor.wdColorWhite);
                            }
                        }
                    }
                }

                // Center align specific columns
                for (int row = 2; row <= 10; row++)
                {
                    for (int col = 2; col <= 5; col++) // Symbol, Unit, Measurement, Limit columns
                    {
                        if (CellExistsT3(mainTable, row, col))
                        {
                            mainTable.Cell(row, col).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                    }
                }

                // Remove table spacing
                mainTable.Range.ParagraphFormat.SpaceAfter = 0;
                mainTable.Range.ParagraphFormat.SpaceBefore = 0;

                // Add space between tables
                Range betweenTablesRange = doc.Range(mainTable.Range.End, mainTable.Range.End);
                betweenTablesRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                betweenTablesRange.InsertParagraphAfter();
                betweenTablesRange.InsertParagraphAfter();

                // Create second table for Blue light hazard radiance and Luminance - REDUCED to 4 rows
                Range secondTableRange = doc.Range(betweenTablesRange.End, betweenTablesRange.End);
                secondTableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                Table secondTable = doc.Tables.Add(secondTableRange, 4, 4);
                secondTable.Borders.Enable = 1;
                secondTable.AllowAutoFit = false;
                secondTable.PreferredWidth = wordApp.InchesToPoints(6.0f);

                // Column widths for second table
                float[] secondColWidths = { 1.8f, 0.8f, 0.8f, 2.6f };
                for (int i = 0; i < 4; i++)
                {
                    secondTable.Columns[i + 1].Width = wordApp.InchesToPoints(secondColWidths[i]);
                }

                // NEW SECOND TABLE HEADER STYLE - Single row with line breaks
                string[] secondHeaders = {
                    "Item Test\r\nItem uji",
                    "Symbol\r\nSimbol",
                    "Unit\r\nSatuan",
                    "Results\r\nHasil uji"
                };

                for (int col = 1; col <= 4; col++)
                {
                    SafeSetCellTextT3(secondTable, 1, col, secondHeaders[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    // Set gray background for header
                    SafeSetCellShadingT3(secondTable, 1, col, WdColor.wdColorGray25);
                }

                // Set row height to accommodate two lines
                secondTable.Rows[1].Height = wordApp.InchesToPoints(0.4f);
                secondTable.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // Second table data with DYNAMIC Results - ADJUSTED ROW NUMBERS (starting from row 2 now)
                string blueLightResult = GetMeasurementResultT3(17828, measurementData);
                string luminanceResult = GetMeasurementResultT3(17829, measurementData);

                // Blue light hazard radiance row (now row 2)
                SafeSetCellTextT3(secondTable, 2, 1, "Blue light hazard radiance");
                SafeSetCellTextT3(secondTable, 2, 2, "L_B");
                SafeSetCellTextT3(secondTable, 2, 3, "W/m².sr");
                SafeSetCellTextT3(secondTable, 2, 4, string.IsNullOrEmpty(blueLightResult) ? "LB (11 mrad)\r\npada hasil uji lab IEC 62788" : blueLightResult);

                // Luminance row (now row 3)  
                SafeSetCellTextT3(secondTable, 3, 1, "Luminance");
                SafeSetCellTextT3(secondTable, 3, 2, "L_v");
                SafeSetCellTextT3(secondTable, 3, 3, "cd/m²");
                SafeSetCellTextT3(secondTable, 3, 4, string.IsNullOrEmpty(luminanceResult) ? "L (11 mrad)\r\npada hasil uji lab IEC 62788" : luminanceResult);

                // Apply formatting to second table
                for (int row = 1; row <= 4; row++)
                {
                    for (int col = 1; col <= 4; col++)
                    {
                        if (CellExistsT3(secondTable, row, col))
                        {
                            Cell cell = secondTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                            // Remove spacing
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;

                            // Set white background for data rows (rows 2-4)
                            if (row >= 2)
                            {
                                SafeSetCellShadingT3(secondTable, row, col, WdColor.wdColorWhite);
                            }
                        }
                    }
                }

                // Center align symbol and unit columns
                for (int row = 2; row <= 3; row++)
                {
                    for (int col = 2; col <= 3; col++)
                    {
                        if (CellExistsT3(secondTable, row, col))
                        {
                            secondTable.Cell(row, col).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                    }
                }

                // Remove spacing from second table
                secondTable.Range.ParagraphFormat.SpaceAfter = 0;
                secondTable.Range.ParagraphFormat.SpaceBefore = 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating photobiological hazards table: {ex.Message}", ex);
            }
        }

        private void SafeSetCellShadingT3(Table table, int row, int col, WdColor color)
        {
            if (CellExistsT3(table, row, col))
            {
                table.Cell(row, col).Shading.BackgroundPatternColor = color;
            }
        }

        private bool CellExistsT3(Table table, int row, int column)
        {
            try
            {
                var cell = table.Cell(row, column);
                return cell != null;
            }
            catch
            {
                return false;
            }
        }

        private void SafeMergeCellsT3(Table table, int startRow, int startCol, int endRow, int endCol)
        {
            if (CellExistsT3(table, startRow, startCol) && CellExistsT3(table, endRow, endCol))
            {
                table.Cell(startRow, startCol).Merge(table.Cell(endRow, endCol));
            }
        }

        private void SafeSetCellTextT3(Table table, int row, int col, string text, bool bold = false, WdParagraphAlignment alignment = WdParagraphAlignment.wdAlignParagraphLeft)
        {
            if (CellExistsT3(table, row, col))
            {
                Cell cell = table.Cell(row, col);
                cell.Range.Text = text;
                cell.Range.Font.Bold = bold ? 1 : 0;
                cell.Range.ParagraphFormat.Alignment = alignment;
            }
        }

        private void SafeSetCellItalicT3(Table table, int row, int col, bool italic)
        {
            if (CellExistsT3(table, row, col))
            {
                table.Cell(row, col).Range.Font.Italic = italic ? 1 : 0;
            }
        }

        private string GetMeasurementResultT3(int reportParamMapID, Dictionary<int, string> measurementData)
        {
            if (measurementData.ContainsKey(reportParamMapID))
            {
                string result = measurementData[reportParamMapID];
                System.Diagnostics.Debug.WriteLine($"Retrieving data for {reportParamMapID}: {result}");
                return result;
            }
            System.Diagnostics.Debug.WriteLine($"No data found for {reportParamMapID}");
            return string.Empty;
        }

        /*T3 end */

        /* T4 Start*/
        private void CreateReportTableT4(Application wordApp, Document doc, string placeholder, DataTable dt)
        {
            try
            {
                Range range = doc.Content;
                Find find = range.Find;
                find.ClearFormatting();
                find.Text = placeholder;

                if (!find.Execute())
                    throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

                // COMPLETELY REMOVE SPACING - Use exact line spacing
                range.Text = "LAMPIRAN D – UJI TERMAL" + (char)11 + "APPENDIX D – THERMAL TESTING";
                range.Font.Size = 12;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                // Remove all spacing
                range.ParagraphFormat.SpaceBefore = 0;
                range.ParagraphFormat.SpaceAfter = 0;
                range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                range.ParagraphFormat.LineSpacing = 14;

                // Create table immediately after placeholder
                Range tableRange = doc.Range(range.End, range.End);
                tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                // Parse data from database based on ParameterID
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

                // Create first table (Klausul table) - 11 rows, 4 columns
                Table firstTable = doc.Tables.Add(tableRange, 11, 4);
                firstTable.Borders.Enable = 1;
                firstTable.AllowAutoFit = false;

                float firstTableWidthInches = 6.5f;
                firstTable.PreferredWidth = wordApp.InchesToPoints(firstTableWidthInches);

                // Column widths for first table
                float[] firstColWidths = { 0.6f, 3.5f, 1.2f, 1.2f };
                for (int i = 0; i < 4; i++)
                {
                    firstTable.Columns[i + 1].Width = wordApp.InchesToPoints(firstColWidths[i]);
                }

                // First table header - multi-line
                string[] firstHeaders = {
                    "Klausul\r\nclause",
                    "Persyaratan\r\nRequirement",
                    "Hasil\r\nResult",
                    "*Keterangan\r\nRemark*"
                };

                for (int col = 1; col <= 4; col++)
                {
                    SafeSetCellTextT4(firstTable, 1, col, firstHeaders[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    SafeSetCellShadingT4(firstTable, 1, col, WdColor.wdColorGray25);
                }

                // Set row height for header
                firstTable.Rows[1].Height = wordApp.InchesToPoints(0.4f);
                firstTable.Rows[1].HeightRule = WdRowHeightRule.wdRowHeightExactly;

                // First table data
                string[,] firstTableData = {
                    { "1.", "Tipe produk.", GetMeasurementValue(17830, 0, measurementData), GetMeasurementValue(17830, 1, measurementData) },
                    { "2.", "Lampu yang digunakan.", GetMeasurementValue(17831, 0, measurementData), GetMeasurementValue(17831, 1, measurementData) },
                    { "3.", "Tipe LED driver yang digunakan.", GetMeasurementValue(17832, 0, measurementData), GetMeasurementValue(17832, 1, measurementData) },
                    { "4.", "Posisi pemasangan saat pengujian.", GetMeasurementValue(17833, 0, measurementData), GetMeasurementValue(17833, 1, measurementData) },
                    { "5.", "Daya (W).", GetMeasurementValue(17834, 0, measurementData), GetMeasurementValue(17834, 1, measurementData) },
                    { "6.", "Arus (A).", GetMeasurementValue(17835, 0, measurementData), GetMeasurementValue(17835, 1, measurementData) },
                    { "7.", "Uji 1:\r\nTegangan pengenal (V).", GetMeasurementValue(17836, 0, measurementData), GetMeasurementValue(17836, 1, measurementData) },
                    { "8.", "Uji 2:\r\n1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17837, 0, measurementData), GetMeasurementValue(17837, 1, measurementData) },
                    { "9.", "Uji 3:\r\nBakan dengan dilengkapi kotak kontak 1,06 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17838, 0, measurementData), GetMeasurementValue(17838, 1, measurementData) },
                    { "10.", "Uji 4:\r\n1,1 x tegangan pengenal atau 1,05 x daya.", GetMeasurementValue(17839, 0, measurementData), GetMeasurementValue(17839, 1, measurementData) }
                };

                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        SafeSetCellTextT4(firstTable, row + 2, col + 1, firstTableData[row, col], false,
                            col == 0 || col == 2 ? WdParagraphAlignment.wdAlignParagraphCenter : WdParagraphAlignment.wdAlignParagraphLeft);
                        SafeSetCellShadingT4(firstTable, row + 2, col + 1, WdColor.wdColorWhite);
                    }
                }

                // Apply formatting to first table
                for (int row = 1; row <= 11; row++)
                {
                    for (int col = 1; col <= 4; col++)
                    {
                        if (CellExistsT4(firstTable, row, col))
                        {
                            Cell cell = firstTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;
                        }
                    }
                }

                firstTable.Range.ParagraphFormat.SpaceAfter = 0;
                firstTable.Range.ParagraphFormat.SpaceBefore = 0;

                // Add space between tables
                Range betweenTablesRange = doc.Range(firstTable.Range.End, firstTable.Range.End);
                betweenTablesRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                betweenTablesRange.InsertParagraphAfter();
                betweenTablesRange.InsertParagraphAfter();

                // Create second table - 9 rows (2 header rows + 7 data rows), 7 columns
                Range secondTableRange = doc.Range(betweenTablesRange.End, betweenTablesRange.End);
                secondTableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

                Table secondTable = doc.Tables.Add(secondTableRange, 9, 7);
                secondTable.Borders.Enable = 1;
                secondTable.AllowAutoFit = false;

                float secondTableWidthInches = 7.0f;
                secondTable.PreferredWidth = wordApp.InchesToPoints(secondTableWidthInches);

                // Column widths for second table
                float[] secondColWidths = { 1.5f, 0.8f, 0.8f, 0.8f, 1.0f, 0.8f, 1.0f };
                for (int i = 0; i < 7; i++)
                {
                    secondTable.Columns[i + 1].Width = wordApp.InchesToPoints(secondColWidths[i]);
                }

                // HEADER ROW 1 - Set all text and formatting BEFORE merging
                // Cell[1,1] - "Bagian yang diuji" (will span rows 1-2)
                SafeSetCellTextT4(secondTable, 1, 1, "Bagian yang diuji", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 1, WdColor.wdColorGray25);

                // Cell[1,2] - "Klausul 12.4..." (will span columns 2-5)
                SafeSetCellTextT4(secondTable, 1, 2, "Klausul 12.4 – Uji termal\r\n(Operasi normal)", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 2, WdColor.wdColorGray25);

                // Cells [1,3], [1,4], [1,5] - Empty (will be merged into cell [1,2])
                SafeSetCellTextT4(secondTable, 1, 3, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 3, WdColor.wdColorGray25);
                SafeSetCellTextT4(secondTable, 1, 4, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 4, WdColor.wdColorGray25);
                SafeSetCellTextT4(secondTable, 1, 5, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 5, WdColor.wdColorGray25);

                // Cell[1,6] - "Klausul 12.5..." (will span columns 6-7)
                SafeSetCellTextT4(secondTable, 1, 6, "Klausul 12.5 – Uji termal\r\n(Operasi abnormal)", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 6, WdColor.wdColorGray25);

                // Cell[1,7] - Empty (will be merged into cell [1,6])
                SafeSetCellTextT4(secondTable, 1, 7, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 1, 7, WdColor.wdColorGray25);

                // HEADER ROW 2
                // Cell[2,1] - Empty (will be merged with [1,1])
                SafeSetCellTextT4(secondTable, 2, 1, "", true, WdParagraphAlignment.wdAlignParagraphCenter);
                SafeSetCellShadingT4(secondTable, 2, 1, WdColor.wdColorGray25);

                // Cells[2,2] through [2,7] - Column headers
                string[] row2Headers = { "", "Uji 1", "Uji 2", "Uji 3", "Limit Maks.", "Uji 4", "Limit Maks." };
                for (int col = 2; col <= 7; col++)
                {
                    SafeSetCellTextT4(secondTable, 2, col, row2Headers[col - 1], true, WdParagraphAlignment.wdAlignParagraphCenter);
                    SafeSetCellShadingT4(secondTable, 2, col, WdColor.wdColorGray25);
                }

                // NOW PERFORM THE MERGES
                // Merge "Bagian yang diuji" - rowspan=2
                SafeMergeCellsT4(secondTable, 1, 1, 2, 1);

                // Merge "Klausul 12.4..." - colspan=4 (columns 2-5)
                SafeMergeCellsT4(secondTable, 1, 2, 1, 5);

                // Merge "Klausul 12.5..." - colspan=2 (columns 6-7)
                SafeMergeCellsT4(secondTable, 1, 6, 1, 7);

                // Data rows (rows 3-9)
                string[,] secondTableData = {
                    { "Suhu ruang uji.",
                      GetMeasurementValue(17840, 0, measurementData), GetMeasurementValue(17840, 1, measurementData), GetMeasurementValue(17840, 2, measurementData),
                      GetMeasurementValue(17840, 3, measurementData), GetMeasurementValue(17840, 4, measurementData), GetMeasurementValue(17840, 5, measurementData) },
                    { "Permukaan luar luminer",
                      GetMeasurementValue(17841, 0, measurementData), GetMeasurementValue(17841, 1, measurementData), GetMeasurementValue(17841, 2, measurementData),
                      GetMeasurementValue(17841, 3, measurementData), GetMeasurementValue(17841, 4, measurementData), GetMeasurementValue(17841, 5, measurementData) },
                    { "Permukaan LED Driver.",
                      GetMeasurementValue(17842, 0, measurementData), GetMeasurementValue(17842, 1, measurementData), GetMeasurementValue(17842, 2, measurementData),
                      GetMeasurementValue(17842, 3, measurementData), GetMeasurementValue(17842, 4, measurementData), GetMeasurementValue(17842, 5, measurementData) },
                    { "LED module / PCB.",
                      GetMeasurementValue(17843, 0, measurementData), GetMeasurementValue(17843, 1, measurementData), GetMeasurementValue(17843, 2, measurementData),
                      GetMeasurementValue(17843, 3, measurementData), GetMeasurementValue(17843, 4, measurementData), GetMeasurementValue(17843, 5, measurementData) },
                    { "Selungkup LED module\r\n(Plastik)",
                      GetMeasurementValue(17844, 0, measurementData), GetMeasurementValue(17844, 1, measurementData), GetMeasurementValue(17844, 2, measurementData),
                      GetMeasurementValue(17844, 3, measurementData), GetMeasurementValue(17844, 4, measurementData), GetMeasurementValue(17844, 5, measurementData) },
                    { "Kabel eksternal.",
                      GetMeasurementValue(17845, 0, measurementData), GetMeasurementValue(17845, 1, measurementData), GetMeasurementValue(17845, 2, measurementData),
                      GetMeasurementValue(17845, 3, measurementData), GetMeasurementValue(17845, 4, measurementData), GetMeasurementValue(17845, 5, measurementData) },
                    { "Kabel internal.",
                      GetMeasurementValue(17846, 0, measurementData), GetMeasurementValue(17846, 1, measurementData), GetMeasurementValue(17846, 2, measurementData),
                      GetMeasurementValue(17846, 3, measurementData), GetMeasurementValue(17846, 4, measurementData), GetMeasurementValue(17846, 5, measurementData) }
                };

                for (int row = 0; row < 7; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        SafeSetCellTextT4(secondTable, row + 3, col + 1, secondTableData[row, col], false,
                            col == 0 ? WdParagraphAlignment.wdAlignParagraphLeft : WdParagraphAlignment.wdAlignParagraphCenter);
                        SafeSetCellShadingT4(secondTable, row + 3, col + 1, WdColor.wdColorWhite);
                    }
                }

                // Apply formatting to entire second table
                for (int row = 1; row <= 9; row++)
                {
                    for (int col = 1; col <= 7; col++)
                    {
                        if (CellExistsT4(secondTable, row, col))
                        {
                            Cell cell = secondTable.Cell(row, col);
                            cell.Range.Font.Size = 10;
                            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            cell.Range.ParagraphFormat.SpaceBefore = 0;
                            cell.Range.ParagraphFormat.SpaceAfter = 0;
                            cell.Range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                            cell.Range.ParagraphFormat.LineSpacing = 12;
                        }
                    }
                }

                secondTable.Range.ParagraphFormat.SpaceAfter = 0;
                secondTable.Range.ParagraphFormat.SpaceBefore = 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating table T4: {ex.Message}", ex);
            }
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

        private bool CellExistsT4(Table table, int row, int column)
        {
            try
            {
                var cell = table.Cell(row, column);
                return cell != null;
            }
            catch
            {
                return false;
            }
        }

        private void SafeMergeCellsT4(Table table, int startRow, int startCol, int endRow, int endCol)
        {
            if (CellExistsT4(table, startRow, startCol) && CellExistsT4(table, endRow, endCol))
            {
                table.Cell(startRow, startCol).Merge(table.Cell(endRow, endCol));
            }
        }

        private void SafeSetCellTextT4(Table table, int row, int col, string text, bool bold = false, WdParagraphAlignment alignment = WdParagraphAlignment.wdAlignParagraphLeft)
        {
            if (CellExistsT4(table, row, col))
            {
                Cell cell = table.Cell(row, col);
                cell.Range.Text = text;
                cell.Range.Font.Bold = bold ? 1 : 0;
                cell.Range.ParagraphFormat.Alignment = alignment;
            }
        }

        private void SafeSetCellShadingT4(Table table, int row, int col, WdColor color)
        {
            if (CellExistsT4(table, row, col))
            {
                table.Cell(row, col).Shading.BackgroundPatternColor = color;
            }
        }

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
                        resultCell.Range.Font.Size = 11; // Larger font for checkbox rows (was 10)
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






        /* New Code for Table 2 */
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

            // Clear TextBoxes after adding
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
            // Create the same structure as CreateGridTable
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("ObjectName");
            gridDt.Columns.Add("Manufacturer");
            gridDt.Columns.Add("TypeOrModel");
            gridDt.Columns.Add("TechnicalData");
            gridDt.Columns.Add("Standard");
            gridDt.Columns.Add("SignOfConformity");

            // Get CSV strings from the saved data (adjust IDs as needed)
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

            // Save to ViewState so Add/Delete works later
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



    }
}