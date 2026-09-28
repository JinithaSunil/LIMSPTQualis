using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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

        private void BindSampleImages()
        {
            int InquiryId = Convert.ToInt32(ViewState["InquiryID"]);
            DataTable dt = _cm.GetSampleImages(InquiryId);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                string fileName = rowView["filename"].ToString();
                Image img = (Image)e.Row.FindControl("imgSample");
                if (img != null && !string.IsNullOrEmpty(fileName))
                {
                    /* string smtpUrl = "http://localhost:41414"; */
                    string smtpUrl = Session["WebsiteURL"] as string ?? "http://localhost:41414";
                    string imageUrl = smtpUrl + "/SampleImages/" + fileName;
                    img.ImageUrl = imageUrl;
                }
            }
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
            dt = _cm.GetProductCategoryDetails(hdSampleCategory.Value);
            if (txtSampleReceivedDate.Text != "")
            {
                if (dt.Rows.Count > 0)
                {
                    DateTime datetime = Convert.ToDateTime(txtSampleReceivedDate.Text.ToString());
                    _cm.department = dt.Rows[0]["DepartmentId"].ToString();
                }
            }
        }
        protected void btnUpload_Click(object sender, EventArgs e)
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
    }
}