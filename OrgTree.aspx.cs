using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class OrgTree : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadTree();
        }
        private void LoadTree()
        {
            DataTable dt = _cm.GetOrgData();

            tvOrg.Nodes.Clear();

            foreach (DataRow row in dt.Select("PID IS NULL OR PID=0"))
            {
                TreeNode node = new TreeNode(
                    $"{row["EmployeeName"]} ({row["DesignationName"]})",
                    row["OrgID"].ToString());

                tvOrg.Nodes.Add(node);

                AddChildren(node, dt);
            }

            // Assign approval levels after the entire tree is built
            foreach (TreeNode node in tvOrg.Nodes)
            {
                AssignApprovalLevels(node); 
            }

            tvOrg.ExpandAll();
        }

        private void AddChildren(TreeNode parent, DataTable dt)
        {
            DataRow[] children = dt.Select("PID=" + parent.Value);

            foreach (DataRow row in children)
            {
                TreeNode child = new TreeNode(
                    $"{row["EmployeeName"]} ({row["DesignationName"]})",
                    row["OrgID"].ToString());

                parent.ChildNodes.Add(child);

                AddChildren(child, dt);
            }
        }
        private int AssignApprovalLevels(TreeNode node)
        {
            if (node.ChildNodes.Count == 0)
                return 0;   // Employee (not an approver)

            int maxChildLevel = 0;

            foreach (TreeNode child in node.ChildNodes)
            {
                maxChildLevel = Math.Max(maxChildLevel, AssignApprovalLevels(child));
            }

            int myLevel = maxChildLevel + 1;

            if (myLevel <= 4)
                node.Text = $"Level {myLevel} - {node.Text}";

            return myLevel;
        }
    }
}