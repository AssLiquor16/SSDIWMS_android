using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerArticleMasterServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster
{
    public class ServerArticleMasterServices : IServerArticleMasterServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public async Task<IEnumerable<ArticleMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter)
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
                        var json = await client.GetStringAsync($"api/ArticleMasters/Get/Active");
                        var articleMasters = JsonConvert.DeserializeObject<IEnumerable<ArticleMasterModel>>(json);
                        return articleMasters;
                    }
                default:
                    return null;
            }
        }
    }
}
