﻿using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSIncomingHeaderServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader
{
    public class SMSIncomingHeaderServices : ISMSIncomingHeaderServices
    {
        string BaseUrl = Preferences.Get("PrefServerAddress", "http://192.168.1.217:80/");
        HttpClient client;

        public async Task<IEnumerable<IncomingHeaderModel>> GetList(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter)
        {
            switch (type)
            {
                case "All":
                    using (client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(40);
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/IncomingHeaders/");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingHeaderModel>>(json);
                        return datas;
                    }
                default: return null;
            }
        }
    }
}
