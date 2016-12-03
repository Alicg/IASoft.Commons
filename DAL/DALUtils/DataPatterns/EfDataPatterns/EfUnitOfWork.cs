using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;

namespace Utils.DAL.DataPatterns.EfDataPatterns
{
    public class EfUnitOfWork : IEfUnitOfWork
    {
        public DbContext Context { get; set; }

        private readonly Stack<TransactionScope> _transactionScopeStack = new Stack<TransactionScope>();

        public EfUnitOfWork(DbContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        #region TransactionScope
        public void BeginTransactionScope()
        {
            _transactionScopeStack.Push(new TransactionScope(TransactionScopeOption.RequiresNew));
        }

        public void CompleteTransactionScope()
        {
            if (_transactionScopeStack.Count > 0)
            {
                var curentTransactionScope = _transactionScopeStack.Peek();
                curentTransactionScope.Complete();
            }
        }

        public void EndTransactionScope()
        {
            if (_transactionScopeStack.Count > 0)
            {
                var curentTransactionScope = _transactionScopeStack.Pop();
                curentTransactionScope.Dispose();
            }
        }

        public void ClearTransactionStack()
        {
            while (_transactionScopeStack.Count > 0)
            {
                var curentTransactionScope = _transactionScopeStack.Pop();
                curentTransactionScope.Dispose();
            }
        }
        #endregion

        public bool LazyLoadingEnabled
        {
            get { return Context.Configuration.LazyLoadingEnabled; }
            set { Context.Configuration.LazyLoadingEnabled = value; }
        }

        public bool ProxyCreationEnabled
        {
            get { return Context.Configuration.ProxyCreationEnabled; }
            set { Context.Configuration.ProxyCreationEnabled = value; }
        }

        public string ConnectionString
        {
            get { return Context.Database.Connection.ConnectionString; }
            set { Context.Database.Connection.ConnectionString = value; }
        }

        public void Dispose()
        {
            ClearTransactionStack();
            Context.Dispose();
        }
    }
}
