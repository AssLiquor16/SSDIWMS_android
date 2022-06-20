using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Pallet
{
    public class PalletDetailsModel
    {
        public int PCreationId { get; set; }
        public int PHeadId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public int Qty { get; set; }
        public DateTime DateCreated { get; set; }
        public int TimesUpdated { get; set; }
        public DateTime DateSync { get; set; }
        [PrimaryKey][AutoIncrement]
        public int PHeaderLocalID { get; set; }
        public string PDetRefId { get; set; }
        public string PalletCode { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
