using SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.StockMovementIncoming;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(ISMSyncerServices))]
namespace SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.StockMovementIncoming
{
    public class ISMSyncerServices : IISMSyncerServices
    {
        readonly IUpdateAllIncomingtransaction updateAllIncomingtransaction;
        readonly IToastNotifService notifyService;
        public ISMSyncerServices()
        {
            updateAllIncomingtransaction = DependencyService.Get<IUpdateAllIncomingtransaction>();
            notifyService = DependencyService.Get<IToastNotifService>();
        }
        public async Task ISMLSyncer()
        {
            await Task.Delay(50);
            var rmin = Preferences.Get("PrefTimerLongRandomVal", 100);

            Device.StartTimer(TimeSpan.FromSeconds(rmin), () => {
                Task.Run(async () =>
                {
                    var prefUserId  = Preferences.Get("PrefUserId",0);
                    var prefUserRole = Preferences.Get("PrefUserRole", string.Empty);
                    var prefWhId = Preferences.Get("PrefUserWarehouseAssignedId",0);

                    if(prefUserId != 0 && !string.IsNullOrWhiteSpace(prefUserRole) && prefWhId != 0)
                    {
                        var busy = Preferences.Get("PrefISMSyncing", false);
                        if (busy == false)
                        {

                            await notifyService.ToastNotif("Pending", "Attempting to sync.");
                            try
                            {
                                await Task.Delay(10);
                                await updateAllIncomingtransaction.UpdateAllIncomingTrans();
                                await notifyService.ToastNotif("Success", "Syncing execute succesfully.");
                            }
                            catch
                            {
                                await notifyService.ToastNotif("Error", "Cannot connect to server.");
                            }

                        }
                        else
                        {
                            await notifyService.ToastNotif("Error", "Attempting failed, syncing busy.");
                        }
                    }
                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }
    }
}
