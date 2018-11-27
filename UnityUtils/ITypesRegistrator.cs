using Unity;

namespace UnityUtils
{
    public interface ITypesRegistrator
    {
        void RegisterAll(IUnityContainer container);
    }
}
