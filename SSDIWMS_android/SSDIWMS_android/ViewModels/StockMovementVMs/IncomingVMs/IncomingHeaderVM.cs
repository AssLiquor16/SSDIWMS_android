using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs
{
    public class IncomingHeaderVM : ViewModelBase
    {
        IncomingHeaderModel _selectedHeader;
        string _isRefreshing;


        public IncomingHeaderModel SelectedHeader { get => _selectedHeader; set => SetProperty(ref _selectedHeader, value); }
        public string IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public ObservableRangeCollection<IncomingHeaderModel> IncomingHeaderList { get; set; }

        public AsyncCommand PageRefreshCommand { get; }

        public IncomingHeaderVM()
        {
            IncomingHeaderList = new ObservableRangeCollection<IncomingHeaderModel>();

            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task PageRefresh()
        {

        }
    }
}
