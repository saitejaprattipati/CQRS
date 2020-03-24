using Author.Query.Persistence.DTO;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL.Types
{
    public class ArticleType : ObjectGraphType<ArticleDTO>, IGraphQLType
    {
        public ArticleType()
        {
            Name = "Article";
            Field(a => a.ArticleId, nullable: true);
            Field(a => a.Title, nullable: true);
            Field(a => a.TitleInEnglishDefault, nullable: true);
            Field(a => a.Author, nullable: true);
            Field(a => a.ContentType, nullable: true);
            Field(a => a.TeaserText, nullable: true);
            Field(a => a.PublishedDate, nullable: true);
            Field(a => a.ImagePath, nullable: true);
            Field(a => a.ImageCredit, nullable: true);
            Field(a => a.ImageDescriptionText, nullable: true);
            Field(a => a.Content, nullable: true);
            Field(a => a.ResourcePosition, nullable: true);
            Field(a => a.Province, nullable: true);
            Field(a => a.CompleteResponse);
            Field(a => a.ContainsYoutubeLink);
            Field<ListGraphType<ContactType>>(
                "relatedContacts",
                resolve: context =>
                {
                    return context.Source.RelatedContacts;
                }
            );
            Field<ListGraphType<CountryType>>(
                "relatedCountries",
                resolve: context =>
                {
                    return context.Source.RelatedCountries;
                }
            );
            Field<CountryType>(
                "relatedCountry",
                resolve: context =>
                {
                    return context.Source.RelatedCountry;
                }
            );
            Field<ListGraphType<TaxTagsType>>(
                "relatedTaxTags",
                resolve: context =>
                {
                    return context.Source.RelatedTaxTags;
                }
            );
            Field<ResourceGroupType>(
                "resourcegroup",
                resolve: context =>
                {
                    return context.Source.ResourceGroup;
                }
            );
            Field<ListGraphType<LanguageType>>(
                "availableLanguages",
                resolve: context =>
                {
                    return context.Source.AvailableLanguages;
                }
            );
            Field<ListGraphType<RelatedArticleType>>(
                "relatedArticles",
                resolve: context =>
                {
                    return context.Source.RelatedArticles;
                }
            );
            Field<ListGraphType<RelatedResourceType>>(
                "relatedResources",
                resolve: context =>
                {
                    return context.Source.RelatedResources;
                }
            );
        }
    }
}
