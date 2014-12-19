using System.Collections.Generic;

namespace PropOhGate.Send
{
    public interface ISender
    {
        string RepositoryHash { get; }

        void Update(byte[] data);
        void Update(IList<byte[]> data);
    }
}
