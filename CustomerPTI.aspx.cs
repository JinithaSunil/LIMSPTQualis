using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerPTI : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        if (inquiryStatus == 5)
                        {
                            divCRF.Visible = true;
                            divFail.Visible = false;
                            Session["InquiryID"] = InquiryId;
                            lblInquiry.Text = "ENQ" + InquiryId;
                            hdInquiryId.Value = InquiryId.ToString();
                            DataTable dt = _cm.BindInquiryCustomer(InquiryId);
                            if (dt.Rows.Count > 0)
                            {
                                txtDateOfRequest.Text = dt.Rows[0]["Created"].ToString();
                                txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                                Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
                            }
                            BindContactPerson(InquiryId);                            
                        }
                        else
                        {
                            divCRF.Visible = false;
                            divFail.Visible = true;
                        }
                    }
                    if (gvSample.Rows.Count == 0)
                    {
                        btnSubmit.Visible = false;
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetTestParameters(string prefixText, int count)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = "Select * from TestParameter where TestParameter like " + " @Search + '%'";
                    com.Parameters.AddWithValue("@Search", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> TestParameter = new List<string>();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            TestParameter.Add(sdr["TestParameter"].ToString());
                        }
                    }
                    con.Close();
                    return TestParameter;


                }

            }

        }
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetTestMethods(string prefixText, int count)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = "Select * from TestMethod where testmethod like " + " @Search + '%'";
                    com.Parameters.AddWithValue("@Search", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> TestMethod = new List<string>();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            TestMethod.Add(sdr["TestMethod"].ToString());
                        }
                    }
                    con.Close();
                    return TestMethod;


                }

            }

        }
        private void BindContactPerson(int InquiryId)
        {
            DataTable dt = _cm.BindContactPerson(InquiryId);
            if (dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "CustomerName";
                ddlContactPerson.DataValueField = "PersonId";
                ddlContactPerson.DataBind();
                ddlContactPerson.Items.Insert(0, new ListItem("Select Person"));
            }
        }

        protected void ddlInquiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            int InquiryId = Convert.ToInt32(lblInquiry.Text);
            DataTable dt = _cm.BindInquiryCustomer(InquiryId);
            if (dt.Rows.Count > 0)
            {
                txtDateOfRequest.Text = dt.Rows[0]["Created"].ToString();
                txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
            }
            BindContactPerson(InquiryId);
        }

        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            int PersonId = 0;
            if (ddlContactPerson.SelectedIndex != 0)
            {
                PersonId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                DataTable dt = _cm.BindCustomerContactPerson(PersonId);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        txtMobile.Text = dt.Rows[i]["Mobile"].ToString();
                        txtEmail.Text = dt.Rows[i]["Email"].ToString();
                        txtPhone.Text = dt.Rows[i]["Phone"].ToString();
                        txtDesignation.Text = dt.Rows[i]["Designation"].ToString();
                        txtDepartment.Text = dt.Rows[i]["Department"].ToString();
                        txtAddress.Text = dt.Rows[0]["AddressLine1"].ToString();
                    }
                }
            }
        }

        protected void gvSample_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DT"];
                dt.Rows[index].Delete();
                gvSample.DataSource = dt;
                gvSample.DataBind();
                if (gvSample.Rows.Count == 0)
                {
                    btnSubmit.Visible = false;
                }
                else
                {
                    btnSubmit.Visible = true;
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            AddSampleDetails();
        }

        private void AddSampleDetails()
        {
            int flag = 0;
            if (txtTestParameter.Text == "")
            {
                txtTestParameter.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtTestParameter.BackColor = System.Drawing.Color.Empty;
            }
            if (txtTestMethod.Text == "")
            {
                txtTestMethod.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtTestMethod.BackColor = System.Drawing.Color.Empty;
            }
            if (txtSizeofSample.Text == "")
            {
                txtSizeofSample.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSizeofSample.BackColor = System.Drawing.Color.Empty;
            }
            if (txtSampleName.Text == "")
            {
                txtSampleName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSampleName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtSampleType.Text == "")
            {
                txtSampleType.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSampleType.BackColor = System.Drawing.Color.Empty;
            } 
            if (txtSizeofSample.Text == "")
            {
                txtSizeofSample.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtSizeofSample.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                DataTable dt = new DataTable();
                if (Session["DT"] != null)
                {
                    dt = (DataTable)Session["DT"];
                }
                else
                {
                    dt.Columns.Add("SampleName");
                    dt.Columns.Add("SampleType");
                    dt.Columns.Add("testParameter");
                    dt.Columns.Add("testMethod");
                    dt.Columns.Add("SizeofSample");
                    dt.Columns.Add("Remarks");
                }
                DataRow dr = dt.NewRow();
                dr["SampleName"] = txtSampleName.Text.ToString();
                dr["testParameter"] = txtTestParameter.Text.ToString();
                dr["testMethod"] = txtTestMethod.Text.ToString();
                dr["SampleType"] = txtSampleType.Text;                                
                dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                dr["Remarks"] = txtRemarks.Text;
                dt.Rows.Add(dr);
                gvSample.DataSource = dt;
                gvSample.DataBind();
                if (gvSample.Rows.Count == 0)
                {
                    btnSubmit.Visible = false;
                }
                else
                {
                    btnSubmit.Visible = true;
                }
                Session["DT"] = dt;
                Clear();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Fill highlighted fields');", true);
            }
        }

        private void Clear()
        {
            // txtSampleName.Text = "";
            //  ddlSampleType.SelectedIndex = 0;
            txtTestMethod.Text = "";
            txtTestParameter.Text = "";
            txtSizeofSample.Text = "";
            txtRemarks.Text = "";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtDateOfRequest.Text == "")
            {
                txtDateOfRequest.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDateOfRequest.BackColor = System.Drawing.Color.Empty;
            }
            if (lblInquiry.Text == "")
            {
                lblInquiry.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                lblInquiry.BackColor = System.Drawing.Color.Empty;
            }
            if (txtEmail.Text == "")
            {
                txtEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (!(System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")))
            {
                txtEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtEmail.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                int PersonId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                _cm.personId = PersonId;
                _cm.customerId = Convert.ToInt32(Session["CustomerId"]);
                string sQuery = "Select cd.*,ld.AddressLine1 from ContactDetails cd inner join LocationDetails ld on cd.CustomerId = ld.CustomerId where cd.PersonId = '" + PersonId + "'";
                DataTable dt1 = _cm.GetDataUsingQuery(sQuery);
                if (dt1.Rows.Count > 0)
                {
                    if (txtPhone.Text != dt1.Rows[0]["Phone"].ToString())
                    {
                        _cm.phone = txtPhone.Text.ToString();
                        _cm.UpdatePhoneNumber(_cm);
                    }
                    if (txtMobile.Text != dt1.Rows[0]["Mobile"].ToString())
                    {
                        _cm.mobile = txtMobile.Text.ToString();
                        _cm.UpdateMobileNumber(_cm);
                    }
                    if (txtDepartment.Text != "")
                    {
                        _cm.department = txtDepartment.Text.ToString();
                        _cm.UpdateDepartment(_cm);
                    }
                    if (txtDesignation.Text != "")
                    {
                        _cm.designation = txtDesignation.Text.ToString();
                        _cm.UpdateDesignation(_cm);
                    }
                    if (txtEmail.Text != "")
                    {
                        string sQuery1 = "Select Email from ContactDetails where PersonId='" + PersonId;
                        DataTable dt2 = _cm.GetDataUsingQuery(sQuery1);
                        string sQuery2 = "Select * from ContactDetails where CustomerId='" + _cm.customerId + "'";
                        DataTable dt3 = _cm.GetDataUsingQuery(sQuery2);
                        if (dt2.Rows.Count == 0)
                        {
                            _cm.CustomerMasterId = Convert.ToInt32(dt3.Rows[0]["CustomerMasterId"].ToString());
                            _cm.customerType = dt3.Rows[0]["CustomerType"].ToString();
                            _cm.personId = PersonId;
                            _cm.email = txtEmail.Text.ToString();
                            _cm.UpdateEmailAddress(_cm);
                        }
                    }
                    if (txtAddress.Text != "")
                    {
                        if (txtAddress.Text != dt1.Rows[0]["AddressLine1"].ToString())
                        {
                            _cm.addressLine1 = txtAddress.Text.ToString();
                            _cm.UpdateAddress(_cm);
                        }
                    }
                }
                int RequestId = 0;
                _cm.dateOfRequest = Convert.ToDateTime(txtDateOfRequest.Text);
                _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                _cm.customerId = Convert.ToInt32(Session["CustomerId"]);
                _cm.CompanyName = txtCustomer.Text.ToString();
                _cm.shippingAgency = "";
                _cm.airwayBillNo = "";
                _cm.personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                _cm.phone = txtPhone.Text.ToString();
                _cm.mobile = txtMobile.Text.ToString();
                _cm.email = txtEmail.Text.ToString();
                _cm.addressLine1 = txtAddress.Text.ToString();
                _cm.designation = txtDesignation.Text.ToString();
                _cm.department = txtDepartment.Text.ToString();

                _cm.requestTypeId = 3;
                RequestId = _cm.AddCustomerRequest(_cm);
                Session["RequestId"] = RequestId;

                /* Update TAFNo in CustomerRequestDetails*/
                _cm.GenerateTAFNo(RequestId,"TAF");                
                /* Update TAFNo in CustomerRequestDetails*/
				
                if (Session["DT"] != null)
                {
                    DataTable dt = (DataTable)Session["DT"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        int SampleId = 0;
                        _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                        _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                        _cm.sampleName = dr["SampleName"].ToString();                        
                        _cm.SampleType = dr["SampleType"].ToString();
                        _cm.batchNo = "";
                        _cm.testMethod = dr["testMethod"].ToString();
                        _cm.testParameter = dr["testParameter"].ToString();
                        _cm.specification = "";
                        _cm.sizeOfSample = dr["sizeOfSample"].ToString();
                        _cm.remarks = dr["Remarks"].ToString();
                        _cm.CRFID = 3;
                        
                        SampleId = _cm.AddCustomerCRFSampleDetails(_cm);

                        //Session["SampleId"] = SampleId;
                        //_cm.sampleId = Convert.ToInt32(SampleId);
                        //_cm.description = dr["PETresin"].ToString();
                        //_cm.description2 = dr["Preforms"].ToString();
                        //_cm.description3 = dr["Bottles"].ToString();
                        //_cm.AddDescriptionDetails(_cm);
                    }
                    _cm.inquiryStatusId = 6;
                    _cm.UpdateInquiryStatus(_cm);

                    ClearAll();
                    Response.Redirect("CreateRequestCustomerPTI.aspx?ID=" + Session["InquiryID"]);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
        }

        private void ClearAll()
        {
            txtDateOfRequest.Text = "";
            lblInquiry.Text = "";
            txtCustomer.Text = "";
            ddlContactPerson.SelectedIndex = 0;
            txtPhone.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtDesignation.Text = "";
            txtDepartment.Text = "";

            gvSample.DataSource = null;
            gvSample.DataBind();
            if (gvSample.Rows.Count == 0)
            {
                btnSubmit.Visible = false;
            }
            else
            {
                btnSubmit.Visible = true;
            }
            Session["DT"] = null;
        }

        protected void gvSample_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int row = e.RowIndex;
            int r = 0;
            DataTable dt = (DataTable)Session["DT"];
            foreach (DataRow row1 in dt.Rows)
            {
                if (r.ToString() == row.ToString())
                {
                    row1.Delete();
                }
                r++;
                if(r == dt.Rows.Count) { break; }                
            }
            dt.AcceptChanges();
            Session["DT"] = dt;
            gvSample.DataSource = dt;
            gvSample.DataBind();
        }
    }
}