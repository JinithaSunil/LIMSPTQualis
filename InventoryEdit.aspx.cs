using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class InventoryEdit : System.Web.UI.Page
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

                    txtCurrentStock.Enabled = false;
                    txtHSNCode.Enabled = false;
                    ddlItemCode.Enabled = false;
                    txtItemName.Enabled = false;
                    ddlUOM.Enabled = false;
                    ddlVAT.Enabled = false;

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

                    List<Item> res = dal.GetItemList();
                    ddlItemCode.DataSource = res;
                    ddlItemCode.DataTextField = "ItemCode";
                    ddlItemCode.DataValueField = "ItemId";
                    ddlItemCode.DataBind();
                    ddlItemCode.Items.Insert(0, new ListItem("Select", ""));
                    rfvItemCode.InitialValue = "";

                    if (Request.QueryString["PageID"] != null)
                    {
                        string eventID = Request.QueryString["PageID"];
                        string encodedString = eventID;
                        byte[] decodedBytes = Convert.FromBase64String(encodedString);
                        string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);

                        lblPageID.Text = decodedEventListId;
                        lblPageID.Visible = false;


                        List<InventoryItemData> dataGet = dal.ActiveInventoryDetailsGet();
                        List<InventoryItemData> filteredData = dataGet.Where(item => item.InventoryId == Convert.ToInt32(lblPageID.Text)).ToList();

                        if (filteredData != null && filteredData.Count > 0)
                        {
                            InventoryItemData selectedItem = filteredData[0];
                            ddlRackType.SelectedValue = selectedItem.InventoryRackTypeId.ToString();
                            ddlItemCode.SelectedValue = selectedItem.InventoryItemId.ToString();
                            ddlUOM.SelectedValue = selectedItem.InventoryUOMId.ToString();
                            ddlVAT.SelectedValue = selectedItem.InventoryGSTId.ToString();
                            txtItemName.Text = selectedItem.ItemName.ToString();
                            txtHSNCode.Text = selectedItem.HSNCode.ToString();
                            txtCurrentStock.Text = selectedItem.InventoryCurrentStock.ToString();
                            txtMinStock.Text = selectedItem.InventoryMinStock.ToString();
                            ddlRackType_SelectedIndexChanged(sender, e);

                            if (Convert.ToDecimal(txtCurrentStock.Text) > 0)
                            {
                                foreach (ListItem item in chkRowSeries.Items)
                                {
                                    string[] getData = selectedItem.InventoryItemLocationStore.Split(',');

                                    foreach (string DataShow in getData)
                                    {
                                        if (DataShow.Trim() == item.Value)
                                        {
                                            item.Selected = true;
                                            item.Enabled = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (ListItem item in chkRowSeries.Items)
                                {
                                    string[] getData = selectedItem.InventoryItemLocationStore.Split(',');

                                    foreach (string DataShow in getData)
                                    {
                                        if (DataShow.Trim() == item.Value)
                                        {
                                            item.Selected = true;
                                            item.Enabled = true;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
                        }
                    }



                }
            }
        }


        protected void ddlRackType_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); </script>");
            }
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
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

                if (!string.IsNullOrEmpty(chkboxSelect))
                {
                    bool IsItemCodeExist = dal.ItemInventoryExist(ddlRackType.SelectedItem.Value, selectedRackNo);

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        string existingRackNos = string.Empty;

                        if (IsItemCodeExist)
                        {
                            // Get existing rack numbers from the database if the item code already exists.
                            string query = @"SELECT InventoryItemLocationStore FROM InventoryMaster 
                                             WHERE InventoryRackTypeId = @InventoryRackTypeId AND InventoryId = @InventoryId";

                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@InventoryRackTypeId", ddlRackType.SelectedItem.Value);
                            cmd.Parameters.AddWithValue("@InventoryId", lblPageID.Text);

                            con.Open();
                            existingRackNos = cmd.ExecuteScalar()?.ToString();
                            con.Close();
                        }

                        // Now we process the rack numbers to remove any deselected ones.
                        List<string> newRackNos = new List<string>(selectedRackNo.Split(','));

                        if (!string.IsNullOrEmpty(existingRackNos))
                        {
                            List<string> existingRackList = new List<string>(existingRackNos.Split(','));

                            // Remove deselected racks from the existing list
                            existingRackList.RemoveAll(r => !newRackNos.Contains(r));

                            // Add the newly selected racks that are not already present
                            foreach (var rack in newRackNos)
                            {
                                if (!existingRackList.Contains(rack))
                                {
                                    existingRackList.Add(rack);
                                }
                            }

                            // Now, existingRackList contains only the racks that should be in InventoryItemLocationStore
                            existingRackNos = string.Join(",", existingRackList);
                        }
                        else
                        {
                            // If no existing racks, just use the new racks
                            existingRackNos = string.Join(",", newRackNos);
                        }

                        string insertQuery = @"UPDATE InventoryMaster set
                                                InventoryRackTypeId = @InventoryRackTypeId,                                                
                                                InventoryCurrentStock = @InventoryCurrentStock,
                                                InventoryMinStock = @InventoryMinStock,
                                                InventoryCreatedEmployeeId = @InventoryCreatedEmployeeId,
                                                InventoryMasterCompanyId = @InventoryMasterCompanyId,
                                                InventoryItemLocationStore = @InventoryItemLocationStore,
                                                InventoryUpdatedDateTime = @InventoryUpdatedDateTime
                                                WHERE InventoryId = @InventoryId";

                        SqlCommand cmdUpdate = new SqlCommand(insertQuery, con);
                        cmdUpdate.Parameters.AddWithValue("@InventoryId", lblPageID.Text);
                        cmdUpdate.Parameters.AddWithValue("@InventoryRackTypeId", ddlRackType.SelectedItem.Value);
                        cmdUpdate.Parameters.AddWithValue("@InventoryCurrentStock", Convert.ToDecimal(txtCurrentStock.Text));
                        cmdUpdate.Parameters.AddWithValue("@InventoryMinStock", Convert.ToDecimal(txtMinStock.Text));
                        cmdUpdate.Parameters.AddWithValue("@InventoryMasterCompanyId", Session["MasterCompanyId"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@InventoryCreatedEmployeeId", Session["EmployeeID"].ToString());
                        cmdUpdate.Parameters.AddWithValue("@InventoryItemLocationStore", existingRackNos);
                        cmdUpdate.Parameters.AddWithValue("@InventoryUpdatedDateTime", GetDateTime);

                        con.Open();
                        int i = cmdUpdate.ExecuteNonQuery();
                        con.Close();

                        if (i > 0)
                        {
                            var TableRowId = lblPageID.Text;
                            bool success = dal.AuditLogData("Inventory Edit", "InventoryMaster", TableRowId, "");
                            if (!success)
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                            }

                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Update Successfully'); window.location.href='InventoryShow.aspx'; </script>");
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something went wrong, try later!'); </script>");
                        }
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Please select at least 1!'); </script>");
                }


            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;

                bool success = dal.ErrorLogData(methodName, ex.Message, "InventoryEdit.aspx.cs", linenum.ToString());
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

    }
}