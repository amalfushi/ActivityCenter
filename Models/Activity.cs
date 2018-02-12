using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DojoActivities.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration { get; set; }
        public string DurationType { get; set; }
        public string Description { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public IEnumerable<RSVP> RSVPs  { get; set; }

        public Activity(){
            RSVPs = new List<RSVP>();
        }
    }

        public class ViewActivity
    {
        // private Regex reg;

        [Required(ErrorMessage="Title is Required")]
        [MinLength(2, ErrorMessage="Titles must be at least 2 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage="Time is Required")]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required(ErrorMessage="Date is Required")]
        [DataType(DataType.Date)]
        [FutureDate]
        public DateTime Date { get; set; }

        [Required(ErrorMessage="Duration is Required")]
        public double Duration { get; set;}

        public string DurationType { get; set;}

        [Required]
        [MinLength(10, ErrorMessage="Descriptions must be at least 10 characters")]
        public string Description { get; set; }

        public class FutureDate : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime date = (DateTime)value;
                return date < DateTime.Now ? new ValidationResult("Date must be in future.") : ValidationResult.Success;
            }
        }
    }
}