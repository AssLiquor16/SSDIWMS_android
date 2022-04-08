using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.MasterListModel
{
    public class ItemMasterModel
    {
        [PrimaryKey]
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string ItemCat1 { get; set; }
        public string ItemCat2 { get; set; }
        public string Barcode { get; set; }
        public string CaseCode { get; set; }
        public string Status { get; set; }
        public string PalletCode { get; set; }
    }
}
