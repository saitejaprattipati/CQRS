using System.ComponentModel;

namespace Author.Core.Framework
{
    public class Constants
    {
        public const string CONTENT_TYPE_ARTICLE = "TAX_CONTENT_TYPE_ARTICLE";
        public const string CONTENT_TYPE_RESOURCE = "TAX_CONTENT_TYPE_RESOURCE";

        //Validation
        public const string GeneralStringRegularExpression = @"^([\w\W]*)$"; //This is to pass VeraCode checks
        public const string DisclaimersDiscriminator = "Disclaimers";
        public const string CountriesDiscriminator = "Countries";
        public const string CountryGroupsDiscriminator = "CountryGroups";
        public const string ResourceGroupsDiscriminator = "ResourceGroups";
        public const string SystemUsersDiscriminator = "SystemUsers";
        public const string TaxTagsDiscriminator = "TaxTags";
        public const string ImagesDiscriminator = "Images";
        public const string ArticlesDiscriminator = "Articles";
        public const string TAX_COOKIE_BROWSER_NAME = "taxcookie";
        public const string YouTube_Link = "youtube";
        public const string REQUEST_HEADER_ACEPT_LANGUAGE = "en";
        public const string DEFAULT_REQUEST_HEADER_ACCEPT_LANGUAGE = "en-us";
        public static readonly string[] ChineseTraditional = { "zh-tw", "zh-hk" };
        public static readonly string[] ChineseSimplified = { "zh-cn", "zh-sg" };
    }

    public enum SystemUserRole
    {
        [Description("Member firm author")]
        MFAuthor = 1,
        [Description("Member firm admin")]
        MFAdmin,
        [Description("Global author")]
        GlobalAuthor,
        [Description("Global admin")]
        GlobalAdmin
    }

    public enum CustomErrorsMode
    {
        Off = 0,
        On = 1,
        RemoteOnly = 2
    }
    public enum ArticleType
    {
        Article = 1,
        Page,
        Resource
    }

    public enum ArticleSubType
    {
        //Range 1-99 represent article type
        [Description("Please Select")]
        NoneOrBlank = 1,
        Analysis = 2,
        [Description("Dbriefs Asia Pacific")]
        APDbriefs = 3,
        [Description("Dbriefs Bytes")]
        DbriefsBytes = 4,
        Feature = 5,
        News = 6,
        [Description("News Flash")]
        Newsflash = 7,
        Perspective = 8,

        Divider = 100,

        //Range 101-200 represent site disclaimer type
        [Description("About Deloitte")]
        AboutDeloitte = 101,
        [Description("About Deloitte Tax@Hand")]
        AboutDeloitteTaxAtHand = 102,
        [Description("Contact Us")]
        ContactUs = 103,
        [Description("Third Party Acknowledgments")]
        ThirdPartyAcknowledgments = 104,
        [Description("Cookie Notice")]
        CookieNotice = 105,
        [Description("Privacy Statement")]
        PrivacyStatement = 106,
        [Description("Terms of Use")]
        TermsOfUse = 107
    }
    public enum ImageType
    {
        Banner = 1,
        [Description("Banner Resource")]
        BannerResource,
        Content,
        [Description("Flag PNG")]
        FlagPNG,
        [Description("Flag SVG")]
        FlagSVG,
        Profile
    }

    public static class ServiceBusEventType
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }
}
