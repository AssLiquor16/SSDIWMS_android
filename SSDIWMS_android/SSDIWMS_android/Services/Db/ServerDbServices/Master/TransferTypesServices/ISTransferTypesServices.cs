using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.TransferTypesServices
{
    public interface ISTransferTypesServices
    {
        Task<IEnumerable<TransferTypesModel>> GetList(TransferTypesModel obj = null, string type = null);
    }
}
