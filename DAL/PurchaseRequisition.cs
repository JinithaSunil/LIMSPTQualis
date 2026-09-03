using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
	public class PurchaseRequisition
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

        string sQuery = "";
        public DataTable GetDataUsingQuery(string sQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                if (_sCon.State != ConnectionState.Open)
                {
                    _sCon.Open();
                }
                SqlCommand cmd = new SqlCommand(sQuery, _sCon);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

            }
            catch
            {

            }
            return dt;
        }
        public DataTable getApprovalList(string PRID)
        {
            DataTable dt = new DataTable();
            string sQuery = @"select currenttier,currentApproverRole, al.* from PurchaseRequisition pr
                            left join ApprovalLevels al on al.ApprovalLevel = pr.currentTier+1
                            where PRID = '" + PRID + "'  order by al.position asc";

            dt = GetDataUsingQuery(sQuery);
            return dt;
        }
    }
    public enum ApprovalTier
    {
        Tier1 = 1,
        Tier2 = 2,
        Tier3 = 3,
        Tier4 = 4
    }

    public enum ApproverRole
    {
        Manager,
        ITManager,
        QMSManager,
        HCGAManager,
        MaintenanceManager,
        SalesMarketingDirector,
        LabDirector,
        Director
    }

}