using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch
{
    public class BatchDetailsModel
    {
        [PrimaryKey][AutoIncrement]
        public int BatchDetId { get; set; }
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public int Qty { get; set; }
        public DateTime DateAdded{get;set;}
        public int TimesUpdated { get; set; }
    }
}
