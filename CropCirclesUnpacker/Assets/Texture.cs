﻿using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Assets
{
  public class Texture : Asset
  {
    private PixelFormat Format = PixelFormat.Format8bppIndexed;

    public Texture(string name, byte[] pixels, int width, int height)
      : base(name, AssetType.Texture)
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
      bitmap.SetPalette(palette.Colours);

      return bitmap;
    }

    public bool ChangeImage(Bitmap bitmap)
    {
      if (bitmap.PixelFormat != Format)
        return false;

      if ((bitmap.Width != Width) || (bitmap.Height != Height))
        return false;

      Pixels = bitmap.GetPixels();
      return (Pixels != null);
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

    public byte[] Pixels
    {
      get;
      private set;
    }
  }
}
