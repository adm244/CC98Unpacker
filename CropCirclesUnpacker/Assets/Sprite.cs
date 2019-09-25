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

    public Sprite(string name, byte[] pixels, int width, int height, bool isBackground)
    {
      Debug.Assert((width > 0) && (height > 0));
      Debug.Assert((width * height) == pixels.Length);

      Name = name;
      Width = width;
      Height = height;
      Background = isBackground;

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

    public override string ToString()
    {
      return Name;
    }

    public string Name
    {
      get;
      private set;
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

    public bool Background
    {
      get;
      private set;
    }
  }
}
