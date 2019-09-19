using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public class ImageStorage : BaseStorage
  {
    private ImageStorage(string libraryPath)
      : base(libraryPath)
    {
    }

    public static Sprite ReadFromFile(string filePath, Palette palette)
    {
      ImageStorage storage = new ImageStorage(filePath);
      storage.ParseFile();

      byte[] pixels = storage.Pixels;
      if (storage.Type == ResourceType.Sprite)
      {
        pixels = storage.Decompress();
      }

      return new Sprite(pixels, storage.Width, storage.Height);
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
          result = ParseOFFSSection(inputReader);
          break;
        case SectionType.OFFI:
          result = ParseOFFISection(inputReader);
          break;
      }

      return result;
    }

    private bool ParseOFFSSection(BinaryReader inputReader)
    {
      //TODO(adm244): read 8 bytes of each offset
      return false;
    }

    private bool ParseOFFISection(BinaryReader inputReader)
    {
      // Initial clip?
      UInt16 left = inputReader.ReadUInt16();
      UInt16 right = inputReader.ReadUInt16();

      return true;
    }
  }
}
