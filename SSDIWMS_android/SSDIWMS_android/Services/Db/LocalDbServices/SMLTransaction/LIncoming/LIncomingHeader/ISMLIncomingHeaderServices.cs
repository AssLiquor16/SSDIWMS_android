using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader
{
    public interface ISMLIncomingHeaderServices
    {
        Task<IEnumerable<IncomingHeaderModel>> GetList(string type = null, string[] stringfilter = null, int[] intfilter = null, DateTime[] datefilter = null, IncomingHeaderModel obj = null);
        Task<IncomingHeaderModel> GetModel(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter);
        Task Insert(string type, IncomingHeaderModel data);
        Task Update(string type, IncomingHeaderModel data);
        Task Clear();
    }
}
