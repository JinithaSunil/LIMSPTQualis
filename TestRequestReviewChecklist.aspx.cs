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
    public partial class TestRequestReviewChecklist : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0;
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
                    if (Request.QueryString["ID"] != null)
                    {
                        Session["InquiryID"] = Request.QueryString["ID"];
                    }
                    if (Session["InquiryID"] != null)
                    {                     
                            int InquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                            lblInquiryID.Text = "ENQ" + InquiryId.ToString();
                            BindProductCategory(InquiryId);
                      
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
        private void BindProductCategory(int InquiryId)
        {
            InquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
           
            txtReviewDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DataTable dt = _cm.GetProductCategory(InquiryId);
            if (dt.Rows.Count > 0)
            {
                lblInquiryID.Text = "R/" + dt.Rows[0]["RequestNo"].ToString();
                hdSampleCategory.Value = dt.Rows[0]["SampleCategory"].ToString();
                hdSampleHistory.Value = dt.Rows[0]["SampleHistory"].ToString();
                hdDepartmentID.Value = dt.Rows[0]["DepartmentID"].ToString();
            }
            DataTable dtCRFDetails = _cm.getCustomerCRFSampleDetails(InquiryId);
            if (dtCRFDetails.Rows.Count > 0)
            {
                lblSampleName.Text = dtCRFDetails.Rows[0]["SampleName"].ToString();
                lblSampleQty.Text = dtCRFDetails.Rows[0]["SizeOfSample"].ToString();
            }
            DataTable dtCustomerDetails = _cm.getCRFCustomer(InquiryId);
            if (dtCustomerDetails.Rows.Count > 0)
            {
                lblCustomerName.Text = dtCustomerDetails.Rows[0]["CustomerName"].ToString();
                lblAddressLine1.Text = dtCustomerDetails.Rows[0]["AddressLine1"].ToString();
                lblAddressLine2.Text = dtCustomerDetails.Rows[0]["AddressLine2"].ToString();
                lblDistrict.Text = dtCustomerDetails.Rows[0]["District"].ToString();
                lblState.Text = dtCustomerDetails.Rows[0]["State"].ToString();
                lblCountry.Text = dtCustomerDetails.Rows[0]["Country"].ToString();
            }

            DataTable dtService = _cm.getAdditionalDetails(InquiryId);


            if (dtService.Rows.Count > 0)
            {
                string service = dtService.Rows[0]["ServiceRequired"].ToString();
                int days = 0;

                switch (service)
                {
                    case "Regular":
                        days = 8;
                        break;
                    case "Express":
                        days = 5;
                        break;
                    case "Urgent":
                        days = 3;
                        break;
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
        }

        protected void AddDetails(int InquiryStatus)
        {
            int flag = 0;
            int counter = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            Random rnd = new Random();
            string ProductCategory = hdSampleCategory.Value;
            int SampleUId = rnd.Next(10000000, 99999999);

            if (txtReviewDate.Text == "")
            {
                txtReviewDate.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtReviewDate.BackColor = System.Drawing.Color.Empty;
            }
            if (txtRemarks.Text == "")
            {
                txtRemarks.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtRemarks.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDeliveryDate.Text == "")
            {
                txtDeliveryDate.BackColor = System.Drawing.Color.LightPink;
                counter = 1;
            }
            else
            {
                txtDeliveryDate.BackColor = System.Drawing.Color.Empty;
            }
            if (radEquip.SelectedIndex == -1)
            {
                radEquip.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                radEquip.BackColor = Color.Empty;
            }
            if (radManpower.SelectedIndex == -1)
            {
                radManpower.BackColor = Color.Red;
                flag = 1;
            }
            else
            {
                radManpower.BackColor = Color.Empty;
            }
            if (radReagent.SelectedIndex == -1)
            {
                radReagent.BackColor = Color.Red;
                flag = 1;
            }
            else
            {
                radReagent.BackColor = Color.Empty;
            }
            if (radTestMethod.SelectedIndex == -1)
            {
                radTestMethod.BackColor = Color.Red;
                flag = 1;
            }
            else
            {
                radTestMethod.BackColor = Color.Empty;
            }
            if (counter == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    if (flag == 0)
                    {
                        if (Session["EmployeeID"] != null)
                        {
                            if (Session["InquiryID"] != null)
                            {
                                int InquiryID = Convert.ToInt32(Session["InquiryID"].ToString());
                                _cm.reviewDate = DateTime.ParseExact(txtReviewDate.Text, "dd/MM/yyyy", null);
                                _cm.inquiryId = Convert.ToInt32(Session["InquiryID"].ToString());
                                _cm.CloneCRFSampleID = 0;
                                _cm.sampleName = lblSampleName.Text;
                                _cm.sampleQty = lblSampleQty.Text;
                                _cm.equipmentCondition = radEquip.SelectedItem.ToString();
                                _cm.manpower = radManpower.SelectedItem.ToString();
                                _cm.reagent = radReagent.SelectedItem.ToString();
                                _cm.testMethod = radTestMethod.SelectedItem.ToString();
                                _cm.sampleQtyAccepted = "";// txtSampleQty.Text;
                                _cm.samplePhyCondn = "";//txtSamplePhysicalCondition.Text;
                                _cm.sampleTemp = "";//txtSampleTemp.Text; 
                                _cm.deliveryDate = DateTime.ParseExact(txtDeliveryDate.Text, "dd/MM/yyyy", null);
                                _cm.SampleVerifiedBy = EmployeeID;
                                _cm.remarks = txtRemarks.Text;
                                _cm.preparedBy = Convert.ToInt32(Session["EmployeeID"].ToString());
                                _cm.CRFStatusID = InquiryStatus;
                                int TestRequestReviewID = _cm.AddTestRequestReview(_cm);
                            }
                        }

                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Fill Highlighted Fields')", true);
            }
        }

        private void Clear()
        {
            txtReviewDate.Text = "";
            //txtSampleQty.Text = "";
            //txtSampleTemp.Text = "";
            txtDeliveryDate.Text = "";
            //  txtSampleDestroyedDate.Text = "";           
        }



        protected void btnAccept_Click(object sender, EventArgs e)
        {
            int InquiryID = 0;
            AddDetails(21);
            if (Session["InquiryID"] != null)
            {
                InquiryID = Convert.ToInt32(Session["InquiryID"].ToString());
                //change CloneCRF Status to CRF Approved
                _cm.inquiryId = InquiryID;
                _cm.inquiryStatusId = 21;
                _cm.UpdateInquiryStatus(_cm);
                               
                Response.Redirect("CustomerCRFList.aspx");
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            int InquiryID = 0;
            AddDetails(1001);
            if (Session["InquiryID"] != null)
            {
                InquiryID = Convert.ToInt32(Session["InquiryID"].ToString());
                //change CloneCRF Status to CRF Rejected
                _cm.inquiryId = InquiryID;
                _cm.inquiryStatusId = 1001;
                _cm.UpdateInquiryStatus(_cm);
                Response.Redirect("CustomerCRFList.aspx");
            }
        }

    }
}