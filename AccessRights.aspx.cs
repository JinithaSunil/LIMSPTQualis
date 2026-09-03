using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class AccessRights : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
               // BindRoles();
            }
        }
        protected void BindRoles()
        {
            DataTable dtRoles = _cm.getAllRoles();
            ddlRoles.DataSource = dtRoles;
            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleID";
            ddlRoles.DataBind();
        }
        protected void ddlRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int RoleID = Convert.ToInt32(ddlRoles.SelectedValue);
            DataTable dtMenuList = _cm.getRoleMenuList(RoleID);
            string MenuList = "";
            
            if (dtMenuList.Rows.Count > 0)
            {
                MenuList = dtMenuList.Rows[0]["MenuIDList"].ToString();
                DataTable dtMenuItems = _cm.getMenuItems(MenuList);
                
                foreach(DataRow dr in dtMenuItems.Rows)
                {
                    if (dr["MenuItem"].ToString() == "liHome1 ") { HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1"); liHome1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liCustomers1") { HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1"); liCustomers1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liInquiries1") { HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1"); liInquiries1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liCRFList1 ") { HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1 "); liCRFList1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liSampleManagement1 ") { HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1"); liSampleManagement1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liTestManagement1") { HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1"); liTestManagement1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liQualityManagement1") { HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1"); liQualityManagement1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liCustomerFeedback1 ") { HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1 "); liCustomerFeedback1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liUserManagement1") { HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1"); liUserManagement1.Attributes.Add("class", "active"); }
                    if (dr["MenuItem"].ToString() == "liSupport1") { HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1"); liSupport1.Attributes.Add("class", "active"); }
                }
            }

        }
    }
}