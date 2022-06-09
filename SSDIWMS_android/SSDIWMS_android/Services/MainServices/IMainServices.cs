using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.MainServices
{
    public interface IMainServices
    {
        Task OnstartSetDefaulPreferences();
        Task RemovePreferences();
        Task TimerSyncAlltransaction();
        Task TimerCheckUser();
        Task ClearTransactionData();
        Task ClearAlldata();
        Task DateCheckTimerInit();
    }

}
