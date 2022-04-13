using SSDIWMS_android.Services.DeviceServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.User;
using SSDIWMS_android.Services.MainServices.SubMainServices.RemovePreferenceServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserCheckerServices))]
namespace SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.User
{
    public class UserCheckerServices : IUserCheckerServices
    {
        IServerUserServices serverDbUserServices;
        IDroidDeviceServices deviceServices;
        IRemovePreferenceServices removePrefServices;
        IToastNotifService notifServices;

        public UserCheckerServices()
        {
            serverDbUserServices = DependencyService.Get<IServerUserServices>();
            deviceServices = DependencyService.Get<IDroidDeviceServices>();
            removePrefServices = DependencyService.Get<IRemovePreferenceServices>();
            notifServices = DependencyService.Get<IToastNotifService>();

        }

        public async Task CheckLoginStatus()
        {
            await Task.Delay(50);
            Random min = new Random();
            int rmin = min.Next(20, 30);

            Device.StartTimer(TimeSpan.FromSeconds(rmin), () => {
                Task.Run(async () =>
                {
                    await Task.Delay(10);
                    var loggedInUserId = Preferences.Get("PrefUserId", 0);
                    var deviceSerial = deviceServices.GetDeviceInfo("Serial").ToUpperInvariant();
                    if(loggedInUserId != 0)
                    {
                        int[] intarray = { loggedInUserId };
                        var user = await serverDbUserServices.ReturnModel("Id", null, intarray);
                        if(user.LoginStatus != deviceSerial)
                        {
                            await removePrefServices.RemovePreference();
                            await notifServices.StaticToastNotif("Error", "Admin forced logout this account.");
                            await Task.Delay(3000);
                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                        }
                        else if(user == null)
                        {
                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                        }
                    }
                    
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }
    }
}
