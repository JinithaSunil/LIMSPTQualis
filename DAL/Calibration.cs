using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
	public class Calibration
	{
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

        string sQuery = "";
        DataTable dt = new DataTable();

        public int GetReseedIdentity(string table, string identityField)
        {
            try
            {
                if (_sCon.State == System.Data.ConnectionState.Open)
                    _sCon.Close();
                _sCon.Open();
                SqlCommand cmd = new SqlCommand(sQuery, _sCon);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select max(" + identityField + ") from " + table;
                int ReseedIdentity = (int)cmd.ExecuteScalar();
                if (_sCon.State == System.Data.ConnectionState.Open)
                    _sCon.Close();
                return ReseedIdentity;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
               // HttpContext.Current.Response.Redirect("~/Error.aspx", false);
                return 0;
            }
        }
        public void ReseedIdentity(string table, int reseedIdentity)
        {
            if (_sCon.State == System.Data.ConnectionState.Open)
                _sCon.Close();
            string sQuery = "DBCC CHECKIDENT('" + table + "', RESEED, " + reseedIdentity + ")";
            _sCon.Open();
            SqlCommand cmd = new SqlCommand(sQuery, _sCon);
            int modified = cmd.ExecuteNonQuery();
            if (_sCon.State == System.Data.ConnectionState.Open)
                _sCon.Close();
        }
        public int AddWorkOrderDetails(DAL.CM obj)
        {
            if (_sCon.State == System.Data.ConnectionState.Open)
                _sCon.Close();

            //Start Reseed to avoid identity jump
            int prevIdentity = GetReseedIdentity("WorkOrder", "WorkOrderID");
            ReseedIdentity("WorkOrder", prevIdentity);
            //End Reseed to avoid identity jump

            string sQuery = @"insert into WorkOrder(QuotationID,
                                WONumber,
                                DateReceived,
                                ServiceDueDate,
                                CertificateIssueDate,
                                ApplicationNo,
                                CustomerMasterID,
                                PreparedBy,
                                PreparedDate,
                                CheckedBy,
                                CheckedDate,
                                ReceivedBy,
                                ReceivedDate,
                                ReturnedBy,
                                ReturnedDate,
                                Lab,
                                CreatedDate, SampleID)output INSERTED.WorkOrderID values(@QuotationID,
                                @WONumber,
                                @DateReceived,
                                @ServiceDueDate,
                                @CertificateIssueDate,
                                @ApplicationNo,
                                @CustomerMasterID,
                                @PreparedBy,
                                @PreparedDate,
                                @CheckedBy,
                                @CheckedDate,
                                @ReceivedBy,
                                @ReceivedDate,
                                @ReturnedBy,
                                @ReturnedDate,
                                @Lab,
                                @CreatedDate,@SampleID)";
            using (SqlCommand cmd = new SqlCommand(sQuery, _sCon))
            {
                cmd.Parameters.AddWithValue("@QuotationID", obj.QuotationID);
                cmd.Parameters.AddWithValue("@WONumber", obj.WONumber);
                cmd.Parameters.AddWithValue("@DateReceived", obj.DateReceived);
                cmd.Parameters.AddWithValue("@ServiceDueDate", obj.ServiceDueDate);
                cmd.Parameters.AddWithValue("@CertificateIssueDate", obj.CertificateIssueDate);
                cmd.Parameters.AddWithValue("@ApplicationNo", obj.ApplicationNo);
                cmd.Parameters.AddWithValue("@CustomerMasterID", obj.CustomerMasterID);
                cmd.Parameters.AddWithValue("@PreparedBy", obj.PreparedBy);
                cmd.Parameters.AddWithValue("@PreparedDate", obj.PreparedDate);
                cmd.Parameters.AddWithValue("@CheckedBy", obj.CheckedBy);
                cmd.Parameters.AddWithValue("@CheckedDate", string.IsNullOrEmpty(obj.CheckedDate) ? (object)DBNull.Value : obj.CheckedDate);
                cmd.Parameters.AddWithValue("@ReceivedBy", obj.ReceivedBy);
                cmd.Parameters.AddWithValue("@ReceivedDate", string.IsNullOrEmpty(obj.ReceivedDate) ? (object)DBNull.Value : obj.ReceivedDate);
                cmd.Parameters.AddWithValue("@ReturnedBy", obj.ReturnedBy);
                cmd.Parameters.AddWithValue("@ReturnedDate", string.IsNullOrEmpty(obj.ReturnedDate) ? (object)DBNull.Value : obj.ReturnedDate);
                cmd.Parameters.AddWithValue("@Lab", obj.Lab);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@SampleID", obj.sampleID);

                _sCon.Open();
                int modified = (int)cmd.ExecuteScalar();

                if (_sCon.State == System.Data.ConnectionState.Open)
                    _sCon.Close();
                return modified;
            }
        }
    }
}