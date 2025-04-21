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
            fields.Add(new FieldDefinition { Name = "ssid", Type = FieldType.String });
            if (subtype is WifiAccountSubTypes.Wpa2Psk or WifiAccountSubTypes.WpaPsk or WifiAccountSubTypes.Wpa3Sae)
            {
                fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });
            }
        }

        if (type == AccountTypes.Server)
        {
            fields.Add(new FieldDefinition { Name = "username", Type = FieldType.String });
            fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });
        }

        if (type == AccountTypes.Email)
        {
            if (subtype is EmailAccountTypes.Pop3 or EmailAccountTypes.Imap)
            {
                fields.Add(new FieldDefinition { Name = "smtp_server", Type = FieldType.String });
                fields.Add(new FieldDefinition { Name = "smtp_port", Type = FieldType.Integer });
            }

            fields.Add(new FieldDefinition { Name = "email", Type = FieldType.String });
            fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });

            if (subtype == EmailAccountTypes.Pop3)
            {
                fields.Add(new FieldDefinition { Name = "pop_server", Type = FieldType.String });
                fields.Add(new FieldDefinition { Name = "pop_port", Type = FieldType.Integer });
            }
            if (subtype == EmailAccountTypes.Imap)
            {
                fields.Add(new FieldDefinition { Name = "imap_server", Type = FieldType.String });
                fields.Add(new FieldDefinition { Name = "imap_port", Type = FieldType.Integer });
            }
        }
        return fields.ToArray();
    }
}
