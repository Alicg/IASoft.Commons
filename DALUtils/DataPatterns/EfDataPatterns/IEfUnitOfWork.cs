using System.Data.Entity;

namespace Utils.DAL.DataPatterns.EfDataPatterns
{
    public interface IEfUnitOfWork : IUnitOfWork
    {
        DbContext Context { get; set; }
    }
}
