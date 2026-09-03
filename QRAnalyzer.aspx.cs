using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class QRAnalyzer : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0; int inquiryID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usertype"] != null)
                {
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void txtSampleID_TextChanged(object sender, EventArgs e)
        {
            string sampleUID = "";
            string sampleIDSeries = txtSampleID.Text;
            string sQuery = "select * from sampledetails where sampleidseries = '" + sampleIDSeries + "'";
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                sampleUID = dt.Rows[0]["SampleID"].ToString();
            }
            BindCustomer(sampleUID);
            BindSampleDetails(sampleUID);
            BindAllocationDetails(sampleUID);
            BindTestResultDetails(sampleUID);
        }
        protected void BindCustomer(string sampleUID)
        {
            string sQuery = @"select distinct c1.customerid,firstname + '' + lastname as customername,AddressLine1,AddressLine2,AddressLine3,Country,State,City from customerdetails c1
                                inner join LocationDetails l on l.customermasterid=c1.customermasterid 
                                inner join 	customerinquiry ci on ci.customermasterid = c1.customermasterid	
                                inner join CRFCloneList cr on cr.EnquiryID = ci.InquiryId
                                inner join sampledetails sd on sd.id = cr.sampleID
                                where sd.sampleid = " + sampleUID +
                                @" union
                                select distinct c2.companyid as customerid,companyname as customername,AddressLine1,AddressLine2,AddressLine3,Country,State,City from CompanyDetails c2
                                inner join LocationDetails l on l.customermasterid=c2.customermasterid 
                                inner join 	customerinquiry ci on ci.customermasterid = c2.customermasterid	
                                inner join CRFCloneList cr on cr.EnquiryID = ci.InquiryId
                                inner join sampledetails sd on sd.id = cr.sampleID
                                where sd.sampleid = " + sampleUID;
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                lblCustomerName.Text = dt.Rows[0]["CustomerName"].ToString();
                lblCustomerAddress.Text = dt.Rows[0]["AddressLine1"].ToString() + ',' + dt.Rows[0]["AddressLine2"].ToString() + ',' + dt.Rows[0]["AddressLine3"].ToString() + ',' + dt.Rows[0]["City"].ToString() + ',' + dt.Rows[0]["State"].ToString() + ',' + dt.Rows[0]["Country"].ToString();
            }
        }
        protected void BindSampleDetails(string sampleUID)
        {
            string sQuery = @"select sd.sampleIDSeries, sd.SampleID,sd.ReceivedDate,pg.productGroupName,ps.SubGroupName,tr.AnalysisStartingDate,tr.AnalysisCompletionDate,t.DestroyDate 
								from customerinquiry ci 
                                inner join ProductSubGroup ps on ci.productCategoryID = subgroupID
                                inner join ProductGroup pg on pg.productgroupID = ps.productgroupID
								inner join CRFCloneList cr on cr.enquiryid = ci.inquiryid
                                inner join sampleDetails sd on sd.ID = cr.sampleID 
                                left join testresultdetails tr on tr.sampleID = sd.id
                                left join TestResults t on t.sampleid = tr.sampleid
                                where sd.SampleID = " + sampleUID;
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                lblCategory.Text = dt.Rows[0]["productGroupName"].ToString();
                lblSubCategory.Text = dt.Rows[0]["subGroupName"].ToString();
                if (dt.Rows[0]["AnalysisStartingDate"].ToString() != "")
                {
                    lblAnalysisStart.Text = Convert.ToDateTime(dt.Rows[0]["AnalysisStartingDate"]).ToString("dd/MM/yyyy");
                }
                if (dt.Rows[0]["AnalysisCompletionDate"].ToString() != "")
                {
                    lblAnalysisEnd.Text = Convert.ToDateTime(dt.Rows[0]["AnalysisCompletionDate"]).ToString("dd/MM/yyyy");
                }
                if (dt.Rows[0]["ReceivedDate"].ToString() != "")
                {
                    lblReceivedDate.Text = Convert.ToDateTime(dt.Rows[0]["ReceivedDate"]).ToString("dd/MM/yyyy");
                }                
            } 
        }
        protected void BindAllocationDetails(string sampleUID)
        {
            string sQuery = @"select EmployeeDepartmentName,EmployeeName,supervisorname from allocation a inner join Employees e on a.EmployeeId = e.EmployeeID 
                                inner join employeedepartment ed on ed.EmployeeDepartmentID = e.DepartmentID 
                                inner join supervisor s on s.employeeid = e.ID 
                                inner join sampleDetails sd on sd.id= a.SampleId 
                                where sd.SampleID = " + sampleUID;
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                lblDepartment.Text = dt.Rows[0]["EmployeeDepartmentName"].ToString();
                lblTechnician.Text = dt.Rows[0]["EmployeeName"].ToString();
                lblManager.Text = dt.Rows[0]["supervisorname"].ToString();
            }
        }
        protected void BindTestResultDetails(string sampleUID)
        {
            string sQuery = @"select * from sampleDetails sd 
                                left join TestResultDetails td on td.sampleID=sd.ID       
                                left join TestResults tr on tr.sampleID=sd.ID           
                                where sd.SampleID = " + sampleUID;
            DataTable dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["DateOfMfg"].ToString() != "")
                {
                    lblManufacturedDate.Text = Convert.ToDateTime(dt.Rows[0]["DateOfMfg"]).ToString("dd/MM/yyyy");
                }
                if (dt.Rows[0]["DestroyDate1"].ToString() != "")
                {
                    lblDestroyDate.Text = Convert.ToDateTime(dt.Rows[0]["DestroyDate1"]).ToString("dd/MM/yyyy");
                }
            }
        }
    }
}