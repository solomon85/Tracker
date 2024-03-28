using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GPS_Tracker.Models
{
    internal class ApplicationDataContext : System.Data.Entity.DbContext
    {
        public ApplicationDataContext() 
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();
            string conStr = configuration.GetSection("ConnectionStrings")["sqlConnection"];
            if (string.IsNullOrEmpty(conStr))
            {
                Console.WriteLine("You most set connection string in user secret file.");
                throw new Exception("You most set connection string in user secret file.");
            }
            this.Database.Connection.ConnectionString = conStr;
        }
        //static ApplicationDataContext()
        //{
        //    System.Data.Entity.Database.SetInitializer(new
        //        System.Data.Entity.CreateDatabaseIfNotExists<ApplicationDataContext>());
        //}
        public static ApplicationDataContext Create()
        {
            return new ApplicationDataContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new TowingData.Configuration());
            modelBuilder.Configurations.Add(new UnCleanData.Configuration());
        }


        public DbSet<Data> Data { get; set; }
        public DbSet<UnCleanData> UnCleanData { get; set; }
        public DbSet<TowingData> TowingData { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<ParkPoint> ParkPoints { get; set; }
        public DbSet<StandbyPoint> StandbyPoints { get; set; }
        public DbSet<Rpt_DailyPerformance> Rpt_DailyPerformances { get; set; }
        public DbSet<Log> Logs { get; set; }

    }
}
