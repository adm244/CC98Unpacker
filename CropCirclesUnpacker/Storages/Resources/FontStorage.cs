using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class FontStorage : ImageResourceStorage
  {
    private Font.GlythOffset[] GlythOffsets;

    private FontStorage()
      : this(string.Empty)
    {
    }

    private FontStorage(string libraryPath)
      : base(libraryPath)
    {
      GlythOffsets = new Font.GlythOffset[0];
    }

    private FontStorage(string libraryPath, Font font)
      : base(libraryPath, font.Texture, ResourceType.Font)
    {
      GlythOffsets = new Font.GlythOffset[font.Offsets.Length];
      font.Offsets.CopyTo(GlythOffsets, 0);
    }

    public static Font LoadFromFile(string filePath)
    {
      FontStorage storage = new FontStorage(filePath);
      using (FileStream inputStream = new FileStream(storage.LibraryPath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, storage.Encoding))
        {
          if (!storage.Parse(inputReader))
          {
            Debug.Assert(false, "Cannot read a font file!");
            return null;
          }
        }
      }

      string name = Path.GetFileNameWithoutExtension(filePath);
      Sprite texture = new Sprite(name, storage.Pixels, storage.Width, storage.Height);
      return new Font(name, texture, storage.GlythOffsets);
    }

    public static bool SaveToFile(string filePath, Font font)
    {
      FontStorage storage = new FontStorage(filePath, font);
      using (FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        using (BinaryWriter outputWriter = new BinaryWriter(outputStream, storage.Encoding))
        {
          if (!storage.Write(outputWriter))
          {
            Debug.Assert(false, "Cannot write a font file!");
            return false;
          }
        }
      }

      return true;
    }

    public static Font LoadFromStream(BinaryReader inputReader, string name)
    {
      FontStorage storage = new FontStorage();
      if (!storage.Parse(inputReader))
      {
        Debug.Assert(false, "Cannot read a font file!");
        return null;
      }

      Sprite texture = new Sprite(name, storage.Pixels, storage.Width, storage.Height);
      return new Font(name, texture, storage.GlythOffsets);
    }

    public static bool SaveToStream(BinaryWriter outputWriter, Font font)
    {
      FontStorage storage = new FontStorage(string.Empty, font);
      if (!storage.Write(outputWriter))
      {
        Debug.Assert(false, "Cannot write a font file!");
        return false;
      }

      return true;
    }

    protected override bool Write(BinaryWriter outputWriter)
    {
      SectionType[] types = new SectionType[] {
        SectionType.INFO,
        SectionType.DATA,
        SectionType.NUMO,
        SectionType.OFFS
      };

      if (!base.Write(outputWriter, types))
        return false;

      return true;
    }

    protected override bool ParseSections(BinaryReader inputReader)
    {
      if (!base.ParseSections(inputReader))
        return false;

      if (!ParseSectionType(inputReader, SectionType.NUMO))
        return false;

      if (!ParseSectionType(inputReader, SectionType.OFFS))
        return false;

      return true;
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.NUMO:
          result = ParseNUMOSection(inputReader);
          break;
        case SectionType.OFFS:
          result = ParseOFFSSection(inputReader);
          break;

        default:
          result = base.ParseSection(inputReader, section);
          break;
      }

      return result;
    }

    protected override bool WriteSection(BinaryWriter outputWriter, SectionType type)
    {
      bool result = false;

      switch (type)
      {
        case SectionType.NUMO:
          result = WriteNUMOSection(outputWriter);
          break;
        case SectionType.OFFS:
          result = WriteOFFSSection(outputWriter);
          break;

        default:
          result = base.WriteSection(outputWriter, type);
          break;
      }

      return result;
    }

    private bool ParseNUMOSection(BinaryReader inputReader)
    {
      Int32 offsetsCount = inputReader.ReadInt32();
      GlythOffsets = new Font.GlythOffset[offsetsCount];

      return true;
    }

    private bool WriteNUMOSection(BinaryWriter outputWriter)
    {
      outputWriter.Write((Int32)GlythOffsets.Length);

      return true;
    }

    private bool ParseOFFSSection(BinaryReader inputReader)
    {
      //FIX(adm244): check section size
      for (int i = 0; i < GlythOffsets.Length; ++i)
      {
        GlythOffsets[i].Left = inputReader.ReadInt32();
        GlythOffsets[i].Right = inputReader.ReadInt32();
      }

      return true;
    }

    private bool WriteOFFSSection(BinaryWriter outputWriter)
    {
      for (int i = 0; i < GlythOffsets.Length; ++i)
      {
        outputWriter.Write((Int32)GlythOffsets[i].Left);
        outputWriter.Write((Int32)GlythOffsets[i].Right);
      }

      return true;
    }
  }
}
