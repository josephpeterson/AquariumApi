namespace AquariumApi.Models
{
    public enum JobEndReason
    {
        Normally,
        MaximumRuntimeReached,
        Error,
        Canceled,
        ForceStop,
    }
}