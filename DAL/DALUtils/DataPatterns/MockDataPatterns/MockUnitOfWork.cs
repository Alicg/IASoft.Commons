namespace Utils.DAL.DataPatterns.MockDataPatterns
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class MockUnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, IList> mockSets;

        public MockUnitOfWork(Dictionary<Type, IList> mockSets)
        {
            this.mockSets = mockSets;
        }

        public List<T> GetMockSet<T>()
        {
            IList mockSet;
            this.mockSets.TryGetValue(typeof(T), out mockSet);
            if (mockSet != null) 
                return mockSet as List<T>;
            throw new Exception($"Type {typeof(T).Name} didnt exist");
        }

        public void BeginTransactionScope()
        {
        }

        public void CompleteTransactionScope()
        {
        }

        public void EndTransactionScope()
        {
        }

        public void Commit()
        {
        }

        public bool LazyLoadingEnabled { get; set; }
        public bool ProxyCreationEnabled { get; set; }
        public string ConnectionString { get; set; }
        public int ReferencesCount { get; set; }
        public int ExecuteSql(string sqlScript)
        {
            return 0;
        }

        public IEnumerable<T> ExecuteSqlQuery<T>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}
