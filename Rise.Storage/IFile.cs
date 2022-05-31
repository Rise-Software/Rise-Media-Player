namespace Rise.Storage
{
    public interface IFile : IBaseStorage
    {
        string Extension { get; }
    }
}
