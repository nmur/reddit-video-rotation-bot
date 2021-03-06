﻿using RedditVideoRotationBot.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace RedditVideoRotationBot.Configurations
{
    [ExcludeFromCodeCoverage]
    public class GfyCatApiConfiguration : IGfyCatApiConfiguration
    {
        private readonly string ClientId;

        private readonly string ClientSecret;

        private readonly int UploadTimeoutInMs;

        public GfyCatApiConfiguration(string clientId, string clientSecret, int uploadTimeoutInMs)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            UploadTimeoutInMs = uploadTimeoutInMs;
        }

        public string GetClientId()
        {
            return ClientId;
        }

        public string GetClientSecret()
        {
            return ClientSecret;
        }

        public int GetUploadStatusPollingPeriodInMs()
        {
            return 5000;
        }

        public int GetUploadTimeoutInMs()
        {
            return UploadTimeoutInMs;
        }
    }
}