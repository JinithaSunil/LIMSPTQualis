using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class AllocateMultiple_ElectricalAccessories : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0; int SampleId = 0;
        int sampleUID = 0; int InquiryId = 0; int CloneCRFSampleID = 0;
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
                    if (Session["EmployeeID"] != null)
                    {
                        int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
                        BindAllocateDropdown(SupervisorID);
                    }
                    BindSampleList();

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void BindAllocateDropdown(int supervisorID)
        {
            try
            {
                DataTable dt3 = _cm.GetEmployeeDetails(supervisorID, true);

                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    ddlAllocate.DataSource = dt3;
                    ddlAllocate.DataTextField = "EmployeeName";
                    ddlAllocate.DataValueField = "ID1";
                    ddlAllocate.DataBind();
                    ddlAllocate.Items.Insert(0, new ListItem("Select Employee", "0"));
                }
                else
                {
                    ddlAllocate.Items.Clear();
                    ddlAllocate.Items.Insert(0, new ListItem("No Employees Found", "0"));
                }
            }
            catch (Exception ex)
            {
                ddlAllocate.Items.Clear();
                ddlAllocate.Items.Insert(0, new ListItem("Error loading employees", "0"));
                throw;
            }
        }

        protected void BindSampleList()
        {

            if (Session["SampleUID"] != null)
            {
                sampleUID = Convert.ToInt32(Session["SampleUID"].ToString());
                DataTable dt1 = _cm.GetCRFListForAllocation_mechanical(sampleUID);
                gvSampleList.DataSource = dt1;
                gvSampleList.DataBind();
            }
        }

        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string InquiryId = ""; int DepartmentID = 0;
                int sampleUID = 0;
                //int SupervisorID = Convert.ToInt32(Session["EmpID"].ToString());
                int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
                DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());

                CloneCRFSampleID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "CloneCRFSampleID").ToString());
                lblCloneCRFSampleID.Text = CloneCRFSampleID.ToString();

                Label lblEmployeeID = (e.Row.FindControl("lblEmployeeID") as Label);
                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                Button btnSampleManagement = (e.Row.FindControl("btnSampleManagement") as Button);
                DropDownList ddlAllocatedTo = e.Row.FindControl("ddlAllocatedTo") as DropDownList;
                DropDownList ddlLocation = (e.Row.FindControl("ddlLocation") as DropDownList);
                Button btnAllocate = (e.Row.FindControl("btnAllocate") as Button);

                // Check if already allocated
                if (lblEmployeeID != null && lblEmployeeID.Text != "")
                {
                    btnAllocate.Visible = false;
                    lblStatusName.Text = "Allocated For Testing";
                    btnAllocateAll.Visible = false;
                }
                else
                {
                    btnAllocate.Visible = true;
                    btnAllocateAll.Visible = true;
                }

                DataTable dt3 = _cm.GetEmployeeDetails(SupervisorID, true);

                if (dt3.Rows.Count > 0)
                {
                    ddlAllocatedTo.DataSource = dt3;
                    ddlAllocatedTo.DataTextField = "EmployeeName";
                    ddlAllocatedTo.DataValueField = "ID1";
                    ddlAllocatedTo.DataBind();
                    ddlAllocatedTo.Items.Insert(0, new ListItem("Select Employee"));
                }

                HyperLink hypAllocation = (e.Row.FindControl("hypAllocation") as HyperLink);

                if (lblEmployeeID != null && !string.IsNullOrEmpty(lblEmployeeID.Text))
                {
                    ListItem item = ddlAllocatedTo.Items.FindByValue(lblEmployeeID.Text);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        Console.WriteLine("Warning: Allocated employee ID not found in dropdown list: " + lblEmployeeID.Text);
                    }

                    ddlAllocatedTo.Enabled = false;
                }
                else
                {
                    ddlAllocatedTo.Enabled = true;
                }

                int index = e.Row.RowIndex;
                int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                lblSampleID.Text = SampleId.ToString();

                sampleUID = int.Parse(gvSampleList.DataKeys[index]["SampleID2"].ToString());
                lblSampleUID.Text = sampleUID.ToString();
                Session["SampleUID"] = sampleUID;

                int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
                DataTable dt4 = _cm.GetCustomerInquiryDetails(InquiryID, 1);
                DataTable dt2 = _cm.getInquiryDetails(InquiryID, 1);

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
                            ddlAllocatedTo.Visible = false;
                            break;
                        case "Sample Accepted By Lab":
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                ddlAllocatedTo.Visible = false;
                            }
                            else
                            {
                                switch (Convert.ToInt32(Session["RoleID"].ToString()))
                                {
                                    case 1:
                                        ddlAllocatedTo.Visible = true;
                                        break;
                                    case 2:
                                        ddlAllocatedTo.Visible = true;
                                        break;
                                    case 3:
                                    case 4:
                                    case 5:
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

        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = int.Parse(e.CommandArgument.ToString());
            int InquiryId = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
            int CRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CRFSampleID"].ToString());
            int CloneCRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CloneCRFSampleID"].ToString());
            int SupervisorID = Convert.ToInt32(Session["EmployeeIDget"].ToString());

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
                string TestItem = row.Cells[1].Text;

                DropDownList ddlAllocatedTo = (DropDownList)gvSampleList.Rows[rowIndex].Cells[3].FindControl("ddlAllocatedTo"); // Changed from Cells[0] to Cells[3] based on your grid structure
                HyperLink hypAllocation = (HyperLink)gvSampleList.Rows[rowIndex].Cells[4].FindControl("hypAllocation"); // Adjusted cell index

                if (ddlAllocatedTo.SelectedIndex == 0)
                {
                    ddlAllocatedTo.BackColor = Color.LightPink;
                    // Show validation message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select an employee to allocate.');", true);
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
                    _cm.ReportName = TestItem;

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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Allocation completed successfully.');", true);
                }
            }
        }

        protected void btnAllocate_Click(object sender, EventArgs e)
        {

            int SupervisorID = Convert.ToInt32(Session["EmployeeIDget"].ToString());
            if (ddlAllocate.Items.Count <= 1)
            {
                BindAllocateDropdown(SupervisorID);
            }
            if (ddlAllocate.SelectedIndex == 0 || string.IsNullOrEmpty(ddlAllocate.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select an employee to allocate.');", true);
                ddlAllocate.BackColor = Color.LightPink;
                return;
            }

            ddlAllocate.BackColor = Color.Empty;

            if (Session["SampleUID"] != null)
            {
                sampleUID = Convert.ToInt32(Session["SampleUID"].ToString());
                DataTable dt1 = _cm.GetCRFListForAllocation_mechanical(sampleUID);

                if (dt1.Rows.Count > 0)
                {
                    string TestItem = "";
                    int allocationIndex = 0;

                    foreach (DataRow dr in dt1.Rows)
                    {
                        _cm.ReportName = dr["ReportName"].ToString();
                        _cm.employeID = ddlAllocate.SelectedItem.Value;

                        if (dr.Table.Columns.Contains("ID"))
                        {
                            _cm.sampleId = Convert.ToInt32(dr["ID"].ToString());
                        }
                        else
                        {

                            _cm.sampleId = SampleId;
                        }

                        _cm.sampleStatusID = 19;
                        _cm.CRFSampleId = Convert.ToInt32(dr["CRFSampleID"].ToString());
                        _cm.inquiryId = Convert.ToInt32(dr["InquiryID"].ToString());
                        _cm.SupervisorID = SupervisorID;
                        _cm.AllocatedAt = DateTime.Now;

                        DataTable dt2 = _cm.GetAllocationID(_cm);
                        if (dt2 != null && dt2.Rows.Count > allocationIndex)
                        {
                            _cm.allocationID = Convert.ToInt32(dt2.Rows[allocationIndex]["AllocationID"]);
                            allocationIndex++;
                        }

                        // Check if this row has the same ReportName as the previous one
                        if (TestItem == dr["ReportName"].ToString() && !string.IsNullOrEmpty(TestItem))
                        {
                            _cm.UpdateAllocation(_cm, "Multiple");
                        }
                        else
                        {
                            _cm.UpdateAllocation(_cm, "Single");
                            TestItem = dr["ReportName"].ToString();
                        }

                        _cm.UpdateAllocation_mechanical(_cm);
                    }

                    BindSampleList();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Bulk allocation completed successfully.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No samples found for allocation.');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Sample UID not found in session.');", true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DataTable dt = _cm.CheckAllAllocated(Convert.ToInt32(Session["CloneCRFSampleID"].ToString()));
            if (dt.Rows.Count == 0)
            {
                Session["SampleID"] = lblSampleID.Text;
                Session["SampleUID"] = lblSampleUID.Text;
                UpdateStatus();
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
                    }

                    int DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                    switch (DepartmentID)
                    {
                        case 4007:
                            Response.Redirect("GenerateTestAllocation_mechanical.aspx");
                            break;
                        case 2:
                            Response.Redirect("TestAllocationMocon.aspx");
                            break;
                        case 3:
                            Response.Redirect("TestAllocationPTI.aspx");
                            break;
                        case 4009:
                            Response.Redirect("GenerateTestAllocation_LED.aspx");
                            break;
                        case 3006:
                            Response.Redirect("GenerateTestAllocation_ElectricalAccessories.aspx");
                            break;
                        default: break;
                    }

                }
            }

        }


    }
}