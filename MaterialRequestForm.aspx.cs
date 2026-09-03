using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;

namespace LIMS
{
    public partial class MaterialRequestForm : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");

        DataAccessLayer dal = new DataAccessLayer();
        //private DataTable DataTable;
        private DataTable DataTable
        {
            get
            {
                if (ViewState["DataTable"] == null)
                {
                    // Create a new DataTable if it doesn't exist in ViewState
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ItemCode");
                    dt.Columns.Add("ItemName");
                    dt.Columns.Add("CurrentStock");
                    dt.Columns.Add("MinStock");
                    dt.Columns.Add("UOM");
                    dt.Columns.Add("RequireQty");
                    dt.Columns.Add("RequireDate");
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
                    string roleId = Session["RoleId"].ToString();
                    string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                    string pageName = System.IO.Path.GetFileName(currentPageUrl);
                    string urlToCheck = pageName;

                    ddlUOM.Enabled = false;
                    btnSubmit.Enabled = false;

                    List<CategoryNameData> res = dal.AllCategoryNameDisplayForDropDown();
                    ddlCategory.DataSource = res;
                    ddlCategory.DataTextField = "CategoryName";
                    ddlCategory.DataValueField = "CategoryId";
                    ddlCategory.DataBind();
                    ddlCategory.Items.Insert(0, new ListItem("Select", ""));
                    rfvddlCategory.InitialValue = "";

                    List<UOMData> uomData = dal.UOMDetailsGet();
                    ddlUOM.DataSource = uomData;
                    ddlUOM.DataTextField = "UOMCode";
                    ddlUOM.DataValueField = "UOMId";
                    ddlUOM.DataBind();
                    ddlUOM.Items.Insert(0, new ListItem("Select", ""));
                                        
                    InitializeDataTable();
                    BindGridView();
                }
                
            }
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
                List<InventoryItemData> res1 = dal.ActiveItemMasterDetailsGet();
                if(res1 != null && res1.Count > 0)
                {
                    InventoryItemData selectedItem1 = res1[0];
                    txtItemName.Text = selectedItem1.ItemName.ToString();
                    ddlUOM.SelectedValue = selectedItem1.UOMId.ToString();
                    txtCurrentStock.Text = "0";
                    txtMinStock.Text = selectedItem1.MinStock.ToString();
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Somethinwent wrong, No Records Found'); </script>");
                    txtItemName.Text = "";
                    txtCurrentStock.Text = "";
                    txtMinStock.Text = "";
                    ddlUOM.SelectedValue = null;
                }

                    
            }
        }

        private void InitializeDataTable()
        {   /* Check if the DataTable already exists in ViewState */
            if (ViewState["DataTable"] == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ItemCode");
                dt.Columns.Add("ItemName");
                dt.Columns.Add("CurrentStock");
                dt.Columns.Add("MinStock");
                dt.Columns.Add("UOM");
                dt.Columns.Add("RequireQty");
                dt.Columns.Add("RequireDate");
                ViewState["DataTable"] = dt;
            }
            else
            {
                DataTable = ViewState["DataTable"] as DataTable;
            }
        }

        private void BindGridView()
        {
            GridView1.DataSource = DataTable;
            GridView1.DataBind();
            btnSubmit.Enabled = DataTable != null && DataTable.Rows.Count > 0;
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedItem != null && !string.IsNullOrEmpty(ddlCategory.SelectedItem.Value))
            {
                int getItemCategoryId = Convert.ToInt32(ddlCategory.SelectedItem.Value);

                List<InventoryItemData> res = dal.ItemNameForDropDown();
                List<InventoryItemData> filteredData = res.Where(item => item.ItemTypeID == getItemCategoryId).ToList();
                ddlItemCode.DataSource = filteredData;
                ddlItemCode.DataTextField = "ItemCode";
                ddlItemCode.DataValueField = "ItemId";
                ddlItemCode.DataBind();
                ddlItemCode.Items.Insert(0, new ListItem("Select", ""));
                rfvItemCode.InitialValue = "";
            }
            else
            {
                ddlItemCode.Items.Clear();
                ddlItemCode.Items.Insert(0, new ListItem("Select", ""));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Select Category.'); </script>");
            }

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
            DataRow row = DataTable.Rows[e.RowIndex];
            row["RequireQty"] = requireQty;


            TextBox txtRequireDate = (TextBox)GridView1.Rows[e.RowIndex].FindControl("txtRequireDate");
            string requireDate = txtRequireDate.Text;
            DataRow rows = DataTable.Rows[e.RowIndex];
            rows["RequireDate"] = requireDate;

            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable.Rows[e.RowIndex].Delete();
            BindGridView();
        }
                
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string itemCode = ddlItemCode.SelectedItem.Text;
            string itemName = txtItemName.Text;
            string currentStock = txtCurrentStock.Text;
            string minStock = txtMinStock.Text;
            string uom = ddlUOM.SelectedItem.Text;
            string requireQty = txtRequireQty.Text;
            string requiredDate = txtRequiredDate.Text;
            //string projectCode = ddlProject.SelectedItem.Text;

            DataRow newRow = DataTable.NewRow();
            newRow["ItemCode"] = itemCode;
            newRow["ItemName"] = itemName;
            newRow["CurrentStock"] = currentStock;
            newRow["MinStock"] = minStock;
            newRow["UOM"] = uom;
            newRow["RequireQty"] = requireQty;
            newRow["RequireDate"] = requiredDate;
            //newRow["ProjectCode"] = projectCode;
            DataTable.Rows.Add(newRow);

            BindGridView();
            //ClearInputFields();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                int prevIdentity = dal.GetReseedIdentity("MaterialRequestMaster", "MRM_ID");
                dal.ReseedIdentity("MaterialRequestMaster", prevIdentity);
                con.Close();

                MRNumberGet();
                string MRM_Number = ViewState["MRM_NUMBER"].ToString();
                string MRM_Counter = ViewState["MRM_NUMBERCounter"].ToString();
                int currentYear = Convert.ToInt32(ViewState["MRM_CurrentYear"].ToString());
                bool s = false;

                string insertQuery = @"INSERT INTO MaterialRequestMaster
                                        (MRM_MasterCompanyId, MRM_EmployeeID, MRM_StatusId, MRM_IsActive, MRM_CounterYear, 
                                         MRM_CounterNumber, MRM_NUMBER, MRM_ApprovedEmployeeID, MRM_HODEmployeeID, MRM_CreatedDateTime,
                                         MRM_SendNotification,MRM_SendNotificationDateTime) 
                                        output INSERTED.MRM_ID values  
                                        (@MRM_MasterCompanyId, @MRM_EmployeeID, @MRM_StatusId, @MRM_IsActive, @MRM_CounterYear, 
                                         @MRM_CounterNumber, @MRM_NUMBER, @MRM_ApprovedEmployeeID, @MRM_HODEmployeeID, @MRM_CreatedDateTime,
                                         @MRM_SendNotification,@MRM_SendNotificationDateTime)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", Session["MasterCompanyId"].ToString());
                    //cmd.Parameters.AddWithValue("@MRM_EmployeeID", Session["EmployeeID"].ToString());
                    cmd.Parameters.AddWithValue("@MRM_EmployeeID", Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@MRM_StatusId", 20);
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_CounterYear", currentYear);
                    cmd.Parameters.AddWithValue("@MRM_CounterNumber", MRM_Counter);
                    cmd.Parameters.AddWithValue("@MRM_NUMBER", MRM_Number);
                    cmd.Parameters.AddWithValue("@MRM_CreatedDateTime", GetDateTime);
                    cmd.Parameters.AddWithValue("@MRM_HODEmployeeID", Session["HODEmployeeId"].ToString());
                    cmd.Parameters.AddWithValue("@MRM_ApprovedEmployeeID", 0);
                    //cmd.Parameters.AddWithValue("@MRM_ProjectCode", ddlProject.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@MRM_SendNotification", 1);
                    cmd.Parameters.AddWithValue("@MRM_SendNotificationDateTime", GetDateTime);

                    con.Open();
                    int getMRM_ID = (int)cmd.ExecuteScalar();
                    con.Close();

                    if (getMRM_ID > 0)
                    {
                        var TableRowId = getMRM_ID.ToString();
                        /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                        bool success = dal.AuditLogData("Material Request Send", "MaterialRequestMaster", TableRowId, "");
                        if (success)
                        {

                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                        }

                        using (SqlConnection connection = new SqlConnection(constr))
                        {
                            StringBuilder itemDetailsBuilder = new StringBuilder();

                            foreach (GridViewRow row in GridView1.Rows)
                            {
                                string itemCode = row.Cells[1].Text;
                                string itemName = row.Cells[2].Text;
                                decimal currentStock = Convert.ToDecimal(row.Cells[3].Text);
                                decimal minimumStock = Convert.ToDecimal(row.Cells[4].Text);
                                string UOMName = WebUtility.HtmlDecode(row.Cells[5].Text);
                                Label requiredStocks = row.FindControl("lblRequireQty") as Label;
                                decimal EmpolyeeRequiredStock = Convert.ToDecimal(requiredStocks.Text);
                                Label requiredDAte = row.FindControl("lblRequireDate") as Label;
                                string EmpolyeeRequireDate = requiredDAte.Text;
                                //string ProjectCode = row.Cells[8].Text;

                                con.Open();
                                int prevIdentity1 = dal.GetReseedIdentity("MaterialRequestDetails", "MR_ID");
                                dal.ReseedIdentity("MaterialRequestDetails", prevIdentity1);
                                con.Close();

                                string Query = @"INSERT INTO MaterialRequestDetails
                                                 (MR_StatusId, MR_MRMID, MR_NUMBER, MR_ItemCode, MR_ItemName, MR_UOM, MR_CurrentStock, MR_MinStock, 
                                                  MR_EmployeeRequireItemQty, MR_ReceiveQty, MR_BalanceQty, MR_RequireDate) 
                                                  output INSERTED.MR_ID values
                                                 (@MR_StatusId, @MR_MRMID, @MR_NUMBER, @MR_ItemCode, @MR_ItemName, @MR_UOM, @MR_CurrentStock, @MR_MinStock, 
                                                  @MR_EmployeeRequireItemQty, @MR_ReceiveQty, @MR_BalanceQty, @MR_RequireDate)";

                                using (SqlCommand cmd1 = new SqlCommand(Query, con))
                                {
                                    cmd1.Parameters.AddWithValue("@MR_StatusId", 20);
                                    cmd1.Parameters.AddWithValue("@MR_MRMID", getMRM_ID);
                                    cmd1.Parameters.AddWithValue("@MR_NUMBER", MRM_Number);
                                    cmd1.Parameters.AddWithValue("@MR_ItemCode", itemCode);
                                    cmd1.Parameters.AddWithValue("@MR_ItemName", itemName);
                                    cmd1.Parameters.AddWithValue("@MR_UOM", UOMName);
                                    cmd1.Parameters.AddWithValue("@MR_CurrentStock", currentStock);
                                    cmd1.Parameters.AddWithValue("@MR_MinStock", minimumStock);
                                    cmd1.Parameters.AddWithValue("@MR_EmployeeRequireItemQty", EmpolyeeRequiredStock);
                                    cmd1.Parameters.AddWithValue("@MR_ReceiveQty", 0);
                                    cmd1.Parameters.AddWithValue("@MR_BalanceQty", EmpolyeeRequiredStock);
                                    cmd1.Parameters.AddWithValue("@MR_RequireDate", EmpolyeeRequireDate);
                                    //cmd1.Parameters.AddWithValue("@MR_ProjectCode", ProjectCode);

                                    con.Open();
                                    int getMR_ID = (int)cmd1.ExecuteScalar();
                                    con.Close();

                                    if (getMR_ID > 0)
                                    {
                                        var TableRowId1 = getMR_ID.ToString();
                                        /* AuditLogData(LogEvent, TableName, TableRowId, Description); */
                                        bool success1 = dal.AuditLogData("Material Request Send", "MaterialRequestDetails", TableRowId1, "");
                                        if (success1)
                                        {

                                        }
                                        else
                                        {
                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                        }
                                        // Build HTML table row for this item
                                        itemDetailsBuilder.AppendFormat("<tr style=\"border: 1px solid #ddd;\">");
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px;\">{0}</td>", itemCode);
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px;\">{0}</td>", itemName);
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px; text-align: right;\">{0}</td>", currentStock);
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px; text-align: right;\">{0}</td>", minimumStock);
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px; text-align: center;\">{0}</td>", UOMName);
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px; text-align: right;\">{0}</td>", EmpolyeeRequiredStock);
                                        itemDetailsBuilder.AppendFormat("<td style=\"border: 1px solid #ddd; padding: 8px; text-align: right;\">{0}</td>", EmpolyeeRequireDate);
                                        itemDetailsBuilder.AppendFormat("</tr>");
                                        s = true;
                                    }
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                                    }
                                }
                            }
                            ViewState["ItemsTableRows"] = itemDetailsBuilder.ToString();
                        }
                        if (s == true)
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MR Generate');  setTimeout(function() { window.location = 'MaterialRequestShow.aspx'; }, 200);</script>");
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialRequestMaster Successfully but something went wrong for MaterialRequestDetails!');</script>");
                        }
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

        public void MRNumberGet()
        {
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now;
            DateTime startOfPeriod = new DateTime(currentYear, 4, 1);       /* Start on April 1st */
            DateTime endOfPeriod = new DateTime(currentYear + 1, 3, 31);    /* End on March 31st of next year */

            string MRM_NUMBER;
            int counter = 1;

            if (currentDate >= startOfPeriod && currentDate <= endOfPeriod)
            {
                int imaxCounter = IGetMaxCounterForCurrentYearFromDatabase();
                if (imaxCounter > 0)
                {
                    counter = imaxCounter + 1;
                    MRM_NUMBER = "MRM/" + currentYear + "-" + (currentYear + 1) + "/" + counter.ToString("D4");
                    ViewState["MRM_NUMBER"] = MRM_NUMBER;
                    ViewState["MRM_NUMBERCounter"] = counter.ToString("D4");
                    ViewState["MRM_CurrentYear"] = currentYear;
                }
                else
                {
                    counter = 1;
                    MRM_NUMBER = "MRM/" + currentYear + "-" + (currentYear + 1) + "/" + counter.ToString("D4");
                    ViewState["MRM_NUMBER"] = MRM_NUMBER;
                    ViewState["MRM_NUMBERCounter"] = counter.ToString("D4");
                    ViewState["MRM_CurrentYear"] = currentYear;
                }
            }
            else
            {
                int imaxCounter = IGetMaxCounterForCurrentYearFromDatabase();
                if (imaxCounter > 0)
                {
                    counter = imaxCounter + 1;
                    MRM_NUMBER = "MRM/" + (currentYear - 1) + "-" + (currentYear) + "/" + counter.ToString("D4");
                    ViewState["MRM_NUMBER"] = MRM_NUMBER;
                    ViewState["MRM_NUMBERCounter"] = counter.ToString("D4");
                    ViewState["MRM_CurrentYear"] = currentYear - 1;
                }
                else
                {
                    counter = 1;
                    MRM_NUMBER = "MRM/" + (currentYear - 1) + "-" + (currentYear) + "/" + counter.ToString("D4");
                    ViewState["MRM_NUMBER"] = MRM_NUMBER;
                    ViewState["MRM_NUMBERCounter"] = counter.ToString("D4");
                    ViewState["MRM_CurrentYear"] = currentYear - 1;
                }
            }
            ViewState["MRM_NUMBER"] = MRM_NUMBER;
            ViewState["MRM_NUMBERCounter"] = counter.ToString("D4");
        }

        private int IGetMaxCounterForCurrentYearFromDatabase()
        {
            int currentYear = DateTime.Now.Year;
            DateTime currentDate = DateTime.Now;
            DateTime startOfPeriod = new DateTime(currentYear, 4, 1); // Start on April 1st
            DateTime endOfPeriod = new DateTime(currentYear + 1, 3, 31); // End on March 31st of next year

            string MRM_NUMBER;
            int counter = 1;
            int imaxCounter = 0;

            if (currentDate >= startOfPeriod && currentDate <= endOfPeriod)
            {
                string query = "SELECT MAX(MRM_CounterNumber) FROM MaterialRequestMaster WHERE MRM_CounterYear = @MRM_CounterYear";
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@MRM_CounterYear", currentYear);
                con.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    imaxCounter = Convert.ToInt32(result);
                }
                con.Close();

                return imaxCounter;
            }
            else
            {
                string query = "SELECT MAX(MRM_CounterNumber) FROM MaterialRequestMaster WHERE MRM_CounterYear = @MRM_CounterYear";
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@MRM_CounterYear", currentYear - 1);
                con.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    imaxCounter = Convert.ToInt32(result);
                }
                con.Close();
                return imaxCounter;
            }
        }


    }
}