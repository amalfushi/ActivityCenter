using Microsoft.EntityFrameworkCore;

namespace DojoActivities.Models
{
    public class DojoActivitiesContext : DbContext
    {
        public DojoActivitiesContext(DbContextOptions<DojoActivitiesContext> options) : base (options){}

        public DbSet<User> users { get; set; }
        public DbSet<Activity> activities { get; set; }
        public DbSet<RSVP> rsvps { get; set;}
    }
}