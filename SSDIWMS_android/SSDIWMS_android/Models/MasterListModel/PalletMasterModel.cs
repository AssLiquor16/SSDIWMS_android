using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.TransactionModels
{
    public class PalletMasterModel 
    {
        [PrimaryKey]
        public int PId { get; set; }
        public string PalletCode { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
        public string PalletStatus { get; set; }
        public string PalletDescription { get; set; }
    }
}
