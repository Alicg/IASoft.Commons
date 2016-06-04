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
        
        private static int lastDbId;
        private static int lastSetId;

        private int GetNextId()
        {
            lock (this.GetType())
            {
                int dbId;
                try
                {
                    dbId = this.All().OrderByDescending(v => v.Id).Select(v => v.Id).FirstOrDefault();
                }
                catch
                {
                    dbId = 0;
                }
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
