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
        bool _formView,_colView;
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
                Search(value);
            }
        }
        public bool FormView { get => _formView; set => SetProperty(ref _formView, value); }
        public bool ColView { get => _colView; set => SetProperty(ref _colView, value); }


        public ObservableRangeCollection<ItemMasterModel> ItemMasterList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> IncomingDetailList { get; set; }
        public ObservableRangeCollection<IncomingDetailModel> SearcheddetailList { get; set; }


        public AsyncCommand PageRefreshCommand { get; }
        public AddDetailModule2VM()
        {
            ItemMasterList = new ObservableRangeCollection<ItemMasterModel>();
            IncomingDetailList = new ObservableRangeCollection<IncomingDetailModel>();
            SearcheddetailList = new ObservableRangeCollection<IncomingDetailModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
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

                FormView = false;
                ColView = false;
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
                DecideView();
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
                    ColView = false;
                    break;
                default:
                    FormView = false;
                    ColView = false;
                    break;

            }
        }

    }
}
