using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseMaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerWarehouseMasterServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseMaster
{
    public class ServerWarehouseMasterServices : IServerWarehouseMasterServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public async Task<WarehouseModel> GetFirstOrDefault(WarehouseModel obj, string type = null)
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
                        var json = await client.GetStringAsync($"api/warehouses/{obj.WarehouseId}");
                        var caseDefault = JsonConvert.DeserializeObject<WarehouseModel>(json);
                        return caseDefault;
                    }


            }
        }

        public async Task<IEnumerable<WarehouseModel>> GetList(WarehouseModel obj = null, string type = null)
        {
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/warehouses");
                        var warehouse = JsonConvert.DeserializeObject<IEnumerable<WarehouseModel>>(json);
                        return warehouse;
                    }
                default: return null;

            }
        }
    }
}
