using System;

namespace Utils
{
    /// <summary>
    /// This class can be used to represent any non-text representation classes to ListBox and ComboBox controls.
    /// </summary>
    public class ListDescriptor<T>
    {
        private readonly Func<T, string> _descriptionDelegate;
        private string _description;

        public ListDescriptor(T key, string description, object tag = null)
        {
            Key = key;
            Description = description;
            Tag = tag;
        }

        public ListDescriptor(T key, Func<T, string> descriptionDelegate, object tag = null)
        {
            Key = key;
            _descriptionDelegate = descriptionDelegate;
            Tag = tag;
        }

        public T Key { get; private set; }

        public string Description
        {
            get
            {
                if (_descriptionDelegate != null)
                    return _descriptionDelegate(Key) ?? _description;
                return _description ?? Key.ToString();
            }
            set { _description = value; }
        }

        public object Tag { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}