namespace UnityUtils
{
    using Microsoft.Practices.Unity;

    public interface ITypesRegistrator
    {
        void RegisterAll(IUnityContainer container);
    }
}
