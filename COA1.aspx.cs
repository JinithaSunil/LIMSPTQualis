using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class COA : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DataTable dtHeader;  // Level 2
        DataTable dtDetail;  // Level 3
        DataTable dtGroup;   // Level 1 (optional if you have group table)
        DataTable dtDetails;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCOA();
                //BindCOATree();
            }
        }
        private void LoadCOA()
        {
            dtGroup = _cm.GetCOA(1);   // Level 1 table
            dtHeader = _cm.GetCOA(2);  // Level 2 table
            dtDetail = _cm.GetCOA(3);  // Level 3 table
            dtDetails = _cm.GetCOADetails();
            BuildTree();
        }
        private void BuildTree()
        {
            tvCOA.Nodes.Clear();

            // LEVEL 1 → groups (e.g., Aktiva, Liability)
            foreach (DataRow g in dtGroup.Rows)
            {
                TreeNode n1 = new TreeNode(
                    $"{g["AccountCode"]}  {g["AccountName"]} {g["AccountNameEng"]}" ,
                    g["AccountID"].ToString()
                );

                tvCOA.Nodes.Add(n1);

                // LEVEL 2 → headers
                DataRow[] headers = dtHeader.Select($"ParentID = {g["AccountID"]}");

                foreach (DataRow h in headers)
                {
                    TreeNode n2 = new TreeNode(
                        $"{h["AccountCode"]}  {h["AccountName"]} {h["AccountNameEng"]}",
                        h["AccountID"].ToString()
                    );
                    n1.ChildNodes.Add(n2);

                    // LEVEL 3 → detail accounts
                    DataRow[] details = dtDetail.Select($"ParentID = {h["AccountID"]}");

                    foreach (DataRow d in details)
                    {
                        TreeNode n3 = new TreeNode(
                            $"{d["AccountCode"]}  {d["AccountName"]} {d["AccountNameEng"]}",
                            d["AccountID"].ToString()
                        );
                        n2.ChildNodes.Add(n3);

                        DataRow[] level4 = dtDetails.Select($"ParentID = {d["AccountID"]}");

                        foreach (DataRow L4 in level4)
                        {
                            TreeNode n4 = new TreeNode(
                                $"{L4["AccountCode"]}  {L4["AccountName"]} {L4["AccountNameEng"]}",
                                L4["AccountID"].ToString()
                            );

                            n3.ChildNodes.Add(n4);
                        }
                    }

                }
            }
        }

        private void BindCOATree()
        {
            DataTable dt = GetCOA();

            // Build root nodes (no ParentID)
            foreach (DataRow row in dt.Select("ParentID=0"))
            {
                TreeNode root = new TreeNode(
                    $"{row["AccountID"]}  {row["AccountName"]}",
                    row["AccountID"].ToString()
                );

                tvCOA.Nodes.Add(root);
                AddChildNodes(root, dt);
            }
        }

        private void AddChildNodes(TreeNode parentNode, DataTable dt)
        {
            DataRow[] children = dt.Select($"ParentID = '{parentNode.Value}'");

            foreach (DataRow child in children)
            {
                TreeNode node = new TreeNode(
                    $"{child["AccountID"]}  {child["AccountName"]}",
                    child["AccountID"].ToString()
                );

                parentNode.ChildNodes.Add(node);

                // Recursively add grandchildren
                AddChildNodes(node, dt);
            }
        }

        private DataTable GetCOA()
        {
            string conStr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);

            SqlDataAdapter da = new SqlDataAdapter(@"
        SELECT AccountID, ParentID, AccountName,AccountCode
        FROM ChartOfAccountsHeader
        ORDER BY AccountID
    ", con);

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}