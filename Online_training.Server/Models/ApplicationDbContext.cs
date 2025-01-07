using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Online_training.Server.Models
{
    public class ApplicationDbContext:IdentityDbContext<User>
    {
        
        

        public DbSet<Participant> Participants { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<PanierItem> PanierItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ParticipantFormation> ParticipantFormations{ get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Participant>().ToTable("Participants");

            builder.Entity<Formation>()
            .HasOne(f => f.Category)
            .WithMany(c => c.Formations)
            .HasForeignKey(f => f.CategoryId);

            builder.Entity<Section>()
                .HasOne(s => s.Formation)
                .WithMany(f => f.Sections)
                .HasForeignKey(s => s.FormationId);

            builder.Entity<Video>()
                .HasOne(v => v.Section)
                .WithMany(s => s.Videos)
                .HasForeignKey(v => v.SectionId);



        }

    }
}
