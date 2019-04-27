namespace RemotePlusLibrary.Core
{
    /// <summary>
    /// Represents an object which has access to the current communication channel.
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    public interface IConnectionObject
    {
        IClientContext ClientContext { get; set; }
    }
}