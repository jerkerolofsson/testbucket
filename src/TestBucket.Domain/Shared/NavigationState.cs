using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.AI.Agent;
using TestBucket.Domain.AI.Agent.Models;
using TestBucket.Domain.Code.Models;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Shared;
public class NavigationState
{
    private List<TestCase> _multiSelectedTestCases = [];
    private List<Requirement> _multiSelectedRequirements = [];

    public event EventHandler<ChatReference?>? ActiveDocumentChanged;

    /// <summary>
    /// Local issue currently in focus
    /// </summary>
    public LocalIssue? SelectedIssue { get; private set; }


    /// <summary>
    /// Linked issue currently in focus
    /// </summary>
    public LinkedIssue? SelectedLinkedIssue { get; set; }

    /// <summary>
    /// Selected requirement specification
    /// </summary>
    public RequirementSpecification? SelectedRequirementSpecification { get; private set; }

    /// <summary>
    /// Selected feature
    /// </summary>
    public Feature? SelectedFeature { get; set; }

    /// <summary>
    /// Selected feature
    /// </summary>
    public Component? SelectedComponent { get; set; }

    /// <summary>
    /// Selected requirement specification folder
    /// </summary>
    public RequirementSpecificationFolder? SelectedRequirementSpecificationFolder { get; set; }

    /// <summary>
    /// Selected requirement
    /// </summary>
    public Requirement? SelectedRequirement { get; private set; }

    /// <summary>
    /// Selected test case
    /// </summary>
    public TestCase? SelectedTestCase { get; private set; }

    /// <summary>
    /// Selected test run
    /// </summary>
    public TestRun? SelectedTestRun { get; set; }

    /// <summary>
    /// Selected test case run
    /// </summary>
    public TestCaseRun? SelectedTestCaseRun { get; set; }

    /// <summary>
    /// Selected test suite
    /// </summary>
    public TestSuite? SelectedTestSuite { get; private set; }

    /// <summary>
    /// Selected folder within a test suite
    /// </summary>
    public TestSuiteFolder? SelectedTestSuiteFolder { get; private set; }

    /// <summary>
    /// Selected folder in test repo
    /// </summary>
    public TestRepositoryFolder? SelectedTestRepositoryFolder { get; private set; }

    /// <summary>
    /// Selected folder in test lab
    /// </summary>
    public TestLabFolder? SelectedTestLabFolder { get; private set; }

    /// <summary>
    /// Selected project
    /// </summary>
    public TestProject? SelectedProject { get; set; }

    /// <summary>
    /// Selected team
    /// </summary>
    public Team? SelectedTeam { get; set; }

    public IReadOnlyList<TestCase> MultiSelectedTestCases => _multiSelectedTestCases;
    public bool IsTestLabSelected { get; private set; }
    public bool IsTestRepositorySelected { get; private set; }

    public ChatReference? ActiveDocument
    {
        get
        {
            if (SelectedTestCase is not null)
            {
                return ChatReferenceBuilder.Create(SelectedTestCase, isActiveDocument:true);
            }
            if (SelectedTestSuite is not null)
            {
                return ChatReferenceBuilder.Create(SelectedTestSuite, isActiveDocument: true);
            }
            if (SelectedRequirement is not null)
            {
                return ChatReferenceBuilder.Create(SelectedRequirement, isActiveDocument: true);
            }
            if (SelectedFeature is not null)
            {
                return ChatReferenceBuilder.Create(SelectedFeature, isActiveDocument: true);
            }
            if (SelectedComponent is not null)
            {
                return ChatReferenceBuilder.Create(SelectedComponent, isActiveDocument: true);
            }
            if (SelectedIssue is not null)
            {
                return ChatReferenceBuilder.Create(SelectedIssue, isActiveDocument: true);
            }
            return null;
        }
    }

    public void SetSelectedTestLabFolder(TestLabFolder folder)
    {
        ClearSelection();
        this.SelectedTestLabFolder = folder;
        ActiveDocumentChanged?.Invoke(this, ActiveDocument);
    }
    public void SetSelectedTestRepositoryFolder(TestRepositoryFolder folder)
    {
        ClearSelection();
        this.SelectedTestRepositoryFolder = folder;
    }
    public void SelectTestRepository()
    {
        ClearSelection();
        IsTestRepositorySelected = true;
    }
    public void SelectTestLab()
    {
        ClearSelection();
        IsTestLabSelected = true;
    }

