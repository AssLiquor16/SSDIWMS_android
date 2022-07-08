using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.ViewModels.StockMovementVMs.IncomingVMs.PurchaseOrderVMs2
{
    public static class POStaticDatas
    {
        public static int UserId;
        public static string UserRole;
        public static int WarehouseId;
        public static string BillDoc;

        public static void setUserId(int id)
        {
            UserId = id;
        }
        public static void SetUserRole(string role)
        {
            UserRole = role;
        }
        public static void SetWarehouseId(int warehouseId)
        {
            WarehouseId = warehouseId;
        }
        public static void SetBillDoc(string billDoc)
        {
            BillDoc = billDoc;
        }

    }
}
