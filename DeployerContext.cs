using DeployerWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeployerWebService;

public class DeployerContext : DbContext
{
    public DbSet<AppConfig> AppConfigurations { get; set; }

    public DeployerContext(DbContextOptions<DeployerContext> options) : base(options) { }
}