using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SDK.WorkflowIO;
using System;

namespace CustomWorkflowIOData
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    internal class CustomIO : WorkflowIOData
    {
        public string MediaID { get; set; }

        public string Vendor { get; set; }

        [JsonProperty("vendor_id")]
        public int VendorID { get; set; }

        public string Title { get; set; }

        [JsonProperty("title_id")]
        public string TitleID { get; set; }

        public string ContentType { get; set; }

        public string Filename { get; set; }

        public DateTimeOffset RightsExpiry { get; set; }
    }
}
