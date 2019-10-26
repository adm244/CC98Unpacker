using System.IO;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class ImageStorage : ImageResourceStorage
  {
    private byte[] OFFI;
    private byte[] OFFS;

    private ImageStorage()
      : this(string.Empty)
    {
    }

    private ImageStorage(string libraryPath)
      : base(libraryPath)
    {
      OFFI = new byte[0];
      OFFS = new byte[0];
    }

    public static Sprite ReadFromStream(BinaryReader inputReader, string name)
    {
      ImageStorage storage = new ImageStorage();
      if (!storage.Parse(inputReader))
        return null;

      return new Sprite(name, storage.Pixels, storage.Width, storage.Height);
    }

    public static Sprite ReadFromFile(string filePath)
    {
      ImageStorage storage = new ImageStorage(filePath);
      if (!storage.ParseFile())
        return null;

      string name = Path.GetFileNameWithoutExtension(filePath);
      return new Sprite(name, storage.Pixels, storage.Width, storage.Height);
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
