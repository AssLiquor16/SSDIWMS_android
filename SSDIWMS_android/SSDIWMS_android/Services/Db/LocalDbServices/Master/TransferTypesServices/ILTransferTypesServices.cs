using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.TransferTypesServices
{
    public interface ILTransferTypesServices
    {
        Task<IEnumerable<TransferTypesModel>> GetList(TransferTypesModel obj = null, string type = null);
        Task<TransferTypesModel> GetFirstOrDefault(TransferTypesModel obj, string type = null);
        Task<TransferTypesModel> Insert(TransferTypesModel obj, string type = null);
        Task<TransferTypesModel> Update(TransferTypesModel obj, string type = null);
    }
}
