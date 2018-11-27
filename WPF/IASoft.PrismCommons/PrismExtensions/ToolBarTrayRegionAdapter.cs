using System.Windows.Controls;
using System.Windows.Markup;
using Prism.Regions;

namespace IASoft.PrismCommons.PrismExtensions
{
    public class ToolBarTrayRegionAdapter : RegionAdapterBase<ToolBarTray>
    {
        public ToolBarTrayRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, ToolBarTray regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
                {
                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    {
                        foreach (var element in e.NewItems)
                        {
                            ((IAddChild)regionTarget).AddChild(element);
                        }
                    }

                    // TODO: handle remove
                };
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}