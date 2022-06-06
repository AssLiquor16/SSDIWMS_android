using SQLite;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalWarehouseMasterServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseMaster
{
    public class LocalWarehouseMasterServices : ILocalWarehouseMasterServices
    {
        SQLiteAsyncConnection db_;
        async Task Init()
        {

            var LocDbAddress = Preferences.Get("PrefLocalAddress", "SSDIWMSLoc.db");
            if (db_ != null)
            {
                return;
            }
            else
            {
                var DbPath = Path.Combine(FileSystem.AppDataDirectory, LocDbAddress);
                db_ = new SQLiteAsyncConnection(DbPath);
                await db_.CreateTableAsync<WarehouseModel>();
            }

        }
        public async Task<WarehouseModel> GetFirstOrDefault(WarehouseModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    return await db_.Table<WarehouseModel>().Where(x=>x.WarehouseId == obj.WarehouseId).FirstOrDefaultAsync();
                default: return null;

            }
        }

        public async Task<IEnumerable<WarehouseModel>> GetList(WarehouseModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null: return await db_.Table<WarehouseModel>().ToListAsync();
                default: return null;
            }
        }

        public async Task Insert(WarehouseModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                case null: await db_.InsertAsync(obj); break;
                default: break;
            }
        }

        public async Task Update(WarehouseModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    var updateA = await db_.Table<WarehouseModel>().Where(x => x.WarehouseId == obj.WarehouseId).FirstOrDefaultAsync();
                    updateA.W_Location = obj.W_Location;
                    updateA.W_LocationInitial = obj.W_LocationInitial;
                    await db_.UpdateAsync(updateA);
                    break;
                default: break;

            }
        }
    }
}
