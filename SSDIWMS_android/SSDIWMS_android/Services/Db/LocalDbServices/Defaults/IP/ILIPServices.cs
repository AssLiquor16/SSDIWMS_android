using SSDIWMS_android.Models.DefaultModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP
{
    public interface ILIPServices
    {
        Task<IEnumerable<IPAddressModel>> GetList(IPAddressModel obj = null, string type = null);
        Task<IPAddressModel> GetFirstorDefault(IPAddressModel obj, string type = null);
        Task<IPAddressModel> Insert(IPAddressModel obj, string type = null);
        Task<IPAddressModel> Update(IPAddressModel obj, string type = null);

    }
}
