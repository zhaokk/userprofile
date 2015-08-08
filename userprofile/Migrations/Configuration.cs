using userprofile.Models;
using System.Data.Entity.Migrations;
namespace userprofile.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<userprofile.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "userprofile.Models.ApplicationDbContext";
        }

        protected override void Seed(userprofile.Models.ApplicationDbContext context)
        {
            this.AddUserAndRoles();


        }
        bool AddUserAndRoles()
        {
            bool success = false;

            var idManager = new IdentityManager();
            success = idManager.CreateRole("Referee");
            if (!success == true) return success;

             idManager = new IdentityManager();
             success = idManager.CreateRole("Admin");
            if (!success == true) return success;


            return success;
        }

    }
}
