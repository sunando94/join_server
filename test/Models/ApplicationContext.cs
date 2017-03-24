using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace test.Models
{
     public class ApplicationUser 
    {
       

    }


    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>());
           this.Configuration.LazyLoadingEnabled = false;
         // this.Configuration.ProxyCreationEnabled = false;
           // this.Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Event>().HasMany<User>(c=>c.joinedUser).WithMany(p=>p.joinedEvents).Map(m=>{m.MapLeftKey("EventId");m.MapRightKey("UserId");m.ToTable("UserJoinedEvents");});
            modelBuilder.Entity<Event>().HasMany<User>(c => c.watchUser).WithMany(p => p.watchedEvents).Map(m => { m.MapLeftKey("EventId"); m.MapRightKey("UserId"); m.ToTable("UserWatchedEvents"); });
           
         

        }
    }
  

}
