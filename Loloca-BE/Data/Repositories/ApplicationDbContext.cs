using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Diagnostics.Contracts;

namespace Loloca_BE.Data.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
