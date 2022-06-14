using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming.Temp
{
    public class DummyIncomingHeaderModel : ViewModelBase
    {
        int _incId;
        string _poNumber,_bollDoc,_cvan,_shipNo,_shipLine, _iNCstatus, _shipCode;
        bool _isConsoLidation, _isSelected;
        public int INCId { get => _incId; set => SetProperty(ref _incId, value); }
        public string PONumber { get => _poNumber; set => SetProperty(ref _poNumber, value); }
        public string BillDoc { get => _bollDoc; set => SetProperty(ref _bollDoc, value); }
        public string CVan { get => _cvan; set => SetProperty(ref _cvan, value); }
        public string ShipNo { get => _shipNo; set => SetProperty(ref _shipNo, value); }
        public string ShipCode { get => _shipCode; set => SetProperty(ref _shipCode, value); }
        public string ShipLine { get => _shipLine; set => SetProperty(ref _shipLine, value); }
        public string INCstatus { get => _iNCstatus; set => SetProperty(ref _iNCstatus, value); }
        public bool IsConsoLidation { get => _isConsoLidation; set => SetProperty(ref _isConsoLidation, value); }
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    }
}
