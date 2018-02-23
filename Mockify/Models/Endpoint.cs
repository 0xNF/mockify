using System;

namespace Mockify.Models {
    /// <summary>
    /// Representation of an Endpoint for Panel Control.
    /// </summary>
    public class Endpoint {
        public RateLimits RateLimits { get; set; } = RateLimits.DEFAULT;
        public SpecialResponseMode ResponseMode { get; set; } = SpecialResponseMode.ServiceOK;
        public readonly string endpoint; // No ending slash - but YES initial slash. /me/playlists 
        public readonly Type ReturnType;
    }

}