    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SE_Project.Models
{
    public class Assets
    {
        public string SelectedDropDownItem { get; set; }
    }
    public class HardwareAssets
    {
        [Display(Name = "Serial No.")]
        [Required]
        public string SerialNo { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "MAC Address")]
        public string MacAddress { get; set; }
        [Display(Name = "Computer Name")]
        public string ComputerName { get; set; }
        [Display(Name = "Computer Model")]
        public string ComputerModel { get; set; }
        [Display(Name = "Computer OS")]
        public string ComputerOS { get; set; }
        
        [Display(Name = "Equipment Type")]
        [Required]
        public string EquipmentType { get; set; }
    }
    public class Hardware_Ownership
    { 
        [Display(Name = "Serial No.")]
        [Required]
        public string SerialNo { get; set; }

        [Display(Name = "Employee ID")]
        [Required]
        public string EmployeeID { get; set; }
        [Display(Name = "Computer Status")]
        [Required]
        public string ComputerStatus { get; set; }
        [Display(Name = "Condition")]
        [Required]
        public string Condition { get; set; }

    }

    public class Edit_Hardware_Ownership_View
    {

        [Display(Name = "Serial No.")]
        [Required]
        public string SerialNo { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "MAC Address")]
        public string MacAddress { get; set; }
        [Display(Name = "Computer Name")]
        public string ComputerName { get; set; }
        [Display(Name = "Computer Model")]
        public string ComputerModel { get; set; }
        [Display(Name = "Computer OS")]
        public string ComputerOS { get; set; }

        [Display(Name = "Equipment Type")]
        [Required]
        public string EquipmentType { get; set; }

        [Display(Name = "Employee ID")]
        [Required]
        public string EmployeeID { get; set; }
        [Display(Name = "Computer Status")]
        [Required]
        public string ComputerStatus { get; set; }
        [Display(Name = "Condition")]
        [Required]
        public string Condition { get; set; }
    }
    public class HardwareViewList
    {
        public string IpAddress { get; set; }
        public string ComputerName { get;set; }
        public string EquipmentType { get; set; }   
        public string FullName { get; set; }    
        public string Office { get; set; }
        public string Department { get; set; }
        public string Condition { get; set; }
        public string ComputerStatus { get; set; }
        public string EmployeeID { get; set; }
    }
    public class SoftwareAssets
    {
        [Display(Name = "Serial No.")]
        [Required]
        public string SerialNo { get; set; }
        [Display(Name = "Software Name")]
        [Required]
        public string SoftwareName { get; set; }
        [Display(Name = "Purchase Date")]
        [Required]
        public DateTime PurchaseDate{ get; set; }
        [Display(Name = "Expiry Date")]
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Display(Name = "Cost")]
        [Required]
        public double Cost { get; set; }
        [Display(Name = "Vendor")]
        [Required]
        public string Vendor { get; set; }
        [Display(Name = "Employee ID")]
        [Required]
        public string EmployeeID { get; set; }
    }

    public class SoftwareViewList
    {
        [Display(Name = "Serial No.")]
        [Required]
        public string SerialNo { get; set; }
        [Display(Name = "Software Name")]
        [Required]
        public string SoftwareName { get; set; }
        [Display(Name = "Purchase Date")]
        [Required]
        public string PurchaseDate { get; set; }
        [Display(Name = "Expiry Date")]
        [Required]
        public string ExpiryDate { get; set; }
        [Display(Name = "Cost")]
        [Required]
        public double Cost { get; set; }
        [Display(Name = "Vendor")]
        [Required]
        public string Vendor { get; set; }
        
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }
        [Display(Name = "Employee ID")]
        [Required]
        public string EmployeeID { get; set; }
    }

    public class HardwareStatusReportView
    {
        public string computerStatus;
        public int hardwareTotalQty;
    }
    public class EquipmentTypeReportView
    {
        public string equipmentType;
        public int equipmentTotalQty;
    }
}