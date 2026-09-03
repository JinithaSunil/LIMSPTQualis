using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    [Serializable]
    public class DebitNoteMaster
    {
        public int DebitNoteID { get; set; }
        public string DebitNoteNo { get; set; }
        public DateTime DebitNoteDate { get; set; }

        public int SupplierID { get; set; }
        public string SupplierName { get; set; }

        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}