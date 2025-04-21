using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Events;

public record TestCaseCreatedEvent(TestCase Test) : INotification;
public record TestCaseDeletedEvent(TestCase Test) : INotification;

public record TestSuiteCreatedEvent(TestSuite Suite) : INotification;
public record TestSuiteDeletedEvent(TestSuite Suite) : INotification;
