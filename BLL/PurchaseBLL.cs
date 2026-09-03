using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using LIMS.DAL;

namespace LIMS.BLL
{
    public class PurchaseBLL
    {
        PurchaseDAL _dal = new PurchaseDAL();
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
            return _dal.GetPRTracking(
                    PRNo,
                    DepartmentId,
                    VendorId,
                    Status,
                    FromDate,
                    ToDate,
                    StartIndex,
                    PageSize);
        }
        public DataTable GetVendorList()
        {
            return _dal.GetVendorList();
        }
        public DataTable GetDepartmentList()
        {
            return _dal.GetDepartmentList();
        }
        public DataSet GetPRCompleteDetails(int PurchaseRequisitionID)
        {
            return _dal.GetPRCompleteDetails(PurchaseRequisitionID);
        }
        public DataTable GetGRNDetailsByPO(int purchaseOrderID)
        {
            return _dal.GetGRNDetailsByPO(purchaseOrderID);
        }
        public DataTable GetGRNDetailsByPODetails(int poDetailsID)
        {
            return _dal.GetGRNDetailsByPODetails(poDetailsID);
        }
    }

}