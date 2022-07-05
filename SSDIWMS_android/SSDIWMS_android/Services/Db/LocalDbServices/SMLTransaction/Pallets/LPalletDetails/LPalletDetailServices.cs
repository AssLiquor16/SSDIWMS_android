using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets.LPalletDetails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LPalletDetailServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets.LPalletDetails
{
    public class LPalletDetailServices : ILPalletDetailServices
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
                await db_.CreateTableAsync<PalletDetailsModel>();
            }

        }
        
        public async Task<PalletDetailsModel> GetFirstOrDefault(PalletDetailsModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default: return await db_.Table<PalletDetailsModel>().Where(x => x.PHeaderLocalID == obj.PHeaderLocalID).FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<PalletDetailsModel>> GetList(PalletDetailsModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case "PalletCode":
                    var test = await db_.Table<PalletDetailsModel>().ToListAsync();
                    var caseA = await db_.Table<PalletDetailsModel>().Where(x => x.PalletCode == obj.PalletCode).ToListAsync();
                    return caseA;
                default: return await db_.Table<PalletDetailsModel>().ToListAsync();
            }
        }

        public async Task<PalletDetailsModel> Insert(PalletDetailsModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var data = obj;
                    await db_.InsertAsync(data);
                    return data;
            }
        }

        public async Task<PalletDetailsModel> Update(PalletDetailsModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default: return null;
            }
        }
        public async Task Delete(PalletDetailsModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default: await db_.DeleteAllAsync<PalletDetailsModel>(); break;
            }
        }
    }
}
