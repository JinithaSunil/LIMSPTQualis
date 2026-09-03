using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
// Use alias to avoid naming conflicts
using DrawingImage = System.Drawing.Image;
using Encoder = System.Drawing.Imaging.Encoder;
using Image = System.Web.UI.WebControls.Image;

namespace LIMS
{
    public partial class SampleManagement : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0; int CloneCRFSampleID = 0;
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
                if (Session["EmployeeID"] != null)
                {
                    EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                }
                if (!Page.IsPostBack)
                {
                    if (Session["InquiryID"] != null)
                    {
                        if (Session["CloneCRFSampleID"] != null)
                        {
                            int InquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                            ViewState["InquiryID"] = InquiryId;
                            CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                            lblEnquiryId.Text = "ENQ" + InquiryId.ToString();
                            lblInquiryID.Text = InquiryId.ToString();
                            divCheckBox.Visible = false;

                            CalendarExtender1.EndDate = DateTime.Now;
                            CalendarExtender2.EndDate = DateTime.Now;

                            BindProductCategory(InquiryId);
                            BindSamplingMethod();
                            BindSampleCheckList();
                            BindDeliveryDate(InquiryId);
                            
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
        private void BindDeliveryDate(int InquiryId)
        {
            int days = 0;

            DataTable dtService = _cm.getAdditionalDetails(InquiryId);
            if (dtService.Rows.Count > 0)
            {
                string service = dtService.Rows[0]["ServiceRequired"].ToString();
                DataTable dtServices = _cm.GetBindServiceRequired(service);
                if (dtServices.Rows.Count > 0)
                {
                    days = Convert.ToInt32(dtServices.Rows[0]["DayCount"].ToString());
                    //switch (service)
                    //{
                    //    case "Regular":
                    //        days = 8;
                    //        break;
                    //    case "Express":
                    //        days = 5;
                    //        break;
                    //    case "Urgent":
                    //        days = 3;
                    //        break;
                    //}
                }
            }

            // Initial approximate delivery date
            DateTime ApproxDeliveryDate = DateTime.Now.AddDays(days);

            string currentYear = DateTime.Now.Year.ToString();
            // Get holiday data
            var holidayData = _cm.getHolidayDetails(currentYear);

            // Loop to check both holidays and weekends
            bool isHolidayOrWeekend = true;
            while (isHolidayOrWeekend)
            {
                isHolidayOrWeekend = false;

                // Check if ApproxDeliveryDate is a holiday
                foreach (DataRow holiday in holidayData.Rows)
                {
                    DateTime holidayDate = Convert.ToDateTime(holiday["Date"]);

                    // If ApproxDeliveryDate matches a holiday, add 1 more day
                    if (ApproxDeliveryDate.Date == holidayDate.Date)
                    {
                        ApproxDeliveryDate = ApproxDeliveryDate.AddDays(1);
                        isHolidayOrWeekend = true;  // Flag to check again after adjustment
                        break;  // Exit loop to recheck after adjusting
                    }
                }

                // Check if ApproxDeliveryDate is on a weekend (Saturday or Sunday)
                if (ApproxDeliveryDate.DayOfWeek == DayOfWeek.Saturday || ApproxDeliveryDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    ApproxDeliveryDate = ApproxDeliveryDate.AddDays(1);  // Add 1 more day to skip the weekend
                    isHolidayOrWeekend = true;  // Flag to check again after adjustment
                }
            }

            // Now, set the adjusted delivery date in the Textbox
            txtDeliveryDate.Text = ApproxDeliveryDate.ToString("dd/MM/yyyy");
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
            //if (j == 15) { txt11.Visible = false; txt12.Visible = false; }
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
            int EnqID = Convert.ToInt32(lblInquiryID.Text);
            int departmentID = 0;
            DataTable dt1 = _cm.GetProductCategory(EnqID);
            if (dt1.Rows.Count > 0)
            {
                departmentID = Convert.ToInt32(dt1.Rows[0]["DepartmentID"].ToString());
                if (departmentID == 4 || departmentID == 4009)
                {
                    int result = _cm.CheckImageCount(EnqID);
                    if (result > 0)
                    {
                        counter = 0;
                    }
                    else
                    {
                        counter = 1;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Image not found!');", true);
                    }
                }
            }

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
            if (txtTransportReceiptTemp.Text == "")
            {
                txtTransportReceiptTemp.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtTransportReceiptTemp.BackColor = System.Drawing.Color.Empty;
            }
            if (txtHumidity.Text == "")
            {
                txtHumidity.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtHumidity.BackColor = System.Drawing.Color.Empty;
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
            if (ddlSamplingMethod.SelectedIndex == 0)
            {
                ddlSamplingMethod.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                ddlSamplingMethod.BackColor = System.Drawing.Color.Empty;
            }

            if (counter == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    string prefix = "";
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    DataTable dtOrg = _cm.GetOrganizationDetails();
                    if (dtOrg.Rows.Count > 0)
                    {
                        prefix = dtOrg.Rows[0]["PrefixSample"].ToString();
                    }
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
                            LastSampleID = dt.Rows[0]["sampleIDSeries"].ToString(); // smp/2026070227

                            string[] parts = LastSampleID.Split('/');

                            prefix = parts[0] + "/";          // smp/
                            string yearMonth = parts[1].Substring(0, 6); // 202607
                            int lastNo = Convert.ToInt32(parts[1].Substring(6, 4)); // 0227 -> 227

                            string currentYearMonth = DateTime.Now.ToString("yyyyMM");
                            lastNo++;
                            //if (yearMonth == currentYearMonth)
                            //{
                            //    lastNo++;
                            //}
                            //else
                            //{
                            //    lastNo = 1;
                            //}

                            SampleIDSeries = prefix + currentYearMonth + lastNo.ToString("D4");
                        }
                        else
                        {
                            SampleIDSeries = prefix + sYear + sMonth + "0001";
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
                        _cm.Humidity = txtHumidity.Text;
                        _cm.SamplingMethod = ddlSamplingMethod.SelectedItem.ToString();
                        try
                        {
                            _cm.deliveryDate = DateTime.ParseExact(txtDeliveryDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);// Convert.ToDateTime(txtDeliveryDate.Text);
                        }
                        catch (Exception ex)
                        {
                            string s = ex.Message;
                        }

                        _cm.cargo_description = "";
                        _cm.date_of_loading = Convert.ToDateTime(txtSampleReceivedDate.Text);
                        _cm.loaded_quantity = "";
                        _cm.mode_of_transport = "";
                        _cm.no_of_trucks = "";
                        _cm.place_of_inspection = "";
                        _cm.receiver = "";
                        _cm.rr_no = "";
                        _cm.rr_date = "";
                        _cm.rr_quantity = "";
                        _cm.stack_quality = "";
                        _cm.vessel_name = "";

                        SampleResultId = _cm.AddSampleDetails(_cm);
                        _cm.UpdateSampleImageUpload(EnqID, SampleResultId);

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
                                    //case 7: _cm.SampleCheckListValue = txt8.Text; break;
                                    //case 8: _cm.SampleCheckListValue = txt9.Text; break;
                                    //case 9: _cm.SampleCheckListValue = txt10.Text; break;
                                    //case 10: _cm.SampleCheckListValue = txt11.Text; break;
                                    //case 11: _cm.SampleCheckListValue = txt12.Text; break;
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
                                _cm.updateCRFTAF(CloneCRFSampleID);
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
                                SampleIDSeries = prefix + sYear + sMonth + "0001";
                            }
                            else
                            {
                                int nextItem = lastItem + 1;
                                SampleIDSeries = nextItem.ToString().PadLeft(4, '0');
                                SampleIDSeries = prefix + sYear + sMonth + SampleIDSeries;
                            }
                        }
                        else
                        {
                            SampleIDSeries = prefix + sYear + sMonth + "0001";
                        }
                        dt2 = _cm.GetProductCategoryDetails(ProductCategory);
                        if (dt2.Rows.Count > 0)
                        {
                            _cm.department = dt2.Rows[0]["DepartmentId"].ToString();
                        }
                        _cm.receivedDate = Convert.ToDateTime(txtSampleReceivedDate.Text.ToString());
                        _cm.sampleConditionRemarks = txtSampleConditionRemarks.Text; // saved in SampleDetails 'Remarks'
                        _cm.inquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                        _cm.sampleId = SampleUId;
                        _cm.sampleIDSeries = SampleIDSeries;
                        _cm.sampleConditionRemarks = txtSampleConditionRemarks.Text;
                        _cm.SampleVerifiedBy = EmployeeID;
                        _cm.Humidity = txtHumidity.Text;
                        _cm.SamplingMethod = ddlSamplingMethod.SelectedItem.ToString();
                        try
                        {
                            _cm.deliveryDate = DateTime.ParseExact(txtDeliveryDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);// Convert.ToDateTime(txtDeliveryDate.Text);
                        }
                        catch (Exception ex)
                        {
                            string s = ex.Message;
                        }
                        SampleResultId = _cm.AddSampleDetails(_cm);
                        _cm.UpdateSampleImageUpload(EnqID, SampleResultId);

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
                    if (txtTransportReceiptTemp.Text != "")
                    {
                        _cm.paramName = "TransportAndReceiptTemp";
                        _cm.paramValue = txtTransportReceiptTemp.Text;
                        _cm.UpdateTable(_cm);
                    }
                    if (txtDateOfSampling.Text != "")
                    {
                        _cm.paramName = "DateOfSampling";
                        _cm.paramValue = txtDateOfSampling.Text;
                        _cm.UpdateTable(_cm);
                    }


                    // MODIFIED SECTION: Open work order in new window and redirect to main page
                    string workOrderUrl = "GenerateWorkorder.aspx?i=" + Session["InquiryID"] + "&s=" + _cm.sampleId;
                    string redirectUrl = "CustomerCRFList.aspx";

                    // JavaScript to open work order in new window and then redirect current page
                    string script = @"window.open('" + workOrderUrl + @"', '_blank', 'width=1200,height=800,scrollbars=yes,resizable=yes');
                                        setTimeout(function() {
                                            window.location.href = '" + redirectUrl + @"';
                                        }, 500); ";

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openWorkOrder", script, true);
                    Clear();
                    //  _cm.sampleId = SampleResultId;
                    //  _cm.inquiryId = Convert.ToInt32(lblInquiryID.Text);
                    //  _cm.UpdateSampleImageDetail(_cm);
                    //  Response.Redirect("CustomerCRFList.aspx");
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
            txtTransportReceiptTemp.Text = "";
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
            string ProductCategory = lblCategoryType.Text.Trim();
            dt = _cm.GetProductCategoryDetails(ProductCategory, 1);// _cm.GetProductCategoryDetails(ProductCategory);
            if (txtSampleReceivedDate.Text != "")
            {
                if (dt.Rows.Count > 0)
                {
                    DateTime datetime = Convert.ToDateTime(txtSampleReceivedDate.Text.ToString());
                    _cm.department = dt.Rows[0]["DepartmentId"].ToString();
                }
            }
        }
      
    }
}