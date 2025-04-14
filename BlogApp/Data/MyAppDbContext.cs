using BlogApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Data
{


    public class MyAppDbContext : IdentityDbContext<IdentityUser>
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Identity tables
            modelBuilder.Entity<IdentityUser>(entity => entity.ToTable("Users"));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));


            modelBuilder.Entity<Category>()
                .HasData(
                    new Category { Id = 1, Name = "Technology" },
                    new Category { Id = 2, Name = "Health" },
                    new Category { Id = 3, Name = "Lifestyle" }
                );
            // Seed 2 Identity Users
            var user1 = new IdentityUser
            {
                Id = "11111111-aaaa-bbbb-cccc-222222222222",
                UserName = "demo@example.com",
                NormalizedUserName = "DEMO@EXAMPLE.COM",
                Email = "demo@example.com",
                NormalizedEmail = "DEMO@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAEMOCKEXAMPLEPASSWORDHASH==",
                SecurityStamp = "seeded-user1-securitystamp",
                ConcurrencyStamp = "seeded-user1-concurrencystamp",

            };

            var user2 = new IdentityUser
            {
                Id = "22222222-aaaa-bbbb-cccc-333333333333",
                UserName = "user2@example.com",
                NormalizedUserName = "USER2@EXAMPLE.COM",
                Email = "user2@example.com",
                NormalizedEmail = "USER2@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAEMOCKEXAMPLEPASSWORDHASH==",
                SecurityStamp = "seeded-user1-securitystamp",
                ConcurrencyStamp = "seeded-user1-concurrencystamp",

            };

            modelBuilder.Entity<IdentityUser>().HasData(user1, user2);

            // Seed Posts
            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 99,
                    Title = "Post by Demo User",
                    Content = "This is a seeded post by demo@example.com.",
                    Author = "demo@example.com",
                    UserId = user1.Id,
                    ImageUrlPath = "https://placehold.co/600x400?text=Post1",
                    CreatedAt = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = 1
                },
                new Post
                {
                    Id = 100,
                    Title = "Post by Second User",
                    Content = "This is a seeded post by user2@example.com.",
                    Author = "user2@example.com",
                    UserId = user2.Id,
                    ImageUrlPath = "https://placehold.co/600x400?text=Post2",
                    CreatedAt = new DateTime(2025, 4, 2, 0, 0, 0, DateTimeKind.Utc),
                    CategoryId = 2
                }
            );

            // Seed Comment
            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = 100,
                    AuthorName = "demo@example.com",
                    Content = "This is a seeded comment from demo user on user2's post.",
                    CreatedAt = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc),
                    PostId = 100,
                    UserId = user1.Id
                }
            );


        }
    }
}