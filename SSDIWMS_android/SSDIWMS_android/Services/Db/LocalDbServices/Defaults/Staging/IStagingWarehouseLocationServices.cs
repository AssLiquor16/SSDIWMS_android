using SSDIWMS_android.Models.DefaultModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Defaults
{
    public interface IStagingWarehouseLocationServices
    {
        Task<IEnumerable<StagingWarehouseLocationModel>> GetList(StagingWarehouseLocationModel obj, string type = null);
        Task<StagingWarehouseLocationModel> GetFirstOrDefault(StagingWarehouseLocationModel obj = null, string type = null);
        Task Insert(StagingWarehouseLocationModel obj, string type = null);
        Task Update(StagingWarehouseLocationModel obj, string type = null);
        Task Delete(StagingWarehouseLocationModel obj = null, string type = null);
    }
}
