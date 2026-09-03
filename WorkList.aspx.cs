using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;

namespace LIMS
{
    public partial class WorkList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int EmployeeID = 0; int roleID = 0;int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)        
        {

            if (Session["Usertype"] != null)
            {
                EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());

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
                if (Session["DepartmentID"] != null)
                {
                    int DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                    if (DepartmentID == 1)
                    {
                        string url = "WorkList_Transport.aspx";
                        Response.Redirect(url);
                    }
                    if (DepartmentID == 2)
                    {
                        string url = "WorkList_Battery.aspx";
                        Response.Redirect(url);
                    }
                    if (DepartmentID == 4006)
                    {
                        string url = "WorkList_PV.aspx";
                        Response.Redirect(url);
                    }
                    if (DepartmentID == 4007)
                    {
                        string url = "WorkList_mechanical.aspx";
                        Response.Redirect(url);
                    }
                    if (DepartmentID == 4008)
                    {
                        string url = "WorkList_HomeAppliances.aspx";
                        Response.Redirect(url);
                    }
                    if (DepartmentID == 4009)
                    {
                        Response.Redirect("WorkList_LED.aspx");
                    }
                    if (DepartmentID == 3006)
                    {
                        Response.Redirect("WorkList_ElectricalAccessories.aspx");
                    }
                    if (DepartmentID == 5020)
	                {
	                    Response.Redirect("WorkList_PhysicalSoftline.aspx");
	                }

                    if (!Page.IsPostBack)
                    {
                        BindYear();
                        //liTestManagement1.Attributes.Add("class", "active");
                        //liWorkList2.Attributes.Add("class", "active");
                        string year = DateTime.Now.Year.ToString();
                        ddlYear.Items.FindByValue(year).Selected = true;
                        string month = DateTime.Now.Month.ToString();
                        ddlMonth.Items.FindByValue(month).Selected = true;
                        BindWorkList();
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }            
        }

        private void BindMessages(string SampleId)
        {
            string Date;
            string Name;
            string Message;
            txtMsg.Text = "";
            DataTable dt = new DataTable();
            dt = _cm.GetChatDetails(SampleId);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Date = dt.Rows[i]["Date"].ToString();
                    Name = dt.Rows[i]["EmployeeName"].ToString();
                    Message = dt.Rows[i]["Message"].ToString();
                    txtMsg.Text += Date + "   " + Name + "::" + Message + Environment.NewLine;
                }
            }       
        }
        protected void BindYear()
        {
            DataTable dt = new DataTable();
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                years.Add(year);
            }

            // Bind the dropdownlist
            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, "Select Year");
        }
        private void BindWorkList()
        {
            DataTable dt = new DataTable();            
            DataTable dt2 = _cm.getRoleDetails(EmployeeID);
            
            if (dt2.Rows.Count > 0)
            {
                roleID = Convert.ToInt32(dt2.Rows[0]["roleID"].ToString());
                if (ddlYear.SelectedIndex != 0)
                {
                    int year = Convert.ToInt32(ddlYear.SelectedValue);
                    int month = Convert.ToInt32(ddlMonth.SelectedValue);
                    switch (roleID)
                    {
                        case 1://QC
                            break;
                        case 2://Manager
                            dt = _cm.GetAllocation(EmployeeID, roleID,year, month);
                            gvWorkList.DataSource = dt;
                            gvWorkList.DataBind();
                            break;
                        case 3://Lab Tech
                            dt = _cm.GetAllocation(EmployeeID, roleID, year, month);
                            gvWorkList.DataSource = dt;
                            gvWorkList.DataBind();
                            break;
                        case 4://Admin
                            break;
                        default: break;

                    }
                }
            }
            
        }

        protected void gvWorkList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int index = e.Row.RowIndex;int sampleUID = 0;
                int sampleid = int.Parse(gvWorkList.DataKeys[index]["ID"].ToString());

                HyperLink hlAllocation = (e.Row.FindControl("hlAllocation") as HyperLink);
                Button btnTestResult = (e.Row.FindControl("btnTestResult") as Button);

              
                DataTable dt1 = new DataTable();
                DataTable dt2 = _cm.getRoleDetails(EmployeeID);

                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                
                roleID = Convert.ToInt32(dt2.Rows[0]["roleID"].ToString());

                switch (lblStatusName.Text)
                {
                    case "Allocated For Testing":
                        
                        dt1 = _cm.getSampleUID(sampleid);
                        if (dt1.Rows.Count > 0)
                        {
                            sampleUID = Convert.ToInt32(dt1.Rows[0]["sampleID"].ToString());
                        }
                        Session["SampleId"] = sampleUID.ToString();
                        hlAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                        switch (roleID)
                        {
                            case 1: // QC
                                //btnTestResult.Visible = false;
                                break;
                            case 2: // Manager
                                //btnTestResult.Visible = false;
                                break;
                            case 3: // Lab Tech
                                //btnTestResult.Visible = true;
                                break;
                            default: // Admin
                                //btnTestResult.Visible = false;
                                break;
                        }
                        break;
                    case "Test Report Submitted for Review":
                        break;
                    default:
                        hlAllocation.Visible = false;
                        break;
                }
            }
        }
        void Popup(bool isDisplay)
        {
            StringBuilder builder = new StringBuilder();
            if (isDisplay)
            {
                builder.Append("<script language=JavaScript> showModal(); </script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPopup", builder.ToString());
            }
            else
            {
                builder.Append("<script language=JavaScript> HidePopup(); </script>\n");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "HidePopup", builder.ToString());
            }
        }
        protected void gvWorkList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = 0; int sampleID = 0;
            int DepartmentId = Convert.ToInt32(Session["DepartmentId"].ToString());

            if (e.CommandName == "Chat")
            {
                if (ddlYear.SelectedIndex != 0)
                {
                    int year = Convert.ToInt32(ddlYear.SelectedValue);
                    rowIndex = int.Parse(e.CommandArgument.ToString());
                    sampleID = (int)this.gvWorkList.DataKeys[rowIndex]["SampleId"];
                    Session["SampleId"] = sampleID;
                    DataTable dt = new DataTable();
                    Popup(true);
                    dt = _cm.GetAllocation(EmployeeID, roleID, year);
                    if (dt.Rows.Count > 0)
                    {
                        Session["SampleId"] = dt.Rows[0]["SampleId"].ToString();
                    }
                    BindMessages(sampleID.ToString());
                }
            }
            if(e.CommandName=="AddTestResults")
            {
                string url = "";
                rowIndex = int.Parse(e.CommandArgument.ToString());
                sampleID = (int)this.gvWorkList.DataKeys[rowIndex]["SampleId"];
                CloneCRFSampleID = (int)this.gvWorkList.DataKeys[rowIndex]["CloneCRFSampleID"];
                int InquiryID = (int)this.gvWorkList.DataKeys[rowIndex]["InquiryID"];

                GridViewRow gvRow = gvWorkList.Rows[rowIndex];

                DataTable dtReport = _cm.GetReportFormat(InquiryID);
                string ReportName = dtReport.Rows[0]["ReportName"].ToString();

                Session["sampleID"] = sampleID;
                Session["CloneCRFSampleID"] = CloneCRFSampleID;
                switch (ReportName)
                {
                    case "Truck_Cert": url = "TestResultsTruck.aspx"; break;
                    default:url = "TestResults.aspx";break;
                }
                switch (DepartmentId)
                {
                    case 4006:
                        url = "WorkList_PV.aspx";
                        break;
                    case 4009:
                        url = "WorkList_LED.aspx";
                        break;
					 case 3006:
                        url = "WorkList_ElectricalAccessories.aspx";
                        break;
                    case 2:
                        url = "WorkList_Battery.aspx";
                        break;
                    default:
                        url = "TestResults.aspx";
                        break;
                }
                    StringBuilder sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.open('");
                sb.Append(url);
                sb.Append("');");
                sb.Append("</script>");
                ClientScript.RegisterStartupScript(this.GetType(),"script", sb.ToString());
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (Session["OrganizationID"] != null)
            {
                _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                string SampleId = Session["SampleId"].ToString();
                // string name = Session["Name"].ToString();
                DateTime DateString = DateTime.Now.AddHours(12.5);
                string Date = DateString.ToString(@"yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US"));
                if (txtSend.Text == "")
                {
                    txtSend.BackColor = System.Drawing.Color.LightPink;
                }
                else
                {
                    txtSend.BackColor = System.Drawing.Color.Empty;
                    string message = txtSend.Text;

                    _cm.sampleId = Convert.ToInt32(Session["SampleId"].ToString());
                    _cm.created = Convert.ToDateTime(Date);
                    _cm.message = message;
                    _cm.employeID = EmployeeID.ToString();
                    _cm.AddChat(_cm);

                    txtSend.Text = "";
                    BindMessages(SampleId);
                    Popup(true);
                }
            }
        }

        protected void tmMsg_Tick(object sender, EventArgs e)
        {
            if(Session["SampleId"] != null)
            {
                string SampleId = Session["SampleId"].ToString();
                BindMessages(SampleId);
            }            
        }
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindWorkList();
            }

        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindWorkList();
            }
        }
    }
}