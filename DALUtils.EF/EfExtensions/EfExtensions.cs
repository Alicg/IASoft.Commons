namespace Utils.DAL.EfExtensions
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;

    using Utils.DAL.BaseEntities;
    using Utils.Extensions;

    public static class EfExtensions
    {
        public static T Update<T>(this IDbSet<T> dbSet, T entity, bool? ignoreNulls = null) where T : BaseEntity
        {
            ignoreNulls = ignoreNulls ?? true;
            T result;
            if(!TryUpdate(dbSet, entity, out result, ignoreNulls))
                throw new Exception($"Не удалось найти элемент '{dbSet.ElementType.Name}' по указанному коду: {entity.Id}");
            return result;
        }

        public static bool TryUpdate<T>(this IDbSet<T> dbSet, T entity, out T result, bool? ignoreNulls = null) where T : BaseEntity
        {
            ignoreNulls = ignoreNulls ?? true;
            result = dbSet.Find(entity.Id);
            if (result != null)
            {
                entity.CopyAllTo(result, ignoreNulls.Value);
                return true;
            }
            return false;
        }

        public static IQueryable<T> Includes<T>(this IQueryable<T> source, params Expression<Func<T, object>>[] paths)
            where T : class
        {
            return paths.Aggregate(source, (current, path) => current.Include(path));
        }

        public static string GetTableName<T>(this DbContext context) where T : class
        {
            return (context as IObjectContextAdapter).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
        }
    }
}
