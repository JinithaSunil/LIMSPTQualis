using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LIMS
{
    public partial class GenerateTestRecord_TensileStrengthandElongation : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int InquiryID = 0;
        int CloneCRFSampleID = 0;
        int SampleID = 0;
        int ReportId = 0;
        int TestRequestReviewID = 0;
        StringBuilder sbCustomer = new StringBuilder();
        StringBuilder sbDates = new StringBuilder();
        StringBuilder sbFooter = new StringBuilder();
        DataTable dt6 = new DataTable();
        string body = "";
        string str = "";
        string filename = "";
        string html = "";
        mailer objEmail = new mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["s"] != null)
                {
                    Session["SampleID"] = Request.QueryString["s"].ToString();
                }
                if (Request.QueryString["r"] != null)
                {
                    Session["ReportId"] = Request.QueryString["r"].ToString();
                }
                if (Session["SampleID"] != null && Session["ReportId"] != null)
                {
                    SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    ReportId = Convert.ToInt32(Session["ReportId"].ToString());

                    if (Session["Usertype"] != null)
                    {
                        string Type = "";

                        // Dictionary to store data - using "1" as the key since we're treating this as one sample set
                        Dictionary<string, List<string>> ItemTest = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Core = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SampleType = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecimenNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThicknessAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Width = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WidthAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Area = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BF = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationL0 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationL1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MedianTS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MedianEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> VariationTS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> VariationEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TestDate = new Dictionary<string, List<string>>();

                        // Single value parameters
                        string Size = "";
                        string Voltage = "";
                        string Date = "";
                        string RoomNo = "";
                        string RoomTemp = "";
                        string RefSpec = "";
                        string TestMethodNo = "";
                        string JobOrderNo = "";
                        string WorkSheetNo = "";
                        string RoomHumidity = "";
                        string EquipmentNo = "";
                        string TestItem = "";
                        string Length = "";
                        string DrumNo = "";

                        // Specified values
                        string SpecThickness = "";
                        string SpecWidth = "";
                        string SpecArea = "";
                        string SpecBF = "";
                        string SpecTS = "";
                        string SpecL0 = "";
                        string SpecL1 = "";
                        string SpecEL = "";
                        string SpecMedianTS = "";
                        string SpecMedianEL = "";
                        string SpecVarTS = "";
                        string SpecVarEL = "";

                        //Date&Time
                        string AgeingCompleteCableStartDate = "";
                        string AgeingCompleteCableFinishDate = "";
                        string AgeingConditioningInDesciccatorsStartDate = "";
                        string AgeingConditioningStartDate = "";
                        string AgeingConditioningFinishDate = "";
                        string AgeingConditioningInDesciccatorsFinishDate = "";
                        string AgeingSpecimenStartDate = "";
                        string AgeingSpecimenFinishDate = "";
                        string AgeingSpecimenAirflow = "";
                        string AgeingCompleteCableAirflow = "";

                        DataTable dtRecordDetails = new DataTable();
                        StringBuilder sbHeader = new StringBuilder();
                        StringBuilder sbDetails = new StringBuilder();
                        StringBuilder sb = new StringBuilder();

                        // Fixed AddDataColumns method
                        void AddDataColumns(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
                            string sampleKey1, int dateIndex, int coreGroupIndex, int specimenIndex, int maxColumns)
                        {
                            List<string> values = new List<string>();

                            if (dataDict.ContainsKey(sampleKey1) && dateIndex < dataDict[sampleKey1].Count)
                            {
                                string dateGroup = dataDict[sampleKey1][dateIndex];
                                string[] coreGroups = dateGroup.Split(';');

                                if (coreGroupIndex < coreGroups.Length)
                                {
                                    string coreGroup = coreGroups[coreGroupIndex];

                                    // Find all parenthetical groups in this core group
                                    List<string> specimenGroups = new List<string>();
                                    int pos = 0;
                                    while (pos < coreGroup.Length)
                                    {
                                        int start = coreGroup.IndexOf('(', pos);
                                        if (start == -1) break;
                                        int end = coreGroup.IndexOf(')', start);
                                        if (end == -1) break;

                                        string content = coreGroup.Substring(start + 1, end - start - 1);
                                        specimenGroups.Add(content);
                                        pos = end + 1;
                                    }

                                    if (specimenIndex < specimenGroups.Count)
                                    {
                                        string specimenData = specimenGroups[specimenIndex];
                                        if (!string.IsNullOrWhiteSpace(specimenData))
                                        {
                                            // Split the specimen data by comma to get individual measurements
                                            values = specimenData.Split(',').Select(v => v.Trim()).ToList();
                                        }
                                    }
                                }
                            }

                            // Print values with padding
                            for (int i = 0; i < maxColumns; i++)
                            {
                                string value = i < values.Count ? values[i] : "-";
                                sb1.Append($"<td style='vertical-align: middle;'>{value}</td>");
                            }
                        }
                        // Fixed AddAvgColumn method for ThicknessAvg and WidthAvg
                        void AddAvgColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
                           string sampleKey1, int dateIndex, int coreGroupIndex, int specimenIndex)
                        {
                            string avgValue = "-";
                            string minValue = "-";

                            if (dataDict.ContainsKey(sampleKey1) && dateIndex < dataDict[sampleKey1].Count)
                            {
                                string dateGroup = dataDict[sampleKey1][dateIndex];
                                string[] coreGroups = dateGroup.Split(';');

                                if (coreGroupIndex < coreGroups.Length)
                                {
                                    string coreGroup = coreGroups[coreGroupIndex];

                                    // Find all parenthetical groups in this core group
                                    List<string> specimenGroups = new List<string>();
                                    int pos = 0;
                                    while (pos < coreGroup.Length)
                                    {
                                        int start = coreGroup.IndexOf('(', pos);
                                        if (start == -1) break;
                                        int end = coreGroup.IndexOf(')', start);
                                        if (end == -1) break;

                                        string content = coreGroup.Substring(start + 1, end - start - 1);
                                        specimenGroups.Add(content);
                                        pos = end + 1;
                                    }

                                    if (specimenIndex < specimenGroups.Count)
                                    {
                                        string specimenData = specimenGroups[specimenIndex];
                                        if (!string.IsNullOrWhiteSpace(specimenData))
                                        {
                                            // Parse the avg/min format: "Avg: 3.00, Min: 3.00"
                                            string[] parts = specimenData.Split(',');
                                            foreach (string part in parts)
                                            {
                                                string trimmedPart = part.Trim();
                                                if (trimmedPart.StartsWith("Avg:"))
                                                {
                                                    avgValue = trimmedPart.Replace("Avg:", "").Trim();
                                                }
                                                else if (trimmedPart.StartsWith("Min:"))
                                                {
                                                    minValue = trimmedPart.Replace("Min:", "").Trim();
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // Display both Avg and Min in the cell
                            string displayValue = avgValue != "-" ? $"Avg: {avgValue}<br/>Min: {minValue}" : "-";
                            sb1.Append($"<td style='vertical-align: middle;'>{displayValue}</td>");
                        }
                        // Fixed AddSingleValueColumn method
                        void AddSingleValueColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
                            string sampleKey1, int dateIndex, int coreGroupIndex, int specimenIndex)
                        {
                            string value = "-";

                            if (dataDict.ContainsKey(sampleKey1) && dateIndex < dataDict[sampleKey1].Count)
                            {
                                string dateGroup = dataDict[sampleKey1][dateIndex];
                                string[] coreGroups = dateGroup.Split(';');

                                if (coreGroupIndex < coreGroups.Length)
                                {
                                    string coreGroup = coreGroups[coreGroupIndex];

                                    // Find all parenthetical groups in this core group
                                    List<string> specimenGroups = new List<string>();
                                    int pos = 0;
                                    while (pos < coreGroup.Length)
                                    {
                                        int start = coreGroup.IndexOf('(', pos);
                                        if (start == -1) break;
                                        int end = coreGroup.IndexOf(')', start);
                                        if (end == -1) break;

                                        string content = coreGroup.Substring(start + 1, end - start - 1);
                                        specimenGroups.Add(content);
                                        pos = end + 1;
                                    }

                                    if (specimenIndex < specimenGroups.Count)
                                    {
                                        string specimenData = specimenGroups[specimenIndex];
                                        if (!string.IsNullOrWhiteSpace(specimenData))
                                        {
                                            // For single value columns, take the first value or the whole content
                                            if (specimenData.Contains(","))
                                            {
                                                value = specimenData.Split(',')[0].Trim();
                                            }
                                            else
                                            {
                                                value = specimenData.Trim();
                                            }
                                        }
                                    }
                                }
                            }

                            sb1.Append($"<td style='vertical-align: middle;'>{value}</td>");
                        }
                        // Fixed GetMaxInputsInDataSet method
                        int GetMaxInputsInDataSet(Dictionary<string, List<string>> dataSet)
                        {
                            int maxInputs = 0;
                            foreach (var sampleKey1 in dataSet.Keys)
                            {
                                foreach (var dateGroup in dataSet[sampleKey1])
                                {
                                    // Split by semicolon to get core groups
                                    string[] coreGroups = dateGroup.Split(';');
                                    foreach (var coreGroup in coreGroups)
                                    {
                                        if (string.IsNullOrWhiteSpace(coreGroup)) continue;

                                        // Find all parenthetical groups in this core group
                                        int pos = 0;
                                        while (pos < coreGroup.Length)
                                        {
                                            int start = coreGroup.IndexOf('(', pos);
                                            if (start == -1) break;
                                            int end = coreGroup.IndexOf(')', start);
                                            if (end == -1) break;

                                            string content = coreGroup.Substring(start + 1, end - start - 1);
                                            if (!string.IsNullOrWhiteSpace(content))
                                            {
                                                // Count comma-separated values in this specimen group
                                                string[] values = content.Split(',');
                                                // For Avg columns, we only count non-Avg/Min entries
                                                if (!content.Contains("Avg:"))
                                                {
                                                    maxInputs = Math.Max(maxInputs, values.Length);
                                                }
                                            }
                                            pos = end + 1;
                                        }
                                    }
                                }
                            }
                            return maxInputs;
                        }


                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        string sampleKey = "1";
                        TestDate[sampleKey] = new List<string>();
                        ItemTest[sampleKey] = new List<string>();
                        Core[sampleKey] = new List<string>();
                        SampleType[sampleKey] = new List<string>();
                        SpecimenNo[sampleKey] = new List<string>();
                        Thickness[sampleKey] = new List<string>();
                        ThicknessAvg[sampleKey] = new List<string>();
                        Width[sampleKey] = new List<string>();
                        WidthAvg[sampleKey] = new List<string>();
                        Area[sampleKey] = new List<string>();
                        BF[sampleKey] = new List<string>();
                        TS[sampleKey] = new List<string>();
                        ElongationL0[sampleKey] = new List<string>();
                        ElongationL1[sampleKey] = new List<string>();
                        ElongationEL[sampleKey] = new List<string>();
                        MedianTS[sampleKey] = new List<string>();
                        MedianEL[sampleKey] = new List<string>();
                        VariationTS[sampleKey] = new List<string>();
                        VariationEL[sampleKey] = new List<string>();
                        if (dtRecordDetails.Rows.Count > 0)
                        {


                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();

                                // Process hierarchical data (pipe-separated date groups)
                                switch (reportParamMapID)
                                {
                                    case "17395": // Date
                                        TestDate[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17396": // Core
                                        Core[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17397": // ItemTest
                                        ItemTest[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17398": // Sample Type
                                        SampleType[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17399": // SpecimenNo
                                        SpecimenNo[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17400": // Thickness
                                        Thickness[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17401": // ThicknessAvg
                                        ThicknessAvg[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17402": // Width
                                        Width[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17403": // WidthAvg
                                        WidthAvg[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17404": // Area
                                        Area[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17405": // BF
                                        BF[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17406": // TS
                                        TS[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17407": // ElongationL0
                                        ElongationL0[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17408": // ElongationL1
                                        ElongationL1[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17409": // ElongationEL
                                        ElongationEL[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17410": // MedianTS
                                        MedianTS[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17411": // MedianEL
                                        MedianEL[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17412": // VariationTS
                                        VariationTS[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17413": // VariationEL
                                        VariationEL[sampleKey].AddRange(result.Split('|'));
                                        break;
                                }

                                // Single value parameters
                                if (dr["ReportParamMapID"].ToString() == "17380") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17381") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17382") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17383") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17384") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17385") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17386") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17387") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17388") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17389") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17390") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17391") { EquipmentNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17392") { TestItem = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17393") { Length = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17394") { DrumNo = dr["Result"].ToString(); }

                                // Specified values
                                if (dr["ReportParamMapID"].ToString() == "17491") { SpecThickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17492") { SpecWidth = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17493") { SpecArea = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17494") { SpecBF = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17495") { SpecTS = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17496") { SpecL0 = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17497") { SpecL1 = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17498") { SpecEL = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17499") { SpecMedianTS = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17500") { SpecMedianEL = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17501") { SpecVarTS = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17502") { SpecVarEL = dr["Result"].ToString(); }

                                //Date&Time
                                if (dr["ReportParamMapID"].ToString() == "17486") { AgeingCompleteCableStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17487") { AgeingCompleteCableFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17488") { AgeingConditioningInDesciccatorsStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17489") { AgeingConditioningStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17490") { AgeingConditioningFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17480") { AgeingConditioningInDesciccatorsFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17481") { AgeingSpecimenStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17482") { AgeingSpecimenFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17483") { AgeingSpecimenAirflow = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17484") { AgeingCompleteCableAirflow = dr["Result"].ToString(); }

                            }
                        }


                        // Build header table
                        sbHeader.Append(@"
                        <table style='width:100%; border:1px solid; border-collapse:collapse; margin-top:10px;' class='tab1' cellpadding='3' cellspacing='0'>
                            <tr>
                                <td style='width:20%;border-right: none;'>The Test </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + TestItem + @"</td>
                                <td style='width:15%;border-right: none;'>Length</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + Length + @"</td>
                                <td style='width:15%;border-right: none;'>Drum No.</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + DrumNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border-right: none;'>Type  </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + Type + @"</td>
                                <td style='border-right: none;'>Room No.</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + RoomNo + @"</td>
                                <td style='border-right: none;'>Ref. Spec.</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + RefSpec + @"</td>
                            </tr>
                            <tr>
                                <td style='border-right: none;'>Size       </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + Size + @"</td>
                                <td style='border-right: none;'>Room Temp. </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + RoomTemp + " °C" + @"</td>
                                <td style='border-right: none;'>Test Method No.</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + TestMethodNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border-right: none;'>Voltage  </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + Voltage + @"</td>
                                <td style='border-right: none;'>Room Humidity</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + RoomHumidity + " %" + @"</td>
                                <td style='border-right: none;'>Job Order No.</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + JobOrderNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border-right: none;'>Date </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + Date + @"</td>
                                <td style='border-right: none;'>Work Sheet No.</td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + WorkSheetNo + @"</td>
                                <td style='border-right: none;'>Equipment No. </td><td style='width:2%;border-left: none;border-right: none;'>:</td><td style='border-left: none;'>" + EquipmentNo + @"</td>
                            </tr>
                        </table>");

                        // Find maximum number of thickness and width measurements
                        int maxThicknessInputs = GetMaxInputsInDataSet(Thickness);
                        int maxWidthInputs = GetMaxInputsInDataSet(Width);

                        // Create expanded rows for the table
                        List<dynamic> expandedRows = new List<dynamic>();

                        // Process each date group
                        for (int dateIndex = 0; dateIndex < TestDate[sampleKey].Count; dateIndex++)
                        {
                            string currentDate = TestDate[sampleKey][dateIndex];
                            string currentItemTest = dateIndex < ItemTest[sampleKey].Count ? ItemTest[sampleKey][dateIndex] : "";
                            string coreGroup = dateIndex < Core[sampleKey].Count ? Core[sampleKey][dateIndex] : "";
                            string sampleGroup = dateIndex < SampleType[sampleKey].Count ? SampleType[sampleKey][dateIndex] : "";
                            string specimenGroup = dateIndex < SpecimenNo[sampleKey].Count ? SpecimenNo[sampleKey][dateIndex] : "";

                            // Split by semicolon to get core groups
                            string[] coreGroups = coreGroup.Split(';');
                            string[] sampleGroups = sampleGroup.Split(';');
                            string[] specimenGroups = specimenGroup.Split(';');

                            // Process each core group
                            for (int coreGroupIndex = 0; coreGroupIndex < coreGroups.Length; coreGroupIndex++)
                            {
                                string currentCoreGroup = coreGroups[coreGroupIndex];
                                string currentSampleGroup = coreGroupIndex < sampleGroups.Length ? sampleGroups[coreGroupIndex] : "";
                                string currentSpecimenGroup = coreGroupIndex < specimenGroups.Length ? specimenGroups[coreGroupIndex] : "";

                                // Split by comma to get individual specimens
                                string[] cores = currentCoreGroup.Split(',');
                                string[] samples = currentSampleGroup.Split(',');
                                string[] specimens = currentSpecimenGroup.Split(',');

                                // Process each specimen
                                for (int specimenIndex = 0; specimenIndex < Math.Max(Math.Max(cores.Length, samples.Length), specimens.Length); specimenIndex++)
                                {
                                    string core = specimenIndex < cores.Length ? cores[specimenIndex].Trim() : "";
                                    string sample = specimenIndex < samples.Length ? samples[specimenIndex].Trim() : "";
                                    string specimen = specimenIndex < specimens.Length ? specimens[specimenIndex].Trim() : "";

                                    expandedRows.Add(new
                                    {
                                        Date = currentDate,
                                        ItemTest = currentItemTest,
                                        Core = core,
                                        SampleType = sample,
                                        SpecimenNo = specimen,
                                        DateIndex = dateIndex,
                                        CoreGroupIndex = coreGroupIndex,
                                        SpecimenIndex = specimenIndex,
                                        GlobalRowIndex = expandedRows.Count
                                    });
                                }
                            }
                        }

                        // Only proceed if we have expanded rows
                        if (expandedRows.Count > 0)
                        {
                            // Calculate rowspans for Date and ItemTest
                            Dictionary<int, int> dateRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> dateFirstOccurrence = new Dictionary<int, bool>();
                            Dictionary<int, int> itemTestRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> itemTestFirstOccurrence = new Dictionary<int, bool>();

                            for (int i = 0; i < expandedRows.Count; i++)
                            {
                                // Date rowspan calculation - now using DateIndex instead of Date value
                                if (i == 0 || expandedRows[i].DateIndex != expandedRows[i - 1].DateIndex)
                                {
                                    dateFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count && expandedRows[j].DateIndex == expandedRows[i].DateIndex; j++)
                                    {
                                        rowspan++;
                                    }
                                    dateRowspans[i] = rowspan;
                                }
                                else
                                {
                                    dateFirstOccurrence[i] = false;
                                }

                                // ItemTest rowspan calculation - also using DateIndex for proper grouping
                                if (i == 0 || expandedRows[i].ItemTest != expandedRows[i - 1].ItemTest || expandedRows[i].DateIndex != expandedRows[i - 1].DateIndex)
                                {
                                    itemTestFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count && expandedRows[j].ItemTest == expandedRows[i].ItemTest && expandedRows[j].DateIndex == expandedRows[i].DateIndex; j++)
                                    {
                                        rowspan++;
                                    }
                                    itemTestRowspans[i] = rowspan;
                                }
                                else
                                {
                                    itemTestFirstOccurrence[i] = false;
                                }
                            }

                            // Build main data table header
                            sbDetails.Append($@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse; margin-top: 10px;'>
                            <tr>
                            <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Date</strong></td>
                            <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Item Test</strong></td>
                            <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Core Phase</strong></td>
                            <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Type of sample test</strong></td>
                            <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Specimen No.</strong></td>
                            <td colspan='{maxThicknessInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>Thickness/Weight</strong></td>
                            <td colspan='{maxWidthInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>Width/Diameter/Length/Density</strong></td>
                            <td rowspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>Area</strong></td>
                            <td rowspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>BF</strong></td>
                            <td rowspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>TS</strong></td>
                            <td colspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Elongation</strong></td>
                            <td colspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>Median</strong></td>
                            <td colspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>Variation</strong></td>
                            </tr>
                            <tr>");

                            // Thickness measurement columns
                            for (int i = 1; i <= maxThicknessInputs; i++)
                            {
                                sbDetails.Append($"<td style='background-color:#bebebe; vertical-align: middle;'><strong>{i}</strong></td>");
                            }
                            sbDetails.Append("<td style='background-color:#bebebe; vertical-align: middle;'><strong>Avg. min.</strong></td>");

                            // Width measurement columns
                            for (int i = 1; i <= maxWidthInputs; i++)
                            {
                                sbDetails.Append($"<td style='background-color:#bebebe; vertical-align: middle;'><strong>{i}</strong></td>");
                            }
                            sbDetails.Append("<td style='background-color:#bebebe; vertical-align: middle;'><strong>Avg. min.</strong></td>");

                            // Other grouped headers
                            sbDetails.Append(@"
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>L0</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>L1</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>EL.</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>TS</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>EL.</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>TS</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>EL.</strong></td>
                        </tr>
                        <tr>");

                            // Units row
                            sbDetails.Append($"<td colspan='{maxThicknessInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>mm/gr</strong></td>");
                            sbDetails.Append($"<td colspan='{maxWidthInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>mm/(g/cm³)</strong></td>");


                            sbDetails.Append(@"
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm²</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>N</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>N/mm²</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>N/mm²</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
                            <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>

                        </tr>");


                            // Generate data rows
                            for (int rowIndex = 0; rowIndex < expandedRows.Count; rowIndex++)
                            {
                                sbDetails.Append("<tr>");

                                // Date column with dynamic rowspan
                                if (dateFirstOccurrence[rowIndex])
                                {
                                    sbDetails.Append($"<td rowspan='{dateRowspans[rowIndex]}' style='vertical-align: middle;'>{expandedRows[rowIndex].Date}</td>");
                                }

                                // ItemTest column with dynamic rowspan
                                if (itemTestFirstOccurrence[rowIndex])
                                {
                                    sbDetails.Append($"<td rowspan='{itemTestRowspans[rowIndex]}' style='vertical-align: middle;'>{expandedRows[rowIndex].ItemTest}</td>");
                                }

                                // Other columns
                                sbDetails.Append($"<td style='vertical-align: middle;'>{expandedRows[rowIndex].Core}</td>");
                                sbDetails.Append($"<td style='vertical-align: middle;'>{expandedRows[rowIndex].SampleType}</td>");
                                sbDetails.Append($"<td style='vertical-align: middle;'>{expandedRows[rowIndex].SpecimenNo}</td>");


                                // Thickness data
                                AddDataColumns(sbDetails, Thickness, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex, maxThicknessInputs);
                                AddAvgColumn(sbDetails, ThicknessAvg, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);

                                // Width data
                                AddDataColumns(sbDetails, Width, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex, maxWidthInputs);
                                AddAvgColumn(sbDetails, WidthAvg, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);

                                // Single value columns
                                AddSingleValueColumn(sbDetails, Area, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, BF, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, TS, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, ElongationL0, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, ElongationL1, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, ElongationEL, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, MedianTS, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, MedianEL, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, VariationTS, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, VariationEL, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);

                                sbDetails.Append("</tr>");
                            }

                            // Add specified row
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td colspan='5' style='background-color:#bebebe; vertical-align: middle;'><strong>Specified</strong></td>");

                            sbDetails.Append($"<td colspan='{maxThicknessInputs + 1}' style='vertical-align: middle;'>{SpecThickness}</td>");
                            sbDetails.Append($"<td colspan='{maxWidthInputs + 1}' style='vertical-align: middle;'>{SpecWidth}</td>");

                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecArea}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecBF}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecTS}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecL0}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecL1}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecEL}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecMedianTS}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecMedianEL}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecVarTS}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecVarEL}</td>");

                            sbDetails.Append("</tr>");
                            sbDetails.Append("</table>");

                            sbDates.Append(@"<table class='tab1'>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Ageing Specimen</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Ageing Complete Cable</td>
                                </tr>
                                <tr>
                                    <td>Airflow: " + AgeingSpecimenAirflow + "</td>" +
                                 @"<td colspan='3'>Standard: 8-20 air change per hour at temperature</td>" +
                                 @"<td>Airflow: " + AgeingCompleteCableAirflow + "</td>" +
                                 @"<td colspan='3'>Standard: 8-20 air change per hour at temperature</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                </tr>
                                <tr>
                                    <td>
                                        " + Convert.ToDateTime(AgeingSpecimenStartDate).ToString("yyyy-MM-dd") +
                                 @"</td>
                                    <td>
                                         " + Convert.ToDateTime(AgeingSpecimenFinishDate).ToString("yyyy-MM-dd") +
                                 @"</td>
                                    <td>
                                         " + Convert.ToDateTime(AgeingSpecimenStartDate).ToString("HH:mm") +
                                 @"</td>
                                    <td>
                                         " + Convert.ToDateTime(AgeingSpecimenFinishDate).ToString("HH:mm") +
                                 @"</td>
                                    <td>
                                        " + Convert.ToDateTime(AgeingCompleteCableStartDate).ToString("yyyy-MM-dd") +
                                 @"</td>
                                    <td> " + Convert.ToDateTime(AgeingCompleteCableFinishDate).ToString("yyyy-MM-dd") +
                                 @"<td>
                                         " + Convert.ToDateTime(AgeingCompleteCableStartDate).ToString("HH:mm") +
                                 @"</td>
                                    <td> " + Convert.ToDateTime(AgeingCompleteCableFinishDate).ToString("HH:mm") +
                                 @"</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Conditioning in desiccators</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Conditioning in desiccators</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                </tr>
                                <tr>
                                    <td>" + Convert.ToDateTime(AgeingConditioningInDesciccatorsStartDate).ToString("yyyy-MM-dd") + "</td>" +
                                @"<td>" + Convert.ToDateTime(AgeingConditioningInDesciccatorsFinishDate).ToString("yyyy-MM-dd") + "</td>" +
                                @"<td> " + Convert.ToDateTime(AgeingConditioningInDesciccatorsStartDate).ToString("HH:mm") + " </td>" +
                                @"<td> " + Convert.ToDateTime(AgeingConditioningInDesciccatorsFinishDate).ToString("HH:mm") + " </td>" +
                                @"<td > " + Convert.ToDateTime(AgeingConditioningStartDate).ToString("yyyy-MM-dd") + " </ td > " +
                                @"<td>" + Convert.ToDateTime(AgeingConditioningFinishDate).ToString("yyyy-MM-dd") + "</td>" +
                                @"<td> " + Convert.ToDateTime(AgeingConditioningStartDate).ToString("HH:mm") + " </td>" +
                                @"<td> " + Convert.ToDateTime(AgeingConditioningFinishDate).ToString("HH:mm") + " </td></tr></table>");

                            sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                     <tr>
                         <td style='font-weight: bold;vertical-align: middle;' rowspan='2' >PT Qualis Indonesia</td>
                         <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
                     </tr>
                     <tr>
                        <td>Name</td><td>Company</td><td>Signature</td>
                     </tr>
                     <tr>
                         <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
                         <td></td><td></td><td></td>
                     </tr>
                 </table>");

                            dt6 = _cm.getTestRequestReview(InquiryID);
                            //Table start.
                            sb.Append("<table border='1' style='border-spacing:0px; width:100%'>");

                            //Adding HeaderRow.
                            sb.Append("<tr>");
                            sb.Append("<td rowspan='2' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Date of Review</b></td>");
                            sb.Append("<td colspan='2' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:20%'><b>Review against</b></td>");
                            sb.Append("<td colspan='9' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:70%'><b>Contract /Test Request Review Checking Points</b></td>");
                            sb.Append("</tr>");
                            sb.Append("<tr>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Sample <br/>Commodity<br/> & No. of<br/> Sample</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Customer<br/> Details</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Equipment<br/> Condition<br/> (OK or not)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Manpower<br/> Availability<br/>(OK or not)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Reagent/Std Availability(OK or not)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Test<br/> Method<br/>/Specification <br/>(OK or not)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:5%'><b>*Sample Quantity <br/>Accepted<br/> (Minimum <br/>Quantity)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>*Physical<br/> Condition of <br/>Sample to be<br/> accepted</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:5%'><b>*Temp <br/>Checking of<br/> sample (if<br/> Required)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Delivery of<br/> Test report <br/>(approx.)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Remarks /<br/> Signature</b></td>");
                            sb.Append("</tr>");

                            for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                            {
                                sb.Append("<tr>");
                                foreach (DataRow dr in dt6.Rows)
                                {
                                    TestRequestReviewID = Convert.ToInt32(dr["TestRequestReviewID"].ToString());
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["reviewDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleName"].ToString() + "," + dr["sampleQty"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + sbCustomer.ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["equipmentCondition"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["manpower"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["reagent"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["testMethod"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleQtyAccepted"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["samplePhyCondn"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleTemp"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["deliveryDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["remarks"].ToString() + "</td>");
                                }
                                sb.Append("</tr>");
                            }
                            //Table end.
                            sb.Append("</table>");

                            string DocumentNo = "";
                            string IssueNo = "";
                            string Revision = "";
                            string title = "";

                            DataTable dtDocument = _cm.getDocumentDetails("Test Request Review Checklist");
                            if (dtDocument.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtDocument.Rows)
                                {
                                    DocumentNo = dr["DocumentNo"].ToString();
                                    IssueNo = dr["IssueNo"].ToString();
                                    Revision = dr["Revision"].ToString();
                                    title = dr["Title"].ToString();
                                }
                            }

                            body = objEmail.PrepareMailBodyWith("TestRecord_TensileStrengthandElongation.html",
                                "{Header}", sbHeader.ToString(),
                                "{TestDetails}", sbDetails.ToString(),
                                "{Dates}", sbDates.ToString(),
                                "{Footer}", sbFooter.ToString(),
                                "{IssueNo}", IssueNo,
                                "{Revision}", Revision,
                                "{Title}", title,
                                "{Details}", sb.ToString());

                            str = body.ToString();
                            filename = Server.MapPath("//TestRecord/TestRecord_TensileStrengthandElongation-" + SampleID + ".html");
                            System.IO.File.WriteAllText(@filename, str);
                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_TensileStrengthandElongation-" + SampleID + ".html"));
                            divCRFReview.InnerHtml = html;
                            GC.Collect();
                            //Response.Redirect("~/SampleList.aspx");
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}