using SQLite;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.SiteMaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalSiteMasterServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.SiteMaster
{
    public class LocalSiteMasterServices : ILocalSiteMasterServices
    {
        SQLiteAsyncConnection db_;
        async Task Init()
        {
            if (db_ != null)
            {
                return;
            }
            else
            {
                var DbPath = Path.Combine(FileSystem.AppDataDirectory, Setup.baseLocalAddress);
                db_ = new SQLiteAsyncConnection(DbPath);
                await db_.CreateTableAsync<SitesModel>();
            }

        }

        public async Task<SitesModel> GetModel(string type, int id)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var all = await db_.Table<SitesModel>().ToListAsync();
                    var aa = await db_.Table<SitesModel>().Where(x => x.SiteId == id).FirstOrDefaultAsync();
                    return aa;
                case "WarehouseId":
                    var alls = await db_.Table<SitesModel>().ToListAsync();
                    var ab = await db_.Table<SitesModel>().Where(x => x.WarehouseId == id).FirstOrDefaultAsync();
                    return ab;
                case "SiteId":
                    return await db_.Table<SitesModel>().Where(x => x.SiteId == id).FirstOrDefaultAsync();
                default: return null;
            }
        }

        public async Task Insert(string type, SitesModel site)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    await db_.InsertAsync(site);
                    break;
                default: break;
            }
        }

        public async Task Update(string type, SitesModel site)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var ca = await db_.Table<SitesModel>().Where(x => x.SiteId == site.SiteId).FirstOrDefaultAsync();
                    ca.SiteDescription = site.SiteDescription;
                    ca.WarehouseId = site.WarehouseId;
                    ca.SiteCompleteDesc = site.SiteCompleteDesc;
                    await db_.UpdateAsync(ca);
                    break;
            }
        }

    }
}
