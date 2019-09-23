using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CropCirclesUnpacker.Extensions
{
  public static class BitmapExtension
  {
    public static void SetPixels(this Bitmap bitmap, byte[] pixels)
    {
      PixelFormat format = bitmap.PixelFormat;

      int bytesPerPixel = (Bitmap.GetPixelFormatSize(format) / 8);
      Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
      BitmapData lockData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, format);

      IntPtr p = lockData.Scan0;
      for (int row = 0; row < bitmap.Height; ++row)
      {
        Marshal.Copy(pixels, row * bitmap.Width * bytesPerPixel, p, bitmap.Width * bytesPerPixel);
        p = new IntPtr(p.ToInt64() + lockData.Stride);
      }

      bitmap.UnlockBits(lockData);
    }

    public static void SetPalette(this Bitmap bitmap, Color[] colors)
    {
      ColorPalette bitmapPalette = bitmap.Palette;
      for (int i = 0; i < bitmapPalette.Entries.Length; ++i)
      {
        bitmapPalette.Entries[i] = colors[i];
      }
      bitmap.Palette = bitmapPalette;
    }
  }
}
