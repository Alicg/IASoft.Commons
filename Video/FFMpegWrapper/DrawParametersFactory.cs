namespace FFMpegWrapper
{
    internal static class DrawParametersFactory
    {
        public static string GetDrawPixelParameter(Pixel pixel)
        {
            const string Template = "drawbox=enable='between(t,{0},{1})' : x={2} : y={3} : w=1 : h=1 : color={4}";
            return string.Format(Template, pixel.StartSecond, pixel.EndSecond, pixel.X, pixel.Y, pixel.Color.ToKnownColor());
        }

        public static string GetDrawPolygonParameter(Pixel[] pixels)
        {
            var retStr = "";
            foreach (var pixel in pixels)
            {
                retStr = $"{retStr},{GetDrawPixelParameter(pixel)}";
            }
            return retStr.Remove(0, 1);
        }
    }
}
