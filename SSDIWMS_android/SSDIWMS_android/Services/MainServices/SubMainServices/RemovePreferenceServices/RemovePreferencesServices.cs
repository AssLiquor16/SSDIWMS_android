using SSDIWMS_android.Services.MainServices.SubMainServices.RemovePreferenceServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.SetPreferenceServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(RemovePreferencesServices))]
namespace SSDIWMS_android.Services.MainServices.SubMainServices.RemovePreferenceServices
{
    public class RemovePreferencesServices : IRemovePreferenceServices
    {
        public async Task RemovePreference()
        {
            Preferences.Remove("PrefUserId");
            Preferences.Remove("PrefUserFullname");
            Preferences.Remove("PrefUserRole");
            Preferences.Remove("PrefUserWarehouseAssignedId");
            Preferences.Remove("PrefPONumber");
            Preferences.Remove("PrefBillDoc");
            Preferences.Remove("PrefCvan");
            Preferences.Remove("PrefShipNo");
            Preferences.Remove("PrefShipLine");
            Preferences.Remove("PrefINCParDetDateCreated");
            Preferences.Remove("PrefBatchCode");
            Preferences.Set("PrefLoggedIn", false);
            await Task.CompletedTask;
        }
    }
}
