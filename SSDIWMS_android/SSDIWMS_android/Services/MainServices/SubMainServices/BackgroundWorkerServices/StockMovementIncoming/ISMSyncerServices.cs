using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.Services.MainServices.SubMainServices.BackgroundWorkerServices.StockMovementIncoming
{
    public class ISMSyncerServices : IISMSyncerServices
    {
        IUpdateAllIncomingtransaction updateAllIncomingtransaction;
        public ISMSyncerServices()
        {
            updateAllIncomingtransaction = DependencyService.Get<IUpdateAllIncomingtransaction>();
        }
        public async Task ISMLSync()
        {
            await Task.Delay(50);
            Random min = new Random();
            int rmin = min.Next(100, 130);

            Device.StartTimer(TimeSpan.FromSeconds(rmin), () => {
                Task.Run(async () =>
                {
                    var busy = Preferences.Get("PrefISMSyncing", false);
                    if(busy == false)
                    {
                        try
                        {
                            await Task.Delay(10);
                            await updateAllIncomingtransaction.UpdateAllIncomingTrans();
                        }
                        catch
                        {

                        }
                        
                    }
                   else
                    {

                    }

                });
                return true; //use this to run continuously // false if you want to stop 

            });
        }
    }
}
