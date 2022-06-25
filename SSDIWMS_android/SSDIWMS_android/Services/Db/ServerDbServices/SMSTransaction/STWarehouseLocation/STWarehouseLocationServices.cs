using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.STWarehouseLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(STWarehouseLocationServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.STWarehouseLocation
{
    public class STWarehouseLocationServices : ISTWarehouseLocationServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<WarehouseLocationModel> GetFirstOrDefault(WarehouseLocationModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WarehouseLocationModel>> GetList(WarehouseLocationModel obj = null, string type = null)
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
                        var json = await client.GetStringAsync($"api/WarehouseLocations");
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<WarehouseLocationModel>>(json);
                        return caseDefault;
                    }
                case "Final_Loc/Warehouse":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/WarehouseLocations/SearchWarehouseLocation/{obj.Final_Location}/{obj.Warehouse}");
                        var caseA = JsonConvert.DeserializeObject<IEnumerable<WarehouseLocationModel>>(json);
                        return caseA;
                    }
                case "Final_Loc":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/WarehouseLocations/getWhLoc/{obj.Final_Location}");
                        var caseB = JsonConvert.DeserializeObject<IEnumerable<WarehouseLocationModel>>(json);
                        return caseB;
                    }
            }
        }

        public Task<WarehouseLocationModel> Insert(WarehouseLocationModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<WarehouseLocationModel> Update(WarehouseLocationModel obj, string type = null)
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
                        var res = await client.PutAsync($"api/WarehouseLocations/{obj.LocId}", content);
                        var sult = res.StatusCode.ToString();
                        return null;
                    }
            }
        }

    }
    
}
