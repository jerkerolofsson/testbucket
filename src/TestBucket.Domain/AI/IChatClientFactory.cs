using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

namespace TestBucket.Domain.AI;
public interface IChatClientFactory
{
    Task<IChatClient?> CreateChatClientAsync();
}
