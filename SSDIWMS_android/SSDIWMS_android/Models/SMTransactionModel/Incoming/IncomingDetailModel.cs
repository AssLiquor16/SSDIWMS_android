using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming
{
    public class IncomingDetailModel
    {
        [PrimaryKey]
        public int INCDetId { get; set; }
        public int INCHeaderId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public int Qty { get; set; }
        public int Cqty { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int TimesUpdated { get; set; }
        public string POHeaderNumber { get; set; }
        public string QTYStatus { get; set; }
    }
}
