using SVA.Infrastructure.Collections;

namespace SVA.Infrastructure.Controls.GroupingListBox
{
    public class ElementWithGroupHeader
    {
        public ElementWithGroupHeader(GroupHeaderTypes groupHeaderType, IElementWithDate element)
        {
            this.GroupHeaderType = groupHeaderType;
            this.Element = element;
        }

        public GroupHeaderTypes GroupHeaderType { get; }

        public IElementWithDate Element { get; }
    }
}