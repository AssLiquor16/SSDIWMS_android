using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.Master.SiteMaster
{
    public interface IServerSiteMasterServices
    {
        Task<IEnumerable<SitesModel>> GetList(string type, string[] stringfilter, int[] intfilter);
    }
}
