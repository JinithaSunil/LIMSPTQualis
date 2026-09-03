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
    public partial class Work_Order_TSQ_CAL_FM_002_SPK : System.Web.UI.Page
    {
        DAL.CM _cm = new CM();
        DAL.Calibration _c = new DAL.Calibration();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInstruments();
                BindCustomerDetails();
                BindToolVerification();
               
                txtWONumber.Text = GenerateWorkOrderNumber();
            }
        }
        protected void BindInstruments()
        {
            if ((Request.QueryString["q"] != null) && (Request.QueryString["r"] != null))
            {
                hdQuotationID.Value = Request.QueryString["q"].ToString();
                hdRequestID.Value = Request.QueryString["r"].ToString();

                DataTable dtCalibRequest = _cm.GetCalibrationRequest(hdQuotationID.Value, hdRequestID.Value);

                if (dtCalibRequest.Rows.Count > 0)
                {
                    if (dtCalibRequest.Rows[0]["ID"] != "")
                    {
                        hdSampleID.Value = dtCalibRequest.Rows[0]["ID"].ToString();
                    }
                    gvInstruments.DataSource = dtCalibRequest;
                    gvInstruments.DataBind();
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
                txtApplicationNo.Text = "ENQ" + dt.Rows[0]["EnquiryID"].ToString();
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
                        case "17"://CS Employee
                            txtPreparedBy.Text = Session["EmployeeName"].ToString();
                            txtPreparedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            break;
                        case "18"://CS Manager
                            txtCheckedBy.Text = Session["EmployeeName"].ToString();
                            txtCheckedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            break;
                        default: break;
                    }
                }

            }
        }
       
        protected void BindToolVerification()
        {
            DataTable dt = _cm.GetToolVerification(hdQuotationID.Value,hdRequestID.Value);
            if(dt.Rows.Count > 0)
            {
                string[] scope = dt.Rows[0]["CalibrationScope"].ToString().Split(',');
                if(scope.Length > 0)
                {
                    DataTable dtScope = _cm.getCalibrationScope(Convert.ToInt32(scope[0].ToString()));
                    if (dtScope.Rows.Count > 0)
                    {
                        txtLab.Text = dtScope.Rows[0]["Lab"].ToString();
                    }
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtLab.Text == "") { flag = 1; txtLab.BackColor = Color.LightPink; } else { txtLab.BackColor = Color.Empty; }
            if (txtWONumber.Text == "") { flag = 1; txtWONumber.BackColor = Color.LightPink; } else { txtWONumber.BackColor = Color.Empty; }
            if (txtDateReceived.Text == "") { flag = 1; txtDateReceived.BackColor = Color.LightPink; } else { txtDateReceived.BackColor = Color.Empty; }
            if (txtServiceDueDate.Text == "") { flag = 1; txtServiceDueDate.BackColor = Color.LightPink; } else { txtServiceDueDate.BackColor = Color.Empty; }
            if (txtCertificateIssueDate.Text == "") { flag = 1; txtCertificateIssueDate.BackColor = Color.LightPink; } else { txtCertificateIssueDate.BackColor = Color.Empty; }
            if (txtApplicationNo.Text == "") { flag = 1; txtApplicationNo.BackColor = Color.LightPink; } else { txtApplicationNo.BackColor = Color.Empty; }
            if (txtCustomerName.Text == "") { flag = 1; txtCustomerName.BackColor = Color.LightPink; } else { txtCustomerName.BackColor = Color.Empty; }
            switch (hdRoleID.Value)
            {
                case "17"://CS Employee
                    if (txtPreparedBy.Text == "") { flag = 1; txtPreparedBy.BackColor = Color.LightPink; } else { txtPreparedBy.BackColor = Color.Empty; }
                    if (txtPreparedDate.Text == "") { flag = 1; txtPreparedDate.BackColor = Color.LightPink; } else { txtPreparedDate.BackColor = Color.Empty; }
                    break;
                case "18"://CS Manager
                    if (txtCheckedBy.Text == "") { flag = 1; txtCheckedBy.BackColor = Color.LightPink; } else { txtCheckedBy.BackColor = Color.Empty; }
                    if (txtCheckedDate.Text == "") { flag = 1; txtCheckedDate.BackColor = Color.LightPink; } else { txtCheckedDate.BackColor = Color.Empty; }
                    break;
                default:
                    if (txtReceivedBy.Text == "") { flag = 1; txtReceivedBy.BackColor = Color.LightPink; } else { txtReceivedBy.BackColor = Color.Empty; }
                    if (txtReceivedDate.Text == "") { flag = 1; txtReceivedDate.BackColor = Color.LightPink; } else { txtReceivedDate.BackColor = Color.Empty; }
                    if (txtReturnedBy.Text == "") { flag = 1; txtReturnedBy.BackColor = Color.LightPink; } else { txtReturnedBy.BackColor = Color.Empty; }
                    if (txtReturnedDate.Text == "") { flag = 1; txtReturnedDate.BackColor = Color.LightPink; } else { txtReturnedDate.BackColor = Color.Empty; }
                    break;

            }
            if (flag == 0)
            {
                txtWONumber.Text = GenerateWorkOrderNumber();
                _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                _cm.Lab = txtLab.Text;
                _cm.WONumber = txtWONumber.Text;
                _cm.DateReceived = Convert.ToDateTime(txtDateReceived.Text);
                _cm.ServiceDueDate = Convert.ToDateTime(txtServiceDueDate.Text);
                _cm.CertificateIssueDate = Convert.ToDateTime(txtCertificateIssueDate.Text);
                _cm.ApplicationNo = txtApplicationNo.Text;
                _cm.CustomerMasterID = Convert.ToInt32(lblCustomerMasterID.Text);
                _cm.PreparedBy = txtPreparedBy.Text;
                _cm.PreparedDate = txtPreparedDate.Text;
                _cm.CheckedBy = txtCheckedBy.Text;
                _cm.CheckedDate = txtCheckedDate.Text;
                _cm.ReceivedBy = txtReceivedBy.Text;
                _cm.ReceivedDate = txtReceivedDate.Text;
                _cm.ReturnedBy = txtReturnedBy.Text;
                _cm.ReturnedDate = txtReturnedDate.Text;
                _cm.sampleID = Convert.ToInt32(hdSampleID.Value);

                int WorkOrderID = _c.AddWorkOrderDetails(_cm);
                _cm.QuotationID =Convert.ToInt32(hdQuotationID.Value);
                _cm.RequestID = Convert.ToInt32(hdRequestID.Value);
                _cm.UpdateSampleWO(_cm);

                Session["QuotationID"] = hdQuotationID.Value;
                Session["RequestID"] = hdRequestID.Value;
                Session["WorkOrderID"] = WorkOrderID;

                _cm.RequestID = Convert.ToInt32(hdRequestID.Value) ;
                _cm.calibrationStatusID = 36;// SPK to Calibration Staff;
                _cm.UpdateToolStatus(_cm);
                _cm.UpdateRequestStatus(_cm);

                DataTable dtComplete = _cm.getToolStatus(36);
                if(dtComplete.Rows.Count == 0)
                {
                    _cm.QuotationStatusID = 36;// SPK to Calibration Staff
                    _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                    _cm.UpdateQuotationCalibStatus(_cm);
                }
                Response.Redirect("generateWork_Order_TSQ_CAL_FM_002_SPK.aspx?q=" + hdQuotationID.Value + "&r=" + hdRequestID.Value);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Fill highlighted fields.')", true);
            }
        }
        private string GenerateWorkOrderNumber()
        {
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string newWorkOrderNumber = "WO/" + year + month; // Default order number in case no orders exist

            DataTable dtQO = _cm.GenerateWorkOrderNumber();
            if (dtQO.Rows.Count > 0)
            {
                newWorkOrderNumber += (dtQO.Rows.Count+1).ToString("0000");
            }
            else
            {
                newWorkOrderNumber += "0001";
            }

                return newWorkOrderNumber;
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
    }
}