using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Utils.Geometry
{
    [DataContract]
    public class Point2D
    {
        [DataMember]
        public double X { get; set; }
        
        [DataMember]
        public double Y { get; set; }
        
        public Point2D()
        {
        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Point2D point)
        {
            return Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2));
        }

        public Point2D GetNormal()
        {
            var startPoint = new Point2D(0,0);
            var dist = startPoint.DistanceTo(this);
            return this/dist;
        }

        public static implicit operator Point2D(Point pt)
        {
            return new Point2D(pt.X, pt.Y);
        }

        public static explicit operator Point(Point2D pt)
        {
            return new Point((int)pt.X, (int)pt.Y);
        }

        public override bool Equals(object obj) {
            return obj is Point2D && this == (Point2D) obj;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Point2D a, Point2D b)
        {
            if (RuntimeHelpers.Equals(a, b))
                return true;
            if (RuntimeHelpers.Equals(a, null) || RuntimeHelpers.Equals(b, null))
                return false;
            return GeometricallyEquals(a,b);
        }

        public static bool operator !=(Point2D a, Point2D b)
        {
            return !(a == b);
        }

        public static Point2D operator +(Point2D point1, Point2D point2)
        {
            return new Point2D(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point2D operator -(Point2D point1, Point2D point2)
        {
            return new Point2D(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static Point2D operator *(Point2D point, double coeff)
        {
            return new Point2D(point.X * coeff, point.Y * coeff);
        }

        public static Point2D operator /(Point2D point, double coeff)
        {
            return new Point2D(point.X / coeff, point.Y / coeff);
        }

        public static bool GeometricallyEquals(Point2D a, Point2D b, double epsilon = double.Epsilon)
        {
            return Math.Abs(a.X - b.X) < epsilon && Math.Abs(a.Y - b.Y) < epsilon;
        }

        /// <summary>
        /// Лежит ли точка в многоугольнике. Считается количество пересечений горизонтального луча из точки со всеми ребрами многоугольника. 
        /// Если кол-во пересечений нечетное - точка внутри, иначе - снаружи.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon">Замкнутый полигон (1я и последняя точка совпадают)</param>
        /// <returns></returns>
        public static bool PointInPolygon(Point2D point, Point2D[] polygon) // whether to repeat the first vertex at the end
        {
            var res = false;
            int i, j, len = polygon.Length;
            for (i = 0, j = len - 1; i < len; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                 (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    res = !res;
            }
            return res;
        }

        #region UtmConvertations
        
        #endregion
    }
}
