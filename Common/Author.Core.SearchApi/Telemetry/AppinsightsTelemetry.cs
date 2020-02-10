using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Author.Core.SearchApi.Telemetry
{
        public class AppinsightsTelemetry : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
                {
                    //set custom role name here
                    telemetry.Context.Cloud.RoleName = "Search Api";
                    telemetry.Context.Cloud.RoleInstance = "Search Instance";
                }
            }
        }    
}
