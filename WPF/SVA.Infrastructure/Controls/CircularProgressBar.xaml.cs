using System.Windows;
using System.Windows.Media;

namespace SVA.Infrastructure.Controls
{
    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar
    {
        static CircularProgressBar()
        {
            //Use a default Animation Framerate of 30, which uses less CPU time
            //than the standard 50 which you get out of the box
            //Timeline.DesiredFrameRateProperty.OverrideMetadata(
              //  typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 25 });
        }

        public static readonly DependencyProperty MainColorProperty =
             DependencyProperty.Register("MainColor", typeof(Brush), typeof(CircularProgressBar), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.AliceBlue)));

        public Brush MainColor
        {
            get { return (Brush)this.GetValue(MainColorProperty); }
            set
            {
                this.SetValue(MainColorProperty, value);
            }
        }

        public static readonly DependencyProperty SecondaryColorProperty =
             DependencyProperty.Register("SecondaryColor", typeof(Brush), typeof(CircularProgressBar), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush SecondaryColor
        {
            get { return (Brush)this.GetValue(SecondaryColorProperty); }
            set
            {
                this.SetValue(SecondaryColorProperty, value);
            }
        }

        public CircularProgressBar()
        {
            InitializeComponent();
        }
    }
}


