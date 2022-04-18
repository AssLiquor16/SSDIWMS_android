using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingPartialDetail
{
    public interface ISMSIncomingPartialDetailServices
    {
        Task<IEnumerable<IncomingPartialDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter);
        Task<IncomingPartialDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter);
        Task Insert(string type, IncomingPartialDetailModel item);
        Task<IncomingPartialDetailModel> SpecialCaseInsert(string type, IncomingPartialDetailModel item);
        Task Update(string type , IncomingPartialDetailModel item);
    }
}
