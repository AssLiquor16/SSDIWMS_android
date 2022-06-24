using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.MasterListModel;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Updater.MasterDatas.UpdateTransferTypes
{
    public class TransferTypesUpdaterPopupVM : ViewModelBase
    {
        PercentageCalculator percentageCalculator = new PercentageCalculator();
        GlobalDependencyServices dependencies = new GlobalDependencyServices();
        string _staticloadingText, _loadingText, _taskType, _errorText;
        public string TaskType { get => _taskType; set => SetProperty(ref _taskType, value); }
        public string StaticLoadingText { get => _staticloadingText; set => SetProperty(ref _staticloadingText, value); }
        public string LoadingText { get => _loadingText; set => SetProperty(ref _loadingText, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }
        public AsyncCommand PageRefreshCommand { get; set; }
        public TransferTypesUpdaterPopupVM()
        {
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }

        private async Task PageRefresh()
        {
            StaticLoadingText = "Updating...";
            await UpdateTransferTypes();
        }
        private async Task UpdateTransferTypes()
        {
            try
            {
                var TransferType = new List<TransferTypesModel>();
                TransferType.AddRange(await dependencies.serverDbTransferTypesService.GetList());
                decimal totcount = TransferType.Count;
                decimal foreachcount = 0;
                foreach (var item in TransferType)
                {
                    var localTransfer = await dependencies.localDbTransferTypesService.GetFirstOrDefault(new TransferTypesModel { TransferId = item.TransferId });

                    if (localTransfer == null)
                    {
                        await dependencies.localDbTransferTypesService.Insert(item);
                    }
                    else
                    {
                        await dependencies.localDbTransferTypesService.Update(item);
                    }
                    foreachcount++;
                    decimal[] decArray = { foreachcount, totcount };
                    LoadingText = await percentageCalculator.GetPercentage("TransferTypes", decArray);
                    await Task.Delay(50);
                }
                await dependencies.notifService.StaticToastNotif("Success", "Transfer types downloaded succesfully.");
            }
            catch
            {
                await dependencies.notifService.StaticToastNotif("Error", ErrorText);
            }
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
