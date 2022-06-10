using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMLBatchHeaderServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader
{
    public class SMLBatchHeaderServices : ISMLBatchHeaderServices
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
                await db_.CreateTableAsync<BatchHeaderModel>();
            }

        }

        public async Task<BatchHeaderModel> GetFirstOrDefault(BatchHeaderModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    return await db_.Table<BatchHeaderModel>().Where(x => x.BatchLocalID == obj.BatchLocalID && x.DateCreated == obj.DateCreated).FirstOrDefaultAsync();
                default: return null;
            }
        }

        public async Task<IEnumerable<BatchHeaderModel>> GetList(BatchHeaderModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    return await db_.Table<BatchHeaderModel>().ToListAsync();
                case "UserId":
                    return await db_.Table<BatchHeaderModel>().Where(x => x.UserCreated == obj.UserCreated).ToListAsync();
                case "ZeroServerId":
                    return await db_.Table<BatchHeaderModel>().Where(x => x.BatchId == 0).ToListAsync();
                default: return null;
            }
        }

        public async Task<BatchHeaderModel> Insert(BatchHeaderModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    await db_.InsertAsync(obj);
                    return null;
                case "GenerateBatchCode":
                    var ret = obj;
                    await db_.InsertAsync(ret);
                    return await Update(ret, "GenerateBatchCode");
                default: return null;
            }
        }

        public async Task<BatchHeaderModel> Update(BatchHeaderModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    var caseA = await db_.Table<BatchHeaderModel>().Where(x=>x.BatchLocalID == obj.BatchLocalID && x.DateCreated == obj.DateCreated).FirstOrDefaultAsync();
                    caseA.BatchId = obj.BatchId;
                    caseA.BatchCode = obj.BatchCode;
                    caseA.UserCreated = obj.UserCreated;
                    caseA.Remarks = obj.Remarks;
                    caseA.TimesUpdated = obj.TimesUpdated;
                    caseA.DateSync = obj.DateSync;
                    await db_.UpdateAsync(caseA);
                    return null;
                case "GenerateBatchCode":
                    var caseB = await db_.Table<BatchHeaderModel>().Where(x => x.BatchLocalID == obj.BatchLocalID && x.DateCreated == obj.DateCreated).FirstOrDefaultAsync();
                    caseB.BatchId = obj.BatchId;
                    caseB.BatchCode = $"{obj.BatchCode}{obj.BatchLocalID}";
                    caseB.UserCreated = obj.UserCreated;
                    caseB.Remarks = obj.Remarks;
                    caseB.TimesUpdated = obj.TimesUpdated;
                    caseB.DateSync = obj.DateSync;
                    await db_.UpdateAsync(caseB);
                    return caseB;
                default: return null;
            }
        }

        public async Task Remove(object obj, string type = null)
        {
            var model = (obj as BatchHeaderModel);
            await Init();
            switch (type)
            {
                case null: await db_.DeleteAsync(model); break;
                case "All": await db_.DeleteAllAsync<BatchHeaderModel>(); break;
            }
        }
    }
}
