using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReactiveUI;
using SVA.Infrastructure.Collections;

namespace SVA.Infrastructure.Controls.GroupingListBox
{
    /// <summary>
    /// Interaction logic for DataGridWithGroupHeaders.xaml
    /// </summary>
    public class DataGridWithGroupHeaders : ItemsControl
    {
        public const string ElementsGridName = "PART_GroupedElementsGrid";
        public const string ScrollName = "PART_ScrollViewer"; 

        public static readonly DependencyProperty ItemsWithDateProperty = DependencyProperty.Register(
            "ItemsWithDate",
            typeof(IReadOnlyReactiveList<IElementWithDate>),
            typeof(DataGridWithGroupHeaders),
            new PropertyMetadata(default(IReadOnlyReactiveList<IElementWithDate>), ItemsWithDatePropertyChanged));

        public static readonly DependencyProperty ItemsWithGroupHeaderProperty = DependencyProperty.Register(
            "ItemsWithGroupHeader",
            typeof(IReadOnlyReactiveList<ElementWithGroupHeader>),
            typeof(DataGridWithGroupHeaders),
            new PropertyMetadata(default(IReadOnlyReactiveList<ElementWithGroupHeader>)));

        public static readonly DependencyProperty SelectedElementWithGroupHeaderProperty = DependencyProperty.Register(
            "SelectedElementWithGroupHeader",
            typeof(ElementWithGroupHeader),
            typeof(DataGridWithGroupHeaders),
            new PropertyMetadata(default(ElementWithGroupHeader), SelectedElementWithGroupHeader_PropertyChangedCallback));

        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register(
            "SelectedElement",
            typeof(IElementWithDate),
            typeof(DataGridWithGroupHeaders),
            new PropertyMetadata(default(IElementWithDate), SelectedElement_PropertyChangedCallback));

        public static readonly DependencyProperty SelectedElementsProperty = DependencyProperty.Register(
            "SelectedElements",
            typeof(IList<IElementWithDate>),
            typeof(DataGridWithGroupHeaders),
            new PropertyMetadata(new List<IElementWithDate>()));

        private static readonly DateToGroupHeaderConverter DateToGroupHeaderConverter = new DateToGroupHeaderConverter();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var elementsGrid = this.GetTemplateChild(ElementsGridName) as DataGrid;
            if (elementsGrid != null)
            {
                elementsGrid.SelectionChanged += this.GroupedElementsGrid_OnSelectionChanged;
                this.ActualizeColumnsBindings();
                elementsGrid.Columns.AddRange(this.Columns);
                elementsGrid.InputBindings.AddRange(this.InputBindings);
                elementsGrid.ContextMenu = this.ContextMenu;
            }
        }
        
        public ObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollection<DataGridColumn>();

        public IReadOnlyReactiveList<IElementWithDate> ItemsWithDate
        {
            get { return (IReadOnlyReactiveList<IElementWithDate>)this.GetValue(ItemsWithDateProperty); }
            set { this.SetValue(ItemsWithDateProperty, value); }
        }

        public IElementWithDate SelectedElement
        {
            get { return (IElementWithDate)this.GetValue(SelectedElementProperty); }
            set { this.SetValue(SelectedElementProperty, value); }
        }

        public IList<IElementWithDate> SelectedElements
        {
            get { return (IList<IElementWithDate>)this.GetValue(SelectedElementsProperty); }
            set { this.SetValue(SelectedElementsProperty, value); }
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
            var control = (DataGridWithGroupHeaders)dependencyObject;
            control.SelectedElement = control.SelectedElementWithGroupHeader?.Element;
        }

        private static void SelectedElement_PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (DataGridWithGroupHeaders)dependencyObject;
            control.SelectedElementWithGroupHeader = control.ItemsWithGroupHeader?.FirstOrDefault(v => v.Element == control.SelectedElement);
        }

        private static void ItemsWithDatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (DataGridWithGroupHeaders)dependencyObject;
            control.ItemsWithGroupHeader = control.ItemsWithDate?.CreateDerivedCollection(
                v => new ElementWithGroupHeader(DateToGroupHeaderConverter.Convert(v.SortingDate), v),
                null,
                (left, right) => left.Element.SortingDate.CompareTo(right.Element.SortingDate));

            var elementsGrid = control.GetTemplateChild(ElementsGridName) as DataGrid;
            if (elementsGrid != null)
            {
                var cv = (CollectionView)CollectionViewSource.GetDefaultView(elementsGrid.ItemsSource);
                var groupDescription = new PropertyGroupDescription("GroupHeaderType");
                var sortDescription = new SortDescription("Element.SortingDate", ListSortDirection.Descending);
                cv.GroupDescriptions.Add(groupDescription);
                cv.SortDescriptions.Add(sortDescription);
            }
        }

        private void GroupedElementsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var elementsGrid = sender as DataGrid;
            if (elementsGrid == null || elementsGrid.SelectedItems.Count == 0)
            {
                this.SelectedElements = null;
                return;
            }
            if (this.SelectedElements == null)
            {
                this.SelectedElements = new List<IElementWithDate>();
            }
            this.SelectedElements.Clear();
            foreach (ElementWithGroupHeader selectedItem in elementsGrid.SelectedItems)
            {
                this.SelectedElements.Add(selectedItem.Element);
            }
            this.SelectedElement = this.SelectedElements.FirstOrDefault();
        }

        private void ActualizeColumnsBindings()
        {
            foreach (var dataGridTextColumn in this.Columns.OfType<DataGridTextColumn>())
            {
                var currentBinding = dataGridTextColumn.Binding as Binding;
                if (currentBinding != null)
                {
                    if (!currentBinding.Path.Path.Contains("Element"))
                    {
                        currentBinding.Path.Path = $"Element.{currentBinding.Path.Path}";
                    }
                }
                else
                {
                    var currentMultiBinding = dataGridTextColumn.Binding as MultiBinding;
                    foreach (Binding binding in currentMultiBinding.Bindings)
                    {
                        if (!binding.Path.Path.Contains("Element"))
                        {
                            binding.Path.Path = $"Element.{binding.Path.Path}";
                        }
                    }
                }
            }
        }
    }
}