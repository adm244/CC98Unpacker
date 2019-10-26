using System.Drawing;

namespace CropCirclesUnpacker.Assets
{
  public class Palette : Asset
  {
    public Color[] Colours;
    public byte[] Lookups;

    public Palette(string name, Color[] colours, byte[] lookups)
      : base(name, AssetType.Palette)
    {
      Colours = new Color[colours.Length];
      colours.CopyTo(Colours, 0);

      Lookups = new byte[lookups.Length];
      lookups.CopyTo(Lookups, 0);
    }
  }
}
