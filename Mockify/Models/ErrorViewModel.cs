using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mockify.Models {
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class ErrorObject {

        public int status { get; }
        public string message { get; }

        public ErrorObject(int status, string mess) {
            this.status = status;
            this.message = mess;
        }

        public JObject ToJson() {

            Dictionary<string, JToken> keySub = new Dictionary<string, JToken>() {
                { nameof(status), status },
                { nameof(message), message }
            };

            Dictionary<string, JToken> keys = new Dictionary<string, JToken>() {
                { "error",  JObject.FromObject(keySub)}
            };

            return JObject.FromObject(keys);
        }

    }

}