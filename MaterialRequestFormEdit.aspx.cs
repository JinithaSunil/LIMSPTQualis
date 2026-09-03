using DocumentFormat.OpenXml.Office.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public partial class MaterialRequestFormEdit : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
        private int Year;
        DataAccessLayer dal = new DataAccessLayer();

        private DataTable DataTable
        {
            get
            {
                if (ViewState["DataTable"] == null)
                {
                    // Create a new DataTable if it doesn't exist in ViewState
                    DataTable dt = new DataTable();
                    dt.Columns.Add("MR_ItemCode");
                    dt.Columns.Add("MR_ItemName");
                    dt.Columns.Add("MR_CurrentStock");
                    dt.Columns.Add("MR_MinStock");
                    dt.Columns.Add("MR_UOM");
                    dt.Columns.Add("MR_EmployeeRequireItemQty");
                    dt.Columns.Add("MR_RequireDate");
                    ViewState["DataTable"] = dt;
                }
                return ViewState["DataTable"] as DataTable;
            }
            set
            {
                ViewState["DataTable"] = value;
            }
        }

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
                    try
                    {
                        string roleId = Session["RoleId"].ToString();
                        string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                        string pageName = System.IO.Path.GetFileName(currentPageUrl);
                        string urlToCheck = pageName;

                        ddlUOM.Enabled = false;

                        List<CategoryNameData> res2 = dal.AllCategoryNameDisplayForDropDown();
                        ddlCategory.DataSource = res2;
                        ddlCategory.DataTextField = "CategoryName";
                        ddlCategory.DataValueField = "CategoryId";
                        ddlCategory.DataBind();
                        ddlCategory.Items.Insert(0, new ListItem("Select", ""));

                        List<UOMData> uomData = dal.UOMDetailsGet();
                        ddlUOM.DataSource = uomData;
                        ddlUOM.DataTextField = "UOMCode";
                        ddlUOM.DataValueField = "UOMId";
                        ddlUOM.DataBind();
                        ddlUOM.Items.Insert(0, new ListItem("Select", ""));

                        if (roleId != "1")
                        {
                            int EmpId = Convert.ToInt32(Session["UserID"].ToString());

                            //List<AllEmployeeDetailsData> responseGet = dal.ActiveCompanyEmployeeDetailsDataGet();
                            //List<AllEmployeeDetailsData> filteredData = responseGet.Where(item => item.EmployeeId == EmpId).ToList();

                            //if (filteredData != null && filteredData.Count > 0)
                            //{
                            //    AllEmployeeDetailsData selectedItem = filteredData[0];
                            //    int HODEmployeeIDGet = Convert.ToInt32(selectedItem.HODEmployeeId.ToString());
                            //    ViewState["HODEmployeeId"] = HODEmployeeIDGet;
                            //}
                            //else
                            //{
                            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                            //}
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Company Admin can not access this'); setTimeout(function() { window.location = 'Dashboard.aspx'; }, 100); </script>");
                        }

                        if (Request.QueryString["PageID"] != null)
                        {
                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                            int employeeId = Convert.ToInt32(Session["UserID"]);

                            List<MaterialRequestData> response = dal.MaterialRequestGetData();
                            List<MaterialRequestData> res = response.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text)).ToList();
                            if (res != null && res.Count > 0)
                            {
                                MaterialRequestData selectedItem = res[0];
                                lblMRNumber.Text = selectedItem.MRM_NUMBER.ToString();
                                lblMRMID.Text = selectedItem.MRM_ID.ToString();
                                DataTable.Rows.Clear();
                                /* AddRowToDataTable(selectedItem); */
                                foreach (MaterialRequestData data in res)
                                {
                                    AddRowToDataTable(data);
                                }
                                BindGridView();
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something wrong, PageID Not Getting!!'); setTimeout(function() { window.location = 'MaterialRequestShow.aspx'; }, 100); </script>");
                        }
                        InitializeDataTable();
                        BindGridView();
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

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int getItemCategoryId = Convert.ToInt32(ddlCategory.SelectedItem.Value);

            List<InventoryItemData> res = dal.ItemNameForDropDown();
            List<InventoryItemData> filteredData = res.Where(item => item.ItemTypeID == getItemCategoryId).ToList();
            ddlItemCode.DataSource = filteredData;
            ddlItemCode.DataTextField = "ItemCode";
            ddlItemCode.DataValueField = "ItemId";
            ddlItemCode.DataBind();
            ddlItemCode.Items.Insert(0, new ListItem("Select", ""));

        }

        protected void ddlItemCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int getItemId = Convert.ToInt32(ddlItemCode.SelectedItem.Value);
            List<InventoryItemData> res = dal.ActiveInventoryDetailsGet();
            List<InventoryItemData> filteredData = res.Where(item => item.InventoryItemId == getItemId).ToList();

            if (filteredData != null && filteredData.Count > 0)
            {
                InventoryItemData selectedItem = filteredData[0];
                txtItemName.Text = selectedItem.ItemName.ToString();
                ddlUOM.SelectedValue = selectedItem.UOMId.ToString();
                txtCurrentStock.Text = selectedItem.InventoryCurrentStock.ToString();
                txtMinStock.Text = selectedItem.InventoryMinStock.ToString();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
            }
        }

        private void InitializeDataTable()
        {   /* Check if the DataTable already exists in ViewState */
            if (ViewState["DataTable"] == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("MR_ItemCode");
                dt.Columns.Add("MR_ItemName");
                dt.Columns.Add("MR_CurrentStock");
                dt.Columns.Add("MR_MinStock");
                dt.Columns.Add("MR_UOM");
                dt.Columns.Add("MR_EmployeeRequireItemQty");
                dt.Columns.Add("MR_RequireDate");
                ViewState["DataTable"] = dt;
            }
            else
            {
                DataTable = ViewState["DataTable"] as DataTable;
            }
        }

        private void AddRowToDataTable(MaterialRequestData data)
        {
            DataRow newRow = DataTable.NewRow();
            newRow["MR_ItemCode"] = data.MR_ItemCode;
            newRow["MR_ItemName"] = data.MR_ItemName;
            newRow["MR_CurrentStock"] = data.MR_CurrentStock;
            newRow["MR_MinStock"] = data.MR_MinStock;
            newRow["MR_UOM"] = data.MR_UOM;
            newRow["MR_EmployeeRequireItemQty"] = data.MR_EmployeeRequireItemQty;
            newRow["MR_RequireDate"] = data.MR_RequireDate;
            DataTable.Rows.Add(newRow);
        }

        private void BindGridView()
        {
            GridView1.DataSource = DataTable;
            GridView1.DataBind();

            btnSubmit.Enabled = DataTable != null && DataTable.Rows.Count > 0;
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TextBox txtRequireQty = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtRequireQty");
            string requireQty = txtRequireQty.Text;

            TextBox txtRequireDate = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtRequireDate");
            string requireDate = txtRequireDate.Text;

            try
            {
                string itemCode = GridView1.DataKeys[e.RowIndex]["MR_ItemCode"].ToString();
                string Query = @"UPDATE MaterialRequestDetails SET 
                                  MR_EmployeeRequireItemQty = @MR_EmployeeRequireItemQty, 
                                  MR_BalanceQty = @MR_BalanceQty, MR_RequireDate = @MR_RequireDate,MR_StatusId=@MR_StatusId
                                 WHERE MR_NUMBER = @MR_NUMBER AND MR_ItemCode = @MR_ItemCode";

                using (SqlCommand cmd1 = new SqlCommand(Query, con))
                {
                    cmd1.Parameters.AddWithValue("@MR_MRMID", lblMRMID.Text);
                    cmd1.Parameters.AddWithValue("@MR_NUMBER", lblMRNumber.Text);
                    cmd1.Parameters.AddWithValue("@MR_ItemCode", itemCode);
                    cmd1.Parameters.AddWithValue("@MR_EmployeeRequireItemQty", txtRequireQty.Text);
                    cmd1.Parameters.AddWithValue("@MR_BalanceQty", txtRequireQty.Text);
                    cmd1.Parameters.AddWithValue("@MR_RequireDate", txtRequireDate.Text);
                    cmd1.Parameters.AddWithValue("@MR_StatusId", 35);

                    con.Open();
                    int I = cmd1.ExecuteNonQuery();
                    con.Close();

                    if (I > 0)
                    {
                        DataRow row = DataTable.Rows[e.RowIndex];
                        row["MR_EmployeeRequireItemQty"] = requireQty;
                        row["MR_RequireDate"] = requireDate;

                        GridView1.EditIndex = -1;
                        /* BindGridView(); */

                        var TableRowId = lblMRMID.Text;
                        /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                        bool success = dal.AuditLogData("Material Request Update", "MaterialRequestDetails", TableRowId, "");
                        if (success)
                        { }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                        }

                        Response.Redirect(Request.RawUrl);
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
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


        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string itemCodeToDelete = GridView1.DataKeys[e.RowIndex]["MR_ItemCode"].ToString();
                string str = "DELETE FROM MaterialRequestDetails WHERE MR_MRMID = @MR_MRMID AND MR_ItemCode = @MR_ItemCode";
                SqlCommand cmd = new SqlCommand(str, con);
                cmd.Parameters.AddWithValue("@MR_MRMID", lblMRMID.Text);
                cmd.Parameters.AddWithValue("@MR_ItemCode", itemCodeToDelete);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    DataTable.Rows[e.RowIndex].Delete();
                    /* BindGridView(); */
                    var TableRowId = lblMRMID.Text;
                    /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                    bool success = dal.AuditLogData("Material Request Delete", "MaterialRequestDetails", TableRowId, "");
                    if (success)
                    { }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                    }

                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong!.'); </script>");
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

        private void ClearInputFields()
        {
            ddlItemCode.SelectedIndex = 0;
            txtItemName.Text = string.Empty;
            txtCurrentStock.Text = string.Empty;
            txtMinStock.Text = string.Empty;
            ddlUOM.SelectedIndex = 0;
            txtRequireQty.Text = string.Empty;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            string itemCode = ddlItemCode.SelectedItem?.Text;
            string itemName = txtItemName.Text;
            string currentStock = txtCurrentStock.Text;
            string minStock = txtMinStock.Text;
            string uom = ddlUOM.SelectedItem?.Text;  // Use null conditional operator to handle null case
            string requireQty = txtRequireQty.Text;
            string requiredDate = txtRequiredDate.Text;

            if (string.IsNullOrWhiteSpace(itemCode) || string.IsNullOrWhiteSpace(itemName) ||
                string.IsNullOrWhiteSpace(currentStock) || string.IsNullOrWhiteSpace(minStock) ||
                string.IsNullOrWhiteSpace(uom) || string.IsNullOrWhiteSpace(requireQty) ||
                string.IsNullOrWhiteSpace(requiredDate))
            {
                // Display alert using JavaScript
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please fill in all fields.');", true);
                return;
            }
            else
            {
                DataRow newRow = DataTable.NewRow();
                newRow["MR_ItemCode"] = itemCode;
                newRow["MR_ItemName"] = itemName;
                newRow["MR_CurrentStock"] = currentStock;
                newRow["MR_MinStock"] = minStock;
                newRow["MR_UOM"] = uom;
                newRow["MR_EmployeeRequireItemQty"] = requireQty;
                newRow["MR_RequireDate"] = requiredDate;
                DataTable.Rows.Add(newRow);

                BindGridView();
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                bool s = false;

                using (SqlConnection connection = new SqlConnection(constr))
                {
                    string str1 = @"UPDATE MaterialRequestMaster set MRM_StatusId = @MRM_StatusId, MRM_UpdatedDateTime = @MRM_UpdatedDateTime
                                    WHERE MRM_ID = @MRM_ID";

                    SqlCommand cmd = new SqlCommand(str1, con);

                    cmd.Parameters.AddWithValue("@MRM_ID", lblPageID.Text);
                    cmd.Parameters.AddWithValue("@MRM_StatusId", 35);
                    cmd.Parameters.AddWithValue("@MRM_UpdatedDateTime", GetDateTime);

                    con.Open();
                    int j = cmd.ExecuteNonQuery();
                    con.Close();

                    if (j > 0)
                    {
                        var TableRowId = lblPageID.Text;
                        bool success = dal.AuditLogData("Material Request Status Update for Approval", "MaterialRequestMaster", TableRowId, "");
                        if (success)
                        { }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                        }
                    }

                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        string itemCode = row.Cells[1].Text;
                        string itemName = row.Cells[2].Text;
                        decimal currentStock = Convert.ToDecimal(row.Cells[3].Text);
                        decimal minimumStock = Convert.ToDecimal(row.Cells[4].Text);
                        string UOMName = row.Cells[5].Text;
                        Label requiredStocks = row.FindControl("lblRequireQty") as Label;
                        decimal EmpolyeeRequiredStock = Convert.ToDecimal(requiredStocks.Text);
                        Label requiredDAte = row.FindControl("lblRequireDate") as Label;
                        string EmpolyeeRequireDate = requiredDAte.Text;


                        bool isMRExist = dal.IsMaterialRequestDetailsExist(lblMRNumber.Text, itemCode);

                        if (isMRExist)
                        {
                            string Query = @"UPDATE MaterialRequestDetails SET 
                                              MR_EmployeeRequireItemQty = @MR_EmployeeRequireItemQty,
                                              MR_BalanceQty = @MR_BalanceQty,
                                              MR_RequireDate = @MR_RequireDate
                                             WHERE MR_NUMBER = @MR_NUMBER AND MR_ItemCode = @MR_ItemCode";

                            using (SqlCommand cmd1 = new SqlCommand(Query, con))
                            {
                                cmd1.Parameters.AddWithValue("@MR_MRMID", lblMRMID.Text);
                                cmd1.Parameters.AddWithValue("@MR_NUMBER", lblMRNumber.Text);
                                cmd1.Parameters.AddWithValue("@MR_ItemCode", itemCode);
                                cmd1.Parameters.AddWithValue("@MR_EmployeeRequireItemQty", EmpolyeeRequiredStock);
                                cmd1.Parameters.AddWithValue("@MR_BalanceQty", EmpolyeeRequiredStock);
                                cmd1.Parameters.AddWithValue("@MR_RequireDate", EmpolyeeRequireDate);

                                con.Open();
                                int I = cmd1.ExecuteNonQuery();
                                con.Close();

                                if (I > 0)
                                {
                                    var TableRowId = lblMRMID.Text;
                                    /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                                    bool success = dal.AuditLogData("Material Request Update", "MaterialRequestDetails", TableRowId, "");
                                    if (success)
                                    { }
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                    }

                                    s = true;
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                                }
                            }
                        }
                        else
                        {
                            con.Open();
                            int prevIdentity1 = dal.GetReseedIdentity("MaterialRequestDetails", "MR_ID");
                            dal.ReseedIdentity("MaterialRequestDetails", prevIdentity1);
                            con.Close();

                            string Query = @"INSERT INTO MaterialRequestDetails
                                                    (MR_StatusId, MR_MRMID, MR_NUMBER, MR_ItemCode, MR_ItemName, MR_UOM, MR_CurrentStock, MR_MinStock, 
                                                     MR_EmployeeRequireItemQty,MR_ReceiveQty,MR_BalanceQty,MR_RequireDate) 
                                                    output INSERTED.MR_ID values
                                                    (@MR_StatusId, @MR_MRMID, @MR_NUMBER, @MR_ItemCode, @MR_ItemName, @MR_UOM, @MR_CurrentStock, @MR_MinStock, 
                                                     @MR_EmployeeRequireItemQty,@MR_ReceiveQty,@MR_BalanceQty,@MR_RequireDate)";

                            using (SqlCommand cmd1 = new SqlCommand(Query, con))
                            {
                                cmd1.Parameters.AddWithValue("@MR_StatusId", 35);
                                cmd1.Parameters.AddWithValue("@MR_MRMID", lblMRMID.Text);
                                cmd1.Parameters.AddWithValue("@MR_NUMBER", lblMRNumber.Text);
                                cmd1.Parameters.AddWithValue("@MR_ItemCode", itemCode);
                                cmd1.Parameters.AddWithValue("@MR_ItemName", itemName);
                                cmd1.Parameters.AddWithValue("@MR_UOM", UOMName);
                                cmd1.Parameters.AddWithValue("@MR_CurrentStock", currentStock);
                                cmd1.Parameters.AddWithValue("@MR_MinStock", minimumStock);
                                cmd1.Parameters.AddWithValue("@MR_EmployeeRequireItemQty", EmpolyeeRequiredStock);
                                cmd1.Parameters.AddWithValue("@MR_ReceiveQty", 0);
                                cmd1.Parameters.AddWithValue("@MR_BalanceQty", EmpolyeeRequiredStock);
                                cmd1.Parameters.AddWithValue("@MR_RequireDate", EmpolyeeRequireDate);

                                con.Open();
                                int getMR_ID = (int)cmd1.ExecuteScalar();
                                con.Close();

                                if (getMR_ID > 0)
                                {
                                    var TableRowId = getMR_ID.ToString();
                                    /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                                    bool success = dal.AuditLogData("Material Request Add", "MaterialRequestDetails", TableRowId, "");
                                    if (success)
                                    { }
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                    }

                                    s = true;
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                                }
                            }
                        }
                    }
                }
                if (s == true)
                {
                    //bool notificationSent = dal.NotificationSend("Your-Department", "MR Edited for Approval", "MR No: " + lblMRNumber.Text + " has been edited for approval and is now ready for your review", ViewState["HODEmployeeId"].ToString());
                    //if (notificationSent)
                    //{
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Update Successfully'); </script>");
                    //}
                    //else
                    //{
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully But Notification not Send');</script>");
                    //}
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong for MaterialRequestDetails not update!');</script>");
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