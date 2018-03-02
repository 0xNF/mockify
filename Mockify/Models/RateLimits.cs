using System;
using System.ComponentModel.DataAnnotations;

namespace Mockify.Models {
    /// <summary>
    /// Rate Limits as applied universally, per endpoint, per user, and per app
    /// Precendce is 
    ///     Server => Endpoint => App => User
    /// </summary>
    public class RateLimits {

        public const int MaxRateLimit = 10_000_000;
        public const int MinRateLimit = 0;
        public const int MaxTimeWindowMinutes = 10_080;
        public const int MinTimeWindowMinutes = 0;
        public const string RateLimitExceededError = "Rate Limit Exceeded";
        public const string RateLimitExceededErrorDescription = "You have exceeded your allowed rate limit.";


        [Key]
        public int RateLimitsId { get; set; }

        /// <summary>
        /// The number of calls an attached object may make in the specified Rate Window
        /// </summary>
        public int CallsPerWindow { get; set; }

        /// <summary>
        /// The Time-Span that a rate window encompasses. 
        /// Call Per Window count is limited to this RateWindow.
        /// When the time-span expires, this attached object may begin making API calls again
        /// </summary>
        public TimeSpan RateWindow { get; set; }

        /// <summary>
        /// For the given RateWindow, the time that the attached object first began making API calls
        /// </summary>
        public DateTime WindowStartTime { get; set; }

        /// <summary>
        /// the number of calls the attached object has made in the given window.
        /// </summary>
        int _CurrentCalls;
        public int CurrentCalls {
            get {
                return _CurrentCalls;
            }
            set {
                if (_CurrentCalls == 0) {
                    /* a new Rate Limit Window is created upon receiving the API call */
                    WindowStartTime = DateTime.UtcNow;
                }
                _CurrentCalls += 1;
            }
        }

        /// <summary>
        /// Whether Rate Limiting is in effect for this object
        /// </summary>
        public bool IsRateLimited {
            get {
                return CurrentCalls >= CallsPerWindow;
            }
        }

        public int RetryAfter {
            get {
                if(!IsRateLimited) {
                    return 0;
                }
                DateTime NextResetAt = WindowStartTime + RateWindow;
                TimeSpan TimeSpanFromNow = NextResetAt - DateTime.UtcNow;
                double SecondsFromNow = TimeSpanFromNow.TotalSeconds;
                int IntSeconds = (int)Math.Ceiling(SecondsFromNow);
                return IntSeconds;
            }
        }

        public void ResetRateWindow() {
            this.CurrentCalls = 0;
        }

        public static readonly RateLimits DEFAULT = new RateLimits() {
            CallsPerWindow = 200,
            RateWindow = TimeSpan.FromHours(1),
            CurrentCalls = 0,
            WindowStartTime = DateTime.MinValue
        };
    }


}