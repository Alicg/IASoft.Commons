using System.Data.SQLite;
using System.Linq;

namespace DALUtils.SQLite.DataPatterns
{
    using Utils.DAL.BaseEntities;
    using Utils.DAL.DataPatterns.EfDataPatterns;

    public class SQLiteEfRepository<T> : EfRepository<T> where T : BaseEntity, new()
    {
        public SQLiteEfRepository(EfUnitOfWork unitOfWork) : base(unitOfWork)
        {
            unitOfWork.Context.Database.ExecuteSqlCommand("PRAGMA foreign_keys = ON;");
        }

        public override void Add(T entity)
        {
            if (entity.IsNew())
                entity.Id = this.GetNextId();
            base.Add(entity);
        }

        public override void AddOrUpdate(T entity)
        {
            if (entity.IsNew())
                entity.Id = this.GetNextId();
            base.AddOrUpdate(entity);
        }

        public override void DeleteById(long id)
        {
            using (var connection = new SQLiteConnection(((EfUnitOfWork)this.UnitOfWork).Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand($"DELETE FROM [{this.TableName}] WHERE Id={id}", connection);
                command.ExecuteNonQuery();
            }
        }

        public override void DeleteByIds(params long[] ids)
        {
            if (!ids.Any())
                return;
            using (var connection = new SQLiteConnection(((EfUnitOfWork)this.UnitOfWork).Context.Database.Connection.ConnectionString))
            {
                connection.Open();
                var parameters = ids.Aggregate("", (total, curr) => total + "," + curr);
                parameters = $"({parameters.Remove(0, 1)})";
                var command = new SQLiteCommand($"DELETE FROM [{this.TableName}] WHERE Id in {parameters}", connection);
                command.ExecuteNonQuery();
            }
        }

        private static long lastDbId;
        private static long lastSetId;

        private long GetNextId()
        {
            lock (this.GetType())
            {
                long dbId;
                try
                {
                    using (var connection = new SQLiteConnection(((EfUnitOfWork)this.UnitOfWork).Context.Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        var command = new SQLiteCommand($"select max(rowid) from [{this.TableName}]", connection);
                        var scalarResult = command.ExecuteScalar();
                        dbId = (long)scalarResult;
                    }
                }
                catch
                {
                    dbId = 0;
                }
                // еще не делали SaveChanges.
                if (dbId == lastDbId)
                {
                    lastSetId++;
                    return lastSetId;
                }
                lastDbId = dbId;
                return lastSetId = dbId + 1;
            }
        }
    }
}
