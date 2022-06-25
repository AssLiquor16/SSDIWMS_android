using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Pallet
{
    public class PalletHeaderModel
    {
        
        public int PHeadId { get; set; }
        public string PalletCode { get; set; }
        public string WarehouseLocation { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public int TimesUpdated { get; set; }
        public DateTime DateSync { get; set; }
        [PrimaryKey]
        [AutoIncrement]
        public int PHeaderLocalID { get; set; }
        public string PHeaderRefID { get; set; }
        public string Warehouse { get; set; }
        public string Area { get; set; }
        public bool IsTransferable { get; set; }
    }
}
