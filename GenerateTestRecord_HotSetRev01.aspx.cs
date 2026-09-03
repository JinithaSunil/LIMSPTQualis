using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
    public partial class GenerateTestRecord_HotSetRev01 : System.Web.UI.Page
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
                        Dictionary<string, List<string>> Material = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Core = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SampleType = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecimenNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThicknessAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Width = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WidthAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Area = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weightload = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Initiallength = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterLength = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterMedian = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AftercoolingLength = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AftercoolingEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AftercoolingMedian = new Dictionary<string, List<string>>();

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


                        // Specified values
                        string SpecThickness = "";
                        string SpecWidth = "";
                        string SpecWeightload = "";
                        string SpecArea = "";
                        string SpecInitiallength = "";
                        string SpecAfter15Length = "";
                        string SpecAfter15EL = "";
                        string SpecAfter15Median = "";
                        string SpecAftercoolinglength = "";
                        string SpecAftercoolingEL = "";
                        string SpecAftercoolingMedian = "";
                        string Aftertemp = "";
                        string underloadatTemp = "";





                        StringBuilder sbDates = new StringBuilder();
                        StringBuilder sbFooter = new StringBuilder();
                        StringBuilder sbCustomer = new StringBuilder();

                        mailer objEmail = new mailer();



                        DataTable dt6 = new DataTable();
                        DataTable dtRecordDetails = new DataTable();
                        StringBuilder sbHeader = new StringBuilder();
                        StringBuilder sbDetails = new StringBuilder();
                        StringBuilder sb = new StringBuilder();

                        void AddDataColumns(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
          string sampleKey1, int materialIndex, int coreGroupIndex, int specimenIndex, int maxColumns)
                        {
                            List<string> values = new List<string>();

                            // Check if we have data for the given sampleKey and material index
                            if (dataDict.ContainsKey(sampleKey1) && materialIndex < dataDict[sampleKey1].Count)
                            {
                                string materialGroup = dataDict[sampleKey1][materialIndex];

                                // Split by semicolon to get core groups
                                string[] coreGroups = materialGroup.Split(';');

                                if (coreGroupIndex < coreGroups.Length)
                                {
                                    string coreGroup = coreGroups[coreGroupIndex];

                                    // Extract parenthetical groups from this core group
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
                                            values = specimenData.Split(',').Select(v => v.Trim()).ToList();
                                        }
                                    }
                                }
                            }

                            // Render the values with padding if fewer than maxColumns
                            for (int i = 0; i < maxColumns; i++)
                            {
                                string value = i < values.Count ? values[i] : "-";
                                sb1.Append($"<td style='vertical-align: middle;'>{value}</td>");
                            }
                        }

                        void AddAvgColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
          string sampleKey1, int materialIndex, int coreGroupIndex, int specimenIndex)
                        {
                            string displayValue = "-";
                            string avgValue = "-";
                            string minValue = "-";
                            string maxValue = "-";

                            if (dataDict.ContainsKey(sampleKey1) && materialIndex < dataDict[sampleKey1].Count)
                            {
                                string materialGroup = dataDict[sampleKey1][materialIndex];
                                string[] coreGroups = materialGroup.Split(';');

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
                                            // Parse the format: "Min: 1.00   Max: 6.00   Avg: 3.50"
                                            // Split into key-value pairs first
                                            string[] pairs = specimenData.Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries);

                                            foreach (string pair in pairs)
                                            {
                                                string[] parts = pair.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                                if (parts.Length >= 2)
                                                {
                                                    string key = parts[0].Trim();
                                                    string value = parts[1].Trim();

                                                    if (key == "Min:")
                                                        minValue = value;
                                                    else if (key == "Max:")
                                                        maxValue = value;
                                                    else if (key == "Avg:")
                                                        avgValue = value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // Display Min, Max, and Avg in the cell
                            if (avgValue != "-")
                            {
                                displayValue = $"Min: {minValue}<br/>Max: {maxValue}<br/>Avg: {avgValue}";
                            }

                            sb1.Append($"<td style='vertical-align: middle;'>{displayValue}</td>");
                        }
                        void AddSingleValueColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
    string sampleKey1, int materialIndex, int coreGroupIndex, int specimenIndex)
                        {
                            string value = "-";

                            if (dataDict.ContainsKey(sampleKey1) && materialIndex < dataDict[sampleKey1].Count)
                            {
                                string materialGroup = dataDict[sampleKey1][materialIndex];
                                string[] coreGroups = materialGroup.Split(';');

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
                        int GetMaxInputsInDataSet(Dictionary<string, List<string>> dataSet)
                        {
                            int maxInputs = 0;
                            foreach (var sampleKey1 in dataSet.Keys)
                            {
                                foreach (var materialGroup in dataSet[sampleKey1])
                                {
                                    // Split by semicolon to get core groups
                                    string[] coreGroups = materialGroup.Split(';');
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
                        void AddMedianColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
    string sampleKey1, int materialIndex, int coreGroupIndex, int specimenIndex, int rowspan)
                        {
                            string value = "-";

                            if (dataDict.ContainsKey(sampleKey1) && materialIndex < dataDict[sampleKey1].Count)
                            {
                                string materialGroup = dataDict[sampleKey1][materialIndex];
                                string[] coreGroups = materialGroup.Split(';');

                                if (coreGroupIndex < coreGroups.Length)
                                {
                                    string coreGroup = coreGroups[coreGroupIndex];

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

                            sb1.Append($"<td rowspan='{rowspan}' style='vertical-align: middle;'>{value}</td>");
                        }

                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        string sampleKey = "1";

                        Material[sampleKey] = new List<string>();
                        Core[sampleKey] = new List<string>();
                        SampleType[sampleKey] = new List<string>();
                        SpecimenNo[sampleKey] = new List<string>();
                        Thickness[sampleKey] = new List<string>();
                        ThicknessAvg[sampleKey] = new List<string>();
                        Width[sampleKey] = new List<string>();
                        WidthAvg[sampleKey] = new List<string>();
                        Area[sampleKey] = new List<string>();
                        Weightload[sampleKey] = new List<string>();
                        Initiallength[sampleKey] = new List<string>();
                        AfterLength[sampleKey] = new List<string>();
                        AfterEL[sampleKey] = new List<string>();
                        AfterMedian[sampleKey] = new List<string>();
                        AftercoolingLength[sampleKey] = new List<string>();
                        AftercoolingEL[sampleKey] = new List<string>();
                        AftercoolingMedian[sampleKey] = new List<string>();

                        if (dtRecordDetails.Rows.Count > 0)
                        {


                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();

                                // Process hierarchical data (pipe-separated date groups)
                                switch (reportParamMapID)
                                {
                                    case "17663": // 
                                        Material[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17662": // Core
                                        Core[sampleKey].AddRange(result.Split('|'));
                                        break;

                                    case "17661": // Sample Type
                                        SampleType[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17677": // SpecimenNo
                                        SpecimenNo[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17678": // Thickness
                                        Thickness[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17679": // ThicknessAvg
                                        ThicknessAvg[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17680": // Width
                                        Width[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17681": // WidthAvg
                                        WidthAvg[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17652": // Area
                                        Area[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17653": // Weightload
                                        Weightload[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17654": // Initial length
                                        Initiallength[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17655": // After15minLength
                                        AfterLength[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17656": // After15minEL
                                        AfterEL[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17657": // After15minMedian
                                        AfterMedian[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17658": // AftercoolingLength
                                        AftercoolingLength[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17659": // AftercoolingEL
                                        AftercoolingEL[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17660": // AftercoolingMedian
                                        AftercoolingMedian[sampleKey].AddRange(result.Split('|'));
                                        break;

                                }
                                // Single value parameters - using new ReportParamMapID
                                if (dr["ReportParamMapID"].ToString() == "17640") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17641") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17642") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17643") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17644") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17645") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17646") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17647") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17648") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17649") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17650") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17651") { EquipmentNo = dr["Result"].ToString(); }

                                // Specified values
                                // Loading 'Specified values' from data rows, using new ReportParamMapIDs
                                if (dr["ReportParamMapID"].ToString() == "17664") { SpecThickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17665") { SpecWidth = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17666") { SpecArea = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17667") { SpecWeightload = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17668") { SpecInitiallength = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17669") { SpecAfter15Length = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17670") { SpecAfter15EL = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17671") { SpecAfter15Median = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17672") { SpecAftercoolinglength = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17673") { SpecAftercoolingEL = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17674") { SpecAftercoolingMedian = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "17675") { Aftertemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17676") { underloadatTemp = dr["Result"].ToString(); }


                            }
                        }


                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px; ' class='tab1'>
                             <tr>
                                 <td>Type</td>
                                 <td>" + Type + "</td>" +
                          @" <td>Ref Spec.</td>
                                 <td>" + RefSpec + "</td>" +
                          @"</tr>
                             <tr>
                                 <td> Size</td>
                                 <td>" + Size + "</td>" +
                          @"<td>Test Method No.</td>
                                <td>" + TestMethodNo + "</td>" +
                          @" </tr>
                             <tr>
                                 <td>Voltage</td>
                                <td>" + Voltage + "</td>" +
                          @"<td>Job Order No.</td>
                                <td>" + JobOrderNo + "</td>" +
                          @"</tr>
                             <tr>
                                 <td>Date</td>
                               <td>" + Date + "</td>" +
                          @"<td>Work Sheet No.</td>
                                <td>" + WorkSheetNo + "</td>" +
                          @"</tr>
                             <tr>
                                 <td>Room No.</td>
                                <td>" + RoomNo + "</td>" +
                          @"<td>Room Humidity</td>
                                 <td>" + RoomHumidity + "%" + "</td>" +
                          @"</tr>
                             <tr>
                                 <td>Room Temp.</td>
                                 <td> " + RoomTemp + "°C" + "</td>" +
                          @" <td>Equipment No.</td>
                                <td>" + EquipmentNo + "</td>" +
                          @"</tr>
                         </table>");


                        // Find maximum number of thickness and width measurements
                        int maxThicknessInputs = GetMaxInputsInDataSet(Thickness);
                        int maxWidthInputs = GetMaxInputsInDataSet(Width);

                        // Create expanded rows for the table
                        List<dynamic> expandedRows = new List<dynamic>();

                        // Process each material group
                        for (int materialIndex = 0; materialIndex < Material[sampleKey].Count; materialIndex++)
                        {
                            string currentMaterial = Material[sampleKey][materialIndex];
                            string coreGroup = materialIndex < Core[sampleKey].Count ? Core[sampleKey][materialIndex] : "";
                            string sampleGroup = materialIndex < SampleType[sampleKey].Count ? SampleType[sampleKey][materialIndex] : "";
                            string specimenGroup = materialIndex < SpecimenNo[sampleKey].Count ? SpecimenNo[sampleKey][materialIndex] : "";

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
                                        Material = currentMaterial,
                                        Core = core,
                                        SampleType = sample,
                                        SpecimenNo = specimen,
                                        MaterialIndex = materialIndex,
                                        CoreGroupIndex = coreGroupIndex,
                                        SpecimenIndex = specimenIndex,
                                        GlobalRowIndex = expandedRows.Count
                                    });
                                }
                            }
                        }

                        if (expandedRows.Count > 0)
                        {
                            // Rowspan logic for Material, Core, and SampleType
                            Dictionary<int, int> materialRowspans = new Dictionary<int, int>();
                            Dictionary<int, int> coreRowspans = new Dictionary<int, int>();
                            Dictionary<int, int> sampleTypeRowspans = new Dictionary<int, int>();

                            Dictionary<int, bool> materialFirstOccurrence = new Dictionary<int, bool>();
                            Dictionary<int, bool> coreFirstOccurrence = new Dictionary<int, bool>();
                            Dictionary<int, bool> sampleTypeFirstOccurrence = new Dictionary<int, bool>();


                            for (int i = 0; i < expandedRows.Count; i++)
                            {
                                // Material rowspan calculation
                                if (i == 0 || expandedRows[i].Material != expandedRows[i - 1].Material)
                                {
                                    materialFirstOccurrence[i] = true;
                                    int rowspan = 1;

                                    for (int j = i + 1; j < expandedRows.Count; j++)
                                    {
                                        if (expandedRows[j].Material == expandedRows[i].Material)
                                        {
                                            rowspan++;
                                        }
                                        else break;
                                    }

                                    materialRowspans[i] = rowspan;
                                }
                                else
                                {
                                    materialFirstOccurrence[i] = false;
                                }

                                // Core rowspan calculation - only if Material and SampleType are the same
                                if (i == 0 ||
                                    expandedRows[i].Material != expandedRows[i - 1].Material ||
                                    expandedRows[i].SampleType != expandedRows[i - 1].SampleType ||
                                    expandedRows[i].Core != expandedRows[i - 1].Core)
                                {
                                    coreFirstOccurrence[i] = true;
                                    int rowspan = 1;

                                    for (int j = i + 1; j < expandedRows.Count; j++)
                                    {
                                        if (expandedRows[j].Material == expandedRows[i].Material &&
                                            expandedRows[j].SampleType == expandedRows[i].SampleType &&
                                            expandedRows[j].Core == expandedRows[i].Core)
                                        {
                                            rowspan++;
                                        }
                                        else break;
                                    }

                                    coreRowspans[i] = rowspan;
                                }
                                else
                                {
                                    coreFirstOccurrence[i] = false;
                                }

                                // SampleType rowspan calculation - only if Material is the same
                                if (i == 0 ||
                                    expandedRows[i].Material != expandedRows[i - 1].Material ||
                                    expandedRows[i].SampleType != expandedRows[i - 1].SampleType)
                                {
                                    sampleTypeFirstOccurrence[i] = true;
                                    int rowspan = 1;

                                    for (int j = i + 1; j < expandedRows.Count; j++)
                                    {
                                        if (expandedRows[j].Material == expandedRows[i].Material &&
                                            expandedRows[j].SampleType == expandedRows[i].SampleType)
                                        {
                                            rowspan++;
                                        }
                                        else break;
                                    }

                                    sampleTypeRowspans[i] = rowspan;
                                }
                                else
                                {
                                    sampleTypeFirstOccurrence[i] = false;
                                }
                            }
                            Dictionary<int, int> medianRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> medianFirstOccurrence = new Dictionary<int, bool>();

                            for (int i = 0; i < expandedRows.Count; i++)
                            {
                                if (i == 0 ||
                                    expandedRows[i].Material != expandedRows[i - 1].Material ||
                                    expandedRows[i].SampleType != expandedRows[i - 1].SampleType)
                                {
                                    medianFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count; j++)
                                    {
                                        if (expandedRows[j].Material == expandedRows[i].Material &&
                                            expandedRows[j].SampleType == expandedRows[i].SampleType)
                                            rowspan++;
                                        else break;
                                    }
                                    medianRowspans[i] = rowspan;
                                }
                                else
                                {
                                    medianFirstOccurrence[i] = false;
                                }
                            }

                            // Build table header (same as before)
                            sbDetails.Append($@"
<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse; margin-top: 10px;'>
<tr>
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Material</strong></td>
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Core Phase</strong></td>
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Type of sample test</strong></td>
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>Specimen No.</strong></td>
    <td colspan='{maxThicknessInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>Thickness/Weight</strong></td>
    <td colspan='{maxWidthInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>Width/Diameter/Length/Density</strong></td>
    <td rowspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>Area</strong></td>
    <td rowspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>Weight load</strong></td>
    <td rowspan='2' style='background-color:#bebebe; vertical-align: middle;'><strong>Initial length</strong></td>
    <td colspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>After {Aftertemp} under load at temperature{underloadatTemp}°C</strong></td>
    <td colspan='3' style='background-color:#bebebe; vertical-align: middle;'><strong>After cooling</strong></td>
</tr>
<tr>");

                            for (int i = 1; i <= maxThicknessInputs; i++)
                                sbDetails.Append($"<td style='background-color:#bebebe; vertical-align: middle;'><strong>{i}</strong></td>");
                            sbDetails.Append("<td style='background-color:#bebebe; vertical-align: middle;'><strong>Avg. min.</strong></td>");

                            for (int i = 1; i <= maxWidthInputs; i++)
                                sbDetails.Append($"<td style='background-color:#bebebe; vertical-align: middle;'><strong>{i}</strong></td>");
                            sbDetails.Append("<td style='background-color:#bebebe; vertical-align: middle;'><strong>Avg. min.</strong></td>");

                            sbDetails.Append($@"
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>Length</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>EL</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>Median</strong></td>
    
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>Length</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>EL</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>Median.</strong></td>
</tr>
<tr>
    <td colspan='{maxThicknessInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
    <td colspan='{maxWidthInputs + 1}' style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm²</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>gr</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>mm</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
    <td style='background-color:#bebebe; vertical-align: middle;'><strong>%</strong></td>
</tr>");

                            // Generate table body
                            for (int rowIndex = 0; rowIndex < expandedRows.Count; rowIndex++)
                            {
                                sbDetails.Append("<tr>");

                                // Material column with rowspan
                                if (materialFirstOccurrence[rowIndex])
                                {
                                    sbDetails.Append($"<td rowspan='{materialRowspans[rowIndex]}' style='vertical-align: middle;'>{expandedRows[rowIndex].Material}</td>");
                                }

                                // Core column with rowspan (only when Material and SampleType are the same)
                                if (coreFirstOccurrence[rowIndex])
                                {
                                    sbDetails.Append($"<td rowspan='{coreRowspans[rowIndex]}' style='vertical-align: middle;'>{expandedRows[rowIndex].Core}</td>");
                                }

                                // SampleType column with rowspan (only when Material is the same)
                                if (sampleTypeFirstOccurrence[rowIndex])
                                {
                                    sbDetails.Append($"<td rowspan='{sampleTypeRowspans[rowIndex]}' style='vertical-align: middle;'>{expandedRows[rowIndex].SampleType}</td>");
                                }

                                // SpecimenNo column without rowspan
                                sbDetails.Append($"<td style='vertical-align: middle;'>{expandedRows[rowIndex].SpecimenNo}</td>");

                                // Data input cells
                                AddDataColumns(sbDetails, Thickness, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex, maxThicknessInputs);
                                AddAvgColumn(sbDetails, ThicknessAvg, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);

                                AddDataColumns(sbDetails, Width, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex, maxWidthInputs);
                                AddAvgColumn(sbDetails, WidthAvg, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);

                                AddSingleValueColumn(sbDetails, Area, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, Weightload, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, Initiallength, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, AfterLength, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, AfterEL, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                if (medianFirstOccurrence[rowIndex])
                                {
                                    AddMedianColumn(sbDetails, AfterMedian, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex, medianRowspans[rowIndex]);
                                }

                             
                                AddSingleValueColumn(sbDetails, AftercoolingLength, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                AddSingleValueColumn(sbDetails, AftercoolingEL, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex);
                                if (medianFirstOccurrence[rowIndex])
                                {
                                    AddMedianColumn(sbDetails, AftercoolingMedian, sampleKey, expandedRows[rowIndex].MaterialIndex, expandedRows[rowIndex].CoreGroupIndex, expandedRows[rowIndex].SpecimenIndex, medianRowspans[rowIndex]);
                                }

                                sbDetails.Append("</tr>");
                            }

                            // Add specified row
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td colspan='4' style='background-color:#bebebe; vertical-align: middle;'><strong>Specified</strong></td>");

                            sbDetails.Append($"<td colspan='{maxThicknessInputs + 1}' style='vertical-align: middle;'>{SpecThickness}</td>");
                            sbDetails.Append($"<td colspan='{maxWidthInputs + 1}' style='vertical-align: middle;'>{SpecWidth}</td>");

                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecArea}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecWeightload}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecInitiallength}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecAfter15Length}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecAfter15EL}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecAfter15Median}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecAftercoolinglength}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecAftercoolingEL}</td>");
                            sbDetails.Append($"<td style='vertical-align: middle;'>{SpecAftercoolingMedian}</td>");

                            sbDetails.Append("</tr>");
                            sbDetails.Append("</table>");

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

                            body = objEmail.PrepareMailBodyWith("TestRecord_hotsetRev01.html",
                                "{Header}", sbHeader.ToString(),
                                "{TestDetails}", sbDetails.ToString(),
                                "{Dates}", sbDates.ToString(),
                                "{Footer}", sbFooter.ToString(),
                                "{IssueNo}", IssueNo,
                                "{Revision}", Revision,
                                "{Title}", title,
                                "{Details}", sb.ToString());

                            str = body.ToString();
                            filename = Server.MapPath("//TestRecord/TestRecord_hotsetRev01-" + SampleID + ".html");
                            System.IO.File.WriteAllText(@filename, str);
                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_hotsetRev01-" + SampleID + ".html"));
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