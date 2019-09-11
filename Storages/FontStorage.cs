using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Assets;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public class FontStorage
  {
    public static readonly string FolderName = "fonts";

    private static readonly Int32 Signature = 0x6F72657A; // "zero"

    private List<Section> Sections;
    private UInt16[] Dimensions;
    private Int32[] Sizes;
    private byte[] Pixels;
    private GlythOffset[] GlythOffsets;

    private FontStorage()
    {
      Sections = new List<Section>(0);
      Dimensions = new UInt16[0];
      Sizes = new Int32[0];
      Pixels = new byte[0];
      GlythOffsets = new GlythOffset[0];
    }

    public static Font ReadFromFile(string filePath)
    {
      FontStorage storage = ParseZFTFile(filePath);

      //TODO(adm244): convert parsed data into a Font object

      return null;
    }

    private static FontStorage ParseZFTFile(string filePath)
    {
      FontStorage storage = new FontStorage();

      using (FileStream inputStream = new FileStream(filePath, FileMode.Open))
      {
        using (BinaryReader r = new BinaryReader(inputStream, Encoding.GetEncoding(1252)))
        {
          Console.WriteLine("Parsing {0}...", Path.GetFileName(filePath));

          if (!IsZFTFile(r))
          {
            Console.WriteLine("");
            return null;
          }

          //NOTE(adm244): do we care about attributes?
          char[] attributes = r.ReadChars(4);

          ParseSectionsTable(r, storage);
          ParseSections(r, storage);

          Console.WriteLine("Done!");
        }
      }

      return storage;
    }

    private static bool IsZFTFile(BinaryReader r)
    {
      Int32 signature = r.ReadInt32();
      if (signature != Signature)
        return false;

      return true;
    }

    private static void ParseSectionsTable(BinaryReader inputReader, FontStorage storage)
    {
      Int32 tableOffset = GetSectionsTableOffset(inputReader);
      inputReader.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

      Console.WriteLine("\tReading sections table...");

      storage.Sections = new List<Section>(4);
      Section currentSection;
      do
      {
        currentSection.Name = inputReader.ReadFixedString(4);
        currentSection.Size = inputReader.ReadInt32();
        currentSection.Offset = inputReader.ReadInt32();

        if (!currentSection.IsNull())
        {
          storage.Sections.Add(currentSection);
          Console.WriteLine("\t\tFound {0} section", currentSection.Name);
        }
      }
      while (!currentSection.IsNull());

      Console.WriteLine("\tDone!");
    }

    private static Int32 GetSectionsTableOffset(BinaryReader inputReader)
    {
      Console.Write("\tLocating sections table...");

      inputReader.BaseStream.Seek(-sizeof(Int32), SeekOrigin.End);
      Int32 tableOffset = inputReader.ReadInt32();

      Console.WriteLine(" Done!");

      return tableOffset;
    }

    private static void ParseSections(BinaryReader inputReader, FontStorage storage)
    {
      Console.WriteLine("\tParsing sections...");
      for (int i = 0; i < storage.Sections.Count; ++i)
      {
        if (storage.Sections[i].IsNull())
        {
          Console.WriteLine("\t\tSkipping empty section...");
          continue;
        }

        inputReader.BaseStream.Seek(storage.Sections[i].Offset, SeekOrigin.Begin);

        Console.Write("\t\tParsing {0} section...", storage.Sections[i].Name);
        
        Type enumType = typeof(SectionNames);
        if (Enum.IsDefined(enumType, storage.Sections[i].Name))
        {
          SectionNames currentSectionName =
            (SectionNames)Enum.Parse(enumType, storage.Sections[i].Name);
          switch (currentSectionName)
          {
            case SectionNames.INFO:
              ParseINFOSection(inputReader, storage);
              break;
            case SectionNames.DATA:
              ParseDATASection(inputReader, storage);
              break;
            case SectionNames.NUMO:
              ParseNUMOSection(inputReader, storage);
              break;
            case SectionNames.OFFS:
              ParseOFFSSection(inputReader, storage);
              break;
          }
        }
        else
        {
          Console.WriteLine(" Skipped (not implemented).");
        }

        Console.WriteLine(" Done!");
      }
      Console.WriteLine("\tDone!");
    }

    private static void ParseINFOSection(BinaryReader inputReader, FontStorage storage)
    {
      Int32 dimensionsCount = inputReader.ReadInt32();
      storage.Dimensions = new UInt16[dimensionsCount];
      for (int i = 0; i < storage.Dimensions.Length; ++i)
      {
        storage.Dimensions[i] = inputReader.ReadUInt16();
      }

      Int32 sizesCount = inputReader.ReadInt32();
      storage.Sizes = new Int32[sizesCount];
      for (int i = 0; i < storage.Sizes.Length; ++i)
      {
        storage.Sizes[i] = inputReader.ReadInt32();
      }
    }

    private static void ParseDATASection(BinaryReader inputReader, FontStorage storage)
    {
      Int32 pixelsCount = (storage.Dimensions[0] * storage.Dimensions[1]);
      storage.Pixels = inputReader.ReadBytes(pixelsCount);
    }

    private static void ParseNUMOSection(BinaryReader inputReader, FontStorage storage)
    {
      Int32 offsetsCount = inputReader.ReadInt32();
      storage.GlythOffsets = new GlythOffset[offsetsCount];
    }

    private static void ParseOFFSSection(BinaryReader inputReader, FontStorage storage)
    {
      for (int i = 0; i < storage.GlythOffsets.Length; ++i)
      {
        storage.GlythOffsets[i].X = inputReader.ReadInt32();
        storage.GlythOffsets[i].Y = inputReader.ReadInt32();
      }
    }

    private enum SectionNames
    {
      INFO,
      DATA,
      NUMO,
      OFFS,
    }

    private struct Section
    {
      public string Name;
      public Int32 Offset;
      public Int32 Size;

      public bool IsNull()
      {
        return (string.IsNullOrEmpty(Name) && (Offset == 0) && (Size == 0));
      }
    }

    private struct GlythOffset
    {
      public Int32 X;
      public Int32 Y;
    }
  }
}
