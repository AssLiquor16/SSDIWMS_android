using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(DateVerifierServices))]
namespace SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date
{
    public class DateVerifierServices : IDateVerifierServices
    {
        IServerDeviceServices serverDbDeviceService;
        IToastNotifService notifyService;
        public DateVerifierServices()
        {
            serverDbDeviceService = DependencyService.Get<IServerDeviceServices>();
            notifyService = DependencyService.Get<IToastNotifService>();
        }

        public async Task DateTimerInitialize()
        {
            await Task.Delay(50);
            Random min = new Random();
            int rmin = min.Next(20, 30);

            Device.StartTimer(TimeSpan.FromSeconds(rmin), () => {
                Task.Run(async () =>
                {
                    var status = await ValidateDateTime();
                    switch (status)
                    {
                        case "Advance":
                            await notifyService.ToastNotif("Error", "Your device time is advance.");
                            break;
                        case "Delay":
                            await notifyService.ToastNotif("Error", "Your device time is delay.");
                            break;
                        default: break;
                            
                    }
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }

        public async Task<string> ValidateDateTime()
        {
            try
            {
                var serverTime = await serverDbDeviceService.GetServerDate();
                var localDate = DateTime.Now;

                var delayDate = serverTime.AddMinutes(-2);
                var advanceDate = serverTime.AddMinutes(2);

                if (localDate < advanceDate && localDate > delayDate)
                {
                    return "Synced";
                }
                else if(localDate > advanceDate)
                {
                    return "Advance";
                }
                else
                {
                    return "Delay";
                }

            }
            catch
            {
                return null;
            }
        }
    }
}
