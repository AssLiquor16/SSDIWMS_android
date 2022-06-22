using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.StockCard
{
    public class StockTransferHistoryModel
    {
        [PrimaryKey]
        public int STId { get; set; }
        public string MobileId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string PalletCode { get; set; }
        public string TransferType { get; set; }
        public string TransactionType { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime DateTransact { get; set; }
        public int UserId { get; set; }
        public int TimesUpdated { get; set; }
        public DateTime DateSync { get; set; }
        public string StockTransferLocalId { get; set; }
        public string Area { get; set; }
        public string Warehouse { get; set; }
        public int Qty { get; set; }
        public string Remarks { get; set; }
    }
}
