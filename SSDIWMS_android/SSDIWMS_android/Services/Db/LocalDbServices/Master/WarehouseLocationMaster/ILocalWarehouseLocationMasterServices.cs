using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster
{
    public interface ILocalWarehouseLocationMasterServices
    {
        Task<IEnumerable<WarehouseLocationModel>> GetList(WarehouseLocationModel obj = null, string type = null);
        Task<WarehouseLocationModel>GetFirstOrDefault(WarehouseLocationModel obj = null, string type = null);
        Task Insert(WarehouseLocationModel obj = null, string type = null);
        Task Update(WarehouseLocationModel obj = null, string type = null);
    }
}
