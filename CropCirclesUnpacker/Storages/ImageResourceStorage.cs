using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public class ImageResourceStorage : ResourceStorage
  {
    protected int Width;
    protected int Height;
    protected byte[] Pixels;
    protected ResourceType Type;

    protected ImageResourceStorage()
      : this(string.Empty)
    {
    }

    protected ImageResourceStorage(string filePath)
      : base(filePath)
    {
      Width = 0;
      Height = 0;
      Pixels = new byte[0];
      Type = ResourceType.Unknown;
    }

    protected override bool ParseSections(BinaryReader inputReader)
    {
      if (!ParseSectionType(inputReader, SectionType.INFO))
        return false;

      if (!ParseSectionType(inputReader, SectionType.DATA))
        return false;

      return true;
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.INFO:
          result = ParseINFOSection(inputReader);
          break;
        case SectionType.DATA:
          result = ParseDATASection(inputReader, section);
          break;

        default:
          Debug.Assert(false, "Attempting to parse an unimplement section!");
          break;
      }

      return result;
    }

    private bool ParseINFOSection(BinaryReader inputReader)
    {
      Int32 resourceType = inputReader.ReadInt32();
      if (Enum.IsDefined(typeof(ResourceType), resourceType))
        Type = (ResourceType)resourceType;

      Width = inputReader.ReadUInt16();
      Height = inputReader.ReadUInt16();

      //NOTE(adm244): seems like those are never used
      Int32 unk04 = inputReader.ReadInt32(); // 0x1
      Int32 unk05 = inputReader.ReadInt32(); // sizeof(DATA)

      return true;
    }

    private bool ParseDATASection(BinaryReader inputReader, Section section)
    {
      if (Type == ResourceType.Sprite)
      {
        Pixels = inputReader.ReadBytes(section.Size);
        Pixels = Decompress(Pixels);
      }
      else
      {
        Pixels = new byte[Width * Height];
        for (int y = 0; y < Height; ++y)
        {
          byte[] row = inputReader.ReadBytes(Width);
          row.CopyTo(Pixels, y * Width);

          int padding = (int)(inputReader.BaseStream.Position % 4);
          if (padding > 0)
            inputReader.BaseStream.Seek(4 - padding, SeekOrigin.Current);
        }
      }

      return (Pixels != null);
    }

    private byte[] Decompress(byte[] buffer)
    {
      MemoryStream inputStream = new MemoryStream(buffer);
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

    protected enum ResourceType
    {
      Unknown = -1,
      Background = 0,
      Sprite = 1,
      Font = 2,
    }
  }
}
