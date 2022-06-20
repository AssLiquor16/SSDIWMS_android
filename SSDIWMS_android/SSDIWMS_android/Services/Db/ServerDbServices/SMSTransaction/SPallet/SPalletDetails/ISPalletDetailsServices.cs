﻿using SSDIWMS_android.Models.SMTransactionModel.Pallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.SMSTransaction.Pallet.SPalletDetails
{
    public interface ISPalletDetailsServices
    {
        Task<PalletDetailsModel> GetFirstOrDefault(PalletDetailsModel obj, string type = null);
        Task<IEnumerable<PalletDetailsModel>> GetList(PalletDetailsModel obj = null, string type = null);
        Task<PalletDetailsModel> Insert(PalletDetailsModel obj, string type = null);
        Task<PalletDetailsModel> Update(PalletDetailsModel obj, string type = null);
    }
}
