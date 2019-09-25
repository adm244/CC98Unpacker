using System;
using System.Drawing;

namespace CropCirclesUnpacker.Assets
{
  public class Palette
  {
    public Color[] Entries;
    public byte[] Lookups;

    public Palette(string name, int[] entries, byte[] lookups)
    {
      Name = name;
      SetEntries(entries);

      Lookups = new byte[lookups.Length];
      lookups.CopyTo(Lookups, 0);
    }

    private void SetEntries(int[] entries)
    {
      Entries = new Color[entries.Length];
      for (int i = 0; i < Entries.Length; ++i)
      {
        int value = entries[i];

        int alpha = 0;
        int red = (value & 0xFF);
        int green = ((value >> 8) & 0xFF);
        int blue = ((value >> 16) & 0xFF);

        Entries[i] = Color.FromArgb(alpha, red, green, blue);
      }
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
  }
}
