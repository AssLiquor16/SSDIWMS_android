using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingPopUpSubVMs
{
    public class IncomingDetailAddPopupVM : ViewModelBase
    {
        ILocalArticleMasterServices localDbItemMasterServie;
        ISMLIncomingDetailServices localDBIncomingDetailService;
        IToastNotifService notifyService;

        IncomingDetailModel e;
        string _scannedCode, _poNumber,_itemCode, _itemDesc, _amount,_palletCode,_warningtext;
        int _cqty, _itemId;
        bool _nullQuery, _notnullQuery,_loadingView;
        public IncomingDetailModel E { get => e; set => SetProperty(ref e, value); }
        public string ScannedCode { get => _scannedCode; set => SetProperty(ref _scannedCode, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public string Amount { get => _amount; set => SetProperty(ref _amount, value); }
        public string PalletCode { get => _palletCode; set => SetProperty(ref _palletCode, value); }
        public string WarningText { get => _warningtext; set => SetProperty(ref _warningtext, value); }
        public int ItemId { get => _itemId; set => SetProperty(ref _itemId, value); }
        public int CQTY { get => _cqty; set => SetProperty(ref _cqty, value); }
        public bool NullQuery { get => _nullQuery; set => SetProperty(ref _nullQuery, value); }
        public bool NotNullQuery { get => _notnullQuery; set => SetProperty(ref _notnullQuery, value); }
        public bool LoadingView { get => _loadingView; set => SetProperty(ref _loadingView, value); }

        public ObservableRangeCollection<ItemMasterModel> ItemList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> ExistingItemList { get; set; }

        public AsyncCommand AddIncomingDetailCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public IncomingDetailAddPopupVM()
        {
            localDbItemMasterServie = DependencyService.Get<ILocalArticleMasterServices>();
            localDBIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            notifyService = DependencyService.Get<IToastNotifService>();

            ItemList = new ObservableRangeCollection<ItemMasterModel>();
            ExistingItemList = new ObservableRangeCollection<IncomingDetailModel>();

            AddIncomingDetailCommand = new AsyncCommand(AddIncomingDetail);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task AddIncomingDetail()
        {
            
            if (ItemId !=0 || CQTY !=0)
            {
                var main = new IncomingDetailListVM();
                E.Cqty = CQTY;
                E.PalletCode = PalletCode;
                E.TimesUpdated += 1;
                try
                {
                    await localDBIncomingDetailService.Update("Common", E);
                    await PopupNavigation.Instance.PopAllAsync(true);
                }
                catch
                {
                    await notifyService.StaticToastNotif("Error", "Something went wrong.");
                }
                main.Trigger = "Go";
            }
        }

        public async Task PageRefresh()
        {
            if (!string.IsNullOrWhiteSpace(ScannedCode))
            {
                LoadingView = true;
                WarningText = "Item not found";
                await SearchItem(ScannedCode);
            }
            
            
        }

        private async Task SearchItem(string barcode)
        {
            ItemList.Clear();
            ExistingItemList.Clear();
            string[] stringarray = { barcode };

            // query to itemmaster
            var initialfilterlist = await localDbItemMasterServie.GetList("CaseCode", stringarray, null);
            ItemList.AddRange(initialfilterlist);
            if (ItemList.Count > 0)
            {
                // query to incomingdetails
                foreach (var item in ItemList)
                {
                    string[] filterarray = { PONumber, item.ItemCode };
                    var retval = await localDBIncomingDetailService.GetModel("PO,ItemCode", filterarray, null);
                    if (retval != null)
                    {
                        ExistingItemList.Add(retval);

                        /*ItemCode = retval.ItemCode;
                        ItemDesc = retval.ItemDesc;
                        Amount = "\u20b1" + " " + retval.Amount;
                        NullQuery = false;
                        NotNullQuery = true;*/
                    }
                }
                if(ExistingItemList.Count == 1)
                {
                    if(ExistingItemList[0].TimesUpdated == 0 && ExistingItemList[0].Cqty ==0)
                    {
                        E = ExistingItemList[0];
                        ItemCode = ExistingItemList[0].ItemCode;
                        ItemDesc = ExistingItemList[0].ItemDesc;
                        Amount = "\u20b1" + " " + ExistingItemList[0].Amount;
                        ItemId = ExistingItemList[0].INCDetId;
                        LoadingView = false;
                        NullQuery = false;
                        NotNullQuery = true;
                    }
                    else
                    {
                        await PopupNavigation.Instance.PopAllAsync(true);
                        await notifyService.StaticToastNotif("Error", "Scanned item already added");
                    }
                }
                else if(ExistingItemList.Count == 0)
                {
                    LoadingView = false;
                    NotNullQuery = false;
                    NullQuery = true;
                }
                else
                {
                    WarningText = "Multiple item detected, ask assistance.";
                    LoadingView = false;
                    NotNullQuery = false;
                    NullQuery = true;
                }

            }
            else
            {
                // item not found
                LoadingView = false;
                NotNullQuery = false;
                NullQuery = true;
            }
        }
    }
}
