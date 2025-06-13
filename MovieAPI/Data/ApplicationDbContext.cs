namespace MoviesAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed AspNetRoles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "b4a2bf0a-074f-4b60-a950-c1d3e10e028e",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "7b9cf762-f7d3-400f-9b24-b62f0bfda476"
                },
                new IdentityRole
                {
                    Id = "ff1166a6-6a4a-416e-8300-a70e3bfe2709",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "bdc4ff4d-97bf-40d3-a318-e024a4b1d107"
                }
            );

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Id)
                      .ValueGeneratedOnAdd();
                entity.HasData(
                    new Genre { Id = 1, Name = "Action" },
                    new Genre { Id = 2, Name = "Comedy" },
                    new Genre { Id = 3, Name = "Drama" },
                    new Genre { Id = 4, Name = "Horror" },
                    new Genre { Id = 5, Name = "Sci-Fi" }
                );
            });

        }
    }
}
