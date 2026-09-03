using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Voltage4Hours_GR : System.Web.UI.Page
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
                        List<string> Duration = new List<string>();
                        List<string> Current = new List<string>();
                        List<string> Measured = new List<string>();

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

                        string voltagetest = "";
                        string durationhours = "";
                        string frequency = "";


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
                                if (dr["ReportParamMapID"].ToString() == "14128")
                                {
                                    Duration = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "14129")
                                {
                                    Current = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "14130")
                                {
                                    Measured = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "14125") { voltagetest = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14126") { durationhours = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14127") { frequency = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "14111") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14112") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14118") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14113") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14119") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14120") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14117") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14122") { EquipmentNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14115") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14121") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14116") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14114") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17682") { requirement = dr["Result"].ToString(); }

                            }
                            string[] reqParts = requirement.Split(',');
                            string reqPart1 = reqParts.Length > 0 ? reqParts[0].Trim() : "";
                            string reqPart2 = reqParts.Length > 1 ? reqParts[1].Trim() : "";

                            sbHeader.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse;'>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Type</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Type + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Ref Spec.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RefSpec + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Size</td>
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

                            // Start of the table definition remains unchanged
                            sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top: 10px;'>
                            <tr>
                                <td colspan='10' style='border: 1px solid black; padding: 5px;'><b>Characteristic of data</b></td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; width:17%;'>Voltage test</td>
                                <td colspan='9' style='border: 1px solid black;text-align: center; vertical-align: middle; padding: 5px;'>" + voltagetest + @" kV</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; width:17%;'>Duration</td>
                                <td colspan='9' style='border: 1px solid black;text-align: center;vertical-align: middle; padding: 5px;'>" + durationhours + @" hours</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; width:17%;'>Frequency</td>
                                <td colspan='9' style='border: 1px solid black;text-align: center;vertical-align: middle; padding: 5px;'>" + frequency + @" Hz</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;vertical-align: middle; background-color: #d3d3d3; width:17%;'>
                                   <b> Item test </b>
                                </td>
                                <td colspan='8' style='border: 1px solid black; padding: 5px; text-align: center;vertical-align: middle; background-color: #d3d3d3;'>
                                   <b> Voltage to applied to core to core<br>and<br>Earth connected to metal screen </b>
                                </td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;vertical-align: middle; background-color: #d3d3d3; width:21%;'>
                                  <b> Requirement </b>
                                </td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; width:17%;'>Duration<br>(minutes)</td>");

                            for (int i = 0; i < Duration.Count; i++)
                            {
                                sbDetails.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + Duration[i] + @"</td>");
                            }

                            sbDetails.Append(@"<td rowspan='2' style='border: 1px solid black; padding: 5px; vertical-align: middle; width:21%;text-align: center;'>" + reqPart1 + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; width:17%;'>Current<br>(mA)</td>");

                            for (int i = 0; i < Current.Count; i++)
                            {
                                sbDetails.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + Current[i] + @"</td>");
                            }

                            sbDetails.Append(@"
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; width:17%;'>Measured</td>");

                            for (int i = 0; i < Measured.Count; i++)
                            {
                                sbDetails.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + Measured[i] + @"</td>");
                            }
                            sbDetails.Append(@"<td style='border: 1px solid black; padding: 5px; vertical-align: middle; width:21%;text-align: center;'>" + reqPart2 + @"</td>
                   
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

                        body = objEmail.PrepareMailBodyWith("Voltage4Hours_GR.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/Voltage4Hours_GR-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/Voltage4Hours_GR-" + SampleID + ".html"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
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