using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mockify.Data {
    public static class Extensions {

        public static IWebHost MigrateDatabase(this IWebHost webHost) {
            using (var scope = webHost.Services.CreateScope()) {
                var services = scope.ServiceProvider;

                try {
                    var db = services.GetRequiredService<MockifyDbContext>();
                    DataSeeder.SeedResponseModes(db).Wait();
                    DataSeeder.SeedServerSettings(db).Wait();
                }
                catch (Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            return webHost;
        }
    }
}
