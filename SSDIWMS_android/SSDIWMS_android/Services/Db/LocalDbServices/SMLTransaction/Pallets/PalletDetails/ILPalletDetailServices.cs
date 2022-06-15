using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets.PalletDetails
{
    public interface ILPalletDetailServices
    {
        Task<IEnumerable<PalletDetailsModel>> GetList(PalletDetailsModel obj = null, string type = null);
        Task<PalletDetailsModel> GetFirstOrDefault(PalletDetailsModel obj, string type = null);
        Task<PalletDetailsModel> Insert(PalletDetailsModel obj, string type = null);
        Task<PalletDetailsModel> Update(PalletDetailsModel obj, string type = null);
        Task Delete(PalletDetailsModel obj, string type = null);
    }
}
