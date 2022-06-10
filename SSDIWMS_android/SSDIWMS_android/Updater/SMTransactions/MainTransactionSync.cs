using SSDIWMS_android.Updater.SMTransactions;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllBatch;
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
        /// <summary>
        /// Transactions : transaction usually are the datas that pass from local(Device) to server(API) vice versa. 
        /// -IncomingHeaders
        /// -IncomingDetails
        /// -IncomingPartialDetails
        /// -BatchHeader
        /// -BatchDetails
        /// </summary>
        IUpdateAllBatchTransaction updateBatchServices;
        IUpdateAllIncomingtransaction updateIncomingServices;
        public MainTransactionSync()
        {
            updateBatchServices = DependencyService.Get<IUpdateAllBatchTransaction>();
            updateIncomingServices = DependencyService.Get<IUpdateAllIncomingtransaction>();
        }

        /// <summary>
        /// this.params:
        /// 
        /// Incoming == Updates AllIncoming transactions
        /// Batch == Updates AllBatch transactions
        /// 
        /// </summary>
        /// <param name="transactionTable"></param>
        /// <returns></returns>
        #region Update All Transactions
        public async Task UpdateAllTransactions(string transactionTable)
        {
            switch (transactionTable)
            {
                case "Incoming":
                    await updateIncomingServices.UpdateAllIncomingTrans();
                    break;
                case "Batch":
                    await updateBatchServices.UpdateAllBatchTrans();
                    break;
                default: break;
            }
        }
        #endregion


        /// <summary>
        /// this.params: (you can add annother param here and set the method in called service (usually in Updater > SmTransactions > UpdateAllBatch.dir || Updater > SmTransactions > UpdateAllIncoming.dir  ))
        /// 
        /// IncomingHeader = updates incomingHeaders
        /// 
        /// </summary>
        /// <param name="transactionTable"></param>
        /// <returns></returns>
        #region UpdateSpecific Transactions
        public async Task UpdateSpecificTransactions(string transactionTable)
        {
            switch (transactionTable)
            {
                case "IncomingHeader":
                    await updateIncomingServices.UpdateIncomingHeaderTrans();
                    break;
                default: break;
            }
        }
        #endregion
    }
}
