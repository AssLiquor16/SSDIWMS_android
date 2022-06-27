using Acr.UserDialogs;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.DefaultModel;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using SSDIWMS_android.Services.DeviceServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using SSDIWMS_android.ViewModels.PopUpVMs;
using SSDIWMS_android.Views;
using SSDIWMS_android.Views.PopUpPages;
using System;
using System.Collections.Generic;
using System.Linq;
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
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        string _username, _password;
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public AsyncCommand SetIpCommand { get; }
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
            SetIpCommand = new AsyncCommand(SetIp);
            RegisterCommand = new AsyncCommand(Register);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        #region Login and set defaults of the user
        public async Task Login()
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                var con = new LoadingPopupVM();
                await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("Login"));
                try
                {
                    var devSerial = deviceService.GetDeviceInfo("Serial").ToUpperInvariant();
                    string[] strignarray = { devSerial };
                    var regDev = await serverDbDeviceService.ReturnInt("DeviceCount", strignarray, null);

                    if (regDev == 1)
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
                                await SetStagingLocation(returnval.WarehouseAssignedId); // sets staging warehouselocation for adding palletheader
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
        private async Task SetStagingLocation(int warehouseAsignedId)
        {
            try
            {
                var warehouse = await dependencies.serverDbWarehouseService.GetFirstOrDefault(new WarehouseModel { WarehouseId = warehouseAsignedId });
                Preferences.Set("PrefWarehouseName", warehouse.W_Location);
                Preferences.Set("PrefWarehouseInitial", warehouse.W_LocationInitial);
                var staging = await dependencies.serverDbTWarehouseLocationService.GetList(new WarehouseLocationModel { Warehouse = warehouse.W_LocationInitial, Final_Location = "STAGE" }, "Final_Loc/Warehouse");
                var dPHeaderLoc = staging.FirstOrDefault();
                if (dPHeaderLoc != null)
                {
                    await dependencies.localDbStagingWarehouseLocationService.Delete();
                    await dependencies.localDbStagingWarehouseLocationService.Insert(new StagingWarehouseLocationModel
                    {
                        LocId = dPHeaderLoc.LocId,
                        Warehouse = dPHeaderLoc.Warehouse,
                        Area = dPHeaderLoc.Area,
                        Rack = dPHeaderLoc.Rack,
                        Level = dPHeaderLoc.Level,
                        Bin = dPHeaderLoc.Bin,
                        UOM = dPHeaderLoc.UOM,
                        Final_Location = dPHeaderLoc.Final_Location,
                        DateCreated = dPHeaderLoc.DateCreated,
                        DateUpdated = dPHeaderLoc.DateUpdated,
                        MultiplePallet = dPHeaderLoc.MultiplePallet,
                        IsBlockStock = dPHeaderLoc.IsBlockStock,
                        MaxPallet = dPHeaderLoc.MaxPallet,
                    });
                }
            }
            catch
            {
                await dependencies.notifService.StaticToastNotif("Error", "Cannot find the warehouse.");
            }
        }
        #endregion
        #region Register device and SetIp
        public async Task SetIp() => await PopupNavigation.Instance.PushAsync(new IPListPopupPage());
        public async Task Register() => await PopupNavigation.Instance.PushAsync(new FormPopupPage("AdminRegDev"));
        #endregion

        public async Task PageRefresh()
        {
            bool login = Preferences.Get("PrefLoggedIn", false);
            if (login == true)
            {
                var route = $"//{nameof(DashBoardPage)}";
                await Shell.Current.GoToAsync(route);
            }
        }

       
        
    }
}
