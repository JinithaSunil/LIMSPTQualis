using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ZXing;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LIMS
{
    public partial class AllocateMultiple : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0; int SampleId = 0;
        int sampleUID = 0; int InquiryId = 0;int CloneCRFSampleID = 0;
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
                    BindSampleList();

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        protected void BindSampleList()
        {
            //Session["SampleUID"] = "34546041";

            if (Session["SampleUID"] != null)
            {
                sampleUID = Convert.ToInt32(Session["SampleUID"].ToString());
                DataTable dt1 = _cm.GetCRFListForAllocation(sampleUID);
                gvSampleList.DataSource = dt1;
                gvSampleList.DataBind();
            }
        }
        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string InquiryId = ""; int DepartmentID = 0;
                int AllocationID = 0; int sampleUID = 0;
                int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
                DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());

                CloneCRFSampleID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "CloneCRFSampleID").ToString());
                lblCloneCRFSampleID.Text = CloneCRFSampleID.ToString();

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblEmployeeID = (e.Row.FindControl("lblEmployeeID") as Label);
                    Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);

                    Button btnSampleManagement = (e.Row.FindControl("btnSampleManagement") as Button);

                    DropDownList ddlAllocatedTo = (e.Row.FindControl("ddlAllocatedTo") as DropDownList);
                    DropDownList ddlLocation = (e.Row.FindControl("ddlLocation") as DropDownList);

                    Button btnAllocate = (e.Row.FindControl("btnAllocate") as Button);

                    if (lblEmployeeID.Text != "")
                    {
                        btnAllocate.Visible = false;
                        lblStatusName.Text = "Allocated For Testing";
                    }
                    DataTable dt3 = _cm.GetEmployeeDetails(SupervisorID, true);

                    if (dt3.Rows.Count > 0)
                    {
                        ddlAllocatedTo.DataSource = dt3;
                        ddlAllocatedTo.DataTextField = "EmployeeName";
                        //ddlAllocatedTo.DataValueField = "EmployeeID"; changed since worklist is based on Id
                        ddlAllocatedTo.DataValueField = "ID1";
                        ddlAllocatedTo.DataBind();
                        ddlAllocatedTo.Items.Insert(0, new ListItem("Select Employee"));

                        ddlAllocate.DataSource = dt3;
                        ddlAllocate.DataTextField = "EmployeeName";
                        //ddlAllocate.DataValueField = "EmployeeID"; changed since worklist is based on Id
                        ddlAllocate.DataValueField = "ID1";
                        ddlAllocate.DataBind();
                        ddlAllocate.Items.Insert(0, new ListItem("Select Employee"));
                    }

                    HyperLink hypAllocation = (e.Row.FindControl("hypAllocation") as HyperLink);

                    Label lblAllocatedTo = (e.Row.FindControl("lblAllocatedTo") as Label);
                    if (lblAllocatedTo.Text != "")
                    {
                        btnAllocate.Visible = false;
                        ddlAllocatedTo.Items.FindByText(lblAllocatedTo.Text).Selected=true;
                    }

                    int index = e.Row.RowIndex;

                    int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                    lblSampleID.Text= SampleId.ToString();

                    sampleUID = int.Parse(gvSampleList.DataKeys[index]["SampleID2"].ToString());
                    lblSampleUID.Text = sampleUID.ToString();

                    Session["SampleUID"] = sampleUID;

                    int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
                    DataTable dt7 = _cm.GetAllocationFileDetails(SampleId);
                    DataTable dt4 = _cm.GetCustomerInquiryDetails(InquiryID, 1);
                    DataTable dt6 = _cm.GetAllocationDetails(SampleId);
                    DataTable dt2 = _cm.getInquiryDetails(InquiryID, 1);
                    DataTable dt8 = new DataTable();
                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            CustomerUID = Convert.ToInt32(dr2["CustomerUID"].ToString());
                        }
                    }
                   
                    if (dt4.Rows.Count > 0)
                    {
                        InquiryId = dt4.Rows[0]["InquiryId"].ToString();
                        DepartmentID = Convert.ToInt32(dt4.Rows[0]["DepartmentID"].ToString());
                                              
                        switch (lblStatusName.Text)
                        {
                            
                            case "Sample Accepted By Sample Management":
                                btnAllocate.Visible = false;     
                                ddlAllocatedTo.Visible = false;
                                break;
                            case "Sample Accepted By Lab":                               
                                if (Session["DepartmentID"].ToString() == "1")
                                {
                                    ddlAllocatedTo.Visible = false;
                                    btnAllocate.Visible = false;
                                }
                                else
                                {
                                    switch (Convert.ToInt32(Session["RoleID"].ToString()))
                                    {
                                        case 1:
                                            ddlAllocatedTo.Visible = true;
                                            btnAllocate.Visible = true;
                                            break;
                                        case 2:
                                            ddlAllocatedTo.Visible = true;
                                            btnAllocate.Visible = true;
                                            break;
                                        case 3:
                                            ddlAllocatedTo.Visible = false;
                                            btnAllocate.Visible = false;
                                            break;
                                        case 4:
                                            ddlAllocatedTo.Visible = false;
                                            btnAllocate.Visible = false;
                                            break;
                                        case 5:
                                            ddlAllocatedTo.Visible = false;
                                            btnAllocate.Visible = false;
                                            break;
                                        default:
                                            ddlAllocatedTo.Visible = false;
                                            btnAllocate.Visible = false;
                                            break;
                                    }

                                }
                                
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            int InquiryId = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
            int CRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CRFSampleID"].ToString());
            int CloneCRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CloneCRFSampleID"].ToString());
            int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());

            _cm.inquiryId = InquiryId;
            SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
            sampleUID = int.Parse(gvSampleList.DataKeys[index]["SampleID2"].ToString());
            Session["SampleUID"] = SampleId;
            Session["SampleUID"] = sampleUID;

            GridView grid = (GridView)sender;
            GridViewRow row1 = grid.Rows[index];
            System.Web.UI.WebControls.Image imgageQRCode = (System.Web.UI.WebControls.Image)row1.FindControl("imgageQRCode");

            Session["CloneCRFSampleID"] = CloneCRFSampleID;
            
            if (e.CommandName == "Save")
            {               
                DataTable dt2 = _cm.GetProductCategory(InquiryId);

                int rowIndex = int.Parse(e.CommandArgument.ToString());
                GridViewRow row = gvSampleList.Rows[index];
                string TestParameter = row.Cells[1].Text;
                string TestSubParameter = row.Cells[2].Text;

                DropDownList ddlAllocatedTo = (DropDownList)gvSampleList.Rows[rowIndex].Cells[0].FindControl("ddlAllocatedTo");
                HyperLink hypAllocation = (HyperLink)gvSampleList.Rows[rowIndex].Cells[0].FindControl("hypAllocation");

                if (ddlAllocatedTo.SelectedIndex == 0)
                {
                    ddlAllocatedTo.BackColor = Color.LightPink;
                }
                else
                {
                    ddlAllocatedTo.BackColor = Color.Empty;
                    foreach (DataRow dr in dt2.Rows)
                    {
                        _cm.department = dr["DepartmentID"].ToString();
                        _cm.productCategory = dr["subgroupid"].ToString();
                    }
                    _cm.employeID = ddlAllocatedTo.SelectedItem.Value;
                    _cm.sampleId = SampleId;
                    _cm.sampleStatusID = 19;
                    _cm.CRFSampleId = CRFSampleID;
                    _cm.SupervisorID = SupervisorID;
                    _cm.AllocatedAt = DateTime.Now;
                    _cm.testParameter = TestParameter;
                    _cm.testSubParameter = TestSubParameter;

                    DataTable dtCheckCount = _cm.checkMultipleCRFSampleDetails(_cm);
                    if (dtCheckCount.Rows.Count > 0)
                    {
                        _cm.UpdateAllocation(_cm, "Multiple");
                    }
                    else
                    {
                        _cm.UpdateAllocation(_cm, "Single");
                    }
                   
                    BindSampleList();
                }
            }
        }


        protected void UpdateStatus()
        {
            if (Session["SampleID"] != null)
            {
                if (Session["CloneCRFSampleID"] != null)
                {
                    SampleId = Convert.ToInt32(Session["SampleID"].ToString());
                    CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());

                    DataTable dt = _cm.GetCustomerInquiryDetails(SampleId);
                    if (dt.Rows.Count > 0)
                    {
                        //change CloneCRF status to 'Allocated for Testing'
                        _cm.CRFStatusID = 19;
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);

                        _cm.sampleId = SampleId;
                        _cm.sampleStatusID = 19;
                        _cm.updateSampleStatus(_cm);

                        //Change Quotation status to 'Sample Accepted By Lab'
                        _cm.QuotationStatusID = 1014;//Sample Accepted By Lab
                        _cm.UpdateQuotationStatus(_cm);
                    }
                    DataTable dtCRFType = _cm.getCRFTypeDetails(Convert.ToInt32(dt.Rows[0]["InquiryID"].ToString()));
                    if (dtCRFType.Rows.Count > 0)
                    {
                        int CRFID = Convert.ToInt32(dtCRFType.Rows[0]["CRFID"].ToString());
                        switch (CRFID)
                        {
                            case 1:
                                Response.Redirect("TestAllocationFM.aspx");
                                break;
                            case 2:
                                Response.Redirect("TestAllocationMocon.aspx");
                                break;
                            case 3:
                                Response.Redirect("TestAllocationPTI.aspx");
                                break;
                            case 4:
                                Response.Redirect("TestAllocationFM.aspx");
                                break;
                            default: break;
                        }
                    }
                }
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DataTable dt = _cm.CheckAllAllocated(Convert.ToInt32(Session["CloneCRFSampleID"].ToString()));
            if(dt.Rows.Count==0)
            {
                Session["SampleID"] = lblSampleID.Text;
                Session["SampleUID"] = lblSampleUID.Text;
                UpdateStatus();
                Response.Redirect("GenerateTestAllocation.aspx?s=" + SampleId);
            }            
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlLocation = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlLocation.NamingContainer;
            DropDownList ddlAllocatedTo = (DropDownList)row.FindControl("ddlAllocatedTo");
            
            int DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());

            DataTable dt3 = _cm.GetEmployeeDetails(DepartmentID, ddlLocation.SelectedValue, true);
            if (dt3.Rows.Count > 0)
            {
                ddlAllocatedTo.DataSource = dt3;
                ddlAllocatedTo.DataTextField = "EmployeeName";
                //ddlAllocatedTo.DataValueField = "EmployeeID"; changed since worklist is based on Id
                ddlAllocatedTo.DataValueField = "ID1";
                ddlAllocatedTo.DataBind();
                ddlAllocatedTo.Items.Insert(0, new ListItem("Select Employee"));
            }
        }

        protected void btnAllocate_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ddlAllocate.SelectedIndex == 0)
            {
                ddlAllocate.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlAllocate.BackColor = Color.Empty;
            }
            if (flag == 0)
            {
                int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
                if (Request.QueryString["ID"] != null)
                {
                    SampleId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    if (Session["SampleUID"] != null)
                    {
                        sampleUID = Convert.ToInt32(Session["SampleUID"].ToString());
                        DataTable dt1 = _cm.GetCRFListForAllocation(sampleUID);
                        if (dt1.Rows.Count > 0)
                        {
                            string TestParameter = "";
                            string TestSubParameter = "";
                            foreach (DataRow dr in dt1.Rows)
                            {
                                _cm.testParameter = TestParameter;
                                _cm.testSubParameter = TestSubParameter;

                                _cm.employeID = ddlAllocate.SelectedItem.Value;
                                _cm.sampleId = SampleId;
                                _cm.sampleStatusID = 19;
                                _cm.CRFSampleId = Convert.ToInt32(dr["CRFSampleID"].ToString());
                                _cm.inquiryId = Convert.ToInt32(dr["InquiryID"].ToString());
                                _cm.SupervisorID = SupervisorID;
                                _cm.AllocatedAt = DateTime.Now;

                                if ((TestParameter == dr["TestParameter"].ToString()) && (TestSubParameter == dr["TestSubParameter"].ToString()))
                                {
                                    _cm.UpdateAllocation(_cm, "Multiple");
                                }
                                else
                                {
                                    _cm.UpdateAllocation(_cm, "Single");
                                    TestParameter = dr["TestParameter"].ToString();
                                    TestSubParameter = dr["TestSubParameter"].ToString();
                                }

                            }

                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Bulk allocation completed successfully. Click on Submit')", true);
                        }
                        BindSampleList();
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
        }
    }
}