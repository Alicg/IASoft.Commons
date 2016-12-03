using NHibernate;

namespace Utils.DAL.DataPatterns.NhDataPatterns
{
    public interface INhUnitOfWork : IUnitOfWork
    {
        ISession Session { get; }
    }
}
