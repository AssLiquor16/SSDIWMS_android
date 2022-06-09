using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchDetails
{
    public interface ISMSBatchDetailServices
    {
        Task<IEnumerable<BatchDetailsModel>> GetList(object obj = null, string type = null);
        Task<BatchDetailsModel> GetFirstOrDefault(object obj = null, string type = null);
        Task<BatchDetailsModel> Insert(object obj = null, string type = null);
        Task<BatchDetailsModel> Update(object obj = null, string type = null);
    }
}
