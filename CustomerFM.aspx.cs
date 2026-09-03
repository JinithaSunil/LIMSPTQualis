using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFM : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    int ReportID = Convert.ToInt32(Request.QueryString["r"].ToString());
                    Session["ReportID"] = ReportID;
                    
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        if (inquiryStatus == 5)
                        {
                            int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());
                            if (DepartmentID == 4)
                            {
                                string url = "CustomerFMN_Chemical.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 4006)
                            {
                                string url = "CustomerFMN_PV.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 4007)
                            {
                                string url = "CustomerFMN_Mechanical.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 4008)
                            {
                                string url = "CustomerFMN_HomeAppliances.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 4009)
                            {
                                string url = "CustomerFMN_LED.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else if (DepartmentID == 3006)
                            {
                                string url = "CustomerFMN_ElectricalAccessories.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                            else
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
                                if (Session["DT"] == null)
                                {
                                    tblTestParam.Visible = false;
                                }
                                else
                                {
                                    tblTestParam.Visible = true;
                                }
                            }
                            
                        }
                        else
                        {
                            divCRF.Visible = false;
                            divFail.Visible = true;
                        }
                    }
                    if (gvCRF.Rows.Count == 0)
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
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod(EnableSession =true)]
        public static List<string> GetTestParameters(string prefixText, int count)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = "Select * from TestParameter where TestParameter like " + " @Search + '%' and ReportID = " + HttpContext.Current.Session["ReportID"].ToString();
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
        private void BindTestParameter()
        {

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

        protected void gvCRF_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DT"];
                dt.Rows[index].Delete();
                gvCRF.DataSource = dt;
                gvCRF.DataBind();
                if (gvCRF.Rows.Count == 0)
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
            AddDetails();
        }

        private void AddDetails()
        {
            int flag = 0;
            if (txtSampleName.Text == "")
            {
                txtSampleName.BackColor = System.Drawing.Color.LightPink;
                txtSampleName.Attributes.Add("style", "outline: 1px solid red");
                flag = 1;
            }
            else
            {
                txtSampleName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtBatchNo.Text == "")
            {
                txtBatchNo.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtBatchNo.BackColor = System.Drawing.Color.Empty;
            }
            if (txtTestParameter.Text == "")
            {
                txtTestParameter.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtTestParameter.BackColor = System.Drawing.Color.Empty;
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
                    dt.Columns.Add("BatchNo");
                    dt.Columns.Add("TestMethod");
                    dt.Columns.Add("TestParameter");
                    dt.Columns.Add("TestSubParameter");
                    dt.Columns.Add("Specification");
                    dt.Columns.Add("SizeofSample");
                    dt.Columns.Add("Remarks");

                }
                DataRow dr = dt.NewRow();
                dr["SampleName"] = txtSampleName.Text.ToString();
                dr["BatchNo"] = txtBatchNo.Text.ToString();
                dr["TestMethod"] = txtTestMethod.Text.ToString();
                dr["TestParameter"] = txtTestParameter.Text;
                if (ddlTestSubParameter.Items.Count > 0)
                {
                    if (ddlTestSubParameter.SelectedIndex == 0)
                    {
                        ddlTestSubParameter.BorderColor = Color.Red;
                        flag = 1;
                    }
                    else
                    {
                        dr["TestSubParameter"] = ddlTestSubParameter.SelectedItem.ToString();
                    }
                }
                else 
                if(ddlTestSubParameter.SelectedItem.ToString() == "")
                {
                    dr["TestSubParameter"] = "";
                }
                
                dr["Specification"] = txtSpecification.Text.ToString();
                dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                dr["Remarks"] = txtRemarks.Text.ToString();

                dt.Rows.Add(dr);
                gvCRF.DataSource = dt;
                gvCRF.DataBind();
                if (gvCRF.Rows.Count == 0)
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
            //txtSampleName.Text = "";
            //txtBatchNo.Text = "";
            txtTestMethod.Text = "";
            txtTestParameter.Text = "";
            txtSpecification.Text = "";
            //txtSizeofSample.Text = "";
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
                _cm.personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                _cm.shippingAgency = txtShippingAgency.Text.ToString();
                _cm.airwayBillNo = txtAirwayBillNo.Text.ToString();
                _cm.requestTypeId = 1;
                
                RequestId = _cm.AddCustomerRequest(_cm);
                Session["RequestId"] = RequestId;

                /* Update TAFNo in CustomerRequestDetails*/
                _cm.GenerateTAFNo(RequestId,"TAF");                
                /* Update TAFNo in CustomerRequestDetails*/

                if (Session["DT"] != null)
                {
                    if (Session["OrganizationID"] != null)
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    }
                    DataTable dt = (DataTable)Session["DT"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                        _cm.inquiryId = Convert.ToInt32(hdInquiryId.Value);
                        _cm.sampleName = dr["SampleName"].ToString();
                        _cm.SampleType = "";
                        _cm.batchNo = dr["BatchNo"].ToString();
                        _cm.testMethod = dr["TestMethod"].ToString();
                        _cm.testParameter = dr["TestParameter"].ToString();
                        _cm.testSubParameter = dr["TestSubParameter"].ToString();
                        _cm.specification = dr["Specification"].ToString();
                        _cm.sizeOfSample = dr["SizeofSample"].ToString();
                        _cm.remarks = dr["Remarks"].ToString();
                        _cm.CRFID = 1;
                        _cm.AddCustomerCRFSampleDetails(_cm);
                    }
                }

                //change customer inquiry Status to CRF Received
                _cm.inquiryStatusId = 6;
                _cm.UpdateInquiryStatus(_cm);


                ClearAll();
                Response.Redirect("CreateRequestCustomerFM.aspx?ID=" + Session["InquiryID"] + "&r=" + Session["ReportID"].ToString());
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
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtDesignation.Text = "";
            txtDepartment.Text = "";

            gvCRF.DataSource = null;
            gvCRF.DataBind();
            if (gvCRF.Rows.Count == 0)
            {
                btnSubmit.Visible = false;
            }
            else
            {
                btnSubmit.Visible = true;
            }
            Session["DT"] = null;
        }
        protected void BindSampleDetails()
        {
            DataTable dt = new DataTable();
            if (Session["DT"] != null)
            {
                dt = (DataTable)Session["DT"];
                if (dt.Rows.Count > 0)
                {
                    lblSampleName.Text = "Sample Name:" + dt.Rows[0]["SampleName"].ToString();
                    lblBatchNo.Text = "Batch#:" + dt.Rows[0]["BatchNo"].ToString();
                    lblSize.Text = "Size/ Nos of Sample:" + dt.Rows[0]["SizeofSample"].ToString();
                }
            }
        }

        protected void btnAddSample_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (Session["OrganizationID"] == null)
            {
                Session["OrganizationID"] = 1;
            }
            if (txtSampleName.Text == "")
            {
                txtSampleName.BackColor = System.Drawing.Color.LightPink;
                txtSampleName.Attributes.Add("style", "outline: 1px solid red");
                flag = 1;
            }
            else
            {
                txtSampleName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtBatchNo.Text == "")
            {
                txtBatchNo.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtBatchNo.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                DataTable dtParam = new DataTable();
                DataTable dt = new DataTable();
                if (Session["DT"] != null)
                {
                    dt = (DataTable)Session["DT"];
                }
                else
                {
                    dt.Columns.Add("SampleName");
                    dt.Columns.Add("BatchNo");
                    dt.Columns.Add("TestMethod");
                    dt.Columns.Add("TestParameter");
                    dt.Columns.Add("TestSubParameter");
                    dt.Columns.Add("Specification");
                    dt.Columns.Add("SizeofSample");
                    dt.Columns.Add("Remarks");

                }
               
               
                if (Request.QueryString["r"]!=null)
                {
                    dtParam = _cm.getReportTestParameter(Convert.ToInt32(Request.QueryString["r"].ToString()));
                    if(dtParam.Rows.Count > 0)
                    {
                        foreach(DataRow dr1 in dtParam.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr["SampleName"] = txtSampleName.Text.ToString();
                            dr["BatchNo"] = txtBatchNo.Text.ToString();
                            dr["SizeofSample"] = txtSizeofSample.Text.ToString();
                            
                            dr["TestParameter"] = dr1["TestParameter"].ToString();
                            dr["TestSubParameter"] = dr1["TestSubParameter"].ToString();
                            if (dr1["HasSubParameter"].ToString() == "1")
                            {
                                dr["TestMethod"] = dr1["Protocol1"].ToString();
                                dr["Specification"] = dr1["Specification1"].ToString();
                            }
                            else
                            {
                                dr["TestMethod"] = dr1["Protocol"].ToString();
                                dr["Specification"] = dr1["Specification"].ToString();
                            }
                           
                            dr["Remarks"] = "";
                            dt.Rows.Add(dr);
                        }                       
                    }
                }

                gvCRF.DataSource = dt;
                gvCRF.DataBind();
                if (gvCRF.Rows.Count == 0)
                {
                    btnSubmit.Visible = false;
                }
                else
                {
                    btnSubmit.Visible = true;
                }
                Session["DT"] = dt;
                tblTestParam.Visible = true;
                BindSampleDetails();
            }
           
        }

        protected void txtTestParameter_TextChanged(object sender, EventArgs e)
        {
            string param = ""; DataTable dt = new DataTable();
            int ReportID = Convert.ToInt32(Request.QueryString["r"].ToString());
            if (txtTestParameter.Text != "")
            {
                param = txtTestParameter.Text;
                dt = _cm.getTestSubParameter(param);
                if (dt.Rows.Count > 0)
                {
                    ddlTestSubParameter.Visible = true;
                    ddlTestSubParameter.DataSource = dt;
                    ddlTestSubParameter.DataTextField = "TestSubParameter";
                    ddlTestSubParameter.DataValueField = "TestSubParameterID";
                    ddlTestSubParameter.DataBind();
                    ddlTestSubParameter.Items.Insert(0, "Select");
                    txtTestMethod.Text = dt.Rows[0]["Protocol"].ToString();
                }
                else
                {
                    dt = _cm.getTestParameter(param, ReportID);
                    ddlTestSubParameter.Visible = false;
                    ddlTestSubParameter.DataSource = null; ddlTestSubParameter.DataBind();
                    ddlTestSubParameter.Items.Insert(0, "NA");
                    if(dt.Rows.Count > 0)
                    {
                        txtTestMethod.Text = dt.Rows[0]["Protocol"].ToString();
                    }
                   
                }
            }
        }

        protected void ddlTestSubParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (ddlTestSubParameter.SelectedIndex != 0)
            {
                dt=_cm.GetProtocolTestSubParameter(ddlTestSubParameter.SelectedItem.ToString());
                if (dt.Rows.Count > 0)
                {
                    txtTestMethod.Text = dt.Rows[0]["Protocol"].ToString();
                }
                else
                {
                    txtTestMethod.Text = "";
                }
            }
           
        }
    }
}