using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class ImageStorage : ResourceStorage
  {
    private byte[] OFFI;
    private byte[] OFFS;

    private ImageStorage()
      : base()
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
      if (!storage.ParseFile(inputReader))
        return null;

      bool isBackground = true;
      byte[] pixels = storage.Pixels;
      if (storage.Type == ResourceType.Sprite)
      {
        isBackground = false;
        pixels = storage.Decompress();
      }

      return new Sprite(name, pixels, storage.Width, storage.Height, isBackground);
    }

    public static Sprite ReadFromFile(string filePath)
    {
      ImageStorage storage = new ImageStorage(filePath);
      if (!storage.ParseFile())
        return null;

      bool isBackground = true;
      byte[] pixels = storage.Pixels;
      if (storage.Type == ResourceType.Sprite)
      {
        isBackground = false;
        pixels = storage.Decompress();
      }

      string name = Path.GetFileNameWithoutExtension(filePath);
      return new Sprite(name, pixels, storage.Width, storage.Height, isBackground);
    }

    private byte[] Decompress()
    {
      MemoryStream inputStream = new MemoryStream(Pixels);
      BinaryReader inputReader = new BinaryReader(inputStream);

      MemoryStream outputStream = new MemoryStream();
      BinaryWriter outputWriter = new BinaryWriter(outputStream);

      while (!inputReader.EOF())
      {
        byte action = inputReader.ReadByte();
        byte count = inputReader.ReadByte();

        switch (action)
        {
          case 0xFF: // skip
            outputWriter.WriteBytes(0x0A, count);
            break;
          case 0xFE: // read
            byte[] colors = inputReader.ReadBytes(count);
            outputWriter.Write(colors);
            break;

          default:
            Debug.Assert(false, "Image data is corrupted");
            break;
        }
      }

      return outputStream.ToArray();
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
          Debug.Assert(false, "Section is not implemented");
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
