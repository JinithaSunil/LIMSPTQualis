using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFeedback : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int sampleID = 0; int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    string CloneCRFSampleID = Request.QueryString["ID"].ToString();
                    Session["CloneCRFSampleID"] = CloneCRFSampleID;
                    if (CloneCRFSampleID != null)
                    {   
                        divCustomer.Visible = true;
                        divFeedback.Visible = true;
                        divSuccess.Visible = false;
                        divFail.Visible = false;
                       
                        string InquiryId =Session["InquiryId"].ToString();
                        lblEnquiryID.Text = "ENQ" + InquiryId;
                        DataTable dt = _cm.GetCustomerForFeedback(CloneCRFSampleID);
                        if (dt.Rows.Count > 0)
                        {
                            txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                            sampleID = Convert.ToInt32(dt.Rows[0]["sampleID"].ToString());
                            Session["ID"] = sampleID;
                        }
                        DateTime date = DateTime.Now.AddHours(12.5);
                        txtDate.Text = date.ToString("dd/MMM/yyyy");
                    }
                    else
                    {
                        divCustomer.Visible = false;
                        divFeedback.Visible = false;
                        divSuccess.Visible = false;
                        divFail.Visible = true;
                    }
                }
                else
                {
                    divCustomer.Visible = false;
                    divFeedback.Visible = false;
                    divSuccess.Visible = false;
                    divFail.Visible = true;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int FeedbackId = 0; int flag = 1;
            string[] feedbackName = new string[10] {"AVAILABILITY OF INFORMATION REGARDING LABORATORY’S SERVICE",
                                                    "RESPONSE TO QUERY OVER TELEPHONE, E-MAIL, FAX.",
                                                    "ATTENTION AND SERVICE DURING VISIT TO LAB.",
                                                    "ADEQUACY OF TEST EQIPMENTS AND TOOLS",
                                                    "ON-TIME COMPLETION OF WORK INCLUDING DELIVERY OF TEST REPORTS.",
                                                    "BRIEFING QUALITY FOR TEST PROCEDURES AND DOCUMENTATION",
                                                    "RELIABILITY OF TEST RESULTS",
                                                    "VALUE FOR MONEY",
                                                    "RECEPTIVENESS TO COMPLAINTS & SUGGESTIONS",
                                                    "OVER ALL SATISFACTION"};
            string[] feedbackValue = new string[10] { "", "", "", "", "", "", "", "", "", "" };
            int[] feedbackWeightage = new int[10];
            string[] customerRemark = new string[10] { "", "", "", "", "", "", "", "", "", "" };

            DataTable dt = new DataTable();
            string InquiryId = Session["InquiryId"].ToString();
            _cm.inquiryId = Convert.ToInt32(InquiryId.ToString());
            if (rdl1.SelectedValue != null && rdl1.SelectedValue != "")
            {
                rdl1.BackColor = System.Drawing.Color.Empty;

                string value = rdl1.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[0] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[0] = value;
                if (txtCustomer1.Text.ToString() != null && txtCustomer1.Text != "")
                {
                    customerRemark[0] = txtCustomer1.Text.ToString();
                }
            }
            else
            {
                rdl1.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl2.SelectedValue != null && rdl2.SelectedValue != "")
            {
                rdl2.BackColor = System.Drawing.Color.Empty;

                string value = rdl2.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[1] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[1] = value;
                if (txtCustomer2.Text.ToString() != null)
                {
                    customerRemark[1] = txtCustomer2.Text.ToString();
                }
            }
            else
            {
                rdl2.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl3.SelectedValue != null && rdl3.SelectedValue != "")
            {
                rdl3.BackColor = System.Drawing.Color.Empty;

                string value = rdl3.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[2] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[2] = value;
                if (txtCustomer3.Text.ToString() != null)
                {
                    customerRemark[2] = txtCustomer3.Text.ToString();
                }
            }
            else
            {
                rdl3.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl4.SelectedValue != null && rdl4.SelectedValue != "")
            {
                rdl4.BackColor = System.Drawing.Color.Empty;

                string value = rdl4.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[3] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[3] = value;
                if (txtCustomer4.Text.ToString() != null)
                {
                    customerRemark[3] = txtCustomer4.Text.ToString();
                }
            }
            else
            {
                rdl4.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl5.SelectedValue != null && rdl5.SelectedValue != "")
            {
                rdl5.BackColor = System.Drawing.Color.Empty;

                string value = rdl5.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[4] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[4] = value;
                if (txtCustomer5.Text.ToString() != null)
                {
                    customerRemark[4] = txtCustomer5.Text.ToString();
                }
            }
            else
            {
                rdl5.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl6.SelectedValue != null && rdl6.SelectedValue != "")
            {
                rdl6.BackColor = System.Drawing.Color.Empty;

                string value = rdl6.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[5] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[5] = value;
                if (txtCustomer6.Text.ToString() != null)
                {
                    customerRemark[5] = txtCustomer6.Text.ToString();
                }
            }
            else
            {
                rdl6.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl7.SelectedValue != null && rdl7.SelectedValue != "")
            {
                rdl7.BackColor = System.Drawing.Color.Empty;

                string value = rdl7.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[6] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[6] = value;
                if (txtCustomer7.Text.ToString() != null)
                {
                    customerRemark[6] = txtCustomer7.Text.ToString();
                }
            }
            else
            {
                rdl7.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl8.SelectedValue != null && rdl8.SelectedValue != "")
            {
                rdl8.BackColor = System.Drawing.Color.Empty;

                string value = rdl8.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[7] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[7] = value;
                if (txtCustomer8.Text.ToString() != null)
                {
                    customerRemark[7] = txtCustomer8.Text.ToString();
                }
            }
            else
            {
                rdl8.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl9.SelectedValue != null && rdl9.SelectedValue != "")
            {
                rdl9.BackColor = System.Drawing.Color.Empty;

                string value = rdl9.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[8] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[8] = value;
                if (txtCustomer9.Text.ToString() != null)
                {
                    customerRemark[8] = txtCustomer9.Text.ToString();
                }
            }
            else
            {
                rdl9.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (rdl10.SelectedValue != null && rdl10.SelectedValue != "")
            {
                rdl10.BackColor = System.Drawing.Color.Empty;

                string value = rdl10.SelectedValue.ToString();
                dt = _cm.GetFeedbackWeightage(value);
                if (dt.Rows.Count > 0)
                {
                    feedbackWeightage[9] = Convert.ToInt32(dt.Rows[0]["FeedbackWeightage"].ToString());
                }
                feedbackValue[9] = value;
                if (txtCustomer10.Text.ToString() != null)
                {
                    customerRemark[9] = txtCustomer10.Text.ToString();
                }
            }
            else
            {
                rdl10.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (flag == 1)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    for (int i = 0; i < 10; i++)
                    {
                        CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                        _cm.feedbackName = feedbackName[i];
                        _cm.feedbackValue = feedbackValue[i];
                        _cm.feedbackWeightage = feedbackWeightage[i];
                        _cm.customerRemark = customerRemark[i];
                        _cm.feedbackDate = DateTime.Now.AddHours(12.5);
                        FeedbackId = _cm.AddCustomerFeedback(_cm);

                        //update CloneCRF inquiry status                    
                        _cm.CRFStatusID = 23; // Feedback Received
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);

                        _cm.sampleId = Convert.ToInt32(Session["ID"].ToString());
                        _cm.sampleStatusID = 23;// Feedback Received
                        _cm.updateSampleStatus(_cm);

                        _cm.inquiryId = Convert.ToInt32(Session["InquiryId"].ToString());
                        _cm.inquiryStatusId = 23;
                        _cm.UpdateInquiryStatus(_cm);

                    }
                    Clear();
                    divCustomer.Visible = false;
                    divFeedback.Visible = false;
                    divSuccess.Visible = true;
                    divFail.Visible = false;
                }
            }
        }

        private void Clear()
        {
            txtCustomer.Text = "";
            txtDate.Text = "";
            rdl1.SelectedIndex = -1;
            rdl2.SelectedIndex = -1;
            rdl3.SelectedIndex = -1;
            rdl4.SelectedIndex = -1;
            rdl5.SelectedIndex = -1;
            rdl6.SelectedIndex = -1;
            rdl7.SelectedIndex = -1;
            rdl8.SelectedIndex = -1;
            rdl9.SelectedIndex = -1;
            rdl10.SelectedIndex = -1;
            txtCustomer1.Text = "";
            txtCustomer2.Text = "";
            txtCustomer3.Text = "";
            txtCustomer4.Text = "";
            txtCustomer5.Text = "";
            txtCustomer6.Text = "";
            txtCustomer7.Text = "";
            txtCustomer8.Text = "";
            txtCustomer9.Text = "";
            txtCustomer10.Text = "";
        }
    }
}