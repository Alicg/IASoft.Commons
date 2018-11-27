namespace IASoft.WPFCommons.Events
{
    public class FullscreenOnRequestEvent
    {
        public FullscreenOnRequestEvent(object view)
        {
            this.View = view;
        }

        public object View { get; }
    }
}