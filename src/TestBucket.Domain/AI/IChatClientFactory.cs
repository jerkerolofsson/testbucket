using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.Domain.AI.Models;

namespace TestBucket.Domain.AI;
public interface IChatClientFactory
{
    Task<IChatClient?> CreateChatClientAsync(ClaimsPrincipal principal, ModelType modelType);
    Task<string?> GetModelNameAsync(ClaimsPrincipal principal, ModelType modelType);
}
