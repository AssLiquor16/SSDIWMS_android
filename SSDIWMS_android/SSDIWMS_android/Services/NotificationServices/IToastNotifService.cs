using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.NotificationServices
{
    public interface IToastNotifService
    {
        Task ToastNotif(string type, string msg);
        Task StaticToastNotif(string type, string msg);
    }
}
