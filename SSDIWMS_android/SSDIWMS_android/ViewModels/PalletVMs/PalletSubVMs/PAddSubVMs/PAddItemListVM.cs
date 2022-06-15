using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.ViewModels.PalletVMs.PalletSubVMs.PAddSubVMs
{
    public class PAddItemListVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();
        public AsyncCommand PageRefreshCommand { get; }
        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; }
        public PAddItemListVM()
        {
            ItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task PageRefresh()
        {
            await livetime.LiveTimer();
            ItemMasterList.Clear();
            ItemMasterList.AddRange(await dependencies.localDbArticleMasterService.GetList("All", null, null));
        }
    }
}
