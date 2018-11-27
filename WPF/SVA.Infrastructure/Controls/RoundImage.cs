using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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