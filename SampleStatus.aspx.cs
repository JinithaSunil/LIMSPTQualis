using Microsoft.Office.Interop.Excel;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using DataTable = System.Data.DataTable;
using Image = System.Drawing.Image;
using Label = System.Web.UI.WebControls.Label;
using Shape = Syncfusion.DocIO.DLS.Shape;
// Use alias to avoid naming conflicts
using DrawingImage = System.Drawing.Image;
using Encoder = System.Drawing.Imaging.Encoder;

namespace LIMS
{
    public partial class SampleStatus : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM(); int CustomerUID = 0;
        private byte[] CleanPdfBytes
        {
            get { return Session["CleanPdfBytes"] as byte[]; }
            set { Session["CleanPdfBytes"] = value; }
        }

        private byte[] WatermarkedPdfBytes
        {
            get { return Session["WatermarkedPdfBytes"] as byte[]; }
            set { Session["WatermarkedPdfBytes"] = value; }
        }

        //private bool _pastFirstPage;
        private bool _pastFirstPage = false;

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
                    //liInquiries1.Attributes.Add("class", "active");

                    if (Request.QueryString["ID"] != null)
                    {
                        string CRFID = Request.QueryString["ID"].ToString();
                        if (CRFID != null)
                        {
                            Session["CRFID"] = CRFID;
                            BindStatus(CRFID);
                            BindSampleImages();
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
        private void BindStatus(string CloneCRFSampleID)
        {
            int CRFStatusID = 0; int sampleID = 0; int TestResultId = 0; string InquiryID = "";
            int AllocationID = 0; int sampleUID = 0; string AllocatedTo = ""; string TestRequestReviewID = "";

            bool nabl1 = false; bool nonnabl1 = false;
            int nabl = 0; int nonnabl = 0;


            DataTable dtReport = _cm.GetReportType(CloneCRFSampleID);
            if (dtReport.Rows.Count > 0)
            {
                if (dtReport.Rows[0]["ReportType"].ToString() == "NABL_Cert_PDP")
                {
                    DataTable dtNABL = _cm.NABLOrNonNABL(dtReport.Rows[0]["ID"].ToString());
                    if (dtNABL.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtNABL.Rows)
                        {
                            if (row["NABLorNonNABL"].ToString() == "Non NABL")
                            {
                                nonnabl++;

                            }
                            if (row["NABLorNonNABL"].ToString() == "NABL")
                            {
                                nabl++;
                            }
                        }
                        if (nabl > 0) { nabl1 = true; nonnabl1 = true; } else { nabl1 = false; nonnabl1 = false; }
                        if (nonnabl > 0) { nonnabl1 = true; } else { nonnabl1 = false; }
                    }
                }
                else
                {
                    nabl1 = true;
                    nonnabl1 = true;
                }
            }
            hypTestResults.Visible = nabl1;
            hypTestResultsNN.Visible = nonnabl1;

            DataTable dt = new DataTable();
            dt = _cm.getCRFcloneStatus(CloneCRFSampleID);
            foreach (DataRow dr in dt.Rows)
            {

                CRFStatusID = Convert.ToInt32(dr["CRFStatusID"].ToString());
                InquiryID = dr["EnquiryID"].ToString();
                lblInquiryID.Text = InquiryID;
                ViewState["InquiryID"] = InquiryID;

                DataTable dtTestRequestReview = _cm.getTestRequestReview(Convert.ToInt32(InquiryID));
                if (dtTestRequestReview.Rows.Count > 0)
                {
                    TestRequestReviewID = dtTestRequestReview.Rows[0]["TestRequestReviewID"].ToString();
                }
                Session["InquiryID"] = InquiryID;

                DataTable dt2 = _cm.getCustomerDetailsFromCRFID(CloneCRFSampleID);
                DataTable dt3 = _cm.GetCloneAllocationDetails(CloneCRFSampleID);


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
                DataTable dt6 = new DataTable();
                if (!dr.IsNull("sampleID"))
                {
                    sampleID = Convert.ToInt32(dr["sampleID"].ToString());
                    Session["sampleID"] = sampleID;
                    dt6 = _cm.getSampleUID(sampleID);
                    if (dt6.Rows.Count > 0)
                    {
                        if (dt6.Rows[0]["SampleStatus"].ToString() == "22")
                        {
                            cardFeedback.Visible = true;
                            lblFeedbackStatus.Text = "Feedback Form Sent";
                        }
                        if (dt6.Rows[0]["SampleStatus"].ToString() == "23")
                        {
                            cardFeedback.Visible = true;
                            lblFeedbackStatus.Text = "Feedback Received";
                        }
                    }
                }

                DataTable dt4 = _cm.getTestDetailsForEnquiryStatus(sampleUID);
                if (dt4.Rows.Count > 0)
                {
                    TestResultId = Convert.ToInt32(dt4.Rows[0]["TestResultId"].ToString());
                    TestResultId = Convert.ToInt32(dt4.Rows[0]["TestResultId"].ToString());
                }
                DataTable dt5 = _cm.GetCustomerInquiryDetails(Convert.ToInt32(Session["InquiryID"].ToString()), 1);
                if (CRFStatusID == 1)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
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
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 3)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF More Info";
                    //hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }

                    cardCRF.Visible = true; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 4)
                {
                    lblInquiryStatus.Text = "Rejected";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 5)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Sent";
                    cardCRF.Visible = true; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 6)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Received";
                    cardCRF.Visible = true;
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                }
                if (CRFStatusID == 7)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 8)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 9)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 10)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 11)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 12 || CRFStatusID == 23 || CRFStatusID == 22 || CRFStatusID == 30 || CRFStatusID == 25 || CRFStatusID == 24)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    lblAllocationStatus.Text = AllocatedTo;
                    hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                    hypTestAllocation.Visible = true;
                    cardAllocation.Visible = true;
                    lblTestingStatus.Text = "Test Report Submitted for Review";
                    hypTestReport.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                    hypTestReport.Visible = true;
                    DataTable dtReview = _cm.GetReviewDetails(sampleID);
                    if (dtReview.Rows.Count > 0)
                    {
                        _cm.ReviewCount = dtReview.Rows.Count;
                        foreach (DataRow dr1 in dtReview.Rows)
                        {
                            HyperLink hyp = new HyperLink();
                            hyp.ID = "hyp" + dr1["ReviewCount"].ToString();
                            hyp.NavigateUrl += string.Format("~/Sample/TestReportReview-" + dr1["SampleUID"].ToString() + "-" + dr1["ReviewCount"].ToString() + ".htm");
                            hyp.Text = "Review - " + dr1["ReviewCount"].ToString();
                            hyp.Target = "_new";
                            PlaceHolderReviewList.Controls.Add(new LiteralControl(", "));
                            PlaceHolderReviewList.Controls.Add(hyp);
                        }

                    }

                    cardTestReportUpload.Visible = true;
                    //  cardReport.Visible = true;
                }
                if (CRFStatusID == 13)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Received"; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 14)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true;
                    lblSampleStatus.Text = "Sample Accepted By Sample Management";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    cardAllocation.Visible = false;
                    cardTesting.Visible = false;
                    cardFeedback.Visible = false;
                }
                if (CRFStatusID == 15)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);
                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 16)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Rejected By Sample Management";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 17)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Rejected By Lab"; cardSample.Visible = true; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 18)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; cardCRF.Visible = false; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 19)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true;
                    lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

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
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    lblAllocationStatus.Text = AllocatedTo;
                    hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                    hypTestAllocation.Visible = true;
                    cardAllocation.Visible = true;
                    cardTesting.Visible = false;
                    cardFeedback.Visible = false;
                }
                if ((CRFStatusID == 21) || (CRFStatusID == 28))
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; cardSample.Visible = false; cardAllocation.Visible = false; cardTesting.Visible = false; cardFeedback.Visible = false;
                }
                if (CRFStatusID == 22)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

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
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

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
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }
                    cardFeedback.Visible = true; lblFeedbackStatus.Text = "Feedback Received";
                    hypCustomerFeedback.NavigateUrl = string.Format("~/CustomerFeedbackStatus.aspx?ID=" + InquiryID);
                    hypCustomerFeedback.Visible = true;
                }
                if (CRFStatusID == 24)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

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
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;// string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }

                }
                if (CRFStatusID == 25)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    lblAllocationStatus.Text = AllocatedTo;
                    hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                    hypTestAllocation.Visible = true;
                    cardAllocation.Visible = true;
                    lblTestingStatus.Text = "Reviewed Rejected"; cardFeedback.Visible = false;
                    dt2 = _cm.getTestReportID(sampleID);
                    if (dt2.Rows.Count > 0)
                    {
                        TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                        cardTesting.Visible = true;
                        lblTestingStatus.Text = "Completed";
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }
                }
                if (CRFStatusID == 26)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

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
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }
                }
                if (CRFStatusID == 27)
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

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
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }
                }
                if (CRFStatusID == 29)
                {
                    cardTestReportUpload.Visible = true;

                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    lblAllocationStatus.Text = AllocatedTo;
                    hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                    hypTestAllocation.Visible = true;
                    cardAllocation.Visible = true;
                    lblTestingStatus.Text = "Test Report Generated";
                    cardTestReportUpload.Visible = true;
                    cardFeedback.Visible = false;
                    dt2 = _cm.getTestReportID(sampleID);
                    if (dt2.Rows.Count > 0)
                    {
                        TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                        cardTesting.Visible = true;
                        lblTestingStatus.Text = "Completed";
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }
                }
                if ((CRFStatusID == 12) || (CRFStatusID == 22) || (CRFStatusID == 23) || (CRFStatusID == 24) || (CRFStatusID == 25) || (CRFStatusID == 27) || (CRFStatusID == 29))
                {
                    lblInquiryStatus.Text = "Accepted";
                    BindEnquiry(dt5);
                    cardEnquiry.Visible = true; lblCRFStatus.Text = "CRF Approved";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    hypCRF.Visible = true;
                    if (TestRequestReviewID != "")
                    {
                        hypCRFReview.NavigateUrl = string.Format("~/TestRequestReview/TestRequestReview-" + TestRequestReviewID + ".htm");
                        hypCRFReview.Visible = true;
                    }
                    cardCRF.Visible = true; lblSampleStatus.Text = "Sample Accepted By Lab";
                    Session["SampleId"] = sampleID;
                    hypSampleVerification.Visible = true;
                    hypSampleVerification.NavigateUrl = string.Format("~/GenerateSampleStatus.aspx");
                    hypSampleMemo.Visible = true;
                    hypSampleMemo.NavigateUrl = string.Format("~/SampleForwardingMemo.aspx?ID1=" + InquiryID + "&ID2=" + CloneCRFSampleID);

                    cardSample.Visible = true;

                    lblSampleID.Text = dr["SampleIDSeries"].ToString();
                    hypWO.NavigateUrl = "GenerateWorkOrder.aspx?i=" + dr["EnquiryID"].ToString() + "&s=" + dr["SampleID"].ToString();
                    hypWO.Target = "_blank";
                    cardWO.Visible = true;

                    lblAllocationStatus.Text = AllocatedTo;
                    hypTestAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                    hypTestAllocation.Visible = true;
                    cardAllocation.Visible = true;
                    lblTestingStatus.Text = "Test Report Generated";
                    dt2 = _cm.getTestReportID(sampleID);
                    if (dt2.Rows.Count > 0)
                    {
                        TestResultId = Convert.ToInt32(dt2.Rows[0]["TestResultId"].ToString());
                        cardTesting.Visible = true;
                        lblTestingStatus.Text = "Completed";
                        hypTestResults.NavigateUrl = "ViewReport.aspx?ID=" + sampleUID;//string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                        hypTestResults.Visible = nabl1;
                        hypTestResultsNN.NavigateUrl = "ReportRevisionDeleteSelect.aspx?ID=" + sampleID + "&SampleID=" + sampleUID;
                        hypTestResultsNN.Visible = nonnabl1;
                    }

                    DataTable dtReportUploads = _cm.getTestReportUploads(CloneCRFSampleID);
                    if (dtReportUploads.Rows.Count > 0)
                    {
                        cardTestReportUpload.Visible = true;
                        foreach (DataRow dr1 in dtReportUploads.Rows)
                        {
                            HyperLink hyp = new HyperLink();
                            hyp.ID = "hyp" + dr1["TestReportNo"].ToString();
                            hyp.NavigateUrl += string.Format("~/TestReportUpload/" + dr1["TestReportFileName"].ToString());
                            hyp.Text = "Uploaded - " + dr1["TestReportNo"].ToString();
                            hyp.Target = "_new";
                            // PlaceHolderReportList.Controls.Add(new LiteralControl("<br />"));
                            // PlaceHolderReportList.Controls.Add(hyp);
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
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CRFID + ".htm");
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

        private void BindMessages(string SampleId)
        {
            string Date;
            string Name;
            string Message;
            txtMsg.Text = "";
            DataTable dt = new DataTable();
            DataTable dt1 = _cm.getSampleUID(Convert.ToInt32(SampleId));
            if (dt1.Rows.Count > 0)
            {
                dt = _cm.GetChatDetails(dt1.Rows[0]["sampleID"].ToString());
            }

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Date = dt.Rows[i]["Date"].ToString();
                    Name = dt.Rows[i]["EmployeeName"].ToString();
                    Message = dt.Rows[i]["Message"].ToString();
                    txtMsg.Text += Date + "   " + Name + "::" + Message + Environment.NewLine;
                }
            }
        }
        protected void tmMsg_Tick(object sender, EventArgs e)
        {
            if (Session["SampleId"] != null)
            {
                string SampleId = Session["SampleId"].ToString();
                BindMessages(SampleId);
            }
        }
        void Popup(bool isDisplay)
        {
            StringBuilder builder = new StringBuilder();
            if (isDisplay)
            {
                builder.Append("<script language=JavaScript> showModal(); </script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPopup", builder.ToString());
            }
            else
            {
                builder.Append("<script language=JavaScript> HidePopup(); </script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "HidePopup", builder.ToString());
            }
        }
        protected void btnChat_Click(object sender, EventArgs e)
        {
            if (Session["SampleId"] != null)
            {
                string SampleId = Session["SampleId"].ToString();
                Popup(true);
                BindMessages(SampleId);
            }
        }


        /* ---------------------| New Code Start (QR Code Remove and DRAFT Watermark for Draft File , and For final as it is) |---------------------- */

        private string _selectedLanguage = "EN"; // Default to English
        private const float MinQrSize = 40f;   // points
        private const float MaxQrSize = 160f;  // points
        private const float AspectTolerance = 0.15f; // 15% width/height difference allowed //        
        private byte[] CleanDocxBytes;
        private byte[] WatermarkedDocxBytes;

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string CRFID = Session["CRFID"].ToString();
                bool hasEN = fpTestReportUploadEN.HasFile;
                bool hasBH = fpTestReportUploadBH.HasFile;
                if (!hasEN && !hasBH)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify",
                        "alert('Select at least one file (English or Bahasa).');", true);
                    fpTestReportUploadEN.BorderColor = System.Drawing.Color.Red;
                    fpTestReportUploadBH.BorderColor = System.Drawing.Color.Red;
                    return;
                }

                const int MAX_FILE_SIZE = 2097152; // 2MB in bytes (1024 * 1024 * 2) // Check file sizes for each uploaded file //
                if (hasEN && fpTestReportUploadEN.PostedFile.ContentLength > MAX_FILE_SIZE)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify",
                        "alert('English report file size exceeds 2MB limit. Please upload a smaller file.');", true);
                    fpTestReportUploadEN.BorderColor = System.Drawing.Color.Red;
                    return;
                }

                if (hasBH && fpTestReportUploadBH.PostedFile.ContentLength > MAX_FILE_SIZE)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify",
                        "alert('Bahasa report file size exceeds 2MB limit. Please upload a smaller file.');", true);
                    fpTestReportUploadBH.BorderColor = System.Drawing.Color.Red;
                    return;
                }

                // ---- Fetch revision info ONCE, shared across both language runs ---- //
                int CloneCRFSampleID = Convert.ToInt32(Request.QueryString["ID"]);
                DataTable dt = _cm.GetRevisionCountCRFSampleIdWise(CloneCRFSampleID);

                int revisionCount = 1;
                string SampleUID = "";
                int Sample_Id = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    revisionCount = row["RevisionCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["RevisionCount"].ToString()) + 1;
                    if (revisionCount == 0)
                    {
                        revisionCount = revisionCount + 1;
                    }
                    SampleUID = row["SampleUID"].ToString();
                    Sample_Id = Convert.ToInt32(row["Sample_ID"].ToString());
                }

                string qrCodeNumberEN = null;
                string qrCodeNumberBH = null;

                if (hasEN)
                {
                    qrCodeNumberEN = ProcessUploadedReport(fpTestReportUploadEN, "EN", revisionCount, SampleUID, Sample_Id);
                }

                if (hasBH)
                {
                    qrCodeNumberBH = ProcessUploadedReport(fpTestReportUploadBH, "BH", revisionCount, SampleUID, Sample_Id);
                }

                // ---- Update DB ONCE, regardless of whether 1 or 2 files were uploaded ---- //
                bool updateSuccess = _cm.UpdateQRCodeNumbers(Sample_Id, qrCodeNumberEN, qrCodeNumberBH, revisionCount);
                if (updateSuccess)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('File Upload Successfully');</script>");
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something went wrong, File not upload!');</script>");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message.Replace("'", "\\'");
                string script = $"alert('Error: {errorMessage}');";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NotifyError", script, true);
            }
        }
        private string ProcessUploadedReport(FileUpload upload, string language, int revisionCount, string sampleUID, int sample_Id)
        {
            string fileExtension = Path.GetExtension(upload.FileName).ToLower();
            if (fileExtension != ".docx" && fileExtension != ".doc")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify",
                    $"alert('{language} file must be a valid Word document (.docx or .doc).');", true);
                return null;
            }

            _selectedLanguage = language;
            return ConvertWordToPdf(upload.FileBytes, upload.FileName, revisionCount, sampleUID, sample_Id);
        }

        private string ConvertWordToPdf(byte[] wordBytes, string uploadedFileName, int revisionCount, string SampleUID, int Sample_Id)
        {
            System.Drawing.Bitmap newQrBitmap = _selectedLanguage == "BH"
                ? GenerateQR_BH(SampleUID, revisionCount)
                : GenerateQR(SampleUID, revisionCount);

            string baseFileName = Path.GetFileNameWithoutExtension(uploadedFileName);
            string languageSuffix = _selectedLanguage == "BH" ? "_BH" : "";

            try
            {
                // ---------- CLEAN DOCX + PDF (new QR inserted, no watermark) ----------
                using (MemoryStream wordStream1 = new MemoryStream(wordBytes))
                using (WordDocument cleanDoc = new WordDocument(wordStream1, FormatType.Docx))
                {
                    if (cleanDoc.Sections.Count > 0)
                    {
                        ReplaceQrCodeInEntities(cleanDoc.Sections[0].Body.ChildEntities, newQrBitmap);
                    }

                    // Save modified DOCX
                    using (MemoryStream cleanDocxStream = new MemoryStream())
                    {
                        cleanDoc.Save(cleanDocxStream, FormatType.Docx);
                        CleanDocxBytes = cleanDocxStream.ToArray();
                    }

                    DocIORenderer renderer1 = new DocIORenderer();
                    PdfDocument cleanPdf = renderer1.ConvertToPDF(cleanDoc);
                    using (MemoryStream cleanStream = new MemoryStream())
                    {
                        cleanPdf.Save(cleanStream);
                        CleanPdfBytes = cleanStream.ToArray();
                    }
                    cleanPdf.Close(true);
                    renderer1.Dispose();
                }

                //\\ ---------- DRAFT DOCX + PDF (new QR inserted, then removed on page 1 only + watermark) ----------//\\
                using (MemoryStream wordStream2 = new MemoryStream(wordBytes))
                using (WordDocument draftDoc = new WordDocument(wordStream2, FormatType.Docx))
                {
                    if (draftDoc.Sections.Count > 0)
                    {
                        ReplaceQrCodeInEntities(draftDoc.Sections[0].Body.ChildEntities, newQrBitmap);
                    }

                    _pastFirstPage = false;
                    RemoveQrCodeFirstPageOnly(draftDoc);

                    using (MemoryStream draftDocxStream = new MemoryStream())
                    {
                        draftDoc.Save(draftDocxStream, FormatType.Docx);
                        WatermarkedDocxBytes = draftDocxStream.ToArray();
                    }

                    DocIORenderer renderer2 = new DocIORenderer();
                    PdfDocument draftPdf = renderer2.ConvertToPDF(draftDoc);
                    AddDraftWatermark(draftPdf);
                    using (MemoryStream watermarkedStream = new MemoryStream())
                    {
                        draftPdf.Save(watermarkedStream);
                        WatermarkedPdfBytes = watermarkedStream.ToArray();
                    }
                    draftPdf.Close(true);
                    renderer2.Dispose();
                }

                // ---------- Save PDFs AND DOCX files to disk (no DB update here anymore) ----------
                SaveGeneratedFilesToDisk(baseFileName, languageSuffix, revisionCount, SampleUID);

                // ---------- Return the QR filename for this language so caller can do ONE combined DB update ----------
                return _selectedLanguage == "BH"
                    ? $"{SampleUID}_Rev{revisionCount}_BH.png"
                    : $"{SampleUID}_Rev{revisionCount}.png";
            }
            finally
            {
                newQrBitmap?.Dispose();
            }
        }

        private void SaveGeneratedFilesToDisk(string baseFileName, string languageSuffix, int revisionCount, string sampleUID)
        {
            string folderPath = Server.MapPath("~/TestReport");
            string folderPath_Draft = Server.MapPath("~/DraftFiles");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!Directory.Exists(folderPath_Draft))
            {
                Directory.CreateDirectory(folderPath_Draft);
            }

            string cleanPath = Path.Combine(folderPath, $"TestReport-{sampleUID}_Rev{revisionCount}{languageSuffix}.pdf");
            string watermarkedPath = Path.Combine(folderPath_Draft, $"TestReport-{sampleUID}_Rev{revisionCount}{languageSuffix}_Draft.pdf");

            File.WriteAllBytes(cleanPath, CleanPdfBytes);
            File.WriteAllBytes(watermarkedPath, WatermarkedPdfBytes);

            string cleanDocxPath = Path.Combine(folderPath, $"TestReport-{sampleUID}_Rev{revisionCount}{languageSuffix}.docx");
            string watermarkedDocxPath = Path.Combine(folderPath_Draft, $"TestReport-{sampleUID}_Rev{revisionCount}{languageSuffix}_Draft.docx");

            if (CleanDocxBytes != null)
            {
                File.WriteAllBytes(cleanDocxPath, CleanDocxBytes);
            }

            if (WatermarkedDocxBytes != null)
            {
                File.WriteAllBytes(watermarkedDocxPath, WatermarkedDocxBytes);
            }

            // NOTE: UpdateQRCodeNumbers moved out of here — now called ONCE in btnUpload_Click
            // after both (or the single) language(s) finish processing.
        }

        private void RemoveQrCodeFirstPageOnly(WordDocument wordDocument)
        {
            if (wordDocument.Sections.Count == 0) return;
            _pastFirstPage = false; // Reset before processing
            RemoveQrFromEntities(wordDocument.Sections[0].Body.ChildEntities);
        }

        private void RemoveQrFromEntities(EntityCollection entities)
        {
            if (entities == null || _pastFirstPage) return;

            for (int i = 0; i < entities.Count; i++)
            {
                if (_pastFirstPage) return;

                Entity entity = entities[i];

                if (entity is WParagraph paragraph)
                {
                    RemoveQrFromParagraph(paragraph);
                }
                else if (entity is WTable table)
                {
                    foreach (WTableRow row in table.Rows)
                    {
                        if (_pastFirstPage) return;
                        foreach (WTableCell cell in row.Cells)
                        {
                            if (_pastFirstPage) return;
                            RemoveQrFromEntities(cell.ChildEntities);
                        }
                    }
                }
            }
        }

        private void RemoveQrFromParagraph(WParagraph paragraph)
        {
            if (paragraph.ParagraphFormat.PageBreakBefore)
            {
                _pastFirstPage = true;
                return;
            }

            for (int j = 0; j < paragraph.ChildEntities.Count; j++)
            {
                Entity child = paragraph.ChildEntities[j];
                if (child is Break brk && brk.BreakType == BreakType.PageBreak)
                {
                    _pastFirstPage = true;
                    return; // stop — anything after this break is page 2+
                }

                if (child is WPicture picture)
                {
                    if (LooksLikeQrCode(picture))
                    {
                        paragraph.ChildEntities.RemoveAt(j);
                        j--; // re-check this index since collection shifted
                    }
                }
                else if (child is Shape shape && shape.TextBody != null)
                {
                    RemoveQrFromEntities(shape.TextBody.ChildEntities);
                }
                else if (child is WTextBox textBox && textBox.TextBoxBody != null)
                {
                    RemoveQrFromEntities(textBox.TextBoxBody.ChildEntities);
                }

                if (_pastFirstPage) return;
            }
        }

        // ========== NEW: REPLACE QR CODE IN PLACE (used before rendering, both Clean & Draft) ========== //
        private void ReplaceQrCodeInEntities(EntityCollection entities, System.Drawing.Image newQrImage)
        {
            if (entities == null || newQrImage == null) return;

            foreach (Entity entity in entities)
            {
                if (entity is WParagraph paragraph)
                {
                    ReplaceQrCodeInParagraph(paragraph, newQrImage);
                }
                else if (entity is WTable table)
                {
                    foreach (WTableRow row in table.Rows)
                    {
                        foreach (WTableCell cell in row.Cells)
                        {
                            ReplaceQrCodeInEntities(cell.ChildEntities, newQrImage);
                        }
                    }
                }
            }
        }

        private void ReplaceQrCodeInParagraph(WParagraph paragraph, System.Drawing.Image newQrImage)
        {
            foreach (Entity child in paragraph.ChildEntities)
            {
                if (child is WPicture picture && LooksLikeQrCode(picture))
                {
                    picture.LoadImage(ImageToByteArray(newQrImage)); // swaps the underlying image, keeps size/position
                }
                else if (child is Shape shape && shape.TextBody != null)
                {
                    ReplaceQrCodeInEntities(shape.TextBody.ChildEntities, newQrImage);
                }
                else if (child is WTextBox textBox && textBox.TextBoxBody != null)
                {
                    ReplaceQrCodeInEntities(textBox.TextBoxBody.ChildEntities, newQrImage);
                }
            }
        }

        // NEW: helper to convert the QR Bitmap/Image into the byte[] that LoadImage expects
        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private bool LooksLikeQrCode(WPicture picture)
        {
            float width = picture.Width;
            float height = picture.Height;

            if (width <= 0 || height <= 0)
                return false;

            bool isNearSquare = Math.Abs(width - height) / Math.Max(width, height) <= AspectTolerance;
            bool isInSizeRange = width >= MinQrSize && width <= MaxQrSize && height >= MinQrSize && height <= MaxQrSize;

            return isNearSquare && isInSizeRange;
        }

        private void AddDraftWatermark(PdfDocument pdfDocument)
        {
            PdfFont watermarkFont = new PdfStandardFont(PdfFontFamily.Helvetica, 80, PdfFontStyle.Bold);

            foreach (PdfPageBase page in pdfDocument.Pages)
            {
                Syncfusion.Drawing.SizeF pageSize = page.Size;

                PdfGraphics graphics = page.Graphics;
                graphics.Save();
                graphics.SetTransparency(0.4f);
                graphics.TranslateTransform(pageSize.Width / 2, pageSize.Height / 2);
                graphics.RotateTransform(-45);
                graphics.TranslateTransform(-pageSize.Width / 2, -pageSize.Height / 2);

                PdfBrush brush = new PdfSolidBrush(new PdfColor(128, 128, 128));
                PdfStringFormat format = new PdfStringFormat();
                format.Alignment = PdfTextAlignment.Center;
                format.LineAlignment = PdfVerticalAlignment.Middle;

                graphics.DrawString("DRAFT", watermarkFont, brush,
                    new Syncfusion.Drawing.RectangleF(0, 0, pageSize.Width, pageSize.Height), format);

                graphics.Restore();
            }
        }

        protected void lnkCleanPdf_Click(object sender, EventArgs e)
        {
            byte[] pdfBytes = CleanPdfBytes;
            if (pdfBytes != null && pdfBytes.Length > 0)
            {
                string languageSuffix = _selectedLanguage == "BH" ? "_BH" : "";
                DownloadPdf(pdfBytes, $"Clean_Document{languageSuffix}.pdf");
            }
            else
            {
                //ShowStatus("Clean PDF not found. Please upload the document again.", false);
            }
        }

        protected void lnkWatermarkedPdf_Click(object sender, EventArgs e)
        {
            byte[] pdfBytes = WatermarkedPdfBytes;
            if (pdfBytes != null && pdfBytes.Length > 0)
            {
                string languageSuffix = _selectedLanguage == "BH" ? "_BH" : "";
                DownloadPdf(pdfBytes, $"Watermarked_Draft{languageSuffix}.pdf");
            }
            else
            {
                //ShowStatus("Watermarked PDF not found. Please upload the document again.", false);
            }
        }

        private void DownloadPdf(byte[] pdfBytes, string fileName)
        {
            try
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                Response.AddHeader("Content-Length", pdfBytes.Length.ToString());
                Response.BinaryWrite(pdfBytes);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                //ShowStatus($"Error downloading file: {ex.Message}", false);
            }
        }

        // ========== GENERATE QR CODE ========== // 
        // CHANGED: now returns the Bitmap (instead of void) so ConvertWordToPdf can insert it into the document //
        protected System.Drawing.Bitmap GenerateQR(string SampleID, int revisionCount)
        {
            System.Drawing.Bitmap barcodeBitmap = null;
            try
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string QRCodeFilePath = baseUrl + $"/TestReport/TestReport-{SampleID}_Rev{revisionCount}.pdf";
                string QRCodeFileName = $"{SampleID}_Rev{revisionCount}.png";

                string barcodeFolderPath = Server.MapPath("~/TestReport/");
                if (!Directory.Exists(barcodeFolderPath))
                {
                    Directory.CreateDirectory(barcodeFolderPath);
                }

                BarcodeWriter barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;

                var options = new ZXing.Common.EncodingOptions
                {
                    Width = 3000,
                    Height = 3000,
                    Margin = 2
                };
                barcodeWriter.Options = options;

                barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);

                string barcodeFilePath = Path.Combine(barcodeFolderPath, QRCodeFileName);
                barcodeBitmap.Save(barcodeFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error generating QR: " + ex.Message);
            }
            return barcodeBitmap; // NEW
        }

        protected System.Drawing.Bitmap GenerateQR_BH(string SampleID, int revisionCount)
        {
            System.Drawing.Bitmap barcodeBitmap = null;
            try
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string QRCodeFilePath = baseUrl + $"/TestReport/TestReport-{SampleID}_Rev{revisionCount}_BH.pdf";
                string QRCodeFileName = $"{SampleID}_Rev{revisionCount}_BH.png";

                string barcodeFolderPath = Server.MapPath("~/TestReport/");
                if (!Directory.Exists(barcodeFolderPath))
                {
                    Directory.CreateDirectory(barcodeFolderPath);
                }

                BarcodeWriter barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;

                var options = new ZXing.Common.EncodingOptions
                {
                    Width = 3000,
                    Height = 3000,
                    Margin = 2
                };
                barcodeWriter.Options = options;

                barcodeBitmap = barcodeWriter.Write(QRCodeFilePath);

                string barcodeFilePath = Path.Combine(barcodeFolderPath, QRCodeFileName);
                barcodeBitmap.Save(barcodeFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error generating QR: " + ex.Message);
            }
            return barcodeBitmap; // NEW
        }



        private byte[] ProcessUploadedImage(FileUpload fileUpload)
        {
            const int MAX_SIZE_KB = 300;
            const int MAX_SIZE_BYTES = MAX_SIZE_KB * 1024;
            const int TARGET_WIDTH = 1056;
            const int TARGET_HEIGHT = 704;
            const int TARGET_DPI = 200;

            try
            {
                byte[] imageBytes = fileUpload.FileBytes;

                // If already under 300KB, return as-is
                if (imageBytes.Length <= MAX_SIZE_BYTES)
                {
                    return imageBytes;
                }

                using (MemoryStream ms = new MemoryStream(imageBytes))
                using (DrawingImage originalImage = DrawingImage.FromStream(ms))
                {
                    // Calculate new dimensions maintaining aspect ratio
                    int newWidth, newHeight;
                    double aspectRatio = (double)originalImage.Width / originalImage.Height;

                    // If image is smaller than target, keep original size but still compress
                    if (originalImage.Width <= TARGET_WIDTH && originalImage.Height <= TARGET_HEIGHT)
                    {
                        newWidth = originalImage.Width;
                        newHeight = originalImage.Height;
                    }
                    else if (aspectRatio > (double)TARGET_WIDTH / TARGET_HEIGHT)
                    {
                        newWidth = TARGET_WIDTH;
                        newHeight = (int)(TARGET_WIDTH / aspectRatio);
                    }
                    else
                    {
                        newHeight = TARGET_HEIGHT;
                        newWidth = (int)(TARGET_HEIGHT * aspectRatio);
                    }

                    using (Bitmap resizedImage = new Bitmap(newWidth, newHeight))
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {
                        // Set high quality rendering
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                        // Set DPI
                        resizedImage.SetResolution(TARGET_DPI, TARGET_DPI);

                        byte[] resultImage = null;
                        long quality = 95; // Start with high quality

                        // Try quality levels from 95 down to 10
                        while (quality >= 10)
                        {
                            using (MemoryStream outputMs = new MemoryStream())
                            {
                                SaveImageAsJpeg(resizedImage, outputMs, quality);
                                byte[] tempBytes = outputMs.ToArray();

                                // Check if we got under 300KB
                                if (tempBytes.Length <= MAX_SIZE_BYTES)
                                {
                                    resultImage = tempBytes;
                                    break;
                                }

                                quality -= 5;
                            }
                        }

                        // If still too large, try exact dimensions with lower quality
                        if (resultImage == null && (newWidth != TARGET_WIDTH || newHeight != TARGET_HEIGHT))
                        {
                            using (Bitmap exactImage = new Bitmap(TARGET_WIDTH, TARGET_HEIGHT))
                            using (Graphics graphicsExact = Graphics.FromImage(exactImage))
                            {
                                graphicsExact.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphicsExact.SmoothingMode = SmoothingMode.HighQuality;
                                graphicsExact.DrawImage(originalImage, 0, 0, TARGET_WIDTH, TARGET_HEIGHT);
                                exactImage.SetResolution(TARGET_DPI, TARGET_DPI);

                                quality = 85;
                                while (quality >= 10)
                                {
                                    using (MemoryStream outputMs = new MemoryStream())
                                    {
                                        SaveImageAsJpeg(exactImage, outputMs, quality);
                                        byte[] tempBytes = outputMs.ToArray();

                                        if (tempBytes.Length <= MAX_SIZE_BYTES)
                                        {
                                            resultImage = tempBytes;
                                            break;
                                        }
                                        quality -= 5;
                                    }
                                }
                            }
                        }

                        // If STILL too large, resize to smaller dimensions
                        if (resultImage == null)
                        {
                            int smallerWidth = 800;
                            int smallerHeight = (int)(800 / aspectRatio);

                            using (Bitmap smallImage = new Bitmap(smallerWidth, smallerHeight))
                            using (Graphics graphicsSmall = Graphics.FromImage(smallImage))
                            {
                                graphicsSmall.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphicsSmall.SmoothingMode = SmoothingMode.HighQuality;
                                graphicsSmall.DrawImage(originalImage, 0, 0, smallerWidth, smallerHeight);
                                smallImage.SetResolution(TARGET_DPI, TARGET_DPI);

                                quality = 75;
                                while (quality >= 10)
                                {
                                    using (MemoryStream outputMs = new MemoryStream())
                                    {
                                        SaveImageAsJpeg(smallImage, outputMs, quality);
                                        byte[] tempBytes = outputMs.ToArray();

                                        if (tempBytes.Length <= MAX_SIZE_BYTES)
                                        {
                                            resultImage = tempBytes;
                                            break;
                                        }
                                        quality -= 5;
                                    }
                                }
                            }
                        }

                        // If STILL too large, try even smaller (600px)
                        if (resultImage == null)
                        {
                            int smallerWidth = 600;
                            int smallerHeight = (int)(600 / aspectRatio);

                            using (Bitmap smallImage = new Bitmap(smallerWidth, smallerHeight))
                            using (Graphics graphicsSmall = Graphics.FromImage(smallImage))
                            {
                                graphicsSmall.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphicsSmall.SmoothingMode = SmoothingMode.HighQuality;
                                graphicsSmall.DrawImage(originalImage, 0, 0, smallerWidth, smallerHeight);
                                smallImage.SetResolution(TARGET_DPI, TARGET_DPI);

                                quality = 60;
                                while (quality >= 10)
                                {
                                    using (MemoryStream outputMs = new MemoryStream())
                                    {
                                        SaveImageAsJpeg(smallImage, outputMs, quality);
                                        byte[] tempBytes = outputMs.ToArray();

                                        if (tempBytes.Length <= MAX_SIZE_BYTES)
                                        {
                                            resultImage = tempBytes;
                                            break;
                                        }
                                        quality -= 5;
                                    }
                                }
                            }
                        }

                        // If STILL too large, try very small (400px)
                        if (resultImage == null)
                        {
                            int smallerWidth = 400;
                            int smallerHeight = (int)(400 / aspectRatio);

                            using (Bitmap smallImage = new Bitmap(smallerWidth, smallerHeight))
                            using (Graphics graphicsSmall = Graphics.FromImage(smallImage))
                            {
                                graphicsSmall.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphicsSmall.SmoothingMode = SmoothingMode.HighQuality;
                                graphicsSmall.DrawImage(originalImage, 0, 0, smallerWidth, smallerHeight);
                                smallImage.SetResolution(TARGET_DPI, TARGET_DPI);

                                quality = 50;
                                while (quality >= 10)
                                {
                                    using (MemoryStream outputMs = new MemoryStream())
                                    {
                                        SaveImageAsJpeg(smallImage, outputMs, quality);
                                        byte[] tempBytes = outputMs.ToArray();

                                        if (tempBytes.Length <= MAX_SIZE_BYTES)
                                        {
                                            resultImage = tempBytes;
                                            break;
                                        }
                                        quality -= 5;
                                    }
                                }
                            }
                        }

                        return resultImage; // Will return null if still too large
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error if needed
                return null;
            }
        }

        private void SaveImageAsJpeg(DrawingImage image, MemoryStream ms, long quality)
        {
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            // Get JPEG codec
            ImageCodecInfo jpegCodec = GetJpegCodec();

            if (jpegCodec != null)
            {
                image.Save(ms, jpegCodec, encoderParams);
            }
            else
            {
                image.Save(ms, ImageFormat.Jpeg);
            }
        }

        private ImageCodecInfo GetJpegCodec()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    // Get the row index from command argument
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the ID and filename from DataKeys
                    int imageId = Convert.ToInt32(GridView1.DataKeys[rowIndex].Values["ID"]);
                    string fileName = GridView1.DataKeys[rowIndex].Values["filename"].ToString();

                    // Delete physical file
                    bool fileDeleted = false;
                    string fileMessage = "";

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string folderPath = Server.MapPath("SampleImages/");
                        string filePath = Path.Combine(folderPath, fileName);

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                                fileDeleted = true;
                                fileMessage = "File deleted successfully.";
                            }
                            catch (Exception ex)
                            {
                                fileDeleted = false;
                                fileMessage = "File could not be deleted: " + ex.Message;
                            }
                        }
                        else
                        {
                            fileMessage = "File not found in folder.";
                        }
                    }
                    else
                    {
                        fileMessage = "No file associated with this record.";
                    }

                    // Delete from database
                    bool isDeleted = _cm.DeleteSampleImage(imageId);

                    if (isDeleted)
                    {
                        string successMessage = "Image deleted successfully! ";
                        if (fileDeleted)
                        {
                            successMessage += "File also removed from folder.";
                            lblMessage.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            successMessage += "Note: " + fileMessage;
                            lblMessage.ForeColor = System.Drawing.Color.Orange;
                        }
                        lblMessage.Text = successMessage;
                        BindSampleImages();
                    }
                    else
                    {
                        lblMessage.Text = "Error deleting image from database!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
        private void BindSampleImages()
        {
            int InquiryId = Convert.ToInt32(ViewState["InquiryID"]);
            DataTable dt = _cm.GetSampleImages(InquiryId);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void btnUploadImage_Click(object sender, EventArgs e)
        {
            int flag = 0;
            _cm.inquiryId = Convert.ToInt32(lblInquiryID.Text);
            //string SampleID = Session["ID"].ToString();
            // _cm.sampleId = Convert.ToInt32(SampleID);
            if (txtSampleName.Text == "")
            {
                txtSampleName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSampleName.BackColor = System.Drawing.Color.Empty;

                if (!fpSampleImage.HasFile)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Select File');", true);
                    fpSampleImage.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
                else
                {
                    // Check if file is JPG/JPEG
                    string fileExtension = Path.GetExtension(fpSampleImage.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                            "alert('Only JPG and JPEG files are allowed. Please select a JPG or JPEG image.');", true);
                        fpSampleImage.BorderColor = Color.Red;
                        return;
                    }

                    int count = 1;
                    fpSampleImage.BorderColor = System.Drawing.Color.Empty;
                    string folderPath = Server.MapPath("SampleImages/");
                    //DataTable chkReport = _cm.CheckSampleImageExists(_cm);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    DataTable chkReport = _cm.CheckSampleImageExistsOfEnquiry(_cm);
                    if (chkReport.Rows.Count > 0)
                    {
                        count = chkReport.Rows.Count + 1;
                    }

                    // Process the uploaded image (resize if needed)
                    byte[] processedImage = ProcessUploadedImage(fpSampleImage);

                    if (processedImage == null)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                            "alert('Failed to process image. Please try with a different image.');", true);
                        return;
                    }

                    // ***** CHECK: Verify the processed image is under 300KB *****
                    const int MAX_SIZE_KB = 300;
                    const int MAX_SIZE_BYTES = MAX_SIZE_KB * 1024;

                    if (processedImage.Length > MAX_SIZE_BYTES)
                    {
                        // File is STILL too large even after processing
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                            $"alert('Image could not be compressed to under {MAX_SIZE_KB}KB. ' + \n" +
                            $"'Current size: {(processedImage.Length / 1024)}KB. ' + \n" +
                            "'Please upload a smaller image or reduce the image quality.');", true);
                        fpSampleImage.BorderColor = Color.Red;
                        return;
                    }

                    // Generate filename - always save as .jpg
                    string fileName = lblInquiryID.Text + "-" + count + ".jpg";
                    string filePath = Path.Combine(folderPath, fileName);

                    // Save the processed image
                    File.WriteAllBytes(filePath, processedImage);

                    _cm.SampleFileName = fileName;
                    _cm.SampleText = txtSampleName.Text;
                    _cm.created = DateTime.Now.AddHours(12.5);

                    int ID = _cm.SampleImageUpload(_cm);
                    BindSampleImages();
                    lblMessage.Text = "";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('File Upload Successful')", true);
                }
            }

        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                string fileName = rowView["filename"].ToString();
                System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgSample");
                if (img != null && !string.IsNullOrEmpty(fileName))
                {
                    /* string smtpUrl = "http://localhost:41414"; */
                    string smtpUrl = Session["WebsiteURL"] as string ?? "http://localhost:41414";
                    string imageUrl = smtpUrl + "/SampleImages/" + fileName;
                    img.ImageUrl = imageUrl;
                }
            }
        }
    }
}