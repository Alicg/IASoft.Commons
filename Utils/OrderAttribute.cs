using System;

namespace Utils
{
    public class OrderAttribute : Attribute
    {
        public OrderAttribute(int position)
        {
            Position = position;
        }

        public int Position { get; private set; }
    }
}