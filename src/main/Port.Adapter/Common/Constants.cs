using System;

namespace works.ei8.Cortex.Sentry.Port.Adapter.Common
{
    public struct EnvironmentVariableKeys
    {
        public const string UserDatabasePath = "USER_DATABASE_PATH";
        public const string TestUserSubjectId = "TEST_USER_SUBJECT_ID";
        public const string RequireAuthentication = "REQUIRE_AUTHENTICATION";
        public const string TokenIssuerAddress = "TOKEN_ISSUER_ADDRESS";
        public const string CortexGraphOutBaseUrl = "CORTEX_GRAPH_OUT_BASE_URL";
        public const string EventSourcingOutBaseUrl = "EVENT_SOURCING_OUT_BASE_URL";
    }
}
