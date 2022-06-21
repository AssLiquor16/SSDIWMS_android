using Newtonsoft.Json;
using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SStockCardServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SStockCard
{
    public class SStockCardServices : ISStockCardServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<StockCardsModel> GetFirstOrDefault(StockCardsModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StockCardsModel>> GetList(StockCardsModel obj = null, string type = null)
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
                        var json = await client.GetStringAsync($"api/StockCards");
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<StockCardsModel>>(json);
                        return caseDefault;
                    }
                case "ItemCode/WarehouseLocation":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/StockCards/GetItemCode/{obj.ItemCode}/{obj.Warehouse_Location}");
                        var caseA = JsonConvert.DeserializeObject<IEnumerable<StockCardsModel>>(json);
                        return caseA;
                    }
            }
        }

        public async Task<StockCardsModel> Insert(StockCardsModel obj, string type = null)
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
                        var r = await client.PostAsync("api/StockCards/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                        var caseDefault = JsonConvert.DeserializeObject<StockCardsModel>(ret);
                        return caseDefault;
                    }

            }
        }

        public async Task<StockCardsModel> Update(StockCardsModel obj, string type = null)
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
                        var res = await client.PutAsync($"api/StockCards/{obj.SCardId}", content);
                        var sult = res.StatusCode.ToString();
                        return null;
                    }
            }
        }
    }
}
