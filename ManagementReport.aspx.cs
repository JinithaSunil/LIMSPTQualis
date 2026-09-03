using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using LIMS.DAL;
namespace LIMS
{
    public partial class ManagementReport : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int SampleID = 0; int InquiryID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["i"] != null)
                {
                    Session["InquiryID"] = Request.QueryString["i"].ToString();
                    InquiryID = Convert.ToInt32(Request.QueryString["i"].ToString());

                    DataTable dtAddressDetails = _cm.GetAddressDetailsByInquiryID(InquiryID);

                    // Get department ID from address details or session
                    int departmentID = 0;
                    if (dtAddressDetails.Rows.Count > 0)
                    {
                        departmentID = dtAddressDetails.Rows[0]["DepartmentID"] != DBNull.Value ?
                            Convert.ToInt32(dtAddressDetails.Rows[0]["DepartmentID"]) : 0;
                    }

                    DataTable dtRecordDetails = new DataTable();
                    DataTable dtWorkOrderDetails = new DataTable();

                    if (departmentID == 4009 || departmentID == 4006 || departmentID == 4008 || departmentID == 3006 || departmentID == 4007)
                    {
                        dtRecordDetails = _cm.GetManagementreport_ForALlDepartment(InquiryID);
                        dtWorkOrderDetails = _cm.GetWorkOrderDetailsByInquiryID_ForALlDepartment(InquiryID);
                    }
                    else
                    {
                        dtRecordDetails = _cm.GetManagementreport_FOODCHEMICAL(InquiryID);
                        dtWorkOrderDetails = _cm.GetWorkOrderDetailsByInquiryID_FOODCHEMICAL(InquiryID);
                    }

                    if (dtAddressDetails.Rows.Count > 0 && dtRecordDetails.Rows.Count > 0)
                    {
                        // Get header data
                        DataRow addressRow = dtAddressDetails.Rows[0];
                        string Department = addressRow["DepartmentName"] != DBNull.Value ? addressRow["DepartmentName"].ToString() : "";
                        string CustomerAddress = addressRow["CustomerAddress"] != DBNull.Value ? addressRow["CustomerAddress"].ToString() : "";
                        string Customer = addressRow["Customer"] != DBNull.Value ? addressRow["Customer"].ToString() : "";

                        DataRow firstRow = dtRecordDetails.Rows[0];
                        string RequestNo = firstRow["RequestNo"] != DBNull.Value ? firstRow["RequestNo"].ToString() : "";
                        string QuotationNo = firstRow["QuotationNumber"] != DBNull.Value ? firstRow["QuotationNumber"].ToString() : "";
                        string CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
                        string PICName = Session["EmployeeName"] != null ? Session["EmployeeName"].ToString() : "";

                        // Get totals from database
                        string discount = firstRow["Discount"] != DBNull.Value ? firstRow["Discount"].ToString() : "0";
                        string vatAmount = firstRow["VatAmount"] != DBNull.Value ? firstRow["VatAmount"].ToString() : "0";
                        string vatPercentage = firstRow["Percentage"] != DBNull.Value ? firstRow["Percentage"].ToString() : "0";
                        string finalGrandTotal = firstRow["GrandTotal"] != DBNull.Value ? firstRow["GrandTotal"].ToString() : "0";
                        string surcharge = "0"; // 

                        // Build Quotation Table
                        StringBuilder sbQuotationTable = new StringBuilder();
                        sbQuotationTable.Append(@"
<table style='width: 100%; border: 2px solid #000; padding: 10px; margin-bottom: 20px;'>
    <tr>
        <td style='padding: 10px;'>
        
        <table style='width: 100%; border-collapse: collapse; border: 1px solid #000; margin-top: 10px;'>
            <tr style='background-color: #f2f2f2;'>
                <th style='width: 10%; text-align: center; border: 1px solid #000; padding: 5px; font-weight: bold;'>No.</th>
                <th style='width: 40%; text-align: center; border: 1px solid #000; padding: 5px; font-weight: bold;'>Tests</th>
                <th style='width: 20%; text-align: center; border: 1px solid #000; padding: 5px; font-weight: bold;'>Method</th>
                <th style='width: 10%; text-align: center; border: 1px solid #000; padding: 5px; font-weight: bold;'>Qty</th>
                <th style='width: 10%; text-align: center; border: 1px solid #000; padding: 5px; font-weight: bold;'>Unit (IDR)</th>
                <th style='width: 10%; text-align: center; border: 1px solid #000; padding: 5px; font-weight: bold;'>Total (IDR)</th>
            </tr>");

                        string previousTAF = "";
                        int rowNumber = 1;
                        decimal currentTAFSubTotal = 0;
                        decimal totalOfAllSubTotals = 0;

                        DataView dv = new DataView(dtRecordDetails);
                        dv.Sort = "TAFNo ASC";
                        DataTable sortedTable = dv.ToTable();

                        foreach (DataRow row in sortedTable.Rows)
                        {
                            string currentTAF = row["TAFNo"] != DBNull.Value ? row["TAFNo"].ToString() : "";

                            if (currentTAF != previousTAF)
                            {
                                // Close previous TAF and show SubTotal
                                if (!string.IsNullOrEmpty(previousTAF))
                                {
                                    sbQuotationTable.Append(@"
                <tr>
                    <td colspan='5' class='tab1' style='text-align: right; font-weight: bold;width: 100%'>Sub Total</td>
                    <td class='tab1' style='text-align: right; font-weight: bold;'>" + currentTAFSubTotal.ToString("N2") + @"</td>
                </tr>
            </table>");

                                    totalOfAllSubTotals += currentTAFSubTotal;
                                    currentTAFSubTotal = 0; // Reset
                                    rowNumber = 1;
                                }

                                string inquiryDetail = row["InquiryDetail"] != DBNull.Value ? row["InquiryDetail"].ToString() : "";
                                string batchNumber = row["batchno"] != DBNull.Value ? row["batchno"].ToString() : "";
                                string workOrder = row["WorkorderNumber"] != DBNull.Value ? row["WorkorderNumber"].ToString() : "";

                                sbQuotationTable.Append(@"
            <tr>
                <td colspan='6' style='padding: 8px; border: 1px solid #000; background-color: #f9f9f9;'>
                    <strong>TAF/" + currentTAF + @" --- Sample Description : " + inquiryDetail + @" Batch: " + batchNumber + @"</strong><br/>
                    WO : " + workOrder + @" SO :
                </td>
            </tr>");

                                previousTAF = currentTAF;
                            }

                            // Get row data from database
                            string samplename = row["samplename"] != DBNull.Value ? row["samplename"].ToString() : "";
                            string testMethods = row["TestMethods"] != DBNull.Value ? row["TestMethods"].ToString() : "";
                            string quantity = row["Quantity"] != DBNull.Value ? row["Quantity"].ToString() : "0";
                            string price = row["Price"] != DBNull.Value ? row["Price"].ToString() : "0";
                            string totalAmount = row["TotalAmount"] != DBNull.Value ? row["TotalAmount"].ToString() : "0"; // Total IDR from DB

                            decimal priceValue = 0;
                            decimal totalAmountValue = 0;
                            decimal.TryParse(price, out priceValue);
                            decimal.TryParse(totalAmount, out totalAmountValue);

                            currentTAFSubTotal += totalAmountValue; // Sum TotalAmount for SubTotal

                            sbQuotationTable.Append(@"
    <tr>
        <td class='tab1' style='text-align: center;'>" + rowNumber + @"</td>
        <td class='tab1'>" + samplename + @"</td>
        <td class='tab1' style='text-align: center;'>" + testMethods + @"</td>
        <td class='tab1' style='text-align: center;'>" + quantity + @"</td>
        <td class='tab1' style='text-align: right;'>" + priceValue.ToString("N2") + @"</td>
        <td class='tab1' style='text-align: right;'>" + totalAmountValue.ToString("N2") + @"</td>
    </tr>");

                            rowNumber++;
                        }

                        // Close last table and add final totals
                        if (!string.IsNullOrEmpty(previousTAF))
                        {
                            sbQuotationTable.Append(@"
        <tr>
            <td colspan='5' class='tab1' style='text-align: right; font-weight: bold;width: 100%'>Sub Total</td>
            <td class='tab1' style='text-align: right; font-weight: bold;'>" + currentTAFSubTotal.ToString("N2") + @"</td>
        </tr>
    </table><br/><br/>");

                            totalOfAllSubTotals += currentTAFSubTotal;

                            // Parse final values
                            decimal discountValue = 0;
                            decimal vatAmountValue = 0;
                            decimal surchargeValue = 0;
                            decimal finalGrandTotalValue = 0;

                            decimal.TryParse(discount, out discountValue);
                            decimal.TryParse(vatAmount, out vatAmountValue);
                            decimal.TryParse(surcharge, out surchargeValue);
                            decimal.TryParse(finalGrandTotal, out finalGrandTotalValue);

                            sbQuotationTable.Append(@"
    <table class='tab1' style='width: 100%; margin-top: 10px;'>
        <tr>
            <td colspan='5' class='tab1' style='text-align: right; font-weight: bold;'>Total</td>
            <td class='tab1' style='text-align: right; font-weight: bold;'>" + totalOfAllSubTotals.ToString("N2") + @"</td>
        </tr>
        <tr>
            <td colspan='5' class='tab1' style='text-align: right;'>Subtotal Before Discount</td>
            <td class='tab1' style='text-align: right;'>" + totalOfAllSubTotals.ToString("N2") + @"</td>
        </tr>
        <tr>
            <td colspan='5' class='tab1' style='text-align: right;'>Discount (" + discount + @"%)</td>
            <td class='tab1' style='text-align: right;'>-" + discountValue.ToString("N2") + @"</td>
        </tr>
        <tr>
            <td colspan='5' class='tab1' style='text-align: right;'>Surcharge (" + surcharge + @"%)</td>
            <td class='tab1' style='text-align: right;'>" + surchargeValue.ToString("N2") + @"</td>
        </tr>
        <tr>
            <td colspan='5' class='tab1' style='text-align: right;'>VAT (" + vatPercentage + @"%)</td>
            <td class='tab1' style='text-align: right;'>" + vatAmountValue.ToString("N2") + @"</td>
        </tr>
        <tr style='background-color: #f0f0f0;'>
            <td colspan='5' class='tab1' style='text-align: right; font-weight: bold; font-size: 16px;'>Grand Total</td>
            <td class='tab1' style='text-align: right; font-weight: bold; font-size: 16px;'>" + finalGrandTotalValue.ToString("N2") + @"</td>
        </tr>
    </table>");
                        }
                        // Close wrapper table for Quotation section
                        sbQuotationTable.Append(@"
        </td>
    </tr>
</table>");
                        StringBuilder sbWorkOrderTable = new StringBuilder();

                        if (dtWorkOrderDetails.Rows.Count > 0)
                        {
                            sbWorkOrderTable.Append(@"
<table style='width: 100%; border: 2px solid #000; padding: 10px; margin-bottom: 20px;'>
    <tr>
        <td style='padding: 10px;'>");

                            // ── ADD HEADER HERE BEFORE LOOP ──
                            if (departmentID == 4009 || departmentID == 4006 || departmentID == 4008 || departmentID == 3006 || departmentID == 4007)
                            {
                                sbWorkOrderTable.Append(@"
        <table style='width: 100%; border-collapse: collapse; border: 1px solid #000;'>
            <tr style='background-color: #f2f2f2;'>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 25%;'>Tests</th>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 15%;'>Method</th>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 10%;'>Part</th>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 15%;'>Requirement</th>
                <th colspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 25%;'>Date Of Test</th>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 10%;'>Remark</th>
            </tr>
            <tr style='background-color: #f2f2f2;'>
                <th style='border: 1px solid #000; padding: 5px; text-align: center;'>Start Test</th>
                <th style='border: 1px solid #000; padding: 5px; text-align: center;'>End Test</th>
            </tr>");
                            }
                            else
                            {
                                sbWorkOrderTable.Append(@"
        <table style='width: 100%; border-collapse: collapse; border: 1px solid #000;'>
            <tr style='background-color: #f2f2f2;'>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 30%;'>Tests</th>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 20%;'>Method</th>
                <th colspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 30%;'>Date Of Test</th>
                <th rowspan='2' style='border: 1px solid #000; padding: 5px; text-align: center; width: 20%;'>Remark</th>
            </tr>
            <tr style='background-color: #f2f2f2;'>
                <th style='border: 1px solid #000; padding: 5px; text-align: center;'>Start Test</th>
                <th style='border: 1px solid #000; padding: 5px; text-align: center;'>End Test</th>
            </tr>");
                            }
                            string previousWorkOrder = "";
                            string previousSampleCode = "";

                            foreach (DataRow woRow in dtWorkOrderDetails.Rows)
                            {
                                string currentWorkOrder = woRow["WorkOrderNumber"] != DBNull.Value ? woRow["WorkOrderNumber"].ToString() : "";
                                string currentSampleCode = woRow["SampleCode"] != DBNull.Value ? woRow["SampleCode"].ToString() : "";

                                if (currentWorkOrder != previousWorkOrder)
                                {
                                    if (!string.IsNullOrEmpty(previousWorkOrder))
                                    {
                                        sbWorkOrderTable.Append(@"</table><br/><br/>");
                                    }

                                    string receivedDate = woRow["ReceivedDate"] != DBNull.Value ? Convert.ToDateTime(woRow["ReceivedDate"]).ToString("dd/MM/yyyy") : "";
                                    string targetDate = woRow["TargetDate"] != DBNull.Value ? Convert.ToDateTime(woRow["TargetDate"]).ToString("dd/MM/yyyy") : "";
                                    string priority = woRow["Priority"] != DBNull.Value ? woRow["Priority"].ToString() : "";
                                    string buyer = woRow["Buyer"] != DBNull.Value ? woRow["Buyer"].ToString() : "-";

                                    sbWorkOrderTable.Append(@"
<table style='width: 100%; margin-top: 20px; font-family: Arial, sans-serif; border-collapse: collapse;'>
    <tr>
        <td colspan='2' style='padding: 10px'>
            <strong>" + currentWorkOrder + @"</strong><br/>
            <strong>" + Department + @"</strong>
        </td>
        <td colspan='2' style='padding: 10px'>
            <strong>Received Date</strong> : " + receivedDate + @"<br/>
            <strong>Target Date</strong> : " + targetDate + @"
        </td>
        <td colspan='2' style='padding: 10px'>
            <strong>Priority</strong> : " + priority + @"<br/>
            <strong>Buyer</strong> : " + buyer + @"
        </td>
    </tr>");

                                    previousWorkOrder = currentWorkOrder;
                                    previousSampleCode = "";
                                }

                                if (currentSampleCode != previousSampleCode)
                                {
                                    if (!string.IsNullOrEmpty(previousSampleCode))
                                    {
                                        sbWorkOrderTable.Append(@"</table></td></tr>");
                                    }

                                    string sampleDescription = woRow["SampleDescription"] != DBNull.Value ? woRow["SampleDescription"].ToString() : "";
                                    string sampleRemark = woRow["Remark"] != DBNull.Value ? woRow["Remark"].ToString() : "";

                                    sbWorkOrderTable.Append(@"
    <tr>
        <td colspan='2' style='padding: 10px; vertical-align: top;'>
            <strong>Sample Code</strong> : " + currentSampleCode + @"<br/>
            <strong>Remark</strong> : " + sampleRemark + @"
        </td>
        <td colspan='4' style='padding: 10px; vertical-align: top;'>
            <strong>Sample Description</strong> : " + sampleDescription + @"
        </td>
    </tr>
    <tr>
        <td colspan='6' style='padding: 0; border: 0;'>
            <table style='width: 100%; border-collapse: collapse; border: 1px solid #000;'>");

                                   
                                    previousSampleCode = currentSampleCode;
                                }

                                // Build test name based on department
                                string testName = "";
                                if (departmentID == 4009 || departmentID == 4006 || departmentID == 4008 || departmentID == 3006)
                                {
                                    testName = woRow["ReportName"] != DBNull.Value ? woRow["ReportName"].ToString() : "";
                                }
                                else
                                {
                                    string testParameter = woRow["TestParameter"] != DBNull.Value ? woRow["TestParameter"].ToString().Trim() : "";
                                    string testSubParameter = woRow["TestSubParameter"] != DBNull.Value ? woRow["TestSubParameter"].ToString().Trim() : "";
                                    string testSubSubParameter = woRow["TestSubSubParameter"] != DBNull.Value ? woRow["TestSubSubParameter"].ToString().Trim() : "";

                                    testName = testParameter;
                                    if (!string.IsNullOrEmpty(testSubParameter)) testName += " - " + testSubParameter;
                                    if (!string.IsNullOrEmpty(testSubSubParameter)) testName += " - " + testSubSubParameter;
                                }

                                string method = woRow["TestMethod"] != DBNull.Value ? woRow["TestMethod"].ToString() : "";
                                string startTest = woRow["StartTest"] != DBNull.Value ? Convert.ToDateTime(woRow["StartTest"]).ToString("dd/MM/yyyy") : "";
                                string endTest = woRow["EndTest"] != DBNull.Value ? Convert.ToDateTime(woRow["EndTest"]).ToString("dd/MM/yyyy") : "";

                                // Add test row based on department
                                if (departmentID == 4009 || departmentID == 4006 || departmentID == 4008 || departmentID == 3006)
                                {
                                    string part = woRow["Part"] != DBNull.Value ? woRow["Part"].ToString() : "";
                                    string requirement = woRow["Requirement"] != DBNull.Value ? woRow["Requirement"].ToString() : "";

                                    sbWorkOrderTable.Append(@"
                <tr>
                    <td style='border: 1px solid #000; padding: 5px;'>" + testName + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + method + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + part + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + requirement + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + startTest + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + endTest + @"</td>
                    <td style='border: 1px solid #000; padding: 5px;'></td>
                </tr>");
                                }
                                else
                                {
                                    sbWorkOrderTable.Append(@"
                <tr>
                    <td style='border: 1px solid #000; padding: 5px;'>" + testName + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + method + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + startTest + @"</td>
                    <td style='border: 1px solid #000; padding: 5px; text-align: center;'>" + endTest + @"</td>
                    <td style='border: 1px solid #000; padding: 5px;'></td>
                </tr>");
                                }
                            }

                            if (!string.IsNullOrEmpty(previousSampleCode))
                            {
                                sbWorkOrderTable.Append(@"</table></td></tr></table>");
                            }                         
                            
                        }
                        sbWorkOrderTable.Append(@"
        </td>
    </tr>
</table>");

                        mailer objEmail = new mailer();
                        string body = objEmail.PrepareMailBodyWith("ManagementReport.html",
                            "{RequestNo}", RequestNo,
                            "{QuotationNo}", QuotationNo,
                            "{Date}", CurrentDate,
                            "{CompanyName}", Customer,
                            "{CompanyAddress}", CustomerAddress,
                            "{Department}", Department,
                            "{PICName}", PICName,
                            "{QuotationTable}", sbQuotationTable.ToString(),
    "{WorkOrderTable}", sbWorkOrderTable.ToString());


                        string reportFolder = Server.MapPath("~/ManagementReport/");
                        if (!Directory.Exists(reportFolder))
                        {
                            Directory.CreateDirectory(reportFolder);
                        }

                        string filename = Server.MapPath("~/ManagementReport/ManagementReport-" + InquiryID + ".html");
                        System.IO.File.WriteAllText(@filename, body);
                        string html = File.ReadAllText(filename);
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