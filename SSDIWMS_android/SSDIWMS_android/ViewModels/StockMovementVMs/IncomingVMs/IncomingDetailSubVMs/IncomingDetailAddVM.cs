using MvvmHelpers.Commands;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailSubVMs
{
    public class IncomingDetailAddVM : ViewModelBase
    {
        ISMLIncomingDetailServices localDbIncomingDetailService;
        ILocalArticleMasterServices localDbItemMasterService;

        string _scannedCode;
        public string ScannedCode { get => _scannedCode; set => SetProperty(ref _scannedCode, value); }
        public AsyncCommand PageRefrehCommand { get; }
        public IncomingDetailAddVM()
        {

            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            localDbItemMasterService = DependencyService.Get<ILocalArticleMasterServices>();

            PageRefrehCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            if (!string.IsNullOrWhiteSpace(ScannedCode))
            {
                await SearchItem();
            }

        }

        private async Task SearchItem()
        {
           
        }
    }
}
