﻿namespace ScottPlot;

/// <summary>
/// This configuration object (reference type) permanently lives inside objects which require styling.
/// It is recommended to use this object as an init-only property.
/// </summary>
public class LineStyle
{
    public float Width { get; set; } = 1.0f;
    public Color Color { get; set; } = Colors.Black;
    public LinePattern Pattern { get; set; } = LinePattern.Solid;
    public bool IsVisible { get; set; } = true;
    public static LineStyle None => new() { IsVisible = false, Color = Colors.Transparent, Width = 0 };
    public bool AntiAlias { get; set; } = true;
}
