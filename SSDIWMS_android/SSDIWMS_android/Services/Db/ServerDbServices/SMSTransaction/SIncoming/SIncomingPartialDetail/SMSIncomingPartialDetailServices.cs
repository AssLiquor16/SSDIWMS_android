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
        Setup setup { get; set; }
        public SMSIncomingPartialDetailServices()
        {
            setup = new Setup();
        }
        HttpClient client;

        public async Task<IEnumerable<IncomingPartialDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter)
        {
            var ip = setup.getIp();
            switch (type)
            {
                case "All":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
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
                        client.BaseAddress = new Uri(ip);
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
        public async Task<IEnumerable<IncomingPartialDetailModel>> NewGetList(IncomingPartialDetailModel obj = null, string type = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(setup.getIp());
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 10000000;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            switch (type)
            {
                default:
                    var jsonDefault = await client.GetStringAsync($"api/IncomingPartialDetails");
                    var caseDefault = JsonConvert.DeserializeObject<IEnumerable<IncomingPartialDetailModel>>(jsonDefault);
                    return caseDefault;
                case "BillDoc":
                    var jasonA = await client.GetStringAsync($"api/IncomingPartialDetails/GetBillDoc/{obj.BillDoc}");
                    var caseA = JsonConvert.DeserializeObject<IEnumerable<IncomingPartialDetailModel>>(jasonA);
                    return caseA;
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

        public async Task<IncomingPartialDetailModel> NewInsert(IncomingPartialDetailModel obj, string type = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(setup.getIp());
            client.DefaultRequestHeaders.Accept.Clear();
            client.MaxResponseContentBufferSize = 10000000;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            switch (type)
            {
                default:
                    var jsonDefault = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(jsonDefault, Encoding.UTF8, "application/json");
                    var r = await client.PostAsync("api/IncomingPartialDetails/", content);
                    var ret = r.Content.ReadAsStringAsync().Result;
                    var res = r.StatusCode.ToString();
                    var n = JsonConvert.DeserializeObject<IncomingPartialDetailModel>(ret);
                    return n;
            }
        }

        public async Task<IncomingPartialDetailModel> SpecialCaseInsert(string type, IncomingPartialDetailModel item)
        {
            var ip = setup.getIp();
            switch (type)
            {
                case "ReturnInsertedItem":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
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
            var ip = setup.getIp();
            switch (type)
            {
                case "Common":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
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
