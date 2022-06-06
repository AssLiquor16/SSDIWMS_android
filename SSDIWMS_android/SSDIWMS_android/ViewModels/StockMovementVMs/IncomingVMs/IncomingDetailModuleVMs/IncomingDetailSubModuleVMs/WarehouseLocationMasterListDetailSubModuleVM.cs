using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs.IncomingDetailSubModuleVMs
{
    public class WarehouseLocationMasterListDetailSubModuleVM : ViewModelBase
    {
        ILocalWarehouseLocationMasterServices localDbWarehouseLocationMasterService;

        public WarehouseLocationMasterListDetailSubModuleVM()
        {
            localDbWarehouseLocationMasterService = DependencyService.Get<ILocalWarehouseLocationMasterServices>();
        }
    }
}
