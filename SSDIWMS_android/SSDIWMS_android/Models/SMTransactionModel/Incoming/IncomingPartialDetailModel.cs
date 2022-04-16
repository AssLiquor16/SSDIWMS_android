using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming
{
    public class IncomingPartialDetailModel
    {
        [PrimaryKey,AutoIncrement]
        public int INCParDetId { get; set; }
        public DateTime DateCreated { get; set; }
        public string RefId { get; set; }           // INCDetId + P + INCParDetId
        public int INCDetId { get; set; }
        public int INCHeaderId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public int PartialCQTY { get; set; }
        public string PalletCode { get; set; }
        public int UserId { get; set; }
        public int TimesUpdated { get; set; }
        public string POHeaderNumber { get; set; }
        public string Status { get; set; }
        public DateTime DateFinalized { get; set; }
    }
}
