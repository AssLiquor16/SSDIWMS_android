using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming
{
    public interface IUpdateAllIncomingtransaction
    {
        Task UpdateAllIncomingTrans();
        Task UpdateIncomingHeaderTrans();
    }
}
