using SSDIWMS_android.Models.MasterListModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.SiteMaster
{
    public interface ILocalSiteMasterServices
    {
        Task<SitesModel> GetModel(string type, int id);
        Task Insert(string type, SitesModel site);
        Task Update(string type,SitesModel site);
    }
}
