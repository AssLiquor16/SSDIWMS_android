using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.MasterListModel
{
    public class DevicesMasterModel
    {
        [PrimaryKey]
        public int DevId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceSerial { get; set; }
    }
}
