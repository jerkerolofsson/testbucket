﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Requirements.Models;
[Table("requirement_test_links")]
public class RequirementTestLink : Entity
{
    /// <summary>
    /// Database ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// FK for requirement
    /// </summary>
    public required long RequirementId { get; set; }

    /// <summary>
    /// External requirement ID / slug
    /// </summary>
    public string? RequirementExternalId { get; set; }

    /// <summary>
    /// FK for test case
    /// </summary>
    public required long TestCaseId { get; set; }

    /// <summary>
    /// Specification
    /// </summary>
    public long? RequirementSpecificationId { get; set; }

    // Navigation

    public Requirement? Requirement { get; set; }
    public RequirementSpecification? RequirementSpecification { get; set; }
    public TestCase? TestCase { get; set; }
}
