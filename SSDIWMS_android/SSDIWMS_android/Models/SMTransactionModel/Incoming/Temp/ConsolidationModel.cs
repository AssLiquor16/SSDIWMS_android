using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp
{
    public class ConsolidationModel
    {
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public int Qty { get; set; }
        public int CQty { get; set; }
        public string QTYStatus { get; set; }
        public string Color { get; set; }
    }
}
