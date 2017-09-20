namespace RemotePlusLibrary.Extension.ExtensionLibraries.LibraryStartupTypes
{
    public interface IClientLibraryStartup
    {
        void ClientInit(ILibraryBuilder builder, IInitEnvironment env);
    }
}