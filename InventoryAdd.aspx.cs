using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class InventoryAdd : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");

        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();
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
                    ddlVAT.Enabled = false;
                    txtItemName.Enabled = false;
                    txtHSNCode.Enabled = false;

                    List<UOMData> uomData = dal.UOMDetailsGet();
                    ddlUOM.DataSource = uomData;
                    ddlUOM.DataTextField = "UOMCode";
                    ddlUOM.DataValueField = "UOMId";
                    ddlUOM.DataBind();
                    ddlUOM.Items.Insert(0, new ListItem("Select", ""));

                    DataTable dt = _cm.getVAT();
                    if(dt.Rows.Count > 0){
                        ddlVAT.DataSource = dt;
                        ddlVAT.DataTextField = "VATCode";
                        ddlVAT.DataValueField = "VATId";
                        ddlVAT.DataBind();
                        ddlVAT.Items.Insert(0, new ListItem("Select", ""));
                    }
                   

                    List<RackTypeDetailsData> response1 = dal.RackTypeDetailsUnitWiseDisplay();
                    ddlRackType.DataSource = response1;
                    ddlRackType.DataTextField = "RackType";
                    ddlRackType.DataValueField = "RackTypeId";
                    ddlRackType.DataBind();
                    ddlRackType.Items.Insert(0, new ListItem("Select", ""));
                    rfvRackType.InitialValue = "";

                    List<PurchaseGroupData> response2 = dal.GetAllPurchaseGroups();
                    ddlCategory.DataSource = response2;
                    ddlCategory.DataTextField = "PurchaseGroupName";
                    ddlCategory.DataValueField = "PurchaseGroupID";
                    ddlCategory.DataBind();
                    ddlCategory.Items.Insert(0, new ListItem("Select", ""));
                    rfvddlCategory.InitialValue = "";
                }
            }
        }

        protected void ddlRackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedUnitId;
            int selectedRackTypeId;

            if (int.TryParse(ddlRackType.SelectedValue, out selectedRackTypeId))
            {
                BindRackRowSeriesNumbers(selectedRackTypeId);
            }
        }

        private void BindRackRowSeriesNumbers(int selectedRackTypeId)
        {
            List<RackNumberData> allRackNumbers = dal.AllRackRowSerialNumberDisplay();
            List<RackNumberData> filteredRackNumbers = allRackNumbers.Where(item => item.RackTypeId == selectedRackTypeId).ToList();

            chkRowSeries.DataSource = filteredRackNumbers;
            chkRowSeries.DataTextField = "RackSerialNumber";
            chkRowSeries.DataValueField = "RackSerialId";
            chkRowSeries.DataBind();

            List<InventoryItemData> response = dal.ActiveInventoryDetailsGet();
            List<InventoryItemData> filteredData1 = response.Where(item => item.InventoryRackTypeId == Convert.ToInt32(ddlRackType.SelectedItem.Value)).ToList();

            if (filteredData1 != null && filteredData1.Count > 0)
            {
                InventoryItemData selectedItem = filteredData1[0];

                List<string> allRackSerialLocations = new List<string>();

                foreach (var items in filteredData1)
                {
                    allRackSerialLocations.Add(items.InventoryItemLocationStore.ToString());
                }

                foreach (ListItem item in chkRowSeries.Items)
                {
                    bool found = false;

                    foreach (string rackSerialLocation in allRackSerialLocations)
                    {
                        string[] getData = rackSerialLocation.Split(',');

                        if (getData.Contains(item.Value.Trim()))
                        {
                            found = true;
                            break;
                        }
                    }

                    item.Enabled = !found;
                }
            }
            else
            {
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('RackType is Fully Empty.'); </script>");                
            }

        }

        protected void ddlItemCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlItemCode.SelectedItem != null && !string.IsNullOrEmpty(ddlItemCode.SelectedItem.Value))
            {
                int getItemId = Convert.ToInt32(ddlItemCode.SelectedItem.Value);

                List<Item> res = dal.GetItemList();
                List<Item> filteredData = res.Where(item => item.ItemID == getItemId).ToList();
                if (filteredData != null && filteredData.Count > 0)
                {
                    Item selectedItem = filteredData[0];
                    //ddlVendor.SelectedValue = selectedItem.ItemVendorId.ToString();
                    txtItemName.Text = selectedItem.ItemName.ToString();
                    txtHSNCode.Text = selectedItem.HSNCode.ToString();
                    int vatID = Convert.ToInt32(selectedItem.VATID.ToString());
                    if (vatID == 1)
                    {
                        ddlVAT.SelectedValue = vatID.ToString();
                    }
                    else
                    {
                        ddlVAT.SelectedValue = "";
                    }

                    ddlUOM.SelectedValue = selectedItem.UOMID.ToString();
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Select Item Code.'); </script>");
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedItem != null && !string.IsNullOrEmpty(ddlCategory.SelectedItem.Value))
            {
                int getItemCategoryId = Convert.ToInt32(ddlCategory.SelectedItem.Value);

                List<PurchaseSubGroupData> res = dal.GetAllPurchaseSubGroups();
                List<PurchaseSubGroupData> filteredData = res.Where(item => item.PurchaseGroupID == getItemCategoryId).ToList();
                ddlSubCategory.DataSource = filteredData;
                ddlSubCategory.DataTextField = "PurchaseSubGroupName";
                ddlSubCategory.DataValueField = "PurchaseSubGroupID";
                ddlSubCategory.DataBind();
                ddlSubCategory.Items.Insert(0, new ListItem("Select", ""));
                rfvItemCode.InitialValue = "";
            }
            else
            {
                ddlSubCategory.Items.Clear();
                ddlSubCategory.Items.Insert(0, new ListItem("Select", ""));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Select Category.'); </script>");
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int ItemId = Convert.ToInt32(ddlItemCode.SelectedItem.Value);
                //int UnitId = Convert.ToInt32(ddlBranchUnit.SelectedItem.Value);

                bool IsInventoryExist = dal.ItemInventoryExistUnitWise(ItemId);
                if (IsInventoryExist)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Already Exist.'); </script>");
                }
                else
                {
                    string chkboxSelect = "";
                    for (int i = 0; i < chkRowSeries.Items.Count; i++)
                    {
                        if (chkRowSeries.Items[i].Selected)
                        {
                            if (chkboxSelect == "")
                            {
                                chkboxSelect = chkRowSeries.Items[i].Value;
                            }
                            else
                            {
                                chkboxSelect += "," + chkRowSeries.Items[i].Value;
                            }
                        }
                    }

                    string selectedRackNo = chkboxSelect;

                    bool IsItemCodeExist = dal.ItemInventoryExist(ddlRackType.SelectedItem.Value, selectedRackNo);

                    if (!IsItemCodeExist)
                    {
                        con.Open();
                        int prevIdentity = dal.GetReseedIdentity("InventoryMaster", "InventoryId");
                        dal.ReseedIdentity("InventoryMaster", prevIdentity);
                        con.Close();

                        string query = @"insert into InventoryMaster 
                                        (InventoryRackTypeId, InventoryItemCategoryId,InventoryItemSubCategoryId, InventoryItemId, InventoryUOMId, InventoryGSTId, 
                                        InventoryCurrentStock, InventoryMinStock, InventoryCreatedEmployeeId, InventoryMasterCompanyId, 
                                        InventoryItemLocationStore, InventoryCreatedDateTime, InventoryUpdatedDateTime)
                                        output INSERTED.InventoryId values 
                                        (@InventoryRackTypeId, @InventoryItemCategoryId,@InventoryItemSubCategoryId, @InventoryItemId, @InventoryUOMId, @InventoryGSTId, 
                                        @InventoryCurrentStock, @InventoryMinStock, @InventoryCreatedEmployeeId, @InventoryMasterCompanyId, 
                                        @InventoryItemLocationStore, @InventoryCreatedDateTime, @InventoryUpdatedDateTime)";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            //cmd.Parameters.AddWithValue("@InventoryCompanyUnitId", ddlBranchUnit.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryRackTypeId", ddlRackType.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryItemId", ddlItemCode.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryItemCategoryId", ddlCategory.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryItemSubCategoryId", ddlSubCategory.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryUOMId", ddlUOM.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryGSTId", ddlVAT.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryCurrentStock", Convert.ToDecimal(txtCurrentStock.Text));
                            cmd.Parameters.AddWithValue("@InventoryMinStock", Convert.ToDecimal(txtMinStock.Text));
                            cmd.Parameters.AddWithValue("@InventoryMasterCompanyId", Session["MasterCompanyId"].ToString());
                            cmd.Parameters.AddWithValue("@InventoryCreatedEmployeeId", Session["EmployeeID"].ToString());
                            cmd.Parameters.AddWithValue("@InventoryItemLocationStore", chkboxSelect);
                            cmd.Parameters.AddWithValue("@InventoryCreatedDateTime", GetDateTime);
                            cmd.Parameters.AddWithValue("@InventoryUpdatedDateTime", GetDateTime);
                            con.Open();
                            /*int i = cmd.ExecuteNonQuery();*/
                            int InsertGetId = (int)cmd.ExecuteScalar();
                            con.Close();
                            if (InsertGetId > 0)
                            {
                                var TableRowId = InsertGetId.ToString();
                                bool success = dal.AuditLogData("Inventory Add", "InventoryMaster", TableRowId, "");
                                if (success)
                                { }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                }
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'InventoryAdd.aspx'; }, 100);</script>");
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                            }
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Rack hasbeen Allocated!');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
                bool success = dal.ErrorLogData(methodName, ex.Message, "InventoryAdd.aspx.cs", linenum.ToString());
                if (success)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Save: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
            }
        }

        protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedItem != null && !string.IsNullOrEmpty(ddlCategory.SelectedItem.Value))
            {
                int getItemCategoryId = Convert.ToInt32(ddlCategory.SelectedItem.Value);

                List<Item> res = dal.GetItemList();
                List<Item> filteredData = res.Where(item => item.PurchaseGroupID == getItemCategoryId).ToList();
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


    }
}