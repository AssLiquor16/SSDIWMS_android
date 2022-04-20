using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail
{
    public interface ISMLIncomingDetailServices
    {
        Task<IEnumerable<IncomingDetailModel>> GetList(string type, string[] stringfilter, int[] intfilter);
        Task<IncomingDetailModel> GetModel(string type, string[] stringfilter, int[] intfilter);
        Task Insert(string type, IncomingDetailModel item);
        Task Update(string type, IncomingDetailModel item);
        Task Clear();
    }
}
