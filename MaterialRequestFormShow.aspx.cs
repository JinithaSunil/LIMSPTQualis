using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;
using ZXing;

namespace LIMS
{
    public partial class MaterialRequestFormShow : System.Web.UI.Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
        private int Year;
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
                    try
                    {
                        string roleId = Session["RoleId"].ToString();
                        string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                        string pageName = System.IO.Path.GetFileName(currentPageUrl);
                        string urlToCheck = pageName;
                        //txtItemName.Enabled = false;
                        txtPreItemSend.Enabled = false;
                        txtPreBalanceQty.Enabled = false;
                        btnSubmit.Enabled = false;
                        ddlUOM.Enabled = false;

                        if (Request.QueryString["PageID"] != null)
                        {

                            if (ddlItemCode.SelectedIndex > 0)
                            {
                                txtSearch.Enabled = true;
                                txtItemQtySend.Enabled = true;
                                GridView2.Visible = true;
                            }
                            else
                            {
                                txtSearch.Text = "";
                                txtSearch.Enabled = false;
                                txtItemQtySend.Text = "";
                                txtItemQtySend.Enabled = false;
                                GridView2.Visible = false;
                            }

                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            List<UOMData> uomData = dal.UOMDetailsGet();
                            ddlUOM.DataSource = uomData;
                            ddlUOM.DataTextField = "UOMCode";
                            ddlUOM.DataValueField = "UOMId";
                            ddlUOM.DataBind();
                            ddlUOM.Items.Insert(0, new ListItem("Select", ""));

                            int employeeId = Convert.ToInt32(Session["UserID"]);

                            List<MaterialRequestData> response = dal.MaterialRequestGetData();
                            List<MaterialRequestData> res = response.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text)).ToList();
                            if (res != null && res.Count > 0)
                            {
                                MaterialRequestData selectedItems = res[0];
                                string SendEmployeeId = selectedItems.MRM_EmployeeID.ToString();
                                ViewState["NotificationToEmployeeId"] = SendEmployeeId.ToString();
                                ViewState["MRM_NUMBER"] = selectedItems.MRM_NUMBER.ToString();

                                string projectCode = selectedItems.MRM_ProjectCode.ToString();
                                ViewState["ProjectCodeGetSet"] = projectCode;

                                GridView1.DataSource = res;
                                GridView1.DataBind();
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found in MaterialRequestGetData.'); </script>");
                            }

