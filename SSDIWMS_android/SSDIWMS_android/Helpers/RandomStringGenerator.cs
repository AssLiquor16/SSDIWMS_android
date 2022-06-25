using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSDIWMS_android.Helpers
{
    public class RandomStringGenerator
    {
        private static Random random = new Random();
        public static string RandomString(string firstchar)
        {
            int length = 3;
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rand = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return $"{firstchar}{rand}{DateTime.Now.ToString("MddyyHHmmssff")}";

        }
    }
}
