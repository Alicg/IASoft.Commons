using System.Data.Entity;
using Utils.DAL.DataPatterns.EfDataPatterns;

namespace DALUtils.SQLite.DataPatterns
{
    public class SqliteEfUnitOfWork : EfUnitOfWork
    {
        public SqliteEfUnitOfWork(DbContext context) : base(context)
        {
            context.Database.ExecuteSqlCommand("PRAGMA foreign_keys = ON;");
        }
    }
}