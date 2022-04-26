using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.MasterListModel
{
    public class SitesModel
    {
        [PrimaryKey]
        public int SiteId { get; set; }
        public string SiteDescription { get; set; }
        public int WarehouseId { get; set; }
        public string SiteCompleteDesc { get; set; }
    }
}
