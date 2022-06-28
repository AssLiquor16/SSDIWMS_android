using MvvmHelpers;
using MvvmHelpers.Commands;
using SSDIWMS_android.Helpers;
using SSDIWMS_android.Models.DefaultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.ViewModels.PopUpVMs
{
    public class IPListPopupVM : ViewModelBase
    {
        GlobalDependencyServices dependencies = new GlobalDependencyServices();

        IPAddressModel _selectedItem;
        public IPAddressModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableRangeCollection<IPAddressModel> IPList { get; set; }

        public AsyncCommand<IPAddressModel> DeleteCommand { get; }
        public AsyncCommand AddIpCommand { get;  }
        public AsyncCommand TappedCommand { get;  }
        public AsyncCommand PageRefreshCommand { get; }
        public IPListPopupVM()
        {
            IPList = new ObservableRangeCollection<IPAddressModel>();
            DeleteCommand = new AsyncCommand<IPAddressModel>(Delete);
            AddIpCommand = new AsyncCommand(AddIp);
            TappedCommand = new AsyncCommand(Tapped);
            PageRefreshCommand = new AsyncCommand(PageRefresh);
        }
        private async Task Delete(IPAddressModel obj)
        {
            if(obj != null) 
            {
                if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to delete the selected ip?", "Yes", "No") == true)
                {
                    if (obj.Ip_Id != 1)
                    {
                        if (obj.Is_Used == true)
                        {
                            IPList.Where(x => x.Ip_Id == 1).FirstOrDefault().Is_Used = true;
                            IPList.Where(x => x.Ip_Id != 1).ToList().ForEach(x => { x.Is_Used = false; });
                            foreach (var ip in IPList)
                            {
                                await dependencies.localDbIpServices.Update(ip);
                            }
                        }
                        await dependencies.localDbIpServices.Delete(obj, "Single");
                        await PageRefresh();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Default ip cannot be deleted", "Ok");
                    }
                }
                
            }
        }
        private async Task AddIp()
        {
            var ip = await App.Current.MainPage.DisplayPromptAsync("Alert", "IP Address", "Yes");
            if(!string.IsNullOrWhiteSpace(ip))
            {
                var i = new IPAddressModel
                {
                    Ip_Address = $"http://{ip}/",
                    Is_Used = false,
                };
                await dependencies.localDbIpServices.Insert(i);
                await PageRefresh();
            }
        }
        private async Task Tapped()
        {
            if(await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to use the selected ip?", "Yes", "No") == true)
            {
                if (SelectedItem != null)
                {
                    IPList.Where(x => x.Ip_Id == SelectedItem.Ip_Id).FirstOrDefault().Is_Used = true;
                    foreach (var ip in IPList.Where(x => x.Ip_Id != SelectedItem.Ip_Id))
                    {
                        ip.Is_Used = false;
                    }

                    foreach (var selectedIps in IPList)
                    {
                        await dependencies.localDbIpServices.Update(selectedIps);
                    }
                }
                await App.Current.MainPage.DisplayAlert("Alert", "Ip set succesfully.", "Ok");
                await Task.Delay(100);
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
            await Task.Delay(1);
        }
        private async Task PageRefresh()
        {
            var iplist = await dependencies.localDbIpServices.GetList();
            IPList.ReplaceRange(iplist);
            await Task.Delay(1);
        }
    }
}
