using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models;
using SSDIWMS_android.Services.Db.LocalDbServices.Master.UserMaster;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateAllUserMaster
{
    public class UserMasterUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percentageCalculator = new PercentageCalculator();
        IMainServices mainServices;
        IToastNotifService notifService;
        ILocalUserServices localDbUserService;
        IServerUserServices serverDbUserService;

        string _staticloadingText, _loadingText, _taskType, _errorText;
        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }
        public ObservableRangeCollection<UsermasterModel> UserMasterList { get; set; }
        public AsyncCommand PageRefreshCommand { get; }
        public UserMasterUpdaterPopupVM()
        {
            mainServices = DependencyService.Get<IMainServices>();
            notifService = DependencyService.Get<IToastNotifService>();
            localDbUserService = DependencyService.Get<ILocalUserServices>();
            serverDbUserService = DependencyService.Get<IServerUserServices>();
            UserMasterList = new ObservableRangeCollection<UsermasterModel>();
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdateUserMaster();
        }

        private async Task UpdateUserMaster()
        {
            try
            {
                UserMasterList.Clear();
                var userMasters = await serverDbUserService.ReturnList("All", null, null);
                UserMasterList.AddRange(userMasters);
                decimal totcount = UserMasterList.Count;
                decimal foreachcount = 0;
                foreach (var userMaster in userMasters)
                {
                    var user = await localDbUserService.GetFirstOrDefault(userMaster);
                    if (user == null)
                    {
                        await localDbUserService.Insert(userMaster);
                    }
                    else
                    {
                        await localDbUserService.Update(userMaster);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percentageCalculator.GetPercentage("Users", decArray);
                    await Task.Delay(50);
                }
                

                await notifService.StaticToastNotif("Success", "User master downloaded succesfully.");
            }
            catch
            {
                await notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAllAsync(true);
        }

    }
}
