using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Issues.States;

namespace TestBucket.Integrations;

/// <summary>
/// API that an extension can use
/// </summary>
public interface IExtensionApi
{
    Task<IStateTranslator> GetStateTranslatorAsync();
}
