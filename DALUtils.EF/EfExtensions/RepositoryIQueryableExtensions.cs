namespace Utils.DAL.EfExtensions
{
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    internal static class RepositoryIQueryableExtensions
    {
        public static IQueryable<T> Include<T>
            (this IQueryable<T> source, string path)
        {
            var dbQuery = source as DbQuery<T>;
            if (dbQuery != null)
            {
                return dbQuery.Include(path);
            }
            return source;
        }
    }
}