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
    public partial class GenerateTestResult : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); string sampleUID = ""; int CloneCRFSampleID = 0;
         DAL.mailer mailer = new DAL.mailer();
        int freqCounter = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {

                int SampleID;
                string SampleReceivedDate = "";
                string ProductCategory = "";
                string AllotedTo = "";
                string AllotedBy = "";
                string BatchNo = "";
                string DateOfRequest = "";
                int EmployeeId;
                string InquiryId = "";
                string Description = "";
                string SampleName = "";
                string TestMethod = "";
                string TestResult = "";
                string TestParameter = "";
                string Id = "";
                string DepartmentId = "";
                string sampleIDSeries = "";
                string sampleIDSeriesAll = "";
                string UnitOfMeasurement = "";
                string Specification = "";
                string Sr = "";
                string Temp = "";
                string RH = "";
                int Dept = 0;
                string TestsRequested = "";
                string SampleDescription = "";
                DateTime AllocationDate;
                string AllocatedDate = "";
                string body = ""; string str = ""; string filename = ""; string html = "";

                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                DataTable dt5 = new DataTable();
                DataTable dt6 = new DataTable();
                DataTable dt7 = new DataTable();
                string AllocationID = Session["AllocationId"].ToString();
                dt = _cm.GetAllocationDetails(AllocationID);
                if (dt.Rows.Count > 0)
                {
                    SampleID = Convert.ToInt32(dt.Rows[0]["SampleId"].ToString());
                    EmployeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"].ToString());
                    AllocationDate = Convert.ToDateTime(dt.Rows[0]["Created"].ToString());
                    AllocatedDate = AllocationDate.ToString("dd/MMM/yyyy");
                    dt2 = _cm.getSampleDetails(SampleID);
                    dt3 = _cm.GetCustomerInquiryDetails(SampleID);
                    dt5 = _cm.GetEmployeeDetails(EmployeeId);
                    if (dt2.Rows.Count > 0)
                    {
                        DateTime ReceivedDate = Convert.ToDateTime(dt2.Rows[0]["ReceivedDate"].ToString());
                        DateTime RequestDate = Convert.ToDateTime(dt2.Rows[0]["Created"].ToString());
                        SampleReceivedDate = ReceivedDate.ToString("dd/MMM/yyyy HH':'mm':'ss");
                        DateOfRequest = RequestDate.ToString("dd/MMM/yyyy HH':'mm':'ss");
                        Id = dt2.Rows[0]["SampleId"].ToString();
                        InquiryId = dt2.Rows[0]["EnquiryId"].ToString();
                        CloneCRFSampleID = Convert.ToInt32(dt2.Rows[0]["CloneCRFSampleID"].ToString());
                    }
                    if (dt3.Rows.Count > 0)
                    {
                        string ProductCategoryId = dt3.Rows[0]["ProductCategoryId"].ToString();
                        Dept = Convert.ToInt32(dt3.Rows[0]["DepartmentID"].ToString());
                        dt4 = _cm.GetProductGroupDetails(ProductCategoryId);
                        if (dt4.Rows.Count > 0)
                        {
                            ProductCategory = dt4.Rows[0]["productgroupname"].ToString();
                        }
                    }
                    if (dt5.Rows.Count > 0)
                    {
                        AllotedTo = dt5.Rows[0]["EmployeeName"].ToString();
                        DepartmentId = dt5.Rows[0]["DepartmentId"].ToString();
                        dt7 = _cm.GetEmployeeDetails(DepartmentId, EmployeeId);
                        if (dt7.Rows.Count > 0)
                        {
                            AllotedBy = dt7.Rows[0]["SupervisorName"].ToString();
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    mailer objEmail = new mailer();
                    StringReader sr = new StringReader(sb.ToString());
                    Session["CloneCRFSampleID"] = CloneCRFSampleID;

                    switch (Dept)
                    {
                        case 1:
                            dt6.Columns.AddRange(new DataColumn[4] {
                                    new DataColumn("sampleIDSeries", typeof(string)),
                                    new DataColumn("Temp",typeof(string)),
                                    new DataColumn("RH",typeof(string)),
                                    new DataColumn("SampleName",typeof(string))
                                });
                            dt6 = _cm.GetCRFListForAllocationMocon(Session["SampleIDSeriesAll"].ToString());
                            SampleDescription = dt6.Rows[0]["SampleName"].ToString();
                            BatchNo = dt6.Rows[0]["BatchNo"].ToString();
                            TestsRequested = dt6.Rows[0]["TestsRequested"].ToString();
                            freqCounter = dt6.Rows.Count;                            

                            //Table start.
                            sb.Append("<table border='1' style='border-spacing:0px; width:100%'>");

                            //Adding HeaderRow.
                            sb.Append("<tr>");
                            sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Sl#</b></td>");
                            sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Lab Sample ID No.</b></td>");
                            sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Analysis Date</b></td>");
                            sb.Append("<td colspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>Test Condition</b></td>");
                            sb.Append("<td colspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>Results</b></td>");
                            sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>Report No./ date</b></td>");
                            sb.Append("<td rowspan='2' style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Remarks</b></td>");
                            sb.Append("</tr>");
                            sb.Append("<tr>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>Temp. (°C)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>% RH</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>WVTR(g/m<span>&#178;</span>•day)</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>OTR(cc/m<span>&#178;</span>•day)</b></td>");
                            sb.Append("</tr>");

                            for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                            {
                                sb.Append("<tr>");
                                {
                                    sampleIDSeries = dt6.Rows[freqCount - 1]["sampleIDSeries"].ToString();
                                    sampleIDSeriesAll += sampleIDSeries + ",";
                                    Temp = dt6.Rows[freqCount - 1]["Temp"].ToString();
                                    RH = dt6.Rows[freqCount - 1]["RH"].ToString();
                                    TestMethod += dt6.Rows[freqCount - 1]["TestMethod"].ToString() + ",";

                                    sb.Append("<td class='no' style='width:5%; border: 1px solid #2F4F4F;'>" + freqCount.ToString("00") + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'>" + sampleIDSeries + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'></td>");
                                    sb.Append("<td style='width:20%; border: 1px solid #2F4F4F;'>" + Temp + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'>" + RH + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'></td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'></td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'></td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'></td>");
                                }
                                sb.Append("</tr>");
                            }
                            //Table end.
                            sb.Append("</table>");
                            sampleIDSeriesAll = string.Join(" ", sampleIDSeriesAll.Split(',').Distinct());
                            body = objEmail.PrepareMailBodyWith("TestResultMocon.html", "{SampleId}", sampleIDSeriesAll, "{SampleReceivedDate}", SampleReceivedDate, "{ProductCategory}", ProductCategory, "{AllotedTo}", AllotedTo, "{AllotedBy}", AllotedBy, "{SampleDescription}", SampleDescription, "{AllocationNumber}", AllocationID.ToString(), "{Date}", SampleReceivedDate, "{BatchNo}", BatchNo, "{TestDetails}", sb.ToString(), "{TestsRequested}", TestsRequested, "{TestMethod}", TestMethod, "{SampleDescription}", SampleDescription, "{AllocationDate}", AllocationDate.ToString());
                            str = body.ToString();
                            for (int i = 0; i < dt6.Rows.Count; i++)
                            {
                                AllocationID = dt6.Rows[i]["AllocationID"].ToString();
                                sampleUID = dt6.Rows[i]["sampleUID"].ToString();
                                filename = Server.MapPath("//TestResult/TestResult-" + AllocationID + sampleUID + ".htm");
                                System.IO.File.WriteAllText(@filename, str);
                            }

                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestResult/TestResult-" + AllocationID + sampleUID + ".htm"));
                            divAllocation.InnerHtml = html;
							GC.Collect();
                            Response.Redirect("~/SampleListMocon.aspx");
                            break;
                        case 3:
                        case 4:
                            dt6.Columns.AddRange(new DataColumn[7] {
                                    new DataColumn("SampleName", typeof(string)),
                                    new DataColumn("BatchNo",typeof(string)),
                                    new DataColumn("TestParameter",typeof(string)),
                                    new DataColumn("TestMethod",typeof(string)),
                                    new DataColumn("TestResult",typeof(string)),
                                    new DataColumn("Unit",typeof(string)),
                                    new DataColumn("Specification",typeof(string))
                                });
                            dt6 = _cm.GetCRFSample(CloneCRFSampleID.ToString());
                            BatchNo = dt6.Rows[0]["BatchNo"].ToString();
                            sampleIDSeries = dt6.Rows[0]["sampleIDSeries"].ToString();
                            TestsRequested = dt6.Rows[0]["TestsRequested"].ToString();
                            TestMethod = dt6.Rows[0]["TestMethod"].ToString();
                            TestResult = dt6.Rows[0]["TestResult"].ToString();
                            freqCounter = dt6.Rows.Count;

                            //Table start.
                            sb.Append("<table border='1' style='border-spacing:0px; width:100%' cellpadding='5'>");

                            //Adding HeaderRow.
                            sb.Append("<tr>");

                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Sl#</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Test Parameter</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Test Method</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Results</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>Unit</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Specification</b></td>");

                            sb.Append("</tr>");

                            for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                            {
                                sb.Append("<tr>");
                                {
                                    SampleName = dt6.Rows[freqCount - 1]["SampleName"].ToString();
                                    BatchNo = dt6.Rows[freqCount - 1]["BatchNo"].ToString();
                                    TestMethod = dt6.Rows[freqCount - 1]["TestMethod"].ToString();
                                    TestResult = dt6.Rows[freqCount - 1]["TestResult"].ToString();
                                    TestParameter = dt6.Rows[freqCount - 1]["TestParameter"].ToString();
                                    Description = dt6.Rows[freqCount - 1]["DescriptionOfTest"].ToString();
                                    UnitOfMeasurement = dt6.Rows[freqCount - 1]["UnitOfMeasurement"].ToString();
                                    Specification = dt6.Rows[freqCount - 1]["Specification"].ToString();
                                    sb.Append("<td style='width:5%'>" + freqCount.ToString("00") + "</td>");
                                    sb.Append("<td style='width:25%'>" + TestParameter + "</td>");
                                    sb.Append("<td style='width:25%'>" + TestMethod + "</td>");
                                    sb.Append("<td style='width:25%'>" + TestResult + "</td>");
                                    sb.Append("<td style='width:20%'>" + UnitOfMeasurement + "</td>");
                                    sb.Append("<td style='width:25%'>" + Specification + "</td>");
                                }
                                sb.Append("</tr>");
                            }
                            //Table end.
                            sb.Append("</table>");
                            body = objEmail.PrepareMailBodyWith("TestResult.html", "{SampleId}", sampleIDSeries, "{SampleReceivedDate}", SampleReceivedDate, "{ProductCategory}", ProductCategory, "{AllotedTo}", AllotedTo, "{AllotedBy}", AllotedBy, "{Sample Description}", Description, "{AllocationNumber}", AllocationID.ToString(), "{Date}", SampleReceivedDate, "{BatchNo}", BatchNo, "{TestsRequested}",TestsRequested, "{TestMethod}",TestMethod, "{TestDetails}", sb.ToString(), "{SampleName}",SampleName);
                            str = body.ToString();
                            filename = Server.MapPath("//TestResult/TestResult-" + AllocationID + Id + ".htm");
                            System.IO.File.WriteAllText(@filename, str);
                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestResult/TestResult-" + AllocationID + Id + ".htm"));
                            divAllocation.InnerHtml = html;
							GC.Collect();
                            break;
                        case 2:
                            dt6.Columns.AddRange(new DataColumn[6] {
                                    new DataColumn("SampleName", typeof(string)),
                                    new DataColumn("TestParameter",typeof(string)),
                                    new DataColumn("TestMethod",typeof(string)),
                                    new DataColumn("TestResult",typeof(string)),
                                    new DataColumn("Unit",typeof(string)),
                                    new DataColumn("Specification",typeof(string))
                                });
                            dt6 = _cm.GetCRFSample(CloneCRFSampleID.ToString());
                            //BatchNo = dt6.Rows[0]["BatchNo"].ToString();
                            sampleIDSeries = dt6.Rows[0]["sampleIDSeries"].ToString();
                            TestsRequested = dt6.Rows[0]["TestsRequested"].ToString();
                            freqCounter = dt6.Rows.Count;

                            //Table start.
                            sb.Append("<table border='1' style='border-spacing:0px; width:100%' cellpadding='5'>");

                            //Adding HeaderRow.
                            sb.Append("<tr>");

                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:5%'><b>Sl#</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Test Parameter</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Test Method</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Results</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:20%'><b>Unit</b></td>");
                            sb.Append("<td style='text-align:center;background-color:#DDDDDD; border: 1px solid #2F4F4F;width:25%'><b>Specification</b></td>");

                            sb.Append("</tr>");

                            for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                            {
                                sb.Append("<tr>");
                                {
                                    Sr = freqCount.ToString();
                                    SampleName = dt6.Rows[freqCount - 1]["SampleName"].ToString();
                                    //BatchNo = dt6.Rows[freqCount - 1]["BatchNo"].ToString();
                                    TestMethod = dt6.Rows[freqCount - 1]["TestMethod"].ToString();
                                    TestResult = dt6.Rows[freqCount - 1]["TestResult"].ToString();
                                    TestParameter = dt6.Rows[freqCount - 1]["TestParameter"].ToString();
                                    UnitOfMeasurement = dt6.Rows[freqCount - 1]["UnitOfMeasurement"].ToString();
                                    sb.Append("<td style='width:5%; border: 1px solid #2F4F4F;'>" + freqCount.ToString("00") + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'>" + TestParameter + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'>" + TestMethod + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'>" + TestResult + "</td>");
                                    sb.Append("<td style='width:20%; border: 1px solid #2F4F4F;'>" + UnitOfMeasurement + "</td>");
                                    sb.Append("<td style='width:25%; border: 1px solid #2F4F4F;'>" + Specification + "</td>");
                                }
                                sb.Append("</tr>");
                            }
                            //Table end.
                            sb.Append("</table>");
                            body = objEmail.PrepareMailBodyWith("TestResultPTI.html", "{SampleId}", sampleIDSeries, "{SampleReceivedDate}", SampleReceivedDate, "{ProductCategory}", ProductCategory, "{AllotedTo}", AllotedTo, "{AllotedBy}", AllotedBy, "{Sample Description}", Description, "{AllocationNumber}", AllocationID.ToString(), "{Date}", SampleReceivedDate, "{BatchNo}", BatchNo, "{TestsRequested}", TestsRequested, "{SampleName}", SampleName, "{TestMethod}", TestMethod, "{TestDetails}", sb.ToString());
                            str = body.ToString();
                            filename = Server.MapPath("//TestResult/TestResult-" + AllocationID + Id + ".htm");
                            System.IO.File.WriteAllText(@filename, str);
                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestResult/TestResult-" + AllocationID + Id + ".htm"));
                            divAllocation.InnerHtml = html;
							GC.Collect();
                            Response.Redirect("~/SampleList.aspx");
                            break;
                        default: break;
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