using TestBucket.Components.Reporting.Models;
using TestBucket.Contracts.Appearance;
using TestBucket.Contracts.Insights;
using TestBucket.Domain.Insights.Model;
using TestBucket.Domain.Issues.Insights;
using TestBucket.Domain.Testing.TestCases.Insights;
using TestBucket.Domain.Testing.TestRuns.Insights;

namespace TestBucket.Domain.Insights;
internal class DashboardManager : IDashboardManager
{
    private readonly TimeProvider _timeProvider;
    private readonly IDashboardRepository _repository;

    public DashboardManager(IDashboardRepository repository, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<Dashboard?> GetDashboardByNameAsync(ClaimsPrincipal principal, long projectId, string name)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Dashboard, PermissionLevel.Read);

        var tenantId = principal.GetTenantIdOrThrow();
        var dashboard = await _repository.GetDashboardByNameAsync(tenantId, projectId, name);

        // If it does not exist, create a default one
        if(dashboard is null)
        {
            if (name == "issues")
            {
                dashboard = GetDefaultIssueDashboard();
            }
            else if (name == "testrun")
            {
                dashboard = GetDefaultTestRunDashboard();
            }
            else if (name == "reporting")
            {
                dashboard = GetDefaultTestResultsDashboard();
            }

            if (dashboard is not null)
            {
                dashboard.TestProjectId = projectId;
                dashboard.Name = name;
                await AddDashboardAsync(principal, dashboard);
            }
        }

