using SSDIWMS_android.Services;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android
{
    public partial class App : Application
    {
        IMainServices mainService;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            mainService = DependencyService.Get<IMainServices>();
        }

        protected override async void OnStart()
        {
            await mainService.OnstartSetDefaulPreferences();
            await mainService.SyncIncomingTransaction();
            await mainService.CheckUser();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