    public void SetSelectedComponent(Component component)
    {
        ClearSelection();
        SelectedComponent = component;
        ActiveDocumentChanged?.Invoke(this, ActiveDocument);
    }
    public void SetSelectedFeature(Feature feature)
    {
        ClearSelection();
        SelectedFeature = feature;
        ActiveDocumentChanged?.Invoke(this, ActiveDocument);
    }
    public void ClearSelection()
    {
        IsTestLabSelected = false;
        IsTestRepositorySelected = false;

        this.SelectedFeature = null;
        this.SelectedComponent = null;

        this.SelectedTestRepositoryFolder = null;
        this.SelectedTestSuite = null;
        this.SelectedTestSuiteFolder = null;
        this.SelectedTestCase = null;

        this.SelectedTestRun = null;
        this.SelectedTestLabFolder = null;

        this.SelectedRequirement = null;
        this.SelectedRequirementSpecification = null;
        this.SelectedRequirementSpecificationFolder = null;

        this.SelectedIssue = null;
    }
    public void SetMultiSelectedRequirements(List<Requirement> list)
    {
        _multiSelectedTestCases = [];
        _multiSelectedRequirements = list;
    }
    public void SetMultiSelectedTestCases(List<TestCase> list)
    {
        _multiSelectedTestCases = list;
        _multiSelectedRequirements = [];
    }

    public void SetMultiSelectedTestSuiteFolders(List<TestSuiteFolder> list)
    {
    }

    public void SetMultiSelectedTestSuites(List<TestSuite> list)
    {
    }

    public void SetSelectedRequirement(Requirement? requirement)
    {
        ClearSelection();
        SelectedRequirement = requirement;
        ActiveDocumentChanged?.Invoke(this, ActiveDocument);
    }
    public void SetSelectedRequirementSpecificationFolder(RequirementSpecificationFolder? folder, RequirementSpecification? spec)
    {
        ClearSelection();
        SelectedRequirementSpecificationFolder = folder;
        SelectedRequirementSpecification = spec;
    }
    public void SetSelectedRequirementSpecificationFolder(RequirementSpecificationFolder? folder)
    {
        ClearSelection();
        SelectedRequirementSpecificationFolder = folder;
    }
    public void SetSelectedRequirementSpecification(RequirementSpecification? spec)
    {
        ClearSelection();
        SelectedRequirementSpecification = spec;
    }
    public void SetSelectedRequirement(Requirement? requirement, RequirementSpecificationFolder? folder, RequirementSpecification? requirementSpecification)
    {
        ClearSelection();
        SelectedRequirement = requirement;
        SelectedRequirementSpecificationFolder = folder;
        SelectedRequirementSpecification = requirementSpecification;
    }
    public void SetSelectedRequirement(RequirementSpecification? requirementSpecification)
    {
        ClearSelection();
        SelectedRequirementSpecification = requirementSpecification;
    }

    public void SetSelectedTestSuite(TestSuite? suite)
    {
        ClearSelection();
        SelectedTestSuite = suite;
        ActiveDocumentChanged?.Invoke(this, ActiveDocument);
    }
    public void SetSelectedTestSuiteFolder(TestSuiteFolder? folder, TestSuite? suite)
    {
        ClearSelection();
        SelectedTestSuiteFolder = folder;
        SelectedTestSuite = suite;
    }
    public void SetSelectedTestSuiteFolder(TestSuiteFolder? folder)
    {
        SetSelectedTestSuiteFolder(folder, SelectedTestSuite);
    }
    public void SetSelectedTestCase(TestCase? testCase)
    {
        ClearSelection();
        SelectedTestCase = testCase;
        ActiveDocumentChanged?.Invoke(this, ActiveDocument);
    }
    public void SetSelectedTestCase(TestCase? testCase, TestSuiteFolder? folder, TestSuite? suite)
    {
        ClearSelection();
        SelectedTestCase = testCase;
        SelectedTestSuiteFolder = folder;
        SelectedTestSuite = suite;
    }

    public void SetSelectedTestCaseRun(TestRun run, TestCaseRun testCaseRun)
    {
        ClearSelection();
        SelectedTestCaseRun = testCaseRun;
        SelectedTestRun = run;
    }
    public void SetSelectedTestCaseRun(TestCaseRun testCaseRun)
    {
        ClearSelection();
        SelectedTestCaseRun = testCaseRun;
    }
    public void SetSelectedTestRun(TestRun testRun)
    {
        ClearSelection();
        SelectedTestRun = testRun;
    }
    public void SetSelectedIssue(LocalIssue issue)
    {
        ClearSelection();
        SelectedIssue = issue;
    }
}
