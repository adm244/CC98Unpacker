using System.Drawing;

namespace CropCirclesUnpacker.Extensions
{
  public static class ColorExtension
  {
    public static int ToABGR(this Color color)
    {
      return (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
    }

    public static Color FromABGR(int abgr)
    {
      int red = (abgr & 0xFF);
      int green = ((abgr >> 8) & 0xFF);
      int blue = ((abgr >> 16) & 0xFF);
      int alpha = ((abgr >> 24) & 0xFF);

      return Color.FromArgb(alpha, red, green, blue);
    }
  }
}
