using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class SampleReturn_TSQ_CAL_FM_028 : System.Web.UI.Page
    {
        DAL.CM _cm = new CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInstruments();
                BindCustomerDetails();
            }
        }
        protected void BindInstruments()
        {
            if (Request.QueryString["q"] != null)
            {
                hdQuotationID.Value = Request.QueryString["q"].ToString();
                DataTable dtCalibRequest = _cm.GetCalibrationRequest(hdQuotationID.Value);
                if (dtCalibRequest.Rows.Count > 0)
                {
                    gvInstruments.DataSource = dtCalibRequest;
                    gvInstruments.DataBind();
                    VerificationStatus();
                }
                else
                {
                    gvInstruments.DataSource = null;
                    gvInstruments.DataBind();
                }
            }
        }
        protected void BindCustomerDetails()
        {
            txtDateReceived.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DataTable dt = _cm.getQuotationCustomer(hdQuotationID.Value);
            if (dt.Rows.Count > 0)
            {
                lblCustomerMasterID.Text = dt.Rows[0]["CustomerMasterID"].ToString();
                txtCustomerName.Text = dt.Rows[0]["CompanyName"].ToString();
            }
            if (Session["EmployeeName"] != null)
            {
                if (Session["RoleID"] != null)
                {
                    hdRoleID.Value = Session["RoleID"].ToString();
                    switch (hdRoleID.Value)
                    {
                        case "20"://Calib Manager
                            txtPreparedBy.Text = Session["EmployeeName"].ToString();
                            txtPreparedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            break;
                        case "19"://Calib Employee
                            btnChecked.Visible = true;
                            txtCheckedBy.Text = Session["EmployeeName"].ToString();
                            txtCheckedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            break;
                        default: break;
                    }
                }

            }
        }
        protected void gvInstruments_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvInstruments.EditIndex = e.NewEditIndex;
            BindInstruments();
        }

        protected void gvInstruments_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvInstruments.Rows[e.RowIndex];
            int RequestID = int.Parse(gvInstruments.DataKeys[e.RowIndex]["RequestID"].ToString());
            RadioButtonList radStatus = gvInstruments.Rows[e.RowIndex].FindControl("radStatus") as RadioButtonList;
            Label lblStatus = gvInstruments.Rows[e.RowIndex].FindControl("lblStatus") as Label;
            _cm.RequestID = RequestID;
            hdRequestID.Value = RequestID.ToString() ;
            //_cm.UpdateToolStatus(_cm);
            gvInstruments.EditIndex = -1;
            BindInstruments();
        }

        protected void gvInstruments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvInstruments.EditIndex = -1;
            BindInstruments();
        }
        protected void VerificationStatus()
        {
            DataTable VerificationStatus = _cm.getToolVerificationStatus(hdQuotationID.Value, hdRequestID.Value);
            if (VerificationStatus.Rows.Count == 0)
            {
                btnSave.Visible = true;
            }
            else
            {
                btnSave.Visible = false;
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtLab.Text == "") { flag = 1; txtLab.BackColor = Color.LightPink; } else { txtLab.BackColor = Color.Empty; }
            if (txtWONumber.Text == "") { flag = 1; txtWONumber.BackColor = Color.LightPink; } else { txtWONumber.BackColor = Color.Empty; }
            if (txtDateReceived.Text == "") { flag = 1; txtDateReceived.BackColor = Color.LightPink; } else { txtDateReceived.BackColor = Color.Empty; }
            if (radPriority.SelectedIndex == -1) { flag = 1; radPriority.BackColor = Color.LightPink; } else { radPriority.BackColor = Color.Empty; }
            if (txtReturnDate.Text == "") { flag = 1; txtReturnDate.BackColor = Color.LightPink; } else { txtReturnDate.BackColor = Color.Empty; }
            if (txtSampleID.Text == "") { flag = 1; txtSampleID.BackColor = Color.LightPink; } else { txtSampleID.BackColor = Color.Empty; }
            if (txtCustomerName.Text == "") { flag = 1; txtCustomerName.BackColor = Color.LightPink; } else { txtCustomerName.BackColor = Color.Empty; }
            switch (hdRoleID.Value)
            {  
                case "20"://Calib Employee
                    if (txtPreparedBy.Text == "") { flag = 1; txtPreparedBy.BackColor = Color.LightPink; } else { txtPreparedBy.BackColor = Color.Empty; }
                    if (txtPreparedDate.Text == "") { flag = 1; txtPreparedDate.BackColor = Color.LightPink; } else { txtPreparedDate.BackColor = Color.Empty; }
                    break;
                case "19"://Calib Manager
                    if (txtCheckedBy.Text == "") { flag = 1; txtCheckedBy.BackColor = Color.LightPink; } else { txtCheckedBy.BackColor = Color.Empty; }
                    if (txtCheckedDate.Text == "") { flag = 1; txtCheckedDate.BackColor = Color.LightPink; } else { txtCheckedDate.BackColor = Color.Empty; }
                    break;
                default:
                    if (txtApprovedBy.Text == "") { flag = 1; txtApprovedBy.BackColor = Color.LightPink; } else { txtApprovedBy.BackColor = Color.Empty; }
                    if (txtApprovedDate.Text == "") { flag = 1; txtApprovedDate.BackColor = Color.LightPink; } else { txtApprovedDate.BackColor = Color.Empty; }
                    if (txtSentBy.Text == "") { flag = 1; txtSentBy.BackColor = Color.LightPink; } else { txtSentBy.BackColor = Color.Empty; }
                    if (txtSentDate.Text == "") { flag = 1; txtSentDate.BackColor = Color.LightPink; } else { txtSentDate.BackColor = Color.Empty; }
                    if (txtAcceptedBy.Text == "") { flag = 1; txtAcceptedBy.BackColor = Color.LightPink; } else { txtAcceptedBy.BackColor = Color.Empty; }
                    if (txtAcceptedDate.Text == "") { flag = 1; txtAcceptedDate.BackColor = Color.LightPink; } else { txtAcceptedDate.BackColor = Color.Empty; }
                    break;

            }
            if (flag == 0)
            {
                _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                _cm.Lab = txtLab.Text;
                _cm.WONumber = txtWONumber.Text;
                _cm.DateReceived = Convert.ToDateTime(txtDateReceived.Text);
                _cm.sampleIDSeries = txtSampleID.Text;
                _cm.Priority =radPriority.SelectedItem.Text;
                _cm.ReturnDate = txtReturnDate.Text;
                _cm.CustomerMasterID = Convert.ToInt32(lblCustomerMasterID.Text);
                _cm.PreparedBy = txtPreparedBy.Text;
                _cm.PreparedDate = txtPreparedDate.Text;

               
                
                int SampleReturnID = 0;
                switch (hdRoleID.Value)
                {
                    case "20"://Calib Employee
                        SampleReturnID = _cm.AddSampleReturn(_cm);
                        _cm.SampleReturn = "SR-" + SampleReturnID.ToString();
                        _cm.UpdateQuotationSampleReturn(_cm);
                        break;
                    case "19"://Calib Manager
                        if (txtCheckedBy.Text == "") { flag = 1; txtCheckedBy.BackColor = Color.LightPink; } else { txtCheckedBy.BackColor = Color.Empty; }
                        if (txtCheckedDate.Text == "") { flag = 1; txtCheckedDate.BackColor = Color.LightPink; } else { txtCheckedDate.BackColor = Color.Empty; }
                        _cm.CheckedBy = txtCheckedBy.Text;
                        _cm.CheckedDate = txtCheckedDate.Text;
                        _cm.ApprovedBy = txtApprovedBy.Text;
                        _cm.ApprovedDate = txtApprovedDate.Text;
                        _cm.SentBy = txtSentBy.Text;
                        _cm.SentDate = txtSentDate.Text;
                        _cm.AcceptedBy = txtAcceptedBy.Text;
                        _cm.AcceptedDate = txtAcceptedDate.Text;
                        _cm.UpdateSampleReturnChecked(_cm);
                        _cm.UpdateSampleReturnApproved(_cm);
                        _cm.UpdateSampleReturnSent(_cm);
                        _cm.UpdateSampleReturnAccepted(_cm);
                        break;
                    default:
                        break;
                }
                
                Session["QuotationID"] = hdQuotationID.Value;
                Session["SampleReturnID"] = SampleReturnID;

                _cm.QuotationStatusID = 23;// MR Published
                _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                _cm.UpdateQuotationCalibStatus(_cm);
                //update Sample Status to Reviewed Accepted 
                _cm.QuotationStatusID = 24;//Reviewed Accepted
                _cm.UpdateQuotationStatus(_cm);
                Response.Redirect("generateSampleReturn_TSQ-CAL-FM-028.aspx");
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Fill highlighted fields.')", true);
            }
        }
        protected void gvInstruments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblStatus = (e.Row.FindControl("lblStatus") as Label);
                if (lblStatus != null)
                {
                    if (lblStatus.Text == "0") { lblStatus.Text = "Tidak Baik"; }
                    if (lblStatus.Text == "1") { lblStatus.Text = "Baik"; }
                }
            }
        }

        protected void btnChecked_Click(object sender, EventArgs e)
        {
            btnApproved.Visible = true;
        }
        protected void btnApproved_Click(object sender, EventArgs e)
        {
            btnSent.Visible = true;
        }
      
        protected void btnSent_Click(object sender, EventArgs e)
        {
            btnAccepted.Visible = true;
        }
        protected void btnAccepted_Click(object sender, EventArgs e)
        {

        }


    }
}