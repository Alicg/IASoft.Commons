using System;
using System.Reactive.Linq;
using IASoft.PrismCommons;
using IASoft.WPFCommons.Events;

namespace SVA.Infrastructure
{
    public class PageViewModel : BasePrismViewModel, IPrismPageViewModel
    {
        private bool isActive;

        protected PageViewModel(IReactiveEventAggregator eventAggregator) : base (eventAggregator)
        {
            // Нажатие клавишь ловим только на уровне PageViewModel, чтобы фильтровать неактивные вкладки.
            this.EventAggregator.GetEvent<KeyPressedEventData>()?.Where(v => this.IsActive).Subscribe(this.ProcessKeyPressing);
        }

        public virtual string PageName { get; }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                this.isActive = value;
                this.OnPropertyChanged();
            }
        }

        public event EventHandler IsActiveChanged;
    }
}
