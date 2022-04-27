using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailPopupModuleVMs.IncomingDetailSubPopupModuleVMs
{
    public class PartialDetailListPopupVM : ViewModelBase
    {
        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        IncomingDetailModel _passedItem;
        PListModel _selectedPallet;
        public IncomingDetailModel PassedItem { get => _passedItem; set => SetProperty(ref _passedItem, value); }
        public PListModel SelectedPallet
        {
            get => _selectedPallet;
            set
            {

                if (value == _selectedPallet)
                    return;
                _selectedPallet = value;
                OnPropertyChanged();
                SortItem();
            }
        }
        public ObservableRangeCollection<PListModel> PList { get; set; }
        public ObservableRangeCollection<IncomingPartialDetailModel> PartialDetailList { get; set; }
        public ObservableRangeCollection<IncomingPartialDetailModel> FilterPartialList { get; set; }
        public AsyncCommand CloseCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public PartialDetailListPopupVM()
        {
            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            PList = new ObservableRangeCollection<PListModel>();
            PartialDetailList = new ObservableRangeCollection<IncomingPartialDetailModel>();
            FilterPartialList = new ObservableRangeCollection<IncomingPartialDetailModel>();
            CloseCommand = new AsyncCommand(Close);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Close()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async Task PageRefresh()
        {
            if(PassedItem != null)
            {
                await GetList();
            }
        }
        private async Task GetList()
        {
            PList.Clear();
            PartialDetailList.Clear();
            int[] e = { PassedItem.INCDetId };
            string[] f = { PassedItem.ItemCode };
            var partialdetret = await localDbIncomingParDetailService.GetList("PONumber&ItemCode&INCDetId", f, e);
            //var partialdetret = await localDbIncomingParDetailService.GetList("PONumber&INCId", null,e);
            PartialDetailList.AddRange(partialdetret);
            FilterPartialList.AddRange(PartialDetailList);

            var id = 1;
            var all = new PListModel
            {
                Id = id,
                PalletCode = "All"
            };
            PList.Add(all);
            foreach(var item in PartialDetailList)
            {
                var pcode = PList.Where(x => x.PalletCode == item.PalletCode).Count();
                if (pcode == 0)
                {
                    id++;
                    var p = new PListModel
                    {
                        Id = id,
                        PalletCode = item.PalletCode
                    };
                    PList.Add(p);
                }

            }
        }
        private void SortItem()
        {
            if (SelectedPallet.PalletCode == "All")
            {
                FilterPartialList.ReplaceRange(PartialDetailList);
            }
            else
            {
                var sort = PartialDetailList.Where(x => x.PalletCode == SelectedPallet.PalletCode);
                FilterPartialList.ReplaceRange(sort);
            }
        }
    }
    public partial class PListModel
    {
        public int Id { get; set; }
       public string PalletCode { get; set; }
    }
}
