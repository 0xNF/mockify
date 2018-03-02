namespace Mockify.ViewModels {
    public class ServerSettingsViewModel {
        public int ServerSettingsId { get; set; }
        public string ResponseModeDescription { get; set; }
        public string DefaultMarket { get; set; }
        public string ResponseModeId { get; set; }
        public RateLimitViewModel RateLimits { get; set; }
    }
    public class RateLimitViewModel {
        public int RateLimitId { get; set; }
        public int RateWindowInMinutes { get; set; }
        public int CallsPerWindow { get; set; }
    }
}
