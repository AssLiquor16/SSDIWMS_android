using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Helpers
{
    public class PercentageCalculator
    {
        public PercentageCalculator()
        {

        }
        public async Task<string> GetPercentage(string type, decimal[] decimalarray)
        {
            await Task.Delay(1);
            decimal e = decimalarray[0] / decimalarray[1];
            decimal f = e * 100;
            decimal g = decimal.Round(f, 2, MidpointRounding.AwayFromZero);
            g = Math.Round(g, 0);
            var msg = "";
            switch (type)
            {
                case "ArticleMaster":
                    msg = "Article master  " + g + "% out of 100%";
                    break;
                case "PalletMaster":
                    msg = "Pallet Master  " + g + "% out of 100%";
                    break;
                case "IncomingDetail":
                    msg = "Acuracy: " + g + " %";
                    break;
                case "Sites":
                    msg = "Sites Master: " + g + " %";
                    break;
                case "WarehouseLocation":
                    msg = "WarehouseLocation Master: " + g + " %";
                    break;
                case "Warehouse":
                    msg = "Warehouse Master: " + g + " %";
                    break;
                default: return null;
            }
            return msg;

        }
    }
}
