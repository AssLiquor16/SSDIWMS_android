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
                    await DateSyncUpdateHeader();
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
            var lbatchHeaders = await localDbBatchHeaderService.GetList(bheaderFilter, "UserId");

            foreach(var lbatchHeader in lbatchHeaders)
            {
                await TimesUpdateUpdateDetails(lbatchHeader);
                var sbatchHeader = sbatchHeaders.Where(x=>x.BatchLocalID == lbatchHeader.BatchLocalID && x.DateCreated == lbatchHeader.DateCreated).FirstOrDefault();
                if(sbatchHeader == null)
                {
                    var sret = await serverDbBatchHeaderService.Insert(lbatchHeader, "ReturnInsertedItem");
                    await localDbBatchHeaderService.Update(sret);
                }
                else
                {
                    if(lbatchHeader.TimesUpdated > sbatchHeader.TimesUpdated)
                    {
                        // update server
                        await serverDbBatchHeaderService.Update(lbatchHeader);
                    }
                    else if(lbatchHeader.TimesUpdated < sbatchHeader.TimesUpdated)
                    {
                        // update local
                        await localDbBatchHeaderService.Update(sbatchHeader);
                    }
                }
            }
            foreach(var sbatchHeader in sbatchHeaders)
            {
                var lbatchheader = lbatchHeaders.Where(x => x.BatchLocalID == sbatchHeader.BatchLocalID && x.DateCreated == sbatchHeader.DateCreated).FirstOrDefault();
                if(lbatchheader == null)
                {
                    await localDbBatchHeaderService.Insert(sbatchHeader);
                }
            }
        }
        private async Task TimesUpdateUpdateDetails(object o)
        {
            var ob = (o as BatchHeaderModel);
            var obj = new BatchDetailsModel
            {
                BatchCode = ob.BatchCode
            };
            var sbatchDetails = await serverDbBatchDetailsService.GetList(obj, "BatchCode");
            var lbatchDetails = await localDbBatchDetailsService.GetList(obj, "BatchCode");
            foreach(var lbatchDetail in lbatchDetails)
            {
                var sbatchDetail = sbatchDetails.Where(x=>x.BatchLocalID == lbatchDetail.BatchLocalID && x.DateAdded == lbatchDetail.DateAdded).FirstOrDefault();
                if(sbatchDetail == null)
                {
                    var rets = await serverDbBatchDetailsService.Insert(lbatchDetail, "ReturnInsertedItem");
                    await localDbBatchDetailsService.Update(rets);
                }
                else
                {
                    if(lbatchDetail.TimesUpdated > sbatchDetail.TimesUpdated)
                    {
                        // update api
                        await serverDbBatchDetailsService.Update(lbatchDetail);
                    }
                    else if(lbatchDetail.TimesUpdated < sbatchDetail.TimesUpdated)
                    {
                        // update local
                        await localDbBatchDetailsService.Update(sbatchDetail);
                    }
                }
            }
            foreach(var sbatchDetail in sbatchDetails)
            {
                var lbatchDetail = lbatchDetails.Where(x=>x.BatchLocalID == sbatchDetail.BatchLocalID && x.DateAdded == sbatchDetail.DateAdded).FirstOrDefault();
                if(lbatchDetail == null)
                {
                    await localDbBatchDetailsService.Insert(sbatchDetail);
                }
            }
            
        }

        private async Task DateSyncUpdateHeader()
        {
            var bheaderFilter = new BatchHeaderModel
            {
                UserCreated = Preferences.Get("PrefUserId", 0)
            };
            var sbatchHeaders = await serverDbBatchHeaderService.GetList(bheaderFilter, "UserId");
            var lbatchHeaders = await localDbBatchHeaderService.GetList(bheaderFilter, "UserId");

            foreach (var lbatchHeader in lbatchHeaders)
            {
                await DateSyncUpdateDetails(lbatchHeader);
                var sbatchHeader = sbatchHeaders.Where(x => x.BatchLocalID == lbatchHeader.BatchLocalID && x.DateCreated == lbatchHeader.DateCreated).FirstOrDefault();
                if (sbatchHeader == null)
                {
                    var sret = await serverDbBatchHeaderService.Insert(lbatchHeader, "ReturnInsertedItem");
                    await localDbBatchHeaderService.Update(sret);
                }
                else
                {
                    if (lbatchHeader.DateSync > sbatchHeader.DateSync)
                    {
                        // update server
                        await serverDbBatchHeaderService.Update(lbatchHeader);
                    }
                    else if (lbatchHeader.DateSync < sbatchHeader.DateSync)
                    {
                        // update local
                        await localDbBatchHeaderService.Update(sbatchHeader);
                    }
                }
            }
            foreach (var sbatchHeader in sbatchHeaders)
            {
                var lbatchheader = lbatchHeaders.Where(x => x.BatchLocalID == sbatchHeader.BatchLocalID && x.DateCreated == sbatchHeader.DateCreated).FirstOrDefault();
                if (lbatchheader == null)
                {
                    await localDbBatchHeaderService.Insert(sbatchHeader);
                }
            }
        }
        private async Task DateSyncUpdateDetails(object o)
        {
            var ob = (o as BatchHeaderModel);
            var obj = new BatchDetailsModel
            {
                BatchCode = ob.BatchCode
            };
            var sbatchDetails = await serverDbBatchDetailsService.GetList(obj, "BatchCode");
            var lbatchDetails = await localDbBatchDetailsService.GetList(obj, "BatchCode");
            foreach (var lbatchDetail in lbatchDetails)
            {
                var sbatchDetail = sbatchDetails.Where(x => x.BatchLocalID == lbatchDetail.BatchLocalID && x.DateAdded == lbatchDetail.DateAdded).FirstOrDefault();
                if (sbatchDetail == null)
                {
                    var rets = await serverDbBatchDetailsService.Insert(lbatchDetail, "ReturnInsertedItem");
                    await localDbBatchDetailsService.Update(rets);
                }
                else
                {
                    if (lbatchDetail.DateSync > sbatchDetail.DateSync)
                    {
                        // update api
                        await serverDbBatchDetailsService.Update(lbatchDetail);
                    }
                    else if (lbatchDetail.DateSync < sbatchDetail.DateSync)
                    {
                        // update local
                        await localDbBatchDetailsService.Update(sbatchDetail);
                    }
                }
            }
            foreach (var sbatchDetail in sbatchDetails)
            {
                var lbatchDetail = lbatchDetails.Where(x => x.BatchLocalID == sbatchDetail.BatchLocalID && x.DateAdded == sbatchDetail.DateAdded).FirstOrDefault();
                if (lbatchDetail == null)
                {
                    await localDbBatchDetailsService.Insert(sbatchDetail);
                }
            }

        }
    }
}
