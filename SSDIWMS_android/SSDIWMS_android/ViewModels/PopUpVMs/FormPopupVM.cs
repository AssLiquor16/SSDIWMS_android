using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Services.Db.ServerDbServices.Devices;
using SSDIWMS_android.Services.DeviceServices;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using SSDIWMS_android.Views.PopUpPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.PopUpVMs
{
    public class FormPopupVM : ViewModelBase
    {
        GlobalDependencyServices dependencies { get; } = new GlobalDependencyServices();
        IMainServices mainService;
        IToastNotifService notifService;
        IDroidDeviceServices deviceService;
        IServerUserServices serverDbUserService;
        IServerDeviceServices serverDbDeviceService;

        string _taskType,_username,_password;
        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        
        public AsyncCommand LoginCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public FormPopupVM()
        {
            notifService = DependencyService.Get<IToastNotifService>();
            deviceService = DependencyService.Get<IDroidDeviceServices>();
            serverDbDeviceService = DependencyService.Get<IServerDeviceServices>();
            serverDbUserService = DependencyService.Get<IServerUserServices>();
            mainService = DependencyService.Get<IMainServices>();


            LoginCommand = new AsyncCommand(Login);
            RefreshCommand = new AsyncCommand(Refresh);
        }
        public async Task Login()
        {
            //await dependencies.dateServices.CheckValidateTime();
            if (TaskType.Contains("Admin"))
            {
                switch (TaskType)
                {
                    case "AdminRegDev":
                        await RegisterDevice();
                        break;
                    case "AdminClearTrans":
                        await ClearTransaction();
                        break;
                    case "ClearAll":
                        await ClearAll();
                        break;
                    case "AdminSearchEnable":
                        await EnableSearch();
                        break;
                    default:
                        break;
                }
            }
            else
            {

            }
        }
        public async Task Refresh()
        {
            await Task.Delay(1);
        }
        public async Task RegisterDevice()
        {
            var con = new LoadingPopupVM();
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage(""));
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                try
                {
                    string[] uandp = { Username, Password };
                    var returnval = await serverDbUserService.ReturnModel("Login", uandp, null);
                    if(returnval != null)
                    {
                        if(returnval.UserStatus == "Active" && returnval.UserRole == "Admin")
                        {
                            // regdev
                            var devName = deviceService.GetDeviceInfo("Name");
                            var devModel = deviceService.GetDeviceInfo("Model");
                            var devSerial = deviceService.GetDeviceInfo("Serial");
                            string[] stringarray = { devSerial,devName,devModel };
                            var ret = await serverDbDeviceService.ReturnInt("DeviceCount", stringarray, null);
                            if(ret == 0)
                            {
                                await serverDbDeviceService.InsertData("RegisterDevice", stringarray, null);
                                await notifService.StaticToastNotif("Success", "Device registered.");
                            }
                            else
                            {
                                await notifService.StaticToastNotif("Error", "Device already been registered.");
                            }
                        }
                        else
                        {
                            await notifService.StaticToastNotif("Error", "Credentials are incorrect.");
                        }
                    }
                    else
                    {
                        await notifService.StaticToastNotif("Error", "User doesnt exist.");
                    }
                }
                catch (Exception)
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
            }
            else
            {
                await notifService.StaticToastNotif("Error", "Missing entry.");
            }
            await con.CloseAll();

        }
        public async Task ClearTransaction()
        {
            var con = new LoadingPopupVM();
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("Clear"));
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                await Task.Delay(3000);

                try
                {
                    string[] uandp = { Username, Password };
                    var returnval = await serverDbUserService.ReturnModel("Login", uandp, null);
                    if (returnval != null)
                    {
                        if (returnval.UserStatus == "Active" && returnval.UserRole == "Admin")
                        {
                            // clear transaction
                            await mainService.ClearTransactionData();
                            await notifService.StaticToastNotif("Success", "Transaction cleared");
                        }
                        else
                        {
                            await notifService.StaticToastNotif("Error", "Credentials are incorrect.");
                        }
                    }
                    else
                    {
                        await notifService.StaticToastNotif("Error", "User doesnt exist.");
                    }
                }
                catch (Exception)
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
            }
            else
            {
                await notifService.StaticToastNotif("Error", "Missing entry.");
            }
            await con.CloseAll();
        }
        public async Task ClearAll()
        {

            var con = new LoadingPopupVM();
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("Clear"));
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                await Task.Delay(3000);

                try
                {
                    string[] uandp = { Username, Password };
                    var returnval = await serverDbUserService.ReturnModel("Login", uandp, null);
                    if (returnval != null)
                    {
                        if (returnval.UserStatus == "Active" && returnval.UserRole == "Admin")
                        {
                            // clear transaction
                            await mainService.ClearTransactionData();
                            await notifService.StaticToastNotif("Success", "Transaction cleared");
                        }
                        else
                        {
                            await notifService.StaticToastNotif("Error", "Credentials are incorrect.");
                        }
                    }
                    else
                    {
                        await notifService.StaticToastNotif("Error", "User doesnt exist.");
                    }
                }
                catch (Exception)
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
            }
            else
            {
                await notifService.StaticToastNotif("Error", "Missing entry.");
            }
            await con.CloseAll();
        }
        public async Task EnableSearch()
        {
            var con = new LoadingPopupVM();
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage(string.Empty));
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                await Task.Delay(3000);

                try
                {
                    var returnval = await serverDbUserService.ReturnModel("Login", new string[] {Username, Password}, null);
                    if (returnval != null)
                    {
                        if (returnval.UserStatus == "Active" && returnval.UserRole == "Admin")
                        {
                            MessagingCenter.Send(this, "SearchEnable", string.Empty);
                        }
                        else
                        {
                            await notifService.StaticToastNotif("Error", "Credentials are incorrect.");
                        }
                    }
                    else
                    {
                        await notifService.StaticToastNotif("Error", "User doesnt exist.");
                    }
                }
                catch (Exception)
                {
                    await notifService.StaticToastNotif("Error", "Cannot connect to server.");
                }
            }
            else
            {
                await notifService.StaticToastNotif("Error", "Missing entry.");
            }
            await con.CloseAll();
        }
    }
}
