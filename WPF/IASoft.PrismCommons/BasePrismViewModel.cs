using System;
using IASoft.WPFCommons;
using IASoft.WPFCommons.Events;
using Prism.Mvvm;

namespace IASoft.PrismCommons
{
    public abstract class BasePrismViewModel : BindableBase, IBaseViewModel
    {
        public event EventHandler Closed;

        protected BasePrismViewModel(IReactiveEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
            
            this.ViewModelPopupRequests = new ViewModelPopupRequests(eventAggregator);
        }
        
        public ViewModelPopupRequests ViewModelPopupRequests { get; }

        protected IReactiveEventAggregator EventAggregator { get; }

        public virtual void ProcessKeyPressing(KeyPressedEventData keyPressedEventData)
        {
        }

        public void Close()
        {
            this.OnClosed();
        }

        protected virtual void OnClosed()
        {
            this.Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
