using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;

namespace LIMS
{
    public partial class RackTypeDetails : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");

        DataAccessLayer dal = new DataAccessLayer();
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["EmailId"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
            }
            else
            {
                if (!IsPostBack)
                {
                    string roleId = Session["RoleId"].ToString();
                    string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                    string pageName = System.IO.Path.GetFileName(currentPageUrl);
                    string urlToCheck = pageName;


                    //List<CompanyUnitData> GetData = dal.ActiveCompanyUnitDataGet();
                    //ddlBranchUnit.DataSource = GetData;
                    //ddlBranchUnit.DataTextField = "UnitName";
                    //ddlBranchUnit.DataValueField = "CompanyUnitId";
                    //ddlBranchUnit.DataBind();
                    //ddlBranchUnit.Items.Insert(0, new ListItem("Select Unit", ""));

                   // string sessiondata = Session["CompanyUnitId"].ToString();


                    if (Request.QueryString["PageID"] != null)
                    {
                        string pageID = Request.QueryString["PageID"];
                        string encodedString = pageID;
                        byte[] decodedBytes = Convert.FromBase64String(encodedString);
                        string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                        lblPageID.Text = decodedEventListId;
                        lblPageID.Visible = false;

                        btnAdd.Visible = false;
                        btnUpdate.Visible = true;

                        //if (roleId != "1")
                        //{
                        //    ddlBranchUnit.Enabled = false;
                        //}
                        //else
                        //{
                        //    ddlBranchUnit.Enabled = true;
                        //}

                        List<RackTypeDetailsData> response = dal.AllRackTypeDetailsDisplay();
                        List<RackTypeDetailsData> filteredData = response.Where(item => item.RackTypeId == Convert.ToInt32(lblPageID.Text)).ToList();

                        if (filteredData != null && filteredData.Count > 0)
                        {
                            RackTypeDetailsData selectedData = filteredData[0];
                            txtRackType.Text = selectedData.RackType.ToString();
                            //ddlBranchUnit.SelectedValue = selectedData.RackCompanyUnitId.ToString();

                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Found, Add Inventory First'); etTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100); </script>");
                        }

                        List<RackTypeDetailsData> filteredRackTypeGet = dal.AllRackTypeDetailsDisplay();
                        //List<RackTypeDetailsData> filteredRackTypeGet = RackTypeGet.Where(item => item.RackCompanyUnitId == Convert.ToInt32(ddlBranchUnit.SelectedItem.Value)).ToList();

                        if (filteredRackTypeGet != null && filteredRackTypeGet.Count > 0)
                        {
                            RepShowData.DataSource = filteredRackTypeGet;
                            RepShowData.DataBind();
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Found, Add Inventory First'); etTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100); </script>");
                        }

                    }
                    else
                    {
                        btnAdd.Visible = true;
                        btnUpdate.Visible = false;

                        //if (roleId != "1")
                        //{
                        //    ListItem itemToSelect = ddlBranchUnit.Items.FindByValue(Session["CompanyUnitId"].ToString());
                        //    if (itemToSelect != null)
                        //    {
                        //        ddlBranchUnit.Enabled = false;
                        //        itemToSelect.Selected = true;
                        //    }
                        //}
                        //else
                        //{
                        //    ListItem itemToSelect = ddlBranchUnit.Items.FindByValue(Session["CompanyUnitId"].ToString());
                        //    if (itemToSelect != null)
                        //    {
                        //        ddlBranchUnit.Enabled = true;
                        //        itemToSelect.Selected = true;
                        //    }
                        //    //ddlBranchUnit.Enabled = true;
                        //}
                        //string RCompanyUnitId = ddlBranchUnit.SelectedItem.Value;
                        List<RackTypeDetailsData> RackTypeGet = dal.AllRackTypeDetailsDisplay();

                        if (RackTypeGet != null && RackTypeGet.Count > 0)
                        {
                            //List<RackTypeDetailsData> filteredRackTypeGet = RackTypeGet.Where(item => item.RackCompanyUnitId == Convert.ToInt32(RCompanyUnitId)).ToList();

                            RepShowData.DataSource = RackTypeGet;
                            RepShowData.DataBind();
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Found, Add Inventory First'); etTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100); </script>");
                        }

                    }

                }

            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("RackTypeDetails.aspx");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bool IsRackNameExists = dal.IsRackNameExists(txtRackType.Text);

                if (!IsRackNameExists)
                {
                    con.Open();
                    int prevIdentity = dal.GetReseedIdentity("RackTypeDetails", "RackTypeId");
                    dal.ReseedIdentity("RackTypeDetails", prevIdentity);
                    con.Close();

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string query = @"INSERT INTO RackTypeDetails 
                                      (RackType, RackMasterCompanyId, RackCreatedEmployeeId, RackCreatedDateTime) 
                                     Output INSERTED.RackTypeId values 
                                      (@RackType, @RackMasterCompanyId, @RackCreatedEmployeeId, @RackCreatedDateTime)";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@RackType", txtRackType.Text);
                            //cmd.Parameters.AddWithValue("@RackCompanyUnitId", ddlBranchUnit.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@RackMasterCompanyId", Session["MasterCompanyId"].ToString());
                            cmd.Parameters.AddWithValue("@RackCreatedEmployeeId", Session["EmployeeID"].ToString());
                            cmd.Parameters.AddWithValue("@RackCreatedDateTime", GetDateTime);

                            con.Open();
                            int GetID = (int)cmd.ExecuteScalar();
                            con.Close();

                            if (GetID > 0)
                            {
                                var TableRowId = GetID.ToString();
                                bool success = dal.AuditLogData("Add Rack Name/Type", "RackTypeDetails", TableRowId, "");
                                if (success) { }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                }
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Add Successfully '); window.location.href='RackTypeDetails.aspx'; </script>");
                            }

                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' All Ready Exist '); window.location.href='RackTypeDetails.aspx'; </script>");
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);
                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Rack Type/Name is already Existed!'); </script>");
                }
            }

        }

        protected void RepShowData_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemEdit")
            {
                string GetID = e.CommandArgument.ToString();
                string originalString = GetID;
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(originalString);
                string encodedEventID = Convert.ToBase64String(bytesToEncode);
                string url = $"RackTypeDetails.aspx?PageID={encodedEventID}";
                Response.Redirect(url);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                bool IsRackNameExists = dal.IsRackNameExists(txtRackType.Text);

                if (!IsRackNameExists)
                {
                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string insertQuery = @"Update RackTypeDetails SET RackType = @RackType, RackUpdatedDateTime = @RackUpdatedDateTime WHERE RackTypeId = @RackTypeId";
                        SqlCommand cmd = new SqlCommand(insertQuery, con);
                        cmd.Parameters.AddWithValue("@RackTypeId", lblPageID.Text);
                        cmd.Parameters.AddWithValue("@RackType", txtRackType.Text);
                        cmd.Parameters.AddWithValue("@RackUpdatedDateTime", date + " " + time);
                        con.Open();
                        int GetID = cmd.ExecuteNonQuery();
                        con.Close();

                        if (GetID > 0)
                        {
                            var TableRowId = lblPageID.Text;
                            bool success = dal.AuditLogData("Rack Type Updated", "RackTypeDetails", TableRowId, "");
                            if (success) { }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }
                            Response.Redirect(Request.RawUrl);
                            //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' Update Successfully '); window.location.href='CompanyAddRoleName.aspx'; </script>");
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Rack Type/Name is already Existed!'); </script>");
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);
                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }

        }


    }
}