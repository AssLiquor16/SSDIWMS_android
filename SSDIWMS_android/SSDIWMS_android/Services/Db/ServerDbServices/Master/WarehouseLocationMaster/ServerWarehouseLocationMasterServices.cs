using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseLocationMaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerWarehouseLocationMasterServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseLocationMaster
{
    public class ServerWarehouseLocationMasterServices : IServerWarehouseLocationMasterServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public async Task<IEnumerable<WarehouseLocationModel>> GetList(WarehouseLocationModel obj = null, string type = null)
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
                        var json = await client.GetStringAsync($"api/WarehouseLocations");
                        var WarehouseLocations = JsonConvert.DeserializeObject<IEnumerable<WarehouseLocationModel>>(json);
                        return WarehouseLocations;
                    }
                default: return null;

            }
        }

    }
}
