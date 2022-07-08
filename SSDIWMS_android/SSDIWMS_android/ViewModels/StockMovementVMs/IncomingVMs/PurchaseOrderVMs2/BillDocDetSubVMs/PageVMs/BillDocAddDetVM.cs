using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.PurchaseOrderVMs2.BillDocDetSubVMs.PageVMs
{
    public class BillDocAddDetVM : ViewModelBase
    {

        string _searchcode, _itemCode, _itemDesc;
        int _qty;
        bool _listView, _formView, _searchEnableSwitch, _searchEnableTrue, _searchEnableFalse;
        IncomingPartialDetailModel _data;
        ItemMasterModel _selectedItem;
        public LiveTime livetime { get; } = new LiveTime();
        GlobalDependencyServices call = new GlobalDependencyServices();


        public IncomingPartialDetailModel Data { get => _data; set => SetProperty(ref _data, value); }
        public ItemMasterModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public bool FormView { get => _formView; set => SetProperty(ref _formView, value); }
        public bool ListView { get => _listView; set => SetProperty(ref _listView, value); }
        public bool SearchEnableSwitch { get => _searchEnableSwitch; set => SetProperty(ref _searchEnableSwitch, value); }
        public bool SearchEnableTrue { get => _searchEnableTrue; set => SetProperty(ref _searchEnableTrue, value); }
        public bool SearchEnableFalse { get => _searchEnableFalse; set => SetProperty(ref _searchEnableFalse, value); }

        public string ItemCode { get => _itemCode; set => SetProperty(ref _itemCode, value); }
        public string ItemDesc { get => _itemDesc; set => SetProperty(ref _itemDesc, value); }
        public int Qty { get => _qty; set => SetProperty(ref _qty, value); }
        public string SearchCode
        {
            get => _searchcode;
            set
            {

                if (value == _searchcode)
                    return;
                _searchcode = value;
                OnPropertyChanged();
                Search(value);

            }
        }


        static List<IncomingDetailModel> MainStaticList;
        public ObservableRangeCollection<ItemMasterModel> MainItemList { get; set; }
        public ObservableRangeCollection<ItemMasterModel> ItemList { get; set; }

        public AsyncCommand AddCommand { get; }
        public AsyncCommand SearchEnableCommand { get; }
        public AsyncCommand TappedCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public BillDocAddDetVM()
        {
            Data = new IncomingPartialDetailModel();

            MainItemList = new ObservableRangeCollection<ItemMasterModel>();
            ItemList = new ObservableRangeCollection<ItemMasterModel>();

            AddCommand = new AsyncCommand(Add);
            SearchEnableCommand = new AsyncCommand(SearchEnable);
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Add()
        {
            await call.notifService.LoadingProcess("Begin", "Processing...");
            Data.RefId = RandomStringGenerator.RandomString("IPD");
            Data.DateCreated = DateTime.Now;
            Data.DateSync = DateTime.Now;
            Data.PartialCQTY = Qty;
            if (Data.PartialCQTY > 0)
            {
                try
                {
                    await call.serverDbIncomingPartialDetailService.NewInsert(Data);
                    await call.notifService.StaticToastNotif("Success", "Item inserted to server database.");
                }
                catch
                {
                    await call.localDbIncomingParDetailService.NewInsert(Data);
                    await call.notifService.StaticToastNotif("Error", "Cannot connect to server. data inserted to the local database.");
                }
            }
            else
            {
                await call.notifService.StaticToastNotif("Error", "Quantity must be greater than 0.");
            }

            FormView = false;
            ListView = true;
            SearchCode = ".";

            SearchEnableSwitch = false;
            SearchEnableFalse = true;
            SearchEnableTrue = false;

            await Task.Delay(100);
            SearchCode = string.Empty;
            Qty = 0;

            await call.notifService.LoadingProcess("End");
        }
        private async Task Tapped()
        {
            await Task.Delay(100);
            if (SelectedItem != null)
            {
                Data.ItemCode = SelectedItem.ItemCode;
                Data.ItemDesc = SelectedItem.ItemDesc;
                ItemCode = SelectedItem.ItemCode;
                ItemDesc = SelectedItem.ItemDesc;
            }
        }
        private async Task SearchEnable()
        {
            await Task.Delay(100);
            switch (SearchEnableSwitch)
            {
                case true:
                    SearchEnableSwitch = false;
                    break;
                case false:
                    SearchEnableSwitch = true;
                    break;
            }
            switch (SearchEnableSwitch)
            {
                case true:
                    SearchEnableTrue = true;
                    SearchEnableFalse = false;
                    break;
                case false:
                    SearchEnableFalse = true;
                    SearchEnableTrue = false;
                    break;
            }
        }
        private async Task PageRefresh()
        {
            await call.notifService.LoadingProcess("Begin", "Loading...");
            await livetime.LiveTimer();
            MainItemList.Clear(); ItemList.Clear();
            Data.UserId = POStaticDatas.UserId;
            Data.BillDoc = POStaticDatas.BillDoc;
            Data.ExpiryDate = DateTime.MinValue;


            if (MainStaticList.Count > 0)
            {
                foreach (var item in MainStaticList)
                {
                    if (item.BillDoc != POStaticDatas.BillDoc)
                    {
                        await QueryData();
                        break;
                    }
                }
            }
            else
            {
                await QueryData();
            }

            foreach(var item in MainStaticList)
            {
                var result = await call.localDbArticleMasterService.GetFirstOrDefault(new ItemMasterModel { ItemCode = item.ItemCode }, "ItemCode");
                if(result != null)
                {
                    MainItemList.Add(result);
                }
            }

            ListView = true;
            SearchEnableFalse = true;
            await call.notifService.LoadingProcess("End");

        }
        private async Task QueryData()
        {
            try
            {
                MainStaticList.Clear();
                var sitems = await call.serverDbIncomingdetailService.NewGetList(new IncomingDetailModel { BillDoc = POStaticDatas.BillDoc }, "BillDoc");
                var groupeddetails = await GroupIncoming(sitems.ToList());
                MainStaticList.AddRange(sitems);
            }
            catch
            {
                await call.notifService.StaticToastNotif("Error", "Cannot load items , unable to connect to server.");
            }
        }
        private async Task<IEnumerable<IncomingDetailModel>> GroupIncoming(List<IncomingDetailModel> sItems)
        {
            await Task.Delay(1);
            var groupeditems = sItems.GroupBy(l => l.ItemCode).Select(cl => new IncomingDetailModel
            {
                ItemDesc = cl.Where(x => x.ItemCode == cl.Key).FirstOrDefault().ItemDesc,
                BillDoc = POStaticDatas.BillDoc
            }).ToList();
            return groupeditems;
        }
        private void Search(string val)
        {
            val = val.ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(val))
            {
                FormView = false;
                ListView = true;
                ItemList.Clear();
            }
            else
            {
                FormView = false;
                ListView = true;
                var results = MainItemList.Where(x => x.ItemCode == val || x.CaseCode == val).ToList();
                if(results.Count == 1)
                {
                    FormView = true;
                    foreach(var item in results)
                    {
                        Data.ItemCode = item.ItemCode;
                        Data.ItemDesc = item.ItemDesc;
                        ItemCode = item.ItemCode;
                        ItemDesc = item.ItemDesc;
                    }
                }
                else
                {
                    FormView = false;
                    ListView = true;
                    ItemList.ReplaceRange(results);
                }
            }
        }
        public static void StaticListInitialize()
        {
            MainStaticList = new List<IncomingDetailModel>();
        }
        
    }
}
