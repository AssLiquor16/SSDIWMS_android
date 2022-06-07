using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails
{
    public interface ISMLBatchDetailsServices
    {
        Task<IEnumerable<BatchDetailsModel>> GetList(BatchDetailsModel obj = null, string type = null);
        Task<BatchDetailsModel>GetFirstOrDefault(BatchDetailsModel obj = null, string type = null);
        Task<BatchDetailsModel>Insert(BatchDetailsModel obj = null, string type = null);
        Task<BatchDetailsModel>Update(BatchDetailsModel obj = null, string type = null);
    }
}
