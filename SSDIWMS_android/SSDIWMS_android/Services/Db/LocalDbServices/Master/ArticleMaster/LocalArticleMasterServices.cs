using SQLite;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalArticleMasterServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.ArticleMaster
{
    public class LocalArticleMasterServices : ILocalArticleMasterServices
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
                await db_.CreateTableAsync<ItemMasterModel>();
            }

        }

        public async Task<IEnumerable<ItemMasterModel>> GetList(string type, string[] stringarray, int[] intarray)
        {
            await Init();
            switch (type)
            {
                case "All":
                    var items = await db_.Table<ItemMasterModel>().ToListAsync();
                    return items;
                case "CaseCode":
                    var caseCodeFilter = stringarray[0];
                    var count = await db_.Table<ItemMasterModel>().CountAsync();
                    var items2 = await db_.Table<ItemMasterModel>().Where(x=>x.CaseCode == caseCodeFilter).ToListAsync();
                    return items2;
                default: return null;
            }
        }
        public async Task<ItemMasterModel> GetModel(string type, string[] stringfilter, int[] intfilter)
        {
            await Init();
            switch (type)
            {
                case "ItemId":
                    var filter = intfilter[0];
                    var item = await db_.Table<ItemMasterModel>().Where(x => x.ItemId == filter).FirstOrDefaultAsync();
                    return item;
                default: return null;
            }

            
            
        }

        public async Task Insert(string type, ItemMasterModel item)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    await db_.InsertAsync(item);
                    break;
                default: break;
            }
        }

        public async Task Update(string type, ItemMasterModel item)
        {
            await Init();
            switch (type)
            {
                case "Common":
                    var data = await db_.Table<ItemMasterModel>().Where(x=>x.ItemId == item.ItemId).FirstOrDefaultAsync();
                    data = item;
                    await db_.UpdateAsync(data);
                    break;
            }
        }

        public async Task Clear()
        {
            await Init();
            await db_.DeleteAllAsync<ItemMasterModel>();
        }

        public async Task<ItemMasterModel> GetFirstOrDefault(ItemMasterModel obj, string type = null)
        {
            await Init();
            switch (type)
            {
                default: return await db_.Table<ItemMasterModel>().Where(x=>x.ItemId ==obj.ItemId).FirstOrDefaultAsync();
                case "ItemCode/ItemDesc":
                    var caseA = await db_.Table<ItemMasterModel>().Where(x => x.ItemCode == obj.ItemCode && x.ItemDesc == obj.ItemDesc).FirstOrDefaultAsync();
                    return caseA;
                case "ItemCode":
                    var caseB = await db_.Table<ItemMasterModel>().Where(x => x.ItemCode == obj.ItemCode).FirstOrDefaultAsync();
                    return caseB;
            }
        }
    }
}
