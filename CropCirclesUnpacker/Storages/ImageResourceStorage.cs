using System;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public abstract class ImageResourceStorage : ResourceStorage
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

    protected ImageResourceStorage(string filePath, Sprite sprite, ResourceType type)
      : base(filePath)
    {
      Width = sprite.Width;
      Height = sprite.Height;

      Pixels = new byte[sprite.Pixels.Length];
      sprite.Pixels.CopyTo(Pixels, 0);

      Type = type;
    }

    protected override bool ParseSections(BinaryReader inputReader)
    {
      if (!ParseSectionType(inputReader, SectionType.INFO))
        return false;

      if (!ParseSectionType(inputReader, SectionType.DATA))
        return false;

      return true;
    }

    protected override bool WriteSections(BinaryWriter outputWriter, SectionType[] types)
    {
      if (!base.WriteSections(outputWriter, types))
        return false;

      Section sectionINFO = GetSection(SectionType.INFO);
      Section sectionDATA = GetSection(SectionType.DATA);

      if (sectionINFO.IsNull() || sectionDATA.IsNull())
      {
        Debug.Assert(false, "Cannot patch INFO section!");
        return false;
      }

      long currentPosition = outputWriter.BaseStream.Position;
      long patchPosition = (sectionINFO.Offset + (sizeof(Int32) * 3));

      outputWriter.BaseStream.Position = patchPosition;
      outputWriter.Write((Int32)sectionDATA.Size);

      outputWriter.BaseStream.Position = currentPosition;

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

    protected override bool WriteSection(BinaryWriter outputWriter, SectionType type)
    {
      bool result = false;

      switch (type)
      {
        case SectionType.INFO:
          result = WriteINFOSection(outputWriter);
          break;
        case SectionType.DATA:
          result = WriteDATASection(outputWriter);
          break;

        default:
          Debug.Assert(false, "Attempting to write an unimplemented section!");
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

    private bool WriteINFOSection(BinaryWriter outputWriter)
    {
      outputWriter.Write((Int32)Type);
      outputWriter.Write((Int16)Width);
      outputWriter.Write((Int16)Height);

      outputWriter.Write((Int32)0x1);
      //NOTE(adm244): patched later by WriteSections
      outputWriter.Write((UInt32)0xDEADBEEF);

      return true;
    }

    private bool ParseDATASection(BinaryReader inputReader, Section section)
    {
      //TODO(adm244): verify that padding is absent in compressed data
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

    private bool WriteDATASection(BinaryWriter outputWriter)
    {
      if (Type == ResourceType.Sprite)
      {
        byte[] buffer = Compress(Pixels);
        outputWriter.Write((byte[])buffer);
      }
      else
      {
        int modulo = (Width % 4);
        int padding = (modulo > 0) ? (4 - modulo) : 0;
        int pitch = Width + padding;

        byte[] row = new byte[pitch];
        for (int y = 0; y < Height; ++y)
        {
          Array.Copy(Pixels, y * Width, row, 0, Width);
          outputWriter.Write((byte[])row);
        }
      }

      return true;
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
            //FIX(adm244): replace 0x0A by clear color constant
            outputWriter.WriteBytes(0x0A, count);
            break;
          case 0xFE: // read
            byte[] colors = inputReader.ReadBytes(count);
            outputWriter.Write(colors);
            break;

          default:
            //TODO(adm244): proper error case handling
            Debug.Assert(false, "Image data is corrupted");
            break;
        }
      }

      return outputStream.ToArray();
    }

    private byte[] Compress(byte[] buffer)
    {
      throw new NotImplementedException();
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
