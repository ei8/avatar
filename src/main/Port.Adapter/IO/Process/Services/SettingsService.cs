using System;
using works.ei8.Cortex.Sentry.Application;
using works.ei8.Cortex.Sentry.Port.Adapter.Common;

namespace works.ei8.Cortex.Sentry.Port.Adapter.IO.Process.Services
{
    public class SettingsService : ISettingsService
    {
        public string CortexInBaseUrl => string.Empty; // DEL:? Environment.GetEnvironmentVariable(EnvironmentVariableKeys.CortexInBaseUrl);

        public string CortexOutBaseUrl => string.Empty; // DEL:? Environment.GetEnvironmentVariable(EnvironmentVariableKeys.CortexOutBaseUrl);

        public string CortexGraphOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.CortexGraphOutBaseUrl);

        public string EventSourcingOutBaseUrl => Environment.GetEnvironmentVariable(EnvironmentVariableKeys.EventSourcingOutBaseUrl);
    }
}
