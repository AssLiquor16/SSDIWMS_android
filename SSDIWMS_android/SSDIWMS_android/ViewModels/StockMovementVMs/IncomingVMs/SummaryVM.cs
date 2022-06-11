using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages.SummaryPopupModulePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs
{
    public class SummaryVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();
        string _totalSelected;
        bool _summaryBtnVisible;
        public string TotalSelected { get => _totalSelected; set => SetProperty(ref _totalSelected, value); }
        public bool SummaryBtnVisible { get => _summaryBtnVisible; set => SetProperty(ref _summaryBtnVisible, value); }
        public ObservableRangeCollection<SelectedIncomingPartialHeaderModel> FinalizedList { get; set; }
        public AsyncCommand NavSubSummaryPopupCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public SummaryVM()
        {
            FinalizedList = new ObservableRangeCollection<SelectedIncomingPartialHeaderModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            NavSubSummaryPopupCommand = new AsyncCommand(NavSubSummaryPopup);
        }

        private async Task NavSubSummaryPopup() 
        {
            List<string> pos = new List<string>();
            foreach(var a in FinalizedList.Where(x=>x.IsSelected == true).ToList())
            {
                pos.Add(a.PONumber);
            }
            await PopupNavigation.Instance.PushAsync(new SummaryPopupSubPage(pos.ToArray()));
        }

        public async Task PageRefresh()
        {
            FinalizedList.Clear();
            await livetime.LiveTimer();
            var datas = await dependencies.localDbIncomingHeaderService.GetList("WhId/CurDate/FinalizedIncStat", null, null, null, new IncomingHeaderModel { WarehouseId = Preferences.Get("PrefUserWarehouseAssignedId", 0) } );
            foreach(var data in datas)
            {
                FinalizedList.Add
                    (
                    new SelectedIncomingPartialHeaderModel
                    {
                        INCId = data.INCId,
                        PONumber = data.PONumber,
                        TimesUpdated = data.TimesUpdated,
                        INCStatus = data.INCstatus,
                        Delcode = data.DelCode,
                        ShipCode = data.ShipCode,
                        BillDoc = data.BillDoc,
                        BatchCode = data.BatchCode,
                        IsSelected = false
                    }
                    );
            }
            TotalSelected = $"Total selected Item(s): {FinalizedList.Where(x => x.IsSelected).Count()}";
            SummaryBtnVisible = false;
        }
        public async Task CheckVerifier()
        {
            await Task.Delay(1);
            TotalSelected = $"Total selected Item(s): {FinalizedList.Where(x => x.IsSelected == true).Count()}";
            if (FinalizedList.Where(x => x.IsSelected == true).Count() > 0)
            {
                SummaryBtnVisible = true;
            }
            else
            {
                SummaryBtnVisible = false;
            }
        }
    }
}
