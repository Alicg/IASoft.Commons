using System.Windows.Input;

namespace IASoft.WPFCommons.Events
{
    public class KeyPressedEventData
    {
        public KeyPressedEventData(object sender, KeyEventArgs args)
        {
            this.Sender = sender;
            this.Args = args;
        }

        public object Sender { get; private set; }

        public KeyEventArgs Args { get; private set; }
    }
}
