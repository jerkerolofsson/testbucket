namespace TestBucket.Domain.Resources.Models;
public enum ResourceKind
{
    /// <summary>
    /// Other kind of device
    /// </summary>
    Other = 0,

    /// <summary>
    /// A device, like an Android device
    /// </summary>
    Device = 1,

    /// <summary>
    /// A kind of account, like wifi, email..
    /// </summary>
    Account = 2,

    /// <summary>
    /// Information about a server
    /// </summary>
    Server = 3,

}
