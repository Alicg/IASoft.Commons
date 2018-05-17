using System.Runtime.InteropServices;

namespace Utils
{
    public class CursorExtensions
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;

            public static implicit operator System.Windows.Point(Point point)
            {
                return new System.Windows.Point(point.X, point.Y);
            }
        }
        
        [DllImport("User32.dll")]
        private static extern bool GetCursorPos(out Point point);
        
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        public static System.Windows.Point GetCursorPosition()
        {
            GetCursorPos(out var point);
            return point;
        }

        public static void SetCursorPosition(System.Windows.Point point)
        {
            SetCursorPos((int) point.X, (int) point.Y);
        }
    }
}