using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingDetail
{
    public interface ISMSIncomingDetailServices
    {
        Task<IncomingDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter);
        Task<IEnumerable<IncomingDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter);
        Task Update(string type, IncomingDetailModel data);
    }
}
