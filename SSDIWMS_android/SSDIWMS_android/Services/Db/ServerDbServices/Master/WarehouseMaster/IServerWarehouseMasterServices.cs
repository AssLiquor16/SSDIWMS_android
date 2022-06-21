using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseMaster
{
    public interface IServerWarehouseMasterServices
    {
        Task<IEnumerable<WarehouseModel>> GetList(WarehouseModel obj = null, string type = null);
        Task<WarehouseModel> GetFirstOrDefault(WarehouseModel obj, string type = null);
    }
}
