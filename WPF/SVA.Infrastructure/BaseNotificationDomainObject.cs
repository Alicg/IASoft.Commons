using System;
using Prism.Mvvm;
using SVA.Infrastructure.Collections;

namespace SVA.Infrastructure
{
    public abstract class BaseNotificationDomainObject<TSnapshot> : BindableBase, IBaseNotificationDomainObject<TSnapshot>
        where TSnapshot : IBaseNotificationDomainObject<TSnapshot>
    {
        private bool contentCommitted;

        protected TSnapshot CommitedSnapshotObject;

        protected BaseNotificationDomainObject(long id)
        {
            this.Id = id;
        }

        public event EventHandler<ObjectCommitedEventArgs<TSnapshot>> ObjectCommited;

        /// <remarks>
        /// Id используется в GetHashCode.
        /// </remarks>
        public long Id { get; set; }

        public bool ContentCommitted
        {
            get { return this.contentCommitted; }
            protected set { this.SetProperty(ref this.contentCommitted, value); }
        }

        public virtual object GetCopy()
        {
            return (TSnapshot)this.MemberwiseClone();
        }

        public abstract bool Same(TSnapshot other);

        public virtual void Commit()
        {
            var oldVersion = this.CommitedSnapshotObject;
            this.CommitedSnapshotObject = (TSnapshot)this.GetCopy();
            this.ContentCommitted = true;
            this.OnObjectCommitedEventHandler(oldVersion);
        }

        public virtual void Revert()
        {
            this.ContentCommitted = true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((TSnapshot)obj);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + this.Id.GetHashCode();
            return hash;
        }

        protected bool Equals(TSnapshot other)
        {
            return this.Id == other.Id;
        }

        protected virtual void OnObjectCommitedEventHandler(TSnapshot versionBeforeCommit)
        {
            this.ObjectCommited?.Invoke(this, new ObjectCommitedEventArgs<TSnapshot>(versionBeforeCommit));
        }
    }
}
