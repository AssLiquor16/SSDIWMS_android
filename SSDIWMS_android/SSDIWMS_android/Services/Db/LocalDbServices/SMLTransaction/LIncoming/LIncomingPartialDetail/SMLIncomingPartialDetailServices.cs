using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMLIncomingPartialDetailServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail
{
    public class SMLIncomingPartialDetailServices : ISMLIncomingPartialDetailServices
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
                await db_.CreateTableAsync<IncomingPartialDetailModel>();
            }

        }

        public async Task<IEnumerable<IncomingPartialDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "All":
                    var all = await db_.Table<IncomingPartialDetailModel>().ToListAsync();
                    return all;
                case "PONumber":
                    var POFilter = Preferences.Get("PrefPONumber", "");
                    var PO = await db_.Table<IncomingPartialDetailModel>().Where(x=>x.POHeaderNumber == POFilter).ToListAsync();
                    return PO;
                case "PONumber&UserId":
                    var POFilter3 = Preferences.Get("PrefPONumber", "");
                    var UserFilter1 = Preferences.Get("PrefUserId", 0);
                    var pritm = await db_.Table<IncomingPartialDetailModel>().Where(x => x.POHeaderNumber == POFilter3 && x.UserId == UserFilter1).ToListAsync();
                    return pritm;
                case "PONumber&INCId":
                    var POFilter1 = Preferences.Get("PrefPONumber", "");
                    var iNCDetId = intfilter[0];
                    var filtereddata = await db_.Table<IncomingPartialDetailModel>().Where(x => x.POHeaderNumber == POFilter1 && x.INCDetId == iNCDetId).ToListAsync();
                    return filtereddata;
                case "PONumber&ItemCode&INCDetId":
                    var POFilter2 = Preferences.Get("PrefPONumber", "");
                    var itemCode = stringfilter[0];
                    var iNCDetId1 = intfilter[0];
                    var e = await db_.Table<IncomingPartialDetailModel>().ToListAsync();
                    var filtereddata1 = await db_.Table<IncomingPartialDetailModel>().Where(x => x.POHeaderNumber == POFilter2 && x.INCDetId == iNCDetId1 && x.ItemCode == itemCode).ToListAsync();
                    return filtereddata1;
                case "PoIc":
                    var firstfilter = stringfilter[0]; // po
                    var secondfilter = stringfilter[1]; ; // itemcode
                    var n = await db_.Table<IncomingPartialDetailModel>().Where(x => x.POHeaderNumber == firstfilter && x.ItemCode == secondfilter).ToListAsync();
                    return n;
                default: return null;
            }
        }

        public async Task<IncomingPartialDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "INCParDetId":
                    var iNCParDetId = intfilter[0];
                    var singleData = await db_.Table<IncomingPartialDetailModel>().Where(x => x.INCParDetId == iNCParDetId).FirstOrDefaultAsync();
                    return singleData;
                case "RefId":
                    var refId = stringfilter[0];
                    var filtereddata = await db_.Table<IncomingPartialDetailModel>().Where(x=>x.RefId == refId).FirstOrDefaultAsync();
                    return filtereddata;
                default: return null;
            }
        }

        public async Task Insert(string type, IncomingPartialDetailModel item)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    await db_.InsertAsync(item);
                    break;
                case "RefIdAutoGenerate":
                    var initialData = new IncomingPartialDetailModel 
                    { 
                        DateCreated = DateTime.Now,
                        RefId = item.INCDetId + "P",
                        INCDetId = item.INCDetId,
                        INCHeaderId = item.INCHeaderId,
                        ItemCode = item.ItemCode,
                        ItemDesc = item.ItemDesc,
                        PartialCQTY = item.PartialCQTY,
                        PalletCode = item.PalletCode,
                        UserId = item.UserId,
                        TimesUpdated = item.TimesUpdated,
                        POHeaderNumber = item.POHeaderNumber,
                        Status = item.Status,
                        ExpiryDate = item.ExpiryDate,
                        DateFinalized = DateTime.MinValue
                    };
                    await db_.InsertAsync(initialData);
                    await Update("RefIdAutoGenerate", initialData);
                    break;
            }
        }

        public async Task Update(string type, IncomingPartialDetailModel item)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var content = await db_.Table<IncomingPartialDetailModel>().Where(x => x.INCParDetId == item.INCParDetId).FirstOrDefaultAsync();
                    content = item;
                    await db_.UpdateAsync(content);
                    break;
                case "RefId":
                    var conte = await db_.Table<IncomingPartialDetailModel>().Where(x => x.RefId == item.RefId).FirstOrDefaultAsync();
                    conte = item;
                    await db_.UpdateAsync(conte);
                    break;
                case "RefIdAutoGenerate":
                    var ret = await db_.Table<IncomingPartialDetailModel>().Where(x => x.INCParDetId == item.INCParDetId).FirstOrDefaultAsync();
                    ret.RefId = ret.RefId + item.INCParDetId;
                    await db_.UpdateAsync(ret);
                    break;
            }
        }

        public async Task Clear()
        {
            await Init();
            await db_.DeleteAllAsync<IncomingPartialDetailModel>();
        }
    }
}
