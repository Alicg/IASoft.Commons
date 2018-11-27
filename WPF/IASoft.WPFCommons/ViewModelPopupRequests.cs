using System;
using System.Windows;
using IASoft.WPFCommons.Events;

namespace IASoft.WPFCommons
{
    public class ViewModelPopupRequests
    {
        public ViewModelPopupRequests(IReactiveEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
        }

        protected IReactiveEventAggregator EventAggregator { get; }

        /// <summary>
        /// Создает форму с сообщением и кнопками Yes\No
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="caption">
        /// </param>
        /// <param name="confirmationCallback">
        /// Колбек с результатом из popup окна.
        /// </param>
        public void YesNoMessageForm(string message, string caption, Action<ConfirmationArgs> confirmationCallback)
        {
            this.EventAggregator.Publish(new DefaultConfirmationRequestData(caption, message, confirmationCallback));
        }

        public void AskForStringForm(string title, Action<ConfirmationArgs> resultCallback)
        {
            this.EventAggregator.Publish(new StringRequestData(title, resultCallback));
        }

        /// <summary>
        /// Создает форму с ошибкой и кнопкой ОК
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public bool ErrorMessage(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK;
        }

        /// <summary>
        /// Добавляет сообщение в стек сообщений
        /// </summary>
        public void InfoMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}