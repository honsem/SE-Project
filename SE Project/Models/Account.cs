using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SE_Project.Models
{
    public class Account
    {

    }

    public class Employee
    {
        [Display(Name = "Employee ID")]
        [Required]
        public string EmployeeID { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Office")]
        [Required]
        public string Office { get; set; }
        [Display(Name = "Department")]
        [Required]
        public string Department { get; set; }
        
    }
    public class LoginViewModel
    {
        [Display(Name ="Email Address")]
        [Required]
        public string Email { get; set; }

        [Display(Name ="Password")]
        [Required]
        public string Password { get; set; }

        public string UserRole { get; set; }

        [Display(Name = "Status (Active/Deactivated)")]
        public bool IsActive { get; set; }
    }
    public class RegistrationViewModel
    {
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Employee ID")]
        [Required]
        public string EmployeeID { get; set; }

        [Display(Name = "User Role")]
        [Required]
        public string UserRole { get; set; }

        [Display(Name = "Status (Active/Deactivated)")]
        public bool IsActive { get; set; }
    }

    public class AccountUpdateViewModel
    {
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Display(Name = "User Role")]
        [Required]
        public string UserRole { get; set; }

        [Display(Name = "Status (Active/Deactivated)")]
        public bool IsActive { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Display(Name = "E-mail Address")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        [Display(Name = "New Password")]
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}