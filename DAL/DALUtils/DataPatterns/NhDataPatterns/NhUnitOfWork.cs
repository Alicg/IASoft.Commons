using System;
using System.Collections.Generic;
using System.Transactions;
using NHibernate;

namespace Utils.DAL.DataPatterns.NhDataPatterns
{
    public class NhUnitOfWork : INhUnitOfWork, IDisposable
    {
        public ISession Session { get; private set; }

        private ITransaction _transaction;

        public NhUnitOfWork(ISessionFactory sessionFactory)
        {
            Session = sessionFactory.OpenSession();
        }

        public void Commit()
        {
        }

        #region TransactionScope
        public void BeginTransactionScope()
        {
            if(_transaction != null)
                return;
            if (Session == null)
                throw new InvalidOperationException("Session has not been initialized.");
            _transaction = Session.BeginTransaction(); // переделать
        }

        public void CompleteTransactionScope()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void EndTransactionScope()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
        
        private void CloseSession()
        {
            Session.Close();
            Session.Dispose();
            Session = null;
        }

        #endregion
        
        public string ConnectionString
        {
            get { return Session.Connection.ConnectionString; }
        }

        #region IDisposable Methods
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    EndTransactionScope();
                    CloseSession();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
