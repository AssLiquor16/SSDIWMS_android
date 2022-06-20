using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet
{
    public interface ISPalletHeaderServices
    {
        Task<IEnumerable<PalletHeaderModel>> GetList(PalletHeaderModel obj = null, string type = null);
        Task<PalletHeaderModel> GetFirstOrdefault(PalletHeaderModel obj, string type = null);
        Task<PalletHeaderModel> Insert(PalletHeaderModel obj, string type = null);
        Task<PalletHeaderModel> Update(PalletHeaderModel obj, string type = null);
    }
}
