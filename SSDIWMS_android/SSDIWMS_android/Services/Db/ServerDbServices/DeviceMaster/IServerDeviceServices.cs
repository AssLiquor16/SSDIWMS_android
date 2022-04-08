using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.Devices
{
    public interface IServerDeviceServices
    {
        Task<int> ReturnInt(string type, string[] stringdata, int[] intdata);
        Task InsertData(string type, string[] stringdata, int[] intdata);
    }
}
