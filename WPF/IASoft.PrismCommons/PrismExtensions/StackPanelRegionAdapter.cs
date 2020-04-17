﻿using System.Windows;
using System.Windows.Controls;
using Prism.Regions;

namespace IASoft.PrismCommons.PrismExtensions
{
    public class StackPanelRegionAdapter : RegionAdapterBase<StackPanel>
    {
        public StackPanelRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, StackPanel regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
                {
                    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    {
                        foreach (FrameworkElement element in e.NewItems)
                        {
                            regionTarget.Children.Add(element);
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