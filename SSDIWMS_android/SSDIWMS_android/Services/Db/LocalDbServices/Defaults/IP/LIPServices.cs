using SQLite;
using SSDIWMS_android.Models.DefaultModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LIPServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Defaults.IP
{
    public class LIPServices : ILIPServices
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
                await db_.CreateTableAsync<IPAddressModel>();
            }

        }

        public async Task<IPAddressModel> GetFirstorDefault(IPAddressModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<IPAddressModel>().Where(x => x.Ip_Id == obj.Ip_Id).FirstOrDefaultAsync();
                    return caseDefault;
            }
        }

        public async Task<IEnumerable<IPAddressModel>> GetList(IPAddressModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<IPAddressModel>().ToListAsync();
                    return caseDefault;
            }
        }

        public async Task<IPAddressModel> Insert(IPAddressModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    await db_.InsertAsync(obj);
                    return null;
            }
        }

        public async Task<IPAddressModel> Update(IPAddressModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default:
                    var caseDefault = await db_.Table<IPAddressModel>().Where(x => x.Ip_Id == obj.Ip_Id).FirstOrDefaultAsync();
                    caseDefault = obj;
                    await db_.UpdateAsync(caseDefault);
                    return null;
            }
        }

        public async Task Delete(IPAddressModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                default: await db_.DeleteAllAsync<IPAddressModel>(); break;
                case "Single": await db_.DeleteAsync(obj); break;
            }
        }
    }
}
