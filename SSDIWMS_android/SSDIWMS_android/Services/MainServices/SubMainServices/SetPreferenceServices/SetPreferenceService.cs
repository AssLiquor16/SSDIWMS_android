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
        public async Task OnStartSetPreference()
        {
            Preferences.Set("PrefLocalAddress", "SSDIWMSLoc.db");
            Preferences.Set("PrefDateTimeFormat","dd MMM yyy hh:mm");
            Preferences.Set("PrefISMSyncing", false);
            Preferences.Set("PrefDateTimeTick", 20);
            var e = await SetRandomVal();
            Preferences.Set("PrefTimerLongRandomVal",e);
        }
        private async Task<int> SetRandomVal()
        {
            await Task.Delay(1);
            Random min = new Random();
            int rmin = min.Next(100, 150);
            return rmin;
        }
    }
}
