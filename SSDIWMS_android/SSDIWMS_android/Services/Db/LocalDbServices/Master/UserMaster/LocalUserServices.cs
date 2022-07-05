using SQLite;
using SSDIWMS_android.Models;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.UserMaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalUserServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.UserMaster
{
    public class LocalUserServices : ILocalUserServices
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
                await db_.CreateTableAsync<UsermasterModel>();
            }

        }
        public async Task<UsermasterModel> GetFirstOrDefault(object obj, string type = null)
        {
            var model = (obj as UsermasterModel);
            await Init();
            switch (type)
            {
                case null: return await db_.Table<UsermasterModel>().Where(x=>x.UserId == model.UserId).FirstOrDefaultAsync();
                default: return null;

            }
        }

        public async Task<IEnumerable<UsermasterModel>> GetList(object obj = null, string type = null)
        {
            var model = (obj as UsermasterModel);
            await Init();
            switch (type)
            {
                case null: return await db_.Table<UsermasterModel>().ToListAsync();
                default: return null;

            }
        }

        public async Task<UsermasterModel> Insert(object obj, string type = null)
        {
            var filter = (obj as UsermasterModel);
            await Init();
            switch (type)
            {
                case null: await db_.InsertAsync(filter); return null;
                default: return null;

            }
        }

        public async Task<UsermasterModel> Update(object obj, string type = null)
        {
            var model = (obj as UsermasterModel);
            await Init();
            switch (type)
            {
                case null:
                    var caseA = await GetFirstOrDefault(model);
                    caseA.UserFullName = model.UserFullName;
                    caseA.UserName = model.UserName;
                    caseA.Password = model.Password;
                    caseA.UserRole = model.UserRole;
                    caseA.UserDeptId = model.UserDeptId;
                    caseA.WarehouseAssignedId = model.WarehouseAssignedId;
                    caseA.UserStatus = model.UserStatus;
                    caseA.PasswordHash = model.PasswordHash;
                    caseA.PasswordSalt = model.PasswordSalt;
                    caseA.LoginStatus = string.Empty;
                    caseA.Salesmancode = model.Salesmancode;
                    await db_.UpdateAsync(caseA);
                    return null;
                default: return null;

            }
        }

        public async Task Remove(object obj = null, string type = null)
        {
            var filter = (obj as UsermasterModel);
            await Init();
            switch (type)
            {
                case null: await db_.DeleteAsync(filter); break;
                case "All": await db_.DeleteAllAsync<UsermasterModel>(); break;
                default: break;

            }
        }
    }
}
