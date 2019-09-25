using System;
using System.Drawing;

namespace CropCirclesUnpacker.Assets
{
  public class Font
  {
    private Sprite Texture;
    private GlythOffset[] Offsets;

    public Font(string name, Sprite texture, GlythOffset[] offsets)
    {
      Name = name;
      Texture = texture;
      Offsets = offsets;
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

    public struct GlythOffset
    {
      public int Left;
      public int Right;
    }
  }
}
