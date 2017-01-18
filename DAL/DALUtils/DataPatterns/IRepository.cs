namespace Utils.DAL.DataPatterns
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Utils.DAL.BaseEntities;

    public interface IRepository : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IRepository<T> : IRepository where T : BaseEntity, new ()
    {
        IQueryable<T> All();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path);
        T GetById(long id);
        void Add(T entity);
        T Update(T entity);
        void AddOrUpdate(T entity);
        T CloneById(long id, params Expression<Func<T, object>>[] paths);
        void Delete(T entity);
        int DeleteAll(Expression<Func<T, bool>> predicate);
        void DeleteById(long id);
        void DeleteByIds(params long[] ids);
        void ExecuteSql(string sql);
    }
}
