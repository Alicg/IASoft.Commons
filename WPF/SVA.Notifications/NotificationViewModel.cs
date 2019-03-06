using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using IASoft.WPFCommons.Events;
using Prism.Mvvm;
using ReactiveUI;
using ReactiveUI.Legacy;
using Utils.Extensions;

namespace SVA.Notifications
{
    using System.Collections.Generic;
    using System.Linq;

    public class NotificationViewModel : BindableBase, INotificationSystem, IDisposable
    {
        private readonly IScheduler uiScheduler;
        private readonly IList<IDisposable> disposables = new List<IDisposable>();

        public NotificationViewModel(IScheduler uiScheduler, IReactiveEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
            this.uiScheduler = uiScheduler;
            const int TemporaryNotificationDurationSeconds = 5;
            var notificationAddedObservable = this.TemporaryNotificationStack.ItemsAdded
                .Delay(TimeSpan.FromSeconds(TemporaryNotificationDurationSeconds))
                .ObserveOn(uiScheduler)
                .Subscribe(v =>
                {
                    lock (this.NotificationMessages)
                    {
                        this.TemporaryNotificationStack.Remove(v);
                    }
                });
            this.disposables.Add(notificationAddedObservable);
        }

        public IReactiveEventAggregator EventAggregator { get; }

        public IReactiveList<NotificationMessage> NotificationMessages { get; private set; } = new ReactiveList<NotificationMessage>();

        public IReactiveList<NotificationMessage> TemporaryNotificationStack { get; private set; } = new ReactiveList<NotificationMessage>();

        public virtual void PushNotification(NotificationMessage notificationMessage)
        {
            this.disposables.Add(
                this.uiScheduler.Schedule(
                    _ =>
                        {
                            lock (this.NotificationMessages)
                            {
                                this.NotificationMessages.Insert(0, notificationMessage);
                                this.TemporaryNotificationStack.Insert(0, notificationMessage);
                            }
                        }));
        }

        public virtual void PushException(Exception exc)
        {
            var exceptionNotification = new NotificationMessage(DateTime.Now, NotificationMessageType.GeneralError, exc.GetFullMessage(), "Unknown error", exc.Source);
            var aggregateException = exc as AggregateException;
            exceptionNotification.AdditionalMessageText = aggregateException == null
                                                              ? exc.StackTrace
                                                              : aggregateException.InnerExceptions.Aggregate(string.Empty, (t, c) => t + "\r\n-----\r\n" + c.StackTrace);
            this.PushNotification(exceptionNotification);
        }

        public void Dispose()
        {
            this.disposables.ForEach(v => v.Dispose());
        }
    }
}