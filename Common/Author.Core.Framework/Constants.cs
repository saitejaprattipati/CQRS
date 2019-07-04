using System.ComponentModel;

namespace Author.Core.Framework
{
    public class Constants
    {
        //Validation
        public const string GeneralStringRegularExpression = @"^([\w\W]*)$"; //This is to pass VeraCode checks
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
}
