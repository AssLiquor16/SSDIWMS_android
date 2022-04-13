using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingSubVMs
{
    public class IncomingDetailAddVM : ViewModelBase
    {
        readonly ILocalArticleMasterServices localDbItemMasterServie;
        readonly ISMLIncomingDetailServices localDBIncomingDetailService;

        string _barcodeVal,_itemCode,_itemDesc,_amount;
        public string BarcodeVal { get => _barcodeVal; set => SetProperty(ref _barcodeVal, value); }
        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public string Amount { get => _amount; set => SetProperty(ref _amount, value); }
        public ObservableRangeCollection<ItemMasterModel> ItemList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public IncomingDetailAddVM()
        {
            localDbItemMasterServie = DependencyService.Get<ILocalArticleMasterServices>();
            localDBIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();

            ItemList = new ObservableRangeCollection<ItemMasterModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            if (!string.IsNullOrWhiteSpace(BarcodeVal))
            {
                await SearchItem(BarcodeVal);
            }
            await LiveTimer();
            UserFullName = Preferences.Get("PrefUserFullname", "");
            PONumber = Preferences.Get("PrefPONumber", "");
        }
        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname, _pONumber;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
        public string PONumber { get => _pONumber; set => SetProperty(ref _pONumber, value); }
        private async Task LiveTimer()
        {
            await Task.Delay(1);
            Device.StartTimer(TimeSpan.FromSeconds(_datetimeTick), () => {
                Task.Run(async () =>
                {
                    await Task.Delay(1);
                    LiveDate = DateTime.Now.ToString(_datetimeFormat);
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }



        private async Task SearchItem(string barcode)
        {
            ItemList.Clear();
            string[] stringarray = { barcode };

            // query to itemmaster
            var initialfilterlist = await localDbItemMasterServie.GetList("CaseCode", stringarray, null);
            ItemList.AddRange(initialfilterlist);
            if(ItemList.Count > 0)
            {
                // query to incomingdetails
                foreach (var item in ItemList)
                {
                    string[] filterarray = { PONumber, item.ItemCode };
                    var retval = await localDBIncomingDetailService.GetModel("PO,ItemCode", filterarray, null);
                    if (retval != null)
                    {
                        ItemCode = retval.ItemCode;
                        ItemDesc = retval.ItemDesc;
                        Amount = "\u20b1" + " " + retval.Amount;
                    }
                }
            }
            else
            {
                // item not found
            }
        }
    }
}
