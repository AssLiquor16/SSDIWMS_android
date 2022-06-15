﻿using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SPalletHeaderServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet
{
    public class SPalletHeaderServices : ISPalletHeaderServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<PalletHeaderModel> GetFirstOrdefault(PalletHeaderModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PalletHeaderModel>> GetList(PalletHeaderModel obj = null, string type = null)
        {
            switch (type)
            {
                default:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/PalletHeaders");
                        var palletHeaders = JsonConvert.DeserializeObject<IEnumerable<PalletHeaderModel>>(json);
                        return palletHeaders;
                    }
            }
        }

        public Task<PalletHeaderModel> Insert(PalletHeaderModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public Task<PalletHeaderModel> Update(PalletHeaderModel obj, string type = null)
        {
            throw new NotImplementedException();
        }
    }
}
