using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mockify.Models {
    /// <summary>
    /// Endpoint/User/Server Special Response Modes
    /// OK, Bad Gateway, etc
    /// </summary>
    public class SpecialResponseMode {

        [Key]
        public int SpecialResponseModeId { get; set; }
        public string Name { get; set; }

        Dictionary<int, SpecialResponseMode> SpecialResponseModemap = new Dictionary<int, SpecialResponseMode>();

        Dictionary<SpecialResponseMode, int> SpecialResponseModeIdMap = new Dictionary<SpecialResponseMode, int>();

        private SpecialResponseMode(int id, string name) {
            this.SpecialResponseModeId = id;
            this.Name = name;
            SpecialResponseModemap.Add(id, this);
            SpecialResponseModeIdMap.Add(this, id);
        }

        public static SpecialResponseMode ServiceOK = new SpecialResponseMode(200, "Service OK");
        public static SpecialResponseMode RequestTimeout = new SpecialResponseMode(408, "Request Timeout");
        public static SpecialResponseMode InternalServerError = new SpecialResponseMode(500, "Internal Server Error");
        public static SpecialResponseMode ServiceUnavailable = new SpecialResponseMode(503, "Service Unavailable");
        public static SpecialResponseMode BadGateway = new SpecialResponseMode(502, "Bad Gateway");

    }

}