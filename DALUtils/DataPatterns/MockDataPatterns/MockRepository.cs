namespace Utils.DAL.DataPatterns.MockDataPatterns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Utils.DAL.BaseEntities;
    using Utils.Extensions;

    public class MockRepository<T> : IRepository<T>
        where T : BaseEntity, new()
    {
        private readonly MockUnitOfWork unitOfWork;

        public IUnitOfWork UnitOfWork => this.unitOfWork;

        public int ReferencesCount { get; set; }

        private List<T> objectSet;

        public MockRepository(MockUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private List<T> ObjectSet => this.objectSet ?? (this.objectSet = this.unitOfWork.GetMockSet<T>());

        public IQueryable<T> All()
        {
            return this.ObjectSet.AsQueryable();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return this.ObjectSet.Where(expression.Compile()).AsQueryable();
        }

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return this.ObjectSet.AsQueryable();
        }

        public T GetById(long id)
        {
            return this.ObjectSet.FirstOrDefault(e => e.Id == id);
        }

        public void Add(T entity)
        {
            this.ObjectSet.Add(entity);
        }

        public T Update(T entity)
        {
            var tmp = this.ObjectSet.FirstOrDefault(e => e.Id == entity.Id);
            entity.CopyAllTo(tmp);
            return tmp;
        }

        public void AddOrUpdate(T entity)
        {
            if (entity.Id == 0) this.Add(entity);
            else
            {
                this.Update(entity);
            }
        }

        public T CloneById(long id, params Expression<Func<T, object>>[] paths)
        {
            var entity = this.ObjectSet.FirstOrDefault(e => e.Id == id);
            if (entity != null) this.ObjectSet.Add(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            this.ObjectSet.Remove(entity);
        }

        public void DeleteAll()
        {
            this.ObjectSet.Clear();
        }

        public int DeleteAll(Expression<Func<T, bool>> predicate)
        {
            return this.ObjectSet.RemoveAll(v => predicate.Compile()(v));
        }

        public void DeleteById(long id)
        {
            this.ObjectSet.RemoveAll(e => e.Id == id);
        }

        public void DeleteByIds(params long[] ids)
        {
            this.ObjectSet.RemoveAll(e => ids.Contains(e.Id));
        }

        public void ExecuteSql(string sql)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (this.ReferencesCount == 0) this.UnitOfWork.Dispose();
        }
    }
}
