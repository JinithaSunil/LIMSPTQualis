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
    public partial class HomeAppliances : System.Web.UI.Page
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
                CreateGridTable2();
                //CreateHeatingTestGrid();

                DataTable existingData = _cm.GetExistingTestResults_Home_RP(lblSampleID.Text, lblReportID.Text);
                
                if (existingData != null && existingData.Rows.Count > 0)
                {
                    PrefillTestResultsGrid(existingData);
                    PrefillTestResultsGrid2(existingData);
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
        private void LoadClauseTable()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);

            DataTable dtcl = _cm.GetCRFCloneList_HA(sampleId);
            int cloneCRFSampleID = Convert.ToInt32(dtcl.Rows[0]["CloneCRFSampleID"].ToString());
            ViewState["CloneCRFSampleID"] = dtcl.Rows[0]["CloneCRFSampleID"].ToString();

            DataTable dts = _cm.GetCRFSampleDetails_HA(sampleId, reportId, cloneCRFSampleID);
            if (dts != null && dts.Rows.Count > 0)
            {
                var standardIds = dts.AsEnumerable()
                                    .Where(row => row["TestStandardID"] != null && !string.IsNullOrEmpty(row["TestStandardID"].ToString()))
                                    .Select(row => row["TestStandardID"].ToString().Trim()).Distinct().ToList();

                var standardNames = dts.AsEnumerable()
                                      .Where(row => row["TestStandard"] != null && !string.IsNullOrEmpty(row["TestStandard"].ToString()))
                                      .Select(row => row["TestStandard"].ToString().Trim()).Distinct().ToList();

                lblstandardID.Text = string.Join(", ", standardIds);
                //  lblstandardID.Text = "2059";

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

                if (!string.IsNullOrEmpty(saved.Rows[0]["TestCompletationDate"].ToString()))
                {
                    DateTime completionDate;
                    if (DateTime.TryParse(saved.Rows[0]["TestCompletationDate"].ToString(), out completionDate))
                    {
                        txtcompletiondate.Text = completionDate.ToString("yyyy-MM-dd");
                    }
                }
            }


            DataTable DisResget = _cm.GetTestResultHA_DISTINCT(lblSampleID.Text, lblReportID.Text);
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

            DataTable dt1 = _cm.GetCustomerEnquiryWithSampleDetails_HA(sampleId);
            if (dt1 != null && dt1.Rows.Count > 0)
            {
                int departmentID = Convert.ToInt32(dt1.Rows[0]["DepartmentID"]);
                int productCategoryID = Convert.ToInt32(dt1.Rows[0]["ProductCategoryID"]);
                string StandardID = dt1.Rows[0]["StandardID"].ToString();
                //string StandardID = "2059";
                int reportID = Convert.ToInt32(Request.QueryString["r"]);

                string TestStandardGet = dts.Rows[0]["TestStandard"].ToString();
                txtproduct.Text = dt1.Rows[0]["ProductName"].ToString();
                //txtproduct.Text = "washing machine";
                txtbrand.Text = dt1.Rows[0]["BrandProduct"].ToString();

                //txtbrand.Text = "hitachi";
                txtmodel.Text = dt1.Rows[0]["ProductSerialNo"].ToString();
                //txtmodel.Text = "X-1245";
                //txtworkorder.Text = "WO/202508014";
                txttestMethodNo.Text = dts.Rows[0]["TestMethod"].ToString();
                txtworkorder.Text = dts.Rows[0]["WorkorderNumber"].ToString();
                //txttestMethodNo.Text = "SNI-0125";
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
                /*if (standardIdList.Count > 0)
                {
                    var matchingRows = dt.AsEnumerable().Where(r => standardIdList.Contains(r["Standard_ID"].ToString()));
                    if (matchingRows.Any())
                        filteredClauseTable = matchingRows.CopyToDataTable();
                }*/


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
                clauseContainer.InnerHtml = sb.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
            }
        }
        private void BindEquipmentCheckboxes()
        {
            string query = "SELECT * FROM Calibration_Equipment WHERE DepartmentId = 4009";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    chkEquipmentList.DataSource = reader;
                    chkEquipmentList.DataTextField = "Equipment"; // Display text
                    chkEquipmentList.DataValueField = "EpID";     // Value field
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
                    /*
                    string inClause = string.Join(",", standardIds.Select(id => $"'{id}'"));
                    using (SqlCommand cmd = new SqlCommand($"SELECT * FROM Clauses WHERE Standard_ID IN ({inClause}) ORDER BY ClauseID", conn))*/
                    /* Build dynamic WHERE clause using LIKE for each ID*/
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
                        string uploadedImages = ViewState["UploadedImages"]?.ToString() ?? "";

                        if (isHeader)
                        {
                            int result0 = _cm.TestResultsAdd_HA(clauseID, txtstartingdate.Text, txtcompletiondate.Text, "", remark, sampleId, sampleIdSeries,
                                 "header", language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                 txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text,
                                 txttemp.Text, txthumidity.Text);
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

                            int result0 = _cm.TestResultsAdd_HA(clauseID, txtstartingdate.Text, txtcompletiondate.Text, result, remark, sampleId, sampleIdSeries,
                                inputType, language, ReportID, reportParamMapID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text,
                                txttemp.Text, txthumidity.Text);

                            if (result0 > 0)
                            {
                                b1 = true;
                            }

                        }

                    }
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



                        //// Add textbox values in the SAME lists
                        //reportParamMapList2.Add("17861"); resultList2.Add(txtdetailsoftestitems.Text);
                        //reportParamMapList2.Add("17862"); resultList2.Add(txtclassification.Text);
                        //reportParamMapList2.Add("17863"); resultList2.Add(txtsupplyconnection.Text);
                        //reportParamMapList2.Add("17864"); resultList2.Add(txtNotApplicable.Text);
                        //reportParamMapList2.Add("17865"); resultList2.Add(txtPass.Text);
                        //reportParamMapList2.Add("17866"); resultList2.Add(txtFail.Text);



                        string[] reportParamMap2 = reportParamMapList2.ToArray();
                        string[] resultNew2 = resultList2.ToArray();

                        int modified2 = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "", sampleId, sampleIdSeries,
                                    "", language, ReportID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                    txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text,
                                    txthumidity.Text, reportParamMap2, resultNew2);

                        if (modified2 > 0) b1 = true;
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

                        List<string> inputDeviations = new List<string>();
                        List<string> pIdentifiers = new List<string>();
                        List<string> pResults = new List<string>();
                        List<string> deltaPs = new List<string>();
                        List<string> conditionDeltaPs = new List<string>();
                        List<string> decisions = new List<string>();
                        List<string> additionalInfos = new List<string>();

                        // Collect data from all rows in the DataTable
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

                        // Convert lists to comma-separated strings
                        string inputDeviationsCsv = string.Join(",", inputDeviations);
                        string pIdentifiersCsv = string.Join(",", pIdentifiers);
                        string pResultsCsv = string.Join(",", pResults);
                        string deltaPsCsv = string.Join(",", deltaPs);
                        string conditionDeltaPsCsv = string.Join(",", conditionDeltaPs);
                        string decisionsCsv = string.Join(",", decisions);
                        string additionalInfosCsv = string.Join(",", additionalInfos);

                        // Prepare data for saving
                        List<string> reportParamMapList = new List<string>();
                        List<string> resultList = new List<string>();

                        // Add the data with corresponding parameter IDs (17847-17853)
                        reportParamMapList.Add("17847"); resultList.Add(inputDeviationsCsv);
                        reportParamMapList.Add("17848"); resultList.Add(pIdentifiersCsv);
                        reportParamMapList.Add("17849"); resultList.Add(pResultsCsv);
                        reportParamMapList.Add("17850"); resultList.Add(deltaPsCsv);
                        reportParamMapList.Add("17851"); resultList.Add(conditionDeltaPsCsv);
                        reportParamMapList.Add("17852"); resultList.Add(decisionsCsv);
                        reportParamMapList.Add("17853"); resultList.Add(additionalInfosCsv);


                        // Convert to arrays
                        string[] reportParamMap = reportParamMapList.ToArray();
                        string[] resultNew = resultList.ToArray();

                        //// Save to database
                        int modified = _cm.TestResultsAdd_HA_RP(0, txtstartingdate.Text, txtcompletiondate.Text, "", sampleId, sampleIdSeries,
                                    "", language, ReportID, equipmentIdsCSV, txtworkorder.Text, txtproduct.Text, txtbrand.Text,
                                    txtmodel.Text, lblstandardID.Text, txtstandard.Text, txttestMethodNo.Text, txttemp.Text,
                                    txthumidity.Text, reportParamMap, resultNew);

                        if (modified > 0)
                        {
                            //Response.Write("<script>alert('Data saved successfully!'); window.location.href = window.location.href;</script>");
                            int sampleStatusID = 27;
                            int res1 = _cm.UpdateCRFSampleDetailsStatus_HA(sampleStatusID, cloneCFRsampleID, lblReportID.Text);
                            if (res1 > 0)
                            {
                                int res2 = _cm.UpdateCRFCloneStatus_HA(sampleStatusID, cloneCFRsampleID);

                                if (res2 > 0)
                                {
                                    int res3 = _cm.updateSampleDetailStatus_HA(sampleStatusID, lblSampleID.Text);

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
                                    CreateBasicTemplate(doc);
                                }
                                DataTable dtReport = GetReportData();
                                CreateReportTable(wordApp, doc, "{TestingAndMeasurement}", dtReport);

                                DataTable dtReportT2 = GetReportDataForT2();
                                CreateReportTableT2(wordApp, doc, "{reporttable}", dtReportT2);

                                // Create Table 2: Heating Test (same structure, different IDs)
                                DataTable dtReportT3 = GetReportDataForT3();
                                CreateReportTableT3(wordApp, doc, "{HeatingTestTable}", dtReportT3);

                                //// Add image insertion
                                //DataTable dtImages = GetImageData();
                                //if (dtImages.Rows.Count > 0)
                                //{
                                //    string imageNames = dtImages.Rows[0]["ImageName"].ToString();
                                //    InsertLabAnalysisImages(doc, "{imageuploadbylabanalyst}", imageNames);
                                //}

                                //DataTable dtReportT3 = GetReportDataForT2();
                                //CreateReportTableT3(wordApp, doc, "{LAMPIRAN_B-BAHAYA_FOTOBIOLOGIS}", dtReportT3);

                                //DataTable dtReportT4 = GetReportDataForT2();
                                //CreateReportTableT4(wordApp, doc, "{LAMPIRAN_D-UJI_TERMAL}", dtReportT4);

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

                                ///*Force - kill any lingering WINWORD processes started by this user session for locally server */
                                //foreach (var process in Process.GetProcessesByName("WINWORD"))
                                //{
                                //    if (string.IsNullOrEmpty(process.MainWindowTitle))
                                //    {
                                //        process.Kill();
                                //    }
                                //}
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
        // Replace this method in  page:
        private DataTable GetImageData()
        {
            int sampleId = Convert.ToInt32(lblSampleID.Text);
            int reportId = Convert.ToInt32(lblReportID.Text);
            return _cm.GetLabAnalysisImages(sampleId, reportId);
        }
        //private void InsertLabAnalysisImages(Document doc, string placeholder, string imageNames)
        //{
        //    if (string.IsNullOrEmpty(imageNames)) return;

        //    Range range = doc.Content;
        //    Find find = range.Find;
        //    find.ClearFormatting();
        //    find.Text = placeholder;

        //    if (!find.Execute()) return;

        //    range.Text = "";

        //    string imagePath = Server.MapPath("~/SampleImages/");
        //    string[] images = imageNames.Split(',');

        //    foreach (string imageName in images)
        //    {
        //        string fullPath = Path.Combine(imagePath, imageName.Trim());
        //        if (File.Exists(fullPath))
        //        {
        //            var inlineShape = range.InlineShapes.AddPicture(fullPath);
        //            inlineShape.Width = 400;
        //            inlineShape.Height = 300;
        //            range.InsertParagraphAfter();
        //            range.Collapse(WdCollapseDirection.wdCollapseEnd);
        //        }
        //    }
        //}      
        //protected void btnUploadImages_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (fuLabAnalysis.HasFiles)
        //        {
        //            string sampleId = lblSampleID.Text;
        //            string uploadPath = Server.MapPath("~/SampleImages/");

        //            if (!Directory.Exists(uploadPath))
        //                Directory.CreateDirectory(uploadPath);

        //            int sequence = 1;
        //            List<string> uploadedImages = new List<string>();

        //            foreach (HttpPostedFile file in fuLabAnalysis.PostedFiles)
        //            {
        //                string extension = Path.GetExtension(file.FileName).ToLower();
        //                string fileName = $"labanalysis_{sampleId}_{sequence}{extension}";
        //                string filePath = Path.Combine(uploadPath, fileName);

        //                file.SaveAs(filePath);
        //                uploadedImages.Add(fileName);
        //                sequence++;
        //            }

        //            ViewState["UploadedImages"] = string.Join(",", uploadedImages);
        //            ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Images uploaded successfully!');", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
        //    }
        //}
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
                    //doc = wordApp.Documents.Open(templatePath);
                    //ReplaceTextInDocument(doc, "{txtdetailsoftestitems}", txtdetailsoftestitems.Text);
                    //ReplaceTextInDocument(doc, "{txtclassification}", txtclassification.Text);
                    //ReplaceTextInDocument(doc, "{txtsupplyconnection}", txtsupplyconnection.Text);
                    //ReplaceTextInDocument(doc, "{txtNotApplicable}", txtNotApplicable.Text);
                    //ReplaceTextInDocument(doc, "{txtPass}", txtPass.Text);
                    //ReplaceTextInDocument(doc, "{txtFail}", txtFail.Text);
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
                CreateReportTableT2(wordApp, doc, "{reporttable}", dtReportT2);

                DataTable dtReportT3 = GetReportDataForT3();
                CreateReportTableT3(wordApp, doc, "{HeatingTestTable}", dtReportT3);

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
                //foreach (var process in Process.GetProcessesByName("WINWORD"))
                //{
                //    if (string.IsNullOrEmpty(process.MainWindowTitle))
                //    {
                //        process.Kill();
                //    }
                //}
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
            range.Text = "TABLE: Power Input Deviation";
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
            { 17847, 0 }, // Input deviation from/on
            { 17848, 1 }, // P Identifier
            { 17849, 2 }, // P Result
            { 17850, 3 }, // ∆P
            { 17851, 4 }, // Condition∆P
            { 17852, 5 }, // Decision
            { 17853, 6 }  // Additional information
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
            dt.Columns.Add("InputDeviation");      // 17847
            dt.Columns.Add("PIdentifier");         // 17848
            dt.Columns.Add("PResult");             // 17849
            dt.Columns.Add("DeltaP");              // 17850
            dt.Columns.Add("ConditionDeltaP");     // 17851
            dt.Columns.Add("Decision");            // 17852
            dt.Columns.Add("AdditionalInfo");      // 17853

            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();
        }
    
        // 2. ADD BUTTON CLICK EVENT
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;
            DataRow dr = dt.NewRow();

            dr["InputDeviation"] = txtInputDeviation.Text.Trim();
            dr["PIdentifier"] = txtPIdentifier.Text.Trim();
            dr["PResult"] = txtPResult.Text.Trim();
            dr["DeltaP"] = txtDeltaP.Text.Trim();
            dr["ConditionDeltaP"] = txtConditionDeltaP.Text.Trim();
            dr["Decision"] = txtDecision.Text.Trim();
            dr["AdditionalInfo"] = txtAdditionalInfo.Text.Trim();

            dt.Rows.Add(dr);
            ViewState["TestData"] = dt;
            gvTests.DataSource = dt;
            gvTests.DataBind();

            // Clear TextBoxes after adding
            txtInputDeviation.Text = "";
            txtPIdentifier.Text = "";
            txtPResult.Text = "";
            txtDeltaP.Text = "";
            txtConditionDeltaP.Text = "";
            txtDecision.Text = "";
            txtAdditionalInfo.Text = "";
        }
        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData"] as DataTable;
            if (dt != null && dt.Rows.Count > e.RowIndex)
            {
                dt.Rows.RemoveAt(e.RowIndex);
                ViewState["TestData"] = dt;
                gvTests.DataSource = dt;
                gvTests.DataBind();
            }
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
            gridDt.Columns.Add("InputDeviation");
            gridDt.Columns.Add("PIdentifier");
            gridDt.Columns.Add("PResult");
            gridDt.Columns.Add("DeltaP");
            gridDt.Columns.Add("ConditionDeltaP");
            gridDt.Columns.Add("Decision");
            gridDt.Columns.Add("AdditionalInfo");

            // Get CSV strings from the saved data (using new IDs 17847-17853)
            string inputDeviationsCsv = GetResultValue(dt, "17847");
            string pIdentifiersCsv = GetResultValue(dt, "17848");
            string pResultsCsv = GetResultValue(dt, "17849");
            string deltaPsCsv = GetResultValue(dt, "17850");
            string conditionDeltaPsCsv = GetResultValue(dt, "17851");
            string decisionsCsv = GetResultValue(dt, "17852");
            string additionalInfosCsv = GetResultValue(dt, "17853");

            if (!string.IsNullOrEmpty(inputDeviationsCsv))
            {
                string[] inputDeviations = inputDeviationsCsv.Split(',');
                string[] pIdentifiers = pIdentifiersCsv?.Split(',') ?? new string[0];
                string[] pResults = pResultsCsv?.Split(',') ?? new string[0];
                string[] deltaPs = deltaPsCsv?.Split(',') ?? new string[0];
                string[] conditionDeltaPs = conditionDeltaPsCsv?.Split(',') ?? new string[0];
                string[] decisions = decisionsCsv?.Split(',') ?? new string[0];
                string[] additionalInfos = additionalInfosCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < inputDeviations.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["InputDeviation"] = inputDeviations[i].Trim();
                    dr["PIdentifier"] = (i < pIdentifiers.Length) ? pIdentifiers[i].Trim() : "";
                    dr["PResult"] = (i < pResults.Length) ? pResults[i].Trim() : "";
                    dr["DeltaP"] = (i < deltaPs.Length) ? deltaPs[i].Trim() : "";
                    dr["ConditionDeltaP"] = (i < conditionDeltaPs.Length) ? conditionDeltaPs[i].Trim() : "";
                    dr["Decision"] = (i < decisions.Length) ? decisions[i].Trim() : "";
                    dr["AdditionalInfo"] = (i < additionalInfos.Length) ? additionalInfos[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            // Save to ViewState so Add/Delete works later
            ViewState["TestData"] = gridDt;
            gvTests.DataSource = gridDt;
            gvTests.DataBind();
        }
        private void CreateGridTable2()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("InputDeviation");      // 17854
            dt.Columns.Add("PIdentifier");         // 17855
            dt.Columns.Add("PResult");             // 17856
            dt.Columns.Add("DeltaP");              // 17857
            dt.Columns.Add("ConditionDeltaP");     // 17858
            dt.Columns.Add("Decision");            // 17859
            dt.Columns.Add("AdditionalInfo");      // 17860

            ViewState["TestData2"] = dt;
            gvTests2.DataSource = dt;
            gvTests2.DataBind();
        }
        protected void btnAdd2_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestData2"] as DataTable;
            DataRow dr = dt.NewRow();

            dr["InputDeviation"] = txtInputDeviation2.Text.Trim();
            dr["PIdentifier"] = txtPIdentifier2.Text.Trim();
            dr["PResult"] = txtPResult2.Text.Trim();
            dr["DeltaP"] = txtDeltaP2.Text.Trim();
            dr["ConditionDeltaP"] = txtConditionDeltaP2.Text.Trim();
            dr["Decision"] = txtDecision2.Text.Trim();
            dr["AdditionalInfo"] = txtAdditionalInfo2.Text.Trim();

            dt.Rows.Add(dr);
            ViewState["TestData2"] = dt;
            gvTests2.DataSource = dt;
            gvTests2.DataBind();

            // Clear TextBoxes after adding
            txtInputDeviation2.Text = "";
            txtPIdentifier2.Text = "";
            txtPResult2.Text = "";
            txtDeltaP2.Text = "";
            txtConditionDeltaP2.Text = "";
            txtDecision2.Text = "";
            txtAdditionalInfo2.Text = "";
        }
        protected void gvTests2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = ViewState["TestData2"] as DataTable;
            if (dt != null && dt.Rows.Count > e.RowIndex)
            {
                dt.Rows.RemoveAt(e.RowIndex);
                ViewState["TestData2"] = dt;
                gvTests2.DataSource = dt;
                gvTests2.DataBind();
            }
        }
        private void PrefillTestResultsGrid2(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("InputDeviation");
            gridDt.Columns.Add("PIdentifier");
            gridDt.Columns.Add("PResult");
            gridDt.Columns.Add("DeltaP");
            gridDt.Columns.Add("ConditionDeltaP");
            gridDt.Columns.Add("Decision");
            gridDt.Columns.Add("AdditionalInfo");

            string inputDeviationsCsv = GetResultValue(dt, "17854");
            string pIdentifiersCsv = GetResultValue(dt, "17855");
            string pResultsCsv = GetResultValue(dt, "17856");
            string deltaPsCsv = GetResultValue(dt, "17857");
            string conditionDeltaPsCsv = GetResultValue(dt, "17858");
            string decisionsCsv = GetResultValue(dt, "17859");
            string additionalInfosCsv = GetResultValue(dt, "17860");

            if (!string.IsNullOrEmpty(inputDeviationsCsv))
            {
                string[] inputDeviations = inputDeviationsCsv.Split(',');
                string[] pIdentifiers = pIdentifiersCsv?.Split(',') ?? new string[0];
                string[] pResults = pResultsCsv?.Split(',') ?? new string[0];
                string[] deltaPs = deltaPsCsv?.Split(',') ?? new string[0];
                string[] conditionDeltaPs = conditionDeltaPsCsv?.Split(',') ?? new string[0];
                string[] decisions = decisionsCsv?.Split(',') ?? new string[0];
                string[] additionalInfos = additionalInfosCsv?.Split(',') ?? new string[0];

                for (int i = 0; i < inputDeviations.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["InputDeviation"] = inputDeviations[i].Trim();
                    dr["PIdentifier"] = (i < pIdentifiers.Length) ? pIdentifiers[i].Trim() : "";
                    dr["PResult"] = (i < pResults.Length) ? pResults[i].Trim() : "";
                    dr["DeltaP"] = (i < deltaPs.Length) ? deltaPs[i].Trim() : "";
                    dr["ConditionDeltaP"] = (i < conditionDeltaPs.Length) ? conditionDeltaPs[i].Trim() : "";
                    dr["Decision"] = (i < decisions.Length) ? decisions[i].Trim() : "";
                    dr["AdditionalInfo"] = (i < additionalInfos.Length) ? additionalInfos[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            ViewState["TestData2"] = gridDt;
            gvTests2.DataSource = gridDt;
            gvTests2.DataBind();
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
        

    //    private void CreateHeatingTestGrid()
    //    {
    //        DataTable dt = new DataTable();
    //        dt.Columns.Add("TestVoltage");           // 17854
    //        dt.Columns.Add("TestVoltageValue");      // 17855
    //        dt.Columns.Add("AmbientTemp");           // 17856
    //        dt.Columns.Add("AmbientTempValue");      // 17857
    //        dt.Columns.Add("ThermocoupleLocation");  // 17858
    //        dt.Columns.Add("MaxMeasuredTemp");       // 17859
    //        dt.Columns.Add("MaxTempLimit");          // 17860

    //        ViewState["HeatingTestData"] = dt;
    //        gvHeatingTests.DataSource = dt;
    //        gvHeatingTests.DataBind();
    //    }

    //    // ADD BUTTON CLICK EVENT
    //    protected void btnAddHeating_Click(object sender, EventArgs e)
    //    {
    //        DataTable dt = ViewState["HeatingTestData"] as DataTable;

    //        if (dt == null)
    //        {
    //            CreateHeatingTestGrid();
    //            dt = ViewState["HeatingTestData"] as DataTable;
    //        }

    //        DataRow dr = dt.NewRow();
    //        dr["TestVoltage"] = txtTestVoltage.Text.Trim();
    //        dr["TestVoltageValue"] = txtTestVoltageValue.Text.Trim();
    //        dr["AmbientTemp"] = txtAmbientTemp.Text.Trim();
    //        dr["AmbientTempValue"] = txtAmbientTempValue.Text.Trim();
    //        dr["ThermocoupleLocation"] = txtThermocoupleLocation.Text.Trim();
    //        dr["MaxMeasuredTemp"] = txtMaxMeasuredTemp.Text.Trim();
    //        dr["MaxTempLimit"] = txtMaxTempLimit.Text.Trim();

    //        dt.Rows.Add(dr);
    //        ViewState["HeatingTestData"] = dt;
    //        gvHeatingTests.DataSource = dt;
    //        gvHeatingTests.DataBind();

    //        // Clear TextBoxes after adding
    //        txtThermocoupleLocation.Text = "";
    //        txtMaxMeasuredTemp.Text = "";
    //        txtMaxTempLimit.Text = "";
    //    }

    //    // DELETE ROW EVENT
    //    protected void gvHeatingTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
    //    {
    //        DataTable dt = ViewState["HeatingTestData"] as DataTable;
    //        if (dt != null && dt.Rows.Count > e.RowIndex)
    //        {
    //            dt.Rows.RemoveAt(e.RowIndex);
    //            ViewState["HeatingTestData"] = dt;
    //            gvHeatingTests.DataSource = dt;
    //            gvHeatingTests.DataBind();
    //        }
    //    }

    //    // PREFILL DATA WHEN EDITING
    //    private void PrefillHeatingTestGrid(DataTable dt)
    //    {
    //        DataTable gridDt = new DataTable();
    //        gridDt.Columns.Add("TestVoltage");
    //        gridDt.Columns.Add("TestVoltageValue");
    //        gridDt.Columns.Add("AmbientTemp");
    //        gridDt.Columns.Add("AmbientTempValue");
    //        gridDt.Columns.Add("ThermocoupleLocation");
    //        gridDt.Columns.Add("MaxMeasuredTemp");
    //        gridDt.Columns.Add("MaxTempLimit");

    //        // Get CSV strings from saved data (using IDs 17854-17860)
    //        string testVoltageCsv = GetResultValue(dt, "17854");
    //        string testVoltageValueCsv = GetResultValue(dt, "17855");
    //        string ambientTempCsv = GetResultValue(dt, "17856");
    //        string ambientTempValueCsv = GetResultValue(dt, "17857");
    //        string thermocoupleLocationsCsv = GetResultValue(dt, "17858");
    //        string maxMeasuredTempsCsv = GetResultValue(dt, "17859");
    //        string maxTempLimitsCsv = GetResultValue(dt, "17860");

    //        // Set the single-value fields
    //        if (!string.IsNullOrEmpty(testVoltageCsv))
    //        {
    //            txtTestVoltage.Text = testVoltageCsv;
    //            txtTestVoltageValue.Text = testVoltageValueCsv ?? "";
    //            txtAmbientTemp.Text = ambientTempCsv ?? "";
    //            txtAmbientTempValue.Text = ambientTempValueCsv ?? "";
    //        }

    //        if (!string.IsNullOrEmpty(thermocoupleLocationsCsv))
    //        {
    //            string[] thermocoupleLocations = thermocoupleLocationsCsv.Split(',');
    //            string[] maxMeasuredTemps = maxMeasuredTempsCsv?.Split(',') ?? new string[0];
    //            string[] maxTempLimits = maxTempLimitsCsv?.Split(',') ?? new string[0];

    //            for (int i = 0; i < thermocoupleLocations.Length; i++)
    //            {
    //                DataRow dr = gridDt.NewRow();
    //                dr["TestVoltage"] = testVoltageCsv;
    //                dr["TestVoltageValue"] = testVoltageValueCsv ?? "";
    //                dr["AmbientTemp"] = ambientTempCsv ?? "";
    //                dr["AmbientTempValue"] = ambientTempValueCsv ?? "";
    //                dr["ThermocoupleLocation"] = thermocoupleLocations[i].Trim();
    //                dr["MaxMeasuredTemp"] = (i < maxMeasuredTemps.Length) ? maxMeasuredTemps[i].Trim() : "";
    //                dr["MaxTempLimit"] = (i < maxTempLimits.Length) ? maxTempLimits[i].Trim() : "";
    //                gridDt.Rows.Add(dr);
    //            }
    //        }

    //        ViewState["HeatingTestData"] = gridDt;
    //        gvHeatingTests.DataSource = gridDt;
    //        gvHeatingTests.DataBind();
    //    }
    //    private void CreateReportTableHeatingTest(Application wordApp, Document doc, string placeholder, DataTable dt)
    //    {
    //        Range range = doc.Content;
    //        Find find = range.Find;
    //        find.ClearFormatting();
    //        find.Text = placeholder;

    //        if (!find.Execute())
    //            throw new Exception($"Placeholder '{placeholder}' not found in the document template.");

    //        range.Text = "TABLE: Heating Test";
    //        range.Font.Size = 12;
    //        range.Font.Bold = 1;
    //        range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
    //        range.ParagraphFormat.SpaceBefore = 0;
    //        range.ParagraphFormat.SpaceAfter = 0;

    //        Range tableRange = doc.Range(range.End, range.End);
    //        tableRange.Collapse(WdCollapseDirection.wdCollapseEnd);

    //        Dictionary<int, int> columnMapping = new Dictionary<int, int>
    //{
    //    { 17854, 0 }, // Test voltage
    //    { 17855, 1 }, // Test voltage value
    //    { 17856, 2 }, // Ambient temperature
    //    { 17857, 3 }, // Ambient temperature value
    //    { 17858, 4 }, // Thermocouple location
    //    { 17859, 5 }, // Max measured temp
    //    { 17860, 6 }  // Max temp limit
    //};

    //        List<string[]> parsedColumns = new List<string[]>();
    //        for (int i = 0; i < 7; i++)
    //        {
    //            parsedColumns.Add(new string[0]);
    //        }

    //        foreach (DataRow row in dt.Rows)
    //        {
    //            int reportParamMapID = Convert.ToInt32(row["ReportParamMapID"]);
    //            if (columnMapping.ContainsKey(reportParamMapID))
    //            {
    //                int columnIndex = columnMapping[reportParamMapID];
    //                string[] values = row["Result"].ToString().Split(',');
    //                parsedColumns[columnIndex] = values;
    //            }
    //        }

    //        int numDataRows = parsedColumns[4].Length; // Based on thermocouple locations

    //        // Create table: header rows (4) + data rows
    //        Table table = doc.Tables.Add(tableRange, 4 + numDataRows, 5);
    //        table.Borders.Enable = 1;
    //        table.AllowAutoFit = false;

    //        float totalWidthInches = 6.5f;
    //        table.PreferredWidth = wordApp.InchesToPoints(totalWidthInches);

    //        // Merge cells for row 1
    //        table.Cell(1, 1).Merge(table.Cell(1, 1));
    //        table.Cell(1, 2).Merge(table.Cell(1, 4));
    //        table.Cell(1, 1).Range.Text = "11.8";
    //        table.Cell(1, 2).Range.Text = "TABLE: Heating Test";
    //        table.Cell(1, 2).Range.Bold = 1;

    //        // Row 2: Test voltage
    //        string testVoltage = parsedColumns[0].Length > 0 ? parsedColumns[0][0] : "";
    //        string testVoltageValue = parsedColumns[1].Length > 0 ? parsedColumns[1][0] : "";

    //        table.Cell(2, 2).Merge(table.Cell(2, 3));
    //        table.Cell(2, 2).Range.Text = testVoltage;
    //        table.Cell(2, 4).Merge(table.Cell(2, 5));
    //        table.Cell(2, 4).Range.Text = testVoltageValue;

    //        // Row 3: Ambient temperature
    //        string ambientTemp = parsedColumns[2].Length > 0 ? parsedColumns[2][0] : "";
    //        string ambientTempValue = parsedColumns[3].Length > 0 ? parsedColumns[3][0] : "";

    //        table.Cell(3, 2).Merge(table.Cell(3, 3));
    //        table.Cell(3, 2).Range.Text = ambientTemp;
    //        table.Cell(3, 4).Merge(table.Cell(3, 5));
    //        table.Cell(3, 4).Range.Text = ambientTempValue;

    //        // Row 4: Headers
    //        table.Cell(4, 1).Merge(table.Cell(4, 2));
    //        table.Cell(4, 1).Range.Text = "Thermocouple location";
    //        table.Cell(4, 1).Range.Bold = 1;
    //        table.Cell(4, 1).Shading.BackgroundPatternColor = WdColor.wdColorGray25;

    //        table.Cell(4, 3).Range.Text = "Max. measured temperature\r\nrise, ΔT (K)";
    //        table.Cell(4, 3).Range.Bold = 1;
    //        table.Cell(4, 3).Shading.BackgroundPatternColor = WdColor.wdColorGray25;
    //        table.Cell(4, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

    //        table.Cell(4, 4).Merge(table.Cell(4, 5));
    //        table.Cell(4, 4).Range.Text = "Maximum temperature rise\r\nlimit, ΔT (K)";
    //        table.Cell(4, 4).Range.Bold = 1;
    //        table.Cell(4, 4).Shading.BackgroundPatternColor = WdColor.wdColorGray25;
    //        table.Cell(4, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

    //        // Fill data rows (starting from row 5)
    //        for (int i = 0; i < numDataRows; i++)
    //        {
    //            int rowNum = 5 + i;

    //            table.Cell(rowNum, 1).Merge(table.Cell(rowNum, 2));
    //            table.Cell(rowNum, 1).Range.Text = i < parsedColumns[4].Length ? parsedColumns[4][i].Trim() : "";

    //            table.Cell(rowNum, 3).Range.Text = i < parsedColumns[5].Length ? parsedColumns[5][i].Trim() : "";
    //            table.Cell(rowNum, 3).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

    //            table.Cell(rowNum, 4).Merge(table.Cell(rowNum, 5));
    //            table.Cell(rowNum, 4).Range.Text = i < parsedColumns[6].Length ? parsedColumns[6][i].Trim() : "";
    //            table.Cell(rowNum, 4).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
    //        }

    //        // Format all cells
    //        for (int row = 1; row <= table.Rows.Count; row++)
    //        {
    //            for (int col = 1; col <= table.Columns.Count; col++)
    //            {
    //                try
    //                {
    //                    Cell cell = table.Cell(row, col);
    //                    cell.Range.Font.Size = 10;
    //                    cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
    //                    cell.Range.ParagraphFormat.SpaceBefore = 0;
    //                    cell.Range.ParagraphFormat.SpaceAfter = 0;
    //                }
    //                catch { } // Skip merged cells
    //            }
    //        }
    //    }
        //private void PrefillAllData(DataTable dt)
        //{
        //    PrefillSingleValues(dt);
        //    PrefillTwoValues(dt);
        //    PrefillSixValues(dt);
        //}
        //private void PrefillSingleValues(DataTable dt)
        //{
        //    Dictionary<string, TextBox> singleValueMap = new Dictionary<string, TextBox>
        //    {
        //        { "17819", txtActinicUV1 },
        //        { "17820", txtActinicUV2 },
        //        { "17821", txtActinicUV3 },
        //        { "17822", txtNearUV1 },
        //        { "17823", txtNearUV2 },
        //        { "17824", txtNearUV3 },
        //        { "17825", txtBlueLight1 },
        //        { "17826", txtBlueLight2 },
        //        { "17827", txtBlueLight3 },
        //        { "17828", txtBlueLightHazardRadiance },
        //        { "17829", txtLuminance }
        //    };

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string paramId = row["ParameterID"].ToString();
        //        string rawValue = row["Result"].ToString();

        //        if (singleValueMap.ContainsKey(paramId))
        //        {
        //            singleValueMap[paramId].Text = rawValue;
        //        }
        //    }
        //}
        //private void PrefillTwoValues(DataTable dt)
        //{
        //    Dictionary<string, (TextBox result, TextBox remark)> twoValueMap = new Dictionary<string, (TextBox, TextBox)>
        //    {
        //        { "17830", (txtProductTypeResult, txtProductTypeRemark) },
        //        { "17831", (txtTheLightsUsedResult, txtTheLightsUsedRemark) },
        //        { "17832", (txtTypeOfLEDDriverUsedResult, txtTypeOfLEDDriverUsedRemark) },
        //        { "17833", (txtMountingPositionDuringTestingResult, txttxtMountingPositionDuringTestingRemark) },
        //        { "17834", (txtPowerWResult, txtPowerWRemark) },
        //        { "17835", (txtCurrentAResult, txtCurrentARemark) },
        //        { "17836", (txtTest1RatedVoltageResult, txtTest1RatedVoltageRemark) },
        //        { "17837", (txtTest2Result, txtTest2Remark) },
        //        { "17838", (txtTest3Result, txtTest3Remark) },
        //        { "17839", (txtTest4Result, txtTest4Remark) }
        //    };

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string paramId = row["ParameterID"].ToString();
        //        string rawValue = row["Result"].ToString();

        //        if (twoValueMap.ContainsKey(paramId))
        //        {
        //            var controls = twoValueMap[paramId];
        //            string[] parts = rawValue.Split('|');

        //            if (parts.Length >= 2)
        //            {
        //                controls.result.Text = parts[0];
        //                controls.remark.Text = parts[1];
        //            }
        //        }
        //    }
        //}
        //private void PrefillSixValues(DataTable dt)
        //{
        //    Dictionary<string, (TextBox test1, TextBox test2, TextBox test3, TextBox limitMax1, TextBox test4, TextBox limitMax2)> sixValueMap = new Dictionary<string, (TextBox, TextBox, TextBox, TextBox, TextBox, TextBox)>
        //    {
        //        { "17840", (txtTestChamberTemperatureTest1, txtTestChamberTemperatureTest2, txtTestChamberTemperatureTest3, txtTestChamberTemperatureLimitsMax1, txtTestChamberTemperatureTest4, txtTestChamberTemperatureLimitsMax2) },
        //        { "17841", (txtOuterSurfaceOfTheLuminaireTest1, txtOuterSurfaceOfTheLuminaireTest2, txtOuterSurfaceOfTheLuminaireTest3, txtOuterSurfaceOfTheLuminaireLimitMax1, txtOuterSurfaceOfTheLuminaireTest4, txtOuterSurfaceOfTheLuminaireLimitMax2) },
        //        { "17842", (txtSurfaceLEDDriverTest1, txtSurfaceLEDDriverTest2, txtSurfaceLEDDriverTest3, txtSurfaceLEDDriverLimitMax1, txtSurfaceLEDDriverTest4, txtSurfaceLEDDriverLimitMax2) },
        //        { "17843", (txtLEDModulePCBTest1, txtLEDModulePCBTest2, txtLEDModulePCBTest3, txtLEDModulePCBLimitMax1, txtLEDModulePCBTest4, txtLEDModulePCBLimitMax2) },
        //        { "17844", (txtLEDModuleEnclosurePlasticTest1, txtLEDModuleEnclosurePlasticTest2, txtLEDModuleEnclosurePlasticTest3, txtLEDModuleEnclosurePlasticLimitMax1, txtLEDModuleEnclosurePlasticTest4, txtLEDModuleEnclosurePlasticLimitMax2) },
        //        { "17845", (txtExternalCableTest1, txtExternalCableTest2, txtExternalCableTest3, txtExternalCableLimitMax1, txtExternalCableTest4, txtExternalCableLimitMax2) },
        //        { "17846", (txtInternalWiringTest1, txtInternalWiringTest2, txtInternalWiringTest3, txtInternalWiringLimitMax1, txtInternalWiringTest4, txtInternalWiringLimitMax2) }
        //    };

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string paramId = row["ParameterID"].ToString();
        //        string rawValue = row["Result"].ToString();

        //        if (sixValueMap.ContainsKey(paramId))
        //        {
        //            var controls = sixValueMap[paramId];
        //            string[] parts = rawValue.Split('|');

        //            if (parts.Length >= 6)
        //            {
        //                controls.test1.Text = parts[0];
        //                controls.test2.Text = parts[1];
        //                controls.test3.Text = parts[2];
        //                controls.limitMax1.Text = parts[3];
        //                controls.test4.Text = parts[4];
        //                controls.limitMax2.Text = parts[5];
        //            }
        //        }
        //    }
        //}

    }
}