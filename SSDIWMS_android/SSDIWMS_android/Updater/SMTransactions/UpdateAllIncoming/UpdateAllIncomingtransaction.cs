using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingDetail;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(UpdateAllIncomingtransaction))]
namespace SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming
{
    public class UpdateAllIncomingtransaction : IUpdateAllIncomingtransaction
    {
        ISMLIncomingHeaderServices localDbIncomingHeaderService;
        ISMSIncomingHeaderServices serverDbIncomingHeaderService;

        ISMLIncomingDetailServices localDbIncomingDetailService;
        ISMSIncomingDetailServices serverDbIncomingDetailService;


        public UpdateAllIncomingtransaction()
        {
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            serverDbIncomingHeaderService = DependencyService.Get<ISMSIncomingHeaderServices>();

            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            serverDbIncomingDetailService = DependencyService.Get<ISMSIncomingDetailServices>();
        }

        public async Task UpdateAllIncomingTrans()
        {
            var WhId = Preferences.Get("PrefUserWarehouseAssignedId", 0);
            int[] WhIdFilter = { WhId };
            var sIncomingHeader = await serverDbIncomingHeaderService.GetList("WhId", null, WhIdFilter, null);
            foreach(var sHeader in sIncomingHeader)
            {
                var INCId = sHeader.INCId;
                int[] INCIdFilter = { INCId };
                var lHeader = await localDbIncomingHeaderService.GetModel("INCId", null, INCIdFilter, null);
                if (lHeader == null)
                {
                    await localDbIncomingHeaderService.Insert("Common", sHeader);
                }
                else
                {
                    if (lHeader.TimesUpdated > sHeader.TimesUpdated)
                    {
                        // update server
                        await serverDbIncomingHeaderService.Update("Common", lHeader);
                    }
                    else if (lHeader.TimesUpdated < sHeader.TimesUpdated)
                    {
                        // update local
                        await localDbIncomingHeaderService.Update("Common", sHeader);
                    }
                }

                string[] poFilter = { sHeader.PONumber };
                var sIncomingDetail = await serverDbIncomingDetailService.GetList("PONumber", poFilter, null);
                if(sIncomingDetail != null)
                {
                    foreach(var sDetail in sIncomingDetail)
                    {
                        int[] INCDetIdFilter = { sDetail.INCDetId };
                        var lDetail = await localDbIncomingDetailService.GetModel("INCDetId", null, INCDetIdFilter);
                        if(lDetail == null)
                        {
                            await localDbIncomingDetailService.Insert("Common", sDetail);
                        }
                        else
                        {
                            if(lDetail.TimesUpdated > sDetail.TimesUpdated)
                            {
                                // update server
                                await serverDbIncomingDetailService.Update("Common", lDetail);
                            }
                            else if(lDetail.TimesUpdated < sDetail.TimesUpdated)
                            {
                                // update local
                                await localDbIncomingDetailService.Update("Common", sDetail);
                            }
                        }
                    }
                }
            }
        }

    }
}
