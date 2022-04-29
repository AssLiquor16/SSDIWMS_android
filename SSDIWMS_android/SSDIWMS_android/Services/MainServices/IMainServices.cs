﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.MainServices
{
    public interface IMainServices
    {
        Task OnstartSetDefaulPreferences();
        Task RemovePreferences();
        Task SyncIncomingTransaction();
        Task CheckUser();
        Task ClearTransactionData();
        Task ClearAlldata();
        Task DateCheckTimerInit();
    }

}
