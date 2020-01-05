using System;
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

    public static Font ReadFromFile(string filePath)
    {
      FontStorage storage = new FontStorage(filePath);
      using (FileStream inputStream = new FileStream(storage.LibraryPath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, storage.Encoding))
        {
          if (!storage.Parse(inputReader))
            return null;
        }
      }

      string name = Path.GetFileNameWithoutExtension(filePath);
      Sprite texture = new Sprite(name, storage.Pixels, storage.Width, storage.Height);
      return new Font(name, texture, storage.GlythOffsets);
    }

    public static Font ReadFromStream(BinaryReader inputReader, string name)
    {
      FontStorage storage = new FontStorage();
      if (!storage.Parse(inputReader))
        return null;

      Sprite texture = new Sprite(name, storage.Pixels, storage.Width, storage.Height);
      return new Font(name, texture, storage.GlythOffsets);
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

    protected override bool WriteSection(BinaryWriter outputWriter, ResourceStorage.SectionType type)
    {
      throw new NotImplementedException();
    }

    private bool ParseNUMOSection(BinaryReader inputReader)
    {
      Int32 offsetsCount = inputReader.ReadInt32();
      GlythOffsets = new Font.GlythOffset[offsetsCount];

      return true;
    }

    private bool ParseOFFSSection(BinaryReader inputReader)
    {
      for (int i = 0; i < GlythOffsets.Length; ++i)
      {
        GlythOffsets[i].Left = inputReader.ReadInt32();
        GlythOffsets[i].Right = inputReader.ReadInt32();
      }

      return true;
    }
  }
}
