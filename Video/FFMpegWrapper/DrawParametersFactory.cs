namespace FFMpegWrapper
{
    public static class DrawParametersFactory
    {
        public static string GetDrawPixelParameter(Pixel pixel)
        {
            const string template = "drawbox=enable='between(t,{0},{1})' : x={2} : y={3} : w=1 : h=1 : color={4}";
            return string.Format(template, pixel.StartSecond, pixel.EndSecond, pixel.X, pixel.Y, pixel.Color.ToKnownColor());
        }

        public static string GetDrawPolygonParameter(Pixel[] pixels)
        {
            var retStr = "";
            foreach (var pixel in pixels)
            {
                retStr = string.Format("{0},{1}", retStr, GetDrawPixelParameter(pixel));
            }
            return retStr.Remove(0, 1);
        }
    }
}
