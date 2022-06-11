using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date
{
    public interface IDateVerifierServices
    {
        Task DateTimerInitialize();
        Task<string> ValidateDateTime();
        Task DatetimeValidate();
    }
}
