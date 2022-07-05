using SQLite;
using SSDIWMS_android.Models.DefaultModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Defaults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(StagingWarehouseLocationServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Defaults
{
    public class StagingWarehouseLocationServices : IStagingWarehouseLocationServices
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
                await db_.CreateTableAsync<StagingWarehouseLocationModel>();
            }

        }

        public async Task Delete(StagingWarehouseLocationModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    await db_.DeleteAllAsync<StagingWarehouseLocationModel>();
                    break;
            }
        }

        public async Task<StagingWarehouseLocationModel> GetFirstOrDefault(StagingWarehouseLocationModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<StagingWarehouseLocationModel>().FirstOrDefaultAsync();
                    return caseDefault;
            }
        }

        public async Task Insert(StagingWarehouseLocationModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    await db_.InsertAsync(obj);
                    break;
            }
        }

        public Task Update(StagingWarehouseLocationModel obj, string type = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StagingWarehouseLocationModel>> GetList(StagingWarehouseLocationModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<StagingWarehouseLocationModel>().ToListAsync();
                    return caseDefault;
            }
        }
    }
}
