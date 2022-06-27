using SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string PalletSaveMethod = "Online";
        public static string baseIp;
        public static string transactionSyncRef = "TimesUpdated"; // TimesUpdated or DateSync
        public string getIp()
        {
            return baseIp;
        }
        public async void SetIp()
        {
            var iplist = await ipServices.GetList();
            baseIp = iplist.Where(x => x.Is_Used == true).FirstOrDefault().Ip_Address;
        }
    }
}
