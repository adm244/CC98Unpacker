using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets
{
  public class Sprite
  {
    private PixelFormat Format = PixelFormat.Format8bppIndexed;
    private byte[] Pixels;

    public Sprite(byte[] pixels, int width, int height)
    {
      Debug.Assert((width > 0) && (height > 0));
      Debug.Assert((width * height) == pixels.Length);

      Width = width;
      Height = height;

      Pixels = new byte[pixels.Length];
      pixels.CopyTo(Pixels, 0);
    }

    public Bitmap CreateBitmap(Palette palette)
    {
      Bitmap bitmap = new Bitmap(Width, Height, Format);
      bitmap.SetPixels(Pixels);
      bitmap.SetPalette(palette.Entries);

      return bitmap;
    }

    public int Width
    {
      get;
      private set;
    }

    public int Height
    {
      get;
      private set;
    }
  }
}
