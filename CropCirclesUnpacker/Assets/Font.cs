using System;
using System.Drawing;

namespace CropCirclesUnpacker.Assets
{
  public class Font : Asset
  {
    private Sprite Texture;
    private GlythOffset[] Offsets;

    public Font(string name, Sprite texture, GlythOffset[] offsets)
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
