using Microsoft.EntityFrameworkCore;

namespace deneme123.Models

{
    public class MyDbContext : DbContext
    {


        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {


        }

        public DbSet<Dead> Deads { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<PasswordCode> PasswordCode { get; set; }

        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }





    }
}
