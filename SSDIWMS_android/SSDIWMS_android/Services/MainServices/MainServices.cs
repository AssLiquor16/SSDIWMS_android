using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
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

        //masterdata
        ILocalArticleMasterServices artMasterService;
        ILocalPalletMasterServices palletMasterService;


        //transaction
        ISMLIncomingDetailServices incDetService;
        ISMLIncomingHeaderServices incHeadService;
        ISMLIncomingPartialDetailServices incParDetService;
        public MainServices()
        {
            onstartPrefService = DependencyService.Get<ISetPreferenceService>();
            removePrefService = DependencyService.Get<IRemovePreferenceServices>();
            userCheckerService = DependencyService.Get<IUserCheckerServices>();
            percentageCalculatorServices = DependencyService.Get<IPercentageCalculatorServices>();

            //masterdata
            artMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            palletMasterService = DependencyService.Get<ILocalPalletMasterServices>();

            //transactions
            incDetService = DependencyService.Get<ISMLIncomingDetailServices>();
            incHeadService = DependencyService.Get<ISMLIncomingHeaderServices>();
            incParDetService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
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
        public async Task ClearTransactionData()
        {
            await incDetService.Clear();
            await incHeadService.Clear();
            await incParDetService.Clear();
        }
        public async Task ClearAlldata()
        {
            await artMasterService.Clear();
            await palletMasterService.Clear();
            await ClearTransactionData();
        }
    }
}
