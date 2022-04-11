using SQLite;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalPalletMasterServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.PalletMaster
{
    public class LocalPalletMasterServices : ILocalPalletMasterServices
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
                await db_.CreateTableAsync<PalletMasterModel>();
            }

        }

        public async Task<IEnumerable<PalletMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "All":
                    var contents = await db_.Table<PalletMasterModel>().ToListAsync();
                    return contents;
                default: return null;
            }
        }

        public async Task<PalletMasterModel> GetModel(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "PId":
                    var contentfilter = intfilter[0];
                    var content = await db_.Table<PalletMasterModel>().Where(x => x.PId == contentfilter).FirstOrDefaultAsync();
                    return content;
                default : return null;
            }
        }

        public async Task Insert(string type, PalletMasterModel content)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    await db_.InsertAsync(content);
                    break;
                default: break;
            }
        }

        public async Task Update(string type, PalletMasterModel content)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var data = await db_.Table<PalletMasterModel>().Where(x=>x.PId == content.PId).FirstOrDefaultAsync();
                    data = content;
                    await db_.UpdateAsync(data);
                    break;
            }
        }
    }
}
