using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.AccountTypes;
public class WifiAccount : IAccountType
{
    public string Type => PrimaryAccountTypes.Wifi;

    public string[] SubTypes => WifiAccountSubTypes.All;

    public FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype)
    {
        var fields = new List<FieldDefinition>();

        fields.Add(new FieldDefinition { Name = "ssid", Type = FieldType.String });

        switch (subtype)
        {
            case WifiAccountSubTypes.Open:
            case WifiAccountSubTypes.Wpa3Owe:
                break;
            default:
                fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });
                break;
        }

        return fields.ToArray();
    }
}
