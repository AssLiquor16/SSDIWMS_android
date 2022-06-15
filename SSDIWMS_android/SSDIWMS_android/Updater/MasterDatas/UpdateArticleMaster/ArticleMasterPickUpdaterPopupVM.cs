using MvvmHelpers.Commands;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateArticleMaster
{
    public class ArticleMasterPickUpdaterPopupVM : ViewModelBase
    {
        public AsyncCommand PageRefreshCommand { get; }
        public ArticleMasterPickUpdaterPopupVM()
        {
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {

        }
    }
}
