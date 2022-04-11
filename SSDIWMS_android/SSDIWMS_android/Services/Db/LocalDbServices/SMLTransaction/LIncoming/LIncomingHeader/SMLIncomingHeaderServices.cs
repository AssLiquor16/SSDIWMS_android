using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMLIncomingHeaderServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader
{
    public class SMLIncomingHeaderServices : ISMLIncomingHeaderServices
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
                await db_.CreateTableAsync<IncomingHeaderModel>();
            }

        }

        public async Task<IEnumerable<IncomingHeaderModel>> GetList(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter)
        {
            await Init();
            switch (type)
            {
                case "All":
                    var Allcontents = await db_.Table<IncomingHeaderModel>().ToListAsync();
                    return Allcontents;
                case "WhId,CurDate,OngoingIncStat":
                    var WhId = Preferences.Get("PrefUserWarehouseAssignedId", 0);
                    var datenow = DateTime.Today;
                    var retcontents = await db_.Table<IncomingHeaderModel>().ToListAsync();
                    var filtercontents = retcontents.Where(x =>x.WarehouseId == WhId && x.ActRecDate == datenow).ToList();
                    return filtercontents;
                default: return null;

            }
        }

        public async Task<IncomingHeaderModel> GetModel(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter)
        {
            await Init();
            switch (type)
            {
                case "INCId":
                    var filter = intfilter[0];
                    var content = await db_.Table<IncomingHeaderModel>().Where(x => x.INCId == filter).FirstOrDefaultAsync();
                    return content;
                default: return null;
            }
        }

        public async Task Insert(string type, IncomingHeaderModel data)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    await db_.InsertAsync(data);
                    break;
            }
        }

        public async Task Update(string type, IncomingHeaderModel data)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var content = await db_.Table<IncomingHeaderModel>().Where(x=>x.INCId == data.INCId).FirstOrDefaultAsync();
                    content = data;
                    await db_.UpdateAsync(content);
                    break;
            }
        }
    }
}
