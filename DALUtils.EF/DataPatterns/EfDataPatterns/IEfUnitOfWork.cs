namespace Utils.DAL.DataPatterns.EfDataPatterns
{
    using System.Data.Entity;

    public interface IEfUnitOfWork : IUnitOfWork
    {
        DbContext Context { get; set; }
    }
}
