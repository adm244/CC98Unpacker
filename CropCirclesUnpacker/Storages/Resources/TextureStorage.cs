using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class TextureStorage : ImageResourceStorage
  {
    private Int32 Pitch;

    public TextureStorage()
      : this(string.Empty)
    {
    }

    public TextureStorage(string libraryPath)
      : base(libraryPath)
    {
      Pitch = 0;
    }

    public TextureStorage(string libraryPath, Texture texture)
      : base(libraryPath, texture, ResourceType.Texture)
    {
      int modulo = (Width % 4);
      int padding = (modulo > 0) ? (4 - modulo) : 0;
      Pitch = Width + padding;
    }

    public static Texture LoadFromFile(string filePath)
    {
      TextureStorage storage = new TextureStorage(filePath);
      using (FileStream inputStream = new FileStream(storage.LibraryPath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, storage.Encoding))
        {
          if (!storage.Parse(inputReader))
            return null;
        }
      }

      string name = Path.GetFileNameWithoutExtension(filePath);
      return new Texture(name, storage.Pixels, storage.Width, storage.Height);
    }

    public static bool SaveToFile(string filePath, Texture texture)
    {
      TextureStorage storage = new TextureStorage(filePath, texture);
      using (FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      {
        using (BinaryWriter outputWriter = new BinaryWriter(outputStream, storage.Encoding))
        {
          if (!storage.Write(outputWriter))
          {
            Debug.Assert(false, "Cannot write a texture file!");
            return false;
          }
        }
      }

      return true;
    }

    public static Texture LoadFromStream(BinaryReader inputReader, string name)
    {
      TextureStorage storage = new TextureStorage();
      if (!storage.Parse(inputReader))
        return null;

      return new Texture(name, storage.Pixels, storage.Width, storage.Height);
    }

    public static bool SaveToStream(BinaryWriter outputWriter, Texture texture)
    {
      TextureStorage storage = new TextureStorage(string.Empty, texture);
      if (!storage.Write(outputWriter))
      {
        Debug.Assert(false, "Cannot write a texture file!");
        return false;
      }

      return true;
    }

    protected override bool Write(BinaryWriter outputWriter)
    {
      SectionType[] types = new SectionType[] {
        SectionType.INFO,
        SectionType.DATA,
        SectionType.RBYT
      };

      if (!base.Write(outputWriter, types))
        return false;

      return true;
    }

    protected override bool ParseSections(BinaryReader inputReader)
    {
      if (!base.ParseSections(inputReader))
        return false;

      if (!ParseSectionType(inputReader, SectionType.RBYT))
        return false;

      return true;
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.RBYT:
          result = ParseRBYTSection(inputReader, section);
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
        case SectionType.RBYT:
          result = WriteRBYTSection(outputWriter);
          break;

        default:
          result = base.WriteSection(outputWriter, type);
          break;
      }

      return result;
    }

    private bool ParseRBYTSection(BinaryReader inputReader, Section section)
    {
      //NOTE(adm244): game's resource loader doesn't use this
      Pitch = inputReader.ReadInt32();

      return true;
    }

    private bool WriteRBYTSection(BinaryWriter outputWriter)
    {
      outputWriter.Write((Int32)Pitch);

      return true;
    }
  }
}
