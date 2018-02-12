using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Controllers
{
    public class DashboardController : Controller
    {
        private ActivityCenterContext _context;

        public DashboardController(ActivityCenterContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }
            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();
            ViewBag.LoggedUser = user;

            List<Activity> activities = _context.activities.Where(a => a.DateTime > DateTime.Now).Include(a => a.RSVPs).Include(a => a.Creator).OrderBy(a => a.DateTime).ToList();


            // grab a list of all the rsvps for the user and put the corresponding Activty id's in a list
            List<RSVP> userAttending = _context.rsvps.Where(a => a.AttendeeId == user.UserId).Where(a => a.Activity.EndTime > DateTime.Now).Include(r => r.Activity).ToList();

            List<int> attendingIds = new List<int>();
            foreach (RSVP r in userAttending)
            {
                attendingIds.Add(r.ActivityId);
            }
            ViewBag.UserAttending = attendingIds;

            //create a list of events with conflicting times
            List<int> cannotAttend = new List<int>();
            foreach (Activity act in activities)
            {
                if (!attendingIds.Contains(act.ActivityId))
                {
                    bool canAttend = true;
                    foreach (RSVP attending in userAttending)
                    {
                        if (!canAttend) break;

                        if (attending.Activity.DateTime <= act.EndTime && act.DateTime <= attending.Activity.EndTime)
                        {
                            canAttend = false;
                        }
                    }

                    if (canAttend == false)
                    {
                        cannotAttend.Add(act.ActivityId);
                    }
                }
            }
            ViewBag.CannotAttend = cannotAttend;

            return View(activities);
        }

//*****************************************Activity Create
        [HttpGet]
        [Route("activity/new")]
        public IActionResult NewActivity()
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }

            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();
            ViewBag.LoggedUser = user;
            return View();
        }

        [HttpPost]
        [Route("CreateActivity")]
        public IActionResult CreateActivity(ViewActivity na)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }

            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();
            ViewBag.LoggedUser = user;

            if (ModelState.IsValid)
            {
                Activity addActivity = new Activity { Title = na.Title, Description = na.Description, Duration = na.Duration, DurationType = na.DurationType, CreatorId = (int)LogId };
                addActivity.DateTime = na.Date.Add(na.Time.TimeOfDay);
                addActivity.EndTime = addActivity.DateTime;

                switch (addActivity.DurationType)
                {
                    case "Minutes":
                        addActivity.EndTime = addActivity.EndTime.AddMinutes(addActivity.Duration);
                        break;
                    case "Hours":
                        addActivity.EndTime = addActivity.EndTime.AddHours(addActivity.Duration);
                        break;
                    case "Days":
                        addActivity.EndTime = addActivity.EndTime.AddDays(addActivity.Duration);
                        break;
                    default:
                        break;
                }
                _context.activities.Add(addActivity);

                //make a new rsvp for the event creator & add to db
                RSVP creatorRsvp = new RSVP { Attendee = user, Activity = addActivity };
                _context.rsvps.Add(creatorRsvp);
                _context.SaveChanges();

                return Redirect("/dashboard");
            }
            return View("NewActivity");
        }

//**************************Activity Delete */
        [HttpGet]
        [Route("/activity/{ActivityId}/delete")]
        public IActionResult DeleteActivity(int ActivityId)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }

            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();

            Activity da = _context.activities.Where(a => a.ActivityId == ActivityId).SingleOrDefault();

            //second check for sneaky deletes
            if (user.UserId != da.CreatorId)
            {
                return Redirect("/");
            }

            _context.activities.Remove(da);
            _context.SaveChanges();

            return Redirect("/dashboard");
        }

//*********************************Activity Show */
        [HttpGet]
        [Route("activity/{ActivityId}/show")]
        public IActionResult ShowActivity(int Activityid)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }

            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();
            ViewBag.LoggedUser = user;

            // grab a list of all the rsvps for the user and put the corresponding Activty id's in a list
            List<RSVP> userAttending = _context.rsvps.Where(a => a.AttendeeId == user.UserId).Include(r => r.Activity).ToList();

            List<int> attendingIds = new List<int>();
            foreach (RSVP r in userAttending)
            {
                attendingIds.Add(r.ActivityId);
            }

            ViewBag.UserAttending = attendingIds;


            List<int> cannotAttend = new List<int>();

            Activity sa = _context.activities.Include(a => a.Creator).Include(a => a.RSVPs).ThenInclude(r => r.Attendee).Where(a => a.ActivityId == Activityid).SingleOrDefault();

            if (!attendingIds.Contains(sa.ActivityId))
            {
                bool canAttend = true;
                foreach (RSVP attending in userAttending)
                {
                    if (!canAttend) break;

                    if (attending.Activity.DateTime <= sa.EndTime && sa.DateTime <= attending.Activity.EndTime)
                    {
                        canAttend = false;
                    }
                }

                if (canAttend == false)
                {
                    cannotAttend.Add(sa.ActivityId);
                }
            }
            ViewBag.CannotAttend = cannotAttend;

            return View(sa);
        }

//*************************************RSVP */
    // should add a check for overlapping times.
        [HttpGet]
        [Route("activity/{ActivityId}/join")]
        public IActionResult Join(int ActivityId)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }

            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();
            ViewBag.LoggedUser = user;

            Activity cur = _context.activities.Where(a => a.ActivityId == ActivityId).SingleOrDefault();

            RSVP nRsvp = new RSVP { Attendee = user, Activity = cur };

            _context.rsvps.Add(nRsvp);
            _context.SaveChanges();

            return Redirect("/dashboard");
        }

//************************************Un-RSVP */
        [HttpGet]
        [Route("activity/{ActivityId}/leave")]
        public IActionResult Leave(int ActivityId)
        {
            int? LogId = HttpContext.Session.GetInt32("UserId");
            if (LogId == null)
            {
                return Redirect("/");
            }

            User user = _context.users.Where(u => u.UserId == LogId).SingleOrDefault();

            RSVP delRSVP = _context.rsvps.Where(r => r.ActivityId == ActivityId && r.AttendeeId == LogId).SingleOrDefault();

            //second check for sneaky deletes, probably unnecessary
            if (user.UserId != delRSVP.AttendeeId)
            {
                return Redirect("/");
            }

            _context.rsvps.Remove(delRSVP);
            _context.SaveChanges();

            return Redirect("/dashboard");
        }

    }
}
