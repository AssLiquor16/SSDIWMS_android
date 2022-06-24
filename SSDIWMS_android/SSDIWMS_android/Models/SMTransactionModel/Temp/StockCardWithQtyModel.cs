using SSDIWMS_android.Models.SMTransactionModel.StockCard;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Temp
{
    public class StockCardWithQtyModel
    {
        public StockCardsModel StockCard { get; set; }
        public int Qty { get; set; }
    }
}
