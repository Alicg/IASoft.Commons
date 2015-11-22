using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using Utils.DAL.BaseEntities;

namespace Utils.DAL.DataPatterns.NhDataPatterns
{
    public class NhRepository<T> : IRepository<T> where T : BaseEntity, new ()
    {
        public readonly INhUnitOfWork _unitOfWork;

        protected ISession Session
        {
            get { return _unitOfWork.Session; }
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public NhRepository(INhUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private IQueryable<T> _objectset;

        private IQueryable<T> ObjectSet
        {
            get { return _objectset ?? (_objectset = _unitOfWork.Session.Query<T>()); }
        }

        public virtual IQueryable<T> All()
        {
            return ObjectSet;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return ObjectSet.Where(expression);
        }

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {//на 28.07.2013 не актуально. Скорее всего nh автоматически втянет свойство
            return ObjectSet;
        }

        public T GetById(int id)
        {
            return Session.Get<T>(id);
            //return ObjectSet.FirstOrDefault(e => e.Id == id);
        }

        public void Add(T entity)
        {
            Session.SaveOrUpdate(entity);
            Session.Refresh(entity);
        }

        public void Update(T entity)
        {
            Session.SaveOrUpdate(entity);
            Session.Refresh(entity);
        }

        public void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public void DeleteById(int id)
        {
            var queryString = string.Format("delete {0} where Id = {1}",
                                           typeof(T), id);
            Session.CreateQuery(queryString)
                   .ExecuteUpdate();
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}