using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.MasterListModel
{
    public class WarehouseModel
    {
        [PrimaryKey]
        public int WarehouseId { get; set; }
        public string W_Location { get; set; }
        public string W_LocationInitial { get; set; }
    }
}
