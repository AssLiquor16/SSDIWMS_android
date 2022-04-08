﻿using SSDIWMS_android.Models.TransactionModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSDIWMS_android.Services.Db.ServerDbServices.PalletMaster
{
    internal interface IServerPalletMasterServices
    {
        Task<IEnumerable<PalletMasterModel>> GetList(string type, string[] stringfilter, int[] intfilter);
    }
}
