using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Online_training.Server.Models
{
    public class ApplicationDbContext:IdentityDbContext<User>
    {
        
        public DbSet<TrainerRequest> TrainerRequests { get; set; }

        public DbSet<Participant> Participants { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Participant>().ToTable("Participants");


        }

    }
}
