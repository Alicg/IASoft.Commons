using System.Threading.Tasks;

namespace Utils.Network
{
    public interface IExternalConnection
    {
        Task Connect();
        void Disconnect();
        bool Connected { get; }
    }
}
