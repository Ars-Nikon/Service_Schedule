using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Service_Schedule.Models;

namespace Service_Schedule.Contexts
{
    public class ApplicationContext:IdentityDbContext<User>
    {
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<TimeTableDate> TimeTableDates { get; set; }
        public DbSet<TimeTableTimeVisit> TimeTableTimeVisits { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
