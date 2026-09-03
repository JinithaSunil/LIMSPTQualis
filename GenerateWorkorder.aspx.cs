using LIMS.DAL;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using DataTable = System.Data.DataTable;

namespace LIMS
{
    public partial class GenerateWorkorder : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int SampleID = 0; int InquiryID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["i"] != null)
                {
                    Session["InquiryID"] = Request.QueryString["i"].ToString();
                }
                if (Request.QueryString["s"] != null)
                {
                    Session["SampleID"] = Request.QueryString["s"].ToString();
                }
                if (Session["SampleID"] != null && Session["InquiryID"] != null)
                {
                    SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    InquiryID = Convert.ToInt32(Session["InquiryID"].ToString());
                    DataTable dtcl = _cm.GetCRFCloneList_LED(SampleID);
                    int cloneCRFSampleID = Convert.ToInt32(dtcl.Rows[0]["CloneCRFSampleID"].ToString());

                    /* missing data */
                    DataTable dtcldep = _cm.GetCRFCloneListDep_LED(cloneCRFSampleID);
                    int departmentID = Convert.ToInt32(dtcldep.Rows[0]["DepartmentID"].ToString());
                    if (Session["Usertype"] != null)
                    {
                        // Get employee ID from session
                        string EmployeeID = Session["EmployeeID"] != null ? Session["EmployeeID"].ToString() : "";

                        // Get department name from database
                        string DepartmentName = "";
                        if (!string.IsNullOrEmpty(EmployeeID))
                        {
                            // Get employee details including department name using the GetEmployeeDetailsWO method
                            DataTable dtEmployee = _cm.GetEmployeeDetailsWO(EmployeeID);

                            if (dtEmployee.Rows.Count > 0)
                            {
                                DepartmentName = dtEmployee.Rows[0]["EmployeeDepartmentName"].ToString();
                            }
                        }

                        string ReceivedDate = "";
                        string Priority = "";
                        string TargetDate = "";
                        string Buyer = "";
                        string WONo = "";
                        string SampleDescription = "";
                        string SampleCode = "";
                        string Remark = "";
                        string RequestNo = "";
                        string Customer = "";
                        string Samplequantity = "";
                        string customeraddress = "";
                        string Testparameter = "";

                        string CompanyName = "";
                        string CompanyAddress = "";
                        string FactoryName = "";
                        string FactoryAddress = "";



                        DataTable dtRecordDetails = new DataTable();

                        StringBuilder sbHeader = new StringBuilder();
                        StringBuilder sbDetails = new StringBuilder();
                        StringBuilder sbIndolang = new StringBuilder();
                        StringBuilder sbFooter = new StringBuilder();
                        StringBuilder sbCustomer = new StringBuilder();
                        StringBuilder sb = new StringBuilder();
                        mailer objEmail = new mailer();
                        StringReader sr = new StringReader(sb.ToString());
                        string body = ""; string str = ""; string filename = ""; string html = "";


                        DataTable dtEnquiry = _cm.GetCustomerInquiryDetails(SampleID);
                        if (dtEnquiry != null)
                        {
                            departmentID = Convert.ToInt32(dtEnquiry.Rows[0]["DepartmentID"]);
                            string reportFormatNumber = "";
                            string ApplicantName = "";
                            string ApplicantAddress = ""; string ApplicantAddress1 = "";

                            if (departmentID == 4009 || departmentID == 3006 || departmentID == 1 || departmentID == 3 || departmentID == 5020)
                            {
                                dtRecordDetails = _cm.getworkorderdetails_Transport(InquiryID, SampleID);
                                DepartmentName = dtRecordDetails.Rows[0]["DepartmentName"].ToString();
                                reportFormatNumber = dtRecordDetails.Rows[0]["ReportFormatNumber"].ToString();
                                ApplicantName = dtRecordDetails.Rows[0]["Customer"].ToString();
                                ApplicantAddress1 = dtRecordDetails.Rows[0]["CustomerAddress"].ToString();
                                ApplicantAddress = ApplicantAddress1.Replace(",,", ",");

                            }
                            else if (departmentID == 4006)
                            {
                                dtRecordDetails = _cm.getworkorderdetailsPV(InquiryID, SampleID);
                                DepartmentName = dtRecordDetails.Rows[0]["DepartmentName"].ToString();
                                reportFormatNumber = dtRecordDetails.Rows[0]["ReportFormatNumber"].ToString();
                            }
                            else if (departmentID == 4008)
                            {
                                dtRecordDetails = _cm.getworkorderdetails_HomeApp(InquiryID, SampleID);
                                DepartmentName = dtRecordDetails.Rows[0]["DepartmentName"].ToString();
                                reportFormatNumber = dtRecordDetails.Rows[0]["ReportFormatNumber"].ToString();
                            }
                            else
                            {
                                dtRecordDetails = _cm.getworkorderdetails(InquiryID, SampleID);
                            }
                            if (dtRecordDetails.Rows.Count > 0)
                            {
                                // Get the first row from result set
                                DataRow row = dtRecordDetails.Rows[0];

                                // Extract all required fields directly using column names
                                ReceivedDate = row["ReceivedDate"] != DBNull.Value ? row["ReceivedDate"].ToString() : "";
                                if (ReceivedDate != "") { ReceivedDate = Convert.ToDateTime(ReceivedDate).ToString("dd-MM-yyyy"); }
                                Priority = row["Priority"] != DBNull.Value ? row["Priority"].ToString() : "";
                                Buyer = row["Buyer"] != DBNull.Value ? row["Buyer"].ToString() : "";
                                TargetDate = row["TargetDate"] != DBNull.Value ? row["TargetDate"].ToString() : "";
                                if (TargetDate != "") { TargetDate = Convert.ToDateTime(TargetDate).ToString("dd-MM-yyyy"); }
                                WONo = "";
                                SampleDescription = row["SampleDescription"] != DBNull.Value ? row["SampleDescription"].ToString() : "";
                                SampleCode = row["SampleCode"] != DBNull.Value ? row["SampleCode"].ToString() : "";
                                string WOCode = string.IsNullOrEmpty(SampleCode) ? "" : SampleCode.Replace("smp/", "WO/");
                                WONo = WOCode;
                                Remark = row["Remark"] != DBNull.Value ? row["Remark"].ToString() : "";
                                RequestNo = string.IsNullOrEmpty(SampleCode) ? "" : SampleCode.Replace("smp/", "R/");
                                Customer = row["Customer"] != DBNull.Value ? row["Customer"].ToString() : "";

                                CompanyName = row["Buyer"] != DBNull.Value ? row["Buyer"].ToString() : "";
                                CompanyAddress = row["CustomerAddress"] != DBNull.Value ? row["CustomerAddress"].ToString() : "";
                                CompanyAddress = CompanyAddress.Replace(",,", ",");

                                FactoryName = row["FactoryName"] != DBNull.Value ? row["FactoryName"].ToString() : "";
                                FactoryAddress = row["FactoryAddress"] != DBNull.Value ? row["FactoryAddress"].ToString() : "";
                                FactoryAddress = FactoryAddress.Replace(",,", ",");


                                // Extract additional fields
                                Samplequantity = row["SampleQuantity"] != DBNull.Value ? row["SampleQuantity"].ToString() : "";
                                customeraddress = row["CustomerAddress"] != DBNull.Value ? row["CustomerAddress"].ToString() : "";
                                // Testparameter = row["TestParameters"] != DBNull.Value ? row["TestParameters"].ToString() : "";

                                sbHeader.Append(@"<table style='width: 100%; border: none; border-collapse: collapse; font-family: Arial, sans-serif;'>
                        <tr>
                            <td colspan='4' style='padding: 5px; font-weight: bold;'>Lab  : " + DepartmentName + @"</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px; width: 15%;font-weight: bold;'>Received Date  </td>
                            <td style='padding: 5px; width: 35%;'><b>:</b> " + ReceivedDate + @"</td>
                            <td style='padding: 5px; width: 15%;font-weight: bold;'>Priority </td>
                            <td style='padding: 5px; width: 35%;'><b>:</b> " + Priority + @"</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px;font-weight: bold;'>Target Date  </td>
                            <td style='padding: 5px;'><b>:</b> " + TargetDate + @"</td>
                            <td style='padding: 5px;font-weight: bold;'>Buyer  </td>
                            <td style='padding: 5px;'><b>:</b> " + Buyer + @"</td>
                        </tr>
                        <tr></tr>
                        <tr>
                            <td style='padding: 5px;font-weight: bold;'>WO No. </td>
                            <td style='padding: 5px;'><b>:</b> " + WONo + @"</td>
                            <td style='padding: 5px;font-weight: bold;'>Sample Description  </td>
                            <td style='padding: 5px;'><b>:</b> " + SampleDescription + @"</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px;font-weight: bold;'>Sample Code  </td>
                            <td style='padding: 5px;'><b>:</b> " + SampleCode + @"</td>
                            <td style='padding: 5px;'></td>
                            <td style='padding: 5px;'></td>
                        </tr>
                        <tr>
                            <td style='padding: 5px;font-weight: bold;'>Remark  </td>
                            <td style='padding: 5px;' colspan='3'><b>:</b> " + Remark + @"</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px;font-weight: bold;'>Request No.  </td>
                            <td style='padding: 5px;'><b>:</b> " + RequestNo + @"</td>
                            <td style='padding: 5px;'></td>
                            <td style='padding: 5px;'></td>
                        </tr>
                        <tr>
                            <td style='padding: 5px;font-weight: bold;'>Customer  </td>
                            <td style='padding: 5px;' colspan='3'><b>:</b> " + Customer + @"</td>
                        </tr>
                    </table>");

                                DataTable dtTestDetails = _cm.GetTestDetailsWO(InquiryID, SampleID);

                                // Replace sbDetails with this new section to match the characteristic data table
                                sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top: 10px;'>
                        <tr style='background-color: #f2f2f2;'>
                            <th style='border: 1px solid black; padding: 5px;width: 35%; text-align: center;'>Tests</th>
                            <th style='border: 1px solid black; padding: 5px;width: 25%; text-align: center;'>Method</th>
                            <th style='border: 1px solid black; padding: 5px;width: 10%; text-align: center;'>Part</th>
                            <th style='border: 1px solid black; padding: 5px;width: 15%; text-align: center;'>Requirement</th>
                            <th style='border: 1px solid black; padding: 5px;width: 15%; text-align: center;'>Remark</th>
                        </tr>");

                                // Loop through test details and add rows to the table
                                if (dtTestDetails.Rows.Count > 0)
                                {
                                    foreach (DataRow testRow in dtTestDetails.Rows)
                                    {
                                        string testName = "";

                                        // Check if department is Microbiology Lab
                                        if (DepartmentName == "Microbiology Lab")
                                        {// Use ReportName column for Microbiology Lab
                                            testName = testRow["ReportName"] != DBNull.Value ? testRow["ReportName"].ToString() : "";
                                        }
                                        else if (departmentID == 4009)
                                        {
                                            testName = testRow["ReportName"] != DBNull.Value ? testRow["ReportName"].ToString() : "";
                                        }
                                        else if (departmentID == 4006)
                                        {
                                            testName = testRow["ReportName"] != DBNull.Value ? testRow["ReportName"].ToString() : "";
                                        }
                                        else if (departmentID == 4008)
                                        {
                                            testName = testRow["ReportName"] != DBNull.Value ? testRow["ReportName"].ToString() : "";
                                        }
                                        else if (departmentID == 5020)
                                        {
                                            testName = testRow["ReportName"] != DBNull.Value ? testRow["ReportName"].ToString() : "";
                                        }
                                        else if (departmentID == 3006)
                                        {
                                            testName = testRow["ReportName"] != DBNull.Value ? testRow["ReportName"].ToString() : "";
                                            /*testName = SampleDescription;*/
                                        }
                                        else
                                        {
                                            // For other departments, concatenate TestParameter + TestSubParameter + TestSubSubParameter
                                            string testParameter = testRow["TestParameter"] != DBNull.Value ? testRow["TestParameter"].ToString() : "";
                                            string testSubParameter = testRow["TestSubParameter"] != DBNull.Value ? testRow["TestSubParameter"].ToString() : "";
                                            string testSubSubParameter = testRow["TestSubSubParameter"] != DBNull.Value ? testRow["TestSubSubParameter"].ToString() : "";

                                            // Build the concatenated test name
                                            testName = testParameter;

                                            if (!string.IsNullOrEmpty(testSubParameter))
                                            {
                                                testName += " - " + testSubParameter;
                                            }

                                            if (!string.IsNullOrEmpty(testSubSubParameter))
                                            {
                                                testName += " - " + testSubSubParameter;
                                            }
                                        }

                                        string method = testRow["TestMethod"] != DBNull.Value ? testRow["TestMethod"].ToString() : "";
                                        string part = testRow["Part"] != DBNull.Value ? testRow["Part"].ToString() : "";
                                        string requirement = testRow["Requirement"] != DBNull.Value ? testRow["Requirement"].ToString() : "";
                                        string remark = testRow["Remarks"] != DBNull.Value ? testRow["Remarks"].ToString() : "";

                                        sbDetails.Append(@"<tr>
                                        <td style='border: 1px solid black; padding: 5px;'>" + testName + @"</td>
                                        <td style='border: 1px solid black; padding: 5px;text-align: center;'>" + method + @"</td>
                                        <td style='border: 1px solid black; padding: 5px;text-align: center;'>" + part + @"</td>
                                        <td style='border: 1px solid black; padding: 5px;text-align: center;'>" + requirement + @"</td>
                                        <td style='border: 1px solid black; padding: 5px;text-align: center;'>" + remark + @"</td>
                                      </tr>");
                                    }
                                }

                                sbDetails.Append("</table>");

                                sbIndolang.Append(@"<table style='width:100%; border:none; border-collapse:collapse; font-family:Arial, sans-serif;'>
                               <tr>
                                   <td style='padding:5px; width:30%;'>Jumlah Sampel </td>
                                   <td style='padding:5px; width:70%;'>: " + Samplequantity + @"</td>
                               </tr>
                               <tr>
                                   <td style='padding:5px; vertical-align:top;'>Penandaan (Labeling) Kabel /Deskripsi Sampel </td>
                                   <td style='padding:5px;'>: " + SampleDescription + @"</td>
                               </tr>
                               <tr>
                                   <td style='padding:5px; vertical-align:top;'>Data Pembuat </td>
                                   <td style='padding:5px;'>: " + customeraddress + @" </td>
                               </tr>
                               <tr>
                                   <td style='padding:5px;'>Parameter Uji </td>
                                   <td style='padding:5px;'>: " + Testparameter + @"</td>
                               </tr>
                             </table>");
                                // Replace sbFooter with a signature section like in the image
                                sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px; border-collapse:collapse;' class='tab1'>
                               <tr>
                                   <td style='width:25%; border-bottom:1px solid black;'>Created by :</td>
                                   <td style='width:50%; border-bottom:1px solid black;' colspan='2'>Confirmed by :</td>
                                   <td style='width:25%; border-bottom:1px solid black;'>Received by :</td>
                               </tr>
                               <tr>
                                   <td style='height:80px;'></td>
                                   <td style='height:80px;' colspan='2'></td>
                                   <td style='height:80px;'></td>
                               </tr>
                               <tr>
                                   <td style='border-bottom:1px solid black;'>Reported by :</td>
                                   <td style='border-bottom:1px solid black;'>Checked by :</td>
                                   <td style='border-bottom:1px solid black;'>Approved by :</td>
                                   <td style='border-bottom:1px solid black;'>Release by :</td>
                               </tr>
                               <tr>
                                   <td style='height:100px;'></td>
                                   <td style='height:100px;'></td>
                                   <td style='height:100px;'></td>
                                   <td style='height:100px;'></td>
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

                            if (departmentID == 4009)
                            {
                                int res1 = _cm.UpdateCRFSampleDetailsWO(cloneCRFSampleID, WONo);
                                if (res1 > 0)
                                {
                                    body = objEmail.PrepareMailBodyWith("WorkOrderLED.html", "{Header}", sbHeader.ToString(), "{Tests}", sbDetails.ToString(), "{Indo-language}", sbIndolang.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString(), "{Documentno}", reportFormatNumber, "{Testreportno}", "");
                                }
                                else
                                {

                                }
                            }
                            else if (departmentID == 4006)
                            {

                                int res1 = _cm.UpdateCRFSampleDetailsWO(cloneCRFSampleID, WONo);
                                if (res1 > 0)
                                {
                                    body = objEmail.PrepareMailBodyWith("WorkOrderPV.html",
                                        "{Header}", sbHeader.ToString(), "{Tests}", sbDetails.ToString(),
                                        "{Indo-language}", sbIndolang.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo,
                                        "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString(),
                                        "{Documentno}", reportFormatNumber, "{Testreportno}", "",
                                        "{NameOfCompany}", CompanyName,
                                   "{CompanyAddress}", CompanyAddress,
                                 "{FactoryAddress}", FactoryAddress);
                                }
                                else
                                {

                                }

                            }
                            else if (departmentID == 4008)
                            {

                                int res1 = _cm.UpdateCRFSampleDetailsWO(cloneCRFSampleID, WONo);
                                if (res1 > 0)
                                {
                                    body = objEmail.PrepareMailBodyWith("WorkOrderHomeAppliances.html",
                                        "{Header}", sbHeader.ToString(), "{Tests}", sbDetails.ToString(),
                                        "{Indo-language}", sbIndolang.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo,
                                        "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString(),
                                        "{Documentno}", reportFormatNumber, "{Testreportno}", "",
                                        "{NameOfCompany}", CompanyName,
                                   "{CompanyAddress}", CompanyAddress,
                                 "{FactoryAddress}", FactoryAddress);
                                }
                                else
                                {

                                }

                            }
                            else
                            {
                                int res1 = _cm.UpdateCRFSampleDetailsWO(cloneCRFSampleID, WONo);
                                if (res1 > 0)
                                {
                                    body = objEmail.PrepareMailBodyWith("WorkOrder.html", "{Header}", sbHeader.ToString(), "{Tests}", sbDetails.ToString(), "{Indo-language}", "", "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString(), "{Documentno}", "", "{Testreportno}", "");
                                }
                            }
                            str = body.ToString();
                            filename = Server.MapPath("//WorkOrder/WorkOrder-" + SampleID + ".html");
                            System.IO.File.WriteAllText(@filename, str);
                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//WorkOrder/WorkOrder-" + SampleID + ".html"));
                            divCRFReview.InnerHtml = html;
                            GC.Collect();
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