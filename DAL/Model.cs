using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS.DAL
{
    public class Model
    {
    }
    public class SampleModel
    {
        public int SampleId { get; set; }
        public string SampleNo { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
    }
}