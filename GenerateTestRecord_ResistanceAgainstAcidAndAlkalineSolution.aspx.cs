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
	public partial class GenerateTestRecord_ResistanceAgainstAcidAndAlkalineSolution : System.Web.UI.Page
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


                        Dictionary<string, List<string>> Number = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TestDate = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ItemTest = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SampleType = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecimenNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThicknessAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Width = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WidthAvg = new Dictionary<string, List<string>>();



                        string Date = "";
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


                        // Fixed AddDataColumns method with better data parsing
                        void AddDataColumns(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
                            string sampleKey1, int dateIndex, int specimenIndex, int maxColumns)
                        {
                            List<string> values = new List<string>();

                            if (dataDict.ContainsKey(sampleKey1) && dateIndex < dataDict[sampleKey1].Count)
                            {
                                string dateGroup = dataDict[sampleKey1][dateIndex];

                                // Split directly by parentheses to get specimen data
                                List<string> specimenGroups = new List<string>();
                                int pos = 0;
                                while (pos < dateGroup.Length)
                                {
                                    int start = dateGroup.IndexOf('(', pos);
                                    if (start == -1) break;
                                    int end = dateGroup.IndexOf(')', start);
                                    if (end == -1) break;

                                    string content = dateGroup.Substring(start + 1, end - start - 1);
                                    specimenGroups.Add(content);
                                    pos = end + 1;
                                }

                                if (specimenIndex < specimenGroups.Count)
                                {
                                    string specimenData = specimenGroups[specimenIndex];
                                    if (!string.IsNullOrWhiteSpace(specimenData))
                                    {
                                        // Check if this looks like measurement data (numeric values)
                                        if (specimenData.Contains(","))
                                        {
                                            values = specimenData.Split(',').Select(v => v.Trim()).ToList();
                                        }
                                        else if (double.TryParse(specimenData.Trim(), out _))
                                        {
                                            // Single numeric value
                                            values.Add(specimenData.Trim());
                                        }
                                    }
                                }
                            }

                            // Print values with padding
                            for (int i = 0; i < maxColumns; i++)
                            {
                                string value = i < values.Count && !string.IsNullOrEmpty(values[i]) ? values[i] : "-";
                                sb1.Append($"<td style='vertical-align: middle; text-align: center;'>{value}</td>");
                            }
                        }

                        void AddAvgColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
                      string sampleKey1, int dateIndex, int specimenIndex)
                        {
                            string displayValue = "-";

                            if (dataDict.ContainsKey(sampleKey1) && dateIndex < dataDict[sampleKey1].Count)
                            {
                                string dateGroup = dataDict[sampleKey1][dateIndex];

                                // Extract all content inside parentheses
                                List<string> specimenGroups = new List<string>();
                                int pos = 0;
                                while (pos < dateGroup.Length)
                                {
                                    int start = dateGroup.IndexOf('(', pos);
                                    if (start == -1) break;
                                    int end = dateGroup.IndexOf(')', start);
                                    if (end == -1) break;

                                    string content = dateGroup.Substring(start + 1, end - start - 1); // Keep full content
                                    specimenGroups.Add(content);
                                    pos = end + 1;
                                }

                                if (specimenIndex < specimenGroups.Count)
                                {
                                    string specimenData = specimenGroups[specimenIndex];
                                    if (!string.IsNullOrWhiteSpace(specimenData))
                                    {
                                        displayValue = specimenData;
                                    }
                                }
                            }

                            sb1.Append($"<td style='vertical-align: middle;'>{displayValue}</td>");
                        }

                        // Modified AddSingleValueColumn method - Direct specimen processing
                        void AddSingleValueColumn(StringBuilder sb1, Dictionary<string, List<string>> dataDict,
                            string sampleKey1, int dateIndex, int specimenIndex)
                        {
                            string value = "-";

                            if (dataDict.ContainsKey(sampleKey1) && dateIndex < dataDict[sampleKey1].Count)
                            {
                                string dateGroup = dataDict[sampleKey1][dateIndex];

                                // Split directly by parentheses to get specimen data
                                List<string> specimenGroups = new List<string>();
                                int pos = 0;
                                while (pos < dateGroup.Length)
                                {
                                    int start = dateGroup.IndexOf('(', pos);
                                    if (start == -1) break;
                                    int end = dateGroup.IndexOf(')', start);
                                    if (end == -1) break;

                                    string content = dateGroup.Substring(start + 1, end - start - 1);
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
                        string sampleKey = "1";

                        Number[sampleKey] = new List<string>();
                        TestDate[sampleKey] = new List<string>();
                        ItemTest[sampleKey] = new List<string>();

                        SampleType[sampleKey] = new List<string>();
                        SpecimenNo[sampleKey] = new List<string>();
                        Thickness[sampleKey] = new List<string>();
                        ThicknessAvg[sampleKey] = new List<string>();
                        Width[sampleKey] = new List<string>();
                        WidthAvg[sampleKey] = new List<string>();



                        string body = ""; string str = ""; string filename = ""; string html = "";
                        int TestRequestReviewID = 0;
                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {


                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();

                                // Process hierarchical data (pipe-separated date groups)
                                switch (reportParamMapID)
                                {
                                    case "17360": // number
                                        Number[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17361": // Date
                                        TestDate[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17362": // ItemTest
                                        ItemTest[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17363": // Sample Type
                                        SampleType[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17587": // SpecimenNo
                                        SpecimenNo[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17364": // Thickness
                                        Thickness[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17365": // ThicknessAvg
                                        ThicknessAvg[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17366": // Width
                                        Width[sampleKey].AddRange(result.Split('|'));
                                        break;
                                    case "17367": // WidthAvg
                                        WidthAvg[sampleKey].AddRange(result.Split('|'));
                                        break;

                                }

                                // Single value parameters
                                if (dr["ReportParamMapID"].ToString() == "17368") { Type = dr["Result"].ToString(); }          // Type
                                if (dr["ReportParamMapID"].ToString() == "17369") { Size = dr["Result"].ToString(); }          // Size
                                if (dr["ReportParamMapID"].ToString() == "17370") { Voltage = dr["Result"].ToString(); }       // Voltage
                                if (dr["ReportParamMapID"].ToString() == "17371") { Date = dr["Result"].ToString(); }          // Date
                                if (dr["ReportParamMapID"].ToString() == "17372") { RoomNo = dr["Result"].ToString(); }        // RoomNo
                                if (dr["ReportParamMapID"].ToString() == "17373") { RoomTemp = dr["Result"].ToString(); }      // RoomTemp
                                if (dr["ReportParamMapID"].ToString() == "17374") { RefSpec = dr["Result"].ToString(); }       // RefSpec
                                if (dr["ReportParamMapID"].ToString() == "17375") { TestMethodNo = dr["Result"].ToString(); }  // TestMethodNo
                                if (dr["ReportParamMapID"].ToString() == "17376") { JobOrderNo = dr["Result"].ToString(); }    // JobOrderNo
                                if (dr["ReportParamMapID"].ToString() == "17377") { WorkSheetNo = dr["Result"].ToString(); }   // WorkSheetNo
                                if (dr["ReportParamMapID"].ToString() == "17378") { RoomHumidity = dr["Result"].ToString(); }  // RoomHumidity
                                if (dr["ReportParamMapID"].ToString() == "17379") { EquipmentNo = dr["Result"].ToString(); }   // EquipmentNo


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


                        // Find maximum number of thickness and diameter measurements
                        int maxThicknessInputs = GetMaxInputsInDataSet(Thickness);
                        int maxDiameterInputs = GetMaxInputsInDataSet(Width);

                        List<dynamic> expandedRows = new List<dynamic>();
                        for (int dateIndex = 0; dateIndex < TestDate[sampleKey].Count; dateIndex++)
                        {
                            string currentDate = TestDate[sampleKey][dateIndex];
                            string currentItemTest = dateIndex < ItemTest[sampleKey].Count ? ItemTest[sampleKey][dateIndex] : "";
                            string itemSubTypeGroup = dateIndex < SampleType[sampleKey].Count ? SampleType[sampleKey][dateIndex] : "";
                            string specimenGroup = dateIndex < SpecimenNo[sampleKey].Count ? SpecimenNo[sampleKey][dateIndex] : "";

                            string numberGroup = dateIndex < Number[sampleKey].Count ? Number[sampleKey][dateIndex] : "";

                            // Split specimens and numbers
                            string[] specimens = specimenGroup.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] itemSubTypes = itemSubTypeGroup.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);


                            string numberValue = numberGroup.Trim();

                            for (int specimenIndex = 0; specimenIndex < specimens.Length; specimenIndex++)
                            {
                                string specimen = specimens[specimenIndex].Trim();
                                string itemSubType = specimenIndex < itemSubTypes.Length ? itemSubTypes[specimenIndex].Trim() :
                                                    (itemSubTypes.Length > 0 ? itemSubTypes[0].Trim() : "");

                                expandedRows.Add(new
                                {
                                    Date = currentDate,
                                    ItemTest = currentItemTest,
                                    ItemSubType = itemSubType,
                                    SpecimenNo = specimen,
                                    Number = numberValue, // FIXED: Use the same number value for all specimens in this date
                                    DateIndex = dateIndex,
                                    SpecimenIndex = specimenIndex,
                                    GlobalRowIndex = expandedRows.Count,
                                    RowNumber = expandedRows.Count + 1
                                });
                            }
                        }

                        // Only proceed if we have expanded rows
                        if (expandedRows.Count > 0)
                        {
                            // Enhanced rowspan calculation
                            Dictionary<int, int> numberRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> numberFirstOccurrence = new Dictionary<int, bool>();
                            Dictionary<int, int> dateRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> dateFirstOccurrence = new Dictionary<int, bool>();
                            Dictionary<int, int> itemTestRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> itemTestFirstOccurrence = new Dictionary<int, bool>();
                            Dictionary<int, int> itemSubTypeRowspans = new Dictionary<int, int>();
                            Dictionary<int, bool> itemSubTypeFirstOccurrence = new Dictionary<int, bool>();

                            for (int i = 0; i < expandedRows.Count; i++)
                            {
                                // FIXED: Number rowspan calculation - group by Date AND Number
                                if (i == 0 ||
                                    expandedRows[i].Date != expandedRows[i - 1].Date ||
                                    expandedRows[i].Number != expandedRows[i - 1].Number)
                                {
                                    numberFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count &&
                                         expandedRows[j].Date == expandedRows[i].Date &&
                                         expandedRows[j].Number == expandedRows[i].Number; j++)
                                    {
                                        rowspan++;
                                    }
                                    numberRowspans[i] = rowspan;
                                }
                                else
                                {
                                    numberFirstOccurrence[i] = false;
                                }

                                // Date rowspan calculation
                                if (i == 0 || expandedRows[i].Date != expandedRows[i - 1].Date)
                                {
                                    dateFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count && expandedRows[j].Date == expandedRows[i].Date; j++)
                                    {
                                        rowspan++;
                                    }
                                    dateRowspans[i] = rowspan;
                                }
                                else
                                {
                                    dateFirstOccurrence[i] = false;
                                }

                                // ItemTest rowspan calculation  
                                if (i == 0 ||
                                    expandedRows[i].ItemTest != expandedRows[i - 1].ItemTest ||
                                    expandedRows[i].Date != expandedRows[i - 1].Date)
                                {
                                    itemTestFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count &&
                                         expandedRows[j].ItemTest == expandedRows[i].ItemTest &&
                                         expandedRows[j].Date == expandedRows[i].Date; j++)
                                    {
                                        rowspan++;
                                    }
                                    itemTestRowspans[i] = rowspan;
                                }
                                else
                                {
                                    itemTestFirstOccurrence[i] = false;
                                }

                                // ItemSubType rowspan calculation
                                if (i == 0 ||
                                    expandedRows[i].ItemSubType != expandedRows[i - 1].ItemSubType ||
                                    expandedRows[i].ItemTest != expandedRows[i - 1].ItemTest ||
                                    expandedRows[i].Date != expandedRows[i - 1].Date)
                                {
                                    itemSubTypeFirstOccurrence[i] = true;
                                    int rowspan = 1;
                                    for (int j = i + 1; j < expandedRows.Count &&
                                         expandedRows[j].ItemSubType == expandedRows[i].ItemSubType &&
                                         expandedRows[j].ItemTest == expandedRows[i].ItemTest &&
                                         expandedRows[j].Date == expandedRows[i].Date; j++)
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


                            // Build main data table header (same as before)
                            sbDetails.Append($@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse; margin-top: 10px;'>
<tr>
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>No.</strong></td>
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Date</strong></td>
    <td rowspan='3' colspan='2' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Item Test</strong></td>
    
    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Specimen</strong></td>
    <td colspan='{maxThicknessInputs + maxDiameterInputs + 2}' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Measurement</strong></td>
</tr>
<tr>
    <td colspan='{maxThicknessInputs + 1}' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Thickness (mm)</strong></td>
    <td colspan='{maxDiameterInputs + 1}' style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Diameter/Width (mm)</strong></td>
</tr>
<tr>");

                            // Thickness measurement columns
                            for (int i = 1; i <= maxThicknessInputs; i++)
                            {
                                sbDetails.Append($"<td style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>{i}</strong></td>");
                            }
                            sbDetails.Append("<td style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Min. Avg.</strong></td>");

                            // Diameter measurement columns
                            for (int i = 1; i <= maxDiameterInputs; i++)
                            {
                                sbDetails.Append($"<td style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>{i}</strong></td>");
                            }
                            sbDetails.Append("<td style='background-color:#bebebe; vertical-align: middle; text-align: center;'><strong>Min. Avg.</strong></td>");

                            sbDetails.Append("</tr>");

                            // Generate data rows with proper rowspan
                            for (int rowIndex = 0; rowIndex < expandedRows.Count; rowIndex++)
                            {
                                sbDetails.Append("<tr>");

                                // Number column with rowspan - ADD THIS
                                if (numberFirstOccurrence[rowIndex])
                                {
                                    string numberValue = string.IsNullOrEmpty(expandedRows[rowIndex].Number) ? "-" : expandedRows[rowIndex].Number;
                                    if (numberRowspans[rowIndex] > 1)
                                    {
                                        sbDetails.Append($"<td rowspan='{numberRowspans[rowIndex]}' style='vertical-align: middle; text-align: center;'>{numberValue}</td>");
                                    }
                                    else
                                    {
                                        sbDetails.Append($"<td style='vertical-align: middle; text-align: center;'>{numberValue}</td>");
                                    }
                                }

                                // Date column with rowspan
                                if (dateFirstOccurrence[rowIndex])
                                {
                                    string dateValue = "-";

                                    if (!string.IsNullOrEmpty(expandedRows[rowIndex].Date))
                                    {
                                        // Try to parse and format the date
                                        if (DateTime.TryParse(expandedRows[rowIndex].Date, out DateTime parsedDate))
                                        {
                                            dateValue = parsedDate.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            // If parsing fails, use the original value
                                            dateValue = expandedRows[rowIndex].Date;
                                        }
                                    }

                                    if (dateRowspans[rowIndex] > 1)
                                    {
                                        sbDetails.Append($"<td rowspan='{dateRowspans[rowIndex]}' style='vertical-align: middle; text-align: center;'>{dateValue}</td>");
                                    }
                                    else
                                    {
                                        sbDetails.Append($"<td style='vertical-align: middle; text-align: center;'>{dateValue}</td>");
                                    }
                                }

                                // ItemTest column with rowspan
                                if (itemTestFirstOccurrence[rowIndex])
                                {
                                    string itemTestValue = string.IsNullOrEmpty(expandedRows[rowIndex].ItemTest) ? "-" : expandedRows[rowIndex].ItemTest;
                                    if (itemTestRowspans[rowIndex] > 1)
                                    {
                                        sbDetails.Append($"<td rowspan='{itemTestRowspans[rowIndex]}' style='vertical-align: middle; text-align: center;'>{itemTestValue}</td>");
                                    }
                                    else
                                    {
                                        sbDetails.Append($"<td style='vertical-align: middle; text-align: center;'>{itemTestValue}</td>");
                                    }
                                }

                                // ItemSubType column with rowspan
                                if (itemSubTypeFirstOccurrence[rowIndex])
                                {
                                    string itemSubTypeValue = string.IsNullOrEmpty(expandedRows[rowIndex].ItemSubType) ? "-" : expandedRows[rowIndex].ItemSubType;
                                    if (itemSubTypeRowspans[rowIndex] > 1)
                                    {
                                        sbDetails.Append($"<td rowspan='{itemSubTypeRowspans[rowIndex]}' style='vertical-align: middle; text-align: center;'>{itemSubTypeValue}</td>");
                                    }
                                    else
                                    {
                                        sbDetails.Append($"<td style='vertical-align: middle; text-align: center;'>{itemSubTypeValue}</td>");
                                    }
                                }

                                // Specimen column (always show)
                                string specimenValue = string.IsNullOrEmpty(expandedRows[rowIndex].SpecimenNo) ? "-" : expandedRows[rowIndex].SpecimenNo;
                                sbDetails.Append($"<td style='vertical-align: middle; text-align: center;'>{specimenValue}</td>");

                                // Thickness data
                                AddDataColumns(sbDetails, Thickness, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].SpecimenIndex, maxThicknessInputs);
                                AddAvgColumn(sbDetails, ThicknessAvg, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].SpecimenIndex);

                                // Diameter data
                                AddDataColumns(sbDetails, Width, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].SpecimenIndex, maxDiameterInputs);
                                AddAvgColumn(sbDetails, WidthAvg, sampleKey, expandedRows[rowIndex].DateIndex, expandedRows[rowIndex].SpecimenIndex);

                                sbDetails.Append("</tr>");
                            }
                            sbDetails.Append("</table>");
                        }

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

                        body = objEmail.PrepareMailBodyWith("TestRecord_ResistanceAgainstAcidAndAlkalineSolution.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_ResistanceAgainstAcidAndAlkalineSolution-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_ResistanceAgainstAcidAndAlkalineSolution-" + SampleID + ".htm"));
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