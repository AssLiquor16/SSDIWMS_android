using Newtonsoft.Json;
using SSDIWMS_android.Models;
using SSDIWMS_android.Services.ServerDbServices.Users;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServerUserServices))]
namespace SSDIWMS_android.Services.ServerDbServices.Users
{
    public class ServerUserServices : IServerUserServices
    {
        string BaseUrl = Preferences.Get("PrefServerAddress", "http://192.168.1.217:80/");
        HttpClient client;

        public async Task<IEnumerable<UsermasterModel>> ReturnList(string type, string[] stringFilter, int[] intFilter)
        {
            if(type == "All")
            {
                using (client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(40);
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.MaxResponseContentBufferSize = 10000000;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var json = await client.GetStringAsync($"api/Usermasters/");
                    var userMasters = JsonConvert.DeserializeObject<IEnumerable<UsermasterModel>>(json);
                    return userMasters;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<UsermasterModel> ReturnModel(string type, string[] stringCredentials, int[] intCredentials)
        {
            if (type == "Login")
            {
                var username = stringCredentials[0];
                var password = stringCredentials[1];
                using (client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.MaxResponseContentBufferSize = 10000000;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var json = await client.GetStringAsync($"api/Usermasters/{username}/{password}");
                    var userMasters = JsonConvert.DeserializeObject<UsermasterModel>(json);
                    return userMasters;
                }
            }
            else if(type == "Id")
            {
                using (client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.MaxResponseContentBufferSize = 10000000;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var json = await client.GetStringAsync($"api/Usermasters/getSpecific/{intCredentials[0]}");
                    var userMasters = JsonConvert.DeserializeObject<UsermasterModel>(json);
                    return userMasters;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task Update(string type, string[] stringdata, int[] intdata, UsermasterModel user)
        {
            switch (type)
            {
                case "Login":
                    using (client = new HttpClient())
                    {
                        var cred = new UsermasterModel
                        {
                            UserId = user.UserId,
                            UserFullName = user.UserFullName,
                            UserName = user.UserName,
                            Password = user.Password,
                            UserRole = user.UserRole,
                            UserDeptId = user.UserDeptId,
                            WarehouseAssignedId = user.WarehouseAssignedId,
                            UserStatus = user.UserStatus,
                            PasswordHash = user.PasswordHash,
                            PasswordSalt = user.PasswordSalt,
                            LoginStatus = stringdata[0]

                        };

                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(cred);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/UserMasters/{user.UserId}", content);
                        var sult = res.StatusCode.ToString();
                    }
                    break;

                case "Logout":
                    using (client = new HttpClient())
                    {
                        var cred = new UsermasterModel
                        {
                            UserId = user.UserId,
                            UserFullName = user.UserFullName,
                            UserName = user.UserName,
                            Password = user.Password,
                            UserRole = user.UserRole,
                            UserDeptId = user.UserDeptId,
                            WarehouseAssignedId = user.WarehouseAssignedId,
                            UserStatus = user.UserStatus,
                            PasswordHash = user.PasswordHash,
                            PasswordSalt = user.PasswordSalt,
                            LoginStatus = " "

                        };

                        client.BaseAddress = new Uri(BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.MaxResponseContentBufferSize = 10000000;
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var json = JsonConvert.SerializeObject(cred);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync($"api/UserMasters/{intdata[0]}", content);
                        var sult = res.StatusCode.ToString();
                    }
                    break;
                default:

                    break;
            }
        }

    }
}
