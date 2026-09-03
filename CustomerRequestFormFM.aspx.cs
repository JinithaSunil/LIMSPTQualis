using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerRequestFormFM : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
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
                if (!Page.IsPostBack)
                {
                    BindCustomer();
                    BindInquiry();
                    if (gvCRF.Rows.Count == 0)
                    {
                        btnSubmit.Visible = false;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
           
        }

        private void BindCustomer()
        {
            string sQuery = "select CustomerId, (Title +' '+ FirstName +' '+ LastName) as CustomerName from CustomerDetails union select CompanyId as CustomerId,CompanyName as CustomerName from CompanyDetails; ";
            //string sQuery = "Select * from ContactPerson";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                //ddlCustomerName.DataSource = dt;
                //ddlCustomerName.DataTextField = "CustomerName";
                //ddlCustomerName.DataValueField = "CustomerId";
                //ddlCustomerName.DataBind();
                //ddlCustomerName.Items.Insert(0, new ListItem("Select Customer", ""));
                //Session["CustMasterId"] = ddlCustomerName.SelectedItem.Value;
                //BindContactPerson();
            }
        }

        private void BindContactPerson(int InquiryId)
        {
            //int InquiryId = Convert.ToInt32(ddlInquiry.SelectedItem.Value);
            string sQuery = "select a.PersonId,(a.Title + ' ' + a.FirstName) as CustomerName from ContactPersonDetails a inner join CustomerDetails b on a.CustomerMasterId = b.CustomerMasterId inner join CustomerInquiry c on b.CustomerId = c.CustomerId where c.InquiryId = '" + InquiryId + "'union select a.PersonId,(a.Title + ' ' + a.FirstName) as CustomerName from ContactPersonDetails a inner join CompanyDetails b on a.CustomerMasterId = b.CustomerMasterId inner join CustomerInquiry c on b.CompanyId = c.CustomerId where c.InquiryId = '" + InquiryId + "'";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "CustomerName";
                ddlContactPerson.DataValueField = "PersonId";
                ddlContactPerson.DataBind();
                ddlContactPerson.Items.Insert(0, new ListItem("Select Person"));
            }
            else
            {
                ddlContactPerson.DataSource = null;
                ddlContactPerson.DataTextField = "";
                ddlContactPerson.DataValueField = "";
                ddlContactPerson.DataBind();
            }
        }

        private void BindInquiry()
        {
            string sQuery = "Select * from CustomerInquiry";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                ddlInquiry.DataSource = dt;
                ddlInquiry.DataTextField = "InquiryId";
                ddlInquiry.DataValueField = "InquiryId";
                ddlInquiry.DataBind();
                ddlInquiry.Items.Insert(0, new ListItem("Select Inquiry Id", ""));
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
            if (txtTestMethod.Text == "")
            {
                txtTestMethod.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtTestMethod.BackColor = System.Drawing.Color.Empty;
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
          

            if (ddlSizeofSample.SelectedIndex == 0)
            {
                ddlSizeofSample.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlSizeofSample.BackColor = System.Drawing.Color.Empty;
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
                dr["TestParameter"] = txtTestParameter.Text.ToString();
                dr["Specification"] = txtSpecification.Text.ToString();
                dr["SizeofSample"] = ddlSizeofSample.SelectedValue.ToString();
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
            txtSampleName.Text = "";
            txtBatchNo.Text = "";
            txtTestMethod.Text = "";
            txtTestParameter.Text = "";
            txtSpecification.Text = "";
            ddlSizeofSample.SelectedIndex = 0;
            txtRemarks.Text = "";
        }

        protected void ddlInquiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            int InquiryId = Convert.ToInt32(ddlInquiry.SelectedItem.Value);
            //string sQuery = "select a.PersonId,(a.Title + ' ' + a.FirstName) as CustomerName from ContactPersonDetails a inner join CustomerDetails b on a.CustomerMasterId = b.CustomerMasterId inner join CustomerInquiry c on b.CustomerId = c.CustomerId where c.InquiryId = 12 union select d.CompanyId,d.CompanyName as CustomerName from CompanyDetails d";
            string sQuery = "select ci.CustomerId,ci.InquiryId,(cd.Title+' '+cd.firstname) as customername from customerdetails cd inner join CustomerInquiry ci on ci.CustomerId = cd.CustomerId where ci.InquiryId = '" + InquiryId + "' union select ci.CustomerId,ci.InquiryId,cm.companyname as customername from companydetails cm inner join CustomerInquiry ci on ci.CustomerId = cm.CompanyId where ci.InquiryId = '" + InquiryId + "'";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
            }
            BindContactPerson(InquiryId);
        }

        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            int PersonId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
            string sQuery = "Select cd.*,ld.AddressLine1 from ContactDetails cd inner join LocationDetails ld on cd.CustomerId = ld.CustomerId where cd.PersonId = '" + PersonId + "'";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                txtMobile.Text = dt.Rows[0]["Mobile"].ToString();
                txtEmail.Text = dt.Rows[0]["Email"].ToString();
                txtFax.Text = dt.Rows[0]["Fax"].ToString();
                txtAddress.Text = dt.Rows[0]["AddressLine1"].ToString();
            }
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
            if (ddlInquiry.SelectedIndex == 0)
            {
                ddlInquiry.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlInquiry.BackColor = System.Drawing.Color.Empty;
            }

            if (flag == 0)
            {
                int RequestId = 0;
                _cm.dateOfRequest = Convert.ToDateTime(txtDateOfRequest.Text);
                _cm.inquiryId = Convert.ToInt32(ddlInquiry.SelectedItem.Value);
                _cm.customerId = Convert.ToInt32(Session["CustomerId"]);
                _cm.CompanyName = txtCustomer.Text.ToString();
                _cm.personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                _cm.mobile = txtMobile.Text.ToString();
                _cm.fax = txtFax.Text.ToString();
                _cm.email = txtEmail.Text.ToString();
                _cm.addressLine1 = txtAddress.Text.ToString();
                _cm.designation = txtDesignation.Text.ToString();
                _cm.department = txtDepartment.Text.ToString();
                _cm.shippingAgency = txtShippingAgency.Text.ToString();
                _cm.airwayBillNo = txtAirwayBillNo.Text.ToString();
                _cm.customerName = txtCustomerName.Text.ToString();
                _cm.customerSignature = txtCustomerSignature.Text.ToString();
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
                        DataTable dt = (DataTable)Session["DT"];
                        foreach (DataRow dr in dt.Rows)
                        {
                            _cm.customerRequestId = Convert.ToInt32(Session["RequestId"]);
                            _cm.inquiryId = Convert.ToInt32(ddlInquiry.SelectedItem.Value);
                            _cm.sampleName = dr["SampleName"].ToString();
                            _cm.SampleType = "";
                            _cm.batchNo = dr["BatchNo"].ToString();
                            _cm.testMethod = dr["TestMethod"].ToString();
                            _cm.testParameter = dr["TestParameter"].ToString();
                            _cm.testSubParameter = dr["TestSubParameter"].ToString();
                            _cm.specification = dr["Specification"].ToString();
                            _cm.sizeOfSample = dr["SizeofSample"].ToString();
                            _cm.remarks = dr["Remarks"].ToString();
                            _cm.AddCRFSampleDetails(_cm);
                        }
                    }
                }
                ClearAll();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
        }

        private void ClearAll()
        {
            txtDateOfRequest.Text = "";
            ddlInquiry.SelectedIndex = 0;
            txtCustomer.Text = "";
            ddlContactPerson.SelectedIndex = 0;
            txtMobile.Text = "";
            txtFax.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtDesignation.Text = "";
            txtDepartment.Text = "";
            txtCustomerName.Text = "";
            txtCustomerSignature.Text = "";

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
    }
}

