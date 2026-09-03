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
    public partial class GenerateTestRecord_MechanicalandElectricalWireTest : System.Web.UI.Page
    {

        DAL.CM _cm = new DAL.CM();
        int InquiryID = 0; int CloneCRFSampleID = 0; int SampleID = 0; int ReportId = 0;
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
                    int SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    int ReportId = Convert.ToInt32(Session["ReportId"].ToString());

                    if (Session["Usertype"] != null)
                    {
                        Dictionary<string, List<string>> No = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Diameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DiameterNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Area = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BF = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationL0 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationL1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Resistance = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Resistivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Stressat1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Wrapping = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Torsion = new Dictionary<string, List<string>>();

                        Dictionary<string, List<string>> SpecifiedDiameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedDiameterNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedArea = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedBF = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedTS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedL0 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedL1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedResistance = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedResistivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedStressat1 = new Dictionary<string, List<string>>();

                        Dictionary<string, List<string>> SpecifiedWrapping = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedTorsion = new Dictionary<string, List<string>>();


                        Dictionary<string, List<string>> CorrectionTemp = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Minimumbrekaingload = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Standard = new Dictionary<string, List<string>>();

                        // Basic report data
                        string Type = "";
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

                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            // DATA PARSING SECTION
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string paramName = dr["ReportParameter"]?.ToString() ?? "";

                                if (reportParamMapID == "12906")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Diameter.ContainsKey(key))
                                                Diameter[key] = new List<string>();
                                            string[] values = coils[i].Split(',');
                                            foreach (string value in values)
                                            {
                                                if (!string.IsNullOrWhiteSpace(value))
                                                    Diameter[key].Add(value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12907")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!DiameterNom.ContainsKey(key))
                                                DiameterNom[key] = new List<string>();
                                            DiameterNom[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12908")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Area.ContainsKey(key))
                                                Area[key] = new List<string>();
                                            Area[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12909")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!BF.ContainsKey(key))
                                                BF[key] = new List<string>();
                                            BF[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12910")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!TS.ContainsKey(key))
                                                TS[key] = new List<string>();
                                            TS[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12911")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!ElongationL0.ContainsKey(key))
                                                ElongationL0[key] = new List<string>();
                                            ElongationL0[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12912")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!ElongationL1.ContainsKey(key))
                                                ElongationL1[key] = new List<string>();
                                            ElongationL1[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12913")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Stressat1.ContainsKey(key))
                                                Stressat1[key] = new List<string>();
                                            Stressat1[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12914")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Resistance.ContainsKey(key))
                                                Resistance[key] = new List<string>();
                                            Resistance[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12915")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Resistivity.ContainsKey(key))
                                                Resistivity[key] = new List<string>();
                                            Resistivity[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12916")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Wrapping.ContainsKey(key))
                                                Wrapping[key] = new List<string>();
                                            Wrapping[key].Add(coils[i].Trim());
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12917")
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string key = (i + 1).ToString();
                                            if (!Torsion.ContainsKey(key))
                                                Torsion[key] = new List<string>();
                                            Torsion[key].Add(coils[i].Trim());
                                        }
                                    }
                                }



                                else if (reportParamMapID == "12918") { Type = result; }
                                else if (reportParamMapID == "12919") { Size = result; }
                                else if (reportParamMapID == "12920") { Voltage = result; }
                                else if (reportParamMapID == "12921") { Date = result; }
                                else if (reportParamMapID == "12922") { RoomNo = result; }
                                else if (reportParamMapID == "12923") { RoomTemp = result; }
                                else if (reportParamMapID == "12924") { RefSpec = result; }
                                else if (reportParamMapID == "12925") { TestMethodNo = result; }
                                else if (reportParamMapID == "12926") { JobOrderNo = result; }
                                else if (reportParamMapID == "12927") { WorkSheetNo = result; }
                                else if (reportParamMapID == "12928") { RoomHumidity = result; }
                                else if (reportParamMapID == "12929") { EquipmentNo = result; }
                                else if (reportParamMapID == "12930")
                                {
                                    string[] parts = result.Trim().Split('|');
                                    if (parts.Length == 2)
                                    {
                                        SpecifiedDiameter["Min"] = new List<string> { parts[0].Trim() };
                                        SpecifiedDiameter["Max"] = new List<string> { parts[1].Trim() };
                                    }
                                    else
                                    {
                                        
                                        SpecifiedDiameter["Value"] = new List<string> { result.Trim() };
                                    }
                                }

                                else if (reportParamMapID == "12931") { SpecifiedDiameterNom["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12932") { SpecifiedArea["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12933") { SpecifiedBF["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12934") { SpecifiedTS["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12935") { SpecifiedL0["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12936") { SpecifiedL1["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12937") { SpecifiedStressat1["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12938") { SpecifiedResistance["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12939") { SpecifiedResistivity["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12940") { SpecifiedWrapping["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12941") { SpecifiedTorsion["Value"] = new List<string> { result.Trim() }; }



                                else if (reportParamMapID == "12942") { CorrectionTemp["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12943") { Minimumbrekaingload["Value"] = new List<string> { result.Trim() }; }
                                else if (reportParamMapID == "12944") { Standard["Value"] = new List<string> { result.Trim() }; }

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
                         </table>"
                                 );
                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");

                        // Row 1 – Main headers
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>No.</th>");
                        sbDetails.Append("<th colspan='7' style='background-color:#bebebe; border: 1px solid #000;'>Diameter ( mm )</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Cross-sectional area</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Breaking force</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Tensile strength</th>");
                        sbDetails.Append("<th colspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Elongation</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Stress at 1%</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Resistance</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Resistivity</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Wrapping</th>");
                        sbDetails.Append("<th rowspan='2' style='background-color:#bebebe; border: 1px solid #000;'>Torsion</th>");
                        sbDetails.Append("</tr>");

                        // Row 2 – Sub-headers
                        sbDetails.Append("<tr>");
                        for (int i = 1; i <= 6; i++)
                        {
                            sbDetails.Append($"<th style='background-color:#bebebe; border: 1px solid #000;'>{i}</th>");
                        }
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>Nom</th>");
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>l₀:</th>");
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>mm</th>");
                        sbDetails.Append("</tr>");

                        // Row 3 – Units
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'> </th>"); // for "No." column
                        for (int i = 0; i < 7; i++)
                        {
                            sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>mm</th>");
                        }
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>mm²</th>");         // Cross-sectional area
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>Kg / N</th>");      // Breaking force
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>Kg/mm²<br>N/mm²</th>"); // Tensile strength
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>mm</th>");          // Elongation l₀
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>%</th>");           // Elongation %
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>Kg/mm²</th>");      // Stress at 1%
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>Ω·m</th>");         // Resistance
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>Ω·mm²</th>");       // Resistivity
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'>…./…./….</th>");    // Wrapping
                        sbDetails.Append("<th style='background-color:#bebebe; border: 1px solid #000;'></th>");           // Torsion
                        sbDetails.Append("</tr>");

                        // TABLE GENERATION SECTION
                        int RowNumber = 1;
                        // Get all unique keys from dictionaries (following your existing pattern)
                        var allKeys = new HashSet<string>();
                        foreach (var dict in new[] {
    Diameter, DiameterNom, Area, BF, TS, ElongationL0, ElongationL1,
    Stressat1, Resistance, Resistivity, Wrapping, Torsion
})
                        {
                            foreach (var key in dict.Keys)
                                allKeys.Add(key);
                        }

                        // Sort keys numerically
                        var sortedKeys = allKeys.OrderBy(k => int.Parse(k)).ToList();

                        // Generate table rows for each key
                        foreach (string key in sortedKeys)
                        {
                            sbDetails.Append("<tr>");

                            // No. column - use the key as row number (1, 2, 3, etc.)
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{key}</td>");

                            // Diameter columns (1-6) - spread the values across 6 columns
                            var diameterValues = Diameter.ContainsKey(key) ? Diameter[key] : new List<string>();
                            for (int j = 0; j < 6; j++)
                            {
                                string diameterValue = j < diameterValues.Count ? diameterValues[j] : "";
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{diameterValue}</td>");
                            }

                            // Diameter Nom column
                            string diameterNom = DiameterNom.ContainsKey(key) && DiameterNom[key].Count > 0 ? DiameterNom[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{diameterNom}</td>");

                            // Cross-sectional area
                            string area = Area.ContainsKey(key) && Area[key].Count > 0 ? Area[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{area}</td>");

                            // Breaking force
                            string bf = BF.ContainsKey(key) && BF[key].Count > 0 ? BF[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{bf}</td>");

                            // Tensile strength (1 column only)
                            string ts = TS.ContainsKey(key) && TS[key].Count > 0 ? TS[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{ts}</td>");

                            // Elongation (2 columns - L0 and %)
                            string elongationL0 = ElongationL0.ContainsKey(key) && ElongationL0[key].Count > 0 ? ElongationL0[key][0] : "";
                            string elongationL1 = ElongationL1.ContainsKey(key) && ElongationL1[key].Count > 0 ? ElongationL1[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{elongationL0}</td>");
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{elongationL1}</td>");

                            // Stress at 1%
                            string stressat1 = Stressat1.ContainsKey(key) && Stressat1[key].Count > 0 ? Stressat1[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{stressat1}</td>");

                            // Resistance
                            string resistance = Resistance.ContainsKey(key) && Resistance[key].Count > 0 ? Resistance[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{resistance}</td>");

                            // Resistivity
                            string resistivity = Resistivity.ContainsKey(key) && Resistivity[key].Count > 0 ? Resistivity[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{resistivity}</td>");

                            // Wrapping
                            string wrapping = Wrapping.ContainsKey(key) && Wrapping[key].Count > 0 ? Wrapping[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{wrapping}</td>");

                            // Torsion
                            string torsion = Torsion.ContainsKey(key) && Torsion[key].Count > 0 ? Torsion[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{torsion}</td>");

                            sbDetails.Append("</tr>");
                        }

                        // Add Standard row with proper colspan structure matching the image
                        sbDetails.Append("<tr style='font-weight:bold;'>");

                        // Standard label in the No. column
                        sbDetails.Append("<td style='text-align:left; border: 1px solid #000;'>Standard</td>");

                        // Diameter columns with proper colspan structure as shown in image
                        string diameterMin = SpecifiedDiameter.ContainsKey("Min") && SpecifiedDiameter["Min"].Count > 0 ? SpecifiedDiameter["Min"][0] : "";
                        string diameterMax = SpecifiedDiameter.ContainsKey("Max") && SpecifiedDiameter["Max"].Count > 0 ? SpecifiedDiameter["Max"][0] : "";
                        string diameterNomSpec = SpecifiedDiameterNom.ContainsKey("Value") && SpecifiedDiameterNom["Value"].Count > 0 ? SpecifiedDiameterNom["Value"][0] : "";

                        // Min: (colspan=2 for columns 1,2)
                        sbDetails.Append($"<td colspan='2' style='text-align:center; border: 1px solid #000;'>Min: {diameterMin}</td>");
                        // Nom: (single column 3)  
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Nom: {diameterNomSpec}</td>");
                        // Max: (colspan=2 for columns 4,5)
                        sbDetails.Append($"<td colspan='2' style='text-align:center; border: 1px solid #000;'>Max: {diameterMax}</td>");
                        // Empty column 6
                        sbDetails.Append("<td style='text-align:center; border: 1px solid #000;'>-</td>");
                        // Nom column 7
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{diameterNomSpec}</td>");

                        // Cross-sectional area
                        string areaSpec = SpecifiedArea.ContainsKey("Value") && SpecifiedArea["Value"].Count > 0 ? SpecifiedArea["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{areaSpec}</td>");

                        // Breaking force with Min value
                        string bfSpec = SpecifiedBF.ContainsKey("Value") && SpecifiedBF["Value"].Count > 0 ? SpecifiedBF["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Min: {bfSpec}</td>");

                        // Tensile strength with Min value
                        string tsSpec = SpecifiedTS.ContainsKey("Value") && SpecifiedTS["Value"].Count > 0 ? SpecifiedTS["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Min: {tsSpec}</td>");

                        // Elongation (2 columns) with Min values
                        string elongationL0Spec = SpecifiedL0.ContainsKey("Value") && SpecifiedL0["Value"].Count > 0 ? SpecifiedL0["Value"][0] : "";
                        string elongationL1Spec = SpecifiedL1.ContainsKey("Value") && SpecifiedL1["Value"].Count > 0 ? SpecifiedL1["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Min: {elongationL0Spec}</td>");
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Min: {elongationL1Spec}</td>");

                        // Stress at 1% with Min value
                        string stressat1Spec = SpecifiedStressat1.ContainsKey("Value") && SpecifiedStressat1["Value"].Count > 0 ? SpecifiedStressat1["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Min: {stressat1Spec}</td>");

                        // Resistance with Max value
                        string resistanceSpec = SpecifiedResistance.ContainsKey("Value") && SpecifiedResistance["Value"].Count > 0 ? SpecifiedResistance["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Max: {resistanceSpec}</td>");

                        // Resistivity with value (shows "No Crack" in image)
                        string resistivitySpec = SpecifiedResistivity.ContainsKey("Value") && SpecifiedResistivity["Value"].Count > 0 ? SpecifiedResistivity["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{(string.IsNullOrEmpty(resistivitySpec) ? "No Crack" : resistivitySpec)}</td>");

                        // Wrapping with Min value
                        string wrappingSpec = SpecifiedWrapping.ContainsKey("Value") && SpecifiedWrapping["Value"].Count > 0 ? SpecifiedWrapping["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>Min: {wrappingSpec}</td>");

                        // Torsion with value
                        string torsionSpec = SpecifiedTorsion.ContainsKey("Value") && SpecifiedTorsion["Value"].Count > 0 ? SpecifiedTorsion["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{torsionSpec}</td>");

                        sbDetails.Append("</tr>");

                        // Add the correction temperature and minimum breaking load row with proper colspan
                        sbDetails.Append("<tr>");

                        // Correction temperature label (colspan=4 to span multiple columns as shown in image)
                        sbDetails.Append("<td colspan='4' style='text-align:left; padding:5px; border: 1px solid #000;'><strong>Correction temperature:</strong></td>");

                        // Correction temperature value (colspan=4) - the yellow highlighted area in image
                        string correctionTemp = CorrectionTemp.ContainsKey("Value") && CorrectionTemp["Value"].Count > 0 ? CorrectionTemp["Value"][0] : "";
                        sbDetails.Append($"<td colspan='4' style='text-align:center; border: 1px solid #000;'>{correctionTemp}</td>");

                        // Minimum breaking load label cell (colspan=2)
                        sbDetails.Append("<td colspan='2' style='text-align:left; padding:5px; border: 1px solid #000;'><strong>Minimum breaking load (calculation):</strong></td>");

                        // Minimum breaking load value cell 
                        string minBreakingLoad = Minimumbrekaingload.ContainsKey("Value") && Minimumbrekaingload["Value"].Count > 0 ? Minimumbrekaingload["Value"][0] : "";
                        sbDetails.Append($"<td style='text-align:center; border: 1px solid #000;'>{minBreakingLoad}</td>");

                        // Standard label (colspan=2)
                        sbDetails.Append("<td colspan='2' style='text-align:left; padding:5px; border: 1px solid #000;'><strong>Standard:</strong></td>");

                        // Standard value (colspan=3) - the yellow highlighted area in image
                        string standard = Standard.ContainsKey("Value") && Standard["Value"].Count > 0 ? Standard["Value"][0] : "";
                        sbDetails.Append($"<td colspan='3' style='text-align:center; border: 1px solid #000;'>{standard}</td>");

                        sbDetails.Append("</tr>");

                        sbDetails.Append("</table>"); sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
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


                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_MechanicalandElectricalWire.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_MechanicalandElectricalWire -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_MechanicalandElectricalWire -" + SampleID + ".htm"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
                    }
                }

                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}