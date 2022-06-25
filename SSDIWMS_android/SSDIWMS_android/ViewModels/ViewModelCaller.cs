using SSDIWMS_android.ViewModels.StockMovementVMs.StockTransferVMs.STPalletToLocationVMs.StockMovementVMs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.ViewModels
{
    public class ViewModelCaller
    {
        public SMPHTrasnferFromVM StockMovementViewModel { get; set; }

        public ViewModelCaller()
        {
            StockMovementViewModel = new SMPHTrasnferFromVM();
        }
    }
}
