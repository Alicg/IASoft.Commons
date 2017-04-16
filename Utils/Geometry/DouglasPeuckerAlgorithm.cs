using System;
using System.Collections.Generic;
using System.Drawing;
using Utils.Common.Geometry;

namespace Utils.Geometry
{
    public class DouglasPeuckerAlgorithm : IReductionAlgorithm
    {
        public List<Point> Reduce(List<Point> points, double tolerance)
        {
            return this.DouglasPeuckerReduction(points, tolerance);
        }

        private List<Point> DouglasPeuckerReduction(List<Point> points, Double tolerance)
        {

            if (points == null || points.Count < 3)
                return points;

            const int firstPoint = 0;
            var lastPoint = points.Count - 1;
            var pointIndexsToKeep = new List<Int32> {firstPoint, lastPoint};

            //Add the first and last index to the keepers


            //The first and the last point can not be the same
            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            this.DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

            var returnPoints = new List<Point>();
            pointIndexsToKeep.Sort();
            foreach (var index in pointIndexsToKeep)
            {
                returnPoints.Add(points[index]);
            }

            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point indexs to keep.</param>
        private void DouglasPeuckerReduction(List<Point> points, Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            var indexFarthest = 0;

            for (var index = firstPoint; index < lastPoint; index++)
            {
                var distance = this.PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                this.DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                this.DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        /// The distance of a point from a line made from point1 and point2.
        /// </summary>
        /// <param name="point1">The PT1.</param>
        /// <param name="point2">The PT2.</param>
        /// <param name="point">The p.</param>
        /// <returns></returns>
        public Double PerpendicularDistance(Point point1, Point point2, Point point)
        {
            var area = Math.Abs(.5 * (point1.X * point2.Y + point2.X * point.Y + point.X * point1.Y - point2.X * point1.Y - point.X * point2.Y - point1.X * point.Y));
            var bottom = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            var height = area / bottom * 2;

            return height;
        }
    }
}
