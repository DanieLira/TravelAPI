
using Microsoft.EntityFrameworkCore;
using TravelAPI.Models;

namespace TravelAPI.DataAccess
{
    public class TravelContext : DbContext
    {
        private readonly IConfiguration Configuration;
        public TravelContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectionString = Configuration.GetConnectionString("TravelReviewsDatabase");
            builder.UseSqlServer(connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
