// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/Autumn/tree/master/Autumn.IOC

namespace Self.Architecture.IOC
{
    public class SharedObject : IShared
    {
        [Inject] public Container container;

        public virtual void Init()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}