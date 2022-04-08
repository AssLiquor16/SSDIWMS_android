using Newtonsoft.Json;
using SSDIWMS_android.Models.TransactionModels;
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
        string BaseUrl = Preferences.Get("PrefServerAddress", "http://192.168.1.217:80/");
        HttpClient client;

        public async Task<IEnumerable<PalletMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter)
        {
            switch (type)
            {
                case "All":
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
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
