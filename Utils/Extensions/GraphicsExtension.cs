using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Utils.Extensions
{
    public static class GraphicsExtension
    {
        /// <summary>
        /// Draws rectangle with rounded corners
        /// </summary>
        /// <param name="gfx">Drawing graphic object</param>
        /// <param name="bounds">Bounds of rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="drawPen">Drawing pen. Must be not null.</param>
        /// <param name="fillBrush">Filling brush. Must be not null.</param>
        public static void DrawRoundedRectangle(this Graphics gfx, Rectangle bounds, int cornerRadius, Pen drawPen,
                                                Brush fillBrush)
        {
            var gfxPath = new GraphicsPath();
            using (var pen = (Pen) drawPen.Clone())
            {
                pen.EndCap = pen.StartCap = LineCap.Round;

                gfxPath.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
                gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
                gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius,
                               cornerRadius, cornerRadius, 0, 90);
                gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                gfxPath.CloseAllFigures();

                gfx.FillPath(fillBrush, gfxPath);
                gfx.DrawPath(pen, gfxPath);
            }
        }

        /// <summary>
        /// Draws rectangle with rounded corners
        /// </summary>
        /// <param name="gfx">Drawing graphic object</param>
        /// <param name="bounds">Bounds of rectangle</param>
        /// <param name="cornerRadius">Radius of the corners</param>
        /// <param name="drawPen">Drawing pen. Must be not null.</param>
        /// <param name="fillColor">Color of filling solid brush</param>
        public static void DrawRoundedRectangle(this Graphics gfx, Rectangle bounds, int cornerRadius, Pen drawPen,
                                                Color fillColor)
        {
            using (var brush = new SolidBrush(fillColor))
            {
                DrawRoundedRectangle(gfx, bounds, cornerRadius, drawPen, brush);
            }
        }

        /// <summary>
        /// Converts byte array to image.
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        public static Image ToImage(this byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length == 0) return null;
                using (var s = new MemoryStream(bytes))
                {
                    return Image.FromStream(s);
                }
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Converts image to byte array.
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="imageFormat">Saving image format</param>
        public static byte[] ToBytes(this Image image, ImageFormat imageFormat)
        {
            try
            {
                if (image == null) return null;
                using (var s = new MemoryStream())
                {
                    image.Save(s, imageFormat);
                    return s.GetBuffer();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Resizes source image to bitmap with defined size with saving aspect ration.
        /// </summary>
        public static Bitmap Resize(this Image sourceImage, int targetWidth, int targetHeight)
        {
            if (sourceImage == null)
                return null;

//            return image == null ? null : new Bitmap(sourceImage.GetThumbnailImage(width, height, null, IntPtr.Zero));

            var res = new Bitmap(targetWidth, targetHeight);
            using (Graphics g = Graphics.FromImage(res))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var x = (float) sourceImage.Width;
                var y = (float) sourceImage.Height;
                if ((float) res.Width/res.Height > x/y)
                {
                    float nx = x*res.Height/y;
                    float dx = (res.Width - nx)/2;
                    g.DrawImage(sourceImage, dx, 0, res.Width - 2*dx, res.Height);
                }
                else
                {
                    float ny = y*res.Width/x;
                    float dy = (res.Height - ny)/2;
                    g.DrawImage(sourceImage, 0, dy, res.Width, res.Height - 2*dy);
                }
            }
            return res;
        }
    }
}