using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchHeader;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSBatchHeaderServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchHeader
{
    public class SMSBatchHeaderServices : ISMSBatchHeaderServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<BatchHeaderModel> GetFirstOrDefault(object obj = null, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BatchHeaderModel>> GetList(object obj = null, string type = null)
        {
            var data = (obj as BatchHeaderModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/BatchHeaders");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<BatchHeaderModel>>(json);
                        return datas;
                    }
                case "UserId":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/BatchHeaders/GetPerUser/{data.UserCreated}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<BatchHeaderModel>>(json);
                        return datas;
                    }
                default: return null;
            }
        }
    
        public async Task<BatchHeaderModel> Insert(object obj = null, string type = null)
        {
            var data = (obj as BatchHeaderModel);
            switch (type)
            {
                case "ReturnInsertedItem":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var r = await client.PostAsync("api/BatchHeaders/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var caseA = JsonConvert.DeserializeObject<BatchHeaderModel>(ret);
                        return caseA;
                    }
                default: return null;
            }
        }

        public async Task<BatchHeaderModel> Update(object obj = null, string type = null)
        {
            var data = (obj as BatchHeaderModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/BatchHeaders/{data.BatchId}", content);
                        var sult = res.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(sult);
                        return null;
                    }
                default: return null;
            }
        }
    }
}
