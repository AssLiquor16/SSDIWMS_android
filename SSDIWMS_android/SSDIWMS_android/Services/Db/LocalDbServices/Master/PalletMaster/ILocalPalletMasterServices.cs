using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster
{
    public interface ILocalPalletMasterServices
    {
        Task<IEnumerable<PalletMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter);
        Task<PalletMasterModel> GetModel(string type, string[] stringfilter, int[] intfilter);
        Task Insert(string type, PalletMasterModel content);
        Task Update(string type, PalletMasterModel content);
        Task Clear();
    }
}
