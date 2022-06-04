using Acr.UserDialogs;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using SSDIWMS_android.Services.DeviceServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using SSDIWMS_android.ViewModels.PopUpVMs;
using SSDIWMS_android.Views;
using SSDIWMS_android.Views.PopUpPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels
{
    public class LoginVM : ViewModelBase
    {
        IToastNotifService toastNotifService;
        IServerUserServices serverDbUserService;
        IServerDeviceServices serverDbDeviceService;
        IDroidDeviceServices deviceService;

        string _username, _password;
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public AsyncCommand LoginCommand { get; }
        public AsyncCommand RegisterCommand { get; }
        public AsyncCommand PageRefreshCommand { get; }
        public LoginVM()
        {
            toastNotifService = DependencyService.Get<IToastNotifService>();
            serverDbUserService = DependencyService.Get<IServerUserServices>();
            serverDbDeviceService = DependencyService.Get<IServerDeviceServices>();
            deviceService = DependencyService.Get<IDroidDeviceServices>();
            LoginCommand = new AsyncCommand(Login);
            RegisterCommand = new AsyncCommand(Register);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        public async Task Login()
        {
            if(!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                var con = new LoadingPopupVM();
                await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("Login"));
                try
                {
                    var devSerial  = deviceService.GetDeviceInfo("Serial").ToUpperInvariant();
                    string[] strignarray = { devSerial };
                    var regDev = await serverDbDeviceService.ReturnInt("DeviceCount", strignarray, null);

                    if(regDev == 1)
                    {
                        string[] uandp = { Username, Password };
                        var returnval = await serverDbUserService.ReturnModel("Login", uandp, null);
                        
                        if (returnval != null)
                        {
                            
                            if (returnval.UserStatus == "Active" && string.IsNullOrWhiteSpace(returnval.LoginStatus)) 
                            {
                                await serverDbUserService.Update("Login", strignarray, null, returnval);
                                Preferences.Set("PrefUserFullname", returnval.UserFullName);
                                Preferences.Set("PrefUserId", returnval.UserId);
                                Preferences.Set("PrefUserRole", returnval.UserRole);
                                Preferences.Set("PrefUserWarehouseAssignedId", returnval.WarehouseAssignedId);
                                Preferences.Set("PrefLoggedIn", true);
                                MessagingCenter.Send(this, "FromLoginToShell", "Go");
                                var route = $"//{nameof(DashBoardPage)}";
                                await Shell.Current.GoToAsync(route);
                            }
                            else
                            {
                                await toastNotifService.StaticToastNotif("Error", "User is currently inactive or already loggedin");
                            }
                        }
                        else
                        {
                            await toastNotifService.StaticToastNotif("Error", "User doesnt exist.");
                        }
                    }
                    else
                    {
                        await toastNotifService.StaticToastNotif("Error", "Device is not registered.");
                    }

                }
                catch
                {
                    await toastNotifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
                await con.CloseAll();
            }
            else
            {
                await toastNotifService.StaticToastNotif("Error", "Missing entry.");
            }
            
        }
        public async Task Register()
        {

            await PopupNavigation.Instance.PushAsync(new FormPopupPage("AdminRegDev"));

        }
        public async Task PageRefresh()
        {   
            bool login = Preferences.Get("PrefLoggedIn", false);
            if(login == true)
            {
                var route = $"//{nameof(DashBoardPage)}";
                await Shell.Current.GoToAsync(route);
            }
        }
        
    }
}
