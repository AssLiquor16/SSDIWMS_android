using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.DefaultModel
{
    public class StagingWarehouseLocationModel
    {
        [PrimaryKey][AutoIncrement]
        public int LocalLocId { get; set; }
        public int LocId { get; set; }
        public string Warehouse { get; set; }
        public string Area { get; set; }
        public string Rack { get; set; }
        public string Level { get; set; }
        public string Bin { get; set; }
        public string UOM { get; set; }
        public string Final_Location { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool MultiplePallet { get; set; }
        public bool IsBlockStock { get; set; }
        public int MaxPallet { get; set; }
    }
}
