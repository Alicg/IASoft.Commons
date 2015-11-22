namespace Utils.Common.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public interface IReductionAlgorithm
    {
        List<Point> Reduce(List<Point> points, Double tolerance);
    }
}
