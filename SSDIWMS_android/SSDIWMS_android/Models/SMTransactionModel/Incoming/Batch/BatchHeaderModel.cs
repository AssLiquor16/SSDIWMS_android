using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch
{
    public class BatchHeaderModel
    {
        [PrimaryKey][AutoIncrement]
        public int BatchId { get; set; }
        public string BatchCode { get; set; }
        public int UserCreated { get; set; }
        public DateTime DateCreated{get;set;}
        public string Remarks { get; set; }
        public int TimesUpdated { get; set; }
        public DateTime DateSync { get; set; }
    }
}
