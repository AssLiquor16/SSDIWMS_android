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
        Setup setup { get; set; }
        public ServerSiteMasterServices()
        {
            setup = new Setup();
        }
        HttpClient client;

        public async Task<IEnumerable<SitesModel>> GetList(string type = null, string[] stringfilter = null, int[] intfilter = null)
        {
            var ip = setup.getIp();
            switch (type)
            {
                case null:
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
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
