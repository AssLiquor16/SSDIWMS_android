using Newtonsoft.Json;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.STPalletMaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(STPalletMasterServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.STPalletMaster
{
    public class STPalletMasterServices : ISTPalletMasterServices
    {
        string BaseUrl = Ip_Conf.baseUrl;
        HttpClient client;

        public Task<PalletMasterModel> GetFirstOrDefault(PalletMasterModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PalletMasterModel>> GetList(PalletMasterModel obj = null, string type = null)
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
                        var json = await client.GetStringAsync($"api/Pallets");
                        var caseDefault = JsonConvert.DeserializeObject<IEnumerable<PalletMasterModel>>(json);
                        return caseDefault;
                    }
                case "PalletCode/Status=Not-Use":
                    using(client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/Pallets/SearchPallet/{obj.PalletCode}/Not-Use");
                        var caseA = JsonConvert.DeserializeObject<IEnumerable<PalletMasterModel>>(json);
                        return caseA;
                    }

            }
        }

        public Task<PalletMasterModel> Insert(PalletMasterModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<PalletMasterModel> Update(PalletMasterModel obj, string type = null)
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
                        var res = await client.PutAsync($"api/Pallets/{obj.PalletCode}", content);
                        var sult = res.StatusCode.ToString();
                        return null;
                    }
            }
        }
    }
}
