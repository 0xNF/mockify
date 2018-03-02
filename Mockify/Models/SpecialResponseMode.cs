using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mockify.Models {
    /// <summary>
    /// Endpoint/User/Server Special Response Modes
    /// OK, Bad Gateway, etc
    /// </summary>
    public class SpecialResponseMode {

        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static Dictionary<int, SpecialResponseMode> SpecialResponseModemap = new Dictionary<int, SpecialResponseMode>();
        public static Dictionary<SpecialResponseMode, int> SpecialResponseModeIdMap = new Dictionary<SpecialResponseMode, int>();
        public static List<SpecialResponseMode> SpecialResponseModes = new List<SpecialResponseMode>();

        public SpecialResponseMode() {

        }

        private SpecialResponseMode(int id, string name, string description) {
            this.id = id;
            this.Name = name;
            this.Description = description;
            if(!SpecialResponseModemap.ContainsKey(id)) {
                SpecialResponseModemap.Add(id, this);
                SpecialResponseModeIdMap.Add(this, id);
                SpecialResponseModes.Add(this);
            }
        }

        public JObject ToJson() {
            Dictionary<string, JToken> keys = new Dictionary<string, JToken>() {
                { "Type", "SpecialResponseMode" },
                { nameof(id), id },
                { nameof(Name), Name },
                { nameof(Description), Description }
            };
            return JObject.FromObject(keys);
        }

        public override string ToString() {
            return this.Name;
        }

        public static SpecialResponseMode ServiceOK = new SpecialResponseMode(200, "Service OK", "The server will handle all requests normally");
        public static SpecialResponseMode RequestTimeout = new SpecialResponseMode(408, "Request Timeout", "The server will respond to all API and Token requests as though it was timing out. Requests will never finish and nothing will be returned to the client.");
        public static SpecialResponseMode InternalServerError = new SpecialResponseMode(500, "Internal Server Error", "The server will respond to all API and Token requests with Error 500.");
        public static SpecialResponseMode ServiceUnavailable = new SpecialResponseMode(503, "Service Unavailable", "The server will respond to all API and Token requests with Error 503.");
        public static SpecialResponseMode BadGateway = new SpecialResponseMode(502, "Bad Gateway", "The server will respond to all API and Token requests with Error 502");



    }

}