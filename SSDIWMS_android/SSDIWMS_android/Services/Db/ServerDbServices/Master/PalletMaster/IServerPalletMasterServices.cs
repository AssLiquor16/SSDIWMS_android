using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster
{
    public interface IServerPalletMasterServices
    {
        Task<IEnumerable<PalletMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter);
    }
}
