using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SVA.Infrastructure.Controls
{
    public class RoundImage : Control
    {
        public static readonly DependencyProperty ImageBytesProperty = DependencyProperty.Register(
            "ImageBytes",
            typeof(IList<byte>),
            typeof(RoundImage),
            new PropertyMetadata(default(IList<byte>)));

        public IList<byte> ImageBytes
        {
            get { return (IList<byte>)GetValue(ImageBytesProperty); }
            set { SetValue(ImageBytesProperty, value); }
        }
        
        public static readonly DependencyProperty BorderBrushColorProperty = DependencyProperty.Register(
            "BorderBrushColor",
            typeof(Brush),
            typeof(RoundImage),
            new PropertyMetadata(default(Brush)));

        public Brush BorderBrushColor
        {
            get { return (Brush)GetValue(BorderBrushColorProperty); }
            set { SetValue(BorderBrushColorProperty, value); }
        }

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height",
            typeof(double),
            typeof(RoundImage),
            new PropertyMetadata(default(double)));

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(double),
            typeof(RoundImage),
            new PropertyMetadata(default(double)));

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
    }
}