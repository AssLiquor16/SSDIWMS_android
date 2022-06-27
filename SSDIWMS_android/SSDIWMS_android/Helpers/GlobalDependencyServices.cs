using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Defaults;
using SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.TransferTypesServices;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets.LPalletDetails;
using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.TransferTypesServices;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.SPalletDetails;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.STPalletMaster;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard.SStockTransferHistories;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.STWarehouseLocation;
using SSDIWMS_android.Services.DeviceServices;
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
        public IDroidDeviceServices droidService { get; }
        public ILocalWarehouseMasterServices localDbWarehouseService { get; }
        public ILocalWarehouseLocationMasterServices localDbWarehouseLocationService { get; }
        public ILocalPalletMasterServices localDbPalletMasterService { get; }
        public IServerDeviceServices serverDbDeviceService { get; }
        public IDateVerifierServices dateServices { get; }
        public IToastNotifService notifService { get; }
        public IMainServices mainService { get; }
        public ILocalArticleMasterServices localDbArticleMasterService { get; }
        public ISMLIncomingHeaderServices localDbIncomingHeaderService { get; }
        public ISMLIncomingDetailServices localDbIncomingDetailService { get; }
        public ISMLIncomingPartialDetailServices localDbIncomingParDetailService { get; }
        public ILPalletHeaderServices localDbPalletHeaderService { get; }
        public ILPalletDetailServices localDbPalletDetailsService { get; }
        public ISPalletHeaderServices serverDbPalletHeaderService { get; }
        public ISPalletDetailsServices serverDbPalletDetailsService { get; }
        public ISTPalletMasterServices serverDbTPalletMasterService { get; }
        public ISTWarehouseLocationServices serverDbTWarehouseLocationService { get; }
        public ISStockCardServices serverDbStockCardService { get; }
        public IServerWarehouseMasterServices serverDbWarehouseService { get; }
        public IStagingWarehouseLocationServices localDbStagingWarehouseLocationService { get; }
        public ISStockTransferHistoriesServices serverDbStockTransferHistoriesService { get; }
        public ISTransferTypesServices serverDbTransferTypesService { get; }
        public ILTransferTypesServices localDbTransferTypesService { get; }
        public ILIPServices localDbIpServices { get; }

        public GlobalDependencyServices()
        {
            droidService = DependencyService.Get<IDroidDeviceServices>();
            serverDbDeviceService = DependencyService.Get<IServerDeviceServices>();
            dateServices = DependencyService.Get<IDateVerifierServices>();
            notifService = DependencyService.Get<IToastNotifService>();
            mainService = DependencyService.Get<IMainServices>();

            localDbStagingWarehouseLocationService = DependencyService.Get<IStagingWarehouseLocationServices>();

            localDbWarehouseService = DependencyService.Get<ILocalWarehouseMasterServices>();
            localDbPalletMasterService = DependencyService.Get<ILocalPalletMasterServices>();
            localDbWarehouseLocationService = DependencyService.Get<ILocalWarehouseLocationMasterServices>();
            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            localDbPalletHeaderService = DependencyService.Get<ILPalletHeaderServices>();
            localDbPalletDetailsService = DependencyService.Get<ILPalletDetailServices>();
            serverDbPalletHeaderService = DependencyService.Get<ISPalletHeaderServices>();
            serverDbPalletDetailsService = DependencyService.Get<ISPalletDetailsServices>();
            serverDbTPalletMasterService = DependencyService.Get<ISTPalletMasterServices>();
            serverDbTWarehouseLocationService = DependencyService.Get<ISTWarehouseLocationServices>();
            serverDbStockCardService = DependencyService.Get<ISStockCardServices>();
            serverDbWarehouseService = DependencyService.Get<IServerWarehouseMasterServices>();
            serverDbStockTransferHistoriesService = DependencyService.Get<ISStockTransferHistoriesServices>();
            serverDbTransferTypesService = DependencyService.Get<ISTransferTypesServices>();
            localDbTransferTypesService = DependencyService.Get<ILTransferTypesServices>();
            localDbIpServices = DependencyService.Get<ILIPServices>();
        }
    }
}
