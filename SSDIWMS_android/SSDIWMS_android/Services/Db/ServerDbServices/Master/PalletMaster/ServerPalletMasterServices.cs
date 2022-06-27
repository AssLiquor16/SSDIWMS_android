using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerPalletMasterServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster
{
    public class ServerPalletMasterServices : IServerPalletMasterServices
    {
        Setup setup { get; set; }
        public ServerPalletMasterServices()
        {
            setup = new Setup();
        }
        HttpClient client;

        public async Task<IEnumerable<PalletMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter)
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
                        var json = await client.GetStringAsync($"api/Pallets");
                        var pallets = JsonConvert.DeserializeObject<IEnumerable<PalletMasterModel>>(json);
                        return pallets;
                    }
                default:
                    return null;
            }
        }
    }
}
