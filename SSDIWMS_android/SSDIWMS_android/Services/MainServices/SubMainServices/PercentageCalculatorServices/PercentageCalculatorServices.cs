using SSDIWMS_android.Services.MainServices.SubMainServices.PercentageCalculatorServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PercentageCalculatorServices))]
namespace SSDIWMS_android.Services.MainServices.SubMainServices.PercentageCalculatorServices
{
    public class PercentageCalculatorServices : IPercentageCalculatorServices
    {
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
                    msg = "PalletMaster  " + g + "% out of 100%";
                    break;
                case "IncomingDetail":
                    msg = "Acuracy: " + g + " %";
                    break;
                default: return null;
            }
            return msg;
            
        }
    }
}
