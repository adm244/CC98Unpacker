using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class PaletteStorage : ResourceStorage
  {
    private Color[] Colours;
    private byte[] Lookups;

    private PaletteStorage()
      : base()
    {
    }

    private PaletteStorage(string libraryPath)
      : base(libraryPath)
    {
      Colours = new Color[0];
      Lookups = new byte[0];
    }

    private PaletteStorage(string libraryPath, Palette palette)
      : base(libraryPath)
    {
      Colours = new Color[palette.Colours.Length];
      palette.Colours.CopyTo(Colours, 0);

      Lookups = new byte[palette.Lookups.Length];
      palette.Lookups.CopyTo(Lookups, 0);
    }

    public static Palette ReadFromFile(string filePath)
    {
      PaletteStorage storage = new PaletteStorage(filePath);
      if (!storage.ParseFile())
        return null;

      string name = Path.GetFileNameWithoutExtension(filePath);
      return new Palette(name, storage.Colours, storage.Lookups);
    }

    public static Palette ReadFromStream(BinaryReader inputReader, string name)
    {
      PaletteStorage storage = new PaletteStorage();
      if (!storage.Parse(inputReader))
        return null;

      return new Palette(name, storage.Colours, storage.Lookups);
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.ZPAL:
          result = ParseZPALSection(inputReader, section.Size);
          break;
        case SectionType.LKUP:
          result = ParseLKUPSection(inputReader, section.Size);
          break;

        default:
          Debug.Assert(false, "Section is not implemented");
          break;
      }

      return result;
    }

    private bool ParseZPALSection(BinaryReader inputReader, int sectionSize)
    {
      int count = (sectionSize / sizeof(Int32));

      Colours = new Color[count];
      for (int i = 0; i < Colours.Length; ++i)
      {
        Int32 value = inputReader.ReadInt32();
        Colours[i] = ColorExtension.FromABGR(value);
      }

      return (Colours.Length > 0);
    }

    private bool ParseLKUPSection(BinaryReader inputReader, int sectionSize)
    {
      //NOTE(adm244): used in fake font anti-aliasing
      Lookups = inputReader.ReadBytes(sectionSize);

      return true;
    }
  }
}
