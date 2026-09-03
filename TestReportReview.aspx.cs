using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class TestReportReview : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0; int CloneCRFSampleID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            //HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            //HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            //HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            //HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            //HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            //HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            //HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            //HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            //HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            //HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
            //HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
            //HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
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
                if (Session["EmployeeID"] != null)
                {
                    EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                }
                if (!Page.IsPostBack)
                {
                    //Session["InquiryID"] = "47";
                    //Session["CloneCRFSampleID"] = "57";

                    if (Session["InquiryID"] != null)
                    {
                        if (Session["CloneCRFSampleID"] != null)
                        {
                            int InquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                            CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                            lblInquiryID.Text = "ENQ" + InquiryId.ToString();
                            BindProductCategory(InquiryId);
                            BindSampleCheckList();
                        }
                    }
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    //switch (RoleID)
                    //{
                    //    case 1://QC
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 2://Manager
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 3://Lab Technician
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = false;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 4://Admin
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 5://Sample Management
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }


        private void BindProductCategory(int InquiryId)
        {
            DataTable dt = _cm.GetProductCategory(InquiryId);
            DataTable dt1 = _cm.GetCRFSample(InquiryId);
            if (dt.Rows.Count > 0)
            {
                hdSampleCategory.Value = dt.Rows[0]["SampleCategory"].ToString();
                lblCategoryType.Text = dt.Rows[0]["ProductGroupName"].ToString();
                lblCategoryName.Text = dt.Rows[0]["SubGroupName"].ToString();
                hdSampleHistory.Value = dt.Rows[0]["SampleHistory"].ToString();
                hdDepartmentID.Value = dt.Rows[0]["DepartmentID"].ToString();
            }
            if (dt1.Rows.Count > 0)
            {
                lblSampleSeries.Text = dt1.Rows[0]["SampleIDseries"].ToString();
                lblSampleID.Text = dt1.Rows[0]["ID"].ToString();
            }
        }

        private void BindSampleCheckList()
        {
            DataTable dt = _cm.GetReviewCheckList();
            if (dt.Rows.Count > 0)
            {
                chkSampleCheck.DataSource = dt;
                chkSampleCheck.DataTextField = "Description";
                chkSampleCheck.DataValueField = "ReviewCheckListID";
                chkSampleCheck.DataBind();
            }
            BindSampleRadioList();
        }
        private void BindSampleRadioList()
        {
            int j = 0;
            rbCheckList.Items.Clear();
            for (int i = 0; i < chkSampleCheck.Items.Count; i++)
            {
                rbCheckList.Items.Add(new ListItem("Yes", j.ToString()));
                rbCheckList.Items.Add(new ListItem("No", (j + 1).ToString()));
                rbCheckList.Items.Add(new ListItem("N/A", (j + 2).ToString()));
                j += 3;
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

        }
        protected void Save(int ReviewStatus)
        {
            int SampleResultId = 0;
            int flag = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            Random rnd = new Random();
            string ProductCategory = hdSampleCategory.Value;
            int SampleUId = rnd.Next(10000000, 99999999);
            _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
            DataTable dtReviewcount = _cm.GetReviewCount(_cm.sampleId);
            int ReviewedBy = Convert.ToInt32(Session["EmployeeID"].ToString());
            _cm.EmployeeID = ReviewedBy;
            if (dtReviewcount.Rows.Count > 0) { _cm.ReviewCount = Convert.ToInt32(dtReviewcount.Rows[0]["ReviewCount"].ToString()); _cm.ReviewCount = _cm.ReviewCount + 1; } else { _cm.ReviewCount = 1; }
            if (flag == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    string[] x = new string[20];
                    x = hdrbCheckList.Value.Split(',');
                    for (int i = 0; i < chkSampleCheck.Items.Count; i++)
                    {
                        switch (x[i])
                        {
                            case "No":
                                _cm.ReviewChecklistStatus = 0;
                                break;
                            case "Yes":
                                _cm.ReviewChecklistStatus = 1;
                                break;
                            case "N/A":
                                _cm.ReviewChecklistStatus = 2;
                                break;
                        }
                        _cm.ReviewChecklistId = chkSampleCheck.Items[i].Value;
                        switch (i)
                        {
                            case 0: _cm.ReviewCheckListValue = txt1.Text; break;
                            case 1: _cm.ReviewCheckListValue = txt2.Text; break;
                            case 2: _cm.ReviewCheckListValue = txt3.Text; break;
                            case 3: _cm.ReviewCheckListValue = txt4.Text; break;
                            case 4: _cm.ReviewCheckListValue = txt5.Text; break;
                            case 5: _cm.ReviewCheckListValue = txt6.Text; break;
                            case 6: _cm.ReviewCheckListValue = txt7.Text; break;
                            case 7: _cm.ReviewCheckListValue = txt8.Text; break;
                            case 8: _cm.ReviewCheckListValue = txt9.Text; break;
                            case 9: _cm.ReviewCheckListValue = txt10.Text; break;
                            case 10: _cm.ReviewCheckListValue = txt11.Text; break;
                            case 11: _cm.ReviewCheckListValue = txt12.Text; break;
                            default: break;
                        }
                        _cm.AddReviewChecklistStatusDetails(_cm);
                    }

                    if (Session["CloneCRFSampleID"] != null)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
                        _cm.CRFStatusID = ReviewStatus;//Review Status
                        _cm.sampleHistory = hdSampleHistory.Value + "[" + SampleResultId + "]";
                        _cm.UpdateCRFCloneList(_cm);
                        _cm.UpdateCRFStatus(_cm);

                        _cm.sampleStatusID = ReviewStatus;//Review Status
                        _cm.updateSampleStatus(_cm);

                    }
                }
            }
            Response.Redirect("GenerateTestReportReview.aspx?s=" + _cm.sampleId + "&c=" + _cm.ReviewCount);
        }


        //protected void Save(int ReviewStatus)
        //{
        //    int SampleResultId = 0;
        //    int flag = 0; 
        //    DataTable dt = new DataTable();
        //    DataTable dt2 = new DataTable();
        //    Random rnd = new Random();
        //    string ProductCategory = hdSampleCategory.Value;
        //    int SampleUId = rnd.Next(10000000, 99999999);
        //    _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
        //    DataTable dtReviewcount = _cm.GetReviewCount(_cm.sampleId);
        //    if (dtReviewcount.Rows.Count > 0) { _cm.ReviewCount = Convert.ToInt32(dtReviewcount.Rows[0]["ReviewCount"].ToString()); _cm.ReviewCount = _cm.ReviewCount + 1; } else { _cm.ReviewCount = 1; }
        //    if (flag == 0)
        //    {
        //        for (int i = 0; i < chkSampleCheck.Items.Count; i++)
        //        {
        //            if (chkSampleCheck.Items[i].Selected == false)
        //            {
        //                if (rbCheckList.Items[i].Text == "No")
        //                {
        //                    ReviewStatus = 25;
        //                }
        //                _cm.ReviewChecklistId = chkSampleCheck.Items[i].Value;
        //                _cm.ReviewChecklistStatus = 0;

        //                switch (i)
        //                {
        //                    case 0: _cm.ReviewCheckListValue = txt1.Text; break;
        //                    case 1: _cm.ReviewCheckListValue = txt2.Text; break;
        //                    case 2: _cm.ReviewCheckListValue = txt3.Text; break;
        //                    case 3: _cm.ReviewCheckListValue = txt4.Text; break;
        //                    case 4: _cm.ReviewCheckListValue = txt5.Text; break;
        //                    case 5: _cm.ReviewCheckListValue = txt6.Text; break;
        //                    case 6: _cm.ReviewCheckListValue = txt7.Text; break;
        //                    case 7: _cm.ReviewCheckListValue = txt8.Text; break;
        //                    case 8: _cm.ReviewCheckListValue = txt9.Text; break;
        //                    case 9: _cm.ReviewCheckListValue = txt10.Text; break;
        //                    case 10: _cm.ReviewCheckListValue = txt11.Text; break;
        //                    case 11: _cm.ReviewCheckListValue = txt12.Text; break;
        //                    default: break;
        //                }
        //                _cm.AddReviewChecklistStatusDetails(_cm);
        //            }
        //            else if (chkSampleCheck.Items[i].Selected == true)
        //            {
        //                _cm.ReviewChecklistId = chkSampleCheck.Items[i].Value;
        //                if (rbCheckList.Items[i].Text == "NA")
        //                {
        //                    _cm.ReviewChecklistStatus = 2;
        //                }

        //                _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
        //                switch (i)
        //                {
        //                    case 0: _cm.ReviewCheckListValue = txt1.Text; break;
        //                    case 1: _cm.ReviewCheckListValue = txt2.Text; break;
        //                    case 2: _cm.ReviewCheckListValue = txt3.Text; break;
        //                    case 3: _cm.ReviewCheckListValue = txt4.Text; break;
        //                    case 4: _cm.ReviewCheckListValue = txt5.Text; break;
        //                    case 5: _cm.ReviewCheckListValue = txt6.Text; break;
        //                    case 6: _cm.ReviewCheckListValue = txt7.Text; break;
        //                    case 7: _cm.ReviewCheckListValue = txt8.Text; break;
        //                    case 8: _cm.ReviewCheckListValue = txt9.Text; break;
        //                    case 9: _cm.ReviewCheckListValue = txt10.Text; break;
        //                    case 10: _cm.ReviewCheckListValue = txt11.Text; break;
        //                    case 11: _cm.ReviewCheckListValue = txt12.Text; break;
        //                    default: break;
        //                }
        //                _cm.AddReviewChecklistStatusDetails(_cm);
        //            }
        //            if (Session["CloneCRFSampleID"] != null)
        //            {
        //                CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
        //                _cm.CloneCRFSampleID = CloneCRFSampleID;
        //                _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
        //                _cm.CRFStatusID = ReviewStatus;//Review Status
        //                _cm.sampleHistory = hdSampleHistory.Value + "[" + SampleResultId + "]";
        //                _cm.UpdateCRFCloneList(_cm);
        //                _cm.UpdateCRFStatus(_cm);

        //                _cm.sampleStatusID = ReviewStatus;//Review Status
        //                _cm.updateSampleStatus(_cm);

        //            }

        //        }
        //        Response.Redirect("GenerateTestReportReview.aspx?s=" + _cm.sampleId + "&c=" + _cm.ReviewCount);
        //    }
        //    else
        //    {
        //        dt = _cm.getSampleUID(SampleUId);

        //        while (dt.Rows.Count == 1)
        //        {
        //            dt = _cm.getSampleUID(SampleUId);
        //        }
        //        string sYear = DateTime.Now.AddHours(12.5).ToString("yy");
        //        string sMonth = DateTime.Now.AddHours(12.5).ToString("MM");
        //        dt = _cm.getLastSampleID();
        //        if (dt.Rows.Count > 0)
        //        {
        //            Session["SampleId"] = SampleResultId;

        //            for (int i = 0; i < chkSampleCheck.Items.Count; i++)
        //            {
        //                if (chkSampleCheck.Items[i].Selected == false)
        //                {
        //                    _cm.ReviewChecklistId = chkSampleCheck.Items[i].Value;
        //                    _cm.ReviewChecklistStatus = 0;
        //                    _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
        //                    switch (i)
        //                    {
        //                        case 0: _cm.ReviewCheckListValue = txt1.Text; break;
        //                        case 1: _cm.ReviewCheckListValue = txt2.Text; break;
        //                        case 2: _cm.ReviewCheckListValue = txt3.Text; break;
        //                        case 3: _cm.ReviewCheckListValue = txt4.Text; break;
        //                        case 4: _cm.ReviewCheckListValue = txt5.Text; break;
        //                        case 5: _cm.ReviewCheckListValue = txt6.Text; break;
        //                        case 6: _cm.ReviewCheckListValue = txt7.Text; break;
        //                        case 7: _cm.ReviewCheckListValue = txt8.Text; break;
        //                        case 8: _cm.ReviewCheckListValue = txt9.Text; break;
        //                        case 9: _cm.ReviewCheckListValue = txt10.Text; break;
        //                        case 10: _cm.ReviewCheckListValue = txt11.Text; break;
        //                        case 11: _cm.ReviewCheckListValue = txt12.Text; break;
        //                        default: break;
        //                    }
        //                    _cm.AddReviewChecklistStatusDetails(_cm);
        //                }
        //                else if (chkSampleCheck.Items[i].Selected == true)
        //                {
        //                    _cm.ReviewChecklistId = chkSampleCheck.Items[i].Value;
        //                    _cm.ReviewChecklistStatus = 1;
        //                    _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
        //                    switch (i)
        //                    {
        //                        case 0: _cm.ReviewCheckListValue = txt1.Text; break;
        //                        case 1: _cm.ReviewCheckListValue = txt2.Text; break;
        //                        case 2: _cm.ReviewCheckListValue = txt3.Text; break;
        //                        case 3: _cm.ReviewCheckListValue = txt4.Text; break;
        //                        case 4: _cm.ReviewCheckListValue = txt5.Text; break;
        //                        case 5: _cm.ReviewCheckListValue = txt6.Text; break;
        //                        case 6: _cm.ReviewCheckListValue = txt7.Text; break;
        //                        case 7: _cm.ReviewCheckListValue = txt8.Text; break;
        //                        case 8: _cm.ReviewCheckListValue = txt9.Text; break;
        //                        case 9: _cm.ReviewCheckListValue = txt10.Text; break;
        //                        case 10: _cm.ReviewCheckListValue = txt11.Text; break;
        //                        case 11: _cm.ReviewCheckListValue = txt12.Text; break;
        //                        default: break;
        //                    }
        //                    _cm.AddReviewChecklistStatusDetails(_cm);
        //                }
        //                if (Session["CloneCRFSampleID"] != null)
        //                {
        //                    CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
        //                    _cm.CloneCRFSampleID = CloneCRFSampleID;
        //                    _cm.sampleId = Convert.ToInt32(lblSampleID.Text);
        //                    _cm.CRFStatusID = ReviewStatus;
        //                    _cm.sampleHistory = hdSampleHistory.Value + "[" + SampleResultId + "]";
        //                    _cm.UpdateCRFCloneList(_cm);
        //                    _cm.UpdateCRFStatus(_cm);

        //                    _cm.sampleStatusID = ReviewStatus;
        //                    _cm.updateSampleStatus(_cm);
        //                }

        //            }
        //            Response.Redirect("GenerateTestReportReview.aspx?s=" + _cm.sampleId + "&c=" + _cm.ReviewCount);
        //        }
        //        _cm.tableName = "SampleDetails";
        //        _cm.paramID = "ID";
        //        _cm.paramIDValue = SampleResultId;

        //        Response.Redirect("CustomerInquiryView.aspx");

        //    }
        //}
        protected void rbCheckList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int hdnValue = 0;

            hdnValue = Convert.ToInt32(HiddenField1.Value);

            int chk = chkSampleCheck.Items.Count;
            int rb = rbCheckList.Items.Count / 3;

            int value = Convert.ToInt32(rbCheckList.SelectedValue) / 3;
            for (int i = 0; i < chkSampleCheck.Items.Count; i++)
            {
                if (value == i)
                {
                    if (rbCheckList.SelectedItem.ToString() == "Yes")
                    {
                        chkSampleCheck.Items[value].Selected = true;
                        hdrbCheckList.Value += rbCheckList.SelectedItem.ToString() + ",";
                    }
                    if (rbCheckList.SelectedItem.ToString() == "No")
                    {
                        chkSampleCheck.Items[value].Selected = false;
                        hdrbCheckList.Value += rbCheckList.SelectedItem.ToString() + ",";
                    }
                    if (rbCheckList.SelectedItem.ToString() == "N/A")
                    {
                        chkSampleCheck.Items[value].Selected = false;
                        hdrbCheckList.Value += rbCheckList.SelectedItem.ToString() + ",";
                    }
                }

            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Save(24);//Reviewed Accepted
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            Save(25);//Reviewed Rejected
        }
    }
}