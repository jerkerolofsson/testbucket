using System.ComponentModel;
using System.Reflection;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using ModelContextProtocol.Server;

using TestBucket.Domain.AI.Mcp.Tools;

namespace TestBucket.Domain.AI.Tools;
public class ToolCollection
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<AIFunction> _functions = [];
    private readonly Dictionary<AIFunction, bool> _enabledFunctions = [];
    private readonly Dictionary<string, List<AIFunction>> _toolsByToolName = [];

    public IList<AIFunction> Functions => _functions;

    public IEnumerable<string> ToolNames => _toolsByToolName.Keys;

    public IList<AIFunction> EnabledFunctions
    {
        get
        {
            var functions = new List<AIFunction>();

            foreach(var function in _functions)
            {
                if (_enabledFunctions.TryGetValue(function, out var isEnabled) && isEnabled)
                {
                    functions.Add(function);
                }
            }

            return functions;
        }
    }

    public ToolCollection(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Scans the provided assembly for types marked with <see cref="McpServerToolTypeAttribute"/>
    /// and adds all their methods as tools.
    /// </summary>
    /// <param name="toolAssembly">The assembly to scan for tool types.</param>
    public void AddMcpServerToolsFromAssembly(ClaimsPrincipal? preAuthenticatedUser, Assembly toolAssembly)
    {
        var toolTypes = toolAssembly.GetTypes().Where(x => x.GetCustomAttribute<McpServerToolTypeAttribute>() is not null);
        foreach (var toolType in toolTypes)
        {
            AddMcpServerToolsFromType(preAuthenticatedUser, toolType);
        }
    }

    /// <summary>
    /// Adds all methods from the specified generic type as tools.
    /// </summary>
    /// <typeparam name="T">The type whose methods will be added as tools.</typeparam>
    public void AddMcpServerToolsFromType<T>(ClaimsPrincipal? preAuthenticatedUser)
    {
        AddMcpServerToolsFromType(preAuthenticatedUser, typeof(T));
    }

    /// <summary>
    /// Adds all methods from the specified type as tools.
    /// </summary>
    /// <param name="toolType">The type whose methods will be added as tools.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="toolType"/> is not a class.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Services"/> is not initialized.</exception>
    public void AddMcpServerToolsFromType(ClaimsPrincipal? preAuthenticatedUser, Type toolType)
    {
        if (!toolType.IsClass)
        {
            throw new ArgumentException($"Expected {nameof(toolType)} to be a class");
        }

        var toolTypeAttribute = toolType.GetCustomAttribute<McpServerToolTypeAttribute>();
        if (toolTypeAttribute is null)
        {
            return;
        }

        object? instance = null;

        MethodInfo[] methods = toolType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (MethodInfo toolMethod in methods)
        {
            var serverToolAttribute = toolMethod.GetCustomAttribute<McpServerToolAttribute>();

            if (serverToolAttribute is not null)
            {
                if (toolMethod.IsStatic)
                {
                    Add(toolMethod, null);
                }
                else
                {
                    instance ??= ActivatorUtilities.CreateInstance(_serviceProvider, toolType);

                    if (instance is AuthenticatedTool auth && preAuthenticatedUser is not null)
                    {
                        auth.SetClaimsPrincipal(preAuthenticatedUser);
                    }

                    Add(toolMethod, instance);
                }
            }
        }
    }

    /// <summary>
    /// Adds a tool by wrapping the specified method and optional target instance.
    /// </summary>
    /// <param name="method">The method information to wrap.</param>
    /// <param name="target">The target object for instance methods, or <c>null</c> for static methods.</param>
    public void Add(MethodInfo method, object? target)
    {
        var serverToolAttribute = method.GetCustomAttribute<McpServerToolAttribute>();
        var descriptionAttribute = method.GetCustomAttribute<DescriptionAttribute>();

        var options = new AIFunctionFactoryOptions
        {
            Description = descriptionAttribute?.Description,
            Name = serverToolAttribute?.Name ?? method.Name,
        };

        var toolName = method.DeclaringType?.Name ?? "UnknownTool";

        if (method.DeclaringType is not null)
        {
            var displayNameAttribute = method.DeclaringType.GetCustomAttribute<DisplayNameAttribute>();
            if(displayNameAttribute?.DisplayName is not null)
            {
                toolName = displayNameAttribute.DisplayName;
            }
        }

        var function = AIFunctionFactory.Create(method, target, options);
        this.Add(toolName, function, enabled: true);
    }

    public void SetToolEnabled(string tool, bool? enabled)
    {
        if(_toolsByToolName.TryGetValue(tool, out var tools))
        {
            foreach (var function in tools)
            {
                _enabledFunctions[function] = true;
            }
        }
    }

    public int GetEnabledFunctionCount(string tool)
    {
        if (_toolsByToolName.TryGetValue(tool, out var tools))
        {
            return tools.Where(IsEnabled).Count();
        }
        return 0;
    }
    public int GetTotalFunctionCount(string tool)
    {
        if (_toolsByToolName.TryGetValue(tool, out var tools))
        {
            return tools.Count;
        }
        return 0;
    }

    public bool? IsToolEnabled(string tool)
    {
        if (_toolsByToolName.TryGetValue(tool, out var tools))
        {
            bool hasEnabled = false;
            bool hasDisabled = false;
            foreach (var function in tools)
            {
                if(!IsEnabled(function))
                {
                    hasDisabled = true;
                }
                else
                {
                    hasEnabled = true;
                }
            }

            if(hasDisabled && hasEnabled)
            {
                return null;
            }

            if (hasEnabled)
            {
                return hasEnabled;
            }
        }
        return false;
    }

    public void ToggleFunctionEnabled(AIFunction function)
    {
        if (_enabledFunctions.TryGetValue(function, out var isEnabled))
        {
            _enabledFunctions[function] = !isEnabled;
        }
        else
        {
            _enabledFunctions[function] = false;
        }
    }
    public void SetFunctionEnabled(AIFunction function, bool enabled)
    {
        _enabledFunctions[function] = enabled;
    }

    public bool IsEnabled(AIFunction function)
    {
        return _enabledFunctions.TryGetValue(function, out var isEnabled) && isEnabled;
    }

    public IReadOnlyList<AIFunction> GetFunctionsByToolName(string toolName)
    {
        if (_toolsByToolName.TryGetValue(toolName, out var tools))
        {
            return tools;
        }
        return [];
    }

    public void Add(string toolName, AIFunction function, bool enabled)
    {
        _functions.Add(function);
        _enabledFunctions[function] = enabled;
        if(!_toolsByToolName.TryGetValue(toolName, out var tools))
        {
            tools = new List<AIFunction>();
            _toolsByToolName[toolName] = tools;
        }
        tools.Add(function);
    }
}
