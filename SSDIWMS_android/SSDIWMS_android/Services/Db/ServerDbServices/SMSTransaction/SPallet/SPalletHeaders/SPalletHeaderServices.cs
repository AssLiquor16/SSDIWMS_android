using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<PalletHeaderModel>>(json);
                        return caseDefault;
                    }
                case "PHeaderRefID":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/palletheaders/GetPHeaderRefID/{obj.PHeaderRefID}");
                        var caseA = JsonConvert.DeserializeObject<IEnumerable<PalletHeaderModel>>(json);
                        return caseA;
                    }
                case "PalletCode":
                    var wh = Preferences.Get("PrefWarehouseName", string.Empty);
                    using (client = new HttpClient())
                    {

                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/palletheaders/GetPalletCode/{obj.PalletCode}/{wh}");
                        var caseB = JsonConvert.DeserializeObject<IEnumerable<PalletHeaderModel>>(json);
                        return caseB;
                    }
            }
        }

        public async Task<PalletHeaderModel> Insert(PalletHeaderModel obj, string type = null)
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
                        var json = JsonConvert.SerializeObject(obj);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var r = await client.PostAsync("api/PalletHeaders/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var caseDefault = JsonConvert.DeserializeObject<PalletHeaderModel>(ret);
                        return caseDefault;
                    }
                    
            }
        }

        public async Task<PalletHeaderModel> Update(PalletHeaderModel obj, string type = null)
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
                        var json = JsonConvert.SerializeObject(obj);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/PalletHeaders/{obj.PHeadId}", content);
                        var sult = res.StatusCode.ToString();
                        return null;
                    }
            }
        }
    }
}
