namespace Utils.Common.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public interface IWpfReductionAlgorithm
    {
        List<Point> Reduce(List<Point> points, Double tolerance);
    }
}
