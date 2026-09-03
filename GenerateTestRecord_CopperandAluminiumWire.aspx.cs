using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.BouncyCastle.Tsp;
using LIMS.DAL;

namespace LIMS
{
    public partial class GenerateTestRecord_CopperandAluminiumWire : System.Web.UI.Page
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
                        Dictionary<string, List<string>> CoilNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecimenNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Diameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DiameterNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Area = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BF = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Temperature = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Density = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationL0 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationL1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ElongationEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MedianTS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MedianEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Resistance = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> kt = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Conductivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Resistivityat20C = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DateList = new Dictionary<string, List<string>>();


                        Dictionary<string, List<string>> SpecifiedDiameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedDiameterNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedArea = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedBF = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedTS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedL0 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedL1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedMedianTS = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedMedianEL = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedMedianResistance = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedTemp = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedKt = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedResistivityat20 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedConductivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> SpecifiedDensity = new Dictionary<string, List<string>>();

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
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string paramName = dr["ReportParameter"]?.ToString() ?? "";

                                if (reportParamMapID == "12841") // CoilNo
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string coilValue = coils[i].Trim();
                                            string key = $"Coil{i + 1}";
                                            if (!CoilNo.ContainsKey(key))
                                                CoilNo[key] = new List<string>();
                                            CoilNo[key].Add(coilValue);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12842") // SpecimenNo
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!SpecimenNo.ContainsKey(key))
                                                        SpecimenNo[key] = new List<string>();
                                                    SpecimenNo[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12843") // Diameter
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Diameter.ContainsKey(key))
                                                        Diameter[key] = new List<string>();
                                                    string[] values = specimens[j].Split(',');
                                                    foreach (string value in values)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(value))
                                                            Diameter[key].Add(value.Trim());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12844") // DiameterNom
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!DiameterNom.ContainsKey(key))
                                                        DiameterNom[key] = new List<string>();
                                                    DiameterNom[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12845") // Area
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Area.ContainsKey(key))
                                                        Area[key] = new List<string>();
                                                    Area[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12846") // BF
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!BF.ContainsKey(key))
                                                        BF[key] = new List<string>();
                                                    BF[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12847") // TS
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!TS.ContainsKey(key))
                                                        TS[key] = new List<string>();
                                                    TS[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12848") // Temperature
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Temperature.ContainsKey(key))
                                                        Temperature[key] = new List<string>();
                                                    Temperature[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12849") // Density
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Density.ContainsKey(key))
                                                        Density[key] = new List<string>();
                                                    Density[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12850") // ElongationL0
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!ElongationL0.ContainsKey(key))
                                                        ElongationL0[key] = new List<string>();
                                                    ElongationL0[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12851") // ElongationL1
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!ElongationL1.ContainsKey(key))
                                                        ElongationL1[key] = new List<string>();
                                                    ElongationL1[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12852") // ElongationEL
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!ElongationEL.ContainsKey(key))
                                                        ElongationEL[key] = new List<string>();
                                                    ElongationEL[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12853") // MedianTS
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!MedianTS.ContainsKey(key))
                                                        MedianTS[key] = new List<string>();
                                                    MedianTS[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12854") // MedianEL
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!MedianEL.ContainsKey(key))
                                                        MedianEL[key] = new List<string>();
                                                    MedianEL[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12855") // Resistance
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Resistance.ContainsKey(key))
                                                        Resistance[key] = new List<string>();
                                                    Resistance[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12856") // kt
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!kt.ContainsKey(key))
                                                        kt[key] = new List<string>();
                                                    kt[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12857") // Conductivity
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Conductivity.ContainsKey(key))
                                                        Conductivity[key] = new List<string>();
                                                    Conductivity[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12858") // Resistivityat20C
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] specimens = coils[i].Split('|');
                                            for (int j = 0; j < specimens.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(specimens[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!Resistivityat20C.ContainsKey(key))
                                                        Resistivityat20C[key] = new List<string>();
                                                    Resistivityat20C[key].Add(specimens[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12889") // DateList
                                {
                                    string[] coils = result.Split(';');
                                    for (int i = 0; i < coils.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(coils[i]))
                                        {
                                            string[] dates = coils[i].Split('|');
                                            for (int j = 0; j < dates.Length; j++)
                                            {
                                                if (!string.IsNullOrEmpty(dates[j]))
                                                {
                                                    string key = $"Coil{i + 1}_Specimen{j + 1}";
                                                    if (!DateList.ContainsKey(key))
                                                        DateList[key] = new List<string>();
                                                    DateList[key].Add(dates[j].Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                            
                                else
                                {
                                    if (reportParamMapID == "12877") { Type = result; }
                                    else if (reportParamMapID == "12883") { RefSpec = result; }
                                    else if (reportParamMapID == "12878") { Size = result; }
                                    else if (reportParamMapID == "12884") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12879") { Voltage = result; }
                                    else if (reportParamMapID == "12885") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12886") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12881") { RoomNo = result; }
                                    else if (reportParamMapID == "12887") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12882") { RoomTemp = result; }
                                    else if (reportParamMapID == "12880") { Date = result; }
                                    else if (reportParamMapID == "12888") { EquipmentNo = result; }

                                    /***************************Specified value**********************************************/

                                    else if (reportParamMapID == "12890") { SpecifiedDiameter["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12891") { SpecifiedDiameterNom["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12892") { SpecifiedArea["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12893") { SpecifiedBF["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12894") { SpecifiedTS["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12895") { SpecifiedL0["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12896") { SpecifiedL1["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12897") { SpecifiedEL["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12898") { SpecifiedMedianTS["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12899") { SpecifiedMedianEL["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12900") { SpecifiedMedianResistance["Value"] = new List<string> { result.Trim() }; }

                                    else if (reportParamMapID == "12901") { SpecifiedTemp["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12902") { SpecifiedKt["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12903") { SpecifiedResistivityat20["Value"] = new List<string> { result.Trim() }; }                                   
                                    else if (reportParamMapID == "12904") { SpecifiedConductivity["Value"] = new List<string> { result.Trim() }; }
                                    else if (reportParamMapID == "12905") { SpecifiedDensity["Value"] = new List<string> { result.Trim() }; }



                                 
                                }
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

                        // First header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Date</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Coil No.</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Specimen No.</strong></td>");

                        // Diameter spans 7 columns (1 to 6 + Nom)
                        sbDetails.Append("<td colspan='7' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Diameter</strong></td>");

                        // Next headers single columns with rowspan=2
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Area</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>BF</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>TS</strong></td>");

                        // Elongation spans 3 columns (you wrote 6 but colspan=3)
                        sbDetails.Append("<td colspan='3' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Elongation</strong></td>");
                        // Median spans 2 columns
                        sbDetails.Append("<td colspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Median</strong></td>");

                        // Remaining columns
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Resistance</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Temp.</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Kt</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Resistivity at 20ºC</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Conductivity</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Density</strong></td>");
                        sbDetails.Append("</tr>");

                        // Second header row
                        sbDetails.Append("<tr>");

                        // Diameter subcolumns 1–6 + Nom
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>1</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>2</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>3</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>4</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>5</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>6</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>Nom</strong></td>");

                        // Elongation subcolumns
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>L0</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>L1</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>EL.</strong></td>");

                        // Median subcolumns
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>TS</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; padding:3px;'><strong>EL.</strong></td>");
                        sbDetails.Append("</tr>");
                        // TABLE GENERATION CODE
                        int RowNumber = 1;

                        // Get all unique keys from SpecimenNo (this will have the most complete key set)
                        var allKeys = SpecimenNo.Keys.ToList();

                        // Create row data with proper grouping information
                        var rowData = new List<(string key, string date, string coilNo, string specimenNo, DateTime parsedDate, int coilIndex)>();
                        foreach (string key in allKeys)
                        {
                            string date = DateList.ContainsKey(key) && DateList[key].Count > 0 ? DateList[key][0] : "";
                            string specimenNo = SpecimenNo.ContainsKey(key) && SpecimenNo[key].Count > 0 ? SpecimenNo[key][0] : "";

                            // Extract coil index from key (e.g., "Coil1_Specimen2" -> 1)
                            int coilIndex = 1;
                            if (key.StartsWith("Coil"))
                            {
                                var parts = key.Split('_');
                                if (parts.Length > 0 && parts[0].Length > 4)
                                {
                                    int.TryParse(parts[0].Substring(4), out coilIndex);
                                }
                            }

                            // Get coil number from CoilNo dictionary
                            string coilKey = $"Coil{coilIndex}";
                            string coilNo = CoilNo.ContainsKey(coilKey) && CoilNo[coilKey].Count > 0 ? CoilNo[coilKey][0] : "";

                            // Parse date for proper sorting
                            DateTime parsedDate = DateTime.MinValue;
                            if (!string.IsNullOrEmpty(date))
                            {
                                DateTime.TryParseExact(date, "dd-MM-yyyy", null, DateTimeStyles.None, out parsedDate);
                            }

                            rowData.Add((key, date, coilNo, specimenNo, parsedDate, coilIndex));
                        }

                        // Sort by Date, then by Coil Index, then by Specimen Number
                        var sortedRowData = rowData
                            .OrderBy(r => r.parsedDate)
                            .ThenBy(r => r.coilIndex)
                            .ThenBy(r => int.TryParse(r.specimenNo, out int specNum) ? specNum : 0)
                            .ToList();

                        // Calculate rowspans for Date and Coil Number
                        var dateRowspans = new Dictionary<int, int>();
                        var coilRowspans = new Dictionary<int, int>();
                        var skipDateCells = new HashSet<int>();
                        var skipCoilCells = new HashSet<int>();

                        // Calculate Date rowspans
                        for (int i = 0; i < sortedRowData.Count; i++)
                        {
                            if (skipDateCells.Contains(i)) continue;

                            int span = 1;
                            string currentDate = sortedRowData[i].date;

                            for (int j = i + 1; j < sortedRowData.Count; j++)
                            {
                                if (sortedRowData[j].date == currentDate)
                                {
                                    span++;
                                    skipDateCells.Add(j);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            dateRowspans[i] = span;
                        }

                        // Calculate Coil Number rowspans
                        for (int i = 0; i < sortedRowData.Count; i++)
                        {
                            if (skipCoilCells.Contains(i)) continue;

                            int span = 1;
                            string currentDate = sortedRowData[i].date;
                            string currentCoil = sortedRowData[i].coilNo;

                            for (int j = i + 1; j < sortedRowData.Count; j++)
                            {
                                if (sortedRowData[j].date == currentDate && sortedRowData[j].coilNo == currentCoil)
                                {
                                    span++;
                                    skipCoilCells.Add(j);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            coilRowspans[i] = span;
                        }

                        // Generate table rows
                        for (int i = 0; i < sortedRowData.Count; i++)
                        {
                            string key = sortedRowData[i].key;
                            sbDetails.Append("<tr>");

                            // Date column with rowspan
                            if (!skipDateCells.Contains(i))
                            {
                                string date = sortedRowData[i].date;
                                int dateSpan = dateRowspans[i];
                                if (dateSpan > 1)
                                {
                                    sbDetails.Append($"<td rowspan='{dateSpan}' style='text-align:center; vertical-align:middle;'>{date}</td>");
                                }
                                else
                                {
                                    sbDetails.Append($"<td style='text-align:center;'>{date}</td>");
                                }
                            }

                            // Coil No column with rowspan
                            if (!skipCoilCells.Contains(i))
                            {
                                string coilNo = sortedRowData[i].coilNo;
                                int coilSpan = coilRowspans[i];
                                if (coilSpan > 1)
                                {
                                    sbDetails.Append($"<td rowspan='{coilSpan}' style='text-align:center; vertical-align:middle;'>{coilNo}</td>");
                                }
                                else
                                {
                                    sbDetails.Append($"<td style='text-align:center;'>{coilNo}</td>");
                                }
                            }

                            // Specimen No column (always individual)
                            string specimenNo = sortedRowData[i].specimenNo;
                            sbDetails.Append($"<td style='text-align:center;'>{specimenNo}</td>");

                            // Diameter columns (1-6)
                            var diameterValues = Diameter.ContainsKey(key) ? Diameter[key] : new List<string>();
                            for (int j = 0; j < 6; j++)
                            {
                                string diameterValue = j < diameterValues.Count ? diameterValues[j] : "";
                                sbDetails.Append($"<td style='text-align:center;'>{diameterValue}</td>");
                            }

                            // Diameter Nom column
                            string diameterNom = DiameterNom.ContainsKey(key) && DiameterNom[key].Count > 0 ? DiameterNom[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{diameterNom}</td>");

                            // Area column
                            string area = Area.ContainsKey(key) && Area[key].Count > 0 ? Area[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{area}</td>");

                            // BF column
                            string bf = BF.ContainsKey(key) && BF[key].Count > 0 ? BF[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{bf}</td>");

                            // TS column
                            string ts = TS.ContainsKey(key) && TS[key].Count > 0 ? TS[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{ts}</td>");

                            // Elongation L0 column
                            string elongationL0 = ElongationL0.ContainsKey(key) && ElongationL0[key].Count > 0 ? ElongationL0[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{elongationL0}</td>");

                            // Elongation L1 column
                            string elongationL1 = ElongationL1.ContainsKey(key) && ElongationL1[key].Count > 0 ? ElongationL1[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{elongationL1}</td>");

                            // Elongation EL column
                            string elongationEL = ElongationEL.ContainsKey(key) && ElongationEL[key].Count > 0 ? ElongationEL[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{elongationEL}</td>");

                            // Median TS column
                            string medianTS = MedianTS.ContainsKey(key) && MedianTS[key].Count > 0 ? MedianTS[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{medianTS}</td>");

                            // Median EL column
                            string medianEL = MedianEL.ContainsKey(key) && MedianEL[key].Count > 0 ? MedianEL[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{medianEL}</td>");

                            // Resistance column
                            string resistance = Resistance.ContainsKey(key) && Resistance[key].Count > 0 ? Resistance[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{resistance}</td>");

                            // Temperature column
                            string temperature = Temperature.ContainsKey(key) && Temperature[key].Count > 0 ? Temperature[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{temperature}</td>");

                            // Kt column
                            string ktValue = kt.ContainsKey(key) && kt[key].Count > 0 ? kt[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{ktValue}</td>");

                            // Resistivity at 20°C column
                            string resistivity = Resistivityat20C.ContainsKey(key) && Resistivityat20C[key].Count > 0 ? Resistivityat20C[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{resistivity}</td>");

                            // Conductivity column
                            string conductivity = Conductivity.ContainsKey(key) && Conductivity[key].Count > 0 ? Conductivity[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{conductivity}</td>");

                            // Density column
                            string density = Density.ContainsKey(key) && Density[key].Count > 0 ? Density[key][0] : "";
                            sbDetails.Append($"<td style='text-align:center;'>{density}</td>");

                            sbDetails.Append("</tr>");
                        }

                        // Add Specified row at the end
                        sbDetails.Append("<tr style='font-weight:bold;'>");

                        sbDetails.Append("<td colspan='2' style='text-align:right;'>Specified</td>");

                        // Diameter 1–6 as one value spanning 6 columns
                        string specifiedDiameterVal = SpecifiedDiameter.ContainsKey("Value") && SpecifiedDiameter["Value"].Count > 0
                                                    ? SpecifiedDiameter["Value"][0]
                                                    : "-";
                        sbDetails.Append($"<td colspan='6' style='text-align:center;'>{specifiedDiameterVal}</td>");

                        // Diameter Nom column
                        string specifiedNom = SpecifiedDiameterNom.ContainsKey("Value") && SpecifiedDiameterNom["Value"].Count > 0
                                            ? SpecifiedDiameterNom["Value"][0]
                                            : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedNom}</td>");

                        // Area
                        string specifiedArea = SpecifiedArea.ContainsKey("Value") && SpecifiedArea["Value"].Count > 0
                                             ? SpecifiedArea["Value"][0]
                                             : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedArea}</td>");

                        // BF
                        string specifiedBF = SpecifiedBF.ContainsKey("Value") && SpecifiedBF["Value"].Count > 0
                                           ? SpecifiedBF["Value"][0]
                                           : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedBF}</td>");

                        // TS
                        string specifiedTS = SpecifiedTS.ContainsKey("Value") && SpecifiedTS["Value"].Count > 0
                                           ? SpecifiedTS["Value"][0]
                                           : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedTS}</td>");

                        // Elongation (L0, L1, EL)
                        string specifiedL0 = SpecifiedL0.ContainsKey("Value") && SpecifiedL0["Value"].Count > 0
                                           ? SpecifiedL0["Value"][0]
                                           : "-";
                        string specifiedL1 = SpecifiedL1.ContainsKey("Value") && SpecifiedL1["Value"].Count > 0
                                           ? SpecifiedL1["Value"][0]
                                           : "-";
                        string specifiedEL = SpecifiedEL.ContainsKey("Value") && SpecifiedEL["Value"].Count > 0
                                           ? SpecifiedEL["Value"][0]
                                           : "-";

                        sbDetails.Append($"<td style='text-align:center;'>{specifiedL0}</td>");
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedL1}</td>");
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedEL}</td>");

                        // Median TS & EL
                        string specifiedMedianTS = SpecifiedMedianTS.ContainsKey("Value") && SpecifiedMedianTS["Value"].Count > 0
                                                 ? SpecifiedMedianTS["Value"][0]
                                                 : "-";
                        string specifiedMedianEL = SpecifiedMedianEL.ContainsKey("Value") && SpecifiedMedianEL["Value"].Count > 0
                                                 ? SpecifiedMedianEL["Value"][0]
                                                 : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedMedianTS}</td>");
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedMedianEL}</td>");

                        // Resistance
                        string specifiedResistance = SpecifiedMedianResistance.ContainsKey("Value") && SpecifiedMedianResistance["Value"].Count > 0
                                                   ? SpecifiedMedianResistance["Value"][0]
                                                   : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedResistance}</td>");

                        // Temperature
                        string specifiedTemperature = SpecifiedTemp.ContainsKey("Value") && SpecifiedTemp["Value"].Count > 0
                                                    ? SpecifiedTemp["Value"][0]
                                                    : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedTemperature}</td>");

                        // Kt
                        string specifiedKt = SpecifiedKt.ContainsKey("Value") && SpecifiedKt["Value"].Count > 0
                                           ? SpecifiedKt["Value"][0]
                                           : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedKt}</td>");

                        // Resistivity at 20ºC
                        string specifiedResistivity = SpecifiedResistivityat20.ContainsKey("Value") && SpecifiedResistivityat20["Value"].Count > 0
                                                    ? SpecifiedResistivityat20["Value"][0]
                                                    : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedResistivity}</td>");

                        // Conductivity
                        string specifiedConductivity = SpecifiedConductivity.ContainsKey("Value") && SpecifiedConductivity["Value"].Count > 0
                                                     ? SpecifiedConductivity["Value"][0]
                                                     : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedConductivity}</td>");

                        // Density
                        string specifiedDensity = SpecifiedDensity.ContainsKey("Value") && SpecifiedDensity["Value"].Count > 0
                                                ? SpecifiedDensity["Value"][0]
                                                : "-";
                        sbDetails.Append($"<td style='text-align:center;'>{specifiedDensity}</td>");

                        // Close Specified row
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
        <td></td><td></td>
    </tr>
</table>");


                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_CopperandAluminium Wire.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_CopperandAluminium Wire -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_CopperandAluminium Wire -" + SampleID + ".htm"));
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