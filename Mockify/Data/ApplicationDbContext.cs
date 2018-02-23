using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mockify.Models;
using Mockify.ViewModels;

namespace Mockify.Data
{
    public class MockifyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MockifyDbContext(DbContextOptions<MockifyDbContext> options)
            : base(options)
        {
        }

        public DbSet<RegisteredApplication> Applications { get; set; }

        public async Task<RegisteredApplication> CreateRegisteredApplication(string appname, string appdescription) {
            string newCid = await RegisteredApplication.CreateClientId(appname);
            string newSecret = await RegisteredApplication.CreateClientSecret(appname);
            RegisteredApplication ra = new RegisteredApplication() {
                ApplicationDescription = appdescription,
                ApplicationName = appname,
                ClientId = newCid,
                ClientSecret = newSecret
            };
            var x = await Applications.AddAsync(ra);
            try {
                await SaveChangesAsync();
                return ra;
            }
            catch (DbUpdateException dbue) {
                return null;
            }
            catch (Exception e) {
                return null;
            }
        }

        public async Task<RegisteredApplication> UpdateClientSecret(string clientid) {
            RegisteredApplication old = await Applications.FindAsync(clientid);
            if (old == null) {
                return null; // No such entity
            }
            old.ClientSecret = await RegisteredApplication.CreateClientSecret(old.ApplicationName);
            Applications.Update(old);
            try {
                await SaveChangesAsync();
                return old;
            }
            catch (DbUpdateException dbue) {
                return null;
            }
            catch (Exception e) {
                return null;
            }
        }

        public async Task<bool> DeleteRegisteredApplication(string clientId) {
            RegisteredApplication ra = await Applications.Include(x => x.RedirectURIs).FirstAsync(x => x.ClientId.Equals(clientId));
            if (ra == null) {
                return false; // No such element
            }
            else {
                Applications.Remove(ra);
                try {
                    await SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException dbue) {
                    return false;
                }
                catch (Exception e) {
                    return false;
                }
            }
        }

        public async Task<RegisteredApplication> UpdateRegisteredApplication(RegisteredApplication ra) {
            if (ra == null) {
                return null; // No such element
            }
            try {
                await SaveChangesAsync();
                return ra;
            }
            catch (DbUpdateException dbue) {
                return null;
            }
            catch (Exception e) {
                return null;
            }
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<RegisteredApplication>().HasMany(x => x.UserApplicationTokens);
            //builder.Entity<UserApplicationToken>().HasOne<ApplicationUser>();
            //builder.Entity<ApplicationUser>().HasMany(x => x.UserApplicationTokens);


            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }


        public DbSet<Mockify.ViewModels.CreateUserViewModel> CreateUserViewModel { get; set; }


        public DbSet<Mockify.Models.ApplicationUser> ApplicationUser { get; set; }


        public DbSet<Mockify.ViewModels.CreateManyUsersViewModel> CreateManyUsersViewModel { get; set; }
    }
}
