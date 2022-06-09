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
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public async Task<BatchDetailsModel> GetFirstOrDefault(object obj = null, string type = null)
        {
            var data = (obj as BatchDetailsModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
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
            var data = (obj as BatchDetailsModel);
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/BatchDetails");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<BatchDetailsModel>>(json);
                        return datas;
                    }
                
                default: return null;
            }
        }

        public Task<BatchDetailsModel> Insert(object obj = null, string type = null)
        {
            throw new NotImplementedException();
        }

        public Task<BatchDetailsModel> Update(object obj = null, string type = null)
        {
            throw new NotImplementedException();
        }
    }
}
