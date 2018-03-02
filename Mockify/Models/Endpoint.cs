using System;
using System.ComponentModel.DataAnnotations;

namespace Mockify.Models {
    /// <summary>
    /// Representation of an Endpoint for Panel Control.
    /// </summary>
    public class Endpoint {

        [Key]
        public int EndpointId { get; set; }
        public RateLimits RateLimits { get; set; } = RateLimits.DEFAULT;
        public SpecialResponseMode ResponseMode { get; set; } = SpecialResponseMode.ServiceOK;
        public string Path { get; set; } // No ending slash - but YES initial slash. /me/playlists 

    }

}