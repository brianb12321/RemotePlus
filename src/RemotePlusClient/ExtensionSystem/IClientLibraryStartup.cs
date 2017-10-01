using RemotePlusLibrary.Extension;

namespace RemotePlusClient.ExtensionSystem
{
    public interface IClientLibraryStartup
    {
        void ClientInit(ILibraryBuilder builder, IInitEnvironment env);
    }
}