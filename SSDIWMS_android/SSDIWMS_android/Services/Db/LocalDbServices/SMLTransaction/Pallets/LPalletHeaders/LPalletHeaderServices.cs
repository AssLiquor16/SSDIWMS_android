using SQLite;
using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LPalletHeaderServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.Pallets
{
    internal class LPalletHeaderServices : ILPalletHeaderServices
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
                await db_.CreateTableAsync<PalletHeaderModel>();
            }

        }

        public async Task<PalletHeaderModel> GetFirstOrDefault(PalletHeaderModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                case "PalletCode":
                    var caseA = await db_.Table<PalletHeaderModel>().Where(x => x.PalletCode == obj.PalletCode).FirstOrDefaultAsync();
                    return caseA;
                default: return await db_.Table<PalletHeaderModel>().Where(x => x.PHeaderLocalID == obj.PHeaderLocalID).FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<PalletHeaderModel>> GetList(PalletHeaderModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default: return await db_.Table<PalletHeaderModel>().ToListAsync();
            }
        }

        public async Task<PalletHeaderModel> Insert(PalletHeaderModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default: await db_.InsertAsync(obj); return null;
            }
        }

        public Task<PalletHeaderModel> Update(PalletHeaderModel obj, string type = null)
        {
            throw new NotImplementedException();
        }
        public async Task Delete(PalletHeaderModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default: await db_.DeleteAllAsync<PalletHeaderModel>(); break;
            }
        }
    }
}
