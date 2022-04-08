using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster
{
    public interface ILocalArticleMasterServices
    {
        Task<IEnumerable<ItemMasterModel>> GetList(string type,string[] stringarray, int[] intarray);
        Task<ItemMasterModel> GetModel(string type, string[] stringfilter, int[] intfilter);
        Task Insert(string type, ItemMasterModel item);
        Task Update(string type, ItemMasterModel item);
    }
}
