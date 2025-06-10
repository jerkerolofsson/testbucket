using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats.Dtos;

namespace TestBucket.Contracts.Testing.Models;

/// <summary>
/// A representation of test suites and test cases
/// </summary>
public class TestRepositoryDto
{
    public List<TestSuiteDto> TestSuites { get; set; } = [];
    public List<TestCaseDto> TestCases { get; set; } = [];
}
