using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Author.Query.New.API.Telemetry
{
    public class AppinsightsTelemetry : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                //set custom role name here
                telemetry.Context.Cloud.RoleName = "AuthorQuery Api";
                telemetry.Context.Cloud.RoleInstance = "AuthorQuery Instance";
            }
        }
    }
}
