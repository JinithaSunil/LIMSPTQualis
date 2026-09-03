using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class Purchase
    {
        private readonly string conString =
            ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        #region Enquiry

        public DataTable GetEnquiryList()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_GetEnquiryList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        #endregion

        #region Vendor

        public DataTable GetVendorList()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_GetVendorList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        #endregion

        #region Quotation Variance

        public DataTable GetQuotationVarianceAnalysis(
            int enquiryID,
            int vendorID,
            int revision,
            bool varianceOnly)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_GetQuotationVarianceAnalysis", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                cmd.Parameters.AddWithValue("@VendorID", vendorID);
                cmd.Parameters.AddWithValue("@VarianceOnly", varianceOnly);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetQuotationVarianceDetail(int vendorQuotationDetailsID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_GetQuotationVarianceDetail", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VendorQuotationDetailsID", vendorQuotationDetailsID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        #endregion

        #region Approval

        public bool ApproveQuotationVariance(
            int vendorQuotationDetailsID,
            string remarks,
            string approvedBy)
        {
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_ApproveQuotationVariance", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VendorQuotationDetailsID", vendorQuotationDetailsID);
                cmd.Parameters.AddWithValue("@Remarks", remarks);
                cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);

                con.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool RejectQuotationVariance(
            int vendorQuotationDetailsID,
            string remarks,
            string rejectedBy)
        {
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_RejectQuotationVariance", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VendorQuotationDetailsID", vendorQuotationDetailsID);
                cmd.Parameters.AddWithValue("@Remarks", remarks);
                cmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);

                con.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Export

        public DataTable ExportQuotationVariance(
            int enquiryID,
            int vendorID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("sp_ExportQuotationVariance", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EnquiryID", enquiryID);
                cmd.Parameters.AddWithValue("@VendorID", vendorID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        #endregion
    }
}
