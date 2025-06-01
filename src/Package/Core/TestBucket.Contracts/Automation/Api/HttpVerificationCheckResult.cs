using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Automation.Api;
public record class HttpVerificationCheckResult(bool IsSuccess, string VerifierId, string PropertyId, string Operation, string? ExpectedValue, string? ActualValue, string? Error)
{
}
