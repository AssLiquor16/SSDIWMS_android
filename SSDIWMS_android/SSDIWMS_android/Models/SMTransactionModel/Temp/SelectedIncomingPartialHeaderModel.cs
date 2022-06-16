using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp
{
    public class SelectedIncomingPartialHeaderModel :ViewModelBase
    {
        string _poNumber,_incStatus, _delCode,_shipcode, _billDoc, _batchCode;
        int _incId,_timesUpdated;
        bool _isSelected, _allowGenBCode;
        public int INCId { get => _incId; set => SetProperty(ref _incId, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public int TimesUpdated { get => _timesUpdated; set => SetProperty(ref _timesUpdated, value); }
        public string INCStatus { get => _incStatus; set => SetProperty(ref _incStatus, value); }
        public string Delcode { get => _delCode; set => SetProperty(ref _delCode, value); }
        public string ShipCode { get => _shipcode; set => SetProperty(ref _shipcode, value); }
        public string BillDoc { get => _billDoc; set => SetProperty(ref _billDoc, value); }
        public string BatchCode { get => _batchCode; set => SetProperty(ref _batchCode, value); }
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
        public bool AllowGenBCode { get => _allowGenBCode; set => SetProperty(ref _allowGenBCode, value); }
    }
}
