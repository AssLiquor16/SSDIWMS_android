using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.User;
using SSDIWMS_android.Services.MainServices.SubMainServices.PercentageCalculatorServices;
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
        IUserCheckerServices userCheckerService;
        IPercentageCalculatorServices percentageCalculatorServices;
        public MainServices()
        {
            onstartPrefService = DependencyService.Get<ISetPreferenceService>();
            removePrefService = DependencyService.Get<IRemovePreferenceServices>();
            userCheckerService = DependencyService.Get<IUserCheckerServices>();
            percentageCalculatorServices = DependencyService.Get<IPercentageCalculatorServices>();
        }

        public async Task CheckUser()
        {
            await userCheckerService.CheckLoginStatus();
        }
        public async Task OnstartSetDefaulPreferences()
        {
            await onstartPrefService.OnStartSetPreference();
        }
        public async Task RemovePreferences()
        {
            await removePrefService.RemovePreference();
        }
        public async Task<string> GetPercentage(string type, decimal[] decimalarray)
        {
            var ans = await percentageCalculatorServices.GetPercentage(type, decimalarray);
            return ans;
        }
    }
}
