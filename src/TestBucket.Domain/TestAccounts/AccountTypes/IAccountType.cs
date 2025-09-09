using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

namespace TestBucket.Domain.TestAccounts.AccountTypes;
public interface IAccountType
{
    string Type { get; }
    string[] SubTypes { get; }

    FieldDefinition[] GetFieldDefinitionsForAccount(IStringLocalizer localizer, string type, string subtype);
}
