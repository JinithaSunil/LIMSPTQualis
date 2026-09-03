using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Default : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0; int DepartmentID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

            int clientId = 1;
            Session["CustomerId"] = clientId;

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
                    hdRoleID.Value = Session["RoleID"].ToString();

                    divCRF.Visible = false;
                    divSamples.Visible = false;
                    divAllocation.Visible = false;
                    divWorkList.Visible = false;
                    divGenReport.Visible = false;
                    divReview.Visible = false;

                    int RoleID = Convert.ToInt32(hdRoleID.Value);
                    switch (RoleID)
                    {
                        case 1://QC
                            divSamples.Visible = true;
                            divReview.Visible = true;
                            break;
                        case 2://Manager
                            divCRF.Visible = true;
                            divSamples.Visible = true;
                            divGenReport.Visible = true;
                            divReview.Visible = true;
                            divAllocation.Visible = true;
                            break;
                        case 3://Lab Technician
                            divWorkList.Visible = true;
                            break;
                        case 4://Admin
                            divSamples.Visible = true;
                            break;
                        case 5://Sample Management
                            divCRF.Visible = true;
                            divSamples.Visible = true;
                            divGenReport.Visible = true;
                            break;
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                        case 26:
                        case 27:
                        case 28:
                            Response.Redirect("DashboardApprovals.aspx");
                            break;
                        default:
                            Response.Redirect("DashboardUser.aspx");
                            break;
                    }


                    if (!Page.IsPostBack)
                    {
                        if (Session["Usertype"] != null)
                        {
                            if (Session["DepartmentID"] != null)
                            {
                                DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                            }
                            EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                            RoleID = Convert.ToInt32(hdRoleID.Value);

                            DataTable[] dt = new DataTable[4];
                            dt = _cm.GetCustomerInquiry();

                            hypNewEnquiries.Text = dt[0].Rows.Count.ToString();
                            hypNewEnquiries.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");

                            hypNewCustomers.Text = dt[1].Rows.Count.ToString();
                            hypNewCustomers.NavigateUrl = string.Format("CustomerCompanyView.aspx?s=1");

                            DataTable[] dt2 = new DataTable[8];
                            dt2 = _cm.GetWork(EmployeeID, RoleID, DepartmentID);
                            if (dt2[0].Rows.Count > 0)
                            {
                                hypSamples.Text = dt2[0].Rows.Count.ToString();
                                hypSamples.NavigateUrl = "AllSamples.aspx";
                            }
                            if (dt2[1].Rows.Count > 0)
                            {
                                hypWorkList.Text = dt2[1].Rows.Count.ToString();
                                hypWorkList.NavigateUrl = "WorkList.aspx";
                            }

                            hypAssigned.Text = dt[2].Rows.Count.ToString();
                            hypAssigned.NavigateUrl = string.Format("SampleList.aspx?s=1");

                            hypCompleted.Text = dt[3].Rows.Count.ToString();
                            hypCompleted.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");

                            DataTable[] dt1 = new DataTable[4];
                            dt1 = _cm.GetSampleCount();

                           
                            dt2 = _cm.GetWork(EmployeeID,RoleID, DepartmentID);
                            if (dt2[0].Rows.Count > 0)
                            {
                                hypSamples.Text = dt2[0].Rows.Count.ToString();
                                hypSamples.NavigateUrl = "AllSamples.aspx";
                            }
                            if (dt2[1].Rows.Count > 0)
                            {
                                hypWorkList.Text = dt2[1].Rows.Count.ToString();
                                hypWorkList.NavigateUrl = "WorkList.aspx";
                            }                            
                            if (dt2[2].Rows.Count > 0)
                            {
                                hypGenReport.Text = dt2[2].Rows.Count.ToString();
                                hypGenReport.NavigateUrl = "GenerateReport.aspx";
                            }
                            if (dt2[3].Rows.Count > 0)
                            {
                                hypReview.Text = dt2[3].Rows.Count.ToString();
                                hypReview.NavigateUrl = "ReviewReport.aspx";
                            }
                            if (dt2[4].Rows.Count > 0)
                            {
                                hypAllocate.Text = dt2[4].Rows.Count.ToString();
                                hypAllocate.NavigateUrl = "SampleList.aspx";
                            }
                            if (RoleID == 2)
                            {
                                if (dt2[5].Rows.Count > 0)
                                {
                                    hypCRF.Text = dt2[5].Rows.Count.ToString();
                                    hypCRF.NavigateUrl = "CustomerCRFList.aspx";
                                }
                            }
                            if (RoleID == 5)
                            {
                                if (dt2[6].Rows.Count > 0)
                                {
                                    hypCRF.Text = dt2[6].Rows.Count.ToString();
                                    hypCRF.NavigateUrl = "CustomerInquiryView.aspx";
                                }
                                if (dt2[7].Rows.Count > 0)
                                {
                                    hypCRF.Text = dt2[7].Rows.Count.ToString();
                                    hypCRF.NavigateUrl = "CustomerCRFList.aspx";
                                }
                            }

                            BindWork();
                        }
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindWork()
        {
            DataTable dt = _cm.GetSupervisors();
            //gvEmployeeWorkStatus.DataSource = dt;
            //gvEmployeeWorkStatus.DataBind();
        }
        protected void btnToday_Click(object sender, EventArgs e)
        {
            hdPeriod.Value = "today";
            divPeriod.InnerHtml = "Today's Status";
            DataTable[] dt = new DataTable[4];
            dt = _cm.GetStatus("today");
            if (dt[0].Rows.Count > 0)
            {
                hypNewEnquiries.Text = dt[0].Rows.Count.ToString();
                hypNewEnquiries.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewEnquiries.Text = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                hypNewCustomers.Text = dt[1].Rows.Count.ToString();
                hypNewCustomers.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewCustomers.Text = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                hypAssigned.Text = dt[2].Rows.Count.ToString();
                hypAssigned.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypAssigned.Text = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                hypCompleted.Text = dt[3].Rows.Count.ToString();
                hypCompleted.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypCompleted.Text = "";
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
                hypNewEnquiries.Text = dt[0].Rows.Count.ToString();
                hypNewEnquiries.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewEnquiries.Text = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                hypNewCustomers.Text = dt[1].Rows.Count.ToString();
                hypNewCustomers.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewCustomers.Text = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                hypAssigned.Text = dt[2].Rows.Count.ToString();
                hypAssigned.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypAssigned.Text = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                hypCompleted.Text = dt[3].Rows.Count.ToString();
                hypCompleted.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypCompleted.Text = "";
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
                hypNewEnquiries.Text = dt[0].Rows.Count.ToString();
                hypNewEnquiries.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewEnquiries.Text = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                hypNewCustomers.Text = dt[1].Rows.Count.ToString();
                hypNewCustomers.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewCustomers.Text = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                hypAssigned.Text = dt[2].Rows.Count.ToString();
                hypAssigned.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypAssigned.Text = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                hypCompleted.Text = dt[3].Rows.Count.ToString();
                hypCompleted.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypCompleted.Text = "";
            }
        }

        protected void btnAll_Click(object sender, EventArgs e)
        {
            DataTable[] dt = new DataTable[4];
            dt = _cm.GetStatus("all");
            if (dt[0].Rows.Count > 0)
            {
                hypNewEnquiries.Text = dt[0].Rows.Count.ToString();
                hypNewEnquiries.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewEnquiries.Text = "";
            }
            if (dt[1].Rows.Count > 0)
            {
                hypNewCustomers.Text = dt[1].Rows.Count.ToString();
                hypNewCustomers.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypNewCustomers.Text = "";
            }
            if (dt[2].Rows.Count > 0)
            {
                hypAssigned.Text = dt[2].Rows.Count.ToString();
                hypAssigned.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypAssigned.Text = "";
            }
            if (dt[3].Rows.Count > 0)
            {
                hypCompleted.Text = dt[3].Rows.Count.ToString();
                hypCompleted.NavigateUrl = string.Format("CustomerInquiryView.aspx?s=1");
            }
            else
            {
                hypCompleted.Text = "";
            }
        }
    }
}