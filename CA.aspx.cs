using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;
using static LIMS.DAL.AccountItem;

namespace LIMS
{
	public partial class CA : System.Web.UI.Page
	{
        
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                // Load your hierarchy here
                var accountTree = LoadAccountHierarchy();

                rptAccounts.DataSource = accountTree;
                rptAccounts.DataBind();
            }
        }
        protected void rptAccounts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                AccountItem item = (AccountItem)e.Item.DataItem;

                Repeater rptChildren = (Repeater)e.Item.FindControl("rptChildren");

                if (item.Children != null && item.Children.Count > 0)
                {
                    rptChildren.DataSource = item.Children;
                    rptChildren.DataBind();
                }
            }
        }

        List<AccountItem> BuildHierarchy(List<AccountItem> flatList)
        {
            var lookup = flatList.ToDictionary(x => x.AccountNumber);

            List<AccountItem> roots = new List<AccountItem>();

            foreach (var item in flatList)
            {
                string parentNumber = GetParentNumber(item.AccountNumber); // e.g. "1.1" → "1"

                if (lookup.ContainsKey(parentNumber))
                    lookup[parentNumber].Children.Add(item);
                else
                    roots.Add(item);
            }

            return roots;
        }
        string GetParentNumber(string accountNumber)
        {
            int pos = accountNumber.LastIndexOf('.');
            if (pos == -1)
                return null;  // this is a root

            return accountNumber.Substring(0, pos);
        }
        List<AccountItem> LoadAccountHierarchy()
        {
            // This simulates a flat list loaded from Excel or DB:
            var flat = new List<AccountItem>
    {
        new AccountItem { AccountNumber="1",     AccountTitle="Assets", Depth=0, BalanceType="Dr" },
        new AccountItem { AccountNumber="1.1",   AccountTitle="Cash and Financial Assets", Depth=1, BalanceType="Dr" },
        new AccountItem { AccountNumber="1.1.1", AccountTitle="Cash and Cash Equivalents", Depth=2, BalanceType="Dr" },
        new AccountItem { AccountNumber="1.1.2", AccountTitle="Financial Assets and Investments", Depth=2, BalanceType="Dr" },
        new AccountItem { AccountNumber="1.2",   AccountTitle="Receivables and Contracts", Depth=1, BalanceType="Dr" },
        new AccountItem { AccountNumber="1.2.1", AccountTitle="Accounts, Notes and Loans", Depth=2, BalanceType="Dr" },
        new AccountItem { AccountNumber="1.2.2", AccountTitle="Contracts with Customers", Depth=2, BalanceType="Dr" },
        new AccountItem { AccountNumber="4",     AccountTitle="Revenue", Depth=0, BalanceType="Cr" },
        new AccountItem { AccountNumber="4.1",   AccountTitle="Recognized at Point of Time", Depth=1, BalanceType="Cr" },
        new AccountItem { AccountNumber="4.1.1", AccountTitle="Goods", Depth=2, BalanceType="Cr" },
    };

            return BuildHierarchy(flat);
        }

    }
}