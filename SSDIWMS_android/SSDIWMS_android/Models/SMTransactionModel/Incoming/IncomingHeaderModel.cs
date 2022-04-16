using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.SMTransactionModel.Incoming
{
    public class IncomingHeaderModel
    {
        [PrimaryKey]
        public int INCId { get; set; }
        public DateTime LoadDate { get; set; }
        public string ShipCode { get; set; }
        public string CstmrName { get; set; }
        public DateTime ReqDelDate { get; set; }
        public string ShipNo { get; set; }
        public string ShipLine { get; set; }
        public string CVAN { get; set; }
        public string SalesDoc { get; set; }
        public string PONumber { get; set; }
        public string BillDoc { get; set; }
        public string DelCode { get; set; }
        public string ReferenceDoc { get; set; }
        public string DelStatus { get; set; }
        public DateTime PlanDelSched { get; set; }
        public DateTime ActRecDate { get; set; }
        public DateTime FinalDate { get; set; }
        public DateTime RecDate { get; set; }
        public string INCstatus { get; set; }
        public DateTime DateUploaded { get; set; }
        public int WarehouseId { get; set; }
        public int FinalUserId { get; set; }
        public int RecUserId { get; set; }
        public int TimesUpdated { get; set; }
        public string StatusColor { get; set; }
    }
}
