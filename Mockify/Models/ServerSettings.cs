using System.Collections.Generic;

namespace Mockify.Models {
    /// <summary>
    /// Server Settings class, storing if the Server is a Special Response Mode, what its base rate limits are, etc
    /// </summary>
    public static class ServerSettings {

        public static string DefaultMarket { get; set; } = "US";

        /// <summary>
        /// The Special Response Mode server is presently in.
        /// </summary>
        public static SpecialResponseMode ResponseMode { get; set; } = SpecialResponseMode.ServiceOK;

        /// <summary>
        /// The number of calls a given user can make in a given Rate Window. 
        /// </summary>
        public static int MaxCallsPerWindow { get; set; }

        /// <summary>
        /// The Server-Wide rate limit object. Can be overrien in an individual user
        /// </summary>
        public static RateLimits RateLimits { get; set; }

        /// <summary>
        /// The endpoints currently being served up by Mockify
        /// </summary>
        public static List<Endpoint> Endpoints { get; set; }

    }

}