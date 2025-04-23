using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mediator;

namespace TestBucket.Domain.Projects.Events;
public record class ProjectCreated(TestProject Project) : INotification;
