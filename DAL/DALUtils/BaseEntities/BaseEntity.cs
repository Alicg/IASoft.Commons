namespace Utils.DAL.BaseEntities
{
    using Utils.Extensions;
    
    public abstract class BaseEntity
    {
        public long Id { get; set; }

        public T GetCopy<T>() where T : class
        {
            return this.MemberwiseClone() as T;
        }

        public virtual bool Same<T>(T other) where T : class
        {
            return other.Equals(this);
        }

        public virtual bool IsNew()
        {
            return this.Id.IsNull() || this.Id == 0;
        }
    }
}
