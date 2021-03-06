using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchDetails;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSBatchDetailServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchDetails
{
    public class SMSBatchDetailServices : ISMSBatchDetailServices
    {
        Setup setup { get; set; }
        public SMSBatchDetailServices()
        {
            setup = new Setup();
        }
        HttpClient client;

        public async Task<BatchDetailsModel> GetFirstOrDefault(object obj = null, string type = null)
        {
            var ip = setup.getIp();
            var data = (obj as BatchDetailsModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/incomingdetails/");
                        var datas = JsonConvert.DeserializeObject<BatchDetailsModel>(json);
                        return datas;
                    }

                default: return null;
            }
        }

        public async Task<IEnumerable<BatchDetailsModel>> GetList(object obj = null, string type = null)
        {
            var ip = setup.getIp();
            var data = (obj as BatchDetailsModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/BatchDetails");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<BatchDetailsModel>>(json);
                        return datas;
                    }
                case "BatchCode":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/BatchDetails/PerBatchCode/{data.BatchCode}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<BatchDetailsModel>>(json);
                        return datas;
                    }
                default: return null;
            }
        }

        public async Task<BatchDetailsModel> Insert(object obj = null, string type = null)
        {
            var ip = setup.getIp();
            var data = (obj as BatchDetailsModel);
            switch (type)
            {
                case "ReturnInsertedItem":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var r = await client.PostAsync("api/BatchDetails/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var caseA = JsonConvert.DeserializeObject<BatchDetailsModel>(ret);
                        return caseA;
                    }
                default: return null;
            }
        }

        public async Task<BatchDetailsModel> Update(object obj = null, string type = null)
        {
            var ip = setup.getIp();
            var data = (obj as BatchDetailsModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/BatchDetails/{data.BatchDetId}", content);
                        var sult = res.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(sult);
                        return null;
                    }
                default: return null;
            }
        }
    }
}
