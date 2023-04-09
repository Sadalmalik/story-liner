namespace Self.Architecture.IOC
{
    public interface IShared : ISharedInterface
    {
        void Init();

        void Dispose();
    }
}