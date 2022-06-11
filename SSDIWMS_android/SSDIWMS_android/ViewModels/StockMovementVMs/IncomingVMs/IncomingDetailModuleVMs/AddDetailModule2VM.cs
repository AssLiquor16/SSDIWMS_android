using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.ViewModels.PopUpVMs;
using SSDIWMS_android.Views.PopUpPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.IncomingDetailModuleVMs
{
    public class AddDetailModule2VM : ViewModelBase
    {
        public GlobalDependencyServices Dependencies { get; } = new GlobalDependencyServices();
        public LiveTime liveTime { get; } = new LiveTime();

        IncomingDetailModel _selectedItem;
        string _searchCode;
        int _partialCqty;
        bool _formView, _colView, _searchEnableTrue, _searchEnableFalse, _searchEnableBool;

        public IncomingDetailModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public string SearchCode
        {
            get => _searchCode;
            set
            {
                if (value == _searchCode)
                    return;
                _searchCode = value;
                OnPropertyChanged();
                PartialCQTY = 0;
                Search(value);
            }
        }
        public int PartialCQTY { get => _partialCqty; set => SetProperty(ref _partialCqty, value); }
        public bool FormView { get => _formView; set => SetProperty(ref _formView, value); }
        public bool ColView { get => _colView; set => SetProperty(ref _colView, value); }
        public bool SearchEnableTrue { get => _searchEnableTrue; set => SetProperty(ref _searchEnableTrue, value); }
        public bool SearchEnableFalse { get => _searchEnableFalse; set => SetProperty(ref _searchEnableFalse, value); }
        public bool SearchEnableBool { get => _searchEnableBool; set => SetProperty(ref _searchEnableBool, value); }


        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> SearcheddetailList { get; set; }

        public AsyncCommand TappedCommand { get; }
        public AsyncCommand CancelCommand { get; }
        public AsyncCommand AddItemCommand { get; }
        public AsyncCommand SearchEnableCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public AddDetailModule2VM()
        {
            ItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();
            SearcheddetailList = new ObservableRangeCollection<IncomingDetailModel>();
            TappedCommand = new AsyncCommand(Tapped);
            CancelCommand = new AsyncCommand(Cancel);
            AddItemCommand = new AsyncCommand(AddItem);
            SearchEnableCommand = new AsyncCommand(SearchEnableKeyPressed);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
            
        }
        private async Task Tapped()
        {
            await Task.Delay(1);
           DecideView("Single");
        }
        private async Task Cancel() => await Shell.Current.GoToAsync("..");
        private async Task AddItem()
        {
            if(PartialCQTY != 0)
            {
                if(Preferences.Get("PrefUserId", 0) != 0)
                {
                    if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to add the item?", "Yes", "No") == true)
                    {
                        var data = new IncomingPartialDetailModel
                        {
                            INCDetId = SelectedItem.INCDetId,
                            INCHeaderId = SelectedItem.INCHeaderId,
                            ItemCode = SelectedItem.ItemCode,
                            ItemDesc = SelectedItem.ItemDesc,
                            PartialCQTY = PartialCQTY,
                            PalletCode = string.Empty,
                            ExpiryDate = DateTime.MinValue,
                            TimesUpdated = 0,
                            POHeaderNumber = SelectedItem.POHeaderNumber,
                            Status = "Ongoing",
                            WarehouseLocation = string.Empty,
                            DateSync = DateTime.Now
                        };
                        await Dependencies.localDbIncomingParDetailService.Insert("RefIdAutoGenerate", data);
                        await Dependencies.notifService.StaticToastNotif("Success", "Item added.");
                        MessagingCenter.Send(this, "FromDetailsAddMSG", "AddRefresh");
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else
                {
                    await Dependencies.mainService.RemovePreferences();
                    await Dependencies.notifService.StaticToastNotif("Error", "User not found.");
                    await Task.Delay(3000);
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                }
            } 
            else
            {
                await Dependencies.notifService.StaticToastNotif("Error", "Missing entry");
            }
        }
        private async Task SearchEnableKeyPressed()
        {
            await Task.Delay(1);
            SearchCode = string.Empty;
            if(SearchEnableBool == false)
            {
                // incase search overide needs permission to admin
                /* await PopupNavigation.Instance.PushAsync(new FormPopupPage("AdminSearchEnable")); 
                
                // put this in constructor of this class
                MessagingCenter.Subscribe<FormPopupVM, string>(this, "SearchEnable", async (page, cmd) =>
                {
                    SearchEnableBool = true;
                    await SearchEnable();
                }); */
                SearchEnableBool = true;
                await SearchEnable();
            }
            else
            {
                SearchEnableBool = false;
                await SearchEnable();
            }
        }
        private async Task SearchEnable()
        {
            await Task.Delay(1);
            switch (SearchEnableBool)
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
            if (Preferences.Get("PrefAddPartialDetail2InitialRefresh", false) == false)
            {
                await liveTime.LiveTimer();
                ItemMasterList.Clear();
                ItemMasterList.AddRange(await Dependencies.localDbArticleMasterService.GetList("All", null, null));
                IncomingDetailList.Clear();
                IncomingDetailList.AddRange(await Dependencies.localDbIncomingDetailService.GetList("PONumber2", new string[] { Preferences.Get("PrefPONumber", string.Empty) }, null));
                SearchEnableBool = false;
                await SearchEnable();
                FormView = false;
                ColView = true;
                Preferences.Set("PrefAddPartialDetail2InitialRefresh", true);
            }
        }
        private void Search(string searchCode)
        {
            searchCode = searchCode.ToUpperInvariant();
            if(searchCode != string.Empty)
            {
                var iMasterRes = ItemMasterList.Where(x => x.CaseCode == searchCode || x.ItemCode == searchCode).ToList();
                if (iMasterRes.Count > 0)
                {
                    foreach (var iMasterRe in iMasterRes)
                    {
                        foreach (var iMaster in iMasterRes)
                        {
                            var res = IncomingDetailList.Where(x => x.ItemCode == iMaster.ItemCode).ToList();
                            if (res.Count() == 0)
                            {
                                SearcheddetailList.Clear();
                            }
                            else
                            {
                                SearcheddetailList.ReplaceRange(res);
                            }
                            
                        }

                    }
                    if (SearcheddetailList.Count == 1)
                    {
                        // formview
                        DecideView("Single");
                        SelectedItem = SearcheddetailList[0];
                    }
                    else if (SearcheddetailList.Count > 1)
                    {
                        // colview
                        DecideView("Multiple");
                    }
                    else
                    {
                        // item not found in po
                        DecideView("NotFound");
                    }
                }
                else
                {
                    // item not found in item master
                    DecideView("NotFound");
                }
            }
            else
            {
                SearcheddetailList.Clear();
                DecideView("NotFound");
            }
        }
        private void DecideView(string resultStatus = null)
        {
            switch (resultStatus)
            {
                case "Single":
                    ColView = false;
                    FormView = true;
                    break;
                case "Multiple":
                    FormView = false;
                    ColView = true;
                    break;
                case "NotFound":
                    FormView = false;
                    ColView = true;
                    break;
                default:
                    FormView = false;
                    ColView = false;
                    break;

            }
        }

    }
}
