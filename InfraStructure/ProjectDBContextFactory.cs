using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lumen.InfraStructure
{
    public class ProjectDBContextFactory : IDesignTimeDbContextFactory<ProjectDBContext>
    {
        public ProjectDBContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json in the current project dir
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("Lumen");

            var optionsBuilder = new DbContextOptionsBuilder<ProjectDBContext>();
            optionsBuilder.UseSqlServer(connectionString, sql =>
            {
                sql.CommandTimeout(120);
                sql.MigrationsAssembly(typeof(ProjectDBContext).Assembly.FullName);
            });

            // Note: the design-time factory can't get ICurrentUserService.
            // The ProjectDBContext constructor must accept a null or default
            // for it, OR you need a second constructor with just options.
            return new ProjectDBContext(optionsBuilder.Options, null!);
        }
    }
}
