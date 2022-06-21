using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard.SStockTransferHistories;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SStockTransferHistoriesServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard.SStockTransferHistories
{
    public class SStockTransferHistoriesServices : ISStockTransferHistoriesServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<StockTransferHistoryModel> GetFirstOrDefault(StockTransferHistoryModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StockTransferHistoryModel>> GetList(StockTransferHistoryModel obj = null, string type = null)
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
                        var json = await client.GetStringAsync($"api/StockTransferHistories");
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<StockTransferHistoryModel>>(json);
                        return caseDefault;
                    }
            }
        }

        public async Task<StockTransferHistoryModel> Insert(StockTransferHistoryModel obj, string type = null)
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
                        var r = await client.PostAsync("api/StockTransferHisotories/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var caseDefault = JsonConvert.DeserializeObject<StockTransferHistoryModel>(ret);
                        return caseDefault;
                    }

            }
        }

        public Task<StockTransferHistoryModel> Update(StockTransferHistoryModel obj, string type = null)
        {
            throw new NotImplementedException();
        }
    }
}
