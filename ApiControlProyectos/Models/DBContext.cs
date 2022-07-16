using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiControlProyectos.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace ApiControlProyectos.Models
{
    public class DBContext: DbContext
    {
        private readonly AppSettings _appSettings;

        //public DBContext(IOptions<AppSettings> appSettings)
        //{
        //    _appSettings = appSettings.Value;
        //}


        public static readonly ILoggerFactory loggerFactory = new LoggerFactory(new[] {
              new ConsoleLoggerProvider((_, __) => true, true)
        });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();


            optionsBuilder.UseLoggerFactory(loggerFactory)  //tie-up DbContext with LoggerFactory object
            .EnableSensitiveDataLogging()
            .UseMySql(configuration["ConnectionStrings:DefaultConnection"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SysCatalog>().HasKey(c => new { c.table_name, c.field_name, c.value });
        }

        public DbSet<AuthPermissions> AuthPermissions { get; set; }
        public DbSet<AuthUserPermission> AuthUserPermissions { get; set; }
        public DbSet<AuthGroupPermission> AuthGroupPermissions { get; set; }
        public DbSet<AuthUserGroup> AuthUserGroups { get; set; }
        public DbSet<AuthGroup> AuthGroups { get; set; }
        public DbSet<AuthUser> AuthUser { get; set; }
        public DbSet<SysCatalog> Catalogs { get; set; }
        public DbSet<ApiControlProyectos.Models.AuthModule> AuthModule { get; set; }
    }
}
