using System.Collections.Immutable;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options){}
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // this for building the key for the junction table
            base.OnModelCreating(builder);
            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId,k.LikedUserId});

            // this is for the SourceUser side
            builder.Entity<UserLike>()
                .HasOne(s =>s.SourceUser )
                .WithMany(l => l.LikedUsers )
                .HasForeignKey(s =>s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // this is for the LikedUser side
            builder.Entity<UserLike>()
                .HasOne(s =>s.LikedUser )
                .WithMany(l => l.LikedByUsers )
                .HasForeignKey(s =>s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
