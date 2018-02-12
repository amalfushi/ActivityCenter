using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ActivityCenter.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId { get; set;}
        public int ActivityId { get; set; }
        public Activity Activity { get; set;}
        public int AttendeeId { get; set; }
        public User Attendee { get; set; }
    }
}