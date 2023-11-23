﻿using SkiaSharp;
using System.Diagnostics;
using System.Reflection;

namespace ScottPlotTests;

internal static class Extensions
{
    internal static void SaveTestImage(this Plot plt, int width = 600, int height = 400, string subName = "")
    {
        // determine filename based on name of calling function
        StackTrace stackTrace = new();
        StackFrame frame = stackTrace.GetFrame(1) ?? throw new InvalidOperationException("bad caller");
        MethodBase method = frame.GetMethod() ?? throw new InvalidDataException("bad method");
        string callingMethod = method.Name;

        if (!string.IsNullOrWhiteSpace(subName))
            subName = "_" + subName;

        string saveFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "test-images");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        string fileName = callingMethod + subName + ".png";
        string filePath = Path.Combine(saveFolder, fileName);
        Console.WriteLine(filePath);

        // actually save the thing
        plt.SaveJpeg(filePath, width, height);
    }

    internal static void SaveTestImage(this SKBitmap bmp, string subName = "")
    {
        // determine filename based on name of calling function
        StackTrace stackTrace = new();
        StackFrame frame = stackTrace.GetFrame(1) ?? throw new InvalidOperationException("bad caller");
        MethodBase method = frame.GetMethod() ?? throw new InvalidDataException("bad method");
        string callingMethod = method.Name;

        if (!string.IsNullOrWhiteSpace(subName))
            subName = "_" + subName;

        string saveFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "test-images");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        string fileName = callingMethod + subName + ".png";
        string filePath = Path.Combine(saveFolder, fileName);
        Console.WriteLine(filePath);

        // actually save the thing
        using SKFileWStream fs = new(filePath);
        bmp.Encode(fs, SKEncodedImageFormat.Png, quality: 100);
    }

    internal static void SaveTestString(this string s, string extension = ".html")
    {
        // determine filename based on name of calling function
        StackTrace stackTrace = new();
        StackFrame frame = stackTrace.GetFrame(1) ?? throw new InvalidOperationException("bad caller");
        MethodBase method = frame.GetMethod() ?? throw new InvalidDataException("bad method");
        string callingMethod = method.Name;

        string saveFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "test-images");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        string fileName = callingMethod + extension;
        string filePath = Path.Combine(saveFolder, fileName);
        Console.WriteLine(filePath);

        // actually save the thing
        File.WriteAllText(filePath, s);
    }
}
