using MvvmHelpers;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Services.NotificationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.ViewModels.PopUpVMs
{
    public class LoadingPopupVM : ViewModelBase
    {

        
        IToastNotifService notifService;

        string _staticloadingText,_loadingText, _taskType,_errorText, _iconName;
        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }
        public string IconName { get => _iconName; set => SetProperty(ref _iconName, value); }


        public AsyncCommand RefreshCommand { get; }
        public LoadingPopupVM()
        {
            notifService = DependencyService.Get<IToastNotifService>();

            RefreshCommand = new AsyncCommand(Refresh);
        }
        public async Task Refresh()
        {
            ErrorText = "Cannot connect to server.";
            await Task.Delay(1);
            if (string.IsNullOrWhiteSpace(TaskType))
            {
                StaticLoadingText = "Processing...";
            }
            else if (TaskType == "Login")
            {
                IconName = "cog.gif";
                StaticLoadingText = "Verifying credentials...";
            }
            else if (TaskType == "Logout")
            {
                IconName = "cog.gif";
                StaticLoadingText = "Logging out...";
            }
            else if(TaskType == "Clear")
            {
                IconName = "bin.gif";
                StaticLoadingText = "Deleting...";
            }
            else
            {
                IconName = "cog.gif";
                StaticLoadingText = "Processing...";
            }

        }
        public async Task CloseAll()
        {
            await PopupNavigation.Instance.PopAllAsync(true);
        }
        public async Task Close()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
