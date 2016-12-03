using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Utils.DAL.BaseEntities;
using Utils.DAL.EfExtensions;
using Utils.DALUtils.EfExtensions;

namespace Utils.DAL.DataPatterns.EfDataPatterns
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity, new ()
    {
        private readonly EfUnitOfWork _unitOfWork;
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public EfRepository(EfUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private IDbSet<T> _objectset;

        private IDbSet<T> ObjectSet
        {
            get
            {
                if (_objectset == null)
                {
                    _objectset = _unitOfWork.Context.Set<T>();
                }
                return _objectset;
            }
        }

        public virtual IQueryable<T> All()
        {
            return ObjectSet.AsQueryable();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return ObjectSet.Where(expression);
        }

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            return ObjectSet.Include(path);
        }

        public T GetById(int id)
        {
            return Where(e => e.Id == id).FirstOrDefault();
        }

        public void Add(T entity)
        {
            ObjectSet.Add(entity);
        }

        public void Update(T entity)
        {
            ObjectSet.Update(entity); //Extension
        }

        public void Delete(T entity)
        {
            ObjectSet.Remove(entity);
        }

        public void DeleteById(int id)
        {
            var tableName = (_unitOfWork.Context as IObjectContextAdapter).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
            _unitOfWork.Context.Database.ExecuteSqlCommand("DELETE FROM [" + tableName + "] WHERE Id={0}", id);
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}