using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Code.Yaml.Models;

namespace TestBucket.Domain.Code.Services.IntegrationImpact;

/// <summary>
/// Contains the name of systems/layers/components/features
/// </summary>
public class CommitImpact
{
    public List<string> Systems { get; } = [];
    public List<string> Layers { get; } = [];
    public List<string> Components { get; } = [];
    public List<string> Features { get; } = [];
}
