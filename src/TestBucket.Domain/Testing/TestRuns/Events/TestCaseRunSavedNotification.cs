using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRuns.Events;
public record class TestCaseRunSavedNotification(ClaimsPrincipal Principal, TestCaseRun TestCaseRun) : INotification;
