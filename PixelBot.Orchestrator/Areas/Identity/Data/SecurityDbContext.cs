using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PixelBot.Orchestrator.Areas.Identity.Data;

namespace PixelBot.Orchestrator.Data
{
    public class SecurityDbContext : IdentityDbContext<BotUser>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }


	public class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
	{

		public const string SqliteConnectionString = "Data Source=identity.db";

		public SecurityDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SecurityDbContext>();
			optionsBuilder.UseSqlite(SqliteConnectionString);

			return new SecurityDbContext(optionsBuilder.Options);
		}
	}
}
