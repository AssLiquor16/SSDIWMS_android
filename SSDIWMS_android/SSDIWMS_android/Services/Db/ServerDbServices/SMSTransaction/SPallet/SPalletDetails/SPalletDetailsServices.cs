using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.SPalletDetails;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SPalletDetailsServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.SPalletDetails
{
    public class SPalletDetailsServices : ISPalletDetailsServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<PalletDetailsModel> GetFirstOrDefault(PalletDetailsModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PalletDetailsModel>> GetList(PalletDetailsModel obj = null, string type = null)
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
                        var json = await client.GetStringAsync($"api/PalletDetails");
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<PalletDetailsModel>>(json);
                        return caseDefault;
                    }
                case "PalletCode":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/PalletDetails");
                        var caseA = JsonConvert.DeserializeObject<IEnumerable<PalletDetailsModel>>(json);
                        return caseA;
                    }
                case "PalletCode/ItemCode":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/PalletDetails/SearchPalletItemCode/{obj.PalletCode}/{obj.ItemCode}");
                        var caseB = JsonConvert.DeserializeObject<IEnumerable<PalletDetailsModel>>(json);
                        return caseB;
                    }
            }
        }

        public async Task<PalletDetailsModel> Insert(PalletDetailsModel obj, string type = null)
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
                        var r = await client.PostAsync("api/Palletdetails/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var caseDefault = JsonConvert.DeserializeObject<PalletDetailsModel>(ret);
                        return caseDefault;
                    }

            }
        }

        public async Task<PalletDetailsModel> Update(PalletDetailsModel obj, string type = null)
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
                        var res = await client.PutAsync($"api/PalletDetails/{obj.PCreationId}", content);
                        var sult = res.StatusCode.ToString();
                        return null;
                    }
            }
        }
    }
}
