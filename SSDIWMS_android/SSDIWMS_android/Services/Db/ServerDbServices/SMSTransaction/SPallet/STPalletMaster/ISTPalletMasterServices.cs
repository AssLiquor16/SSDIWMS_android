using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.STPalletMaster
{
    public interface ISTPalletMasterServices
    {
        Task<IEnumerable<PalletMasterModel>> GetList(PalletMasterModel obj = null, string type = null);
        Task<PalletMasterModel> GetFirstOrDefault(PalletMasterModel obj, string type = null);
        Task<PalletMasterModel> Insert(PalletMasterModel obj, string type = null);
        Task<PalletMasterModel> Update(PalletMasterModel obj, string type = null);
    }
}
