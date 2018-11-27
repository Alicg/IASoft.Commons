using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using SVA.Infrastructure.Collections;

namespace SVA.Infrastructure.Controls.GroupingListBox
{
    /// <summary>
    /// Interaction logic for ListBoxWithGroupHeaders.xaml
    /// </summary>
    public partial class ListBoxWithGroupHeaders : UserControl
    {
        public static readonly DependencyProperty ItemsWithDateProperty = DependencyProperty.Register(
            "ItemsWithDate",
            typeof(IReactiveCollection<IElementWithDate>),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(default(IReactiveCollection<IElementWithDate>), ItemsWithDatePropertyChanged));

        public static readonly DependencyProperty ItemsWithGroupHeaderProperty = DependencyProperty.Register(
            "ItemsWithGroupHeader",
            typeof(IReadOnlyReactiveList<ElementWithGroupHeader>),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(default(IReadOnlyReactiveList<ElementWithGroupHeader>)));

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty SelectedElementWithGroupHeaderProperty = DependencyProperty.Register(
            "SelectedElementWithGroupHeader",
            typeof(ElementWithGroupHeader),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(default(ElementWithGroupHeader), SelectedElementWithGroupHeader_PropertyChangedCallback));

        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register(
            "SelectedElement",
            typeof(IElementWithDate),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(default(IElementWithDate), SelectedElement_PropertyChangedCallback));

        public static readonly DependencyProperty ItemsPanelTemplateProperty = DependencyProperty.Register(
            "ItemsPanelTemplate",
            typeof(ItemsPanelTemplate),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(GetDefaultItemsPanelTemplate()));

        public static readonly DependencyProperty SelectedElementsProperty = DependencyProperty.Register(
            "SelectedElements",
            typeof(IList),
            typeof(ListBoxWithGroupHeaders),
            new PropertyMetadata(default(IList)));

        private static readonly DateToGroupHeaderConverter DateToGroupHeaderConverter = new DateToGroupHeaderConverter();

        public ListBoxWithGroupHeaders()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        public IReactiveCollection<IElementWithDate> ItemsWithDate
        {
            get { return (IReactiveCollection<IElementWithDate>)this.GetValue(ItemsWithDateProperty); }
            set { this.SetValue(ItemsWithDateProperty, value); }
        }

        public IElementWithDate SelectedElement
        {
            get { return (IElementWithDate)this.GetValue(SelectedElementProperty); }
            set { this.SetValue(SelectedElementProperty, value); }
        }

        public IList SelectedElements
        {
            get { return (IList)this.GetValue(SelectedElementsProperty); }
            set { this.SetValue(SelectedElementsProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        public ItemsPanelTemplate ItemsPanelTemplate
        {
            get { return (ItemsPanelTemplate)this.GetValue(ItemsPanelTemplateProperty); }
            set { this.SetValue(ItemsPanelTemplateProperty, value); }
        }

        internal IReadOnlyReactiveList<ElementWithGroupHeader> ItemsWithGroupHeader
        {
            get { return (IReadOnlyReactiveList<ElementWithGroupHeader>)this.GetValue(ItemsWithGroupHeaderProperty); }
            set { this.SetValue(ItemsWithGroupHeaderProperty, value); }
        }

        internal ElementWithGroupHeader SelectedElementWithGroupHeader
        {
            get { return (ElementWithGroupHeader)this.GetValue(SelectedElementWithGroupHeaderProperty); }
            set { this.SetValue(SelectedElementWithGroupHeaderProperty, value); }
        }

        private static void SelectedElementWithGroupHeader_PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ListBoxWithGroupHeaders)dependencyObject;
            control.SelectedElement = control.SelectedElementWithGroupHeader?.Element;
        }

        private static void SelectedElement_PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ListBoxWithGroupHeaders)dependencyObject;
            control.SelectedElementWithGroupHeader = control.ItemsWithGroupHeader?.FirstOrDefault(v => v.Element == control.SelectedElement);
        }

        private static void ItemsWithDatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ListBoxWithGroupHeaders)dependencyObject;
            control.ItemsWithGroupHeader = control.ItemsWithDate?.CreateDerivedCollection(v => new ElementWithGroupHeader(DateToGroupHeaderConverter.Convert(v.SortingDate), v));
        }
        
        private static ItemsPanelTemplate GetDefaultItemsPanelTemplate()
        {
            ItemsPanelTemplate itemsPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
            itemsPanelTemplate.Seal();
            return itemsPanelTemplate;
        }

        private void GroupedElementsItemsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.GroupedElementsItemsControl.SelectedItems.Count == 0)
            {
                this.SelectedElements = null;
                return;
            }
            if (this.SelectedElements == null)
            {
                this.SelectedElements = new ArrayList();
            }
            this.SelectedElements.Clear();
            foreach (ElementWithGroupHeader selectedItem in this.GroupedElementsItemsControl.SelectedItems)
            {
                this.SelectedElements.Add(selectedItem.Element);
            }
        }
    }
}
