using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets
{
    public interface ILPalletHeaderServices
    {
        Task<IEnumerable<PalletHeaderModel>> GetList(PalletHeaderModel obj = null, string type = null);
        Task<PalletHeaderModel> GetFirstOrDefault(PalletHeaderModel obj, string type = null);
        Task<PalletHeaderModel> Insert(PalletHeaderModel obj, string type = null);
        Task<PalletHeaderModel> Update(PalletHeaderModel obj, string type = null);
        Task Delete(PalletHeaderModel obj = null, string type = null);
    }
}
