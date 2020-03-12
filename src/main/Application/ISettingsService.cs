using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Sentry.Application
{
    public interface ISettingsService
    {
        string CortexInBaseUrl { get; }
        string CortexOutBaseUrl { get; }

        string CortexGraphOutBaseUrl { get; }
    }
}
