using System;

namespace Mockify.Models {
    /// <summary>
    /// Rate Limits as applied universally, per endpoint, or per user. 
    /// Precendce is 
    ///     Server => User => Endpoint
    /// </summary>
    public class RateLimits {

        public const string RateLimitExceededError = "Rate Limit Exceeded";
        public const string RateLimitExceededErrorDescription = "You have exceeded your allowed rate limit.";

        /// <summary>
        /// The number of calls a user may make in the specified Rate Window
        /// </summary>
        int CallsPerWindow { get; set; }

        /// <summary>
        /// The Time-Span that a rate window encompasses. When the time-span expires, the user may begin making API calls again
        /// </summary>
        TimeSpan RateWindow { get; set; }

        /// <summary>
        /// For the given RateWindow, the time that a user first began making API calls
        /// </summary>
        DateTime WindowStartTime { get; set; }

        /// <summary>
        /// the number of calls this user has made in the given window.
        /// </summary>
        int CurrentCalls { get; set; }

        /// <summary>
        /// Whether the user is presently limited
        /// </summary>
        bool IsRateLimited { get; set; }

        public static readonly RateLimits DEFAULT = new RateLimits() {
            CallsPerWindow = 200,
            RateWindow = TimeSpan.FromHours(1),
            IsRateLimited = false,
            CurrentCalls = 0,
            WindowStartTime = DateTime.MinValue
        };
    }


}