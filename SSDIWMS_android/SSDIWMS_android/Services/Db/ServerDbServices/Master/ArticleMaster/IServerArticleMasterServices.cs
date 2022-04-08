using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.ArticleMaster
{
    public interface IServerArticleMasterServices
    {
        Task<IEnumerable<ArticleMasterModel>> GetList (string type, string[] stringfilter,int[] intfilter);
    }
}
