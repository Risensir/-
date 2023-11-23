﻿namespace ScottPlot;

public interface IZoomRectangle
{
    bool VerticalSpan { get; set; }
    bool HorizontalSpan { get; set; }
    bool IsVisible { get; set; }

    public Pixel MouseDown { get; }
    public Pixel MouseUp { get; }
    void Update(Pixel mouseDown, Pixel mouseUp);
    void Clear();

    void Render(RenderPack rp);
}
