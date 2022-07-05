using SSDIWMS_android.Models.DefaultModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android
{
    public class Setup
    {
        ILIPServices ipServices;
        public Setup()
        {
            ipServices = DependencyService.Get<ILIPServices>();
        }
        public static string baseLocalAddress = "WMSLocalDb.db";
        public static string PalletSaveMethod = "Online";
        public static string baseIp;
        public static string transactionSyncRef = "TimesUpdated"; // TimesUpdated or DateSync

        public string getLocal()
        {
            return baseLocalAddress;
        }
        public string getIp()
        {
            return baseIp;
        }
        public async void SetIp()
        {
            var iplist = await ipServices.GetList();
            baseIp = iplist.Where(x => x.Is_Used == true).FirstOrDefault().Ip_Address;
        }
        public async Task CreateIP()
        {
            var defaultIp = new IPAddressModel
            {
                Ip_Id = 1,
                Ip_Address = "http://192.168.1.217:80/",
                Is_Used = true
            };
            if (await ipServices.GetFirstorDefault(defaultIp) == null)
            {
                await ipServices.Insert(defaultIp);
            }
        }
    }
}
