using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchDetails;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchHeader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android.Updater.SMTransactions.UpdateAllBatch
{
    public class UpdateAllBatchTransaction : IUpdateAllBatchTransaction
    {
        ISMLBatchHeaderServices localDbBatchHeaderService;
        ISMSBatchHeaderServices serverDbBatchHeaderService;

        ISMSBatchDetailServices serverDbBatchDetailsService;
        ISMLBatchDetailsServices localDbBatchDetailsService;
        public UpdateAllBatchTransaction()
        {

            serverDbBatchHeaderService = DependencyService.Get<ISMSBatchHeaderServices>();
            localDbBatchHeaderService = DependencyService.Get<ISMLBatchHeaderServices>();

            serverDbBatchDetailsService = DependencyService.Get<ISMSBatchDetailServices>();
            localDbBatchDetailsService = DependencyService.Get<ISMLBatchDetailsServices>();
        }

        public async Task UpdateAllBatchTrans()
        {
            switch (Setup.transactionSyncRef)
            {
                case "TimesUpdated":
                    await TimesUpdateUpdatedHeader();
                    break;
                case "DateSync":
                    break;
                default: break;
            }
        }
        private async Task TimesUpdateUpdatedHeader()
        {
            var bheaderFilter = new BatchHeaderModel
            {
                UserCreated = Preferences.Get("PrefUserId", 0)
            };
            var sbatchHeaders = await serverDbBatchHeaderService.GetList(bheaderFilter, "UserId");
            foreach (var sbatchHeader in sbatchHeaders)
            {
                var sbdetailsListFilter = new BatchDetailsModel
                {
                    BatchCode = sbatchHeader.BatchCode,
                };
                await TimesUpdateUpdateDetails(sbdetailsListFilter);

            }
        }
        private async Task TimesUpdateUpdateDetails(object ob = null)
        {
            var obj = (ob as BatchDetailsModel);
            var sbatchDetails = await serverDbBatchDetailsService.GetList(obj, "BatchCode");
            foreach (var sbatchDetail in sbatchDetails)
            {
                var sbdetailsDataFilter = new BatchDetailsModel
                {
                    BatchDetId = sbatchDetail.BatchDetId,
                    DateAdded = sbatchDetail.DateAdded,
                };
                var lbatchDetail = await localDbBatchDetailsService.GetFirstOrDefault(sbdetailsDataFilter, "BatchDetId/DateCreated");
                if (lbatchDetail == null)
                {
                    await localDbBatchDetailsService.Insert(sbatchDetail);
                }
                else
                {
                    if(lbatchDetail.TimesUpdated > sbatchDetail.TimesUpdated)
                    {
                        //update server
                        await serverDbBatchDetailsService.Update(lbatchDetail);
                    }
                    else if(lbatchDetail.TimesUpdated < sbatchDetail.TimesUpdated)
                    {
                        // update local
                        await localDbBatchDetailsService.Update(sbatchDetail);
                    }
                }
            }

            var lbatchDetails = await localDbBatchDetailsService.GetList();
            foreach(var lbatchDetail in lbatchDetails)
            {
                var sBatchDet = sbatchDetails.Where(x => x.BatchDetId == lbatchDetail.BatchDetId && x.DateAdded == lbatchDetail.DateAdded).FirstOrDefault();
                if(sBatchDet == null)
                {
                    await serverDbBatchDetailsService.Insert(lbatchDetail);
                }
            }
            var lbatchDetails1 = await localDbBatchDetailsService.GetList(obj, "ZeroServerId"); // to be continue;
        }
    }
}
