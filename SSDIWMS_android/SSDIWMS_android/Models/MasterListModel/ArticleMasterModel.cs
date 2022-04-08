using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSDIWMS_android.Models.MasterListModel
{
    public class ArticleMasterModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Article_Code { get; set; }
        public string Article_Description { get; set; }
        public string Barcode { get; set; }
        public string Casecode { get; set; }
        public string Primary_Price_CS { get; set; }
        public string Secondary_Price_CS { get; set; }
        public string Article_Short_Description { get; set; }
        public string Unit_Conversion2 { get; set; }
        public string Status { get; set; }
        public string Site { get; set; }
        public string Uom2 { get; set; }
        public string Uom3 { get; set; }
        public string Volume_CS { get; set; }
        public string Volume_CS_L { get; set; }
        public string Volume_PC { get; set; }
        public string Volume_PC_L { get; set; }
        public string Width_CS { get; set; }
        public string Width_PC { get; set; }
        public string Base_Unit_of_Measure { get; set; }
        public string Uom1 { get; set; }
        public string Unit_Conversion1 { get; set; }
        public string Unit_Conversion3 { get; set; }
        public string Merchandise_Category { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Division { get; set; }
        public string Assortment_No { get; set; }
        public string Listing_Module { get; set; }
        public string Assortment_status { get; set; }
        public string Gross_Weight_PC { get; set; }
        public string Net_Weight_PC { get; set; }
        public string Gross_Weight_CS { get; set; }
        public string Net_Weight_CS { get; set; }
        public string Length_PC { get; set; }
        public string Height_PC { get; set; }
        public string Length_CS { get; set; }
        public string Height_CS { get; set; }
        public DateTime Updated_Date { get; set; }


    }
}
