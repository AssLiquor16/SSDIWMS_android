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

        public async Task<IEnumerable<IncomingHeaderModel>> GetList(string type = null, string[] stringfilter = null, int[] intfilter = null, DateTime[] datefilter = null, IncomingHeaderModel obj = null)
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
                case "WhId/CurDate/RecievedIncStat":
                    var retcontents2 = await db_.Table<IncomingHeaderModel>().ToListAsync();
                    var filtercontents2 = retcontents2.Where(x => x.WarehouseId == obj.WarehouseId && x.ActRecDate == DateTime.Today && x.INCstatus == "Recieved").ToList();
                    return filtercontents2;
                case "WhId/CurDate/FinalizedIncStat":
                    var case3 = await db_.Table<IncomingHeaderModel>().ToListAsync();
                    var filteredCase3 = case3.Where(x => x.WarehouseId == obj.WarehouseId && x.ActRecDate == DateTime.Today && x.INCstatus == "Finalized").ToList();
                    return filteredCase3;
                case "WhId/CurDate/RecievedIncStat&FinalizedIncStat":
                    var case4 = await db_.Table<IncomingHeaderModel>().ToListAsync();
                    var filteredCase4 = case4.Where(x => x.WarehouseId == obj.WarehouseId && x.ActRecDate == DateTime.Today && x.INCstatus == "Finalized" || x.WarehouseId == obj.WarehouseId && x.ActRecDate == DateTime.Today && x.INCstatus == "Recieved").ToList();
                    return filteredCase4;
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
                case "PONumber":
                    var stringfilterPo = stringfilter[0];
                    var conts = await db_.Table<IncomingHeaderModel>().Where(x => x.PONumber == stringfilterPo).FirstOrDefaultAsync();
                    return conts;
                case "INCId&PO":
                    var sfilter = stringfilter[0];
                    var ifilter = intfilter[0];
                    var cont = await db_.Table<IncomingHeaderModel>().Where(x => x.INCId == ifilter && x.PONumber == sfilter).FirstOrDefaultAsync();
                    return cont;
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
                case "PONumber":
                    var n = await db_.Table<IncomingHeaderModel>().Where(x => x.PONumber == data.PONumber).FirstOrDefaultAsync();
                    n.FinalDate = DateTime.Now;
                    n.INCstatus = data.INCstatus;
                    n.FinalUserId = data.FinalUserId;
                    n.TimesUpdated += data.TimesUpdated;
                    n.DateSync = data.DateSync;
                    await db_.UpdateAsync(n);
                    break;
                case "PONumber1":
                    var o = await db_.Table<IncomingHeaderModel>().Where(x => x.PONumber == data.PONumber).FirstOrDefaultAsync();
                    o.RecDate = DateTime.Now;
                    o.INCstatus = data.INCstatus;
                    o.RecUserId = data.RecUserId;
                    o.TimesUpdated += data.TimesUpdated;
                    o.DateSync = data.DateSync;
                    await db_.UpdateAsync(o);
                    break;
                case "BatchCode":
                    var p = await db_.Table<IncomingHeaderModel>().Where(x=>x.PONumber == data.PONumber && x.INCId == data.INCId).FirstOrDefaultAsync();
                    p.BatchCode = data.BatchCode;
                    p.TimesUpdated = data.TimesUpdated;
                    p.DateSync = data.DateSync;
                    await db_.UpdateAsync(p);
                    break;
            }
        }

        public async Task Clear()
        {
            await Init();
            await db_.DeleteAllAsync<IncomingHeaderModel>();
        }
    }
}
