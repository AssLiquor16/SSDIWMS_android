using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Updater.SMTransactions
{
    public interface IMainTransactionSync
    {
        Task UpdateAllTransactions(string transactionTable);
        Task UpdateSpecificTransactions(string transactionTable);
    }
}
