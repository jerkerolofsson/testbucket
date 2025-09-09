using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.AccountTypes;
public class GenericUsernamePassword : IAccountType
{
    public string Type => PrimaryAccountTypes.Server;

    public string[] SubTypes => [];

    public FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype)
    {
        var fields = new List<FieldDefinition>();
        fields.Add(new FieldDefinition { Name = "username", Type = FieldType.String });
        fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });
        return fields.ToArray();
    }
}
