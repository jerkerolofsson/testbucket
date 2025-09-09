using Microsoft.Extensions.Localization;

using TestBucket.Domain.TestAccounts.AccountTypes;

namespace TestBucket.Domain.TestAccounts.Helpers;
public static class TestAccountTemplateHelper
{
    private static readonly IAccountType[] AccountTypes = [new GenericUsernamePassword() , new WifiAccount(), new EmailAccount(), new WechatAcccount(), new GoogleAccount(), new FacebookAccount(), new OutlookAccount()];

    public static string[] Types => AccountTypes.Select(x => x.Type).ToArray();

    public static string[] GetSubTypesForAccountType(string type)
    {
        var accountType = AccountTypes.FirstOrDefault(x => x.Type == type);
        return accountType?.SubTypes ?? [];
    }

    public static FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype)
    {
        var accountType = AccountTypes.FirstOrDefault(x => x.Type == type);
        return accountType?.GetFieldDefinitionsForAccount(localizer, type, subtype) ?? [];   
    }
}
