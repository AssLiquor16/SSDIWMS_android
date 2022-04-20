using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader
{
    public interface ISMSIncomingHeaderServices
    {
        Task<IEnumerable<IncomingHeaderModel>> GetList(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter);
        Task Update(string type, IncomingHeaderModel data);
        Task<string> GetString(string type, string[] stringfilter, int[] intfilter);
    }
}
