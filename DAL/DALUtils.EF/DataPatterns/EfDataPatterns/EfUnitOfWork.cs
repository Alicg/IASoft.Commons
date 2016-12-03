namespace Utils.DAL.DataPatterns.EfDataPatterns
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Transactions;

    public class EfUnitOfWork : IEfUnitOfWork
    {
        public DbContext Context { get; set; }

        public int ReferencesCount { get; set; }

        public int ExecuteSql(string sqlScript)
        {
            return this.Context.Database.ExecuteSqlCommand(sqlScript);
        }

        public IEnumerable<T> ExecuteSqlQuery<T>(string sql, params object[] parameters)
        {
            return this.Context.Database.SqlQuery<T>(sql, parameters);
        }

        private readonly Stack<TransactionScope> transactionScopeStack = new Stack<TransactionScope>();

        public EfUnitOfWork(DbContext context)
        {
            this.Context = context;
            this.ProxyCreationEnabled = false;
        }

        public void Commit()
        {
            this.Context.SaveChanges();
        }

        #region TransactionScope

        public void BeginTransactionScope()
        {
            this.transactionScopeStack.Push(new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(0, 15, 0)));
        }

        public void CompleteTransactionScope()
        {
            if (this.transactionScopeStack.Count > 0)
            {
                var curentTransactionScope = this.transactionScopeStack.Peek();
                curentTransactionScope.Complete();
            }
        }

        public void EndTransactionScope()
        {
            if (this.transactionScopeStack.Count > 0)
            {
                var curentTransactionScope = this.transactionScopeStack.Pop();
                curentTransactionScope.Dispose();
            }
        }

        public void ClearTransactionStack()
        {
            while (this.transactionScopeStack.Count > 0)
            {
                var curentTransactionScope = this.transactionScopeStack.Pop();
                curentTransactionScope.Dispose();
            }
        }

        #endregion

        public bool LazyLoadingEnabled
        {
            get
            {
                return this.Context.Configuration.LazyLoadingEnabled;
            }
            set
            {
                this.Context.Configuration.LazyLoadingEnabled = value;
            }
        }

        public bool ProxyCreationEnabled
        {
            get
            {
                return this.Context.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Context.Configuration.ProxyCreationEnabled = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.Context.Database.Connection.ConnectionString;
            }
            set
            {
                this.Context.Database.Connection.ConnectionString = value;
            }
        }

        public void Dispose()
        {
            //13.08.2013 добавил, тестирую
            if (this.ReferencesCount == 0)
            {
                this.ClearTransactionStack();
                this.Context.Dispose();
            }
        }
    }
}
