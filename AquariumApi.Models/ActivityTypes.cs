namespace AquariumApi.Models
{
    public enum ActivityTypes
    {
        CreateAccountActivity = 3200,
        DeleteAccountActivity = 3201,
        LoginAccountActivity = 3205,
        DeviceLoginAccountActivity = 3206,

        CreateAquariumActivity = 1200,
        DeleteAquariumActivity = 1201,
        UpdateAquariumActivity = 1202,

        CreateAquariumTestResults = 1300,
        DeleteAquariumTestResults = 1301,
        UpdateAquariumTestResults = 1302,
    }
}