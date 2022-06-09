using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMLBatchDetailsServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails
{
    public class SMLBatchDetailsServices : ISMLBatchDetailsServices
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
                await db_.CreateTableAsync<BatchDetailsModel>();
            }

        }

        public async Task<BatchDetailsModel> GetFirstOrDefault(BatchDetailsModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null: 
                    return await db_.Table<BatchDetailsModel>().Where(x=>x.BatchDetId == obj.BatchDetId && x.DateAdded == obj.DateAdded).FirstOrDefaultAsync();
                case "BatchDetId/DateCreated":
                    return await db_.Table<BatchDetailsModel>().Where(x => x.BatchDetId == obj.BatchDetId && x.DateAdded == obj.DateAdded).FirstOrDefaultAsync();
                default: return null;

            }
        }

        public async Task<IEnumerable<BatchDetailsModel>> GetList(BatchDetailsModel obj = null, string type = null)
        {
                await Init();
                switch (type)
                {
                    case null: return await db_.Table<BatchDetailsModel>().ToListAsync();
                    case "BatchCode": return await db_.Table<BatchDetailsModel>().Where(x => x.BatchCode == obj.BatchCode).ToListAsync();
                    default: return null;

                }
        }

        public async Task<BatchDetailsModel> Insert(BatchDetailsModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    var caseA = obj;
                    await db_.InsertAsync(caseA);
                    return caseA;
                default: return null;

            }
        }

        public async Task<BatchDetailsModel> Update(BatchDetailsModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    var caseA = await db_.Table<BatchDetailsModel>().Where(x => x.BatchDetId == obj.BatchDetId && x.DateAdded == obj.DateAdded).FirstOrDefaultAsync();
                    caseA.BatchId = obj.BatchId;
                    caseA.BatchCode = obj.BatchCode;
                    caseA.ItemCode = obj.ItemCode;
                    caseA.ItemDesc = obj.ItemDesc;
                    caseA.Qty = obj.Qty;
                    caseA.TimesUpdated = obj.TimesUpdated;
                    await db_.UpdateAsync(caseA);
                    return caseA;
                default: return null;

            }
        }

        public async Task Remove(object obj, string type = null)
        {
            var model = (obj as BatchDetailsModel);
            await Init();
            switch (type)
            {
                case null: await db_.DeleteAsync(model); break;
                case "All": await db_.DeleteAllAsync<BatchDetailsModel>(); break;
            }
        }
    }
}
