using System;

namespace Utils.Extensions
{
    public sealed class DbFieldAttribute : Attribute
    {
        public DbFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public DbFieldAttribute(string fieldName, bool useConverter)
        {
            FieldName = fieldName;
            UseConverter = useConverter;
        }

        public string FieldName { get; private set; }
        public bool UseConverter { get; private set; }
    }
}