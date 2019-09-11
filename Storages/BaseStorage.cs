using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public abstract class BaseStorage
  {
    protected static readonly Int32 Signature = 0x6F72657A; // "zero"

    protected string LibraryPath;
    private List<Section> Sections;

    protected BaseStorage(string filePath)
    {
      LibraryPath = filePath;
      Sections = new List<Section>(0);
    }

    protected abstract bool ParseSection(BinaryReader inputReader, SectionNames sectionName);

    protected bool ParseFile()
    {
      bool result = false;

      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding.GetEncoding(1252)))
        {
          Console.WriteLine("Parsing {0}...", Path.GetFileName(LibraryPath));

          if (!IsValidFile(inputReader))
          {
            Console.WriteLine("Failed. Invalid or corrupt file detected!");
            return false;
          }

          //NOTE(adm244): do we care about attributes?
          char[] attributes = inputReader.ReadChars(4);

          result = ParseSectionsTable(inputReader);
          result = ParseSections(inputReader);

          Console.WriteLine("Done!");
        }
      }

      return result;
    }

    protected bool IsValidFile(BinaryReader inputReader)
    {
      Int32 signature = inputReader.ReadInt32();
      if (signature != Signature)
        return false;

      return true;
    }

    private bool ParseSectionsTable(BinaryReader inputReader)
    {
      Int32 tableOffset = GetSectionsTableOffset(inputReader);
      inputReader.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

      Console.WriteLine("\tReading sections table...");

      Sections = new List<Section>(4);
      Section currentSection;
      do
      {
        currentSection.Name = inputReader.ReadFixedString(4);
        currentSection.Size = inputReader.ReadInt32();
        currentSection.Offset = inputReader.ReadInt32();

        if (!currentSection.IsNull())
        {
          Sections.Add(currentSection);
          Console.WriteLine("\t\tFound {0} section", currentSection.Name);
        }
      }
      while (!currentSection.IsNull());

      Console.WriteLine("\tDone!");

      return true;
    }

    private Int32 GetSectionsTableOffset(BinaryReader inputReader)
    {
      Console.Write("\tLocating sections table...");

      inputReader.BaseStream.Seek(-sizeof(Int32), SeekOrigin.End);
      Int32 tableOffset = inputReader.ReadInt32();

      Console.WriteLine(" Done!");

      return tableOffset;
    }

    private bool ParseSections(BinaryReader inputReader)
    {
      Console.WriteLine("\tParsing sections...");

      for (int i = 0; i < Sections.Count; ++i)
      {
        if (Sections[i].IsNull())
        {
          Console.WriteLine("\t\tSkipping empty section...");
          continue;
        }

        inputReader.BaseStream.Seek(Sections[i].Offset, SeekOrigin.Begin);

        Console.Write("\t\tParsing {0} section...", Sections[i].Name);

        bool isSectionImplemented = Enum.IsDefined(typeof(SectionNames), Sections[i].Name);
        if (!isSectionImplemented)
        {
          Console.WriteLine(" Skipped (not implemented).");
          continue;
        }

        SectionNames currentSectionName =
          (SectionNames)Enum.Parse(typeof(SectionNames), Sections[i].Name);
        if (!ParseSection(inputReader, currentSectionName))
          Console.WriteLine(" Error.");
        else
          Console.WriteLine(" Done!");
      }

      Console.WriteLine("\tDone!");

      return true;
    }

    protected enum SectionNames
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
  }
}
