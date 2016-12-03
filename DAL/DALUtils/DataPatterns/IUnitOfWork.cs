namespace Utils.DAL.DataPatterns
{
    using System;
    using System.Collections.Generic;

    public interface IUnitOfWork : IDisposable
    {
        void BeginTransactionScope();
        void CompleteTransactionScope();
        void EndTransactionScope();
        void Commit();
        string ConnectionString { get; }
        int ReferencesCount { get; set; }
        int ExecuteSql(string sqlScript);
        IEnumerable<T> ExecuteSqlQuery<T>(string sql, params object[] parameters);
    }
}
