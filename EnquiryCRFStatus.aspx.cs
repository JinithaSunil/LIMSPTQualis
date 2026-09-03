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
    public partial class EnquiryCRFStatus : System.Web.UI.Page
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
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());                   
                }

                if (!Page.IsPostBack)
                {
                    BindEnquiryList();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        protected void BindEnquiryList()
        {
            string DepartmentID = Session["DepartmentID"].ToString();
            DataTable dt = _cm.getEnquiryCRFStatus(DepartmentID);
            gvEnquiryList.DataSource = dt;
            gvEnquiryList.DataBind();
            ViewState["dirState"] = dt;
            ViewState["sortdr"] = "Asc";
        }
        protected string Limit(object desc, int maxLength)
        {
            var description = (string)desc;
            if (string.IsNullOrEmpty(description)) { return description; }
            return description.Length <= maxLength ?
                description : description.Substring(0, maxLength) + "...";
        }
        protected bool SetVisibility(object desc, int maxLength)
        {
            var description = (string)desc;
            if (string.IsNullOrEmpty(description)) { return false; }
            return description.Length > maxLength;
        }
        protected void ReadMoreLinkButton_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            GridViewRow row = button.NamingContainer as GridViewRow;
            Label descLabel = row.FindControl("lblInquiryDetail") as Label;
            button.Text = (button.Text == "Read More") ? "Hide" : "Read More";
            string temp = descLabel.Text;
            descLabel.Text = descLabel.ToolTip;
            descLabel.ToolTip = temp;
        }

        protected void gvEnquiryList_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvEnquiryList.DataSource = dtrslt;
                gvEnquiryList.DataBind();
            }
        }

        protected void gvEnquiryList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEnquiryList.EditIndex = e.NewEditIndex;
            this.BindEnquiryList();
        }

        protected void gvEnquiryList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvEnquiryList.EditIndex == e.Row.RowIndex)
            {
                DropDownList ddlDepartment = (DropDownList)e.Row.FindControl("ddlDepartment");
                DataTable dt = _cm.GetAllDepartments("LIMS",1);
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                string selectedDept = DataBinder.Eval(e.Row.DataItem, "DepartmentName").ToString();
                ddlDepartment.Items.FindByText(selectedDept).Selected = true;

                DropDownList ddlSubGroupName = (DropDownList)e.Row.FindControl("ddlSubGroupName");
                DataTable dt2 = _cm.getProductCategories();
                ddlSubGroupName.DataSource = dt2;
                ddlSubGroupName.DataTextField = "SubGroupName";
                ddlSubGroupName.DataValueField = "SubGroupID";
                ddlSubGroupName.DataBind();
                string selectedSubGroup = DataBinder.Eval(e.Row.DataItem, "SubGroupName").ToString();
                ddlSubGroupName.Items.FindByText(selectedSubGroup).Selected = true;
            }
        }

        protected void gvEnquiryList_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string Department = (gvEnquiryList.Rows[e.RowIndex].FindControl("ddlDepartment") as DropDownList).SelectedItem.Value;
            string SubGroupName = (gvEnquiryList.Rows[e.RowIndex].FindControl("ddlSubGroupName") as DropDownList).SelectedItem.Value;
            string InquiryID = gvEnquiryList.DataKeys[e.RowIndex].Value.ToString();
            _cm.UpdateEnquiry(InquiryID, Department, SubGroupName);
            gvEnquiryList.EditIndex = -1;
            this.BindEnquiryList();
        }

        protected void gvEnquiryList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEnquiryList.EditIndex = -1;
            this.BindEnquiryList();
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlDepartment = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlDepartment.NamingContainer;
            DropDownList ddlSubGroupName = (DropDownList)row.FindControl("ddlSubGroupName");
            ddlSubGroupName.DataSource = _cm.getProductCategories(ddlDepartment.SelectedValue);
            ddlSubGroupName.DataBind();

        }
    }
}