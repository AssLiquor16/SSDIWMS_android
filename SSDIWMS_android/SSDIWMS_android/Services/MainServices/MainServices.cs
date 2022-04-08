using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.RemovePreferenceServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.SetPreferenceServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(MainServices))]
namespace SSDIWMS_android.Services.MainServices
{
    public class MainServices : IMainServices
    {
        ISetPreferenceService onstartPrefService;
        IRemovePreferenceServices removePrefService;
        public MainServices()
        {
            onstartPrefService = DependencyService.Get<ISetPreferenceService>();
            removePrefService = DependencyService.Get<IRemovePreferenceServices>();
        }


        public async Task OnstartSetDefaulPreferences()
        {
            await onstartPrefService.OnStartSetPreference();
        }

        public async Task RemovePreferences()
        {
            await removePrefService.RemovePreference();
        }
    }
}
