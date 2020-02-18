using Author.Core.Framework;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using Author.Query.Persistence.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Author.Query.Persistence
{
    public class DisclaimerService : IDisclaimerService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ICacheService<Disclaimers, DisclaimerDTO> _cacheService;
        private readonly ICommonService _commonService;

        public DisclaimerService(IOptions<AppSettings> appSettings,
            ICacheService<Disclaimers, DisclaimerDTO> cacheService, ICommonService commonService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _appSettings = appSettings;
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
        }
        public async Task<DisclaimerResult> GetAllDisclaimersAsync()
        {
            var languageDetails = _commonService.GetLanguageDetails();

            // By default pick the localLanguage value
            var disclaimers = await GetDisclaimersAsync(languageDetails.LocaleLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (disclaimers.Disclaimers.Count == 0)
            {
                disclaimers = await GetDisclaimersAsync(languageDetails.DefaultLanguageId);
            }

            return disclaimers;
        }

        private async Task<DisclaimerResult> GetDisclaimersAsync(int languageId)
        {
            var disclaimerResult = new DisclaimerResult();

            var disclaimersFromCache = await _cacheService.GetAllAsync("disclaimersCacheKey");

            var disclaimers = disclaimersFromCache.Where(d => d.LanguageId.Equals(languageId)).ToList();

            if (disclaimers.Count() == 0)
            {
                return null;
            }
            disclaimerResult.Disclaimers = disclaimers;
            return disclaimerResult;
        }

        public async Task<DisclaimerDTO> GetDiscalimerAsync(int disclaimerId)
        {
            var languageDetails = _commonService.GetLanguageDetails();

            // By default pick the localLanguage value
            var disclaimer = await GetDisclaimerDataAsync(disclaimerId, languageDetails.LocaleLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (disclaimer == null)
            {
                disclaimer = await GetDisclaimerDataAsync(disclaimerId, languageDetails.DefaultLanguageId);
            }

            return disclaimer;
        }

        private async Task<DisclaimerDTO> GetDisclaimerDataAsync(int disclaimerId, int languageId)
        {
            var disclaimersFromCache = await _cacheService.GetAllAsync("disclaimersCacheKey");

            return disclaimersFromCache.FirstOrDefault(d => d.LanguageId.Equals(languageId) && d.DisclaimerId.Equals(disclaimerId));
        }
    }
}
