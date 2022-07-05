using SSDIWMS_android.Models.DefaultModel;
using SSDIWMS_android.Services;
using SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.Date;
using SSDIWMS_android.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android
{
    public partial class App : Application
    {
        Setup setup = new Setup();
        ILIPServices ipService;
        IDateVerifierServices dateService;
        IMainServices mainService;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            ipService = DependencyService.Get<ILIPServices>();
            dateService = DependencyService.Get<IDateVerifierServices>();
            mainService = DependencyService.Get<IMainServices>();
        }

        protected override async void OnStart()
        {
            await Task.Delay(100);
            await setup.CreateIP();
            setup.SetIp();
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
