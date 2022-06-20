using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Temp
{
    public class ItemWithQtyModel
    {
        public ItemMasterModel Item { get; set; }
        public int Qty { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
