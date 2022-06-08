using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.StockMovementIncoming;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.User;
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
        IISMSyncerServices incomingstocksyncer;
        IUserCheckerServices userCheckerService;
        IDateVerifierServices dateCheckerService;

        //masterdata
        ILocalArticleMasterServices artMasterService;
        ILocalPalletMasterServices palletMasterService;


        //transaction
        ISMLIncomingDetailServices incDetService;
        ISMLIncomingHeaderServices incHeadService;
        ISMLIncomingPartialDetailServices incParDetService;
        ISMLBatchHeaderServices batchHeadservice;
        ISMLBatchDetailsServices batchDetailservice;
        public MainServices()
        {
            onstartPrefService = DependencyService.Get<ISetPreferenceService>();
            removePrefService = DependencyService.Get<IRemovePreferenceServices>();
            incomingstocksyncer = DependencyService.Get<IISMSyncerServices>();
            userCheckerService = DependencyService.Get<IUserCheckerServices>();
            dateCheckerService = DependencyService.Get<IDateVerifierServices>();

            //masterdata
            artMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            palletMasterService = DependencyService.Get<ILocalPalletMasterServices>();

            //transactions
            incDetService = DependencyService.Get<ISMLIncomingDetailServices>();
            incHeadService = DependencyService.Get<ISMLIncomingHeaderServices>();
            incParDetService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            batchHeadservice = DependencyService.Get<ISMLBatchHeaderServices>();
            batchDetailservice = DependencyService.Get<ISMLBatchDetailsServices>();
        }

        public async Task CheckUser()
        {
            await userCheckerService.CheckLoginStatus();
        }
        public async Task SyncIncomingTransaction()
        {
            await incomingstocksyncer.ISMLSyncer();
        }
        public async Task OnstartSetDefaulPreferences()
        {
            await onstartPrefService.OnStartSetPreference();
        }
        public async Task RemovePreferences()
        {
            await removePrefService.RemovePreference();
        }
        public async Task ClearTransactionData()
        {
            await incDetService.Clear();
            await incHeadService.Clear();
            await incParDetService.Clear();
            await batchHeadservice.Remove("All");
            await batchDetailservice.Remove("All");
        }
        public async Task ClearAlldata()
        {
            await artMasterService.Clear();
            await palletMasterService.Clear();
            await ClearTransactionData();
        }
        public async Task DateCheckTimerInit()
        {
            await dateCheckerService.DateTimerInitialize();
        }
    }
}
