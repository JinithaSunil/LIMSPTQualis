using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class WorkListMoconCompleted : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int EmployeeID = 0; int roleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");

            HtmlControl liWorkList2 = (HtmlGenericControl)this.Master.FindControl("liWorkList2");
            
            HtmlControl liViewSamples2 = (HtmlGenericControl)this.Master.FindControl("liViewSamples2");
			HtmlControl liAllSamples2 = (HtmlGenericControl)this.Master.FindControl("liAllSamples2");
            
            HtmlControl liGenerateReport2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReport2");
            
            HtmlControl liWorkListMoconCompleted2 = (HtmlGenericControl)this.Master.FindControl("liWorkListMoconCompleted2");
            HtmlControl liQRAnalyzer = (HtmlGenericControl)this.Master.FindControl("liQRAnalyzer");

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
                    switch (RoleID)
                    {
                        case 1://QC
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 2://Manager
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = true;
							liAllSamples2.Visible = true;
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                liViewSamples2.Visible = false;
                                liWorkList2.Visible = false;
                                liGenerateReport2.Visible = false;
                            }
                            else
                            {
                                
                                
                               
                            }
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 3://Lab Technician
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = false;
                            liTestManagement1.Visible = true;
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                liWorkList2.Visible = false;
                            }
                            else
                            {
                                
                            }
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            liQRAnalyzer.Visible = false;
                            liWorkListMoconCompleted2.Visible = true;
                            break;
                        case 4://Admin
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 5://Sample Management
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
                if (!Page.IsPostBack)
                {
                    BindYear();
                    liTestManagement1.Attributes.Add("class", "active");
                    liWorkListMoconCompleted2.Attributes.Add("class", "active");
                    BindWorkList();
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
                    switch (roleID)
                    {
                        case 1://QC
                            break;
                        case 2://Manager
                            dt = _cm.GetAllocation(EmployeeID, roleID, year);
                            gvWorkList.DataSource = dt;
                            gvWorkList.DataBind();
                            break;
                        case 3://Lab Tech
                            dt = _cm.GetTestingCompleted(EmployeeID, roleID, year);
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
                int index = e.Row.RowIndex; int AllocationID = 0; int sampleUID = 0; int sampleid = 0;
                int Id = int.Parse(gvWorkList.DataKeys[index]["AllocationId"].ToString());
                string AllocationId = Id.ToString();

                HyperLink hlAllocation = (e.Row.FindControl("hlAllocation") as HyperLink);
                Button btnTestResult = (e.Row.FindControl("btnTestResult") as Button);

                DataTable dt = _cm.GetAllocationDetails(AllocationId);
                DataTable dt1 = new DataTable();
                DataTable dt2 = _cm.getRoleDetails(EmployeeID);

                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);

                roleID = Convert.ToInt32(dt2.Rows[0]["roleID"].ToString());

                switch (lblStatusName.Text)
                {
                    case "Allocated For Testing":
                        AllocationID = Convert.ToInt32(dt.Rows[0]["AllocationId"].ToString());
                        Session["AllocationId"] = AllocationID.ToString();
                        sampleid = Convert.ToInt32(dt.Rows[0]["SampleId"].ToString());
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
                                btnTestResult.Visible = true;
                                Session["LabTechViewResult"] = 1;
                                break;
                            default: // Admin
                                //btnTestResult.Visible = false;
                                break;
                        }
                        break;
                    case "Test Report Submitted for Review":
                        break;
                    default:
                        //hlAllocation.Visible = false;
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
            if (e.CommandName == "ViewTestResults")
            {
                rowIndex = int.Parse(e.CommandArgument.ToString());
                sampleID = (int)this.gvWorkList.DataKeys[rowIndex]["SampleId"];
                Session["sampleID"] = sampleID;
                Session["LabTechViewResult"] = 1;
                string url = "TestResultsMocon.aspx";
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.open('");
                sb.Append(url);
                sb.Append("');");
                sb.Append("</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "script", sb.ToString());
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
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

        protected void tmMsg_Tick(object sender, EventArgs e)
        {
            if (Session["SampleId"] != null)
            {
                string SampleId = Session["SampleId"].ToString();
                BindMessages(SampleId);
            }
        }

        protected void btnTestResult_Click(object sender, EventArgs e)
        {
            Response.Redirect("TestResultsMocon.aspx");
            
        }
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindWorkList();

        }
    }
}