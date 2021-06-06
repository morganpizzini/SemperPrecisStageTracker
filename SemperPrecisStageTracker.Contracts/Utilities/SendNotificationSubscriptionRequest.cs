namespace SemperPrecisStageTracker.Contracts.Utilities
{
    public class SendNotificationSubscriptionRequest{
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }
}