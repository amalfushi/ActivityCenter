using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DojoActivities.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public IEnumerable<RSVP> Attending { get; set; }

        public User(){
            Attending = new List<RSVP>();
        }
        // public double Wallet { get; set; }
    }

        public class LoginUser
    {
        [Required(ErrorMessage="Email is required")]
        // [RegularExpression()]
        [Display(Name= "Email")]
        public string LogEmail { get; set; }

        [Required(ErrorMessage="Password is required")]
        [Display(Name = "Password")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string LogPassword { get; set; }
    }

        public class RegisterUser
    {
        // private Regex reg;

        [Required(ErrorMessage="First Name is required")]
        [Display(Name = "First Name")]
        [MinLength(2, ErrorMessage="Names must be at least 2 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last Name is required")]
        [Display(Name = "Last Name")]
        [MinLength(2, ErrorMessage="Names must be at least 2 characters")]
        
        public string LastName { get; set; }

        [Required(ErrorMessage="Email is required")]
        [MinLength(3, ErrorMessage="Emails must be between 3 and 20 characters")]
        [MaxLength(20, ErrorMessage="Emails must be between 3 and 20 characters")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage ="Passwords must be at least 8 characters")]
        [DataType(DataType.Password)]
        // [RegularExpression(@"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?\d)(?=.*?[!@#$%\^&*\(\)\-_+=;:'""\/\[\]{},.<>|`])", ErrorMessage="Passwords must contain at least 1 Capitol Letter, 1 Number, and 1 Special Character")]
        public string Password { get; set; }

        [Required(ErrorMessage="Password Confirmation is required")]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage="Passwords do not match")]
        [DataType(DataType.Password)]
        public string C_Password { get; set; }
    }
}