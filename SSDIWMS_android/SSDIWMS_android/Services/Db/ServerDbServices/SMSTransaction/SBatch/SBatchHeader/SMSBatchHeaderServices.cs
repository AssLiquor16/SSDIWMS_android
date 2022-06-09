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
    
        public Task<BatchHeaderModel> Insert(object obj = null, string type = null)
        {
            throw new NotImplementedException();
        }

        public Task<BatchHeaderModel> Update(object obj = null, string type = null)
        {
            throw new NotImplementedException();
        }
    }
}
