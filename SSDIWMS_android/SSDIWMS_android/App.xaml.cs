using SSDIWMS_android.Services;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date;
using SSDIWMS_android.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android
{
    public partial class App : Application
    {
        IDateVerifierServices dateService;
        IMainServices mainService;

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            dateService = DependencyService.Get<IDateVerifierServices>();
            mainService = DependencyService.Get<IMainServices>();
        }

        protected override async void OnStart()
        {
            await mainService.OnstartSetDefaulPreferences();
            await dateService.DatetimeValidate();
            await mainService.TimerCheckUser();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
