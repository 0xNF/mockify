using System.Threading.Tasks;

namespace Mockify.Services {

    public interface IRateLimitService {
        /// <summary>
        /// Checks that the supplied User is within their overall Mockify rate-limit boundaries.
        /// This is independant of their rate-limiting status in any individual application
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Task<bool> UserWithinRateLimit(string user_id);

        /// <summary>
        /// Checks that the supplied User is within their rate limit for the specified application
        /// </summary>
        /// <param name="client_id"></param>
        /// <param name="user_id"></param>
        /// <param name="grant_type"></param>
        /// <returns></returns>
        Task<bool> UserWithinAppRateLimit(string client_id, string user_id, string grant_type);

        /// <summary>
        /// Checks that the supplied Application is within its rate limit
        /// </summary>
        /// <param name="client_id"></param>
        /// <returns></returns>
        Task<bool> ApplicationWithinRateLimit(string client_id);

        /// <summary>
        /// Increases the API Call count of the given user by one.
        /// Implementations need to be sure to also implement the increase to the indi
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Task IncrementUserAPICallCount(string user_id, string client_id = null);

    }


}
