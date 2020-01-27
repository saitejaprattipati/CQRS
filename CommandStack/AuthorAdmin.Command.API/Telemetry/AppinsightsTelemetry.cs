using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace AuthorAdmin.Command.API.Telemetry
{
    public class AppinsightsTelemetry : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                //set custom role name here
                telemetry.Context.Cloud.RoleName = "AuthorAdmin Api";
                telemetry.Context.Cloud.RoleInstance = "AuthorAdmin Instance";
            }
        }
    }
}
