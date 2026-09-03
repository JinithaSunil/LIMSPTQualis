using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
	public class AccountItem
    {
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
        public int Depth { get; set; }
        public string BalanceType { get; set; }

        public List<AccountItem> Children { get; set; } = new List<AccountItem>();
    }
}