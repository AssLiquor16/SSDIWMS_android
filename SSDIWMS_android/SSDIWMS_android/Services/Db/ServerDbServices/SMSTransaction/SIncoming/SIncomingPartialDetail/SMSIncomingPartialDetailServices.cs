using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingPartialDetail;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSIncomingPartialDetailServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingPartialDetail
{
    public class SMSIncomingPartialDetailServices : ISMSIncomingPartialDetailServices
    {
        string BaseUrl = Preferences.Get("PrefServerAddress", "http://192.168.1.217:80/");
        HttpClient client;

        public async Task<IEnumerable<IncomingPartialDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter)
        {
            switch (type)
            {
                case "All":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/IncomingPartialDetails/");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingPartialDetailModel>>(json);
                        return datas;
                    }
                case "GetItemCodePO":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/IncomingPartialDetails/GetItemCodePO/{stringfilter[0]}/{stringfilter[1]}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingPartialDetailModel>>(json);
                        return datas;
                    }
                default: return null;
            }
        }

        public Task<IncomingPartialDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter)
        {
            throw new NotImplementedException();
        }

        public Task Insert(string type, IncomingPartialDetailModel item)
        {
            throw new NotImplementedException();
        }

        public async Task<IncomingPartialDetailModel> SpecialCaseInsert(string type, IncomingPartialDetailModel item)
        {
            switch (type)
            {
                case "ReturnInsertedItem":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(item);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var r = await client.PostAsync("api/IncomingPartialDetails/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var n = JsonConvert.DeserializeObject<IncomingPartialDetailModel>(ret);
                        return n;
                    }
                default: return null;
            }
        }

        public async Task Update(string type, IncomingPartialDetailModel item)
        {
            switch (type)
            {
                case "Common":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(item);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/IncomingPartialDetails/{item.INCServerId}", content);
                        var sult = res.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(sult);
                    }
                    break;
                default: break;
            }
        }
    }
}
