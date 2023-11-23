﻿using ScottPlot;

namespace WinForms_Demo.Demos;

public partial class MouseTracker : Form, IDemoWindow
{
    public string Title => "Mouse Tracker";

    public string Description => "Demonstrates how to interact with the mouse " +
        "and convert between screen units (pixels) and axis units (coordinates)";

    readonly ScottPlot.Plottables.Crosshair CH;

    public MouseTracker()
    {
        InitializeComponent();

        CH = formsPlot1.Plot.Add.Crosshair(0, 0);

        formsPlot1.MouseMove += (s, e) =>
        {
            // demonstrate pixel-to-coordinates
            Pixel px = new(e.X, e.Y);
            Coordinates coordinates = formsPlot1.GetCoordinates(px);

            // demonstrate coordinates-to-pixel
            Pixel px2 = formsPlot1.Plot.GetPixel(coordinates);

            // place the crosshair where the mouse is
            CH.Position = coordinates;
            formsPlot1.Refresh();

            // display where the mouse is in the titlebar
            Text = $"{px2} {coordinates}";
        };
    }
}
