using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.Master.TransferTypesServices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(STransferTypesServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.TransferTypesServices
{
    public class STransferTypesServices : ISTransferTypesServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public async Task<IEnumerable<TransferTypesModel>> GetList(TransferTypesModel obj = null, string type = null)
        {
            switch (type)
            {
                default:
                    using (client = new HttpClient())
                    {
                        //client.Timeout = TimeSpan.FromSeconds(40);
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/TransferTypes/");
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<TransferTypesModel>>(json);
                        return caseDefault;
                    }

            }
        }
    }
}
