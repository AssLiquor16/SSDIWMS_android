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
                        var e = new serverIncomingDet
                        {
                            INCDetId = data.INCDetId,
                            INCHeaderId = data.INCHeaderId,
                            ItemCode = data.ItemCode,
                            ItemDesc = data.ItemDesc,
                            Qty = data.Qty,
                            Cqty = data.Cqty,
                            UserId = data.UserId,
                            Amount = data.Amount,
                            TimesUpdated = data.TimesUpdated,
                            POHeaderNumber = data.POHeaderNumber
                        };
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(e);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/IncomingDetails/{e.INCDetId}", content);
                        var sult = res.StatusCode.ToString();
                    }
                    break;
            }
        }
    }
    public partial class serverIncomingDet
    {
        public int INCDetId { get; set; }

        public int INCHeaderId { get; set; }

        public string ItemCode { get; set; }

        public string ItemDesc { get; set; }

        public int Qty { get; set; }

        public int Cqty { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public int TimesUpdated { get; set; }

        public string POHeaderNumber { get; set; }


    }
}
