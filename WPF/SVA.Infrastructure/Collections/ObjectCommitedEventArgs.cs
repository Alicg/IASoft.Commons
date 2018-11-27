using System;

namespace SVA.Infrastructure.Collections
{
    public class ObjectCommitedEventArgs<TSnapshot> : EventArgs
        where TSnapshot : IBaseNotificationDomainObject<TSnapshot>
    {
        public ObjectCommitedEventArgs(TSnapshot oldCommitedVersion)
        {
            this.OldCommitedVersion = oldCommitedVersion;
        }

        public TSnapshot OldCommitedVersion { get; private set; }
    }
}
