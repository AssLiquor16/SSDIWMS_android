using Newtonsoft.Json;
using SSDIWMS_android.Models;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerDeviceServices))]
namespace SSDIWMS_android.Services.Db.ServerDbServices.Devices
{
    public class ServerDeviceServices : IServerDeviceServices
    {
        Setup setup { get; set; }
        public ServerDeviceServices()
        {
            setup = new Setup();
        }
        HttpClient client;

        public async Task<int> ReturnInt(string type, string[] stringdata, int[] intdata)
        {
            var ip = setup.getIp();
            switch (type)
            {
                case "DeviceCount":
                    
                    var serial = stringdata[0];
                    using (client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = await client.GetStringAsync($"api/Devices/Checkserial/{serial}");
                        var i = JsonConvert.DeserializeObject<int>(json);
                        return i;
                    }
                default:
                    return 0;
            }
        }
        public async Task InsertData(string type, string[] stringdata, int[] intdata)
        {
            var ip = setup.getIp();
            var firstdata = stringdata[0].ToUpperInvariant();
            var secondata = stringdata[1].ToUpperInvariant();
            var thirddata = stringdata[2].ToUpperInvariant();
            switch (type)
            {
                
                case "RegisterDevice":
                    using (client = new HttpClient())
                    {
                        var i = new DevicesMasterModel
                        {
                            DeviceName = secondata,
                            DeviceModel = thirddata,
                            DeviceSerial = firstdata
                        };
                        client.BaseAddress = new Uri(ip);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(i);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var r = await client.PostAsync("api/Devices/", content);
                        var ret = r.Content.ReadAsStringAsync().Result;
                        var res = r.StatusCode.ToString();
                    }
                    break;
                default:

                    break;

            }
        }

        public async Task<DateTime> GetServerDate()
        {
            var ip = setup.getIp();
            using (client = new HttpClient())
            {
                client.BaseAddress = new Uri(ip);
                client.DefaultRequestHeaders.Accept.Clear();
                client.MaxResponseContentBufferSize = 10000000;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = await client.GetStringAsync($"api/Devices/ServerDate");
                var i = JsonConvert.DeserializeObject<DateTime>(json);
                return i;
            }
        }
    }
}
