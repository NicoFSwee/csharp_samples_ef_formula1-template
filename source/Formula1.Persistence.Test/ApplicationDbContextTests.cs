using Formula1.Core.Entities;
using Formula1.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Formula1.PersistenceTests
{
    [TestClass()]
    public class ApplicationDbContextTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            // Build the ApplicationDbContext 
            //  - with InMemory-DB
            return new ApplicationDbContext(
              new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging()
                .Options);
        }

        [TestMethod]
        public async Task D01_FirstDataAccessTest()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                Team team = new Team
                {
                    Name = "Red Bull",
                    Nationality = "Austria"
                };
                dbContext.Teams.Add(team);
                await dbContext.SaveChangesAsync();
            }
            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                var firstOrDefault = dbContext.Teams.FirstOrDefault(t => t.Name == "Red Bull");
                Assert.IsNotNull(firstOrDefault, "Zumindest ein Team muss in DB sein");
                Assert.AreEqual("Red Bull", firstOrDefault.Name);
            }
        }

        [TestMethod]
        public async Task D02_NationalityTest()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                Team team1 = new Team
                {
                    Name = "Red Bull",
                    Nationality = "Austria"
                };
                Team team2 = new Team
                {
                    Name = "Ferrari",
                    Nationality = "Italy"
                };
                dbContext.Teams.Add(team1);
                dbContext.Teams.Add(team2);
                await dbContext.SaveChangesAsync();
            }

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                Team team = dbContext.Teams.FirstOrDefault(t => t.Nationality == "Italy");
                Assert.IsNotNull(team, "Zumindest ein Team muss in DB sein");
                Assert.AreEqual("Italy", team.Nationality);
            }
        }

        [TestMethod]
        public async Task D03_InsertDriverWithLinkedPropertie()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                Team team1 = new Team
                {
                    Name = "Red Bull",
                    Nationality = "Austria"
                };
                Team team2 = new Team
                {
                    Name = "Ferrari",
                    Nationality = "Italy"
                };
                Driver driver1 = new Driver
                {
                    FirstName = "Hannes",
                    LastName = "Bauer",
                    Nationality = "Austria"
                };
                Driver driver2 = new Driver
                {
                    FirstName = "Giovanni",
                    LastName = "Giotto",
                    Nationality = "Italy"
                };
                Result result = new Result
                {
                    Race = new Race
                    {
                        Country = "Austria",
                        City = "Salzburg",
                        Date = DateTime.Now,
                    },
                    Team = team1,
                    Driver = driver1,
                    Position = 3,
                    Points = 20
                };

                dbContext.Teams.Add(team2);
                dbContext.Drivers.Add(driver2);
                dbContext.Results.Add(result);

                await dbContext.SaveChangesAsync();
            }

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                Driver driver = dbContext.Drivers.FirstOrDefault(d => d.FirstName == "Hannes");
                Assert.IsNotNull(driver, "Zumindest ein Driver muss in DB sein");
                Assert.AreEqual("Hannes", driver.FirstName);
            }
        }

    }
}