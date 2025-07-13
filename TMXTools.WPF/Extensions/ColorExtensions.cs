using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

using NewColor = System.Windows.Media.Color;
using NewPixelFormat = System.Windows.Media.PixelFormat;
using NewPixelFormats = System.Windows.Media.PixelFormats;
using OldColor = System.Drawing.Color;
using OldPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace TMXTools.WPF.Extensions;

public record ColorRecord(int R, int G, int B, int A);

public static class ColorExtensions
{
    public static ColorRecord ToRecord(this OldColor color)
    {
        return new ColorRecord(color.R, color.G, color.B, color.A);
    }

    public static ColorRecord ToRecord(this NewColor color)
    {
        return new ColorRecord(color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Converts a <see cref="OldColor"/> to an <see cref="NewColor"/>.
    /// <br/>
    /// Supports interoperability between WPF and System.Drawing (WinForms/GDI).
    /// </summary>
    /// <param name="color">The <see cref="OldColor"/> to convert.</param>
    /// <returns>A <see cref="NewColor"/> with the same ARGB values</returns>
    public static NewColor Convert(this OldColor color)
    {
        NewColor ret = new()
        {
            R = color.R,
            G = color.G,
            B = color.B,
            A = color.A,
        };
        return ret;
    }

    /// <summary>
    /// Converts a <see cref="NewColor"/> to an <see cref="OldColor"/>.
    /// <br/>
    /// Supports interoperability between WPF and System.Drawing (WinForms/GDI).
    /// </summary>
    /// <param name="color">The <see cref="NewColor"/> to convert.</param>
    /// <returns>A <see cref="OldColor"/> with the same ARGB values</returns>
    public static OldColor Convert(this NewColor color)
    {
        return OldColor.FromArgb(color.A, color.R, color.G, color.B);
    }

    /// <summary>
    /// Converts a <see cref="OldPixelFormat"/> to an <see cref="NewPixelFormat"/>.
    /// <br/>
    /// Supports interoperability between WPF and System.Drawing (WinForms/GDI).
    /// </summary>
    /// <param name="format">The <see cref="OldPixelFormat"/> to convert.</param>
    /// <returns>The closest matching <see cref="NewPixelFormat"/></returns>
    public static NewPixelFormat Convert(this OldPixelFormat format)
    {
        return format switch
        {
            OldPixelFormat.Format8bppIndexed => NewPixelFormats.Indexed8,
            OldPixelFormat.Format24bppRgb => NewPixelFormats.Rgb24,
            OldPixelFormat.Format32bppArgb => NewPixelFormats.Bgra32, //TODO: Is this correct? Bgra -> Argb probably doesn't work
            _ => NewPixelFormats.Default,
        };
    }

    /// <summary>
    /// Converts a <see cref="NewPixelFormat"/> to an <see cref="OldPixelFormat"/>.
    /// <br/>
    /// Supports interoperability between WPF and System.Drawing (WinForms/GDI).
    /// </summary>
    /// <param name="format">The <see cref="NewPixelFormat"/> to convert.</param>
    /// <returns>The closest matching <see cref="OldPixelFormat"/></returns>
    public static OldPixelFormat Convert(this NewPixelFormat format)
    {
        //Can't use a switch expression because NewPixelFormats is not a constant
        if (format == NewPixelFormats.Rgb24)
            return OldPixelFormat.Format24bppRgb;
        else if (format == NewPixelFormats.Indexed8)
            return OldPixelFormat.Format8bppIndexed;
        else if (format == NewPixelFormats.Bgra32) //TODO: Is this correct? Bgra -> Argb probably doesn't work
            return OldPixelFormat.Format32bppArgb;

        return OldPixelFormat.DontCare;
    }

    /// <summary>
    /// Converts a <see cref="ColorPalette"/> to an <see cref="BitmapPalette"/>.
    /// <br/>
    /// Supports interoperability between WPF and System.Drawing (WinForms/GDI).
    /// </summary>
    /// <param name="palette">The <see cref="ColorPalette"/> to convert, taken from a Bitmap.</param>
    /// <returns>
    /// A <see cref="BitmapPalette"/> containing the color entries of the original <paramref name="palette"/>, if possible;
    /// Otherwise, <see langword="null"/>
    /// </returns>
    public static BitmapPalette? Convert(this ColorPalette? palette)
    {
        NewColor[]? colors = palette?.Entries?.Select(c => c.Convert())?.ToArray();
        if (colors is null || colors.Length <= 0 || colors.Length > 256)
            return null;

        return new BitmapPalette(colors);
    }

    /// <summary>
    /// Creates a <see cref="WriteableBitmap"/> using the <paramref name="bitmap"/> data as the backbuffer.
    /// <br/>
    /// Supports interoperability between WPF and System.Drawing (WinForms/GDI).
    /// </summary>
    /// <param name="bitmap">The source <see cref="Bitmap"/>.</param>
    /// <returns>
    /// A <see cref="WriteableBitmap"/> using the <paramref name="bitmap"/> data as the backbuffer, if possible;
    /// Otherwise, <see langword="null"/>
    /// </returns>
    public static WriteableBitmap? ToWriteableBitmap(this Bitmap? bitmap)
    {
        if (bitmap is null || bitmap.Width <= 0 || bitmap.Height <= 0)
            return null;

        BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), nint.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        return new WriteableBitmap(bitmapSource);
    }
}
