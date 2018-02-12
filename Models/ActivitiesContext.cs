using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{
    public class ActivityCenterContext : DbContext
    {
        public ActivityCenterContext(DbContextOptions<ActivityCenterContext> options) : base (options){}

        public DbSet<User> users { get; set; }
        public DbSet<Activity> activities { get; set; }
        public DbSet<RSVP> rsvps { get; set;}
    }
}