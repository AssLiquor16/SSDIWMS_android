using SSDIWMS_android.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.ServerDbServices.Users
{
    public interface IServerUserServices
    {
        Task<UsermasterModel> ReturnModel(string type, string[] stringCredentials, int[] intCredentials);
        Task<IEnumerable<UsermasterModel>> ReturnList(string type, string[] stringFilter, int[] intFilter);
        Task Update(string type, string[] stringdata, int[] intdata, UsermasterModel user);
    }
}
