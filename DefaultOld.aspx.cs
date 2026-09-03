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
    public partial class DefaultOld : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome = (HtmlGenericControl)this.Master.FindControl("liHome1");
            liHome.Attributes.Add("class", "active");
            int clientId = 1;
            Session["CustomerId"] = clientId;
            DataTable[] dt = new DataTable[4];
            dt = _cm.GetCustomerInquiry();
            divNewInquiries.InnerHtml = dt[0].Rows.Count.ToString();
            divNewCustomers.InnerHtml = dt[1].Rows.Count.ToString();
            divAssigned.InnerHtml = dt[2].Rows.Count.ToString();
            divCompleted.InnerHtml = dt[3].Rows.Count.ToString();
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
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
            
        }
        protected void btnToday_Click(object sender, EventArgs e)
        {
            hdPeriod.Value = "today";
            divPeriod.InnerHtml = "Today's Status";
            DataTable[] dt = new DataTable[4];
            dt = _cm.GetStatus("today");
            if (dt[0].Rows.Count > 0)
            {
                divNewInquiries.InnerHtml = dt[0].Rows.Count.ToString();
            }
            else
            {
                divNewInquiries.InnerHtml = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                divNewCustomers.InnerHtml = dt[1].Rows.Count.ToString();
            }
            else
            {
                divNewCustomers.InnerHtml = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                divAssigned.InnerHtml = dt[2].Rows.Count.ToString();
            }
            else
            {
                divAssigned.InnerHtml = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                divCompleted.InnerHtml = dt[3].Rows.Count.ToString();
            }
            else
            {
                divCompleted.InnerHtml = "";
            }
        }

        protected void btnWeek_Click(object sender, EventArgs e)
        {
            hdPeriod.Value = "today";
            divPeriod.InnerHtml = "Week's Status";
            DataTable[] dt = new DataTable[4];
            dt = _cm.GetStatus("week");
            if (dt[0].Rows.Count > 0)
            {
                divNewInquiries.InnerHtml = dt[0].Rows.Count.ToString();
            }
            else
            {
                divNewInquiries.InnerHtml = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                divNewCustomers.InnerHtml = dt[1].Rows.Count.ToString();
            }
            else
            {
                divNewCustomers.InnerHtml = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                divAssigned.InnerHtml = dt[2].Rows.Count.ToString();
            }
            else
            {
                divAssigned.InnerHtml = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                divCompleted.InnerHtml = dt[3].Rows.Count.ToString();
            }
            else
            {
                divCompleted.InnerHtml = "";
            }
        }

        protected void btnMonth_Click(object sender, EventArgs e)
        {
            hdPeriod.Value = "today";
            divPeriod.InnerHtml = "Month's Status";
            DataTable[] dt = new DataTable[4];
            dt = _cm.GetStatus("month");
            if (dt[0].Rows.Count > 0)
            {
                divNewInquiries.InnerHtml = dt[0].Rows.Count.ToString();
            }
            else
            {
                divNewInquiries.InnerHtml = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                divNewCustomers.InnerHtml = dt[1].Rows.Count.ToString();
            }
            else
            {
                divNewCustomers.InnerHtml = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                divAssigned.InnerHtml = dt[2].Rows.Count.ToString();
            }
            else
            {
                divAssigned.InnerHtml = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                divCompleted.InnerHtml = dt[3].Rows.Count.ToString();
            }
            else
            {
                divCompleted.InnerHtml = "";
            }
        }

    }
}