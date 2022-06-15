using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets;
using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSDIWMS_android.Helpers
{
    public class GlobalDependencyServices : ViewModelBase
    {
        public IServerDeviceServices serverDbDeviceService { get; }
        public IDateVerifierServices dateServices { get; }
        public IToastNotifService notifService { get; }
        public IMainServices mainService { get; }
        public ILocalArticleMasterServices localDbArticleMasterService { get; }
        public ISMLIncomingHeaderServices localDbIncomingHeaderService{ get; }
        public ISMLIncomingDetailServices localDbIncomingDetailService { get; }
        public ISMLIncomingPartialDetailServices localDbIncomingParDetailService { get; }
        public ILPalletHeaderServices localDbPalletHeaderService { get; }

        public GlobalDependencyServices()
        {
            serverDbDeviceService = DependencyService.Get<IServerDeviceServices>();
            dateServices = DependencyService.Get<IDateVerifierServices>();
            notifService = DependencyService.Get<IToastNotifService>();
            mainService = DependencyService.Get<IMainServices>();

            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            localDbPalletHeaderService = DependencyService.Get<ILPalletHeaderServices>();

            
        }
    }
}
