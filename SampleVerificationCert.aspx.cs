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
    public partial class SampleVerificationCert : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0; int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
            HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
            HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
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
                    if (Session["InquiryID"] != null)
                    {
                        DataTable dtReportFormat = _cm.GetReportFormat(Convert.ToInt32(Session["InquiryID"].ToString()));
                        if (dtReportFormat.Rows.Count > 0)
                            switch (dtReportFormat.Rows[0]["ReportType"].ToString())
                            {
                                case "Rake_Cert":
                                    txtReceiver.Visible = true;
                                    txtStackQty.Visible = false;
                                    txtLoadedQty.Visible = true;
                                    txtVesselName.Visible = false;

                                    div3.Visible = false;
                                    break;
                                case "Stack Sampling":
                                    txtReceiver.Visible = false;
                                    txtStackQty.Visible = true;
                                    txtLoadedQty.Visible = false;
                                    txtVesselName.Visible = false;
                                    
                                    div1.Visible = false;
                                    div2.Visible = false;
                                    div3.Visible = false;
                                    break;
                                case "Truck_Cert":
                                    txtReceiver.Visible = true;
                                    txtStackQty.Visible = false;
                                    txtLoadedQty.Visible = true;
                                    txtVesselName.Visible = false;
                                    
                                    div1.Visible = true;
                                    div2.Visible = false;
                                    div3.Visible = true;
                                    break;
                                case "VESSEL CERTIFICATE":
                                    txtReceiver.Visible = false;
                                    txtStackQty.Visible = false;
                                    txtLoadedQty.Visible = true;                                    
                                    txtTransportMode.Visible = false;
                                    txtVesselName.Visible = true;
                                   
                                    div1.Visible = true;
                                    div2.Visible = false;
                                    div3.Visible = false;
                                    div4.Visible = false;
                                    break;
                                default: break;
                            }
                        if (Session["CloneCRFSampleID"] != null)
                        {
                            int InquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                            CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                            lblInquiryID.Text = "ENQ" + InquiryId.ToString();
                            divCheckBox.Visible = false;
                            BindProductCategory(InquiryId);
                            BindSamplingMethod();
                            BindSampleCheckList();
                        }
                    }
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    switch (RoleID)
                    {
                        case 1://QC
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 2://Manager
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 3://Lab Technician
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = false;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 4://Admin
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 5://Sample Management
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
       
        protected int SampleRake()
        {
            int flag = 0;
            if (txtCargoDescription.Text == "") { txtCargoDescription.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtCargoDescription.BackColor = System.Drawing.Color.Empty; }
            if (txtReceiver.Text == "") { txtReceiver.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtReceiver.BackColor = System.Drawing.Color.Empty; }
            if (txtTransportMode.Text == "") { txtTransportMode.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtTransportMode.BackColor = System.Drawing.Color.Empty; }
            if (txtLoadingDate.Text == "") { txtLoadingDate.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtLoadingDate.BackColor = System.Drawing.Color.Empty; }
            if (txtSamplingLocation.Text == "") { txtSamplingLocation.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtSamplingLocation.BackColor = System.Drawing.Color.Empty; }
            if (txtRRNo.Text == "") { txtRRNo.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtRRNo.BackColor = System.Drawing.Color.Empty; }
            if (txtRRDate.Text == "") { txtRRDate.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtRRDate.BackColor = System.Drawing.Color.Empty; }
            if (txtRRQty.Text == "") { txtRRQty.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtRRQty.BackColor = System.Drawing.Color.Empty; }
            if (txtLoadedQty.Text == "") { txtLoadedQty.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtLoadedQty.BackColor = System.Drawing.Color.Empty; }
            return flag;
        }
        protected int SampleStack()
        {
            int flag = 0;
            if (txtCargoDescription.Text == "") { txtCargoDescription.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtCargoDescription.BackColor = System.Drawing.Color.Empty; }
            if (txtDateOfSampling.Text == "") { txtDateOfSampling.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtDateOfSampling.BackColor = System.Drawing.Color.Empty; }
            if (txtSamplingLocation.Text == "") { txtSamplingLocation.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtSamplingLocation.BackColor = System.Drawing.Color.Empty; }
            if (txtStackQty.Text == "") { txtStackQty.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtStackQty.BackColor = System.Drawing.Color.Empty; }
            return flag;
        }
       
        protected int SampleTruck()
        {
            int flag = 0;
            if (txtCargoDescription.Text == "") { txtCargoDescription.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtCargoDescription.BackColor = System.Drawing.Color.Empty; }
            if (txtReceiver.Text == "") { txtReceiver.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtReceiver.BackColor = System.Drawing.Color.Empty; }
            if (txtTransportMode.Text == "") { txtTransportMode.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtTransportMode.BackColor = System.Drawing.Color.Empty; }
            if (txtLoadingDate.Text == "") { txtLoadingDate.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtLoadingDate.BackColor = System.Drawing.Color.Empty; }
            if (txtSamplingLocation.Text == "") { txtSamplingLocation.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtSamplingLocation.BackColor = System.Drawing.Color.Empty; }
            if (txtLoadedQty.Text == "") { txtLoadedQty.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtLoadedQty.BackColor = System.Drawing.Color.Empty; }
            if (txtNoOfTrucks.Text == "") { txtNoOfTrucks.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtNoOfTrucks.BackColor = System.Drawing.Color.Empty; }
            return flag;
        }
        protected int SampleVessel()
        {
            int flag = 0;
            if (txtVesselName.Text == "") { txtVesselName.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtVesselName.BackColor = System.Drawing.Color.Empty; }
            if (txtCargoDescription.Text == "") { txtCargoDescription.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtCargoDescription.BackColor = System.Drawing.Color.Empty; }
            if (txtLoadingDate.Text == "") { txtLoadingDate.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtLoadingDate.BackColor = System.Drawing.Color.Empty; }
            if (txtLoadedQty.Text == "") { txtLoadedQty.BackColor = System.Drawing.Color.LightPink; flag = 1; }
            else { txtLoadedQty.BackColor = System.Drawing.Color.Empty; }
            return flag;

        }
        private void BindProductCategory(int InquiryId)
        {
            DataTable dt = _cm.GetProductCategory(InquiryId);
            if (dt.Rows.Count > 0)
            {
                hdSampleCategory.Value = dt.Rows[0]["SampleCategory"].ToString();
                lblCategoryType.Text = dt.Rows[0]["ProductGroupName"].ToString();
                lblCategoryName.Text = dt.Rows[0]["SubGroupName"].ToString();
                hdSampleHistory.Value = dt.Rows[0]["SampleHistory"].ToString();
                hdDepartmentID.Value = dt.Rows[0]["DepartmentID"].ToString();
            }
        }
        private void BindSamplingMethod()
        {
            DataTable dt = _cm.GetSamplingMethod();
            if (dt.Rows.Count > 0)
            {
                ddlSamplingMethod.DataSource = dt;
                ddlSamplingMethod.DataTextField = "SamplingStandard";
                ddlSamplingMethod.DataValueField = "SamplingMethodID";
                ddlSamplingMethod.DataBind();
                ddlSamplingMethod.Items.Insert(0, "Select Sampling Method");
                ddlSamplingMethod.Items.Insert(1, "NA");
            }
        }
        private void BindSampleCheckList()
        {
            string SampleCategory = hdSampleCategory.Value;

            DataTable dt = _cm.GetSampleCheckList(SampleCategory);
            if (dt.Rows.Count > 0)
            {
                chkSampleCheck.DataSource = dt;
                chkSampleCheck.DataTextField = "SampleChecklistName";
                chkSampleCheck.DataValueField = "SampleChecklistId";
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
                rbCheckList.Items.Add(new ListItem("Pass", j.ToString()));
                rbCheckList.Items.Add(new ListItem("Fail", (j + 1).ToString()));
                rbCheckList.Items.Add(new ListItem("N/A", (j + 2).ToString()));
                j += 3;
            }
            if (j == 15) { txt11.Visible = false; txt12.Visible = false; }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int SampleResultId = 0;
            int flag = 0;
            int counter = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            Random rnd = new Random();
            string ProductCategory = hdSampleCategory.Value;
            int SampleUId = rnd.Next(10000000, 99999999);
            string LastSampleID = "";
            string SampleIDSeries = "";
            if (txtSampleReceivedDate.Text == "")
            {
                txtSampleReceivedDate.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtSampleReceivedDate.BackColor = System.Drawing.Color.Empty;
            }

            if (txtSamplingLocation.Text == "")
            {
                txtSamplingLocation.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtSamplingLocation.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDateOfSampling.Text == "")
            {
                txtDateOfSampling.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtDateOfSampling.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlSamplingMethod.SelectedIndex==0)
            {
                ddlSamplingMethod.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                ddlSamplingMethod.BackColor = System.Drawing.Color.Empty;
            }
            DataTable dtReportFormat = _cm.GetReportFormat(Convert.ToInt32(Session["InquiryID"].ToString()));
            if (dtReportFormat.Rows.Count > 0)
                switch (dtReportFormat.Rows[0]["ReportType"].ToString())
                {
                    case "Rake_Cert": flag = SampleRake(); break;
                    case "Stack Sampling": flag = SampleStack(); break;
                    case "Truck_Cert": flag = SampleTruck(); break;
                    case "VESSEL CERTIFICATE": flag = SampleVessel(); break;
                    default: break;
                }
            if (counter == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    foreach (ListItem item in chkSampleCheck.Items)
                    {
                        if (item.Selected == false)
                        {
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        dt = _cm.getSampleUID(SampleUId);

                        while (dt.Rows.Count == 1)
                        {
                            dt = _cm.getSampleUID(SampleUId);
                        }
                        string sYear = DateTime.Now.AddHours(12.5).ToString("yy");
                        string sMonth = DateTime.Now.AddHours(12.5).ToString("MM");
                        dt = _cm.getLastSampleID();
                        if (dt.Rows.Count > 0)
                        {
                            LastSampleID = dt.Rows[0]["sampleIDSeries"].ToString();
                            string[] TestReportString = LastSampleID.Split('/');
                            int lastItem = Convert.ToInt32(TestReportString[TestReportString.Length - 1]);

                            //Check year change. Reset series to '0001'
                            string midTerm = TestReportString[TestReportString.Length - 2];
                            if (midTerm.Substring(0, 2) != sYear)
                            {
                                SampleIDSeries = "CISCO/PDP/" + sYear + sMonth + "/" + "0001";
                            }
                            else
                            {
                                int nextItem = lastItem + 1;
                                SampleIDSeries = nextItem.ToString().PadLeft(4, '0');
                                SampleIDSeries = "CISCO/PDP/" + sYear + sMonth + "/" + SampleIDSeries;
                            }
                        }
                        else
                        {
                            SampleIDSeries = "CISCO/PDP/" + sYear + sMonth + "/" + "0001";
                        }
                        dt2 = _cm.GetProductCategoryDetails(ProductCategory);
                        if (dt2.Rows.Count > 0)
                        {
                            _cm.department = dt2.Rows[0]["DepartmentId"].ToString();
                        }
                        _cm.receivedDate = Convert.ToDateTime(txtSampleReceivedDate.Text.ToString());
                        _cm.inquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                        _cm.sampleId = SampleUId;
                        _cm.sampleIDSeries = SampleIDSeries;
                        _cm.sampleConditionRemarks = txtSampleConditionRemarks.Text;
                        _cm.SampleVerifiedBy = EmployeeID;
                        _cm.SamplingMethod = ddlSamplingMethod.SelectedItem.ToString();
                        _cm.Humidity = "";
                        _cm.cargo_description = txtCargoDescription.Text;
                        _cm.date_of_loading = Convert.ToDateTime(txtLoadingDate.Text);
                        _cm.loaded_quantity = txtLoadedQty.Text;
                        _cm.mode_of_transport = txtTransportMode.Text;
                        _cm.no_of_trucks = txtNoOfTrucks.Text;
                        _cm.place_of_inspection = txtSamplingLocation.Text;
                        _cm.receiver = txtReceiver.Text;
                        _cm.rr_no = txtRRNo.Text;
                        _cm.rr_date = txtRRDate.Text;
                        _cm.rr_quantity = txtRRQty.Text;
                        _cm.stack_quality = txtStackQty.Text;
                        _cm.vessel_name = txtVesselName.Text;

                        SampleResultId = _cm.AddSampleDetails(_cm);

                        for (int i = 0; i < chkSampleCheck.Items.Count; i++)
                        {
                            if (chkSampleCheck.Items[i].Selected == false)
                            {
                                _cm.sampleChecklistId = chkSampleCheck.Items[i].Value;
                                _cm.sampleChecklistStatus = false;
                                _cm.sampleId = SampleResultId;
                                switch (i)
                                {
                                    case 0: _cm.SampleCheckListValue = txt1.Text; break;
                                    case 1: _cm.SampleCheckListValue = txt2.Text; break;
                                    case 2: _cm.SampleCheckListValue = txt3.Text; break;
                                    case 3: _cm.SampleCheckListValue = txt4.Text; break;
                                    case 4: _cm.SampleCheckListValue = txt5.Text; break;
                                    case 5: _cm.SampleCheckListValue = txt6.Text; break;
                                    case 6: _cm.SampleCheckListValue = txt7.Text; break;
                                    case 7: _cm.SampleCheckListValue = txt8.Text; break;
                                    case 8: _cm.SampleCheckListValue = txt9.Text; break;
                                    case 9: _cm.SampleCheckListValue = txt10.Text; break;
                                    case 10: _cm.SampleCheckListValue = txt11.Text; break;
                                    case 11: _cm.SampleCheckListValue = txt12.Text; break;
                                    default: break;
                                }
                                _cm.AddSampleChecklistStatusDetails(_cm);
                            }
                            else if (chkSampleCheck.Items[i].Selected == true)
                            {
                                _cm.sampleChecklistId = chkSampleCheck.Items[i].Value;
                                _cm.sampleChecklistStatus = true;
                                _cm.sampleId = SampleResultId;
                                switch (i)
                                {
                                    case 0: _cm.SampleCheckListValue = txt1.Text; break;
                                    case 1: _cm.SampleCheckListValue = txt2.Text; break;
                                    case 2: _cm.SampleCheckListValue = txt3.Text; break;
                                    case 3: _cm.SampleCheckListValue = txt4.Text; break;
                                    case 4: _cm.SampleCheckListValue = txt5.Text; break;
                                    case 5: _cm.SampleCheckListValue = txt6.Text; break;
                                    case 6: _cm.SampleCheckListValue = txt7.Text; break;
                                    case 7: _cm.SampleCheckListValue = txt8.Text; break;
                                    case 8: _cm.SampleCheckListValue = txt9.Text; break;
                                    case 9: _cm.SampleCheckListValue = txt10.Text; break;
                                    case 10: _cm.SampleCheckListValue = txt11.Text; break;
                                    case 11: _cm.SampleCheckListValue = txt12.Text; break;
                                    default: break;
                                }
                                _cm.AddSampleChecklistStatusDetails(_cm);
                            }
                            if (Session["CloneCRFSampleID"] != null)
                            {
                                CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                                _cm.CloneCRFSampleID = CloneCRFSampleID;
                                _cm.sampleId = SampleResultId;
                                _cm.CRFStatusID = 14;//Sample Accepted By Sample Management
                                _cm.sampleHistory = hdSampleHistory.Value + "[" + SampleResultId + "]";
                                _cm.UpdateCRFCloneList(_cm);
                                _cm.UpdateCRFStatus(_cm);

                                _cm.sampleStatusID = 14;// Sample Accepted By Sample Management
                                _cm.updateSampleStatus(_cm);

                            }

                        }
                    }
                    else
                    {
                        dt = _cm.getSampleUID(SampleUId);

                        while (dt.Rows.Count == 1)
                        {
                            dt = _cm.getSampleUID(SampleUId);
                        }
                        string sYear = DateTime.Now.AddHours(12.5).ToString("yy");
                        string sMonth = DateTime.Now.AddHours(12.5).ToString("MM");
                        dt = _cm.getLastSampleID();
                        if (dt.Rows.Count > 0)
                        {
                            LastSampleID = dt.Rows[0]["sampleIDSeries"].ToString();
                            string[] TestReportString = LastSampleID.Split('/');
                            int lastItem = Convert.ToInt32(TestReportString[TestReportString.Length - 1]);

                            //Check year change. Reset series to '0001'
                            string midTerm = TestReportString[TestReportString.Length - 2];
                            if (midTerm.Substring(0, 2) != sYear)
                            {
                                SampleIDSeries = "CISCO/PDP/" + sYear + sMonth + "/" + "0001";
                            }
                            else
                            {
                                int nextItem = lastItem + 1;
                                SampleIDSeries = nextItem.ToString().PadLeft(4, '0');
                                SampleIDSeries = "CISCO/PDP/" + sYear + sMonth + "/" + SampleIDSeries;
                            }
                        }
                        else
                        {
                            SampleIDSeries = "CISCO/PDP/" + sYear + sMonth + "/" + "0001";
                        }
                        dt2 = _cm.GetProductCategoryDetails(ProductCategory);
                        if (dt2.Rows.Count > 0)
                        {
                            _cm.department = dt2.Rows[0]["DepartmentId"].ToString();
                        }
                        _cm.receivedDate = Convert.ToDateTime(txtSampleReceivedDate.Text.ToString());
                        _cm.sampleConditionRemarks = txtSampleConditionRemarks.Text;
                        //  _cm.destroyDate = Convert.ToDateTime(txtSampleDestroyedDate.Text.ToString());
                        _cm.inquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                        _cm.sampleId = SampleUId;
                        _cm.sampleIDSeries = SampleIDSeries;
                        _cm.sampleConditionRemarks = txtSampleConditionRemarks.Text;
                        _cm.SampleVerifiedBy = EmployeeID;
                        _cm.SamplingMethod = ddlSamplingMethod.SelectedItem.ToString();
                        _cm.Humidity = "";
                        _cm.cargo_description = txtCargoDescription.Text;
                        _cm.date_of_loading = Convert.ToDateTime(txtLoadingDate.Text);
                        _cm.loaded_quantity = txtLoadedQty.Text;
                        _cm.mode_of_transport = txtTransportMode.Text;
                        _cm.no_of_trucks = txtNoOfTrucks.Text;
                        _cm.place_of_inspection = txtSamplingLocation.Text;
                        _cm.receiver = txtReceiver.Text;
                        _cm.rr_no = txtRRNo.Text;
                        _cm.rr_date = txtRRDate.Text;
                        _cm.rr_quantity = txtRRQty.Text;
                        _cm.stack_quality = txtStackQty.Text;
                        _cm.vessel_name = txtVesselName.Text;
                        SampleResultId = _cm.AddSampleDetails(_cm);

                        Session["SampleId"] = SampleResultId;

                        for (int i = 0; i < chkSampleCheck.Items.Count; i++)
                        {
                            if (chkSampleCheck.Items[i].Selected == false)
                            {
                                _cm.sampleChecklistId = chkSampleCheck.Items[i].Value;
                                _cm.sampleChecklistStatus = false;
                                _cm.sampleId = SampleResultId;
                                switch (i)
                                {
                                    case 0: _cm.SampleCheckListValue = txt1.Text; break;
                                    case 1: _cm.SampleCheckListValue = txt2.Text; break;
                                    case 2: _cm.SampleCheckListValue = txt3.Text; break;
                                    case 3: _cm.SampleCheckListValue = txt4.Text; break;
                                    case 4: _cm.SampleCheckListValue = txt5.Text; break;
                                    case 5: _cm.SampleCheckListValue = txt6.Text; break;
                                    case 6: _cm.SampleCheckListValue = txt7.Text; break;
                                    case 7: _cm.SampleCheckListValue = txt8.Text; break;
                                    case 8: _cm.SampleCheckListValue = txt9.Text; break;
                                    case 9: _cm.SampleCheckListValue = txt10.Text; break;
                                    case 10: _cm.SampleCheckListValue = txt11.Text; break;
                                    case 11: _cm.SampleCheckListValue = txt12.Text; break;
                                    default: break;
                                }
                                _cm.AddSampleChecklistStatusDetails(_cm);
                            }
                            else if (chkSampleCheck.Items[i].Selected == true)
                            {
                                _cm.sampleChecklistId = chkSampleCheck.Items[i].Value;
                                _cm.sampleChecklistStatus = true;
                                _cm.sampleId = SampleResultId;
                                switch (i)
                                {
                                    case 0: _cm.SampleCheckListValue = txt1.Text; break;
                                    case 1: _cm.SampleCheckListValue = txt2.Text; break;
                                    case 2: _cm.SampleCheckListValue = txt3.Text; break;
                                    case 3: _cm.SampleCheckListValue = txt4.Text; break;
                                    case 4: _cm.SampleCheckListValue = txt5.Text; break;
                                    case 5: _cm.SampleCheckListValue = txt6.Text; break;
                                    case 6: _cm.SampleCheckListValue = txt7.Text; break;
                                    case 7: _cm.SampleCheckListValue = txt8.Text; break;
                                    case 8: _cm.SampleCheckListValue = txt9.Text; break;
                                    case 9: _cm.SampleCheckListValue = txt10.Text; break;
                                    case 10: _cm.SampleCheckListValue = txt11.Text; break;
                                    case 11: _cm.SampleCheckListValue = txt12.Text; break;
                                    default: break;
                                }
                                _cm.AddSampleChecklistStatusDetails(_cm);
                            }
                            if (Session["CloneCRFSampleID"] != null)
                            {
                                CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                                _cm.CloneCRFSampleID = CloneCRFSampleID;
                                _cm.sampleId = SampleResultId;
                                _cm.CRFStatusID = 16;// Sample Rejected By Lab
                                _cm.sampleHistory = hdSampleHistory.Value + "[" + SampleResultId + "]";
                                _cm.UpdateCRFCloneList(_cm);
                                _cm.UpdateCRFStatus(_cm);

                                _cm.sampleStatusID = 16;// Sample Rejected By Lab
                                _cm.updateSampleStatus(_cm);
                            }

                        }
                    }
                    _cm.tableName = "SampleDetails";
                    _cm.paramID = "ID";
                    _cm.paramIDValue = SampleResultId;

                    if (txtSamplingLocation.Text != "")
                    {
                        _cm.paramName = "SamplingLocation";
                        _cm.paramValue = txtSamplingLocation.Text;
                        _cm.UpdateTable(_cm);
                    }
                    if (txtDateOfSampling.Text != "")
                    {
                        _cm.paramName = "DateOfSampling";
                        _cm.paramValue = txtDateOfSampling.Text;
                        _cm.UpdateTable(_cm);
                    }
                    Clear();
                    Response.Redirect("CustomerInquiryView.aspx");
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Fill Highlighted Fields')", true);
            }
        }

        private void Clear()
        {
            txtSampleReceivedDate.Text = "";
            txtSamplingLocation.Text = "";
            txtDateOfSampling.Text = "";
            //  txtSampleDestroyedDate.Text = "";           
        }
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
                    if ((rbCheckList.SelectedItem.ToString() == "Pass") || (rbCheckList.SelectedItem.ToString() == "N/A"))
                    {
                        chkSampleCheck.Items[value].Selected = true;
                    }
                    else
                    {
                        chkSampleCheck.Items[value].Selected = false;
                    }
                }
            }
        }

        protected void txtSampleReceivedDate_TextChanged(object sender, EventArgs e)
        {
            divCheckBox.Visible = true;
            divDynamicTextBox.Visible = true;
            DataTable dt = new DataTable();
            string ProductCategory = lblCategoryType.Text;
            dt = _cm.GetProductCategoryDetails(ProductCategory);
            if (txtSampleReceivedDate.Text != "")
            {
                if (dt.Rows.Count > 0)
                {
                    DateTime datetime = Convert.ToDateTime(txtSampleReceivedDate.Text.ToString());
                    _cm.department = dt.Rows[0]["SubDepartmentId"].ToString();
                    //if (_cm.department == "1")
                    //{
                    //    txtSampleDestroyedDate.Text = datetime.AddMonths(6).ToString("dd/MMM/yyyy HH':'mm':'ss");
                    //}
                    //if (_cm.department == "2")
                    //{
                    //    txtSampleDestroyedDate.Text = datetime.AddMonths(3).ToString("dd/MMM/yyyy HH':'mm':'ss");
                    //}
                    //if (_cm.department == "3")
                    //{
                    //    txtSampleDestroyedDate.Text = datetime.AddDays(1).ToString("dd/MMM/yyyy HH':'mm':'ss");
                    //}
                    //if (_cm.department == "4")
                    //{
                    //    txtSampleDestroyedDate.Text = datetime.AddMonths(1).ToString("dd/MMM/yyyy HH':'mm':'ss");
                    //}
                    //if (_cm.department == "5")
                    //{
                    //    txtSampleDestroyedDate.Text = datetime.AddDays(1).ToString("dd/MMM/yyyy HH':'mm':'ss");
                    //}
                    //if (_cm.department == "6")
                    //{
                    //    txtSampleDestroyedDate.Text = datetime.AddMonths(1).ToString("dd/MMM/yyyy HH':'mm':'ss");
                    //}
                }
            }
        }
    }
}