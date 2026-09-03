using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class EnquiryStatus : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM(); int CustomerUID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["EmployeeName"] != null)
                {
                    Label lblUserName = (Label)Master.FindControl("lblUserName");
                    lblUserName.Text = Session["EmployeeName"].ToString();
                }
                if (Session["Email"] != null)
                {
                    Label lblUserEmail = (Label)Master.FindControl("lblUserEmail");
                    lblUserEmail.Text = Session["Email"].ToString();
                }
                if (!Page.IsPostBack)
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        string InquiryID = Request.QueryString["ID"].ToString();
                        if (InquiryID != null)
                        {
                            Session["InquiryID"] = InquiryID;
                            BindStatus(Convert.ToInt32(InquiryID));
                        }
                    }
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        private void BindStatus(int InquiryID)
        {
            int CRFStatusID = 0; int sampleID = 0; int TestResultId = 0; int CloneCRFSampleID = 0;
            int AllocationID = 0; int sampleUID = 0; string AllocatedTo = "";
            int ReportId = 0;
            DataTable dt = new DataTable();
            dt = _cm.GetCustomerInquiryDetails(InquiryID, 1);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["CRFStatusID"].ToString() != "")
                {
                    ReportId = Convert.ToInt32(dr["ReportID"].ToString());
                    CRFStatusID = Convert.ToInt32(dr["CRFStatusID"].ToString());
                    CloneCRFSampleID = Convert.ToInt32(dr["CloneCRFSampleID"].ToString());
                    InquiryID = Convert.ToInt32(dr["InquiryID"].ToString());
                    DataTable dt2 = _cm.getInquiryDetails(InquiryID, 1);
                    DataTable dt3 = _cm.GetCloneAllocationDetails(CloneCRFSampleID.ToString());

                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            CustomerUID = Convert.ToInt32(dr2["CustomerUID"].ToString());
                        }
                    }
                    if (dt3.Rows.Count > 0)
                    {
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            sampleUID = Convert.ToInt32(dr3["sampleUID"].ToString());
                            AllocatedTo = dr3["employeeName"].ToString();
                        }
                    }
                    if (!dr.IsNull("sampleID"))
                    {
                        sampleID = Convert.ToInt32(dr["sampleID"].ToString());
                    }

                    DataTable dt4 = _cm.getTestDetailsForEnquiryStatus(sampleUID);
                    if (dt4.Rows.Count > 0)
                    {
                        TestResultId = Convert.ToInt32(dt4.Rows[0]["TestResultId"].ToString());
                    }
                    if (CRFStatusID == 1)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true;
                        cardCRF.Visible = false;
                        cardSample.Visible = false;
                        cardAllocation.Visible = false;
                        cardTesting.Visible = false;
                        cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 2)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 3)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF More Info";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 4)
                    {
                        lblInquiryStatus.Text = "Rejected";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 5)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Sent";
                        cardCRF.Visible = true; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 6)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Received";
                        cardCRF.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                    }
                    if (CRFStatusID == 7)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 8)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 9)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 10)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 11)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 12)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        lblTestingStatus.Text = "Test Report Submitted for Review"; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 13)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Received"; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 14)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true;
                        lblSampleStatus.Text = "Sample Accepted By Sample Management";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        cardAllocation.Visible = false;
                        cardTesting.Visible = false;
                        cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 15)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 16)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Rejected By Sample Management";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 17)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Rejected By Lab"; cardSample.Visible = true; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 18)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 19)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true;
                        lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        cardTesting.Visible = false;
                        cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 20)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        cardTesting.Visible = false;
                        cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 21)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                    }
                    if (CRFStatusID == 22)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        lblTestingStatus.Text = "Reviewed Accepted";
                        cardFeedback.Visible = true; lblFeedbackStatus.Text = "Feedback Form Sent";
                    }
                    if (CRFStatusID == 23)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        dt2 = _cm.getTestReportID(sampleID);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                            cardTesting.Visible = true;
                            lblTestingStatus.Text = "Reviewed Accepted";
                            hypTestResults.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestResults.Visible = true;
                        }
                        cardFeedback.Visible = true; lblFeedbackStatus.Text = "Feedback Received";
                        hypCustomerFeedback.NavigateUrl = string.Format("~/CustomerFeedbackStatus.aspx?ID=" + InquiryID);
                        hypCustomerFeedback.Visible = true;
                    }
                    if (CRFStatusID == 24)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        lblTestingStatus.Text = "Reviewed Accepted"; cardFeedback.Visible = false;
                        dt2 = _cm.getTestReportID(sampleID);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                            cardTesting.Visible = true;
                            lblTestingStatus.Text = "Completed";
                            hypTestResults.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestResults.Visible = true;
                        }

                    }
                    if (CRFStatusID == 25)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        lblTestingStatus.Text = "Reviewed Rejected"; cardFeedback.Visible = false;
                        lblTestingStatus.Text = "Reviewed Accepted"; cardFeedback.Visible = false;
                        dt2 = _cm.getTestReportID(sampleID);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                            cardTesting.Visible = true;
                            lblTestingStatus.Text = "Completed";
                            hypTestResults.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestResults.Visible = true;
                        }
                    }
                    if (CRFStatusID == 26)
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        lblTestingStatus.Text = "Test Report Generated"; cardFeedback.Visible = false;
                        dt2 = _cm.getTestReportID(sampleID);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                            cardTesting.Visible = true;
                            lblTestingStatus.Text = "Completed";
                            hypTestResults.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                            hypTestResults.Visible = true;
                        }
                    }
                    if ((CRFStatusID == 27)|| (CRFStatusID == 29))
                    {
                        lblInquiryStatus.Text = "Accepted";
                        BindEnquiry(dt);
                        cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                        Session["SampleId"] = sampleID;
                        hypSampleVerification.Visible = true;
                        hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                        cardSample.Visible = true;
                        lblAllocationStatus.Text = AllocatedTo;
                        hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        hypTestAllocation.Visible = true;
                        cardAllocation.Visible = true;
                        lblTestingStatus.Text = "Test Report Generated"; cardFeedback.Visible = false;

                        dt2 = _cm.getTestReportID(sampleID);
                        if (dt2.Rows.Count > 0)
                        {
                            TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                            cardTesting.Visible = true;
                            lblTestingStatus.Text = "Completed";

                            string pdf1, pdf2, pdf3, pdf4;
                            string id = sampleUID.ToString();
                            int revisionCount = 0;
                            string SampleIDSeries = "";
                            DataTable dtSID = _cm.GetRevisionCountSampleUIDWise(id);
                            if (dtSID != null || dtSID.Rows.Count > 0)
                            {
                                DataRow row = dtSID.Rows[0];
                                revisionCount = row["RevisionCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["RevisionCount"].ToString());
                                SampleIDSeries = row["sampleIDSeries"].ToString();
                            }

                            if (ReportId == 26 || ReportId == 27 || ReportId == 28 || ReportId == 6417 || ReportId == 6418 || ReportId == 6419)
                            {
                                if (revisionCount > 0)
                                {
                                    pdf1 = $"~/TestReport/TestReport-{sampleUID}_Rev{revisionCount}.pdf";
                                    pdf2 = $"~/TestReport/TestReport-{sampleUID}_Rev{revisionCount}_BH.pdf";
                                    pdf3 = $"~/DraftFiles/TestReport-{sampleUID}_Rev{revisionCount}_Draft.pdf";
                                    pdf4 = $"~/DraftFiles/TestReport-{sampleUID}_Rev{revisionCount}_BH_Draft.pdf";

                                hypTestResults1.NavigateUrl = pdf1;
                                hypTestResults1.Visible = true;

                                    hypTestResults2.NavigateUrl = pdf2;
                                    hypTestResults2.Visible = true;

                                    hypTestResults3.NavigateUrl = pdf3;
                                    hypTestResults3.Visible = true;

                                    hypTestResults4.NavigateUrl = pdf4;
                                    hypTestResults4.Visible = true;
                                }
                                else
                                {
                                    pdf1 = $"~/TestReport/TestReport-{sampleUID}.pdf";
                                    pdf2 = $"~/TestReport/TestReport-{sampleUID}_BH.pdf";

                                hypTestResults1.NavigateUrl = pdf1;
                                hypTestResults1.Visible = true;

                                    hypTestResults2.NavigateUrl = pdf2;
                                    hypTestResults2.Visible = true;
                                }
                            }
                            else
                            {
                                if (revisionCount > 0)
                                {
                                    pdf1 = $"~/TestReport/TestReport-{sampleUID}_Rev{revisionCount}.pdf";
                                }
                                else
                                {
                                    pdf1 = $"~/TestReport/TestReport-{sampleUID}.pdf";
                                }

                                hypTestResults1.NavigateUrl = pdf1;
                                hypTestResults1.Visible = true;
                            }


                        }
                    }
                }
            }
        }
        private void BindEnquiry(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                lblEnquiryID.Text = "ENQ" + dr["InquiryID"].ToString();
                lblInquiryDetail.Text = Limit(System.Web.HttpUtility.HtmlDecode(dr["InquiryDetail"].ToString()), 40);
                lblInquiryDetail.ToolTip = System.Web.HttpUtility.HtmlDecode(dr["InquiryDetail"].ToString());

                ReadMoreLinkButton.Visible = SetVisibility(dr["InquiryDetail"].ToString(), 40);
            }
        }
        private void BindCRF(int enquiryID, int enquiryStatusID)
        {
            int CRFID = 0;
            DataTable dt = _cm.getCRFDetails(enquiryID);
            switch (enquiryStatusID)
            {
                case 6:
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            CRFID = Convert.ToInt32(dr["CustomerRequestId"].ToString());
                        }
                    }
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + CRFID + ".htm");
                    hypCRF.Visible = true;
                    break;
            }
        }


        protected string Limit(object desc, int maxLength)
        {
            var description = (string)desc;
            if (string.IsNullOrEmpty(description)) { return description; }
            return description.Length <= maxLength ?
                description : description.Substring(0, maxLength) + "...";
        }
        protected bool SetVisibility(object desc, int maxLength)
        {
            var description = (string)desc;
            if (string.IsNullOrEmpty(description)) { return false; }
            return description.Length > maxLength;
        }
        protected void ReadMoreLinkButton_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            button.Text = (button.Text == "Read More") ? "Hide" : "Read More";
            string temp = lblInquiryDetail.Text;
            lblInquiryDetail.Text = lblInquiryDetail.ToolTip;
            lblInquiryDetail.ToolTip = temp;
        }
    }
}