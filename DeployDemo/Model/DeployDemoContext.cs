using Microsoft.EntityFrameworkCore;

namespace DeployDemo.Model
{
    public class DeployDemoContext : DbContext
    {
        public DeployDemoContext(DbContextOptions<DeployDemoContext> options) : base(options)
        {
        }
        public DbSet<DeployDTO> DeployDemos { get; set; }
    }


   
}
