using System;
using System.Drawing;

namespace CropCirclesUnpacker.Assets
{
  public class Font : Asset
  {
    public Texture Texture;
    public GlythOffset[] Offsets;

    public Font(string name, Texture texture, GlythOffset[] offsets)
      : base(name, AssetType.Font)
    {
      Texture = texture;
      Offsets = offsets;
    }

    public struct GlythOffset
    {
      public int Left;
      public int Right;
    }
  }
}
