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
        Task<IEnumerable<IncomingPartialDetailModel>> NewGetList(IncomingPartialDetailModel obj = null, string type = null);
        Task<IncomingPartialDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter);
        Task Insert(string type, IncomingPartialDetailModel item);
        Task<IncomingPartialDetailModel> NewInsert(IncomingPartialDetailModel obj, string type = null);
        Task<IncomingPartialDetailModel> SpecialCaseInsert(string type, IncomingPartialDetailModel item);
        Task Update(string type , IncomingPartialDetailModel item);
    }
}
