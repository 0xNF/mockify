using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mockify.Models {
    /// <summary>
    /// Server Settings class, storing if the Server is a Special Response Mode, what its base rate limits are, etc
    /// </summary>
    public class ServerSettings {

        [Key]
        public int ServerSettingsId { get; set; }

        public string DefaultMarket { get; set; } = "US";

        /// <summary>
        /// The Special Response Mode server is presently in.
        /// </summary>
        public SpecialResponseMode ResponseMode { get; set; } = SpecialResponseMode.ServiceOK;

        /// <summary>
        /// The Server-Wide rate limit object. Can be overrien in an individual user
        /// </summary>
        public RateLimits RateLimits { get; set; }

        /// <summary>
        /// The endpoints currently being served up by Mockify
        /// </summary>
        public List<Endpoint> Endpoints { get; set; }

    }

}