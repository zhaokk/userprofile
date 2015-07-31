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
            success = idManager.CreateRole("Admin");
            if (!success == true) return success;

            success = idManager.CreateRole("CanEdit");
            if (!success == true) return success;

            success = idManager.CreateRole("User");
            if (!success) return success;


            var newUser = new ApplicationUser()
            {
                UserName = "jatten",
                firstName = "John",
                lastName = "Atten",
                email = "jatten@typecastexception.com",
                photoDir="~\\userprofile\\zhaokk.jpg",
                phoneNum=451411202,
                country="australia",
                postcode=2500,
                street="crown",
                state="nsw",
                streetNumber=59,
                dob="21/07/2015"
            };

            // Be careful here - you  will need to use a password which will 
            // be valid under the password rules for the application, 
            // or the process will abort:
            success = idManager.CreateUser(newUser, "Password");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Admin");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "CanEdit");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "User");
            if (!success) return success;

            return success;
        }

    }
}
