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
    public partial class GenerateTestReport_LossOfMass_Rev02 : System.Web.UI.Page
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
                        //List<string> Thickness = new List<string>();
                        //List<string> ThicknessAvg = new List<string>();

                        Dictionary<string, Dictionary<int, List<string>>> ThicknessBySpecimen = new Dictionary<string, Dictionary<int, List<string>>>();
                        Dictionary<string, Dictionary<int, List<string>>> DiameterBySpecimen = new Dictionary<string, Dictionary<int, List<string>>>();
                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThicknessAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Diameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DiameterAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> CrossSectionArea = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> beforeageing = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> afterageing = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> lossofmass = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> median = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> desciccatorsbeforeageingdate = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> desciccatorsbeforeageingTime = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> desciccatorafterageingdate = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> desciccatorsafterageingTime = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Ageingairovendate = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AgeingairovenTime = new Dictionary<string, List<string>>();


                        List<string> ExtractSpecimenValues(string resultString)
                        {
                            return resultString
                                .Split('|')
                                .Select(entry =>
                                {
                                    var parts = entry.Split(':');
                                    return parts.Length == 2 ? parts[1] : null;
                                })
                                .Where(val => val != null)
                                .ToList();
                        }

                        // Specified values
                        string SpecifiedThickness = "";
                        string SpecifiedDiameter = "";
                        string Specifiedcrosssectionarea = "";
                        string Specifiedbeforeageing = "";
                        string Specifiedafterageing = "";
                        string SpecifiedMedian = "";
                        string Airflow = "";
                        string OverTemp = "";




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
                                string[] specifications = result.Split('|');

                                if (reportParamMapID == "14173") // Thickness
                                {
                                    foreach (var spec in specifications)
                                    {
                                        var parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string[] phaseSpec = parts[0].Split(';');
                                            if (phaseSpec.Length == 2)
                                            {
                                                string phase = phaseSpec[0];
                                                if (!int.TryParse(phaseSpec[1], out int specimenNo)) continue;

                                                string[] values = parts[1].Split(',');

                                                if (!ThicknessBySpecimen.ContainsKey(phase))
                                                    ThicknessBySpecimen[phase] = new Dictionary<int, List<string>>();

                                                if (!ThicknessBySpecimen[phase].ContainsKey(specimenNo))
                                                    ThicknessBySpecimen[phase][specimenNo] = new List<string>();

                                                ThicknessBySpecimen[phase][specimenNo].AddRange(values);
                                            }
                                        }
                                    }
                                }

                                if (reportParamMapID == "14175") // Diameter
                                {
                                    foreach (var spec in specifications)
                                    {
                                        var parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string[] phaseSpec = parts[0].Split(';');
                                            if (phaseSpec.Length == 2)
                                            {
                                                string phase = phaseSpec[0];
                                                if (!int.TryParse(phaseSpec[1], out int specimenNo)) continue;

                                                string[] values = parts[1].Split(',');

                                                if (!DiameterBySpecimen.ContainsKey(phase))
                                                    DiameterBySpecimen[phase] = new Dictionary<int, List<string>>();

                                                if (!DiameterBySpecimen[phase].ContainsKey(specimenNo))
                                                    DiameterBySpecimen[phase][specimenNo] = new List<string>();

                                                DiameterBySpecimen[phase][specimenNo].AddRange(values);
                                            }
                                        }
                                    }
                                }

                                // Handle Thickness (14173) - Store all values in single cell
                                if (reportParamMapID == "14173")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string[] values = parts[1].Split(',');

                                            // Extract the phase part (before the semicolon)
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!Thickness.ContainsKey(phase))
                                                Thickness[phase] = new List<string>();

                                            Thickness[phase].AddRange(values);
                                        }
                                    }
                                }

                                // Handle Diameter (14175) - Store all values in single cell
                                if (reportParamMapID == "14175")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string[] values = parts[1].Split(',');
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!Diameter.ContainsKey(phase))
                                                Diameter[phase] = new List<string>();

                                            Diameter[phase].AddRange(values);
                                        }
                                    }
                                }

                                var entries = result.Split('|');

                                if (reportParamMapID == "14172") // CrossSectionArea
                                {
                                    foreach (var entry in entries)
                                    {
                                        var parts = entry.Split(';');
                                        if (parts.Length != 2) continue;

                                        var phase = parts[0].Trim();
                                        var specimenValue = parts[1].Split(':');
                                        if (specimenValue.Length != 2) continue;

                                        var value = specimenValue[1].Trim();

                                        if (!CrossSectionArea.ContainsKey(phase))
                                            CrossSectionArea[phase] = new List<string>();

                                        CrossSectionArea[phase].Add(value);
                                    }
                                }
                                else if (reportParamMapID == "14177") // beforeageing
                                {
                                    foreach (var entry in entries)
                                    {
                                        var parts = entry.Split(';');
                                        if (parts.Length != 2) continue;

                                        var phase = parts[0].Trim();
                                        var specimenValue = parts[1].Split(':');
                                        if (specimenValue.Length != 2) continue;

                                        var value = specimenValue[1].Trim();

                                        if (!beforeageing.ContainsKey(phase))
                                            beforeageing[phase] = new List<string>();

                                        beforeageing[phase].Add(value);
                                    }
                                }
                                else if (reportParamMapID == "14178") // afterageing
                                {
                                    foreach (var entry in entries)
                                    {
                                        var parts = entry.Split(';');
                                        if (parts.Length != 2) continue;

                                        var phase = parts[0].Trim();
                                        var specimenValue = parts[1].Split(':');
                                        if (specimenValue.Length != 2) continue;

                                        var value = specimenValue[1].Trim();

                                        if (!afterageing.ContainsKey(phase))
                                            afterageing[phase] = new List<string>();

                                        afterageing[phase].Add(value);
                                    }
                                }
                                else if (reportParamMapID == "14179") // median
                                {
                                    foreach (var entry in entries)
                                    {
                                        var parts = entry.Split(';');
                                        if (parts.Length != 2) continue;

                                        var phase = parts[0].Trim();
                                        var specimenValue = parts[1].Split(':');
                                        if (specimenValue.Length != 2) continue;

                                        var value = specimenValue[1].Trim();

                                        if (!median.ContainsKey(phase))
                                            median[phase] = new List<string>();

                                        median[phase].Add(value);
                                    }
                                }
                                else if (reportParamMapID == "14180") // lossofmass
                                {
                                    foreach (var entry in entries)
                                    {
                                        var parts = entry.Split(';');
                                        if (parts.Length != 2) continue;

                                        var phase = parts[0].Trim();
                                        var specimenValue = parts[1].Split(':');
                                        if (specimenValue.Length != 2) continue;

                                        var value = specimenValue[1].Trim();

                                        if (!lossofmass.ContainsKey(phase))
                                            lossofmass[phase] = new List<string>();

                                        lossofmass[phase].Add(value);
                                    }
                                }
                                // Handle ThicknessAvg (14174) - Parse phase;specimen:avg,min format
                                if (reportParamMapID == "14174")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        var parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string[] phaseSpec = parts[0].Split(';');
                                            if (phaseSpec.Length == 2)
                                            {
                                                string phase = phaseSpec[0];
                                                if (!int.TryParse(phaseSpec[1], out int specimenNo)) continue;

                                                string avgMinValues = parts[1]; // "3.5,1"

                                                if (!ThicknessAvg.ContainsKey(phase))
                                                    ThicknessAvg[phase] = new List<string>();

                                                ThicknessAvg[phase].Add(avgMinValues);
                                            }
                                        }
                                    }
                                }

                                // Handle DiameterAvg (14176) - Parse phase;specimen:avg,min format  
                                if (reportParamMapID == "14176")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        var parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string[] phaseSpec = parts[0].Split(';');
                                            if (phaseSpec.Length == 2)
                                            {
                                                string phase = phaseSpec[0];
                                                if (!int.TryParse(phaseSpec[1], out int specimenNo)) continue;

                                                string avgMinValues = parts[1]; // "avg,min"

                                                if (!DiameterAvg.ContainsKey(phase))
                                                    DiameterAvg[phase] = new List<string>();

                                                DiameterAvg[phase].Add(avgMinValues);
                                            }
                                        }
                                    }
                                }

                                if (reportParamMapID == "14186")  // desciccatorsbeforeageingdate
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            string[] dateValues = parts[1].Split(';');  // "startDate;finishDate"
                                            if (!desciccatorsbeforeageingdate.ContainsKey(phase))
                                                desciccatorsbeforeageingdate[phase] = new List<string>();

                                            desciccatorsbeforeageingdate[phase].AddRange(dateValues);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "14187")  // desciccatorsbeforeageingTime
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            string[] timeValues = parts[1].Split(';');  // "startTime;finishTime"
                                            if (!desciccatorsbeforeageingTime.ContainsKey(phase))
                                                desciccatorsbeforeageingTime[phase] = new List<string>();

                                            desciccatorsbeforeageingTime[phase].AddRange(timeValues);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "14188")  // desciccatorafterageingdate
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            string[] dateValues = parts[1].Split(';');
                                            if (!desciccatorafterageingdate.ContainsKey(phase))
                                                desciccatorafterageingdate[phase] = new List<string>();

                                            desciccatorafterageingdate[phase].AddRange(dateValues);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "14189")  // desciccatorsafterageingTime
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            string[] timeValues = parts[1].Split(';');
                                            if (!desciccatorsafterageingTime.ContainsKey(phase))
                                                desciccatorsafterageingTime[phase] = new List<string>();

                                            desciccatorsafterageingTime[phase].AddRange(timeValues);
                                        }
                                    }
                                }

                                // Single value assignments
                                if (dr["ReportParamMapID"].ToString() == "14160") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14161") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14162") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14163") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14164") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14165") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14166") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14167") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14168") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14169") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14170") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14171") { EquipmentNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14181") { SpecifiedThickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14182") { SpecifiedDiameter = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14194") { Specifiedcrosssectionarea = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14183") { Specifiedbeforeageing = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14184") { Specifiedafterageing = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14185") { SpecifiedMedian = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14190") { Airflow = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14191") { OverTemp = dr["Result"].ToString(); }
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
                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>
                                            <tr>
                            <td rowspan='3' style='background-color:#bebebe;'>Core Phase</td>
                            <td rowspan='3' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Specimen No.</td>
                            <td colspan='7' style='background-color:#bebebe;'>Thickness/Weight</td>
                            <td colspan='7' style='background-color:#bebebe;'>Width/Diameter/Length/Density</td>
                            <td rowspan='2' colspan='1' style='background-color:#bebebe;'>Cross sectional area</td>
                            <td colspan='2' style='background-color:#bebebe;'>Weight</td>
                            <td rowspan='2' style='background-color:#bebebe;'>Loss of mass</td>
                            <td rowspan='2' style='background-color:#bebebe;'>Median</td>
                                </tr>
                                   <tr>
                                     <td style='background-color:#bebebe;'>1</td>
                                     <td style='background-color:#bebebe;'>2</td>
                                     <td style='background-color:#bebebe;'>3</td>
                                     <td style='background-color:#bebebe;'>4</td>
                                     <td style='background-color:#bebebe;'>5</td>
                                     <td style='background-color:#bebebe;'>6</td>
                                     <td style='background-color:#bebebe;'>Avg<br/>Min.</td>
                                     <td style='background-color:#bebebe;'>1</td>
                                     <td style='background-color:#bebebe;'>2</td>
                                     <td style='background-color:#bebebe;'>3</td>
                                     <td style='background-color:#bebebe;'>4</td>
                                     <td style='background-color:#bebebe;'>5</td>
                                     <td style='background-color:#bebebe;'>6</td>
                                     <td style='background-color:#bebebe;'>Avg<br/>Min.</td>
                                     <td style='background-color:#bebebe;'>Before Ageing</td>
                                     <td style='background-color:#bebebe;'>After Ageing</td>
                                     </tr>

                          <tr>
                                <td colspan='7' style='background-color:#bebebe;'>mm/gr</td>
                                <td colspan='7' style='background-color:#bebebe;'>mm/[gr/cm²]</td>
                                <td style='background-color:#bebebe;'>cm²</td>
                                <td colspan='2' style='background-color:#bebebe;'>mg</td>
                                <td colspan='2' style='background-color:#bebebe;'>mg/cm²</td>
                           </tr>
                                          </tr>");
                        var distinctPhases = Thickness.Keys.ToList();

                        int maxMeasurementsPerRow = 6; // how many values per row (per specimen)

                        foreach (string phase in distinctPhases)
                        {
                            var allThicknessValues = Thickness.ContainsKey(phase) ? Thickness[phase] : new List<string>();
                            var allDiameterValues = Diameter.ContainsKey(phase) ? Diameter[phase] : new List<string>();
                            var allCrossSectionarea = CrossSectionArea.ContainsKey(phase) ? CrossSectionArea[phase] : new List<string>();
                            var allbeforeageing = beforeageing.ContainsKey(phase) ? beforeageing[phase] : new List<string>();
                            var allafterageing = afterageing.ContainsKey(phase) ? afterageing[phase] : new List<string>();
                            var allMedianValues = median.ContainsKey(phase) ? median[phase] : new List<string>();
                            var alllossofmassValues = lossofmass.ContainsKey(phase) ? lossofmass[phase] : new List<string>();

                            // Get ThicknessAvg and DiameterAvg for current phase
                            List<string> parsedThicknessAvg = new List<string>();
                            List<string> parsedDiameterAvg = new List<string>();

                            // Parse ThicknessAvg - now stored as individual "avg,min" strings per specimen
                            if (ThicknessAvg.ContainsKey(phase))
                            {
                                foreach (string avgMinPair in ThicknessAvg[phase])
                                {
                                    string[] avgMin = avgMinPair.Split(',');
                                    if (avgMin.Length == 2)
                                    {
                                        // Format as "Avg: 3.5, Min: 1"
                                        parsedThicknessAvg.Add($"Avg: {avgMin[0].Trim()}, Min: {avgMin[1].Trim()}");
                                    }
                                    else
                                    {
                                        // Fallback if format is unexpected
                                        parsedThicknessAvg.Add(avgMinPair);
                                    }
                                }
                            }

                            // Parse DiameterAvg - now stored as individual "avg,min" strings per specimen
                            if (DiameterAvg.ContainsKey(phase))
                            {
                                foreach (string avgMinPair in DiameterAvg[phase])
                                {
                                    string[] avgMin = avgMinPair.Split(',');
                                    if (avgMin.Length == 2)
                                    {
                                        // Format as "Avg: 9.5, Min: 7"
                                        parsedDiameterAvg.Add($"Avg: {avgMin[0].Trim()}, Min: {avgMin[1].Trim()}");
                                    }
                                    else
                                    {
                                        // Fallback if format is unexpected
                                        parsedDiameterAvg.Add(avgMinPair);
                                    }
                                }
                            }

                            // Calculate how many specimen rows to create
                            int specimenCount = (int)Math.Ceiling((double)Math.Max(allThicknessValues.Count, allDiameterValues.Count) / maxMeasurementsPerRow);

                            // Pick a median value to display for the phase
                            string mergedMedianValue = "-";
                            if (allMedianValues.Count > 0)
                            {
                                mergedMedianValue = allMedianValues.Max();
                            }

                            for (int i = 0; i < specimenCount; i++)
                            {
                                sbDetails.Append("<tr>");

                                if (i == 0)
                                {
                                    sbDetails.Append($"<td rowspan='{specimenCount}' style='text-align:center;'>{phase}</td>");
                                }

                                sbDetails.Append($"<td style='text-align:center;'>{i + 1}</td>");

                                // Thickness values for specimen i (6 max per row)
                                for (int j = 0; j < maxMeasurementsPerRow; j++)
                                {
                                    int index = i * maxMeasurementsPerRow + j;
                                    string val = index < allThicknessValues.Count ? allThicknessValues[index] : "-";
                                    sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                                }

                                // Thickness Avg/Min for specimen i
                                string thicknessAvg = i < parsedThicknessAvg.Count ? parsedThicknessAvg[i] : "-";
                                sbDetails.Append($"<td style='text-align:center;'>{thicknessAvg}</td>");

                                // Diameter values for specimen i
                                for (int j = 0; j < maxMeasurementsPerRow; j++)
                                {
                                    int index = i * maxMeasurementsPerRow + j;
                                    string val = index < allDiameterValues.Count ? allDiameterValues[index] : "-";
                                    sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                                }

                                // Diameter Avg/Min for specimen i
                                string diameterAvg = i < parsedDiameterAvg.Count ? parsedDiameterAvg[i] : "-";
                                sbDetails.Append($"<td style='text-align:center;'>{diameterAvg}</td>");

                                // Cross Section Area for specimen i
                                string crosssectionarea = i < allCrossSectionarea.Count ? allCrossSectionarea[i] : "-";
                                sbDetails.Append($"<td style='text-align:center;'>{crosssectionarea}</td>");

                                // Before Ageing for specimen i
                                string beforeageingValue = i < allbeforeageing.Count ? allbeforeageing[i] : "-";
                                sbDetails.Append($"<td style='text-align:center;'>{beforeageingValue}</td>");

                                // After Ageing for specimen i
                                string afterageingValue = i < allafterageing.Count ? allafterageing[i] : "-";
                                sbDetails.Append($"<td style='text-align:center;'>{afterageingValue}</td>");

                                // Loss of Mass for specimen i
                                string lossofmassValue = i < alllossofmassValues.Count ? alllossofmassValues[i] : "-";
                                sbDetails.Append($"<td style='text-align:center;'>{lossofmassValue}</td>");

                                // Median value only once per phase
                                if (i == 0)
                                {
                                    sbDetails.Append($"<td rowspan='{specimenCount}' style='text-align:center; vertical-align:middle;'>{mergedMedianValue}</td>");
                                }

                                sbDetails.Append("</tr>");
                            }
                        }


                        sbDetails.Append("</table>");
                        string descBeforeDateStart = "", descBeforeDateEnd = "";
                        string descBeforeTimeStart = "", descBeforeTimeEnd = "";
                        string descAfterDateStart = "", descAfterDateEnd = "";
                        string descAfterTimeStart = "", descAfterTimeEnd = "";
                        string ageingOvenDateStart = "", ageingOvenDateEnd = "";
                        string ageingOvenTimeStart = "", ageingOvenTimeEnd = "";
                        foreach (DataRow dr in dtRecordDetails.Rows)
                        {
                            string reportParamMapID = dr["ReportParamMapID"].ToString();
                            string result = dr["Result"].ToString();

                            string[] parts = result.Split(';');
                            string part1 = parts.Length > 0 ? parts[0] : "";
                            string part2 = parts.Length > 1 ? parts[1] : "";

                            if (reportParamMapID == "14186") // Before ageing date
                            {
                                descBeforeDateStart = part1;
                                descBeforeDateEnd = part2;
                            }
                            else if (reportParamMapID == "14187") // Before ageing time
                            {
                                descBeforeTimeStart = part1;
                                descBeforeTimeEnd = part2;
                            }
                            else if (reportParamMapID == "14188") // After ageing date
                            {
                                descAfterDateStart = part1;
                                descAfterDateEnd = part2;
                            }
                            else if (reportParamMapID == "14189") // After ageing time
                            {
                                descAfterTimeStart = part1;
                                descAfterTimeEnd = part2;
                            }
                            else if (reportParamMapID == "14192") // Ageing oven date
                            {
                                ageingOvenDateStart = part1;
                                ageingOvenDateEnd = part2;
                            }
                            else if (reportParamMapID == "14193") // Ageing oven time
                            {
                                ageingOvenTimeStart = part1;
                                ageingOvenTimeEnd = part2;
                            }
                        }

                        // Build airflow and oven temperature content
                        string airflowContent = !string.IsNullOrEmpty(Airflow) ? Airflow : "";
                        string ovenTempContent = !string.IsNullOrEmpty(OverTemp) ? OverTemp : "";

                        // Convert dates from yyyy-mm-dd to dd-mm-yy format
                        string ConvertDateFormat(string dateStr)
                        {
                            if (string.IsNullOrEmpty(dateStr) || dateStr.Length < 10) return dateStr;
                            try
                            {
                                DateTime date = DateTime.ParseExact(dateStr, "yyyy-MM-dd", null);
                                return date.ToString("dd-MM-yy");
                            }
                            catch
                            {
                                return dateStr; // Return original if conversion fails
                            }
                        }

                        // Convert all date values to dd-mm-yy format
                        descBeforeDateStart = ConvertDateFormat(descBeforeDateStart);
                        descBeforeDateEnd = ConvertDateFormat(descBeforeDateEnd);
                        descAfterDateStart = ConvertDateFormat(descAfterDateStart);
                        descAfterDateEnd = ConvertDateFormat(descAfterDateEnd);
                        ageingOvenDateStart = ConvertDateFormat(ageingOvenDateStart);
                        ageingOvenDateEnd = ConvertDateFormat(ageingOvenDateEnd);
                        sbFooter.Append($@"
<table style='width:100%; font-family:Times New Roman; font-size:10px; border-collapse: collapse; line-height:1.2;' class='tab1' border='1'>
    <!-- Main Header Row -->
    <tr style='height:25px;'>
        <td colspan='4' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Conditioning in desiccators before ageing</td>
        <td colspan='4' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Ageing in air oven</td>
        <td rowspan='3' style='font-weight:bold; vertical-align:middle; text-align:center; background-color:#f0f0f0; padding:2px;'>PT Qualis Indonesia</td>
        <td colspan='6' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Witnessed by</td>
    </tr>
    
    <!-- Sub Header Row 1 -->
    <tr style='height:25px;'>
        <td colspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Date</td>
        <td colspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Time</td>
        <td colspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Date</td>
        <td colspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Time</td>
        <td rowspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Name</td>
        <td rowspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Company</td>
        <td  rowspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Signature</td>
    </tr>
    
    <!-- Sub Header Row 2 -->
    <tr style='height:25px;'>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Start</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Finish</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Start</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Finish</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Start</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Finish</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Start</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Finish</td>
    
    </tr>
    
    <!-- Data Row 1 -->
    <tr style='height:25px;'>
        <td style='text-align:center; padding:2px;'>{descBeforeDateStart}</td>
        <td style='text-align:center; padding:2px;'>{descBeforeDateEnd}</td>
        <td style='text-align:center; padding:2px;'>{descBeforeTimeStart}</td>
        <td style='text-align:center; padding:2px;'>{descBeforeTimeEnd}</td>
        <td style='text-align:center; padding:2px;'>{ageingOvenDateStart}</td>
        <td style='text-align:center; padding:2px;'>{ageingOvenDateEnd}</td>
        <td style='text-align:center; padding:2px;'>{ageingOvenTimeStart}</td>
        <td style='text-align:center; padding:2px;'>{ageingOvenTimeEnd}</td>
        <td rowspan='4' style='padding:2px;'>
                                    Tested   by:
                                </td>
                               
                                <td rowspan='5'></td>
                                <td rowspan='5'></td>
                                <td rowspan='5'></td>
                                
    </tr>
    
    <!-- Second Section Header -->
    <tr style='height:25px;'>
        <td colspan='4' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Conditioning in desiccators after ageing</td>
        <td colspan ='2' rowspan='3' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Airflow</td>
        <td colspan = '2' rowspan='3' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Oven Temperature</td>
       
    </tr>
    
    <!-- Second Section Sub Headers -->
    <tr style='height:25px;'>
        <td colspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Date</td>
        <td colspan='2' style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px;'>Time</td>

     
    </tr>
    
    <!-- Second Section Column Headers -->
    <tr style='height:25px;'>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Start</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Finish</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Start</td>
        <td style='text-align:center; font-weight:bold; background-color:#f0f0f0; padding:2px; font-size:9px;'>Finish</td>
     
    </tr>
    
    <!-- Final Data Row -->
    <tr style='height:25px;'>
        <td style='text-align:center; padding:2px;'>{descAfterDateStart}</td>
        <td style='text-align:center; padding:2px;'>{descAfterDateEnd}</td>
        <td style='text-align:center; padding:2px;'>{descAfterTimeStart}</td>
        <td style='text-align:center; padding:2px;'>{descAfterTimeEnd}</td>
     <td colspan='2' style='text-align:center; vertical-align:middle; padding:2px;'>{airflowContent}</td>
     <td colspan='2' style='text-align:center; vertical-align:middle; padding:2px;'>{ovenTempContent}</td>
     <td  style='padding:2px;'>Checked By</td>
        
        
     
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_LossOfMassRev02.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_LossOfMassRev02-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_LossOfMassRev02-" + SampleID + ".htm"));
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