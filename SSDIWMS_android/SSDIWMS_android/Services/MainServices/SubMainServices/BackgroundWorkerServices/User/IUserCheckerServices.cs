using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.User
{
    public interface IUserCheckerServices
    {
        Task CheckLoginStatus();
    }
}