        return dashboard;
    }

    private async Task CreateDefaultDashboardsAsync(ClaimsPrincipal principal, long projectId)
    {
        await GetDashboardByNameAsync(principal, projectId, "issues");
        await GetDashboardByNameAsync(principal, projectId, "testrun");
    }


    private Dashboard? GetDefaultProductStateDashboard()
    {
        return new Dashboard
        {
            Name = "product-state",
            Specifications =
            [
                new InsightsVisualizationSpecification
                {
                    Name = "count-per-testcategory",
                    AllowedChartTypes = ChartType.Bar | ChartType.Donut |ChartType.Pie | ChartType.Text,
                    DataQueries = [new InsightsDataQuery
                    {
                        DataSource = TestCaseDataSourceNames.CountByCategory,
                    }],
                    ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ChartType = ChartType.Donut,
                    ShowLegend = true,
                    ShowDataTable = false,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "latest-test-results",
                    ChartType = ChartType.Bar,
                    AllowedChartTypes = ChartType.Bar | ChartType.Donut | ChartType.Line | ChartType.Pie | ChartType.Text,
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.CountByLatestResult,
                            Colors = DefaultPalettes.TestResultColors,
                            Query = ""
                        }],
                    ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                }
            ]
        };
    }

    private Dashboard? GetDefaultTestResultsDashboard()
    {
        return new Dashboard
        {
            Name = "test-results",
            Specifications =
            [
                new InsightsVisualizationSpecification
                {
                    Name = "test-results",
                    ChartType = ChartType.Donut,
                    AllowedChartTypes = ChartType.Bar|ChartType.Donut|ChartType.Line|ChartType.Pie|ChartType.Text,
                    DataQueries = [new InsightsDataQuery
                    {
                        DataSource = TestRunDataSourceNames.CountByResult,
                        Colors = DefaultPalettes.TestResultColors,
                        Query = ""
                    }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "test-results-by-user",
                    ChartType = ChartType.Bar,
                    AllowedChartTypes = ChartType.Bar | ChartType.Donut | ChartType.Pie,
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.ExecutedTestsByAssignee
                        }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "test-results-by-component",
                    ChartType = ChartType.StackedBar,
                    AllowedChartTypes = ChartType.StackedBar,
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.ResultsByComponent,
                        }],
                        ColorMode = ChartColorMode.BySeries,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                }
            ]
        };
    }

    private Dashboard? GetDefaultTestRunDashboard()
    {
        return new Dashboard
        {
            Name = "testrun",
            Specifications =
            [
                new InsightsVisualizationSpecification
                {
                    Name = "test-results",
                    ChartType = ChartType.Donut,
                    AllowedChartTypes = ChartType.Bar|ChartType.Donut|ChartType.Line|ChartType.Pie|ChartType.Text,
                    DataQueries = [new InsightsDataQuery
                    {
                        DataSource = TestRunDataSourceNames.CountByResult,
                        Colors = DefaultPalettes.TestResultColors,
                        Query = ""
                    }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "test-results-by-user",
                    ChartType = ChartType.Bar,
                    AllowedChartTypes = ChartType.Bar | ChartType.Donut | ChartType.Pie,
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.ExecutedTestsByAssignee
                        }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "test-results-by-component",
                    ChartType = ChartType.StackedBar,
                    AllowedChartTypes = ChartType.StackedBar,
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.ResultsByComponent,
                        }],
                        ColorMode = ChartColorMode.BySeries,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = true,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "passed",
                    ChartType = ChartType.Text,
                    AllowedChartTypes = ChartType.Text,
                    Field = "Passed",
                    TextFormat = "{0}",
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.CountByResult,
                        }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = false,
                    ShowDataTable = false,
                    Columns = 5,
                    Rows = 8,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "code-coverage",
                    ChartType = ChartType.Text,
                    AllowedChartTypes = ChartType.Text,
                    Field = "Code Coverage",
                    TextFormat = "{0}%",
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = TestRunDataSourceNames.CodeCoverage,
                        }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = false,
                    ShowDataTable = false,
                    Columns = 5,
                    Rows = 7,
                },
            ]
        };
    }

    private Dashboard? GetDefaultIssueDashboard()
    {
        return new Dashboard
        {
            Name = "issues",
            Specifications = 
            [
                new InsightsVisualizationSpecification
                {
                    Name = "issue-inflow-outflow",
                    ChartType = ChartType.ActivityHeatmap,
                    AllowedChartTypes = ChartType.Line | ChartType.ActivityHeatmap,
                    DataQueries = [new InsightsDataQuery
                    {
                        DataSource = IssueDataSourceNames.IssuesInflowOutflow,
                        Query = ""
                    }],
                        ColorMode = ChartColorMode.BySeries,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = false,
                    ShowDataTable = false,
                },
                new InsightsVisualizationSpecification
                {
                    Name = "issues-by-state",
                    AllowedChartTypes = ChartType.Bar | ChartType.Donut | ChartType.Pie | ChartType.Text,
                    DataQueries = [new InsightsDataQuery
                        {
                            DataSource = IssueDataSourceNames.IssuesByState,
                            Colors = new Dictionary<string, string> {
                                ["Open"]="#06D6A0",
                                ["Closed"]="#EF476F",
                                ["Canceled"]="#a4133c",
                            },
                            Query = ""
                        }],
                        ColorMode = ChartColorMode.ByLabel,
                    DarkModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#444",
                        TickLabelColor = "#777"
                    },
                    LightModeColors = new ChartColors()
                    {
                        Palette = DefaultPalettes.ReportingDefault,
                        GridLineColor = "#ddd",
                        TickLabelColor = "#aaa"
                    },
                    ShowLegend = true,
                    ShowDataTable = false,
                }
            ]
        };
    }

    public async Task<Dashboard?> GetDashboardAsync(ClaimsPrincipal principal, long id)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Dashboard, PermissionLevel.Read);

        var tenantId = principal.GetTenantIdOrThrow();
        return await _repository.GetDashboardAsync(tenantId, id);
    }

    public async Task<IEnumerable<Dashboard>> GetAllDashboardsAsync(ClaimsPrincipal principal, long projectId)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Dashboard, PermissionLevel.Read);

        var tenantId = principal.GetTenantIdOrThrow();
        var dashboards = (await _repository.GetAllDashboardsAsync(projectId)).ToList();

        if (dashboards.Count == 0)
        {
            await CreateDefaultDashboardsAsync(principal, projectId);
        }
        return dashboards;
    }

    public async Task AddDashboardAsync(ClaimsPrincipal principal, Dashboard dashboard)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Dashboard, PermissionLevel.Write);

        var tenantId = principal.GetTenantIdOrThrow();
        dashboard.TenantId = tenantId;
        dashboard.Created = dashboard.Modified = _timeProvider.GetUtcNow();
        dashboard.CreatedBy = dashboard.ModifiedBy = principal.Identity?.Name ?? throw new ArgumentException("ClaimsPrincipal has no identity name");

        await _repository.AddDashboardAsync(dashboard);
    }

    public async Task UpdateDashboardAsync(ClaimsPrincipal principal, Dashboard dashboard)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Dashboard, PermissionLevel.Write);

        var tenantId = principal.GetTenantIdOrThrow();
        dashboard.TenantId = tenantId;
        dashboard.Modified = _timeProvider.GetUtcNow();
        dashboard.ModifiedBy = principal.Identity?.Name ?? throw new ArgumentException("ClaimsPrincipal has no identity name");

        await _repository.UpdateDashboardAsync(dashboard);
    }

    public async Task DeleteDashboardAsync(ClaimsPrincipal principal, long id)
    {
        principal.ThrowIfNoPermission(PermissionEntityType.Dashboard, PermissionLevel.Delete);

        var tenantId = principal.GetTenantIdOrThrow();
        await _repository.DeleteDashboardAsync(tenantId, id);
    }
}

