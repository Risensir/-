﻿using ScottPlot.Panels;
using ScottPlot.Plottables;
using ScottPlot.DataSources;
using System;

namespace ScottPlot;

/// <summary>
/// Helper methods to create plottable objects and add them to the plot
/// </summary>
public class PlottableAdder
{
    private readonly Plot Plot;

    public IPalette Palette { get; set; } = new Palettes.Category10();

    public Color NextColor => Palette.Colors[Plot.PlottableList.Count % Palette.Colors.Length];

    public PlottableAdder(Plot plot)
    {
        Plot = plot;
    }

    public Crosshair Crosshair(double x, double y)
    {
        Crosshair ch = new()
        {
            Position = new(x, y)
        };
        ch.LineStyle.Color = NextColor;
        Plot.PlottableList.Add(ch);
        return ch;
    }

    public Heatmap Heatmap(double[,] intensities)
    {
        Heatmap heatmap = new(intensities);
        Plot.PlottableList.Add(heatmap);
        return heatmap;
    }

    public Scatter Line(double x1, double y1, double x2, double y2)
    {
        Coordinates[] coordinates = { new(x1, y1), new(x2, y2) };
        return Scatter(coordinates);
    }

    public Scatter Line(Coordinates pt1, Coordinates pt2)
    {
        Coordinates[] coordinates = { pt1, pt2 };
        return Scatter(coordinates);
    }

    public Pie Pie(IList<PieSlice> slices)
    {
        Pie pie = new(slices);
        Plot.PlottableList.Add(pie);
        return pie;
    }

    public Pie Pie(IEnumerable<double> values)
    {
        var slices = values.Select(v => new PieSlice
        {
            Value = v,
            Fill = new() { Color = NextColor },
        }).ToList();
        var pie = Pie(slices);
        Plot.PlottableList.Add(pie);
        return pie;
    }

    public void Plottable(IPlottable plottable)
    {
        Plot.PlottableList.Add(plottable);
    }

    public Scatter Scatter(IScatterSource data, Color? color = null)
    {
        Color nextColor = color ?? NextColor;
        Scatter scatter = new(data);
        scatter.LineStyle.Color = nextColor;
        scatter.MarkerStyle.Fill.Color = nextColor;
        Plot.PlottableList.Add(scatter);
        return scatter;
    }

    public Scatter Scatter(IReadOnlyList<double> xs, IReadOnlyList<double> ys, Color? color = null)
    {
        return Scatter(new ScatterSourceXsYs(xs, ys), color);
    }

    public Scatter Scatter(IReadOnlyList<Coordinates> coordinates, Color? color = null)
    {
        return Scatter(new ScatterSourceCoordinates(coordinates), color);
    }

    public Signal Signal(IReadOnlyList<double> ys, double period = 1, Color? color = null)
    {
        Color nextColor = color ?? NextColor;
        SignalSource data = new(ys, period);
        var sig = new Signal(data);
        sig.LineStyle.Color = nextColor;
        sig.Marker.Fill.Color = nextColor;
        Plot.PlottableList.Add(sig);
        return sig;
    }

    public BarPlot Bar(double[] values)
    {
        IList<Bar> bars = values.Select(x => new Bar() { Value = x }).ToList();
        return Bar(bars);
    }

    public BarPlot Bar(IList<BarSeries> series)
    {
        var barPlot = new BarPlot(series);
        Plot.PlottableList.Add(barPlot);
        return barPlot;
    }

    public BarPlot Bar(IList<Bar> bars, Color? color = null, string? label = null)
    {
        var series = new BarSeries()
        {
            Bars = bars,
            Color = color ?? NextColor,
            Label = label
        };

        List<BarSeries> seriesList = new() { series };

        return Bar(seriesList);
    }

    public BoxPlot Box(IList<Box> boxes)
    {
        BoxGroup singleGroup = new()
        {
            Boxes = boxes,
        };

        singleGroup.Fill.Color = NextColor;

        IList<BoxGroup> groups = new List<BoxGroup>() { singleGroup };

        return Box(groups);
    }

    public BoxPlot Box(IList<BoxGroup> groups)
    {
        BoxGroups boxGroups = new()
        {
            Series = groups,
        };

        BoxPlot boxPlot = new()
        {
            Groups = boxGroups,
        };

        Plot.PlottableList.Add(boxPlot);
        return boxPlot;
    }

    public CandlestickPlot Candlestick(IList<IOHLC> ohlcs)
    {
        OHLCSource dataSource = new(ohlcs);
        CandlestickPlot candlestickPlot = new(dataSource);
        Plot.PlottableList.Add(candlestickPlot);
        return candlestickPlot;
    }

    public ColorBar ColorBar(IHasColorAxis source, Edge edge = Edge.Right)
    {
        ColorBar colorBar = new(source, edge);

        Plot.Panels.Add(colorBar);
        return colorBar;
    }

    public ErrorBar ErrorBar(IReadOnlyList<double> xs, IReadOnlyList<double> ys, IReadOnlyList<double> yErrors)
    {
        ErrorBar eb = new(xs, ys, null, null, yErrors, yErrors)
        {
            Color = NextColor,
        };

        Plot.PlottableList.Add(eb);
        return eb;
    }

    public OhlcPlot OHLC(IList<IOHLC> ohlcs)
    {
        OHLCSource dataSource = new(ohlcs);
        OhlcPlot ohlc = new(dataSource);
        Plot.PlottableList.Add(ohlc);
        return ohlc;
    }

    public Polygon Polygon(Coordinates[] coordinates)
    {
        Polygon poly = new Polygon(coordinates);
        Plot.PlottableList.Add(poly);
        return poly;
    }

    /// <summary>
    /// Fill the vertical range between two Y points for each X point
    /// </summary>
    public FillY FillY(double[] xs, double[] ys1, double[] ys2)
    {
        List<(double, double, double)> data = new();

        for (int i = 0; i < xs.Length; i++)
        {
            data.Add((xs[i], ys1[i], ys2[i]));
        }

        return FillY(data);
    }

    /// <summary>
    /// Fill the vertical range between two Y points for each X point
    /// </summary>
    public FillY FillY(Scatter scatter1, Scatter scatter2)
    {
        FillY rangePlot = new(scatter1, scatter2);
        rangePlot.FillStyle.Color = NextColor;
        Plot.PlottableList.Add(rangePlot);
        return rangePlot;
    }

    /// <summary>
    /// Fill the vertical range between two Y points for each X point
    /// </summary>
    public FillY FillY(ICollection<(double X, double Top, double Bottom)> data)
    {
        FillY rangePlot = new();
        rangePlot.FillStyle.Color = NextColor;
        rangePlot.SetDataSource(data);
        Plot.PlottableList.Add(rangePlot);
        return rangePlot;
    }

    /// <summary>
    /// Fill the vertical range between two Y points for each X point
    /// This overload uses a custom function to calculate X, Y1, and Y2 values
    /// </summary>
    public FillY FillY<T>(ICollection<T> data, Func<T, (double X, double Top, double Bottom)> function)
    {
        var rangePlot = new FillY();
        rangePlot.FillStyle.Color = NextColor;
        rangePlot.SetDataSource(data, function);
        Plot.PlottableList.Add(rangePlot);
        return rangePlot;
    }
}
