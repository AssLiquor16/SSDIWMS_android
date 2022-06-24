using SQLite;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.TransferTypesServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LTransferTypesServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.TransferTypesServices
{
    public class LTransferTypesServices : ILTransferTypesServices
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
                await db_.CreateTableAsync<TransferTypesModel>();
            }

        }
        public async Task<TransferTypesModel> GetFirstOrDefault(TransferTypesModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<TransferTypesModel>().Where(x => x.TransferId == obj.TransferId).FirstOrDefaultAsync();
                    return caseDefault;
            }
        }
        public async Task<IEnumerable<TransferTypesModel>> GetList(TransferTypesModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    return await db_.Table<TransferTypesModel>().ToListAsync();
            }
        }

        public async Task<TransferTypesModel> Insert(TransferTypesModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    await db_.InsertAsync(obj);
                    return null;
            }
        }

        public async Task<TransferTypesModel> Update(TransferTypesModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<TransferTypesModel>().Where(x=>x.TransferId == obj.TransferId).FirstOrDefaultAsync();
                    var caseDefaultData = obj;
                    await db_.UpdateAsync(caseDefaultData);
                    return null;

            }
        }
    }
}
