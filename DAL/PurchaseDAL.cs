using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class PurchaseDAL
    {
        public DataTable GetPRTracking(
                        string PRNo,
                        int DepartmentId,
                        int VendorId,
                        string Status,
                        string FromDate,
                        string ToDate,
                        int StartIndex,
                        int PageSize)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@PRNo",PRNo),
                new SqlParameter("@DepartmentId",DepartmentId),
                new SqlParameter("@VendorId",VendorId),
                new SqlParameter("@Status",Status),
                new SqlParameter("@FromDate",
                    string.IsNullOrWhiteSpace(FromDate)
                        ? (object)DBNull.Value
                        : Convert.ToDateTime(FromDate)),
                new SqlParameter("@ToDate",
                    string.IsNullOrWhiteSpace(ToDate)
                        ? (object)DBNull.Value
                        : Convert.ToDateTime(ToDate)),
                new SqlParameter("@Offset",StartIndex),
                new SqlParameter("@PageSize",PageSize)
            };

            return DbHelper.ExecuteStoredProcDT(
                        "sp_PRToPOTracking",
                        param);
        }
        public DataTable GetVendorList()
        {
            return DbHelper.ExecuteDataSet(
                        CommandType.StoredProcedure,
                        "sp_GetVendorList").Tables[0];
        }
        public DataTable GetDepartmentList()
        {
            return DbHelper.ExecuteDataSet(
                        CommandType.StoredProcedure,
                        "sp_GetDepartmentList").Tables[0];
        }

        public DataSet GetPRCompleteDetails(int PurchaseRequisitionID)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@PurchaseRequisitionID", PurchaseRequisitionID)
            };

            return DbHelper.ExecuteDataSet(
                    CommandType.StoredProcedure,
                    "sp_GetPRCompleteDetails",
                    param);
        }
        public DataTable GetGRNDetailsByPO(int purchaseOrderID)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@PurchaseOrderID", purchaseOrderID)
            };

            return DbHelper.ExecuteDataSet(
                CommandType.StoredProcedure,
                "sp_GetGRNDetailsByPO",
                param).Tables[0];
        }
        public DataTable GetGRNDetailsByPODetails(int poDetailsID)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@PODetailsID", poDetailsID)
            };

            return DbHelper.ExecuteDataSet(
                CommandType.StoredProcedure,
                "sp_GetGRNDetailsByPODetails",
                param).Tables[0];
        }
    }
}