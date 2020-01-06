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

    public static Palette LoadFromFile(string filePath)
    {
      PaletteStorage storage = new PaletteStorage(filePath);
      using (FileStream inputStream = new FileStream(storage.LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, storage.Encoding))
        {
          if (!storage.Parse(inputReader))
          {
            Debug.Assert(false, "Cannot parse a palette file!");
            return null;
          }
        }
      }

      string name = Path.GetFileNameWithoutExtension(filePath);
      return new Palette(name, storage.Colours, storage.Lookups);
    }

    public static bool SaveToFile(string filePath, Palette palette)
    {
      PaletteStorage storage = new PaletteStorage(filePath, palette);
      using (FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        using (BinaryWriter outputWriter = new BinaryWriter(outputStream, storage.Encoding))
        {
          if (!storage.Write(outputWriter))
          {
            Debug.Assert(false, "Cannot write a palette file!");
            return false;
          }
        }
      }

      return true;
    }

    public static Palette LoadFromStream(BinaryReader inputReader, string name)
    {
      PaletteStorage storage = new PaletteStorage();
      if (!storage.Parse(inputReader))
      {
        Debug.Assert(false, "Cannot parse a palette file!");
        return null;
      }

      return new Palette(name, storage.Colours, storage.Lookups);
    }

    public static bool SaveToStream(BinaryWriter outputWriter, Palette palette)
    {
      PaletteStorage storage = new PaletteStorage(string.Empty, palette);
      if (!storage.Write(outputWriter))
      {
        Debug.Assert(false, "Cannot write a palette file!");
        return false;
      }

      return true;
    }

    protected override bool Write(BinaryWriter outputWriter)
    {
      SectionType[] types = new SectionType[] {
        SectionType.ZPAL,
        SectionType.LKUP
      };

      if (!base.Write(outputWriter, types))
        return false;

      return true;
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

    protected override bool WriteSection(BinaryWriter outputWriter, SectionType type)
    {
      bool result = false;

      switch (type)
      {
        case SectionType.ZPAL:
          result = WriteZPALSection(outputWriter);
          break;
        case SectionType.LKUP:
          result = WriteLKUPSection(outputWriter);
          break;

        default:
          Debug.Assert(false, "Section is not implemented!");
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

    private bool WriteZPALSection(BinaryWriter outputWriter)
    {
      for (int i = 0; i < Colours.Length; ++i)
      {
        Int32 value = Colours[i].ToABGR();
        outputWriter.Write((Int32)value);
      }

      return true;
    }

    private bool ParseLKUPSection(BinaryReader inputReader, int sectionSize)
    {
      //NOTE(adm244): used in fake font anti-aliasing
      Lookups = inputReader.ReadBytes(sectionSize);

      return true;
    }

    private bool WriteLKUPSection(BinaryWriter outputWriter)
    {
      outputWriter.Write((byte[])Lookups);

      return true;
    }
  }
}
