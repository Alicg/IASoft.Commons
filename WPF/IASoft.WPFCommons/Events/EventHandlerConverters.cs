using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace IASoft.WPFCommons.Events
{
    public static class EventHandlerConverters
    {
        public static Func<EventHandler<ElapsedEventArgs>, ElapsedEventHandler> ElapsedEventHandlerConversion
        {
            get
            {
                return handler =>
                {
                    ElapsedEventHandler targetHandler = (s, a) => handler(s, a);
                    return targetHandler;
                };
            }
        }
        
        public static Func<EventHandler<MouseButtonEventArgs>, MouseButtonEventHandler> MouseButtonEventHandlerConversion
        {
            get
            {
                return handler =>
                {
                    MouseButtonEventHandler targetHandler = (s, a) => handler(s, a);
                    return targetHandler;
                };
            }
        }
        
        public static Func<EventHandler<MouseEventArgs>, MouseEventHandler> MouseEventHandlerConversion
        {
            get
            {
                return handler =>
                {
                    MouseEventHandler targetHandler = (s, a) => handler(s, a);
                    return targetHandler;
                };
            }
        }
        
        public static Func<EventHandler<SizeChangedEventArgs>, SizeChangedEventHandler> SizeChangedEventHandlerConversion
        {
            get
            {
                return handler =>
                {
                    SizeChangedEventHandler targetHandler = (s, a) => handler(s, a);
                    return targetHandler;
                };
            }
        }
        
        public static Func<EventHandler<KeyEventArgs>, KeyEventHandler> KeyEventHandlerConversion
        {
            get
            {
                return handler =>
                {
                    KeyEventHandler targetHandler = (s, a) => handler(s, a);
                    return targetHandler;
                };
            }
        }
    }
}
