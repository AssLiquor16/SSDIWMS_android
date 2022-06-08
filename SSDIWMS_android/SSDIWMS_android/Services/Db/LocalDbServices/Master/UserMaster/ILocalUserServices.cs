using SSDIWMS_android.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.UserMaster
{
    public interface ILocalUserServices
    {
        Task<IEnumerable<UsermasterModel>> GetList(object obj = null, string type = null);
        Task<UsermasterModel> GetFirstOrDefault(object obj, string type = null);
        Task<UsermasterModel> Insert(object obj, string type = null);
        Task<UsermasterModel> Update(object obj, string type = null);
        Task Remove(object obj = null, string type = null);
    }
}
