using SQLite;
using SSDIWMS_android.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.DefaultModel
{
    public class IPAddressModel : ViewModelBase
    {
        int _ip_Id;
        string _ipAddress;
        bool _is_Used;


        [PrimaryKey]
        [AutoIncrement]
        public int Ip_Id { get => _ip_Id; set => SetProperty(ref _ip_Id, value); }
        public string Ip_Address { get => _ipAddress; set => SetProperty(ref _ipAddress, value); }
        public bool Is_Used { get => _is_Used; set => SetProperty(ref _is_Used, value); }
    }

}
