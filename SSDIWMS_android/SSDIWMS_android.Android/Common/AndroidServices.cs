using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SSDIWMS_android.Droid.Common;
using SSDIWMS_android.Services.DeviceServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.Provider.Settings;

[assembly: Dependency(typeof(AndroidServices))]
namespace SSDIWMS_android.Droid.Common
{
    public class AndroidServices : IDroidDeviceServices
    {
        
        public string GetDeviceInfo(string type)
        {
            switch (type)
            {
                case "Serial":
                    var context = Android.App.Application.Context;
                    string serial = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Secure.AndroidId);
                    return serial;
                case "Model":
                    var model = DeviceInfo.Model;
                    return model;
                case "Manufacturer":
                    var manufacturer = DeviceInfo.Manufacturer;
                    return manufacturer;
                case "Name":
                    var name = DeviceInfo.Name;
                    return name;
                case "Version":
                    var version = DeviceInfo.Version.ToString();
                    return version;
                case "Platform":
                    var platform = DeviceInfo.Platform.ToString();
                    return platform;
                case "Idiom":
                    var idiom = DeviceInfo.Idiom.ToString();
                    return idiom;
                case "Type":
                    var devtype = DeviceInfo.DeviceType.ToString();
                    return devtype;
                default:
                    return null;

            }
        }
    }
}