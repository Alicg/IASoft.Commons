namespace IASoft.WPFCommons.Events
{
    public class FullscreenOffRequestEvent
    {
        public FullscreenOffRequestEvent(object view)
        {
            this.View = view;
        }

        public object View { get; }
    }
}