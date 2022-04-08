using SSDIWMS_android.Services.MainServices.SubMainServices.SetPreferenceServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SetPreferenceService))]
namespace SSDIWMS_android.Services.MainServices.SubMainServices.SetPreferenceServices
{
    public class SetPreferenceService : ISetPreferenceService
    {
        public Task OnStartSetPreference()
        {
            Preferences.Set("PrefServerAddress", "http://192.168.1.217:80/");
            Preferences.Set("PrefLocalAddress", "SSDIWMSLoc.db");
            Preferences.Set("PrefDateTimeFormat", "MM/dd/yyyy HH:mm:ss");
            Preferences.Set("PrefDateTimeTick", 1);
            return Task.CompletedTask;
        }
    }
}
