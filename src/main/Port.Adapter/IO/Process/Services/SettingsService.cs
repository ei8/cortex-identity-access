using ei8.Cortex.IdentityAccess.Domain.Model;
using ei8.Cortex.IdentityAccess.Port.Adapter.Common;
using System;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.IO.Process.Services
{
    public class SettingsService : ISettingsService
    {
        public string UserDatabasePath => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.UserDatabasePath);

        public string CortexGraphOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.CortexGraphOutBaseUrl);

        public string EventSourcingOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.EventSourcingOutBaseUrl);
    }
}
