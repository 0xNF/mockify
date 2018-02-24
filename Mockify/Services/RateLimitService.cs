using Microsoft.Extensions.Logging;
using Mockify.Data;
using System.Threading.Tasks;

namespace Mockify.Services {
    public class RateLimitService : IRateLimitService {
        MockifyDbContext _mc;
        ILogger<RateLimitService> _logger;

        public RateLimitService(MockifyDbContext mc, ILogger<RateLimitService> logger) {
            this._mc = mc;
            this._logger = logger;
        }

        public RateLimitService() {
        }

        /// <summary>
        /// Checks that the given user is within their overall rate-limiting across all applications
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<bool> UserWithinRateLimit(string user_id) {
            return true;
        }

        /// <summary>
        /// Checks that a given user is within their rate-limit for a specific application
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UserWithinAppRateLimit(string client_id, string user_id, string grant_type) {
            if(grant_type == "authorization_code") { //highest limit

            }
            return true;
        }

        /// <summary>
        /// Checks that the application is within its rate limit
        /// </summary>
        /// <param name="client_id"></param>
        /// <returns></returns>
        public async Task<bool> ApplicationWithinRateLimit(string client_id) {
            return true;
        }


        /// <summary>
        /// Increases 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="client_id"></param>
        /// <param name="grant_type"></param>
        /// <returns></returns>
        public async Task IncrementUserAPICallCount(string user_id, string client_id = null) {
            //increment overall by 1.
            if (!string.IsNullOrWhiteSpace(client_id)) {
                //then also increment for the client_id:user_id
            }
        }
    }


}
