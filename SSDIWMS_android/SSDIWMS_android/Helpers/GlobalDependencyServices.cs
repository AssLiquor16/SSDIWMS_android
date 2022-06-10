using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SSDIWMS_android.Helpers
{
    public class GlobalDependencyServices : ViewModelBase
    {
        public ILocalArticleMasterServices localDbArticleMasterService { get; }
        public ISMLIncomingDetailServices localDbIncomingDetailService { get; }

        public GlobalDependencyServices()
        {
            localDbArticleMasterService = DependencyService.Get<ILocalArticleMasterServices>();
            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
        }
    }
}
