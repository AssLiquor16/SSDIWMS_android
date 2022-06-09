﻿using MvvmHelpers.Commands;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
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

        public AsyncCommand PageRefreshCommand { get; }
        public AsyncCommand TestCommand { get; }
        public DashBoardVM()
        {
            TestCommand = new AsyncCommand(Test);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task Test()
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Some SKU have variance.", "Proceed", "Cancel");
        }
        private async Task PageRefresh()
        {
            bool login = Preferences.Get("PrefLoggedIn", false);
            if (login == false)
            {
                var route = $"//{nameof(LoginPage)}";
                await Shell.Current.GoToAsync(route);
            }
            await LiveTimer();
            var userfullname = Preferences.Get("PrefUserFullname", "");
            var name = userfullname.Split(' ');
            UserFullName = name[0];


        }

        static int _datetimeTick = Preferences.Get("PrefDateTimeTick", 20);
        static string _datetimeFormat = Preferences.Get("PrefDateTimeFormat", "ddd, dd MMM yyy hh:mm tt"), _userFullname;
        string _liveDate = DateTime.Now.ToString(_datetimeFormat);
        public string LiveDate { get => _liveDate; set => SetProperty(ref _liveDate, value); }
        public string UserFullName { get => _userFullname; set => SetProperty(ref _userFullname, value); }
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
