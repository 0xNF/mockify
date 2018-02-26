using Microsoft.Extensions.Logging;
using Mockify.Data;
using Mockify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mockify.Services {

    public interface IRateLimitsGetter {
        IQueryable<RateLimits> ExpiredLimits { get; }
        Task Save();
    }
    public class RateLimitsGetter : IRateLimitsGetter {
        MockifyDbContext _mc;

        public IQueryable<RateLimits> ExpiredLimits { get; }

        public RateLimitsGetter(MockifyDbContext mc) {
            _mc = mc;
            ExpiredLimits = _mc.RateLimits.Where(x => (x.WindowStartTime + x.RateWindow) <= DateTime.UtcNow);
        }

        public async Task Save() {
            await _mc.SaveChangesAsync();
        }
    }

    public class RateLimitResetService : HostedService {

        private static TimeSpan ts = TimeSpan.FromMinutes(1);
        private ILogger<RateLimitResetService> _logger;
        private readonly IRateLimitsGetter _getter;

        public RateLimitResetService(ILogger<RateLimitResetService> logger) {
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {

            while (!cancellationToken.IsCancellationRequested) {
                try {
                    /* All Expired Rate Limits */
                    foreach(RateLimits rl in _getter.ExpiredLimits) {
                        rl.CurrentCalls = 0;
                    }
                    await _getter.Save();
                }
                catch (Exception e) {
                }
                await Task.Delay(ts, cancellationToken);
            }
        }
    }
}
