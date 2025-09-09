using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using TestBucket.Domain.TestAccounts.Models;

namespace TestBucket.Domain.TestAccounts.AccountTypes;
public class EmailAccount : IAccountType
{
    public string Type => PrimaryAccountTypes.Email;

    public string[] SubTypes => EmailAccountTypes.All;

    public FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype)
    {
        var fields = new List<FieldDefinition>();

        fields.Add(new FieldDefinition { Name = "email", Type = FieldType.String });
        fields.Add(new FieldDefinition { Name = "password", Type = FieldType.String });

        if (subtype is EmailAccountTypes.Pop3 or EmailAccountTypes.Imap)
        {
            fields.Add(new FieldDefinition { Name = "smtp_server", Type = FieldType.String });
            fields.Add(new FieldDefinition { Name = "smtp_port", Type = FieldType.Integer });
        }
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
        return fields.ToArray();
    }
}
