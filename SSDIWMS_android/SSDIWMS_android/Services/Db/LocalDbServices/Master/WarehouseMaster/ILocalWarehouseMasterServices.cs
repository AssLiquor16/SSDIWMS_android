using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster
{
    public interface ILocalWarehouseMasterServices
    {
        Task<IEnumerable<WarehouseModel>> GetList(WarehouseModel obj = null, string type = null);
        Task<WarehouseModel> GetFirstOrDefault(WarehouseModel obj = null, string type = null);
        Task Insert(WarehouseModel obj, string type = null);
        Task Update(WarehouseModel obj, string type = null);
    }
}
