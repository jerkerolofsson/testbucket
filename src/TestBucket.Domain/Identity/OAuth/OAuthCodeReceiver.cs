using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.OAuth;

/// <summary>
/// Receives auth codes (from redirect) and forwards them to listeners
/// </summary>
public class OAuthCodeReceiver
{
    private readonly ConcurrentDictionary<string, object> _receivers = [];
}
