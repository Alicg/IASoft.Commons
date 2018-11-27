using System;
using System.Linq;
using System.Reactive.Linq;
using IASoft.WPFCommons;
using IASoft.WPFCommons.Reactive;
using Prism.Regions;

namespace IASoft.PrismCommons.PrismExtensions
{
    public class ClosePageViewModelRegionBehavior : RegionBehavior
    {
        protected override void OnAttach()
        {
            this.Region.Views.ObservableFromCollectionChanged()
                .SelectMany(v => v.OldItems?.OfType<IPageViewModel>() ?? new IPageViewModel[0])
                .Subscribe(v => v.Close());
        }
    }
}