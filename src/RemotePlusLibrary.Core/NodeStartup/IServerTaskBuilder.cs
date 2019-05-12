namespace RemotePlusLibrary.Core.NodeStartup
{
    /// <summary>
    /// Provides a way to plug in initialization steps when the server starts.
    /// </summary>
    public interface IServerTaskBuilder : INodeBuilder<IServerTaskBuilder>
    {
        
    }
}
