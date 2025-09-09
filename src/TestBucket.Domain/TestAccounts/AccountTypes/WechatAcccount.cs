using Microsoft.Extensions.Localization;

namespace TestBucket.Domain.TestAccounts.AccountTypes;
public class WechatAcccount : IAccountType
{
    public string Type => "wechat";

    public string[] SubTypes => [];

    public FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype)
    {
        var fields = new List<FieldDefinition>();

        fields.Add(new FieldDefinition { Name = "phonenumber", Type = FieldType.String });
        fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });

        return fields.ToArray();
    }
}
