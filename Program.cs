using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker
{
  public struct Section
  {
    public string Name;
    public Int32 Offset;
    public Int32 Size;

    public bool IsNull()
    {
      return (string.IsNullOrEmpty(Name) && (Offset == 0) && (Size == 0));
    }
  }

  public struct GlythOffset
  {
    public Int32 X;
    public Int32 Y;
  }

  class Program
  {
    private static UInt16[] Dimensions = new UInt16[0];
    private static Int32[] Sizes = new Int32[0];
    private static byte[] Pixels = new byte[0];
    private static GlythOffset[] GlythOffsets = new GlythOffset[0];

    public static void Main(string[] args)
    {
      string filePath = args[0];
      
      // media.dat extraction
      /*MediaStorage media = MediaStorage.ReadFromFile(filePath);
      media.ExtractTo(Environment.CurrentDirectory);*/

      // zft parsing
      using (FileStream inputStream = new FileStream(filePath, FileMode.Open))
      {
        using (BinaryReader r = new BinaryReader(inputStream, Encoding.GetEncoding(1252)))
        {
          Console.WriteLine("Parsing {0}...", Path.GetFileName(filePath));

          // read signature
          Int32 signature = r.ReadInt32();
          Debug.Assert(signature == 0x6F72657A);

          // read file attributes
          char[] attributes = r.ReadChars(4);

          // locate sections
          r.BaseStream.Seek(-sizeof(Int32), SeekOrigin.End);
          Int32 sectionsOffset = r.ReadInt32();

          r.BaseStream.Seek(sectionsOffset, SeekOrigin.Begin);

          Console.WriteLine("Reading sections table...");

          // read sections table
          List<Section> sections = new List<Section>(4);
          Section currentSection;
          do
          {
            currentSection.Name = r.ReadFixedString(4);
            currentSection.Size = r.ReadInt32();
            currentSection.Offset = r.ReadInt32();

            if (!currentSection.IsNull())
            {
              sections.Add(currentSection);
              Console.WriteLine("\tFound {0} section", currentSection.Name);
            }
          }
          while (!currentSection.IsNull());

          // parse sections
          for (int i = 0; i < sections.Count; ++i)
          {
            if (sections[i].IsNull())
              continue;

            Console.Write("Parsing {0} section...", sections[i].Name);

            r.BaseStream.Seek(sections[i].Offset, SeekOrigin.Begin);

            switch (sections[i].Name)
            {
              case "INFO":
                {
                  Int32 dimensionsCount = r.ReadInt32();
                  Dimensions = new UInt16[dimensionsCount];
                  for (int j = 0; j < Dimensions.Length; ++j)
                  {
                    Dimensions[j] = r.ReadUInt16();
                  }

                  Int32 sizesCount = r.ReadInt32();
                  Sizes = new Int32[sizesCount];
                  for (int j = 0; j < Sizes.Length; ++j)
                  {
                    Sizes[j] = r.ReadInt32();
                  }
                }
                break;

              case "DATA":
                {
                  Int32 pixelsCount = (Dimensions[0] * Dimensions[1]);
                  Pixels = r.ReadBytes(pixelsCount);
                }
                break;

              case "NUMO":
                {
                  Int32 offsetsCount = r.ReadInt32();
                  GlythOffsets = new GlythOffset[offsetsCount];
                }
                break;

              case "OFFS":
                {
                  for (int j = 0; j < GlythOffsets.Length; ++j)
                  {
                    GlythOffsets[j].X = r.ReadInt32();
                    GlythOffsets[j].Y = r.ReadInt32();
                  }
                }
                break;

              default:
                break;
            }

            Console.WriteLine(" Done!");
          }

          Console.WriteLine("Done!");
        }
      }
    }
  }
}
