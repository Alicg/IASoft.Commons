using System;
using IASoft.WPFCommons;
using Prism.Interactivity.InteractionRequest;

namespace IASoft.PrismCommons.PrismExtensions
{
    public class CustomPopupWindow : INotification
    {
        public CustomPopupWindow(string title, IPageViewModel pageViewModel)
        {
            this.Title = title;
            this.PageViewModel = pageViewModel;
        }

        public string Title { get; set; }

        public IPageViewModel PageViewModel { get; set; }

        [Obsolete("Is not used in this class. PageViewModel property is used instead.")]
        public object Content { get; set; }
    }
}