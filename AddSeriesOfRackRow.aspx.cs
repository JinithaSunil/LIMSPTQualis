using DocumentFormat.OpenXml.Office2019.Presentation;
using LIMS.Models;
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

namespace LIMS
{
    public partial class AddSeriesOfRackRow : System.Web.UI.Page
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
                    RackSerialDataDisplay();

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
                    }
                    else
                    {
                        btnAdd.Visible = true;
                        btnUpdate.Visible = false;

                        List<SerialNumberMasterData> response2 = dal.AlphaSerialNumberDisplay();
                        ddlAlphabet.DataSource = response2;
                        ddlAlphabet.DataTextField = "SerialNumber";
                        ddlAlphabet.DataValueField = "SerialNumberId";
                        ddlAlphabet.DataBind();
                        ddlAlphabet.Items.Insert(0, new ListItem("Select", ""));


                        List<RackTypeDetailsData> RackTypeGet = dal.AllRackTypeDetailsDisplay();
                        //List<RackTypeDetailsData> filteredRackTypeGet = RackTypeGet.Where(item => item.RackCompanyUnitId == Convert.ToInt32(ddlBranchUnit.SelectedItem.Value)).ToList();
                        ddlRackType.DataSource = RackTypeGet;
                        ddlRackType.DataTextField = "RackType";
                        ddlRackType.DataValueField = "RackTypeId";
                        ddlRackType.DataBind();
                        ddlRackType.Items.Insert(0, new ListItem("Select", ""));

                        //List<CompanyUnitData> GetData = dal.ActiveCompanyUnitDataGet();
                        //ddlBranchUnit.DataSource = GetData;
                        //ddlBranchUnit.DataTextField = "UnitName";
                        //ddlBranchUnit.DataValueField = "CompanyUnitId";
                        //ddlBranchUnit.DataBind();
                        //ddlBranchUnit.Items.Insert(0, new ListItem("Select Unit", ""));

                        //if (roleId != "1")
                        //{
                        //    ListItem itemToSelect = ddlBranchUnit.Items.FindByValue(Session["CompanyUnitId"].ToString());
                        //    if (itemToSelect != null)
                        //    {
                        //        ddlBranchUnit.Enabled = false;
                        //        itemToSelect.Selected = true;
                        //        ddlBranchUnit_SelectedIndexChanged(sender, e);
                        //    }
                        //}
                        //else
                        //{
                        //    ddlBranchUnit.Enabled = true;
                        //}
                    }

                }
            }
        }

        protected void RackSerialDataDisplay()
        {
            List<RackNumberData> response = dal.AllTotalRackRowSerialNumberDisplay();
            if (response != null && response.Count > 0)
            {
                RepShowData.DataSource = response;
                RepShowData.DataBind();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Not Found.'); </script>");
            }


        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtRack.Text))
                {
                    //int branchUnitId = Convert.ToInt32(ddlBranchUnit.SelectedItem.Value);
                    int rackTypeId = Convert.ToInt32(ddlRackType.SelectedItem.Value);
                    int rackSerialNumberId = Convert.ToInt32(ddlAlphabet.SelectedItem.Value);

                    bool IsCheckRackUnitAndSerialWiseExists = dal.CheckRackUnitAndSerialWiseExists(rackTypeId, rackSerialNumberId);

                    if (!IsCheckRackUnitAndSerialWiseExists)
                    {
                        int numberOfEntries = Convert.ToInt32(txtRack.Text);
                        bool success = false;

                        for (int i = 0; i < numberOfEntries; i++)
                        {
                            int rowNumber = i + 1;
                            string rackSerialNumber = ddlAlphabet.SelectedItem.Text + "-" + rowNumber;
                            success = SaveToDatabase(rackTypeId, rackSerialNumber);
                        }

                        if (success)
                        {
                            //Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessMessage", "<script> alert('Add Successfully'); setTimeout(function() { window.location = 'AddSeriesOfRackRow.aspx'; }, 100); </script>");
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessMessage", "<script> alert('Add Successfully'); </script>");
                            RackSerialDataDisplay();
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "FailureMessage", "<script> alert('Something went wrong, try later!'); </script>");
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "FailureMessage", "<script> alert('Duplicate Serial Numbers are not allowed in Unit!'); </script>");
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "FailureMessage", "<script> alert('Blank not Allowed'); </script>");
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

        private bool SaveToDatabase(int rackTypeId, string rackSerialNumber)
        {
            try
            {
                con.Open();
                int prevIdentity = dal.GetReseedIdentity("RackSerialDetails", "RackSerialId");
                dal.ReseedIdentity("RackSerialDetails", prevIdentity);
                con.Close();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = @"INSERT INTO RackSerialDetails 
                                    (RackAlphSeries,RackSerialNumber,SerialNumberId,TotalSerialNumber,RackTypeId,
                                     CreatedEmployeeId,MasterCompanyId) 
                                    Output INSERTED.RackSerialId values 
                                    (@RackAlphSeries, @RackSerialNumber, @SerialNumberId, @TotalSerialNumber, @RackTypeId, 
                                     @CreatedEmployeeId, @MasterCompanyId)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@RackAlphSeries", ddlAlphabet.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@RackSerialNumber", rackSerialNumber);
                        cmd.Parameters.AddWithValue("@SerialNumberId", ddlAlphabet.SelectedItem.Value);
                        cmd.Parameters.AddWithValue("@TotalSerialNumber", txtRack.Text);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        //cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@CreatedEmployeeId", Session["EmployeeID"].ToString());
                        cmd.Parameters.AddWithValue("@MasterCompanyId", Session["MasterCompanyId"].ToString());

                        con.Open();
                        int GetID = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (GetID > 0)
                        {
                            var TableRowId = GetID.ToString();
                            bool success = dal.AuditLogData("Add Series Of RackRow", "RackSerialDetails", TableRowId, "");
                            if (success)
                            { }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }
                            return true;
                        }
                    }
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
            return false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddRack.aspx");
        }

        protected void ddlBranchUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //List<RackTypeDetailsData> RackTypeGet = dal.AllRackTypeDetailsDisplay();
            ////List<RackTypeDetailsData> filteredRackTypeGet = RackTypeGet.Where(item => item.RackCompanyUnitId == Convert.ToInt32(ddlBranchUnit.SelectedItem.Value)).ToList();
            //ddlRackType.DataSource = RackTypeGet;
            //ddlRackType.DataTextField = "RackType";
            //ddlRackType.DataValueField = "RackTypeId";
            //ddlRackType.DataBind();
            //ddlRackType.Items.Insert(0, new ListItem("Select", ""));
        }


    }
}