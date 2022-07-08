using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.PurchaseOrderVMs2;
using SSDIWMS_android.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels
{
    public class DashBoardVM : ViewModelBase
    {
        public LiveTime livetime { get; } = new LiveTime();
        public AsyncCommand PageRefreshCommand { get; }
        public DashBoardVM()
        {
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task PageRefresh()
        {
            bool login = Preferences.Get("PrefLoggedIn", false);
            if (login == false)
            {
                var route = $"//{nameof(LoginPage)}";
                await Shell.Current.GoToAsync(route);
            }
            else
            {
                var userId = Preferences.Get("PrefUserId", 0);
                var role = Preferences.Get("PrefUserRole", string.Empty);
                var whid = Preferences.Get("PrefUserWarehouseAssignedId", 0);
                POStaticDatas.setUserId(userId);
                POStaticDatas.SetUserRole(role);
                POStaticDatas.SetWarehouseId(whid);
            }
            await livetime.LiveTimer();
        }
        public IncomingDetailModel model1 = new IncomingDetailModel();
        public IncomingPartialDetailModel model = new IncomingPartialDetailModel();
        object a = new object();

        string _getVal1, _getVal2,_passval1,_passval2;
        public string GetVal1 { get => _getVal1; set => SetProperty(ref _getVal1, value); }
        public string GetVal2 { get => _getVal2; set => SetProperty(ref _getVal2, value); }
        public string PassVal1 { get => _passval1; set => SetProperty(ref _passval1, value); }
        public string Passval2 { get => _passval2; set => SetProperty(ref _passval2, value); }

    }
}
