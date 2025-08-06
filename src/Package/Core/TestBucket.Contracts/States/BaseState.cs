namespace TestBucket.Contracts.States;
public class BaseState
{
    /// <summary>
    /// Name of the state (this can be anything, defined by external systems)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Color associated with the state
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Allowed state transitions
    /// If null, it is not set and any state transition is allowed
    /// 
    /// The key represents the current state, and the list defines the possible states
    /// </summary>
    public List<string>? AllowedStates { get; set; }

    /// <summary>
    /// Is the final state
    /// </summary>
    public bool IsFinal { get; set; }

    /// <summary>
    /// Is the initial state
    /// </summary>
    public bool IsInitial { get; set; }

    /// <summary>
    /// Tries to set a mapped state from a string
    /// </summary>
    /// <param name="name"></param>
    public virtual bool SetMappedState(string name) => false;

    /// <summary>
    /// Returns an array of possible mapped state for this entity type
    /// </summary>
    /// <returns></returns>
    public virtual string[] GetMappedStates() => [];

    /// <summary>
    /// Returns the mapped state, as a string
    /// </summary>
    /// <returns></returns>
    public virtual string? GetMappedState() => null;

    public override int GetHashCode()
    {
        if(Name is not null)
        {
            return Name.GetHashCode();
        }
        return 0;
    }

    public override bool Equals(object? obj)
    {
        if(obj is BaseState baseState)
        {
            if(baseState.Name == Name)
            {
                return true;
            }
        }
        return false;
    }
}
