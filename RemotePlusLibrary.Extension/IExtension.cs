namespace RemotePlusLibrary.Extension
{
    public interface IExtension<T> where T : ExtensionDetails
    {
        T GeneralDetails { get; }
    }
}