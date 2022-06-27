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
        ILIPServices ipService;
        IDateVerifierServices dateService;
        IMainServices mainService;
        Setup setup { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            ipService = DependencyService.Get<ILIPServices>();
            dateService = DependencyService.Get<IDateVerifierServices>();
            mainService = DependencyService.Get<IMainServices>();
            setup = new Setup();
        }

        protected override async void OnStart()
        {
            await CreateIP();
            await mainService.OnstartSetDefaulPreferences();
            await dateService.DatetimeValidate();
            await mainService.TimerCheckUser();
            setup.SetIp();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private async Task CreateIP()
        {
            List<IPAddressModel> IpList = new List<IPAddressModel>();
            var defaultIp = new IPAddressModel
            {
                Ip_Id = 1,
                Ip_Address = "http://192.168.1.217:80/",
                Is_Used = true
            };
            IpList.Add(defaultIp);
            if (await ipService.GetFirstorDefault(defaultIp) == null)
            {
                foreach (var ip in IpList)
                {
                    await ipService.Insert(ip);
                }
            }
        }

    }
}
