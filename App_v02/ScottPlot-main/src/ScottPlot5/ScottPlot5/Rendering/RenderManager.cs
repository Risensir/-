﻿namespace ScottPlot.Rendering;

public class RenderManager
{
    /// <summary>
    /// This list of actions is performed in sequence to render a plot.
    /// It may be modified externally to inject custom functionality.
    /// </summary>
    public List<IRenderAction> RenderActions { get; }

    /// <summary>
    /// Information about the previous render
    /// </summary>
    public RenderDetails LastRender { get; private set; }

    /// <summary>
    /// Total number of renders
    /// </summary>
    public int RenderCount { get; private set; } = 0;

    /// <summary>
    /// This event is invoked after each render
    /// </summary>
    public event EventHandler<RenderDetails> RenderFinished = delegate { };

    /// <summary>
    /// Indicates whether this plot is in the process of executing a render
    /// </summary>
    public bool IsRendering { get; private set; } = false;

    /// <summary>
    /// Disable this in multiplot environments
    /// </summary>
    public bool ClearCanvasBeforeRendering { get; set; } = true;

    private Plot Plot { get; }

    public RenderManager(Plot plot)
    {
        Plot = plot;
        RenderActions = new(DefaultRenderActions());
    }

    public IRenderAction[] DefaultRenderActions()
    {
        return new IRenderAction[]
        {
            new RenderActions.ClearCanvas(),
            new RenderActions.ReplaceNullAxesWithDefaults(),
            new RenderActions.AutoAxisAnyUnsetAxes(),
            new RenderActions.EnsureAxesHaveArea(),
            new RenderActions.CalculateLayout(),
            new RenderActions.RegenerateTicks(),
            new RenderActions.RenderBackground(),
            new RenderActions.RenderGridsBelowPlottables(),
            new RenderActions.RenderPlottables(),
            new RenderActions.RenderGridsAbovePlottables(),
            new RenderActions.RenderLegends(),
            new RenderActions.RenderPanels(),
            new RenderActions.RenderZoomRectangle(),
            new RenderActions.SyncGLPlottables(),
            new RenderActions.RenderBenchmark(),
        };
    }

    public void Render(SKCanvas canvas, PixelRect rect)
    {
        IsRendering = true;
        canvas.Scale(Plot.ScaleFactor);

        List<(string, TimeSpan)> actionTimes = new();

        RenderPack rp = new(Plot, rect, canvas);

        Stopwatch sw = new();
        foreach (IRenderAction action in RenderActions)
        {
            if ((action is RenderActions.ClearCanvas) && (!ClearCanvasBeforeRendering))
            {
                continue;
            }

            sw.Restart();
            action.Render(rp);
            actionTimes.Add((action.ToString() ?? string.Empty, sw.Elapsed));
        }

        LastRender = new(rp, actionTimes.ToArray());

        RenderCount += 1;
        RenderFinished.Invoke(Plot, LastRender);
        IsRendering = false;
    }
}
