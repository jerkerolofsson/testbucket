using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.Helpers;
public static class TestAccountTemplateHelper
{

    public static string[] GetSubTypesForAccountType(string type)
    {
        return type switch
        {
            AccountTypes.Wifi => WifiAccountSubTypes.All,
            AccountTypes.Email => EmailAccountTypes.All,
            _ => []
        };
    }

    public static FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype)
    {
        var fields = new List<FieldDefinition>();
        if (type == AccountTypes.Wifi)
        {
            fields.Add(new FieldDefinition { Name = "SSID", Type = FieldType.String });
            if (subtype is WifiAccountSubTypes.Wpa2Psk or WifiAccountSubTypes.WpaPsk or WifiAccountSubTypes.Wpa3Sae)
            {
                fields.Add(new FieldDefinition { Name = localizer["password"], Type = FieldType.String, WriteOnly = true });
            }
        }
        if (type == AccountTypes.Email)
        {
            if (subtype is EmailAccountTypes.Pop3 or EmailAccountTypes.Imap)
            {
                fields.Add(new FieldDefinition { Name = "SMTP Server", Type = FieldType.String });
                fields.Add(new FieldDefinition { Name = "SMTP Port", Type = FieldType.Integer });
            }

            fields.Add(new FieldDefinition { Name = localizer["email"], Type = FieldType.String });
            fields.Add(new FieldDefinition { Name = localizer["password"], Type = FieldType.String, WriteOnly = true });

            if (subtype == EmailAccountTypes.Pop3)
            {
                fields.Add(new FieldDefinition { Name = "POP Server", Type = FieldType.String });
                fields.Add(new FieldDefinition { Name = "POP Port", Type = FieldType.Integer });
            }
            if (subtype == EmailAccountTypes.Imap)
            {
                fields.Add(new FieldDefinition { Name = "IMAP Server", Type = FieldType.String });
                fields.Add(new FieldDefinition { Name = "IMAP Port", Type = FieldType.Integer });
            }
        }
        return fields.ToArray();
    }
}
