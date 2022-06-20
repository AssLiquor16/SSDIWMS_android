using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.StockCard
{
    public class StockCardsModel
    {
        
        public int SCardId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public int OnStock { get; set; }
        public int OnCommited { get; set; }
        public int OnBegginning { get; set; }
        public string ItemCategory1 { get; set; }
        public string ItemCategory2 { get; set; }
        public string Brand { get; set; }
        public string Warehouse_Location { get; set; }
        public DateTime DateCreated { get; set; }
        public int TimesUpdated { get; set; }
        public DateTime DateSync { get; set; }
        [PrimaryKey]
        public int StockCardLocalId { get; set; }
        public string Area { get; set; }
        public string Warehouse { get; set; }
    }
}
