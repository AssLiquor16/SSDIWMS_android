using SSDIWMS_android.Updater.SMTransactions;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MainTransactionSync))]
namespace SSDIWMS_android.Updater.SMTransactions
{
    public class MainTransactionSync : IMainTransactionSync
    {
        IUpdateAllIncomingtransaction updateIncomingServices;

        public MainTransactionSync()
        {
            updateIncomingServices = DependencyService.Get<IUpdateAllIncomingtransaction>();
        }

        public async Task UpdateAllTransactions(string transactionTable)
        {
            switch (transactionTable)
            {
                case "Incoming":
                    await updateIncomingServices.UpdateAllIncomingTrans();
                    break;
                case "Batch":
                    break;
                default: break;
            }
        }
    }
}
