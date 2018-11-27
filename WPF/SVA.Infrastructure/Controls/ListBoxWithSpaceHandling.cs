using System.Windows.Controls;
using System.Windows.Input;

namespace SVA.Infrastructure.Controls
{
    public class ListBoxWithSpaceHandling : ListBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    e.Handled = false;
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }
    }
}