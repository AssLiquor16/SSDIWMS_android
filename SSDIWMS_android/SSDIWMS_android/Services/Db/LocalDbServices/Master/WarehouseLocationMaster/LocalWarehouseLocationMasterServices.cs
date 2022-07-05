using SQLite;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalWarehouseLocationMasterServices))]
namespace SSDIWMS_android.Services.Db.LocalDbServices.Master.WarehouseLocationMaster
{
    public class LocalWarehouseLocationMasterServices : ILocalWarehouseLocationMasterServices
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
                await db_.CreateTableAsync<WarehouseLocationModel>();
            }

        }

        public async Task<WarehouseLocationModel> GetFirstOrDefault(WarehouseLocationModel obj = null, string type = null)
        {
           await Init();
            switch (type)
            {
                case null:
                    return await db_.Table<WarehouseLocationModel>().Where(x => x.LocId == obj.LocId).FirstOrDefaultAsync();
                default: return null;
            }
        }

        public async Task<IEnumerable<WarehouseLocationModel>> GetList(WarehouseLocationModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    return await db_.Table<WarehouseLocationModel>().ToListAsync();
                case "Area":
                    return await db_.Table<WarehouseLocationModel>().Where(x=>x.Area == obj.Area).ToListAsync();
                case "Warehouse":
                    var a = await db_.Table<WarehouseLocationModel>().ToListAsync();
                    return await db_.Table<WarehouseLocationModel>().Where(x => x.Warehouse == obj.Warehouse).ToListAsync();
                default: return null;
            }
        }

        public async Task Insert(WarehouseLocationModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null:
                    await db_.InsertAsync(obj);
                    break;
            }
        }

        public async Task Update(WarehouseLocationModel obj = null, string type = null)
        {
            await Init();
            switch (type)
            {
                case null :
                    var updateA = await db_.Table<WarehouseLocationModel>().Where(x => x.LocId == obj.LocId).FirstOrDefaultAsync();
                    updateA.Warehouse = obj.Warehouse;
                    updateA.Area = obj.Area;
                    updateA.Rack = obj.Rack;
                    updateA.Level = obj.Level;
                    updateA.Bin = obj.Bin;
                    updateA.UOM = obj.UOM;
                    updateA.Final_Location = obj.Final_Location;
                    updateA.DateCreated = obj.DateCreated;
                    updateA.DateUpdated = obj.DateUpdated;
                    updateA.MultiplePallet = obj.MultiplePallet;
                    updateA.IsBlockStock = obj.IsBlockStock;
                    updateA.MaxPallet = obj.MaxPallet;
                    await db_.UpdateAsync(updateA);
                    break;
                default:break;
            }
        }
    }
}
