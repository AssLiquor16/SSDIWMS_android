using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace SSDIWMS_android.Helpers
{
    public class RandomStringGenerator
    {
        private static Random random = new Random();
        public static string RandomString(string firstchar)
        {
            int length = 2;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var site = Preferences.Get("PrefUserWarehouseAssignedId", 0);
            var date = DateTime.Now.ToString("MMddyymmssff");
            var rand = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            var generated = $"{firstchar}-{site}{rand}{date}";
            return $"{firstchar}{rand}{DateTime.Now.ToString("MMddyymmssfffff")}";
        }
        
    }
}
