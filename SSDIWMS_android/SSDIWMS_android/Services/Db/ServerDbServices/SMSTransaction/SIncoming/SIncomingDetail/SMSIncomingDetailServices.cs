using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingDetail;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSIncomingDetailServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingDetail
{
    public class SMSIncomingDetailServices : ISMSIncomingDetailServices
    {
        string BaseUrl = Preferences.Get("PrefServerAddress", "http://192.168.1.217:80/");
        HttpClient client;

        public async Task<IEnumerable<IncomingDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter)
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
                        var json = await client.GetStringAsync($"api/incomingdetails/");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingDetailModel>>(json);
                        return datas;
                    }
                case "PONumber":
                    using (client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(40);
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/incomingdetails/GetPODetails/{stringfilter[0]}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingDetailModel>>(json);
                        return datas;
                    }
                default: return null;
            }
        }

        public Task<IncomingDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string type, IncomingDetailModel data)
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
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/IncomingDetails/{data.INCDetId}", content);
                        var sult = res.StatusCode.ToString();
                    }
                    break;
            }
        }
    }
}
