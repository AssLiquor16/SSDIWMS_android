using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMLIncomingDetailServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail
{
    public class SMLIncomingDetailServices : ISMLIncomingDetailServices
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
                await db_.CreateTableAsync<IncomingDetailModel>();
            }

        }
        public async Task<IEnumerable<IncomingDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "All":
                    var NoFilterDatas = await db_.Table<IncomingDetailModel>().ToListAsync();
                    return NoFilterDatas;
                case "PONumber":
                    var POFilter = Preferences.Get("PrefPONumber", "");
                    var alls = await db_.Table<IncomingDetailModel>().ToListAsync();
                    var POFilterDatas = await db_.Table<IncomingDetailModel>().Where(x => x.POHeaderNumber == POFilter).ToListAsync();
                    return POFilterDatas;
                case "PONumber,TimesUpdated":
                    var POFilter1 = Preferences.Get("PrefPONumber", "");
                    var POTimesUpdateFilterDatas = await db_.Table<IncomingDetailModel>().Where(x => x.POHeaderNumber == POFilter1 && x.TimesUpdated > 0).ToListAsync();
                    return POTimesUpdateFilterDatas;
                case "PONumber2":
                    var POFilter2 = stringfilter[0];
                    var alls2 = await db_.Table<IncomingDetailModel>().ToListAsync();
                    var POFilterDatas2 = await db_.Table<IncomingDetailModel>().Where(x => x.POHeaderNumber == POFilter2).ToListAsync();
                    return POFilterDatas2;
                default: return null;
            }
        }

        public async Task<IncomingDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "INCDetId":
                    var INCIdFilter = intfilter[0];
                    var INCIdFiterData = await db_.Table<IncomingDetailModel>().Where(x => x.INCDetId == INCIdFilter).FirstOrDefaultAsync();
                    return INCIdFiterData;
                case "PO,ItemCode":
                    var poNumber = stringfilter[0];
                    var itemCode = stringfilter[1];
                    var POAndItemCodeFilteredItem = await db_.Table<IncomingDetailModel>().Where(x => x.POHeaderNumber == poNumber && x.ItemCode == itemCode).FirstOrDefaultAsync();
                    return POAndItemCodeFilteredItem;
                default: return null;
            }
        }

        public async Task Insert(string type, IncomingDetailModel item)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    await db_.InsertAsync(item);
                    break;
            }
        }

        public async Task Update(string type, IncomingDetailModel item)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var content = await db_.Table<IncomingDetailModel>().Where(x => x.INCDetId == item.INCDetId).FirstOrDefaultAsync();
                    content = item;
                    content.TimesUpdated = item.TimesUpdated;
                    content.UserId = item.UserId;
                    content.Cqty = item.Cqty;
                    content.DateSync = item.DateSync;

                    await db_.UpdateAsync(content);
                    break;
            }
        }

        public async Task Clear()
        {
            await Init();
            await db_.DeleteAllAsync<IncomingDetailModel>();
        }
    }
}
