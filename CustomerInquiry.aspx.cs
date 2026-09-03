using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerInquiry : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.emailCommunication _email = new DAL.emailCommunication();
        private DataTable GetPreviewTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("GroupId");
            dt.Columns.Add("SubGroupId");
            dt.Columns.Add("GroupName");
            dt.Columns.Add("SubGroupName");
            dt.Columns.Add("InstrumentIds");
            dt.Columns.Add("InstrumentNames");
            return dt;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
                        BindDivisions();
                        txtInquiryDetail.Visible = true;
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void BindCustomer()
        {
            DataTable dt = _cm.getAllCustomers();
            if (dt.Rows.Count > 0)
            {
                ddlCustomerName.DataSource = dt;
                ddlCustomerName.DataTextField = "CustomerName";
                ddlCustomerName.DataValueField = "CustomerMasterId";
                ddlCustomerName.DataBind();
                ddlCustomerName.Items.Insert(0, new ListItem("Select Customer", ""));
            }
            if (Request.QueryString["ID"] != null)
            {
                int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                ddlCustomerName.SelectedValue = customerMasterID.ToString();
            }
        }
        private void BindCustomer(int DepartmentID)
        {
            DataTable dt = new DataTable();
            if (chkAll.Checked)
            {
                dt = _cm.getAllCustomers();
            }
            else
            {
                dt = _cm.getAllCustomers(DepartmentID);
            }

            if (dt.Rows.Count > 0)
            {
                ddlCustomerName.DataSource = dt;
                ddlCustomerName.DataTextField = "CustomerName";
                ddlCustomerName.DataValueField = "CustomerMasterId";
                ddlCustomerName.DataBind();
                ddlCustomerName.Items.Insert(0, new ListItem("Select Customer", ""));
            }
            else
            {
                ddlCustomerName.DataSource = null;
                ddlCustomerName.DataBind();
            }
            if (Request.QueryString["ID"] != null)
            {
                int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                ddlCustomerName.SelectedValue = customerMasterID.ToString();
            }
        }
        private void BindDivisions()
        {
            DataTable dt = _cm.GetAllDivisions("LIMS");
            if (dt.Rows.Count > 0)
            {
                ddlDivision.DataSource = dt;
                ddlDivision.DataTextField = "DivisionName";
                ddlDivision.DataValueField = "DivisionID";
                ddlDivision.DataBind();
                ddlDivision.Items.Insert(0, new ListItem("Select Department", ""));
            }

        }
        private void BindDepartment(string DivisionID)
        {
            DataTable dt = _cm.GetAllDepartments(DivisionID);
            if (dt.Rows.Count > 0)
            {
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Division", ""));
            }
        }
        private void BindReportType(int DepartmentID)
        {

            DataTable dt = _cm.GetAllReportType(DepartmentID);
            if (dt.Rows.Count > 0)
            {
                ddlReportFormat.DataSource = dt;
                ddlReportFormat.DataTextField = "ReportName";
                ddlReportFormat.DataValueField = "ReportID";
                ddlReportFormat.DataBind();
                ddlReportFormat.Items.Insert(0, new ListItem("Select Report Type", ""));
            }
        }
        private void BindTestCategory(int DepartmentID)
        {
            if (DepartmentID == 3006)
            {
                int ProductGroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
                DataTable dt = _cm.GetTestCategoryProductWise(ProductGroupID);
                if (dt.Rows.Count > 0)
                {
                    ddlTestCategory.DataSource = dt;
                    ddlTestCategory.DataTextField = "TestCategory";
                    ddlTestCategory.DataValueField = "TestCategoryID";
                    ddlTestCategory.DataBind();
                    ddlTestCategory.Items.Insert(0, new ListItem("Select Test Category", ""));
                }
                else
                {
                    ddlTestCategory.DataSource = null;
                    ddlTestCategory.DataBind();
                }
            }
            else if (DepartmentID == 5020)
            {
                int ProductGroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
                DataTable dt = _cm.GetTestCategoryProductWise(ProductGroupID);
                if (dt.Rows.Count > 0)
                {
                    ddlTestCategory.DataSource = dt;
                    ddlTestCategory.DataTextField = "TestCategory";
                    ddlTestCategory.DataValueField = "TestCategoryID";
                    ddlTestCategory.DataBind();
                    ddlTestCategory.Items.Insert(0, new ListItem("Select Test Category", ""));
                }
                else
                {
                    ddlTestCategory.DataSource = null;
                    ddlTestCategory.DataBind();
                }
            }
            else
            {
                DataTable dt = _cm.GetTestCategory(DepartmentID);
                if (dt.Rows.Count > 0)
                {
                    ddlTestCategory.DataSource = dt;
                    ddlTestCategory.DataTextField = "TestCategory";
                    ddlTestCategory.DataValueField = "TestCategoryID";
                    ddlTestCategory.DataBind();
                    ddlTestCategory.Items.Insert(0, new ListItem("Select Test Category", ""));
                }
                else
                {
                    ddlTestCategory.DataSource = null;
                    ddlTestCategory.DataBind();
                }
            }
        }
        private void BindStandards(int DepartmentID)
        {
            DataTable dt = _cm.GetStandards(DepartmentID);
            if (dt.Rows.Count > 0)
            {
                ddlStandard.DataSource = dt;
                ddlStandard.DataTextField = "StandardName";
                ddlStandard.DataValueField = "StandardID";
                ddlStandard.DataBind();
                ddlStandard.Items.Insert(0, new ListItem("Select Standard", ""));
            }
        }
        private void BindCableTypeStandards()
        {
            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();
            int departmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
            if (departmentID == 3006)
            {
                if (ddlTestCategory.SelectedIndex != 0)
                {
                    int ProductGroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
                    dt = _cm.GetTestCategoryProductWise(ProductGroupID);
                    if (dt.Rows.Count > 0)
                    {
                        string Standards = dt.Rows[0]["Standards"].ToString();
                        string CableType = dt.Rows[0]["CableType"].ToString();
                        string ReportList = dt.Rows[0]["ReportID"].ToString();
                        string query = "Select * from Standards where StandardID in(" + Standards + ")";
                        dt = _cm.GetTestCategoryDetails(query);
                        if (dt.Rows.Count > 0)
                        {
                            ddlStandard.DataSource = dt;
                            ddlStandard.DataTextField = "StandardName";
                            ddlStandard.DataValueField = "StandardID";
                            ddlStandard.DataBind();
                            ddlStandard.Items.Insert(0, new ListItem("Select Standard", ""));
                        }
                        else
                        {
                            ddlStandard.DataSource = null;
                            ddlStandard.DataBind();
                        }
                        query = "Select * from CableType where CableTypeID in(" + CableType + ")";
                        dt = _cm.GetTestCategoryDetails(query);
                        if (dt.Rows.Count > 0)
                        {
                            ddlCableType.DataSource = dt;
                            ddlCableType.DataTextField = "CableType";
                            ddlCableType.DataValueField = "CableTypeID";
                            ddlCableType.DataBind();
                            ddlCableType.Items.Insert(0, new ListItem("Select Cable Type", ""));
                        }
                        else
                        {
                            ddlCableType.DataSource = null;
                            ddlCableType.DataBind();
                        }
                    }

                    string TestCategoryID = ddlTestCategory.SelectedValue;
                    dtNew = _cm.GetTestCategory(TestCategoryID);
                    if (dtNew.Rows.Count > 0)
                    {
                        string Standards = dtNew.Rows[0]["Standards"].ToString();
                        string CableType = dtNew.Rows[0]["CableType"].ToString();
                        string ReportList = dtNew.Rows[0]["ReportID"].ToString();
                        if (ReportList.Length > 0)
                        {
                            string query1 = "Select * from ReportList where ReportID in(" + ReportList + ")";

                            dtNew = _cm.GetTestCategoryDetails(query1);
                            if (dtNew.Rows.Count > 0)
                            {
                                gvTests.DataSource = dtNew;
                                gvTests.DataBind();
                            }
                            else
                            {
                                gvTests.DataSource = null;
                                gvTests.DataBind();
                            }
                        }
                        else
                        {
                            gvTests.DataSource = null;
                            gvTests.DataBind();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Test Record Not Found!');", true);
                        }

                    }


                }
            }
            else if (departmentID == 5020)
            {
                if (ddlTestCategory.SelectedIndex != 0)
                {
                    int ProductGroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
                    dt = _cm.GetTestCategoryProductWise(ProductGroupID);
                    if (dt.Rows.Count > 0)
                    {
                        string Standards = dt.Rows[0]["Standards"].ToString();
                        string CableType = dt.Rows[0]["CableType"].ToString();
                        string ReportList = dt.Rows[0]["ReportID"].ToString();
                        string query = "Select * from Standards where StandardID in(" + Standards + ")";
                        dt = _cm.GetTestCategoryDetails(query);
                        if (dt.Rows.Count > 0)
                        {
                            ddlStandard.DataSource = dt;
                            ddlStandard.DataTextField = "StandardName";
                            ddlStandard.DataValueField = "StandardID";
                            ddlStandard.DataBind();
                            ddlStandard.Items.Insert(0, new ListItem("Select Standard", ""));
                        }
                        else
                        {
                            ddlStandard.DataSource = null;
                            ddlStandard.DataBind();
                        }
                    }

                    string TestCategoryID = ddlTestCategory.SelectedValue;
                    dtNew = _cm.GetTestCategory(TestCategoryID);
                    if (dtNew.Rows.Count > 0)
                    {
                        string Standards = dtNew.Rows[0]["Standards"].ToString();
                        string CableType = dtNew.Rows[0]["CableType"].ToString();
                        string ReportList = dtNew.Rows[0]["ReportID"].ToString();
                        if (ReportList.Length > 0)
                        {
                            string query1 = "Select * from ReportList where ReportID in(" + ReportList + ")";

                            dtNew = _cm.GetTestCategoryDetails(query1);
                            if (dtNew.Rows.Count > 0)
                            {
                                gvTests.DataSource = dtNew;
                                gvTests.DataBind();
                            }
                            else
                            {
                                gvTests.DataSource = null;
                                gvTests.DataBind();
                            }
                        }
                        else
                        {
                            gvTests.DataSource = null;
                            gvTests.DataBind();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Test Record Not Found!');", true);
                        }

                    }


                }
            }
            else
            {
                if (ddlTestCategory.SelectedIndex != 0)
                {
                    string TestCategoryID = ddlTestCategory.SelectedValue;
                    dt = _cm.GetTestCategory(TestCategoryID);
                    if (dt.Rows.Count > 0)
                    {
                        string Standards = dt.Rows[0]["Standards"].ToString();
                        string CableType = dt.Rows[0]["CableType"].ToString();
                        string ReportList = dt.Rows[0]["ReportID"].ToString();

                        string query = "Select * from Standards where StandardID in(" + Standards + ")";
                        dt = _cm.GetTestCategoryDetails(query);
                        if (dt.Rows.Count > 0)
                        {
                            ddlStandard.DataSource = dt;
                            ddlStandard.DataTextField = "StandardName";
                            ddlStandard.DataValueField = "StandardID";
                            ddlStandard.DataBind();
                            ddlStandard.Items.Insert(0, new ListItem("Select Standard", ""));
                        }
                        else
                        {
                            ddlStandard.DataSource = null;
                            ddlStandard.DataBind();
                        }
                        if ((ddlDepartment.SelectedValue != "4006")&& (ddlDepartment.SelectedValue != "2"))
                        {
                        query = "Select * from CableType where CableTypeID in(" + CableType + ")";
                        dt = _cm.GetTestCategoryDetails(query);
                        if (dt.Rows.Count > 0)
                        {
                            ddlCableType.DataSource = dt;
                            ddlCableType.DataTextField = "CableType";
                            ddlCableType.DataValueField = "CableTypeID";
                            ddlCableType.DataBind();
                            ddlCableType.Items.Insert(0, new ListItem("Select Cable Type", ""));
                        }
                        else
                        {
                            ddlCableType.DataSource = null;
                            ddlCableType.DataBind();
                            }
                        }
                        else
                        {
                            ddlCableType.Visible = false;
                        }
                        query = "Select * from ReportList where ReportID in(" + ReportList + ")";
                        dt = _cm.GetTestCategoryDetails(query);
                        if (dt.Rows.Count > 0)
                        {
                            gvTests.DataSource = dt;
                            gvTests.DataBind();
                        }
                        else
                        {
                            gvTests.DataSource = null;
                            gvTests.DataBind();
                        }
                    }
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int flag = 0;

            if (txtInquiryDetail.Text == "")
            {
                txtInquiryDetail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtInquiryDetail.BackColor = System.Drawing.Color.Empty;
            }
            if ((ddlCustomerName.SelectedIndex == 0) || (ddlCustomerName.SelectedIndex == 0))
            {
                ddlCustomerName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlCustomerName.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlDepartment.SelectedIndex == 0)
            {
                ddlDepartment.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlDepartment.BackColor = System.Drawing.Color.Empty;
            }
            if ((ddlDepartment.SelectedValue == "4007") ||// Electrical Cable & FO Lab
                (ddlDepartment.SelectedValue == "4011") ||  //LS Pro
                (ddlDepartment.SelectedValue == "4012") || //Calibration Lab
                (ddlDepartment.SelectedValue == "4013") || //IAT
                (ddlDepartment.SelectedValue == "4009") || //LED
                (ddlDepartment.SelectedValue == "4006") || //PV
                (ddlDepartment.SelectedValue == "4008") || //home appliances
                (ddlDepartment.SelectedValue == "4014") ||   //LSSM, LSMK3, LSMKI, LVV
                (ddlDepartment.SelectedValue == "3006") ||  //Electrical Accessories
                (ddlDepartment.SelectedValue == "5020")  ||    //Physical Softline
                (ddlDepartment.SelectedValue == "2")     //Physical Softline
                )
            {

            }
            else
            {
                if (ddlProductGroup.SelectedIndex == 0)
                {
                    ddlProductGroup.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    ddlProductGroup.BackColor = System.Drawing.Color.Empty;
                }
                if (ddlProductSubGroup.SelectedIndex == 0)
                {
                    ddlProductSubGroup.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    ddlProductSubGroup.BackColor = System.Drawing.Color.Empty;
                }
            }
            if (ddlDepartment.SelectedValue == "4012")
            {
                if (ddlProductGroup.SelectedIndex == 0)
                {
                    ddlProductGroup.BackColor = Color.LightPink;
                    flag = 1;
                }
                if (ddlProductSubGroup.SelectedIndex == 0)
                {
                    ddlProductSubGroup.BackColor = Color.LightPink;
                    flag = 1;
                }
                DataTable dt = ViewState["Preview"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                    return;

                hdSelectedInstruments.Value = string.Join(" | ",
                    dt.AsEnumerable().Select(r =>
                        $"{r["GroupID"]}:{r["SubGroupID"]}:{r["InstrumentIds"]}"
                    )
                );

                if (hdSelectedInstruments.Value.Length == 0)
                {
                    divError.Visible = true;
                    lblMsg.Text = "Select Instruments to be calibrated";
                }
            }

            if ((ddlReportFormat.SelectedIndex == -1) || (ddlReportFormat.SelectedIndex == 0))
            {
                if (!(ddlDepartment.SelectedValue == "3006") &&
                    !(ddlDepartment.SelectedValue == "4007") &&
                   !(ddlDepartment.SelectedValue == "4011") &&
                   !(ddlDepartment.SelectedValue == "4012") &&
                   !(ddlDepartment.SelectedValue == "4013") &&
                   !(ddlDepartment.SelectedValue == "4009") &&
                   !(ddlDepartment.SelectedValue == "4006") &&
                   !(ddlDepartment.SelectedValue == "4008") &&
                   !(ddlDepartment.SelectedValue == "4014")&&
                   !(ddlDepartment.SelectedValue == "2") &&
                  !(ddlDepartment.SelectedValue == "5020"))
                {
                    ddlReportFormat.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    _cm.ReportID = "1";
                }
            }
            else
            {
                ddlReportFormat.BackColor = System.Drawing.Color.Empty;
                _cm.ReportID = ddlReportFormat.SelectedValue.ToString();
            }
            if (txtInquiryDetail.Text == "")
            {
                txtInquiryDetail.BorderColor = System.Drawing.Color.LightPink;
                divError.Visible = true;
                lblMsg.Text = "Enter Inquiry Details";
            }
            else
            {
                txtInquiryDetail.BackColor = System.Drawing.Color.Empty;
                if (!(System.Text.RegularExpressions.Regex.IsMatch(txtInquiryDetail.Text, @"^[\s\S]{0,5000}$")))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Maximum 1000 Characters are Allowed.')", true);
                    txtInquiryDetail.BorderColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
            }
            if (!(System.Text.RegularExpressions.Regex.IsMatch(txtInquiryDetail.Text, @"^[\s\S]{0,5000}$")))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert('Maximum 1000 Characters are Allowed.')", true);
            }
            else
            {
                if (flag == 0)
                {
                    if (Session["OrganizationID"] != null)
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());

                        string customerMasterID = ddlCustomerName.SelectedValue;
                        hdcustomerMasterID.Value = customerMasterID;
                        DataTable dt = _cm.GetCustomerID(Convert.ToInt32(customerMasterID));
                        foreach (DataRow dr in dt.Rows)
                        {
                            hdcustomerID.Value = dt.Rows[0]["customerID"].ToString();
                        }
                        int inq = 0;
                        _cm.inquiry = txtInquiryDetail.Text;
                        _cm.customerId = Convert.ToInt32(hdcustomerID.Value);
                        _cm.CustomerMasterId = Convert.ToInt32(hdcustomerMasterID.Value);
                        _cm.inquiryStatusId = 1;
                        _cm.created = DateTime.Now;
                        _cm.employeeDepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                        if ((ddlProductSubGroup.SelectedIndex != -1) && (ddlProductSubGroup.SelectedIndex != 0))
                        {
                            _cm.subgroupid = Convert.ToInt32(ddlProductSubGroup.SelectedValue);
                        }
                        else
                        {
                            _cm.subgroupid = 0;
                        }
                        int productSubGropuId = 0;
                        if ((ddlProductSubGroup.SelectedIndex != -1) && (ddlProductSubGroup.SelectedIndex != 0))
                        {
                            productSubGropuId = Convert.ToInt32(ddlProductSubGroup.SelectedValue);
                        }
                        if ((ddlStandard.SelectedIndex != -1) && (ddlStandard.SelectedIndex != 0))
                        {
                            _cm.StandardID = Convert.ToInt32(ddlStandard.SelectedValue);
                        }
                        else
                        {
                            _cm.StandardID = 0;
                        }
                        DataTable dtEnquiry = _cm.getLastEnquiryNo();
                        if (dtEnquiry.Rows.Count > 0)
                        {
                            _cm.EnquirySeries = Convert.ToInt32(dtEnquiry.Rows[0]["EnquirySeries"]) + 1;
                        }
                        else
                        {
                            _cm.EnquirySeries = 1;
                        }
                        //string sYear = DateTime.Now.AddHours(1.5).ToString("yyyy");
                        //_cm.RequestNo = sYear + _cm.EnquirySeries.ToString().PadLeft(6, '0');
                        string yearMonth = DateTime.Now.ToString("yyyyMM");
                        _cm.RequestNo = yearMonth + _cm.EnquirySeries.ToString().PadLeft(4, '0');

                        if ((ddlReportFormat.SelectedIndex == -1) || (ddlReportFormat.SelectedIndex == 0)) { _cm.ReportID = "1"; }
                        else { _cm.ReportID = ddlReportFormat.SelectedValue.ToString(); }

                        inq = _cm.AddInquiryDetails(_cm);
                        _cm.inquiryId = inq;
                        _cm.UpdateInquiryRequestNo(_cm);
                        _cm.inquiryId = inq;
                        _cm.testList = "";
                        _cm.methodList = "";
                        _cm.remarks = "";
                        _cm.part = "";
                        _cm.requirement = "";
                        _cm.standardlist = "";

                        if (ddlDepartment.SelectedValue == "4012")
                        {
                            if (hdSelectedInstruments.Value.Length > 0)
                            {
                                _cm.Instruments = hdSelectedInstruments.Value;
                                _cm.updateCalibInstrument(_cm);
                            }
                        }

                        List<string> uniqueStandardIds = new List<string>();
                        string CategoryTypeStd = "";
                        if (gvSelectedTests.Rows.Count > 0)
                        {
                            foreach (GridViewRow row in gvSelectedTests.Rows)
                            {
                                int index = row.RowIndex;
                                Label lblSelectedReportID = (Label)gvSelectedTests.Rows[index].FindControl("lblSelectedReportID");
                                Label lblSelectedMethod = (Label)gvSelectedTests.Rows[index].FindControl("lblSelectedMethod");
                                Label lblSelectedRemarks = (Label)gvSelectedTests.Rows[index].FindControl("lblSelectedRemarks");
                                Label lblSelectedPart = (Label)gvSelectedTests.Rows[index].FindControl("lblSelectedPart");
                                Label lblSelectedRequirement = (Label)gvSelectedTests.Rows[index].FindControl("lblSelectedRequirement");
                                Label lblSelectedStandardID = (Label)gvSelectedTests.Rows[index].FindControl("lblSelectedStandardID");
                                Label lblCategoryTypeStd = (Label)gvSelectedTests.Rows[index].FindControl("lblCategoryTypeStd");

                                if (lblSelectedReportID != null)
                                {
                                    _cm.testList += lblSelectedReportID.Text + ",";
                                    _cm.methodList += (lblSelectedMethod?.Text ?? "") + ",";
                                    _cm.remarks += (lblSelectedRemarks?.Text ?? "") + ",";
                                    _cm.part += (lblSelectedPart?.Text ?? "-") + ",";
                                    _cm.requirement += (lblSelectedRequirement?.Text ?? "-") + ",";
                                    _cm.CategoryTypeStdTest += lblCategoryTypeStd.Text;
                                    CategoryTypeStd = _cm.CategoryTypeStdTest;
                                    // Add to unique list only if not already present
                                    string standardId = lblSelectedStandardID.Text;
                                    if (!uniqueStandardIds.Contains(standardId))
                                    {
                                        uniqueStandardIds.Add(standardId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DataTable selectedTests = SelectedTestsData;
                            foreach (DataRow dr in selectedTests.Rows)
                            {
                                _cm.testList += dr["ReportID"].ToString() + ",";
                                _cm.methodList += dr["TestMethod"].ToString() + ",";
                                _cm.remarks += dr["Remarks"].ToString() + ",";
                                _cm.part += dr["Part"].ToString() + ",";
                                _cm.requirement += dr["Requirement"].ToString() + ",";
                                _cm.CategoryTypeStdTest = dr["CategoryTypeStd"].ToString();
                                CategoryTypeStd = dr["CategoryTypeStd"].ToString();
                                // Add to unique list only if not already present
                                string standardId = dr["StandardID"].ToString();
                                if (!uniqueStandardIds.Contains(standardId))
                                {
                                    uniqueStandardIds.Add(standardId);
                                }
                            }
                        }

                        string[] values = CategoryTypeStd.Split(',');
                        string[] distinctArray = values.Distinct().ToArray();
                        CategoryTypeStd = "";
                        foreach (string s in distinctArray)
                        {
                            if (s != "") { CategoryTypeStd += s + ","; }
                        }
                        if (CategoryTypeStd.Length > 0)
                        {
                            CategoryTypeStd = CategoryTypeStd.Remove(CategoryTypeStd.Length - 1);
                        }

                        _cm.CategoryTypeStdTest = CategoryTypeStd;
                        _cm.updateCategoryTypeStd(_cm);
                        // Create comma-separated string from unique StandardIDs
                        _cm.standardlist = string.Join(",", uniqueStandardIds);

                        // Remove trailing commas for other fields
                        if (_cm.testList.Length > 0)
                        {
                            _cm.testList = hdTests.Value;// _cm.testList.Remove(_cm.testList.Length - 1);
                            _cm.methodList = _cm.methodList.Remove(_cm.methodList.Length - 1);
                            _cm.remarks = _cm.remarks.Remove(_cm.remarks.Length - 1);
                            _cm.part = _cm.part.Remove(_cm.part.Length - 1);
                            _cm.requirement = _cm.requirement.Remove(_cm.requirement.Length - 1);
                        }
                        if (Session["Tests"] != null)
                        {
                            _cm.testList = Session["Tests"].ToString();
                        }
                        else
                        {
                            _cm.testList = hdTests.Value;
                        }
                        if (Session["Methods"] != null)
                        {
                            _cm.methodList = Session["Methods"].ToString();
                        }
                        else
                        {
                            _cm.methodList = hdMethods.Value;
                        }
                        if (Session["Parts"] != null)
                        {
                            _cm.partList = Session["Parts"].ToString();
                        }
                        else
                        {
                            _cm.partList = hdParts.Value;
                        }
                        if (Session["Reqmts"] != null)
                        {
                            _cm.reqmtList = Session["Reqmts"].ToString();
                        }
                        else
                        {
                            _cm.reqmtList = hdReqmts.Value;
                        }
                        if (Session["Remarks"] != null)
                        {
                            _cm.remarksList = Session["Remarks"].ToString();
                        }
                        else
                        {
                            _cm.remarksList = hdRemarks.Value;
                        }
                        _cm.updateTestList(_cm);

                        Clear();

                        ViewState["SelectedTestsData"] = null;

                        Response.Redirect("CustomerInquiryView.aspx");
                    }
                }
            }


        }
        private void Clear()
        {
            txtInquiryDetail.Text = "";
            ddlCustomerName.SelectedIndex = 0;
            // Clear Session
            Session["Tests"] = null;
            Session["Methods"] = null;
            Session["Parts"] = null;
            Session["Reqmts"] = null;
            Session["Remarks"] = null;

            // Clear hidden fields
            hdTests.Value = "";
            hdMethods.Value = "";
            hdParts.Value = "";
            hdReqmts.Value = "";
            hdRemarks.Value = "";

            // Clear ViewState
            ViewState["SelectedTestsData"] = null;
        }
        protected void txtInquiryDetail_TextChanged(object sender, EventArgs e)
        {

        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDepartment.SelectedIndex != 0)
            {
                int DepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                BindProductGroup(DepartmentID);
                //BindReportType(DepartmentID);
                if (DepartmentID == 4007)
                {
                    divFormat.Visible = false;
                    divGroup.Visible = false;
                    divCableType.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = true;
                    BindTestCategory(DepartmentID);
                }
                else if (DepartmentID == 4006)
                {
                    divCableType.Visible = false;
                    divFormat.Visible = true;
                    divGroup.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = true;
                    BindTestCategory(DepartmentID);
                    BindReportType(DepartmentID);
                }
                else if (DepartmentID == 2) //battery
                {
                    divCableType.Visible = false;
                    divFormat.Visible = true;
                    divGroup.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = true;
                    BindTestCategory(DepartmentID);
                    BindReportType(DepartmentID);
                }
                else if (DepartmentID == 3006)//Electrical Accessories
                {
                    divFormat.Visible = false;
                    divGroup.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = false;                   
                    divCableType.Visible = false;
                    divStandard.Visible = true;
                    //BindStandards(DepartmentID);
                    //BindTestCategory(DepartmentID);
                }
                else if (DepartmentID == 5020)  //Physical Softline
                {
                    divFormat.Visible = false;
                    divGroup.Visible = true;
                    divCategory.Visible = true;
                    divStandard.Visible = false;
                    divCableType.Visible = false;
                    divStandard.Visible = true;
                }
                else if (DepartmentID == 4009)
                {
                    divCableType.Visible = false;
                    divFormat.Visible = false;
                    divGroup.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = true;
                    BindTestCategory(DepartmentID);
                }
                else if (DepartmentID == 4006)
                {
                    divCableType.Visible = false;
                    divFormat.Visible = false;
                    divGroup.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = true;
                    BindTestCategory(DepartmentID);

                }
                else if (DepartmentID == 4008)
                {
                    divCableType.Visible = false;
                    divFormat.Visible = false;
                    divGroup.Visible = true;

                    divCategory.Visible = true;
                    divStandard.Visible = true;
                    BindTestCategory(DepartmentID);

                }
                else if (DepartmentID == 4012)//Calibration
                {
                    divGroup.Visible = true;
                    divCalibInstruments.Visible = true;

                    divFormat.Visible = false;
                    divCategory.Visible = false;
                    divStandard.Visible = false;

                    BindReportType(DepartmentID);
                }                
                else
                {
                    divFormat.Visible = true;
                    divGroup.Visible = true;

                    divCategory.Visible = false;
                    divStandard.Visible = false;

                    BindReportType(DepartmentID);
                }

                BindCustomer(DepartmentID);
                gvTests.DataSource = null;
                gvTests.DataBind();
            }

        }
        private void BindProductGroup(int DepartmentID)
        {
            DataTable dt = _cm.getProductGroup(DepartmentID);
            if (dt.Rows.Count > 0)
            {
                ddlProductGroup.DataSource = dt;
                ddlProductGroup.DataTextField = "ProductGroupName";
                ddlProductGroup.DataValueField = "ProductGroupID";
                ddlProductGroup.DataBind();
                ddlProductGroup.Items.Insert(0, new ListItem("Select Group", ""));
            }
            else
            {
                ddlProductGroup.DataSource = null;
                ddlProductGroup.DataBind();

                ddlProductSubGroup.DataSource = null;
                ddlProductSubGroup.DataBind();

                ddlTestCategory.DataSource = null;
                ddlTestCategory.DataBind();

                ddlStandard.DataSource = null;
                ddlStandard.DataBind();
            }
        }
        private void BindProductSubGroup(int GroupID)
        {
            DataTable dt = _cm.getProductSubGroup(GroupID);
            if (dt.Rows.Count > 0)
            {
                ddlProductSubGroup.DataSource = dt;
                ddlProductSubGroup.DataTextField = "SubGroupName";
                ddlProductSubGroup.DataValueField = "SubGroupID";
                ddlProductSubGroup.DataBind();
                ddlProductSubGroup.Items.Insert(0, new ListItem("Select Sub Group", ""));
            }
        }
        protected void ddlProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProductGroup.SelectedIndex != 0)
            {
                int GroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
                BindProductSubGroup(GroupID);
                if (ddlDepartment.SelectedIndex != 0)
                {
                    int DepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                    if (DepartmentID == 3006)//Electrical Accessories
                    {
                        if (ddlProductGroup.SelectedIndex != 0)
                        {
                            BindProductSubGroup(GroupID);
                            BindTestCategory(DepartmentID);
                            ddlTestCategory.DataSource = null;
                            ddlTestCategory.DataBind();
                            gvTests.DataSource = null;
                            gvTests.DataBind();
                        }
                        else
                        {
                            ddlTestCategory.DataSource = null;
                            ddlTestCategory.DataBind();
                        }

                    }
                    else if (DepartmentID == 5020)  //Physical Softline
                    {
                        if (ddlProductGroup.SelectedIndex != 0)
                        {
                            BindProductSubGroup(GroupID);
                            BindTestCategory(DepartmentID);
                            ddlTestCategory.DataSource = null;
                            ddlTestCategory.DataBind();
                            gvTests.DataSource = null;
                            gvTests.DataBind();
                        }
                        else
                        {
                            ddlTestCategory.DataSource = null;
                            ddlTestCategory.DataBind();
                        }

                    }

                }
            }
        }
        protected void ddlProductSubGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SubGroupID = Convert.ToInt32(ddlProductSubGroup.SelectedValue);
            if (ddlDepartment.SelectedValue == "4012")//Calibration
            {
                BindCalibInstruments(SubGroupID);
            }
        }

        protected void ddlCustomerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string customerMasterID = ddlCustomerName.SelectedValue;
            hdcustomerMasterID.Value = customerMasterID;
            DataTable dt = _cm.GetCustomerID(Convert.ToInt32(customerMasterID));
            foreach (DataRow dr in dt.Rows)
            {
                hdcustomerID.Value = dt.Rows[0]["customerID"].ToString();
            }
        }

        protected void ddlReportFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlReportFormat.SelectedIndex != 0)
            {
                hypReport.Text = "Format Like: " + ddlReportFormat.SelectedItem.ToString();
                hypReport.NavigateUrl = "~/InputFiles/" + ddlReportFormat.SelectedValue.ToString() + ".pdf";
                hypReport.Target = "_blank";
            }

        }

        protected void ddlStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStandard.SelectedIndex != 0)
            {
                //BindSampleRadioList(ddlStandard.SelectedValue.ToString());
                btnAddSelected.Visible = true;
                selectedtests.Visible = true;
            }
        }
        private void BindSampleRadioList(string StandardID)
        {
            DataTable dt = _cm.getStandardTests(StandardID);
            int j = 0;
            if (dt.Rows.Count > 0)
            {
                gvTests.DataSource = dt;
                gvTests.DataBind();
            }
            else
            {
                gvTests.DataSource = null;
                gvTests.DataBind();
            }
        }
        protected void txtCount_TextChanged(object sender, EventArgs e)
        {
            GridViewRow row = ((GridViewRow)((TextBox)sender).NamingContainer);
            int index = row.RowIndex;
            Label lblReportID = (Label)gvTests.Rows[index].FindControl("lblReportID");
            TextBox txtCount = (TextBox)gvTests.Rows[index].FindControl("txtCount");
            CheckBox cb1 = (CheckBox)gvTests.Rows[index].FindControl("chkTest");
            TextBox txtPart = (TextBox)gvTests.Rows[index].FindControl("txtPart");
            TextBox txtRequirement = (TextBox)gvTests.Rows[index].FindControl("txtRequirement");
            if (!cb1.Checked) { cb1.BorderColor = Color.Red; }
            if (txtCount.Text == "") { txtCount.BorderColor = Color.Red; }
            if ((txtCount.Text != "") && (cb1.Checked))
            {
                cb1.BorderColor = Color.Empty; txtCount.BorderColor = Color.Empty;
            }
        }
        protected void gvTests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            int ReportID = Convert.ToInt32(e.CommandArgument);
            CheckBox cbox = (CheckBox)row.Cells[1].Controls[0];
        }
        protected void chkTest_CheckedChanged(object sender, EventArgs e)
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            Label lblReportID = (Label)gvTests.Rows[index].FindControl("lblReportID");
            TextBox txtCount = (TextBox)gvTests.Rows[index].FindControl("txtCount");
            CheckBox cb1 = (CheckBox)gvTests.Rows[index].FindControl("chkTest");
            TextBox txtPart = (TextBox)gvTests.Rows[index].FindControl("txtPart");
            TextBox txtRequirement = (TextBox)gvTests.Rows[index].FindControl("txtRequirement");
            if (!cb1.Checked) { cb1.BorderColor = Color.Red; }

        }

        protected void gvTests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable dt = new DataTable();
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblMethod = (e.Row.FindControl("lblMethod") as Label);
                TextBox txtMethod = (e.Row.FindControl("txtMethod") as TextBox);
                if (!string.IsNullOrEmpty(lblMethod.Text))
                {
                    dt = _cm.getTestMethod(lblMethod.Text); // use only ID(s)
                    if (dt.Rows.Count > 0)
                    {
                        string methods = string.Join(", ", dt.AsEnumerable().Select(dr => dr["TestMethod"].ToString()));
                        txtMethod.Text = methods; // display names only
                    }
                }
            }

        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.Checked)
            {
                BindCustomer();
            }
            else
            {
                if ((ddlDepartment.SelectedIndex != 0)&& (ddlDepartment.SelectedIndex != -1))
                {
                    BindCustomer(Convert.ToInt32(ddlDepartment.SelectedValue));
                }
            }
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDivision.SelectedIndex != 0)
            {
                string DivisionID = ddlDivision.SelectedValue;
                BindDepartment(DivisionID);
                ddlTestCategory.DataSource = null;
                ddlTestCategory.DataBind();

                gvTests.DataSource = null;
                gvTests.DataBind();
            }
            else
            {
                ddlDepartment.Items.Clear();
                ddlDepartment.Items.Insert(0, new ListItem("", "0"));

                ddlCustomerName.Items.Clear();
                ddlCustomerName.Items.Insert(0, new ListItem("", "0"));

                ddlProductGroup.Items.Clear();
                ddlProductGroup.Items.Insert(0, new ListItem("", "0"));

                ddlProductSubGroup.Items.Clear();
                ddlProductSubGroup.Items.Insert(0, new ListItem("", "0"));

                ddlTestCategory.Items.Clear();
                ddlTestCategory.Items.Insert(0, new ListItem("", "0"));

                ddlStandard.Items.Clear();
                ddlStandard.Items.Insert(0, new ListItem("", "0"));

                gvTests.DataSource = null;
                gvTests.DataBind();
            }
        }
        protected void btnAddSelected_Click(object sender, EventArgs e)
        {
            DataTable selectedData = SelectedTestsData;

            string currentStandard = "";
            int currentStandardID = 0;
            if (ddlStandard.SelectedIndex > 0)
            {
                currentStandard = ddlStandard.SelectedItem.Text;
                currentStandardID = Convert.ToInt32(ddlStandard.SelectedValue);
            }
            else
            {
                currentStandard = "No Standard";
                currentStandardID = 0;
            }
            hdTests.Value += "[";
            hdMethods.Value += "[";
            hdParts.Value += "[";
            hdReqmts.Value += "[";
            hdRemarks.Value += "[";
            foreach (GridViewRow row in gvTests.Rows)
            {
                CheckBox chkTest = (CheckBox)row.FindControl("chkTest");

                if (chkTest != null && chkTest.Checked)
                {
                    Label lblReportID = (Label)row.FindControl("lblReportID");
                    TextBox txtMethod = (TextBox)row.FindControl("txtMethod");
                    TextBox txtPart = (TextBox)row.FindControl("txtPart");
                    TextBox txtRequirement = (TextBox)row.FindControl("txtRequirement");
                    TextBox txtRemarks = (TextBox)row.FindControl("txtRemarks");

                    if (lblReportID != null)
                    {
                        int reportID = Convert.ToInt32(lblReportID.Text);

                        int departmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                        if (departmentID == 3006 || departmentID == 5020 || departmentID == 4006 || departmentID == 4007 || departmentID == 4008 || departmentID == 4009 || departmentID == 2)
                        {
                            // Check if this test is already added to avoid duplicates //
                            /* //DataRow[] existingRows = selectedData.Select($"ReportID = {reportID}"); // single ReportID only */
                            DataRow[] existingRows = selectedData.Select($"ReportID = {reportID} AND StandardID = '{currentStandardID},'");

                            if (existingRows.Length == 0)
                            {
                                DataRow newRow = selectedData.NewRow();
                                newRow["ReportID"] = reportID;
                                newRow["ReportName"] = chkTest.Text;
                                newRow["Standard"] = currentStandard + ",";  // Add standard name
                                newRow["StandardID"] = currentStandardID + ",";  // Add standard ID
                                newRow["TestMethod"] = txtMethod?.Text ?? "";
                                newRow["Part"] = txtPart?.Text ?? "-";
                                newRow["Requirement"] = txtRequirement?.Text ?? "-";
                                newRow["Remarks"] = txtRemarks?.Text ?? "";
                                newRow["CategoryTypeStd"] = ddlTestCategory.SelectedValue + ":" + ddlCableType.SelectedValue + ":" + ddlStandard.SelectedValue + ",";

                                selectedData.Rows.Add(newRow);
                                hdTests.Value += reportID + ",";
                                hdMethods.Value += txtMethod.Text + ",";
                                hdParts.Value += txtPart.Text + ",";
                                hdReqmts.Value += txtRequirement.Text + ",";
                                hdRemarks.Value += txtRemarks.Text + ",";
                            }

                        }
                        else
                        {

                        }



                    }
                }
            }
            if (hdTests.Value.Length > 0)
            {
                hdTests.Value = hdTests.Value.Remove(hdTests.Value.Length - 1);
            }
            hdTests.Value += "]";
            if (hdMethods.Value.Length > 0)
            {
                hdMethods.Value = hdMethods.Value.Remove(hdMethods.Value.Length - 1);
            }
            hdMethods.Value += "]";
            if (hdParts.Value.Length > 0)
            {
                hdParts.Value = hdParts.Value.Remove(hdParts.Value.Length - 1);
            }
            hdParts.Value += "]";
            if (hdReqmts.Value.Length > 0)
            {
                hdReqmts.Value = hdReqmts.Value.Remove(hdReqmts.Value.Length - 1);
            }
            hdReqmts.Value += "]";
            if (hdRemarks.Value.Length > 0)
            {
                hdRemarks.Value = hdRemarks.Value.Remove(hdRemarks.Value.Length - 1);
            }
            hdRemarks.Value += "]";
            if (Session["Tests"] != null)
            {
                Session["Tests"] = Session["Tests"] + hdTests.Value;
            }
            else
            {
                Session["Tests"] = hdTests.Value;
            }
            if (Session["Methods"] != null)
            {
                Session["Methods"] = Session["Methods"] + hdMethods.Value;
            }
            else
            {
                Session["Methods"] = hdMethods.Value;
            }
            if (Session["Parts"] != null)
            {
                Session["Parts"] = Session["Parts"] + hdParts.Value;
            }
            else
            {
                Session["Parts"] = hdParts.Value;
            }
            if (Session["Reqmts"] != null)
            {
                Session["Reqmts"] = Session["Reqmts"] + hdReqmts.Value;
            }
            else
            {
                Session["Reqmts"] = hdReqmts.Value;
            }
            if (Session["Remarks"] != null)
            {
                Session["Remarks"] = Session["Remarks"] + hdRemarks.Value;
            }
            else
            {
                Session["Remarks"] = hdRemarks.Value;
            }
            // Update ViewState and bind the selected tests grid
            SelectedTestsData = selectedData;
            gvTests.DataSource = null;
            gvTests.DataBind();
            ddlStandard.ClearSelection();
            BindSelectedTestsGrid();
        }

        private void BindSelectedTestsGrid()
        {
            gvSelectedTests.DataSource = SelectedTestsData;
            gvSelectedTests.DataBind();
        }
        protected void btnRemove_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "RemoveTest")
            {
                int reportID = Convert.ToInt32(e.CommandArgument);
                DataTable selectedData = SelectedTestsData;

                // Find and remove the row
                for (int i = selectedData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = selectedData.Rows[i];
                    if (Convert.ToInt32(row["ReportID"]) == reportID)
                    {
                        selectedData.Rows.RemoveAt(i);
                        break;
                    }
                }

                // Update ViewState and rebind
                SelectedTestsData = selectedData;
                ViewState["SelectedTestsData"] = selectedData;

                // Rebuild hidden fields
                hdTests.Value = "[";
                hdMethods.Value = "[";
                hdParts.Value = "[";
                hdReqmts.Value = "[";
                hdRemarks.Value = "[";

                foreach (DataRow dr in selectedData.Rows)
                {
                    hdTests.Value += dr["ReportID"].ToString() + ",";
                    hdMethods.Value += dr["TestMethod"].ToString() + ",";
                    hdParts.Value += dr["Part"].ToString() + ",";
                    hdReqmts.Value += dr["Requirement"].ToString() + ",";
                    hdRemarks.Value += dr["Remarks"].ToString() + ",";
                }

                if (hdTests.Value.Length > 1) hdTests.Value = hdTests.Value.Remove(hdTests.Value.Length - 1);
                if (hdMethods.Value.Length > 1) hdMethods.Value = hdMethods.Value.Remove(hdMethods.Value.Length - 1);
                if (hdParts.Value.Length > 1) hdParts.Value = hdParts.Value.Remove(hdParts.Value.Length - 1);
                if (hdReqmts.Value.Length > 1) hdReqmts.Value = hdReqmts.Value.Remove(hdReqmts.Value.Length - 1);
                if (hdRemarks.Value.Length > 1) hdRemarks.Value = hdRemarks.Value.Remove(hdRemarks.Value.Length - 1);

                hdTests.Value += "]";
                hdMethods.Value += "]";
                hdParts.Value += "]";
                hdReqmts.Value += "]";
                hdRemarks.Value += "]";

                Session["Tests"] = hdTests.Value;
                Session["Methods"] = hdMethods.Value;
                Session["Parts"] = hdParts.Value;
                Session["Reqmts"] = hdReqmts.Value;
                Session["Remarks"] = hdRemarks.Value;
                BindSelectedTestsGrid();
            }
        }
        //New code (4/8/2025)
        private DataTable SelectedTestsData
        {
            get
            {
                if (ViewState["SelectedTestsData"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ReportID", typeof(int));
                    dt.Columns.Add("ReportName", typeof(string));
                    dt.Columns.Add("Standard", typeof(string));  // Standard Name
                    dt.Columns.Add("StandardID", typeof(string));   // Standard ID
                    dt.Columns.Add("TestMethod", typeof(string));
                    dt.Columns.Add("Part", typeof(string));
                    dt.Columns.Add("Requirement", typeof(string));
                    dt.Columns.Add("Remarks", typeof(string));
                    dt.Columns.Add("CategoryTypeStd", typeof(string));   // Concat Category + Cable Type + Standard
                    ViewState["SelectedTestsData"] = dt;
                }
                return (DataTable)ViewState["SelectedTestsData"];
            }
            set
            {
                ViewState["SelectedTestsData"] = value;
            }
        }

        protected void ddlTestCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTestCategory.SelectedIndex != 0)
            {
                //if (ddlDepartment.SelectedValue.ToString() != "3006")
                //{
                BindCableTypeStandards();
                ddlTestCategory.BackColor = Color.White;
                //}
            }
            else
            {
                ddlTestCategory.BackColor = Color.LightPink;
                gvTests.DataSource = null;
                gvTests.DataBind();
            }
        }

        protected void BindCalibInstruments(int SubGrouypID)
        {
            DataTable dt = _cm.getCalibInstruments(SubGrouypID);
            if (dt.Rows.Count > 0)
            {
                chkInstrument.DataSource = dt;
                chkInstrument.DataTextField = "InstrumentName";
                chkInstrument.DataValueField = "InstrumentID";
                chkInstrument.DataBind();
                divCalibInstruments.Visible = true;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["Preview"] as DataTable ?? GetPreviewTable();

            string groupId = ddlProductGroup.SelectedValue;
            string groupName = ddlProductGroup.SelectedItem.Text;

            string subGroupId = ddlProductSubGroup.SelectedValue;
            string subGroupName = ddlProductSubGroup.SelectedItem.Text;

            string instrumentIds = string.Join(",",
                chkInstrument.Items.Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => i.Value));

            string instrumentNames = string.Join(", ",
                chkInstrument.Items.Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => i.Text));

            if (string.IsNullOrEmpty(instrumentIds))
                return;
            string[] id = instrumentIds.Split(',');
            string[] name = instrumentNames.Split(',');
            for (int i = 0; i < id.Length; i++)
            {
                dt.Rows.Add(groupId, subGroupId, groupName, subGroupName,
                        id[i], name[i]);
            }


            ViewState["Preview"] = dt;

            gvPreview.DataSource = dt;
            gvPreview.DataBind();
            divInstruments.Visible = true;
        }

        protected void gvPreview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveRow")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                int rowIndex = row.RowIndex;

                DataTable dt = ViewState["Preview"] as DataTable;

                string instToRemove = e.CommandArgument.ToString().Trim();

                var names = dt.Rows[rowIndex]["InstrumentNames"].ToString()
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => x != instToRemove)
                    .ToList();

                dt.Rows.RemoveAt(rowIndex);


                ViewState["Preview"] = dt;

                gvPreview.DataSource = dt;
                gvPreview.DataBind();
            }
        }

    }
}