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
    public partial class GenerateTestRecord_DimensionLowVoltageCable : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0; int CloneCRFSampleID = 0; int SampleID = 0; int ReportId = 0;
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
       

                        Dictionary<string, List<string>> ItemType = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ItemsubType = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MinMaxAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Core = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Inputs = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Specification = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> numberofsample = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Units = new Dictionary<string, List<string>>();



                        string TestDate = "";
                        string RoomNo = "";
                        string RoomTemp = "";
                        string RefSpec = "";
                        string TestMethodNo = "";
                        string JobOrderNo = "";
                        string WorkSheetNo = "";
                        string RoomHumidity = "";
                        string EquipmentNo = "";
                        string Size = "";
                        string Voltage = "";
                        string Date = "";


                        DataTable dt6 = new DataTable();
                        DataTable dtRecordDetails = new DataTable();
                        StringBuilder sbHeader = new StringBuilder();
                        StringBuilder sbDetails = new StringBuilder();
                        StringBuilder sbDates = new StringBuilder();
                        StringBuilder sbFooter = new StringBuilder();
                        StringBuilder sbCustomer = new StringBuilder();
                        StringBuilder sb = new StringBuilder();
                        mailer objEmail = new mailer();
                        StringReader sr = new StringReader(sb.ToString());

                        string body = ""; string str = ""; string filename = ""; string html = "";
                        int TestRequestReviewID = 0;
                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);

                        // Sample DataRow (replace this with actual data from dtRecordDetails)
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string[] sampleParts = result.Split('|'); // each sample

                                for (int sampleIndex = 0; sampleIndex < sampleParts.Length; sampleIndex++)
                                {
                                    string sampleNo = (sampleIndex + 1).ToString();

                                    // Initialize dictionaries for this sample if not exists
                                    if (!numberofsample.ContainsKey(sampleNo))
                                    {
                                        numberofsample[sampleNo] = new List<string>();
                                        ItemType[sampleNo] = new List<string>();
                                        ItemsubType[sampleNo] = new List<string>();
                                        Core[sampleNo] = new List<string>();
                                        Units[sampleNo] = new List<string>();
                                        Inputs[sampleNo] = new List<string>();
                                        MinMaxAvg[sampleNo] = new List<string>();
                                        Specification[sampleNo] = new List<string>();
                                    }

                                    string part = sampleParts[sampleIndex];

                                    if (reportParamMapID == "17203") // Sample Number
                                    {
                                        numberofsample[sampleNo].Add(part.Trim());
                                    }
                                    else if (reportParamMapID == "17188") // Item Type
                                    {
                                        // Split by semicolon to get each item type
                                        string[] itemTypes = part.Split(';');
                                        foreach (var item in itemTypes)
                                        {
                                            if (!string.IsNullOrWhiteSpace(item))
                                            {
                                                ItemType[sampleNo].Add(item.Trim());
                                            }
                                        }
                                    }
                                    else if (reportParamMapID == "17216") // Item Subtype
                                    {
                                        // Split by semicolon first, then add each group as separate entries
                                        string[] subTypeGroups = part.Split(';');
                                        foreach (var group in subTypeGroups)
                                        {
                                            if (!string.IsNullOrWhiteSpace(group))
                                            {
                                                // Add the entire group as one entry 
                                                ItemsubType[sampleNo].Add(group.Trim());
                                            }
                                        }
                                    }
                                    else if (reportParamMapID == "17201") // Core
                                    {
                                       
                                        // Split by semicolon to get core groups for each item type
                                        string[] coreGroups = part.Split(';');
                                        foreach (var group in coreGroups)
                                        {
                                            if (!string.IsNullOrWhiteSpace(group))
                                            {
                                                // Keep the original grouped format
                                                Core[sampleNo].Add(group.Trim());
                                            }
                                        }
                                    }
                                    else if (reportParamMapID == "17202") // Units  
                                    {
                                        // KEEP THE GROUPED STRUCTURE - don't flatten!
                                        // Split by semicolon to get unit groups for each item type
                                        string[] unitGroups = part.Split(';');
                                        foreach (var group in unitGroups)
                                        {
                                            if (!string.IsNullOrWhiteSpace(group))
                                            {
                                                // Keep the original grouped format
                                                Units[sampleNo].Add(group.Trim());
                                            }
                                        }
                                    }
                                    else if (reportParamMapID == "17217") // Inputs
                                    {
                                        // Split by semicolon first to get input groups for each subtype
                                        string[] inputGroups = part.Split(';');
                                        foreach (var group in inputGroups)
                                        {
                                            if (!string.IsNullOrWhiteSpace(group))
                                            {
                                                // Split by ),( to get individual input sets within this subtype group
                                                string[] individualInputs = group.Split(new string[] { "),(" }, StringSplitOptions.None);
                                                for (int i = 0; i < individualInputs.Length; i++)
                                                {
                                                    string input = individualInputs[i].Trim();
                                                    // Clean up the parentheses
                                                    if (i == 0 && input.StartsWith("("))
                                                        input = input.Substring(1);
                                                    if (i == individualInputs.Length - 1 && input.EndsWith(")"))
                                                        input = input.Substring(0, input.Length - 1);

                                                    if (!string.IsNullOrWhiteSpace(input))
                                                    {
                                                        Inputs[sampleNo].Add("(" + input + ")");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (reportParamMapID == "17218") // Min/Max/Avg
                                    {
                                        // Split by semicolon first to get stat groups for each subtype
                                        string[] statsGroups = part.Split(';');
                                        foreach (var group in statsGroups)
                                        {
                                            if (!string.IsNullOrWhiteSpace(group))
                                            {
                                                // Split by ),( to get individual stat sets within this subtype group
                                                string[] individualStats = group.Split(new string[] { "),(" }, StringSplitOptions.None);
                                                for (int i = 0; i < individualStats.Length; i++)
                                                {
                                                    string stat = individualStats[i].Trim();
                                                    // Clean up the parentheses
                                                    if (i == 0 && stat.StartsWith("("))
                                                        stat = stat.Substring(1);
                                                    if (i == individualStats.Length - 1 && stat.EndsWith(")"))
                                                        stat = stat.Substring(0, stat.Length - 1);

                                                    if (!string.IsNullOrWhiteSpace(stat))
                                                    {
                                                        // DON'T add parentheses back - keep the original content
                                                        MinMaxAvg[sampleNo].Add(stat);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (reportParamMapID == "17319")
                                    {
                                        // Split by semicolon first to get specified groups for each item type
                                        string[] specifiedGroups = part.Split(';');
                                        foreach (var group in specifiedGroups)
                                        {
                                            if (!string.IsNullOrWhiteSpace(group))
                                            {
                                                // Add the entire group as one entry to maintain structure
                                                Specification[sampleNo].Add(group.Trim());
                                            }
                                        }
                                    }
                                }
                                 // Single value parameters (not sample-specific)
                            if (dr["ReportParamMapID"].ToString() == "17200") { Type = dr["Result"].ToString(); }            // cableType
                                if (dr["ReportParamMapID"].ToString() == "17194") { RefSpec = dr["Result"].ToString(); }         // refSpec
                                if (dr["ReportParamMapID"].ToString() == "17189") { Size = dr["Result"].ToString(); }            // cableSize
                                if (dr["ReportParamMapID"].ToString() == "17195") { TestMethodNo = dr["Result"].ToString(); }    // testMethodNo
                                if (dr["ReportParamMapID"].ToString() == "17190") { Voltage = dr["Result"].ToString(); }         // voltage
                                if (dr["ReportParamMapID"].ToString() == "17196") { JobOrderNo = dr["Result"].ToString(); }      // jobOrderNo
                                if (dr["ReportParamMapID"].ToString() == "17197") { WorkSheetNo = dr["Result"].ToString(); }     // worksheetno
                                if (dr["ReportParamMapID"].ToString() == "17199") { EquipmentNo = dr["Result"].ToString(); }     // equipmentno
                                if (dr["ReportParamMapID"].ToString() == "17192") { RoomNo = dr["Result"].ToString(); }          // roomno
                                if (dr["ReportParamMapID"].ToString() == "17198") { RoomHumidity = dr["Result"].ToString(); }    // roomhumidity
                                if (dr["ReportParamMapID"].ToString() == "17193") { RoomTemp = dr["Result"].ToString(); }        // roomtemp
                                if (dr["ReportParamMapID"].ToString() == "17191") { Date = dr["Result"].ToString(); }            // date
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

                        int numberOfSamples = numberofsample.Keys.Count;

                        // Find the maximum number of inputs in any single input set
                        int maxInputsInAnySet = 0;
                        foreach (var sampleKey in Inputs.Keys)
                        {
                            foreach (var inputGroup in Inputs[sampleKey])
                            {
                                if (!string.IsNullOrWhiteSpace(inputGroup))
                                {
                                    // Parse multiple parentheses groups: (1,2,3,4,5,6),(7,8,9,10,11,12)
                                    List<string> allInputValues = new List<string>();
                                    int pos = 0;
                                    while (pos < inputGroup.Length)
                                    {
                                        int start = inputGroup.IndexOf('(', pos);
                                        if (start == -1) break;
                                        int end = inputGroup.IndexOf(')', start);
                                        if (end == -1) break;

                                        string content = inputGroup.Substring(start + 1, end - start - 1);
                                        if (!string.IsNullOrWhiteSpace(content))
                                        {
                                            string[] values = content.Split(',');
                                            foreach (var value in values)
                                            {
                                                string trimmedValue = value.Trim();
                                                if (!string.IsNullOrWhiteSpace(trimmedValue))
                                                {
                                                    allInputValues.Add(trimmedValue);
                                                }
                                            }
                                        }
                                        pos = end + 1;
                                    }

                                    maxInputsInAnySet = Math.Max(maxInputsInAnySet, allInputValues.Count);
                                }
                            }
                        }

                        List<dynamic> expandedRows = new List<dynamic>();
                        string firstSampleKey = numberofsample.Keys.OrderBy(k => int.Parse(k)).First();

                        // Process each item type and expand its subtypes into separate rows
                        if (ItemType.ContainsKey(firstSampleKey) && ItemsubType.ContainsKey(firstSampleKey))
                        {
                            int globalRowIndex = 0;

                            for (int itemTypeIndex = 0; itemTypeIndex < ItemType[firstSampleKey].Count; itemTypeIndex++)
                            {
                                string currentItemType = ItemType[firstSampleKey][itemTypeIndex];

                                // Get the subtype group for this item type
                                string subTypeGroup = ItemsubType[firstSampleKey][itemTypeIndex];

                                // Get the specified group for this item type
                                string specifiedGroup = "";
                                if (Specification.ContainsKey(firstSampleKey) && itemTypeIndex < Specification[firstSampleKey].Count)
                                {
                                    specifiedGroup = Specification[firstSampleKey][itemTypeIndex];
                                }

                                // Split subtypes by comma to get individual subtypes
                                string[] individualSubTypes = subTypeGroup.Split(',');
                                string[] individualSpecified = specifiedGroup.Split(',');

                                // Get the core group for this item type (NOW GROUPED FORMAT)
                                string coreGroup = "";
                                if (Core.ContainsKey(firstSampleKey) && itemTypeIndex < Core[firstSampleKey].Count)
                                {
                                    coreGroup = Core[firstSampleKey][itemTypeIndex];
                                }

                                // Parse cores - they are in format like "(Red,Yellow,Blue,Sheath),(Red,Yellow,Blue,Sheath),..."
                                List<List<string>> coresBySubType = new List<List<string>>();
                                if (!string.IsNullOrWhiteSpace(coreGroup))
                                {
                                    int pos = 0;
                                    while (pos < coreGroup.Length)
                                    {
                                        int start = coreGroup.IndexOf('(', pos);
                                        if (start == -1) break;
                                        int end = coreGroup.IndexOf(')', start);
                                        if (end == -1) break;

                                        string content = coreGroup.Substring(start + 1, end - start - 1);
                                        List<string> coresForThisSubType = new List<string>();
                                        if (!string.IsNullOrWhiteSpace(content))
                                        {
                                            string[] phases = content.Split(',');
                                            foreach (var phase in phases)
                                            {
                                                string trimmedPhase = phase.Trim();
                                                if (!string.IsNullOrWhiteSpace(trimmedPhase))
                                                {
                                                    coresForThisSubType.Add(trimmedPhase);
                                                }
                                            }
                                        }
                                        coresBySubType.Add(coresForThisSubType);
                                        pos = end + 1;
                                    }
                                }

                                // Get the unit group for this item type (NOW GROUPED FORMAT)
                                string unitGroup = "";
                                if (Units.ContainsKey(firstSampleKey) && itemTypeIndex < Units[firstSampleKey].Count)
                                {
                                    unitGroup = Units[firstSampleKey][itemTypeIndex];
                                }

                                // Parse units - they are in format like "(mm),(mm),(mm);(mm)"
                                List<string> unitsBySubType = new List<string>();
                                if (!string.IsNullOrWhiteSpace(unitGroup))
                                {
                                    int pos = 0;
                                    while (pos < unitGroup.Length)
                                    {
                                        int start = unitGroup.IndexOf('(', pos);
                                        if (start == -1) break;
                                        int end = unitGroup.IndexOf(')', start);
                                        if (end == -1) break;

                                        string content = unitGroup.Substring(start + 1, end - start - 1);
                                        unitsBySubType.Add(content.Trim());
                                        pos = end + 1;
                                    }
                                }

                                // For each subtype, create rows for each phase
                                for (int subTypeIndex = 0; subTypeIndex < individualSubTypes.Length; subTypeIndex++)
                                {
                                    string currentSubType = individualSubTypes[subTypeIndex].Trim();
                                    string currentSpecified = subTypeIndex < individualSpecified.Length ? individualSpecified[subTypeIndex].Trim() : "";
                                    string currentUnit = subTypeIndex < unitsBySubType.Count ? unitsBySubType[subTypeIndex] : "";

                                    // Get cores for this specific subtype
                                    List<string> availableCores = new List<string>();
                                    if (subTypeIndex < coresBySubType.Count)
                                    {
                                        availableCores = coresBySubType[subTypeIndex];
                                    }

                                    // If no cores found, create at least one row
                                    if (availableCores.Count == 0)
                                    {
                                        availableCores.Add(""); // Empty core
                                    }

                                    // Create a row for each available core/phase
                                    foreach (string core in availableCores)
                                    {
                                        expandedRows.Add(new
                                        {
                                            ItemType = currentItemType,
                                            ItemSubType = currentSubType,
                                            Core = core,
                                            Unit = currentUnit,
                                            Specified = currentSpecified,
                                            ItemTypeIndex = itemTypeIndex,
                                            SubTypeIndex = subTypeIndex,
                                            CoreIndex = 0, // Not used in this approach
                                            GlobalRowIndex = globalRowIndex,
                                            InputIndex = globalRowIndex,
                                            StatsIndex = globalRowIndex
                                        });

                                        globalRowIndex++;
                                    }
                                }
                            }
                        }

                        // Calculate rowspans for ItemType and ItemSubType
                        Dictionary<int, int> itemTypeRowspans = new Dictionary<int, int>();
                        Dictionary<int, bool> itemTypeFirstOccurrence = new Dictionary<int, bool>();
                        Dictionary<int, int> itemSubTypeRowspans = new Dictionary<int, int>();
                        Dictionary<int, bool> itemSubTypeFirstOccurrence = new Dictionary<int, bool>();

                        for (int i = 0; i < expandedRows.Count; i++)
                        {
                            // ItemType rowspan calculation
                            if (i == 0 || expandedRows[i].ItemType != expandedRows[i - 1].ItemType)
                            {
                                itemTypeFirstOccurrence[i] = true;
                                int rowspan = 1;
                                for (int j = i + 1; j < expandedRows.Count && expandedRows[j].ItemType == expandedRows[i].ItemType; j++)
                                {
                                    rowspan++;
                                }
                                itemTypeRowspans[i] = rowspan;
                            }
                            else
                            {
                                itemTypeFirstOccurrence[i] = false;
                            }

                            // ItemSubType rowspan calculation
                            if (i == 0 || expandedRows[i].ItemSubType != expandedRows[i - 1].ItemSubType || expandedRows[i].ItemType != expandedRows[i - 1].ItemType)
                            {
                                itemSubTypeFirstOccurrence[i] = true;
                                int rowspan = 1;
                                for (int j = i + 1; j < expandedRows.Count && expandedRows[j].ItemSubType == expandedRows[i].ItemSubType && expandedRows[j].ItemType == expandedRows[i].ItemType; j++)
                                {
                                    rowspan++;
                                }
                                itemSubTypeRowspans[i] = rowspan;
                            }
                            else
                            {
                                itemSubTypeFirstOccurrence[i] = false;
                            }
                        }

                        int totalColumns = numberOfSamples * (maxInputsInAnySet + 1);

                        // Build table header
                        sbDetails.Append($@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>
<tr>
    <td rowspan='3' colspan='2'style='background-color:#bebebe;'>Item Test</td>
    <td rowspan='3' style='background-color:#bebebe;'>Phase</td>
    <td rowspan='3' style='background-color:#bebebe;'>Unit</td>
    <td colspan='{totalColumns}' style='background-color:#bebebe;'>Measured of sample</td>
    <td rowspan='3' style='background-color:#bebebe;'>Specified</td>
</tr>");

                        // Sample number row
                        sbDetails.Append("<tr>");
                        for (int sample = 1; sample <= numberOfSamples; sample++)
                        {
                            sbDetails.Append($"<td colspan='{maxInputsInAnySet + 1}' style='background-color:#bebebe;'>{sample}</td>");
                        }
                        sbDetails.Append("</tr>");

                        // Input number columns
                        sbDetails.Append("<tr>");
                        for (int sample = 1; sample <= numberOfSamples; sample++)
                        {
                            for (int inputNum = 1; inputNum <= maxInputsInAnySet; inputNum++)
                            {
                                sbDetails.Append($"<td style='background-color:#bebebe;'>{inputNum}</td>");
                            }
                            sbDetails.Append("<td style='background-color:#bebebe;'>Min Max Avg</td>");
                        }
                        sbDetails.Append("</tr>");

                        // Generate data rows
                        for (int rowIndex = 0; rowIndex < expandedRows.Count; rowIndex++)
                        {
                            sbDetails.Append("<tr>");

                            // ItemType column with dynamic rowspan - only show on first occurrence
                            if (itemTypeFirstOccurrence[rowIndex])
                            {
                                sbDetails.Append($"<td rowspan='{itemTypeRowspans[rowIndex]}'>{expandedRows[rowIndex].ItemType}</td>");
                            }

                            // ItemSubType (Phase) column - each subtype gets its own row with rowspan
                            if (itemSubTypeFirstOccurrence[rowIndex])
                            {
                                sbDetails.Append($"<td rowspan='{itemSubTypeRowspans[rowIndex]}'>{expandedRows[rowIndex].ItemSubType}</td>");
                            }

                            // Core column - use the core from the expanded row
                            sbDetails.Append($"<td>{expandedRows[rowIndex].Core}</td>");

                            // Unit column - use the unit from the expanded row
                            sbDetails.Append($"<td>{expandedRows[rowIndex].Unit}</td>");

                            // Data columns for each sample
                            for (int sample = 1; sample <= numberOfSamples; sample++)
                            {
                                string sampleKey = sample.ToString();

                                // Get input data - use GlobalRowIndex to map to correct input group
                                if (Inputs.ContainsKey(sampleKey))
                                {
                                    int inputGroupIndex = expandedRows[rowIndex].GlobalRowIndex;
                                    List<string> inputValues = new List<string>();

                                    if (inputGroupIndex < Inputs[sampleKey].Count)
                                    {
                                        string inputData = Inputs[sampleKey][inputGroupIndex];

                                        // Parse multiple parentheses groups: (1,2,3,4,5,6),(7,8,9,10,11,12)
                                        if (!string.IsNullOrWhiteSpace(inputData))
                                        {
                                            int pos = 0;
                                            while (pos < inputData.Length)
                                            {
                                                int start = inputData.IndexOf('(', pos);
                                                if (start == -1) break;
                                                int end = inputData.IndexOf(')', start);
                                                if (end == -1) break;

                                                string content = inputData.Substring(start + 1, end - start - 1);
                                                if (!string.IsNullOrWhiteSpace(content))
                                                {
                                                    string[] values = content.Split(',');
                                                    foreach (var value in values)
                                                    {
                                                        string trimmedValue = value.Trim();
                                                        if (!string.IsNullOrWhiteSpace(trimmedValue))
                                                        {
                                                            inputValues.Add(trimmedValue);
                                                        }
                                                    }
                                                }
                                                pos = end + 1;
                                            }
                                        }
                                    }

                                    // Output input values
                                    for (int inputNum = 0; inputNum < maxInputsInAnySet; inputNum++)
                                    {
                                        string value = inputNum < inputValues.Count ? inputValues[inputNum] : "-";
                                        sbDetails.Append($"<td>{value}</td>");
                                    }
                                }
                                else
                                {
                                    for (int inputNum = 0; inputNum < maxInputsInAnySet; inputNum++)
                                    {
                                        sbDetails.Append("<td>-</td>");
                                    }
                                }

                                // Min Max Avg column
                                string minMaxAvgValue = "-";
                                if (MinMaxAvg.ContainsKey(sampleKey))
                                {
                                    int statsGroupIndex = expandedRows[rowIndex].GlobalRowIndex;

                                    if (statsGroupIndex < MinMaxAvg[sampleKey].Count)
                                    {
                                        string minMaxAvgData = MinMaxAvg[sampleKey][statsGroupIndex];

                                        // Skip if data is null or empty
                                        if (!string.IsNullOrWhiteSpace(minMaxAvgData))
                                        {
                                            // Remove parentheses if they exist
                                            string content = minMaxAvgData;
                                            if (minMaxAvgData.StartsWith("(") && minMaxAvgData.EndsWith(")"))
                                            {
                                                content = minMaxAvgData.Substring(1, minMaxAvgData.Length - 2);
                                            }

                                            // Parse Min, Max, Avg values if they exist
                                            if (content.Contains("Min:"))
                                            {
                                                string minValue = "-";
                                                string maxValue = "-";
                                                string avgValue = "-";

                                                // Extract Min value
                                                int minStart = content.IndexOf("Min:") + 4;
                                                int minEnd = content.IndexOf("Max:", minStart);
                                                if (minEnd > minStart)
                                                {
                                                    minValue = content.Substring(minStart, minEnd - minStart).Trim();
                                                }

                                                // Extract Max value
                                                int maxStart = content.IndexOf("Max:") + 4;
                                                int maxEnd = content.IndexOf("Avg:", maxStart);
                                                if (maxEnd > maxStart)
                                                {
                                                    maxValue = content.Substring(maxStart, maxEnd - maxStart).Trim();
                                                }

                                                // Extract Avg value
                                                int avgStart = content.IndexOf("Avg:") + 4;
                                                avgValue = content.Substring(avgStart).Trim();

                                                minMaxAvgValue = $"Min :{minValue} Max: {maxValue} Avg: {avgValue}";
                                            }
                                            else
                                            {
                                                // If no "Min:" found, use the raw data
                                                minMaxAvgValue = content;
                                            }
                                        }
                                    }
                                }
                                sbDetails.Append($"<td>{minMaxAvgValue}</td>");
                            }

                            // Specified column - Display the specified value for this row
                            sbDetails.Append($"<td>{expandedRows[rowIndex].Specified}</td>");
                            sbDetails.Append("</tr>");
                        }
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
                                 <td></td><td></td>
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
                            {
                                foreach (DataRow dr in dt6.Rows)
                                {
                                    TestRequestReviewID = Convert.ToInt32(dr["TestRequestReviewID"].ToString());
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["reviewDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleName"].ToString() + "," + dr["sampleQty"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + sbCustomer.ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["equipmentCondition"].ToString() + "</ td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["manpower"].ToString() + "</ td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["reagent"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["testMethod"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleQtyAccepted"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["samplePhyCondn"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleTemp"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["deliveryDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["remarks"].ToString() + "</td>");
                                }

                            }
                            sb.Append("</tr>");
                        }
                        //Table end.
                        sb.Append("</table>");
                        string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_DimenisonLowVoltageCable.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_DimenisonLowVoltageCable-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_DimenisonLowVoltageCable-" + SampleID + ".htm"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
                        //Response.Redirect("~/SampleList.aspx");

                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        // Add this helper method to get values safely
        private string GetValueSafely(Dictionary<string, List<string>> data, string key, int index)
        {
            if (data.ContainsKey(key) && index < data[key].Count)
                return data[key][index];
            return "";
        }

    }
}