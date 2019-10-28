using Author.Query.Domain.DBAggregate;

namespace Author.Query.Persistence.Interfaces
{
    public interface ICommonService
    {
        Languages GetLanguageFromLocale(string locale);
    }
}
