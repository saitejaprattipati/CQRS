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
        private readonly TaxathandDbContext _dbContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ICacheService<Disclaimers, DisclaimerDTO> _cacheService;

        public DisclaimerService(TaxathandDbContext dbContext, IOptions<AppSettings> appSettings,
            ICacheService<Disclaimers, DisclaimerDTO> cacheService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _appSettings = appSettings;
        }
        public async Task<DisclaimerResult> GetAllDisclaimersAsync(LanguageDTO language, int pageNo, int pageSize)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var disclaimers = await GetDisclaimersAsync(localeLangId, pageNo, pageSize);

            // If localLanguage data is not available then pull the data based on default language
            if (disclaimers.Disclaimers.Count == 0)
            {
                disclaimers = await GetDisclaimersAsync(dftLanguageId, pageNo, pageSize);
            }

            return disclaimers;
        }

        private async Task<DisclaimerResult> GetDisclaimersAsync(int localeLangId, int pageNo, int pageSize)
        {
            var disclaimerResult = new DisclaimerResult();

            var disclaimersFromCache = await _cacheService.GetAllAsync("disclaimersCacheKey");

            var disclaimers = disclaimersFromCache.Where(d => d.LanguageId.Equals(localeLangId)).ToList();

            if (disclaimers.Count() == 0)
            {
                return null;
            }
            disclaimerResult.Disclaimers = disclaimers;
            return disclaimerResult;
        }

        public async Task<DisclaimerDTO> GetDiscalimerAsync(LanguageDTO language, int disclaimerId)
        {
            var localeLangId = language.LanguageId;
            var dftLanguageId = int.Parse(_appSettings.Value.DefaultLanguageId);

            // By default pick the localLanguage value
            var disclaimer = await GetDisclaimerDataAsync(disclaimerId, localeLangId);

            // If localLanguage data is not available then pull the data based on default language
            if (disclaimer == null)
            {
                disclaimer = await GetDisclaimerDataAsync(disclaimerId, dftLanguageId);
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
