using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.MainServices.SubMainServices.PercentageCalculatorServices
{
    public interface IPercentageCalculatorServices
    {
        Task<string> GetPercentage(string type, decimal[] decimalarray); 
    }
}
