using Microsoft.Extensions.Options;
using System;

namespace Author.Core.Framework.Utilities
{
    public class UtilityService : IUtilityService
    {
        private readonly IOptions<AppSettings> _appSettings;
        public UtilityService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public bool IsTrusted()
        {
            return _appSettings.Value.CustomErrorsMode.Trim().Equals(Convert.ToString(CustomErrorsMode.Off), StringComparison.OrdinalIgnoreCase);
        }
    }
}
