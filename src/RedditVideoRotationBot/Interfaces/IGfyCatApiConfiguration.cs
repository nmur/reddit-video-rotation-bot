namespace RedditVideoRotationBot.Interfaces
{
    public interface IGfyCatApiConfiguration
    {
        string GetClientId();

        string GetClientSecret();

        int GetUploadTimeoutInMs();

        int GetUploadStatusPollingPeriodInMs();
    }
}