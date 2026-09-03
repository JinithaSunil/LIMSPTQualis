using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    
    public partial class Organization : System.Web.UI.Page
    {
        DAL.OrgTree orgTree = new DAL.OrgTree();       

        protected void Page_Load(object sender, EventArgs e)
        {
           if (Session["Usertype"] != null)
            {                
                HtmlControl liOrganization2 = (HtmlGenericControl)this.Master.FindControl("liOrganization2");

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
                if (!Page.IsPostBack) 
                { 

                    //Assing output of the CreateTreeHtml function to Div
                    abc.InnerHtml = orgTree.CreateTreeHtml();
                }
                
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        
        
    }
}