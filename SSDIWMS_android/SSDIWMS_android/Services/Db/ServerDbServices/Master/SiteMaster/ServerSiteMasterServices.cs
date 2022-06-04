using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.SiteMaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerSiteMasterServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.SiteMaster
{

    public class ServerSiteMasterServices : IServerSiteMasterServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public async Task<IEnumerable<SitesModel>> GetList(string type, string[] stringfilter, int[] intfilter)
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
                        var json = await client.GetStringAsync($"api/Sites");
                        var pallets = JsonConvert.DeserializeObject<IEnumerable<SitesModel>>(json);
                        return pallets;
                    }
                default:
                    return null;
            }
        }
    }
}
