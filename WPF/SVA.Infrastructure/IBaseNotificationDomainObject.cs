using SVA.Infrastructure.Collections;

namespace SVA.Infrastructure
{
    using System;
    using System.ComponentModel;

    public interface IBaseNotificationDomainObject<TSnapshot> : INotifyPropertyChanged where TSnapshot : IBaseNotificationDomainObject<TSnapshot>
    {
        event EventHandler<ObjectCommitedEventArgs<TSnapshot>> ObjectCommited;

        long Id { get; set; }

        bool ContentCommitted { get; }

        object GetCopy();
        
        bool Same(TSnapshot other);

        void Commit();

        void Revert();
    }
}