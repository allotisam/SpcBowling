namespace SpcBowling.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SpcBowling.Models;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<SpcBowling.Models.SpcBowlingDbContext>
    {
        public Configuration()
        {
            // have this setting to false unless you made changes to the Model and uploading to Azure 
            AutomaticMigrationsEnabled = false;

            // have this setting to false unless deploying the app for first time with no db in Azure.
            // otherwise be very very care setting this to true.. it can mess up Azure DB and data starts
            // getting dupliated.
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(SpcBowling.Models.SpcBowlingDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            //AddUserAndRole(context);

            context.Players.AddOrUpdate(i => i.PlayerID,
                new Player { PlayerID = 1, FirstName = "Jeong Hyuk", LastName = "Park", PhoneNumber = "571-230-7352", Gender=Gender.Male },
                new Player { PlayerID = 2, FirstName = "Choong Hwan", LastName = "Lee", PhoneNumber = "703-899-1177", Gender=Gender.Male },
                new Player { PlayerID = 3, FirstName = "Hyunah", LastName = "Lim", PhoneNumber = "703-999-3992", Gender=Gender.Female },
                new Player { PlayerID = 4, FirstName = "Dong Chul", LastName = "Kang", PhoneNumber = "703-919-8263", Gender=Gender.Male },
                new Player { PlayerID = 5, FirstName = "Scott", LastName = "Kim", PhoneNumber = "571-230-6063", Gender=Gender.Male },
                new Player { PlayerID = 5, FirstName = "Dong Min", LastName = "Suh", PhoneNumber = "571-435-6476", Gender=Gender.Male }

                );

            context.Scores.AddOrUpdate(i => i.ScoreID,
                new Score { ScoreID = 1, Date = new DateTime(2015, 05, 11), BowlingScore = 204, HandiScore=30, PlayerID = 1 }
                //new Score { ScoreID = 2, Date = new DateTime(2015, 05, 11), BowlingScore = 148, PlayerID = 1 },
                //new Score { ScoreID = 3, Date = new DateTime(2015, 05, 11), BowlingScore = 168, PlayerID = 1 },
                //new Score { ScoreID = 4, Date = new DateTime(2015, 05, 11), BowlingScore = 175, PlayerID = 1 },
                //new Score { ScoreID = 5, Date = new DateTime(2015, 05, 11), BowlingScore = 170, PlayerID = 1 },
                //new Score { ScoreID = 6, Date = new DateTime(2015, 05, 04), BowlingScore = 137, PlayerID = 1 },
                //new Score { ScoreID = 7, Date = new DateTime(2015, 05, 04), BowlingScore = 144, PlayerID = 1 },
                //new Score { ScoreID = 8, Date = new DateTime(2015, 05, 04), BowlingScore = 195, PlayerID = 1 },
                //new Score { ScoreID = 9, Date = new DateTime(2015, 05, 04), BowlingScore = 168, PlayerID = 1 },
                //new Score { ScoreID = 10, Date = new DateTime(2015, 05, 04), BowlingScore = 190, PlayerID = 1 },
                //new Score { ScoreID = 11, Date = new DateTime(2015, 05, 04), BowlingScore = 116, PlayerID = 1 },
                //new Score { ScoreID = 12, Date = new DateTime(2015, 05, 04), BowlingScore = 217, PlayerID = 1 }
                );

            context.SaveChanges();
        }

        bool AddUserAndRole(SpcBowling.Models.SpcBowlingDbContext context)
        {
            IdentityResult ir;
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            ir = rm.Create(new IdentityRole("canEdit"));

            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = new ApplicationUser()
            {
                UserName = "allotisam@hotmail.com",
            };
            ir = um.Create(user, "Admin1!");

            if (ir.Succeeded == false)
                return ir.Succeeded;
            
            ir = um.AddToRole(user.Id, "canEdit");
            return ir.Succeeded;
        }
    }
}
