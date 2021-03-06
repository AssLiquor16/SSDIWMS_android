using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail
{
    public interface ISMLIncomingPartialDetailServices
    {
        Task<IEnumerable<IncomingPartialDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter);
        Task<IncomingPartialDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter, DateTime[] datefilter);
        Task Insert(string type, IncomingPartialDetailModel item);
        Task<IncomingPartialDetailModel> NewInsert(IncomingPartialDetailModel obj, string type = null);
        Task Update(string type, IncomingPartialDetailModel item);
        Task Clear();
    }
}
