using Utils.Extensions;

namespace Utils.DAL.DataPatterns.EfDataPatterns
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;

    using Utils.DAL.BaseEntities;
    using Utils.DAL.EfExtensions;

    public class EfRepository<T> : IRepository<T> where T : BaseEntity, new ()
    {
        private string tableName;
        private readonly EfUnitOfWork unitOfWork;
        public IUnitOfWork UnitOfWork => this.unitOfWork;

        public EfRepository(EfUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            if (!Configuration.IsNull("EfCommandTimeout"))
            {
                ((IObjectContextAdapter)unitOfWork.Context).ObjectContext.CommandTimeout =
                    Configuration.AsInt("EfCommandTimeout");
            }

            //13.08.2013 добавил, тестирую
            this.unitOfWork.ReferencesCount++;
        }

        private IDbSet<T> objectset;

        protected IDbSet<T> ObjectSet => this.objectset ?? (this.objectset = this.unitOfWork.Context.Set<T>());

        public virtual IQueryable<T> All()
        {
            return this.ObjectSet.AsQueryable();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return this.ObjectSet.Where(expression);
        }

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return this.ObjectSet.Include(path);
        }

        public T GetById(long id)
        {
            return this.Where(e => e.Id == id).FirstOrDefault();
        }

        public virtual void Add(T entity)
        {
            this.ObjectSet.Add(entity);
        }

        public T Update(T entity)
        {
            return this.ObjectSet.Update(entity); //Extension
        }

        public virtual void AddOrUpdate(T entity)
        {
            this.ObjectSet.AddOrUpdate(entity);
        }

        public T CloneById(long id, params Expression<Func<T, object>>[] paths)
        {
            var entity = this.ObjectSet.Includes(paths).AsNoTracking().FirstOrDefault(sc => sc.Id == id);
            if (entity != null) this.ObjectSet.Add(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            this.ObjectSet.Remove(entity);
        }

        public int DeleteAll(Expression<Func<T, bool>> predicate)
        {
            return this.Where(predicate).ForEach(v => this.DeleteById(v.Id)).Count();
        }

        public virtual void DeleteById(long id)
        {
            var existingObject = this.ObjectSet.Local.FirstOrDefault(v => v.Id == id);
            if (existingObject != null)
            {
                this.ObjectSet.Remove(existingObject);
            }
            else
            {
                this.unitOfWork.Context.Database.ExecuteSqlCommand("DELETE FROM [" + this.GetTableName() + "] WHERE Id={0}", id);
            }
        }

        public virtual void DeleteByIds(params long[] ids)
        {
            foreach (var id in ids)
            {
                this.DeleteById(id);
            }
        }

        public void ExecuteSql(string sql)
        {
            this.unitOfWork.Context.Database.ExecuteSqlCommand(sql);
        }

        protected string TableName => this.tableName ?? (this.tableName = this.GetTableName());

        private string GetTableName()
        {
            return (this.unitOfWork.Context as IObjectContextAdapter).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
        }

        public void Dispose()
        {
            //13.08.2013 добавил, тестирую
            this.UnitOfWork.ReferencesCount--;
            this.UnitOfWork.Dispose();
        }
    }
}