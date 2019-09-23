using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class PaletteStorage : ResourceStorage
  {
    private Int32[] Colours;
    private byte[] Lookups;

    private PaletteStorage(string libraryPath)
      : base(libraryPath)
    {
      Colours = new Int32[0];
      Lookups = new byte[0];
    }

    public static Palette ReadFromFile(string filePath)
    {
      PaletteStorage storage = new PaletteStorage(filePath);
      storage.ParseFile();

      Palette palette = new Palette(storage.Colours, storage.Lookups);
      return palette;
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
      int size = (sectionSize / sizeof(Int32));

      Colours = new Int32[size];
      for (int i = 0; i < Colours.Length; ++i)
      {
        Colours[i] = inputReader.ReadInt32();
      }

      return true;
    }

    private bool ParseLKUPSection(BinaryReader inputReader, int sectionSize)
    {
      Lookups = inputReader.ReadBytes(sectionSize);

      return true;
    }
  }
}