                            List<MaterialRequestData> response1 = dal.MaterialRequestThrowItemCode();
                            List<MaterialRequestData> res1 = response1.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text)
                                                                                  && item.MR_BalanceQty > 0).ToList();
                            if (res1 != null && res1.Count > 0)
                            {
                                MaterialRequestData selectedItem = res1[0];
                                ddlItemCode.DataSource = res1;
                                ddlItemCode.DataTextField = "MR_ItemCode";
                                ddlItemCode.DataValueField = "MR_ID";
                                ddlItemCode.DataBind();
                                ddlItemCode.Items.Insert(0, new ListItem("Select", ""));
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MR Completed.'); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something wrong, PageID Not Getting!!'); setTimeout(function() { window.location = 'GRNGenerateShow.aspx'; }, 100); </script>");
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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                /* Assuming the index of the column MR_RequireDate is 8 */
                int columnIndex = 8;

                if (e.Row.Cells.Count > columnIndex)
                {
                    string originalDate = e.Row.Cells[columnIndex].Text;

                    if (!string.IsNullOrEmpty(originalDate))
                    {
                        DateTime parsedDate;
                        if (DateTime.TryParse(originalDate, out parsedDate))
                        {
                            e.Row.Cells[columnIndex].Text = parsedDate.ToString("dd-MMM-yyyy");
                        }
                    }
                }
            }
        }

        protected void ddlItemCode_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlItemCode.SelectedIndex > 0)
            {
                txtSearch.Enabled = true;
                txtItemQtySend.Enabled = true;
            }
            else
            {
                txtSearch.Text = "";
                txtSearch.Enabled = false;
                txtItemQtySend.Text = "";
                txtItemQtySend.Enabled = false;

                GridView3.Visible = false;
                GridView2.Visible = false;

                txtPreItemSend.Text = "";
                txtPreBalanceQty.Text = "";
            }

            string getItemCode = ddlItemCode.SelectedItem.Text;

            List<InventoryItemData> res1 = dal.ItemNameForDropDown();
            List<InventoryItemData> filteredData1 = res1.Where(item => item.ItemCode == getItemCode).ToList();

            if (filteredData1 != null && filteredData1.Count > 0)
            {
                InventoryItemData itemMasterData = filteredData1[0];
                int getItemId = itemMasterData.ItemID;
                ViewState["ItemId"] = getItemId;                
            }

            List<MaterialRequestData> response = dal.MaterialRequestGetData();
            List<MaterialRequestData> res3 = response.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text) &&
                                                                    item.MR_ItemCode == ddlItemCode.SelectedItem.Text).ToList();
            if (res3 != null && res3.Count > 0)
            {
                MaterialRequestData selectedItem = res3[0];

                ddlUOM.SelectedItem.Text = selectedItem.MR_UOM;

                ViewState["MR_BalanceQty"] = selectedItem.MR_BalanceQty;

                decimal MR_ReceiveQty = selectedItem.MR_ReceiveQty;

                if (MR_ReceiveQty != 0)
                {
                    txtItemQtySend.Text = selectedItem.MR_BalanceQty.ToString();
                }
                else
                {
                    txtItemQtySend.Text = selectedItem.MR_EmployeeRequireItemQty.ToString();
                }

                if (Convert.ToDecimal(txtItemQtySend.Text) == 0)
                {
                    btnSubmit.Enabled = false;
                }
                else
                {
                    btnSubmit.Enabled = true;
                }

                ViewState["MR_EmployeeRequireItemQty"] = selectedItem.MR_EmployeeRequireItemQty.ToString();
                btnFilterData_Click(sender, e);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
            }

        }

        protected void btnFilterData_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    int Tid = Convert.ToInt32(txtSearch.Text);

                    if (Tid > 0)
                    {
                        List<ItemTransactionData> filteredData2 = dal.ItemTransactionMasterDataGetforReleased(ddlItemCode.SelectedItem.Text, txtItemQtySend.Text, txtSearch.Text, ViewState["ProjectCodeGetSet"].ToString());

                        if (filteredData2 != null && filteredData2.Count > 0)
                        {
                            GridView3.Visible = false;
                            GridView2.Visible = true;
                            GridView2.DataSource = filteredData2;
                            GridView2.DataBind();
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Invalid Search Data'); </script>");
                    }
                }
                else
                {
                    decimal ReQuiredQty = Convert.ToDecimal(ViewState["MR_EmployeeRequireItemQty"].ToString());
                    decimal GivenQty = Convert.ToDecimal(txtItemQtySend.Text);

                    if (GivenQty > ReQuiredQty)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Require qty is not valid'); </script>");
                        btnSubmit.Enabled = false;
                    }
                    else
                    {
                        btnSubmit.Enabled = true;
                        List<ItemTransactionData> filteredData2 = dal.ItemTransactionMasterDataGetforMRSendItem(ddlItemCode.SelectedItem.Text, txtItemQtySend.Text, ViewState["ProjectCodeGetSet"].ToString());

                        if (filteredData2 != null && filteredData2.Count > 0)
                        {
                            //ItemTransactionData selectedItem2 = filteredData2[0];
                            //List<ItemTransactionData> filteredDataWithPositiveTotalItem = filteredData2.Where(data => Convert.ToDecimal(data.TotalItemInRack.ToString()) > 0).ToList();
                            //GridView3.DataSource = filteredDataWithPositiveTotalItem;
                            GridView2.Visible = false;
                            GridView3.Visible = true;
                            GridView3.DataSource = filteredData2;
                            GridView3.DataBind();

                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                        }

                        List<MasterTransactionData> response = dal.MasterTransactionDataGet();
                        List<MasterTransactionData> filteredData1 = response.Where(item => item.MT_MRMID == Convert.ToInt32(lblPageID.Text) &&
                                                                                           item.MT_ItemCode == ddlItemCode.SelectedItem.Text).ToList();
                        if (filteredData1 != null && filteredData1.Count > 0)
                        {
                            MasterTransactionData selectedItem = filteredData1[0];
                            txtPreItemSend.Text = selectedItem.MT_ReceivedQty.ToString();
                            txtPreBalanceQty.Text = selectedItem.MT_BalanceQty.ToString();

                            if (Convert.ToDecimal(selectedItem.MT_BalanceQty.ToString()) == 0)
                            {
                                btnSubmit.Enabled = false;
                            }
                            else
                            {
                                btnSubmit.Enabled = true;
                            }
                        }
                        else
                        {
                            txtPreItemSend.Text = "0";
                            txtPreBalanceQty.Text = ViewState["MR_BalanceQty"].ToString();
                            decimal preBalance = Convert.ToDecimal(txtPreBalanceQty.Text);
                            if (preBalance > 0)
                            {
                                btnSubmit.Enabled = true;
                            }
                            else
                            {
                                btnSubmit.Enabled = false;
                            }
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
        }

        protected void chkIsQtyGo_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkIsQtyGo = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chkIsQtyGo.NamingContainer;
            TextBox txtCustom = (TextBox)row.FindControl("txtCustom");

            txtCustom.Enabled = chkIsQtyGo.Checked;

            if (!chkIsQtyGo.Checked)
            {
                txtCustom.Text = string.Empty;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                bool c1 = false, c2 = false;
                decimal EmpolyeeRequiredStock = Convert.ToDecimal(ViewState["MR_EmployeeRequireItemQty"].ToString());
                decimal PreBalanceStock = Convert.ToDecimal(txtPreBalanceQty.Text);
                decimal ItemQtySend = Convert.ToDecimal(txtItemQtySend.Text);
                decimal ReQuiredQty = Convert.ToDecimal(ViewState["MR_EmployeeRequireItemQty"].ToString());
                decimal GivenQty = Convert.ToDecimal(txtItemQtySend.Text);

                string projectCodeset = ViewState["ProjectCodeGetSet"].ToString();

                if (GivenQty > ReQuiredQty)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('Require qty is not valid');", true);
                }
                else
                {
                    if (ItemQtySend == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('No Valid Item Qty.');", true);
                    }
                    else
                    {
                        int ItemId = Convert.ToInt32(ViewState["ItemId"].ToString());

                        List<InventoryItemData> response2 = dal.ActiveInventoryDetailsGet();
                        List<InventoryItemData> filteredData2 = response2.Where(items => items.InventoryItemId == ItemId).ToList();

                        if (filteredData2 != null && filteredData2.Count > 0)
                        {
                            InventoryItemData selectedItem2 = filteredData2[0];

                            decimal CurrentStock = selectedItem2.InventoryCurrentStock;

                            if (CurrentStock >= ItemQtySend)
                            {
                                List<MaterialRequestData> response3 = dal.MaterialRequestGetData();
                                List<MaterialRequestData> res3 = response3.Where(item => item.MRM_ID == Convert.ToInt32(lblPageID.Text) &&
                                                                                        item.MR_ItemCode == ddlItemCode.SelectedItem.Text).ToList();
                                if (res3 != null && res3.Count > 0)
                                {
                                    MaterialRequestData selectedItem3 = res3[0];
                                    decimal totalReceivedQty = selectedItem3.MR_ReceiveQty;
                                    decimal TotalItemQtySend = totalReceivedQty + ItemQtySend;
                                    decimal BalanceQty = PreBalanceStock - ItemQtySend;

                                    con.Open();
                                    int prevIdentity = dal.GetReseedIdentity("MaterialTransactionMaster", "MT_ID");
                                    dal.ReseedIdentity("MaterialTransactionMaster", prevIdentity);
                                    con.Close();

                                    string insertQuery = @"INSERT INTO MaterialTransactionMaster
                                                           (MT_MRMID, MT_MRID, MT_ItemCode, MT_ProjectCode, MT_RequiredQty, MT_ReceivedQty, MT_BalanceQty, 
                                                            MT_PreviousReceivedQty, MT_PreviousBalanceQty, MT_CreatedDateTime, 
                                                            MT_MasterCompanyId, MT_EmployeeId, MT_StatusId) 
                                                           output INSERTED.MT_ID values  
                                                           (@MT_MRMID, @MT_MRID, @MT_ItemCode, @MT_ProjectCode, @MT_RequiredQty, @MT_ReceivedQty, @MT_BalanceQty, 
                                                            @MT_PreviousReceivedQty, @MT_PreviousBalanceQty, @MT_CreatedDateTime, 
                                                            @MT_MasterCompanyId, @MT_EmployeeId, @MT_StatusId)";

                                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                                    {
                                        cmd.Parameters.AddWithValue("@MT_MRMID", lblPageID.Text);
                                        cmd.Parameters.AddWithValue("@MT_MRID", ddlItemCode.SelectedItem.Value);
                                        cmd.Parameters.AddWithValue("@MT_ItemCode", ddlItemCode.SelectedItem.Text);
                                        cmd.Parameters.AddWithValue("@MT_ProjectCode", projectCodeset);
                                        cmd.Parameters.AddWithValue("@MT_RequiredQty", EmpolyeeRequiredStock);
                                        cmd.Parameters.AddWithValue("@MT_ReceivedQty", ItemQtySend);
                                        cmd.Parameters.AddWithValue("@MT_BalanceQty", BalanceQty);
                                        cmd.Parameters.AddWithValue("@MT_PreviousReceivedQty", Convert.ToDecimal(txtPreItemSend.Text));
                                        cmd.Parameters.AddWithValue("@MT_PreviousBalanceQty", Convert.ToDecimal(txtPreBalanceQty.Text));
                                        cmd.Parameters.AddWithValue("@MT_CreatedDateTime", GetDateTime);
                                        cmd.Parameters.AddWithValue("@MT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                        cmd.Parameters.AddWithValue("@MT_EmployeeId", Session["EmployeeID"].ToString());

                                        if (TotalItemQtySend == EmpolyeeRequiredStock)
                                        {
                                            cmd.Parameters.AddWithValue("@MT_StatusId", 3);
                                        }
                                        else
                                        {
                                            cmd.Parameters.AddWithValue("@MT_StatusId", 4);
                                        }

                                        con.Open();
                                        int getID = (int)cmd.ExecuteScalar();
                                        ViewState["MT_ID"] = getID;
                                        con.Close();

                                        if (getID > 0)
                                        {
                                            var TableRowId = getID.ToString();
                                            bool success = dal.AuditLogData("Material Transaction Add", "MaterialTransactionMaster", TableRowId, "");
                                            if (success)
                                            { }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                            }

                                            string Query = @"UPDATE MaterialRequestDetails SET MR_ReceiveQty = @MR_ReceiveQty, MR_BalanceQty = @MR_BalanceQty
                                                             WHERE MR_MRMID = @MR_MRMID AND MR_ItemCode = @MR_ItemCode";

                                            using (SqlCommand cmd1 = new SqlCommand(Query, con))
                                            {
                                                cmd1.Parameters.AddWithValue("@MR_MRMID", lblPageID.Text);
                                                cmd1.Parameters.AddWithValue("@MR_ItemCode", ddlItemCode.SelectedItem.Text);
                                                cmd1.Parameters.AddWithValue("@MR_ReceiveQty", TotalItemQtySend);
                                                cmd1.Parameters.AddWithValue("@MR_BalanceQty", BalanceQty);
                                                con.Open();
                                                int I = cmd1.ExecuteNonQuery();
                                                con.Close();
                                                if (I > 0)
                                                {
                                                    var TableRowId1 = lblPageID.Text;
                                                    bool success1 = dal.AuditLogData("Material Release", "MaterialRequestDetails", TableRowId1, "");
                                                    if (success1)
                                                    { }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                    }

                                                    decimal remainingStock = CurrentStock - ItemQtySend;

                                                    string Query2 = @"UPDATE InventoryMaster SET InventoryCurrentStock = @InventoryCurrentStock 
                                                                        WHERE InventoryItemId = @InventoryItemId";

                                                    using (SqlCommand cmd2 = new SqlCommand(Query2, con))
                                                    {
                                                        cmd2.Parameters.AddWithValue("@InventoryItemId", ItemId);
                                                        cmd2.Parameters.AddWithValue("@InventoryCurrentStock", remainingStock);

                                                        con.Open();
                                                        int j = cmd2.ExecuteNonQuery();
                                                        con.Close();
                                                        if (j > 0)
                                                        {
                                                            var TableRowId2 = ItemId.ToString();
                                                            bool success2 = dal.AuditLogData("InventoryMaster Stock Update When Material Release", "InventoryMaster", TableRowId2, "");
                                                            if (success2)
                                                            { }
                                                            else
                                                            {
                                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                            }

                                                            List<WarehouseStockDetailsData> responseData11 = dal.WarehouseStockDetailsGet();
                                                            List<WarehouseStockDetailsData> filteredData11 = responseData11.Where(item => item.SC_ItemCode == ddlItemCode.SelectedItem.Text && item.SC_EntryDate == date).ToList();

                                                            if (filteredData11 != null && filteredData11.Count > 0)
                                                            {
                                                                WarehouseStockDetailsData selectedItems11 = filteredData11[0];
                                                                decimal SC_TotalReceivedQty = selectedItems11.SC_TotalReceivedQty;
                                                                decimal SCTotalReleasedQty = selectedItems11.SC_TotalReleasedQty;
                                                                decimal TotalReleasedQty = SCTotalReleasedQty + Convert.ToDecimal(txtItemQtySend.Text);
                                                                string scDate = selectedItems11.SC_ID.ToString();

                                                                bool IsExisted = dal.IsDateExistForDataStoreOrRelease(date, ddlItemCode.SelectedItem.Text);

                                                                if (IsExisted)
                                                                {
                                                                    using (SqlConnection con5 = new SqlConnection(constr))
                                                                    {
                                                                        string str5 = @"UPDATE WarehouseStockDetails set SC_TotalReleasedQty = @SC_TotalReleasedQty, SC_UpdatedDateTime = @SC_UpdatedDateTime
                                                                                        WHERE SC_EntryDate = @SC_EntryDate AND SC_ItemCode = @SC_ItemCode AND SC_MasterCompanyId = @SC_MasterCompanyId";

                                                                        SqlCommand cmd5 = new SqlCommand(str5, con5);
                                                                        cmd5.Parameters.AddWithValue("@SC_TotalReleasedQty", TotalReleasedQty);
                                                                        cmd5.Parameters.AddWithValue("@SC_ItemCode", ddlItemCode.SelectedItem.Text);
                                                                        cmd5.Parameters.AddWithValue("@SC_MasterCompanyId", Session["MasterCompanyId"]);
                                                                        cmd5.Parameters.AddWithValue("@SC_EntryDate", date);
                                                                        cmd5.Parameters.AddWithValue("@SC_UpdatedDateTime", GetDateTime);

                                                                        con5.Open();
                                                                        int f1 = cmd5.ExecuteNonQuery();
                                                                        con5.Close();

                                                                        if (f1 > 0)
                                                                        {
                                                                            var TableRowId3 = scDate;
                                                                            bool success3 = dal.AuditLogData("WarehouseStock Stock Update When Material Release", "WarehouseStockDetails", TableRowId3, "");
                                                                            if (success3)
                                                                            { }
                                                                            else
                                                                            {
                                                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                            }

                                                                            c1 = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            throw new Exception("Failed to update WarehouseStockDetails.");
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    /* insert the data to w.stock*/
                                                                    con.Open();
                                                                    int prevIdentity1 = dal.GetReseedIdentity("WarehouseStockDetails", "SC_ID");
                                                                    dal.ReseedIdentity("WarehouseStockDetails", prevIdentity1);
                                                                    con.Close();

                                                                    string query1 = @"INSERT INTO WarehouseStockDetails 
                                                                                       (SC_ItemCode, SC_TotalReleasedQty, SC_TotalReceivedQty, SC_BalanceQty,
                                                                                        SC_MasterCompanyId, SC_EmployeeId, SC_EntryDate, SC_UOM, SC_CreatedDateTime)
                                                                                      OUTPUT INSERTED.SC_ID VALUES 
                                                                                       (@SC_ItemCode, @SC_TotalReleasedQty, @SC_TotalReceivedQty, @SC_BalanceQty,
                                                                                        @SC_MasterCompanyId, @SC_EmployeeId, @SC_EntryDate, @SC_UOM, @SC_CreatedDateTime)";

                                                                    using (SqlCommand cmds = new SqlCommand(query1, con))
                                                                    {
                                                                        cmds.Parameters.AddWithValue("@SC_ItemCode", ddlItemCode.SelectedItem.Text);
                                                                        cmds.Parameters.AddWithValue("@SC_TotalReleasedQty", TotalReleasedQty);
                                                                        cmds.Parameters.AddWithValue("@SC_TotalReceivedQty", 0);
                                                                        cmds.Parameters.AddWithValue("@SC_BalanceQty", 0);
                                                                        cmds.Parameters.AddWithValue("@SC_MasterCompanyId", Session["MasterCompanyId"]);
                                                                        cmds.Parameters.AddWithValue("@SC_EmployeeId",Session["UserID"]);
                                                                        cmds.Parameters.AddWithValue("@SC_EntryDate", date);
                                                                        cmds.Parameters.AddWithValue("@SC_UOM", ddlUOM.SelectedItem.Text);
                                                                        cmds.Parameters.AddWithValue("@SC_CreatedDateTime", GetDateTime);

                                                                        con.Open();
                                                                        int InsertGetId1 = (int)cmds.ExecuteScalar();
                                                                        con.Close();

                                                                        if (InsertGetId1 > 0)
                                                                        {
                                                                            var TableRowId3 = InsertGetId1.ToString();
                                                                            bool success3 = dal.AuditLogData("WarehouseStock Stock ADD", "WarehouseStockDetails", TableRowId3, "");
                                                                            if (success3)
                                                                            { }
                                                                            else
                                                                            {
                                                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                            }

                                                                            c1 = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            throw new Exception("Failed to SAVE Item Store in warehouse details.");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                bool IsExisted = dal.IsDateExistForDataStoreOrRelease(date, ddlItemCode.SelectedItem.Text);

                                                                if (IsExisted)
                                                                {
                                                                    throw new Exception("Data not Getting!");
                                                                }
                                                                else
                                                                {
                                                                    /* insert the data to w.stock*/
                                                                    con.Open();
                                                                    int prevIdentity1 = dal.GetReseedIdentity("WarehouseStockDetails", "SC_ID");
                                                                    dal.ReseedIdentity("WarehouseStockDetails", prevIdentity1);
                                                                    con.Close();

                                                                    string query1 = @"INSERT INTO WarehouseStockDetails 
                                                                                       (SC_ItemCode, SC_TotalReleasedQty, SC_TotalReceivedQty, SC_BalanceQty, 
                                                                                        SC_MasterCompanyId, SC_EmployeeId, SC_EntryDate, SC_UOM, SC_CreatedDateTime)
                                                                                      OUTPUT INSERTED.SC_ID VALUES 
                                                                                       (@SC_ItemCode, @SC_TotalReleasedQty, @SC_TotalReceivedQty, @SC_BalanceQty,
                                                                                        @SC_MasterCompanyId, @SC_EmployeeId, @SC_EntryDate, @SC_UOM, @SC_CreatedDateTime)";

                                                                    using (SqlCommand cmda = new SqlCommand(query1, con))
                                                                    {
                                                                        cmda.Parameters.AddWithValue("@SC_ItemCode", ddlItemCode.SelectedItem.Text);
                                                                        cmda.Parameters.AddWithValue("@SC_TotalReleasedQty", Convert.ToDecimal(txtItemQtySend.Text));
                                                                        cmda.Parameters.AddWithValue("@SC_TotalReceivedQty", 0);
                                                                        cmda.Parameters.AddWithValue("@SC_BalanceQty", 0);
                                                                        cmda.Parameters.AddWithValue("@SC_MasterCompanyId", Session["MasterCompanyId"]);
                                                                        cmda.Parameters.AddWithValue("@SC_EmployeeId",Session["UserID"]);
                                                                        cmda.Parameters.AddWithValue("@SC_EntryDate", date);
                                                                        cmda.Parameters.AddWithValue("@SC_UOM", ddlUOM.SelectedItem.Text);
                                                                        cmda.Parameters.AddWithValue("@SC_CreatedDateTime", GetDateTime);

                                                                        con.Open();
                                                                        int InsertGetId1 = (int)cmda.ExecuteScalar();
                                                                        con.Close();

                                                                        if (InsertGetId1 > 0)
                                                                        {
                                                                            var TableRowId3 = InsertGetId1.ToString();
                                                                            bool success3 = dal.AuditLogData("WarehouseStock Stock ADD", "WarehouseStockDetails", TableRowId3, "");
                                                                            if (success3)
                                                                            { }
                                                                            else
                                                                            {
                                                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                            }

                                                                            c1 = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            throw new Exception("Failed to SAVE Item Store in warehouse details.");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            //c1 = true;
                                                        }
                                                        else
                                                        {
                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialTransactionMaster Successfully Insert but MaterialRequestDetails not Update');</script>");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    c1 = false;
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialTransactionMaster Successfully Insert but MaterialRequestDetails not Update');</script>");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                                        }
                                    }

                                    if (c1 == true)
                                    {
                                        if (GridView2.Rows.Count > 0)
                                        {
                                            foreach (GridViewRow row in GridView2.Rows)
                                            {
                                                CheckBox chkIsQtyGo = row.FindControl("chkIsQtyGo") as CheckBox;
                                                if (chkIsQtyGo != null)
                                                {
                                                    bool isQtyGoChecked = chkIsQtyGo.Checked;
                                                    TextBox txtCustom = (TextBox)row.FindControl("txtCustom");

                                                    if (isQtyGoChecked)
                                                    {
                                                        int ISItemStoreId = Convert.ToInt32(GridView2.DataKeys[row.RowIndex]["IS_ItemStoreId"]);
                                                        string itemCode = row.Cells[1].Text;
                                                        decimal itemRowStock = Convert.ToDecimal(row.Cells[6].Text);
                                                        string itemBatchNo = row.Cells[8].Text;
                                                        string itemExpiryDate = row.Cells[9].Text;
                                                        string description = txtCustom.Text;

                                                        List<ItemStoreData> response = dal.DataStoreinRack();
                                                        List<ItemStoreData> filteredData1 = response.Where(item => item.IS_ItemStoreId == ISItemStoreId).ToList();

                                                        if (filteredData1 != null && filteredData1.Count > 0)
                                                        {
                                                            ItemStoreData selectedItem = filteredData1[0];
                                                            decimal StoreQty = selectedItem.IS_StoreQty;
                                                            decimal remainingIsStoreQty = StoreQty - ItemQtySend;
                                                            int mtID = Convert.ToInt32(ViewState["MT_ID"].ToString());

                                                            if (remainingIsStoreQty >= 0)
                                                            {
                                                                string Query = @"UPDATE ItemStore SET IS_StoreQty = @IS_StoreQty WHERE IS_ItemStoreId = @IS_ItemStoreId";
                                                                using (SqlCommand cmd1 = new SqlCommand(Query, con))
                                                                {
                                                                    cmd1.Parameters.AddWithValue("@IS_ItemStoreId", ISItemStoreId);
                                                                    cmd1.Parameters.AddWithValue("@IS_StoreQty", remainingIsStoreQty);
                                                                    con.Open();
                                                                    int I = cmd1.ExecuteNonQuery();
                                                                    con.Close();
                                                                    if (I > 0)
                                                                    {
                                                                        var TableRowId3 = ISItemStoreId.ToString();
                                                                        bool success3 = dal.AuditLogData("ItemStore Stock Update when Material Release", "ItemStore", TableRowId3, "");
                                                                        if (success3)
                                                                        { }
                                                                        else
                                                                        {
                                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                        }

                                                                        con.Open();
                                                                        int prevIdentity1 = dal.GetReseedIdentity("MaterialTransationSubData", "MTS_ID");
                                                                        dal.ReseedIdentity("MaterialTransationSubData", prevIdentity1);
                                                                        con.Close();

                                                                        string insertQuery1 = @"INSERT INTO MaterialTransationSubData
                                                                                                (MTS_MTID, MTS_ItemStock, MTS_BatchNo, MTS_ExpiryDate, MTS_CreatedDateTime, MTS_Description) 
                                                                                                output INSERTED.MTS_ID values  
                                                                                                (@MTS_MTID, @MTS_ItemStock, @MTS_BatchNo, @MTS_ExpiryDate, @MTS_CreatedDateTime, @MTS_Description)";

                                                                        using (SqlCommand cmd = new SqlCommand(insertQuery1, con))
                                                                        {
                                                                            cmd.Parameters.AddWithValue("@MTS_MTID", mtID);
                                                                            cmd.Parameters.AddWithValue("@MTS_ItemStock", ItemQtySend);
                                                                            cmd.Parameters.AddWithValue("@MTS_BatchNo", itemBatchNo);
                                                                            cmd.Parameters.AddWithValue("@MTS_ExpiryDate", itemExpiryDate);
                                                                            cmd.Parameters.AddWithValue("@MTS_Description", description);
                                                                            cmd.Parameters.AddWithValue("@MTS_CreatedDateTime", GetDateTime);

                                                                            con.Open();
                                                                            int getIDs = (int)cmd.ExecuteScalar();
                                                                            con.Close();

                                                                            if (getIDs > 0)
                                                                            {
                                                                                var TableRowId4 = getIDs.ToString();
                                                                                bool success4 = dal.AuditLogData("Material Transation-Subdata ADD When Material Release", "MaterialTransationSubData", TableRowId4, "");
                                                                                if (success4)
                                                                                { }
                                                                                else
                                                                                {
                                                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                                }

                                                                                c2 = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialTransactionMaster Successfully Insert but MaterialRequestDetails not Update');</script>");
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ItemQtySend = ItemQtySend - StoreQty;
                                                                string Query = @"UPDATE ItemStore SET IS_StoreQty = @IS_StoreQty WHERE IS_ItemStoreId = @IS_ItemStoreId";
                                                                using (SqlCommand cmd1 = new SqlCommand(Query, con))
                                                                {
                                                                    cmd1.Parameters.AddWithValue("@IS_ItemStoreId", ISItemStoreId);
                                                                    cmd1.Parameters.AddWithValue("@IS_StoreQty", 0);

                                                                    con.Open();
                                                                    int I = cmd1.ExecuteNonQuery();
                                                                    con.Close();
                                                                    if (I > 0)
                                                                    {
                                                                        var TableRowId3 = ISItemStoreId.ToString();
                                                                        bool success3 = dal.AuditLogData("ItemStore Stock Update", "ItemStore", TableRowId3, "");
                                                                        if (success3)
                                                                        { }
                                                                        else
                                                                        {
                                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                        }

                                                                        con.Open();
                                                                        int prevIdentity1 = dal.GetReseedIdentity("MaterialTransationSubData", "MTS_ID");
                                                                        dal.ReseedIdentity("MaterialTransationSubData", prevIdentity1);
                                                                        con.Close();

                                                                        string insertQuery1 = @"INSERT INTO MaterialTransationSubData
                                                                                                (MTS_MTID, MTS_ItemStock, MTS_BatchNo, MTS_ExpiryDate, MTS_CreatedDateTime, MTS_Description) 
                                                                                                output INSERTED.MTS_ID values  
                                                                                                (@MTS_MTID, @MTS_ItemStock, @MTS_BatchNo, @MTS_ExpiryDate, @MTS_CreatedDateTime, @MTS_Description)";

                                                                        using (SqlCommand cmd = new SqlCommand(insertQuery1, con))
                                                                        {
                                                                            cmd.Parameters.AddWithValue("@MTS_MTID", mtID);
                                                                            cmd.Parameters.AddWithValue("@MTS_ItemStock", itemRowStock);
                                                                            //cmd.Parameters.AddWithValue("@MTS_ItemStock", ItemQtySend);//
                                                                            cmd.Parameters.AddWithValue("@MTS_BatchNo", itemBatchNo);
                                                                            cmd.Parameters.AddWithValue("@MTS_ExpiryDate", itemExpiryDate);
                                                                            cmd.Parameters.AddWithValue("@MTS_Description", description);
                                                                            cmd.Parameters.AddWithValue("@MTS_CreatedDateTime", GetDateTime);
                                                                            con.Open();
                                                                            int getIDs = (int)cmd.ExecuteScalar();
                                                                            con.Close();
                                                                            if (getIDs > 0)
                                                                            {
                                                                                var TableRowId4 = getIDs.ToString();
                                                                                bool success4 = dal.AuditLogData("Material Transation-Subdata ADD", "MaterialTransationSubData", TableRowId4, "");
                                                                                if (success4)
                                                                                { }
                                                                                else
                                                                                {
                                                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                                }
                                                                                c2 = true;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialTransactionMaster Successfully Insert but MaterialRequestDetails not Update');</script>");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int ISItemStoreId = Convert.ToInt32(GridView2.DataKeys[row.RowIndex]["IS_ItemStoreId"]);
                                                        string itemCode = row.Cells[1].Text;
                                                    }
                                                }
                                            }
                                        }
                                        else if (GridView3.Rows.Count > 0)
                                        {
                                            foreach (GridViewRow row in GridView3.Rows)
                                            {
                                                CheckBox chkIsQtyGo = row.FindControl("chkIsQtyGo") as CheckBox;
                                                if (chkIsQtyGo != null)
                                                {
                                                    bool isQtyGoChecked = chkIsQtyGo.Checked;
                                                    TextBox txtCustom = (TextBox)row.FindControl("txtCustom");

                                                    if (isQtyGoChecked)
                                                    {
                                                        int ISItemStoreId = Convert.ToInt32(GridView3.DataKeys[row.RowIndex]["IS_ItemStoreId"]);
                                                        string itemCode = row.Cells[1].Text;
                                                        decimal itemRowStock = Convert.ToDecimal(row.Cells[6].Text);
                                                        string itemBatchNo = row.Cells[8].Text;
                                                        string itemExpiryDate = row.Cells[9].Text;
                                                        string description = txtCustom.Text;

                                                        List<ItemStoreData> response = dal.DataStoreinRack();
                                                        List<ItemStoreData> filteredData1 = response.Where(item => item.IS_ItemStoreId == ISItemStoreId).ToList();

                                                        if (filteredData1 != null && filteredData1.Count > 0)
                                                        {
                                                            ItemStoreData selectedItem = filteredData1[0];
                                                            decimal StoreQty = selectedItem.IS_StoreQty;
                                                            decimal remainingIsStoreQty = StoreQty - ItemQtySend;
                                                            int mtID = Convert.ToInt32(ViewState["MT_ID"].ToString());

                                                            if (remainingIsStoreQty >= 0)
                                                            {
                                                                string Query = @"UPDATE ItemStore SET IS_StoreQty = @IS_StoreQty WHERE IS_ItemStoreId = @IS_ItemStoreId";
                                                                using (SqlCommand cmd1 = new SqlCommand(Query, con))
                                                                {
                                                                    cmd1.Parameters.AddWithValue("@IS_ItemStoreId", ISItemStoreId);
                                                                    cmd1.Parameters.AddWithValue("@IS_StoreQty", remainingIsStoreQty);
                                                                    con.Open();
                                                                    int I = cmd1.ExecuteNonQuery();
                                                                    con.Close();
                                                                    if (I > 0)
                                                                    {
                                                                        var TableRowId3 = ISItemStoreId.ToString();
                                                                        bool success3 = dal.AuditLogData("ItemStore Stock Update when Material Release", "ItemStore", TableRowId3, "");
                                                                        if (success3)
                                                                        { }
                                                                        else
                                                                        {
                                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                        }

                                                                        con.Open();
                                                                        int prevIdentity1 = dal.GetReseedIdentity("MaterialTransationSubData", "MTS_ID");
                                                                        dal.ReseedIdentity("MaterialTransationSubData", prevIdentity1);
                                                                        con.Close();

                                                                        string insertQuery1 = @"INSERT INTO MaterialTransationSubData
                                                                                        (MTS_MTID, MTS_ItemStock, MTS_BatchNo, MTS_ExpiryDate, MTS_CreatedDateTime, MTS_Description) 
                                                                                        output INSERTED.MTS_ID values  
                                                                                        (@MTS_MTID, @MTS_ItemStock, @MTS_BatchNo, @MTS_ExpiryDate, @MTS_CreatedDateTime, @MTS_Description)";

                                                                        using (SqlCommand cmd = new SqlCommand(insertQuery1, con))
                                                                        {
                                                                            cmd.Parameters.AddWithValue("@MTS_MTID", mtID);
                                                                            cmd.Parameters.AddWithValue("@MTS_ItemStock", ItemQtySend);
                                                                            cmd.Parameters.AddWithValue("@MTS_BatchNo", itemBatchNo);
                                                                            cmd.Parameters.AddWithValue("@MTS_ExpiryDate", itemExpiryDate);
                                                                            cmd.Parameters.AddWithValue("@MTS_Description", description);
                                                                            cmd.Parameters.AddWithValue("@MTS_CreatedDateTime", GetDateTime);

                                                                            con.Open();
                                                                            int getIDs = (int)cmd.ExecuteScalar();
                                                                            con.Close();

                                                                            if (getIDs > 0)
                                                                            {
                                                                                var TableRowId4 = getIDs.ToString();
                                                                                bool success4 = dal.AuditLogData("Material Transation-Subdata ADD When Material Release", "MaterialTransationSubData", TableRowId4, "");
                                                                                if (success4)
                                                                                { }
                                                                                else
                                                                                {
                                                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                                }

                                                                                c2 = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialTransactionMaster Successfully Insert but MaterialRequestDetails not Update');</script>");
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ItemQtySend = ItemQtySend - StoreQty;
                                                                string Query = @"UPDATE ItemStore SET IS_StoreQty = @IS_StoreQty WHERE IS_ItemStoreId = @IS_ItemStoreId";
                                                                using (SqlCommand cmd1 = new SqlCommand(Query, con))
                                                                {
                                                                    cmd1.Parameters.AddWithValue("@IS_ItemStoreId", ISItemStoreId);
                                                                    cmd1.Parameters.AddWithValue("@IS_StoreQty", 0);

                                                                    con.Open();
                                                                    int I = cmd1.ExecuteNonQuery();
                                                                    con.Close();
                                                                    if (I > 0)
                                                                    {
                                                                        var TableRowId3 = ISItemStoreId.ToString();
                                                                        bool success3 = dal.AuditLogData("ItemStore Stock Update", "ItemStore", TableRowId3, "");
                                                                        if (success3)
                                                                        { }
                                                                        else
                                                                        {
                                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                        }

                                                                        con.Open();
                                                                        int prevIdentity1 = dal.GetReseedIdentity("MaterialTransationSubData", "MTS_ID");
                                                                        dal.ReseedIdentity("MaterialTransationSubData", prevIdentity1);
                                                                        con.Close();

                                                                        string insertQuery1 = @"INSERT INTO MaterialTransationSubData
                                                                                            (MTS_MTID, MTS_ItemStock, MTS_BatchNo, MTS_ExpiryDate, MTS_CreatedDateTime, MTS_Description) 
                                                                                            output INSERTED.MTS_ID values  
                                                                                            (@MTS_MTID, @MTS_ItemStock, @MTS_BatchNo, @MTS_ExpiryDate, @MTS_CreatedDateTime, @MTS_Description)";

                                                                        using (SqlCommand cmd = new SqlCommand(insertQuery1, con))
                                                                        {
                                                                            cmd.Parameters.AddWithValue("@MTS_MTID", mtID);
                                                                            cmd.Parameters.AddWithValue("@MTS_ItemStock", itemRowStock);
                                                                            //cmd.Parameters.AddWithValue("@MTS_ItemStock", ItemQtySend);//
                                                                            cmd.Parameters.AddWithValue("@MTS_BatchNo", itemBatchNo);
                                                                            cmd.Parameters.AddWithValue("@MTS_ExpiryDate", itemExpiryDate);
                                                                            cmd.Parameters.AddWithValue("@MTS_Description", description);
                                                                            cmd.Parameters.AddWithValue("@MTS_CreatedDateTime", GetDateTime);
                                                                            con.Open();
                                                                            int getIDs = (int)cmd.ExecuteScalar();
                                                                            con.Close();
                                                                            if (getIDs > 0)
                                                                            {
                                                                                var TableRowId4 = getIDs.ToString();
                                                                                bool success4 = dal.AuditLogData("Material Transation-Subdata ADD", "MaterialTransationSubData", TableRowId4, "");
                                                                                if (success4)
                                                                                { }
                                                                                else
                                                                                {
                                                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                                                }
                                                                                c2 = true;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('MaterialTransactionMaster Successfully Insert but MaterialRequestDetails not Update');</script>");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        int ISItemStoreId = Convert.ToInt32(GridView3.DataKeys[row.RowIndex]["IS_ItemStoreId"]);
                                                        string itemCode = row.Cells[1].Text;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('some thing wrong GridView Not Get');", true);
                                        }

                                    }

                                    if (c2 == true)
                                    {
                                        //bool notificationSent = dal.NotificationSend("Store Department", "MR Successfully Released", "MR No: " + ViewState["MRM_NUMBER"].ToString() + " -Released successful, you can now proceed.", ViewState["NotificationToEmployeeId"].ToString());
                                        //if (notificationSent)
                                        //{
                                        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "SuccessScript", "<script> alert('Successfully Approve'); setTimeout(function() { window.location = 'MRShowforApproval.aspx'; }, 100); </script>");
                                        //}
                                        //else
                                        //{
                                        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully But Notification not Send');</script>");
                                        //}

                                        Response.Redirect(Request.RawUrl);
                                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Save Successfully'); setTimeout(function() { window.location = 'MaterialRequestFormShow.aspx'; }, 100);</script>");
                                    }
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Material Request Data not getting'); </script>");
                                }
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Stock Not enough'); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
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

        }




        /* usable*/
        [WebMethod]
        public static string SearchData(string transactionID)
        {
            DataAccessLayer dal = new DataAccessLayer();
            List<ItemTransactionData> filteredData2 = dal.ItemTransactionMasterDataGetforQRCode(transactionID);
            if (filteredData2 != null && filteredData2.Count > 0)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonData = serializer.Serialize(filteredData2);
                return jsonData;
            }
            else
            {
                return "{\"alert\": \"No Records Found.\"}";
            }
        }


    }
}