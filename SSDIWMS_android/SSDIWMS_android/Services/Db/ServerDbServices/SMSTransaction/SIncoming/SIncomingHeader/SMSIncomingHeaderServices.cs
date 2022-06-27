using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMSIncomingHeaderServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader
{
    public class SMSIncomingHeaderServices : ISMSIncomingHeaderServices
    {
        Setup setup { get; set; }
        public SMSIncomingHeaderServices()
        {
            setup = new Setup();
        }
        HttpClient client;

        public async Task<IEnumerable<IncomingHeaderModel>> GetList(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter)
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
                        var json = await client.GetStringAsync($"api/IncomingHeaders/");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingHeaderModel>>(json);
                        return datas;
                    }
                case "WhId":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/incomingHeaders/GetSpecificWarehouse/{intfilter[0]}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingHeaderModel>>(json);
                        return datas;
                    }
                case "GetFinalize": 
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var intf = intfilter[0];
                        var json = await client.GetStringAsync($"api/incomingHeaders/GetFinalize/{intf}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingHeaderModel>>(json);
                        return datas;
                    }
                case "GetOngoing":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        int intf = intfilter[0];
                        var json = await client.GetStringAsync($"api/incomingHeaders/GetOngoing/{intf}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingHeaderModel>>(json);
                        return datas;
                    }
                case "GetActDate":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        int intf = intfilter[0];
                        var json = await client.GetStringAsync($"api/IncomingHeaders/GetActDate/{intf}");
                        var datas = JsonConvert.DeserializeObject<IEnumerable<IncomingHeaderModel>>(json);
                        return datas;
                    }
                default: return null;
            }
        }

        public async Task<string> GetString(string type, string[] stringfilter, int[] intfilter)
        {
            var ip = setup.getIp();
            switch (type)
            {
                case "ReturnStatus":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/incomingHeaders/GetPOStatus/{stringfilter[0]}");
                        var datas = JsonConvert.DeserializeObject<string>(json);
                        return datas;
                    }
                default: return null;
            }
        }

        public async Task Update(string type, IncomingHeaderModel data)
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
                        var json = JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/incomingHeaders/{data.INCId}", content);
                        var sult = res.StatusCode.ToString();
                    }
                    break;
            }
        }
    }
}
