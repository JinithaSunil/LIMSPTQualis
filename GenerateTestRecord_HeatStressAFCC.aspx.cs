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
    public partial class GenerateTestRecord_HeatStressAFCC : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int SampleID = 0; int ReportId = 0;
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
                        string requirement = "";

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

                        string[] LengthOfSample = new string[2];
                        string[] ActualDiameterOfCore = new string[2];
                        string[] ActualCrosssectionalArea = new string[2];
                        string[] TemperatureofTgTest = new string[2];
                        string[] TensionOf25pcofMinUltimateTensileStrength = new string[2];
                        string[] HeatStressdurationfor25pcofRTS = new string[2];
                        string[] Visualafterexposure = new string[2];
                        string[] Tensilemin95OfRTS = new string[2];


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


                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                switch (dr["ReportParamMapID"].ToString())
                                {
                                    case "12259": LengthOfSample = dr["Result"].ToString().Split('|'); break;
                                    case "12260": ActualDiameterOfCore = dr["Result"].ToString().Split('|'); break;
                                    case "12261": ActualCrosssectionalArea = dr["Result"].ToString().Split('|'); break;
                                    case "12262": TemperatureofTgTest = dr["Result"].ToString().Split('|'); break;
                                    case "12263": TensionOf25pcofMinUltimateTensileStrength = dr["Result"].ToString().Split('|'); break;
                                    case "12264": HeatStressdurationfor25pcofRTS = dr["Result"].ToString().Split('|'); break;
                                    case "12265": Visualafterexposure = dr["Result"].ToString().Split('|'); break;
                                    case "12266": Tensilemin95OfRTS = dr["Result"].ToString().Split('|'); break;

                                    case "12245": Type = dr["Result"].ToString(); break;
                                    case "12246": Size = dr["Result"].ToString(); break;
                                    case "12247": Voltage = dr["Result"].ToString(); break;
                                    case "12248": Date = dr["Result"].ToString(); break;
                                    case "12249": RoomNo = dr["Result"].ToString(); break;
                                    case "12250": RoomTemp = dr["Result"].ToString(); break;
                                    case "12251": RefSpec = dr["Result"].ToString(); break;
                                    case "12252": TestMethodNo = dr["Result"].ToString(); break;
                                    case "12253": JobOrderNo = dr["Result"].ToString(); break;
                                    case "12254": WorkSheetNo = dr["Result"].ToString(); break;
                                    case "12255": RoomHumidity = dr["Result"].ToString(); break;
                                    case "12256": EquipmentNo = dr["Result"].ToString(); break;
                                    case "12257": requirement = dr["Result"].ToString(); break;
                                }
                            }
                            string[] reqParts = requirement.Split(',');
                            string reqPart1 = reqParts.Length > 0 ? reqParts[0].Trim() : "";
                            string reqPart2 = reqParts.Length > 1 ? reqParts[1].Trim() : "";
                            string reqPart3 = reqParts.Length > 2 ? reqParts[2].Trim() : "";
                            string reqPart4 = reqParts.Length > 3 ? reqParts[3].Trim() : "";

                            sbHeader.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse;'>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Type</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Type + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Ref Spec.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RefSpec + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Diameter</td>
                                <td style='border: 1px solid black; padding: 5px;width:41%'>" + Size + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Test Method No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + TestMethodNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Voltage</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Voltage + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Job Order No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + JobOrderNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Date</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Date + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Work Sheet No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + WorkSheetNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Room No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomNo + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Room Humidity</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomHumidity + @" %</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Room Temp.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomTemp + @" °C</td>
                                <td style='border: 1px solid black; padding: 5px;'>Equipment No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + EquipmentNo + @"</td>
                            </tr>
                        </table>");

                            // Start of the table definition
                            sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top: 10px;'>
                            <tr>
                            <th width='10%' style='border: 1px solid black; padding: 5px;'>No</th>
                            <th width='30%' style='border: 1px solid black; padding: 5px;'>Description test</th>
                            <th width='20%' style='border: 1px solid black; padding: 5px;'>unit</th>
                            <th width='20%' style='border: 1px solid black; padding: 5px;'>Specified</th>
                            <th width='20%' style='border: 1px solid black; padding: 5px;'>Value test</th>
                            </tr>
                             <tr>
 				                <td style='border: 1px solid black; padding: 5px;'>1</td>
                                <td style='border: 1px solid black; padding: 5px;'>Length of sample</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + LengthOfSample[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + LengthOfSample[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + LengthOfSample[2] + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>2</td>
                                <td style='border: 1px solid black; padding: 5px;'>Actual Diameter of core</td>
                                <td style='border: 1px solid black; padding: 5px;width:41%'>" + ActualDiameterOfCore[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + ActualDiameterOfCore[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + ActualDiameterOfCore[2] + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>3</td>
                                <td style='border: 1px solid black; padding: 5px;'>Actual Cross sectional area</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + ActualCrosssectionalArea[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + ActualCrosssectionalArea[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + ActualCrosssectionalArea[2] + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>4</td>
                                <td style='border: 1px solid black; padding: 5px;'>Temperature of Tg -Test</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + TemperatureofTgTest[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + TemperatureofTgTest[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + TemperatureofTgTest[2] + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>5</td>
                                <td style='border: 1px solid black; padding: 5px;'>Tension of 25% of Min. Ultimate Tensile Strength</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Tensilemin95OfRTS[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Tensilemin95OfRTS[1] + @" %</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Tensilemin95OfRTS[2] + @" %</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>6</td>
                                <td style='border: 1px solid black; padding: 5px;'>Heat/Stress duration for 25 % of RTS</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + HeatStressdurationfor25pcofRTS[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + HeatStressdurationfor25pcofRTS[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + HeatStressdurationfor25pcofRTS[2] + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>7</td>
                                <td style='border: 1px solid black; padding: 5px;'>Visual after exposure</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Visualafterexposure[0] + @" </td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Visualafterexposure[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Visualafterexposure[2] + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>8</td>
                                <td style='border: 1px solid black; padding: 5px;'>Tensile min. 95 % of RTS</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Tensilemin95OfRTS[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Tensilemin95OfRTS[1] + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Tensilemin95OfRTS[2] + @"</td>
                            </tr>
                                </table>");

                            // Replace sbFooter with a signature section like in the image
                            sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                                <tr>
                                    <td rowspan='2'  style=' vertical-align: middle' ><b>PT Qualis Indonesia</b></td>
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
                        }
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_VoltageTestafterimpulse.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_VoltageTestafterimpulse-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_VoltageTestafterimpulse-" + SampleID + ".html"));
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
    }
}
