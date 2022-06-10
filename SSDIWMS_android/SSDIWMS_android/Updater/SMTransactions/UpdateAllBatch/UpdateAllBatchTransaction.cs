using SSDIWMS_android.Models.SMTransactionModel.Incoming.Batch;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchDetails;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LBatch.LBatchHeader;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchDetails;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SBatch.SBatchHeader;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(UpdateAllBatchTransaction))]
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
                    await TimesUpdateUpdateDetails(sbatchHeader);
                    var sbdetailsListFilter = new BatchDetailsModel
                    {
                        BatchCode = sbatchHeader.BatchCode,
                    };
                    var lbatchHeader = await localDbBatchHeaderService.GetFirstOrDefault(sbatchHeader);
                    if (lbatchHeader == null)
                    {
                        await localDbBatchHeaderService.Insert(sbatchHeader);
                    }
                    else
                    {
                        if (lbatchHeader.TimesUpdated > sbatchHeader.TimesUpdated)
                        {
                            // update server
                            await serverDbBatchDetailsService.Update(lbatchHeader);
                        }
                        else if (lbatchHeader.TimesUpdated < sbatchHeader.TimesUpdated)
                        {
                            // update local
                            await localDbBatchHeaderService.Update(sbatchHeader);
                        }
            }
            }
            var lbatchHeaders = await localDbBatchHeaderService.GetList();
            foreach(var lbatchHeader in lbatchHeaders)
            {
                var sbatchhead = sbatchHeaders.Where(x=>x.BatchLocalID == lbatchHeader.BatchLocalID && x.DateCreated == lbatchHeader.DateCreated).FirstOrDefault();
                if(sbatchhead == null)
                {
                    var retval = await serverDbBatchHeaderService.Insert(lbatchHeader, "ReturnInsertedItem");
                    await localDbBatchHeaderService.Update(retval);
                }
            }
            var lbatchHeaders1 = await localDbBatchHeaderService.GetList(bheaderFilter, "ZeroServerId");
            foreach(var sbatchHeader1 in sbatchHeaders)
            {
                var sbatchHead1 = lbatchHeaders1.Where(x=>x.BatchId == sbatchHeader1.BatchId && x.DateCreated == sbatchHeader1.DateCreated).FirstOrDefault();
                if(sbatchHead1 != null)
                {
                    await localDbBatchHeaderService.Update(sbatchHeader1);
                }
            }

            if(sbatchHeaders.Count() == 0)
            {
                await TimesUpdateUpdatedHeader();
            }
        }
        private async Task TimesUpdateUpdateDetails(object o = null)
        {
            var ob = (o as BatchHeaderModel);
            var obj = new BatchDetailsModel
            {
                BatchCode = ob.BatchCode
            };
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
                    var retitm = await serverDbBatchDetailsService.Insert(lbatchDetail, "ReturnInsertedItem");
                    await localDbBatchDetailsService.Update(retitm);
                }
            }
            var lbatchDetails1 = await localDbBatchDetailsService.GetList(obj, "ZeroServerId");
            foreach(var sbatchDetail1 in sbatchDetails)
            {
                var sbatchDet1 = lbatchDetails1.Where(x => x.BatchLocalID == sbatchDetail1.BatchLocalID && x.DateAdded == sbatchDetail1.DateAdded);
                if(sbatchDet1 != null)
                {
                    await localDbBatchDetailsService.Update(sbatchDetail1);
                }
            }
        }
    }
}
