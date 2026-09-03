using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf.security;

namespace LIMS
{
    public partial class GenrateCode : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());

        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");

        string CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string UpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    btnSubmit.Enabled = false;
                    if (Request.QueryString["ID"] != null)
                    {
                        _cm.InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                        String Type = Request.QueryString["Type"].ToString();

                    }
                    else if (Request.QueryString["Type"] != null)
                    {
                        String Type = Request.QueryString["Type"].ToString();
                        if (Type == "Ven")
                        {
                            ddlType.SelectedValue = "Vendor";
                            CheckType();
                        }
                    }

                    //if (Session["Email"] != null)
                    //{
                    //    txtEmail.Text = Session["Email"].ToString();
                    //}
                    //if (Session["RegType"] != null)
                    //{
                    //    ddlType.SelectedValue = "Customer";
                    //    CheckType();
                    //}
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
                BindGridView();
            }
            
        }
        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckType();
        }
        protected void CheckType()
        {
            string type = "";
            if (ddlType.SelectedIndex != 0)
            {
                type = ddlType.SelectedItem.ToString();
                switch (type)
                {
                    case "Vendor":
                        Session["PreFix"] = "VEN";
                        //Session["Link"] = "VendorNew.aspx";
                        Session["Link"] = "http://ptqualis.rnisolutions.in:4545/VendorNew.aspx";
                        break;
                    //case "RM Vendor":
                    //    Session["PreFix"] = "RMVEN";
                    //    Session["Link"] = "";
                    //    break;
                    case "Customer":
                        Session["PreFix"] = "CUS";
                       // Session["Link"] = "";
                        break;
                    default: break;
                }
            }
            else
            {
                ddlType.BackColor = System.Drawing.Color.LightPink;
            }
        }

        protected void btnGenerateReference_Click(object sender, EventArgs e)
        {
            Random rand = new System.Random(); int count = 0;
            int flag = 1;

            // Check if type is selected
            if (ddlType.SelectedIndex != 0)
            {
                ddlType.BackColor = System.Drawing.Color.Empty;
            }
            else
            {
                ddlType.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            if (txtEmail.Text == "")
            {
                txtEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 0;
            }
            else
            {
                txtEmail.BackColor = System.Drawing.Color.Empty;
                if (txtEmail.Text != "")
                {
                    string emailID = txtEmail.Text.ToString();
                    string[] emailList = emailID.Split(',');
                    foreach (string email in emailList)
                    {
                        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        Match match = regex.Match(email);
                        if (match.Success)
                        {
                            count++;
                            txtEmail.BorderColor = System.Drawing.Color.Empty;
                        }
                        else
                        {
                            txtEmail.BorderColor = System.Drawing.Color.Red;
                            flag = 1;
                            txtEmail.Focus();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Enter valid email Address(s)!');", true);
                        }
                    }
                }
            }

            if (flag == 1)
            {
                int randomNumber1 = 0;
                int randomNumber2 = 0;
                Boolean Exists = true;

                // Generate unique code and reference number
                while (Exists)
                {
                    randomNumber1 = rand.Next(10000000, 100000000);
                    randomNumber2 = rand.Next(1000, 10000);
                    DataTable dt = _cm.CheckRefCode(randomNumber1, randomNumber2, Session["PreFix"].ToString());
                    if (dt.Rows.Count > 0)
                    {
                        Exists = true;
                    }
                    else
                    {
                        Exists = false;
                        dt = _cm.CheckCode(randomNumber2, Session["PreFix"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            Exists = true;
                        }
                        else
                        {
                            Exists = false;
                        }
                    }
                }

                // Set the generated values to textboxes
                txtReference.Text = randomNumber1.ToString();
                txtCode.Text = Session["PreFix"].ToString() + randomNumber2.ToString();
                btnGenerateReference.Enabled = false;
                btnSubmit.Enabled = true;

                if (Exists == false)
                {
                    // Set basic properties
                    _cm.Type = ddlType.SelectedItem.ToString();
                    _cm.Code = txtCode.Text;
                    _cm.Reference = txtReference.Text;
                    _cm.ContactEmail = txtEmail.Text;

                    // Handle query string ID if exists
                    if (Request.QueryString["ID"] != null)
                    {
                        _cm.inquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                        String Type = Request.QueryString["Type"].ToString();

                        DataTable dtID = _cm.getID(_cm);
                    }
                }
            }
        }
        private void BindGridView()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = @"SELECT [VerificationID], [Code], [Reference], [Type], [created] 
                                    FROM [LIMSPTQualis].[dbo].[Verification] 
                                    ORDER BY [created] DESC";
                        cmd.Connection = con;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        con.Open();
                        da.Fill(dt);
                        con.Close();

                        if (dt.Rows.Count > 0)
                        {
                            gvVerification.DataSource = dt;
                            gvVerification.DataBind();
                        }
                        else
                        {
                            gvVerification.DataSource = null;
                            gvVerification.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Add proper error handling
                ScriptManager.RegisterStartupScript(this, this.GetType(), "error",
                    "alert('Error loading verification data: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        // Add this method to handle paging
        protected void gvVerification_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVerification.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        //protected void btnGenerateReference_Click(object sender, EventArgs e)
        //{
        //    Random rand = new System.Random(); int flag = 1; int count = 0;
        //    if (ddlType.SelectedIndex != 0)
        //    {
        //        ddlType.BackColor = System.Drawing.Color.Empty;
        //    }
        //    else
        //    {
        //        ddlType.BackColor = System.Drawing.Color.LightPink;
        //        flag = 0;
        //    }
        //    if (txtEmail.Text == "")
        //    {
        //        txtEmail.BackColor = System.Drawing.Color.LightPink;
        //        flag = 0;
        //    }
        //    else
        //    {
        //        txtEmail.BackColor = System.Drawing.Color.Empty;
        //        if (txtEmail.Text != "")
        //        {
        //            string emailID = txtEmail.Text.ToString();
        //            string[] emailList = emailID.Split(',');
        //            foreach (string email in emailList)
        //            {
        //                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        //                Match match = regex.Match(email);
        //                if (match.Success)
        //                {
        //                    count++;
        //                    txtEmail.BorderColor = System.Drawing.Color.Empty;
        //                }
        //                else
        //                {
        //                    txtEmail.BorderColor = System.Drawing.Color.Red;
        //                    flag = 1;
        //                    txtEmail.Focus();
        //                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Enter valid email Address(s)!');", true);
        //                }
        //            }
        //        }
        //    }
        //    if (flag == 1)
        //    {
        //        int randomNumber1 = 0; int randomNumber2 = 0; Boolean Exists = true; string type = "";

        //        while (Exists)
        //        {
        //            randomNumber1 = rand.Next(10000000, 100000000);
        //            randomNumber2 = rand.Next(1000, 10000);
        //            DataTable dt = _cm.CheckRefCode(randomNumber1, randomNumber2, Session["PreFix"].ToString());
        //            if (dt.Rows.Count > 0)
        //            {
        //                Exists = true;
        //            }
        //            else
        //            {
        //                Exists = false;
        //                dt = _cm.CheckCode(randomNumber2, Session["PreFix"].ToString());
        //                if (dt.Rows.Count > 0)
        //                {
        //                    Exists = true;
        //                }
        //                else
        //                {
        //                    Exists = false;
        //                }
        //            }
        //        }
        //        txtReference.Text = randomNumber1.ToString();
        //        txtCode.Text = Session["PreFix"].ToString() + randomNumber2.ToString();
        //        btnGenerateReference.Enabled = false;
        //        btnSubmit.Enabled = true;
        //        if (Exists == false)
        //        {
        //            _cm.Type = ddlType.SelectedItem.ToString();
        //            _cm.Code = txtCode.Text;
        //            _cm.Reference = txtReference.Text;
        //            _cm.ContactEmail = txtEmail.Text;

        //            //int VerID = _cm.AddVerificationReferenceCode(_cm);

        //            //Session["verificationid"] = VerID;

        //            if (Session["ReferenceCallID"] != null)
        //            {
        //                _cm.CallID = Session["ReferenceCallID"].ToString();
        //                _cm.CustomerCode = txtCode.Text;
        //                _cm.updateCallLog(_cm);
        //            }


        //            if (Session["ReferenceAgentID"] != null)
        //            {
        //                _cm.AgentID = Session["ReferenceAgentID"].ToString();
        //                _cm.CustomerCode = txtCode.Text;
        //                _cm.updateAgentDetails(_cm);
        //            }

        //            if (Request.QueryString["ID"] != null)
        //            {
        //                _cm.EnquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());
        //                String Type = Request.QueryString["Type"].ToString();

        //                DataTable dtID = _cm.getID(_cm);
        //                if (dtID.Rows.Count > 0)
        //                {
        //                    //_cm.Type = dtID.Rows[0]["Type"].ToString();
        //                    //string EnqType = _cm.Type;
        //                }

        //                if (Type == "CALLLOG")
        //                {
        //                    ddlType.SelectedValue = "Customer";
        //                    _cm.CallID = dtID.Rows[0]["CustomerID"].ToString();
        //                    _cm.CustomerCode = txtCode.Text;
        //                    _cm.updateCallLog(_cm);
        //                }
        //                if (Type == "Agent")
        //                {
        //                    ddlType.SelectedValue = "Customer";
        //                    _cm.AgentID = dtID.Rows[0]["AgentID"].ToString();
        //                    _cm.CustomerCode = txtCode.Text;
        //                    _cm.updateAgentDetails(_cm);
        //                }

        //            }
        //        }
        //    }
        //}


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            _cm.Type = ddlType.SelectedItem.ToString();
            _cm.Code = txtCode.Text;
            _cm.Reference = txtReference.Text;
            _cm.ContactEmail = txtEmail.Text;
            _cm.AddVerificationReferenceCode(_cm);

            //Send Mail
            string loginLink = "http://ptqualis.rnisolutions.in:4545/Login.aspx";
            string filePath = Session["Link"].ToString() + "?ID=" + txtReference.Text;

            // string filePath = filename;
            StringBuilder sbLink = new StringBuilder();
            sbLink.Append("<a href='" + loginLink + "'>Login Here</a><br/>");
            sbLink.Append("<a href='" + filePath + "'>Access Details</a>");
            string Subject = "";
            string FromAddress = "";
            string ToAddress = "";
            string CCAddress = "";
            string BCCAddress = "";
            DataTable dtEmailDetails = _cm.getEmaildetails("Generate " + _cm.Type + " Code");
            if (dtEmailDetails.Rows.Count > 0)
            {
                if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                {
                    Subject = dtEmailDetails.Rows[0]["EmailSubject"].ToString();
                }
                if (dtEmailDetails.Rows[0]["FromAddress"].ToString() != "")
                {
                    FromAddress = dtEmailDetails.Rows[0]["FromAddress"].ToString();
                }
                if (dtEmailDetails.Rows[0]["ToAddress"].ToString() != "")
                {
                    ToAddress += dtEmailDetails.Rows[0]["ToAddress"].ToString();
                }
                if (dtEmailDetails.Rows[0]["CCAddress"].ToString() != "")
                {
                    CCAddress = dtEmailDetails.Rows[0]["CCAddress"].ToString();
                }
                if (dtEmailDetails.Rows[0]["BCCAddress"].ToString() != "")
                {
                    BCCAddress = dtEmailDetails.Rows[0]["BCCAddress"].ToString();
                }
                int UserID = Convert.ToInt32(Session["UserID"].ToString());
                DataTable dt13 = _cm.getUserDetails(UserID);
                int EmployeeID = Convert.ToInt32(dt13.Rows[0]["EmployeeID"].ToString());
                DataTable dt14 = _cm.GetEmployeeDetails(EmployeeID);
                string PurchaseEmail = dt14.Rows[0]["Email"].ToString();
                string BodyVendor = objEmail.PrepareMailBodyWith("EmailVerification.html", "{Type}", _cm.Type, "{Email}", _cm.ContactEmail, "{Code}", _cm.Code, "{Reference}", _cm.Reference, "{Link}", sbLink.ToString());
                string BodyFarmson = objEmail.PrepareMailBodyWith("EmailVerificationSent.html", "{Type}", _cm.Type, "{Email}", _cm.ContactEmail, "{Code}", _cm.Code, "{Reference}", _cm.Reference, "{Link}", sbLink.ToString());

                _cm.FromAddress = FromAddress;
                _cm.ToAddress = _cm.ContactEmail;
                _cm.CCAddress = CCAddress;
                _cm.BCCAddress = BCCAddress + "," + PurchaseEmail;
                _cm.Subject = Subject;
                _cm.Body = BodyVendor;
                _cm.Purpose = ddlType.SelectedItem.ToString() + " Code sent for " + ddlType.SelectedItem.ToString() + " Verification";
                _cm.FileName = "EmailVerification.html";
                _cm.SentTo = _cm.ContactEmail;

                try
                {
                    Session["Failed"] = "0";


                    objEmail.SendMail(FromAddress, _cm.ContactEmail, _cm.CCAddress, _cm.BCCAddress, Subject, BodyVendor);
                    objEmail.SendMail(FromAddress, _cm.BCCAddress, Subject, BodyFarmson);

                    if (Session["Failed"].ToString() == "0")
                    {
                        _cm.SentStatusforcode = true;
                        _cm.AddMailBoxDetails(_cm);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                    }
                    else
                    {
                        _cm.SentStatusforcode = false;
                        _cm.AddMailBoxDetails(_cm);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed. Try again Later!');", true);
                    string ErrorMessage = ex.Message;
                    string Source = ex.Source;
                    string StackTrace = ex.StackTrace;


                    Session["ErrorMessage"] = "[Message]:" + ErrorMessage + ",[Source]:" + Source + ",[StackTrace]:" + StackTrace;
                    Response.Redirect("Error.aspx");
                }

            }
            ClearAll();
        }
        protected void ClearAll()
        {
            ddlType.SelectedIndex = 0;
            txtCode.Text = "";
            txtReference.Text = "";
            btnSubmit.Enabled = false;
            btnGenerateReference.Enabled = true;
            txtEmail.Text = "";
        }
        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        con.Open();
        //        int prevIdentity = _cm.GetReseedIdentity("Verification", "VerificationID");
        //        _cm.ReseedIdentity("Verification", prevIdentity);
        //        con.Close();

        //        string query = @"insert into Verification (Code,Reference,Type,Created,Action) OUTPUT INSERTED.VerificationID values (@Code,@Reference,@Type,@Created,@action)";

        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Code", txtCode.Text);
        //            cmd.Parameters.AddWithValue("@Reference", txtReference.Text);
        //            cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Value);
        //            cmd.Parameters.AddWithValue("@Created", DateTime.Now);
        //            cmd.Parameters.AddWithValue("@action", 0);
        //            con.Open();
        //            int InsertGetId = (int)cmd.ExecuteScalar();
        //            con.Close();

        //            if (InsertGetId > 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Added successfully');", true);
        //                ClearAll();
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Something went wrong, try later!');", true);

        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Save: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");

        //    }






        //}
    }
}