using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.WarehouseLocationMaster
{
    public interface IServerWarehouseLocationMasterServices
    {
        Task<IEnumerable<WarehouseLocationModel>> GetList(WarehouseLocationModel obj = null, string type = null);
    }
}
