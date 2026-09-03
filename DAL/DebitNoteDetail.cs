using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    [Serializable]
    public class DebitNoteDetail
    {
        public int DetailID { get; set; }
        public int DebitNoteID { get; set; }

        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }

        public decimal Amount
        {
            get { return Qty * Rate; }
        }
    }
}