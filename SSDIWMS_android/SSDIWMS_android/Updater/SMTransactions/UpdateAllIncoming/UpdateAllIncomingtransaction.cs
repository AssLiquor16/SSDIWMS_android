using MvvmHelpers;
using SSDIWMS_android.Models.SMTransactionModel.Incoming;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingDetail;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingHeader;
using SSDIWMS_android.Services.Db.LocalDbServices.SMLTransaction.LIncoming.LIncomingPartialDetail;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingDetail;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingHeader;
using SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.SIncoming.SIncomingPartialDetail;
using SSDIWMS_android.Updater.SMTransactions.UpdateAllIncoming;
using System;
using System.Collections.Generic;
using System.Linq;
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

        ISMLIncomingPartialDetailServices localDbIncomingParDetailService;
        ISMSIncomingPartialDetailServices serverDbIncomingParDetailService;

        public ObservableRangeCollection<IncomingHeaderModel> IncomingHeaderList { get; set; }

        public UpdateAllIncomingtransaction()
        {
            localDbIncomingHeaderService = DependencyService.Get<ISMLIncomingHeaderServices>();
            serverDbIncomingHeaderService = DependencyService.Get<ISMSIncomingHeaderServices>();

            localDbIncomingDetailService = DependencyService.Get<ISMLIncomingDetailServices>();
            serverDbIncomingDetailService = DependencyService.Get<ISMSIncomingDetailServices>();

            localDbIncomingParDetailService = DependencyService.Get<ISMLIncomingPartialDetailServices>();
            serverDbIncomingParDetailService = DependencyService.Get<ISMSIncomingPartialDetailServices>();

            IncomingHeaderList = new ObservableRangeCollection<IncomingHeaderModel>();
        }


        // you can make another sync methods and put the method in this main method.
        public async Task UpdateAllIncomingTrans()
        {
            IncomingHeaderList.Clear();
            var WhId = Preferences.Get("PrefUserWarehouseAssignedId", 0);
            var role = Preferences.Get("PrefUserRole", string.Empty);
            int[] WhIdFilter = { WhId };
            /*var OngoingMethod = "GetOngoing";
            var FinalizedMethod = "GetFinalize";
            var sIncomingOngoingHeader = await serverDbIncomingHeaderService.GetList(OngoingMethod, null, WhIdFilter, null);
            var sIncomingFinalizeHeader = await serverDbIncomingHeaderService.GetList(FinalizedMethod, null, WhIdFilter, null);
            IncomingHeaderList.AddRange(sIncomingOngoingHeader);
            IncomingHeaderList.AddRange(sIncomingFinalizeHeader);*/
            var sIncomingFinalizeHeader = await serverDbIncomingHeaderService.GetList("GetActDate", null, WhIdFilter, null);
            IncomingHeaderList.AddRange(sIncomingFinalizeHeader);
            foreach (var sHeader in IncomingHeaderList)
            {
                int[] x = { sHeader.INCId };
                string[] y = { sHeader.PONumber };
                var lHeader = await localDbIncomingHeaderService.GetModel("INCId&PO", y, x, null);
                string[] sfilter = { sHeader.PONumber };
                await UpdateDetail(sfilter);
                if (lHeader == null)
                {
                    // insert to local
                    await localDbIncomingHeaderService.Insert("Common", sHeader);

                }
                else
                {
                    if (lHeader.TimesUpdated > sHeader.TimesUpdated)
                    {
                        //update server
                        await serverDbIncomingHeaderService.Update("Common", lHeader);
                    }
                    else if (lHeader.TimesUpdated < sHeader.TimesUpdated)
                    {
                        // update local
                        await localDbIncomingHeaderService.Update("Common", sHeader);
                    }
                }

            }
        }
        public async Task UpdateDetail(string[] poFilter)
        {

            var sDetails = await serverDbIncomingDetailService.GetList("PONumber", poFilter, null);
            foreach(var sDetail in sDetails)
            {
                int[] vs = { sDetail.INCDetId};
                var lDetail = await localDbIncomingDetailService.GetModel("INCDetId", null, vs);
                string[] PoIcFilter = { sDetail.POHeaderNumber, sDetail.ItemCode };
                await UpdatePartialDetail(PoIcFilter);
                if (lDetail == null)
                {
                    await localDbIncomingDetailService.Insert("Common", sDetail);
                }
                else
                {
                    if(lDetail.TimesUpdated > sDetail.TimesUpdated)
                    {
                        //update server
                        await serverDbIncomingDetailService.Update("Common", lDetail);
                    }
                    else if(lDetail.TimesUpdated < sDetail.TimesUpdated)
                    {
                        //update local
                        await localDbIncomingDetailService.Update("Comon", sDetail);
                    }
                }
                
            }

        }
        public async Task UpdatePartialDetail(string[] PoIcFilter)
        {
            var spardetails = await serverDbIncomingParDetailService.GetList("GetItemCodePO",PoIcFilter, null);
            foreach(var spardetail in spardetails)
            {
                string[] b = { spardetail.RefId };
                var lpardetail = await localDbIncomingParDetailService.GetModel("RefId", b, null);
                if(lpardetail == null)
                {
                    await localDbIncomingParDetailService.Insert("Common", spardetail);
                }
                else
                {
                    if(lpardetail.TimesUpdated > spardetail.TimesUpdated)
                    {
                        //update server
                        await serverDbIncomingParDetailService.Update("Common", lpardetail);
                    }
                    else if(lpardetail.TimesUpdated < spardetail.TimesUpdated)
                    {
                        // update local
                        await localDbIncomingParDetailService.Update("RefId", spardetail);
                    }
                }
            }
            var lpardetails = await localDbIncomingParDetailService.GetList("PoIc", PoIcFilter,null);
            foreach(var lpDet in lpardetails)
            {
                var spDet = spardetails.Where(x => x.RefId == lpDet.RefId && x.DateCreated == lpDet.DateCreated).FirstOrDefault();
                if(spDet == null)
                {
                    var role = Preferences.Get("PrefUserRole", "");
                    if(role == "Check")
                    {
                        var retsval = await serverDbIncomingParDetailService.SpecialCaseInsert("ReturnInsertedItem", lpDet);
                        await localDbIncomingParDetailService.Update("RefId", retsval);
                    }
                }
            }
        }
        //pangitaon ang error sa di mu update ang qty sa pikas cp 
        //reject
        public async Task UpdateSpecifiedTrans()
        {
            var WhId = Preferences.Get("PrefUserWarehouseAssignedId", 0);
            var role = Preferences.Get("PrefUserRole", string.Empty);
            int[] WhIdFilter = { WhId };
            var method = "";
            switch (role)
            {
                case "Check":
                    method = "GetOngoing";
                    break;
                case "Pick":
                    method = "GetFinalize";
                    break;
                default: method = ""; break;
            }

            var sIncomingHeader = await serverDbIncomingHeaderService.GetList(method, null, WhIdFilter, null);
            foreach (var sHeader in sIncomingHeader)
            {
                int[] x = { sHeader.INCId };
                string[] y = { sHeader.PONumber };
                var lHeader = await localDbIncomingHeaderService.GetModel("INCId&PO", y, x, null);
                if (lHeader == null)
                {
                    // insert to local
                    await localDbIncomingHeaderService.Insert("Common", sHeader);

                }
                else
                {
                    if (lHeader.TimesUpdated > sHeader.TimesUpdated)
                    {
                        //update server
                        await serverDbIncomingHeaderService.Update("Common", lHeader);
                    }
                    else if (lHeader.TimesUpdated < sHeader.TimesUpdated)
                    {
                        // update local
                        await localDbIncomingHeaderService.Update("Common", sHeader);
                    }
                }
                string[] sfilter = { sHeader.PONumber };
                await UpdateDetail(sfilter);

            }


        }

    }
}
