using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class EntityStorage : ImageResourceStorage
  {
    private byte[] OFFI;
    private byte[] OFFS;

    private EntityStorage()
      : this(string.Empty)
    {
    }

    private EntityStorage(string libraryPath)
      : base(libraryPath)
    {
      OFFI = new byte[0];
      OFFS = new byte[0];
    }

    private EntityStorage(string libraryPath, Entity entity)
      : base(libraryPath, entity.Texture, ResourceType.Entity)
    {

    }

    public static Entity LoadFromFile(string filePath)
    {
      EntityStorage storage = new EntityStorage(filePath);
      using (FileStream inputStream = new FileStream(storage.LibraryPath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, storage.Encoding))
        {
          if (!storage.Parse(inputReader))
            return null;
        }
      }

      string name = Path.GetFileNameWithoutExtension(filePath);
      Texture texture = new Texture(name, storage.Pixels, storage.Width, storage.Height);
      return new Entity(name, texture, storage.OFFI, storage.OFFS);
    }

    public static bool SaveToFile(string filePath, Entity entity)
    {
      EntityStorage storage = new EntityStorage(filePath, entity);
      using (FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        using (BinaryWriter outputWriter = new BinaryWriter(outputStream, storage.Encoding))
        {
          if (!storage.Write(outputWriter))
          {
            Debug.Assert(false, "Cannot write an entity file!");
            return false;
          }
        }
      }

      return true;
    }

    public static Entity LoadFromStream(BinaryReader inputReader, string name)
    {
      EntityStorage storage = new EntityStorage();
      if (!storage.Parse(inputReader))
        return null;

      Texture texture = new Texture(name, storage.Pixels, storage.Width, storage.Height);
      return new Entity(name, texture, storage.OFFI, storage.OFFS);
    }

    public static bool SaveToStream(BinaryWriter outputWriter, Entity entity)
    {
      EntityStorage storage = new EntityStorage(string.Empty, entity);
      if (!storage.Write(outputWriter))
      {
        Debug.Assert(false, "Cannot write an entity file!");
        return false;
      }

      return true;
    }

    protected override bool Write(BinaryWriter outputWriter)
    {
      SectionType[] types = new SectionType[] {
        SectionType.INFO,
        SectionType.DATA,
        SectionType.OFFS,
        SectionType.OFFI
      };

      if (!base.Write(outputWriter, types))
        return false;

      return true;
    }

    protected override bool ParseSections(BinaryReader inputReader)
    {
      if (!base.ParseSections(inputReader))
        return false;

      if (!ParseSectionType(inputReader, SectionType.OFFS))
        return false;

      if (!ParseSectionType(inputReader, SectionType.OFFI))
        return false;

      return true;
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.OFFS:
          result = ParseOFFSSection(inputReader, section);
          break;
        case SectionType.OFFI:
          result = ParseOFFISection(inputReader, section);
          break;

        default:
          result = base.ParseSection(inputReader, section);
          break;
      }

      return result;
    }

    protected override bool WriteSection(BinaryWriter outputWriter, SectionType type)
    {
      throw new NotImplementedException();
    }

    private bool ParseOFFSSection(BinaryReader inputReader, Section section)
    {
      OFFS = inputReader.ReadBytes(section.Size);

      return true;
    }

    private bool ParseOFFISection(BinaryReader inputReader, Section section)
    {
      OFFI = inputReader.ReadBytes(section.Size);

      return true;
    }
  }
}
