using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mockify.Models;

namespace Mockify.Data {


    public class DataSeeder {

        public async static Task SeedResponseModes(MockifyDbContext context) {
            if (!context.ResponseModes.Any()) {
                context.AddRange(SpecialResponseMode.SpecialResponseModes);
                await context.SaveChangesAsync();
            }
        }

        public async static Task SeedServerSettings(MockifyDbContext context) { 
            if (!context.ServerSettings.Any()) {
                ServerSettings.Settings = ServerSettings.DEFAULT;
                context.Add(ServerSettings.Settings.RateLimits);
                context.Add(ServerSettings.Settings);
                await context.SaveChangesAsync();
            }
            else {
                ServerSettings.Settings = await context.ServerSettings.Include(x=>x.RateLimits).Include(x=>x.ResponseMode).OrderBy(x => x.ServerSettingsId).FirstAsync();
            }
        }
    }
}
