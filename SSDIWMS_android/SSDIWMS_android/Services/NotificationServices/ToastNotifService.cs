using Acr.UserDialogs;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToastNotifService))]
namespace SSDIWMS_android.Services.NotificationServices
{
    public class ToastNotifService : IToastNotifService
    {
        public async Task ToastNotif(string type, string msg)
        {
            var toastNotif = Preferences.Get("NotifyIO", false);
            if(toastNotif == true)
            {
                ToastConfig toastcon = new ToastConfig(msg);
                Color color = Color.Black;
                switch (type)
                {
                    case "Pending":
                        color = Color.Orange;
                        break;
                    case "Error":
                        color = Color.Red;
                        break;
                    case "Success":
                        color = Color.Green;
                        break;
                    default:
                        break;
                }
                toastcon.BackgroundColor = color;
                UserDialogs.Instance.Toast(toastcon);
            }
            await Task.CompletedTask;
        }

        public async Task StaticToastNotif(string type, string msg)
        {
            ToastConfig toastcon = new ToastConfig(msg);
            Color color = Color.Black;
            switch (type)
            {
                case "Pending":
                    color = Color.Orange;
                    break;
                case "Error":
                    color = Color.Red;
                    break;
                case "Success":
                    color = Color.Green;
                    break;
                default:
                    break;
            }
            toastcon.BackgroundColor = color;
            UserDialogs.Instance.Toast(toastcon);
            await Task.CompletedTask;
        }

        public Task LoadingProcess(string type, string msg)
        {
            var load = UserDialogs.Instance.Loading(msg);
            switch (type)
            {
                case "Begin":
                    
                    load.Show();
                    break;
                case "End":
                    load.Dispose();
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
