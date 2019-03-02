namespace RemotePlusLibrary.VFS
{
    /// <summary>
    /// Represents a virtual file system.
    /// </summary>
    public interface IFileSystem
    {
        void AddFile<TFile>(UPath path, TFile file) where TFile : class, IFile;
        TFile GetFile<TFile>(UPath path) where TFile : class, IFile;
        void AddDirectory(UPath path);
        void DeleteFile(UPath path);
    }
}