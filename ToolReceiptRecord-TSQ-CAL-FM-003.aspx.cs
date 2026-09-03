using LIMS.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ToolReceiptRecord_TSQ_CAL_FM_003 : System.Web.UI.Page
    {
        DAL.CM _cm = new CM();
        int EmployeeID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInstruments();
                BindCustomerDetails();
                BindCalibrationScope();
                txtReceiptDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        protected void BindCalibrationScope()
        {
            DataTable dt = _cm.getCalibrationScope();
            if (dt.Rows.Count > 0)
            {
                chkScope.DataSource = dt;
                chkScope.DataTextField = "Lab";
                chkScope.DataValueField = "calibrationScopeID";
                chkScope.DataBind();
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
            txtReceiptDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DataTable dt = _cm.getQuotationCustomer(hdQuotationID.Value);
            if (dt.Rows.Count > 0)
            {
                lblCustomerMasterID.Text = dt.Rows[0]["CustomerMasterID"].ToString();
                txtCustomerName.Text = dt.Rows[0]["CompanyName"].ToString();
                txtPONumber.Text = dt.Rows[0]["PONumber"].ToString();
            }
            if (Session["EmployeeName"] != null)
            {
                txtCheckedBy.Text = Session["EmployeeName"].ToString();
            }
            if (Session["Usertype"] != null)
            {
                txtCheckedByDesignation.Text = Session["Usertype"].ToString();
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
            _cm.VerificationStatus = Convert.ToInt32(radStatus.SelectedValue);
            _cm.UpdateToolVerificationStatus(_cm);
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
            Random rnd = new Random();
            int SampleUId = rnd.Next(10000000, 99999999);
            string LastSampleID = "";
            string SampleIDSeries = "";

            if (txtReceiptDate.Text == "") { flag = 1; txtReceiptDate.BackColor = Color.LightPink; } else { txtReceiptDate.ForeColor = Color.Empty; }
            if (txtPlace.Text == "") { flag = 1; txtPlace.BackColor = Color.LightPink; } else { txtPlace.ForeColor = Color.Empty; }
            if (txtCheckedBy.Text == "") { flag = 1; txtCheckedBy.BackColor = Color.LightPink; } else { txtCheckedBy.ForeColor = Color.Empty; }
            if (txtCheckedByDesignation.Text == "") { flag = 1; txtCheckedByDesignation.BackColor = Color.LightPink; } else { txtCheckedByDesignation.ForeColor = Color.Empty; }
            if (txtCustomerContact.Text == "") { flag = 1; txtCustomerContact.BackColor = Color.LightPink; } else { txtCustomerContact.ForeColor = Color.Empty; }
            if (txtCustomerContactDesignation.Text == "") { flag = 1; txtCustomerContactDesignation.BackColor = Color.LightPink; } else { txtCustomerContactDesignation.ForeColor = Color.Empty; }
            if (chkScope.SelectedIndex == -1) { flag = 1; chkScope.BackColor = Color.LightPink; } else { chkScope.ForeColor = Color.Empty; }
            if (flag == 0)
            {
                _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                _cm.RequestID = Convert.ToInt32(hdRequestID.Value);
                _cm.ToolReceiptDate = Convert.ToDateTime(txtReceiptDate.Text);
                _cm.Place = txtPlace.Text;
                _cm.CheckedBy = txtCheckedBy.Text;
                _cm.CheckedByDesignation = txtCheckedByDesignation.Text;
                _cm.CustomerContact = txtCustomerContact.Text;
                _cm.CustomerContactDesignation = txtCustomerContactDesignation.Text;
                _cm.Notes = txtNotes.Text;
                
                for (int i = 0; i < chkScope.Items.Count; i++)
                {
                    if (chkScope.Items[i].Selected == true)
                    {
                        _cm.CalibrationScope += chkScope.Items[i].Value + ",";
                    }
                }
                _cm.AddToolVerification(_cm);

                if (Session["OrganizationID"] != null)
                {
                    string prefix = ""; DataTable dt = new DataTable();
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    DataTable dtOrg = _cm.GetOrganizationDetails();
                    if (dtOrg.Rows.Count > 0)
                    {
                        prefix = dtOrg.Rows[0]["PrefixSample"].ToString();
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
                            int lastItem = Convert.ToInt32(TestReportString[1].Substring(TestReportString[1].Length - 4));// Convert.ToInt32(TestReportString[TestReportString.Length - 1]);

                            //Check year change. Reset series to '0001'
                            string midTerm = TestReportString[1].Substring(0, 2);// TestReportString[TestReportString.Length - 2];
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
                        if (Session["EmployeeID"] != null)
                        {
                            EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                        }
                        _cm.receivedDate = Convert.ToDateTime(txtReceiptDate.Text);
                        _cm.sampleId = SampleUId;
                        _cm.created = DateTime.Now;
                        _cm.sampleIDSeries = SampleIDSeries;
                        _cm.Remarks = txtNotes.Text;
                        _cm.SampleVerifiedBy = EmployeeID;
                        _cm.SamplingLocation = txtPlace.Text;
                        _cm.DateOfSampling = DateTime.Now;
                        _cm.OrganizationID = 1;
                        _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                        _cm.RequestID = Convert.ToInt32(hdRequestID.Value);
                    }
                }
                Boolean Accepted = true;
                foreach (GridViewRow row in gvInstruments.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        Label lblStatus = row.FindControl("lblStatus") as Label;

                        if (lblStatus != null)
                        {
                            string statusText = lblStatus.Text;

                            // Check label text
                            if (statusText == "Tidak Baik")
                            {
                                Accepted = false;
                            }
                        }
                    }
                }
                _cm.RequestID = Convert.ToInt32(hdRequestID.Value);
                if (Accepted)
                {                   
                    _cm.sampleStatusID = 51;//Tools Accepted by CS
                    _cm.AddSampleDetailsCalib(_cm);
                   
                    _cm.calibrationStatusID = 51;
                    _cm.UpdateToolStatus(_cm);
                    _cm.UpdateRequestStatus(_cm);
                }
                //Add Sample

                else
                {                   
                    _cm.sampleStatusID = 52;//Tools Rejected by CS
                    _cm.AddSampleDetailsCalib(_cm);
                    
                    _cm.calibrationStatusID = 52;
                    _cm.UpdateToolStatus(_cm);
                    _cm.UpdateRequestStatus(_cm);
                }
                Session["QuotationID"] = hdQuotationID.Value;
                DataTable VerificationStatus = _cm.getToolVerificationStatus(hdQuotationID.Value);
                if (VerificationStatus.Rows.Count == 0)
                {
                    _cm.QuotationStatusID = 22;// Tools Received by CS
                    _cm.QuotationID = Convert.ToInt32(hdQuotationID.Value);
                    _cm.UpdateQuotationCalibStatus(_cm);
                }
                Response.Redirect("generateToolReceiptRecord_TSQ_CAL_FM_003.aspx?q="+hdQuotationID.Value + "&r=" + hdRequestID.Value);

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
    }
}