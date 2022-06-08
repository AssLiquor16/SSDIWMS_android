using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models
{
    public class UsermasterModel
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public int UserDeptId { get; set; }
        public int WarehouseAssignedId { get; set; }
        public string UserStatus { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string LoginStatus { get; set; }
        public string Salesmancode { get; set; }
    }
}
