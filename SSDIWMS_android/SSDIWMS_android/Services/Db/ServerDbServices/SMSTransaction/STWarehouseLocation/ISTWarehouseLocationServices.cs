using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.STWarehouseLocation
{
    public interface ISTWarehouseLocationServices
    {
        Task<IEnumerable<WarehouseLocationModel>> GetList(WarehouseLocationModel obj = null, string type = null);
        Task<WarehouseLocationModel> GetFirstOrDefault(WarehouseLocationModel obj, string type = null);
        Task<WarehouseLocationModel> Insert(WarehouseLocationModel obj, string type = null);
        Task<WarehouseLocationModel> Update(WarehouseLocationModel obj, string type = null);
    }
}
