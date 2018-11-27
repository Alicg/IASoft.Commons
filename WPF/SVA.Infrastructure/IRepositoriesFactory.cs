namespace SVA.Infrastructure
{
    using Utils.DAL.BaseEntities;
    using Utils.DAL.DataPatterns;

    public interface IRepositoriesFactory
    {
        IRepository<T> CreateRepository<T>() where T : BaseEntity, new();

        IRepository<T> CreateRepository<T>(IUnitOfWork unitOfWork) where T : BaseEntity, new();
    }
}
