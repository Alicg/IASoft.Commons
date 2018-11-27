using System;
using IASoft.WPFCommons.Events;

namespace IASoft.WPFCommons
{
    public interface IBaseViewModel
    {
        event EventHandler Closed;

        ViewModelPopupRequests ViewModelPopupRequests { get; }

        void ProcessKeyPressing(KeyPressedEventData keyPressedEventData);

        void Close();
    }
}