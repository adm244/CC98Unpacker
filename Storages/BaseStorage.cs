using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CropCirclesUnpacker.Extensions;
using CropCirclesUnpacker.Utils;

namespace CropCirclesUnpacker.Storages
{
  public abstract class BaseStorage
  {
    protected static readonly Int32 Signature = 0x6F72657A; // "zero"

    protected string LibraryPath;
    protected Asset[] Assets;
    protected string[] Folders;
    protected string[] FileExtensions;
    private List<Section> Sections;

    private Encoding Encoding = Encoding.GetEncoding(1252);

    protected BaseStorage(string filePath)
    {
      LibraryPath = filePath;
      Assets = new Asset[0];
      Folders = new string[0];
      FileExtensions = new string[0];
      Sections = new List<Section>(0);
    }

    protected abstract bool ParseSection(BinaryReader inputReader, Section section);

    protected bool ParseArchive()
    {
      bool result = false;

      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
        {
          Console.WriteLine("Parsing {0}...", Path.GetFileName(LibraryPath));

          if (!IsValidFile(inputReader))
          {
            Console.WriteLine("Failed. Invalid or corrupt file detected!");
            return false;
          }

          //NOTE(adm244): do we care about attributes?
          char[] attributes = inputReader.ReadChars(4);

          result = ParseAssetsTable(inputReader);
          Folders = ParseStrings(inputReader);
          FileExtensions = ParseStrings(inputReader);

          Console.WriteLine("Done!");
        }
      }

      return result;
    }

    protected bool ParseFile()
    {
      bool result = false;

      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
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

    private bool ParseAssetsTable(BinaryReader inputReader)
    {
      FilesTableInfo tableInfo = GetFilesTableInfo(inputReader);
      inputReader.BaseStream.Seek(tableInfo.Offset, SeekOrigin.Begin);

      Console.WriteLine("\tReading assets table...");

      Assets = new Asset[tableInfo.Count];
      for (int i = 0; i < Assets.Length; ++i)
      {
        Assets[i].FolderIndex = inputReader.ReadInt16();
        Assets[i].ExtensionIndex = inputReader.ReadInt16();
        Assets[i].Offset = inputReader.ReadInt32();
        Assets[i].Size = inputReader.ReadInt32();
        Assets[i].Name = inputReader.ReadCString();

        Console.WriteLine("\t\tFound {0} asset", Assets[i].Name);
      }

      Console.WriteLine("\tDone!");

      return true;
    }

    private FilesTableInfo GetFilesTableInfo(BinaryReader inputReader)
    {
      FilesTableInfo tableInfo;

      Console.Write("\tLocating files table...");

      inputReader.BaseStream.Seek(-(2 * sizeof(Int32)), SeekOrigin.End);

      tableInfo.Offset = inputReader.ReadInt32();
      tableInfo.Count = inputReader.ReadInt32();

      Console.WriteLine(" Done!");

      return tableInfo;
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
        currentSection = new Section(SectionNames.Unknown, 0, 0);

        string name = inputReader.ReadFixedString(4);
        bool isSectionImplemented = Enum.IsDefined(typeof(SectionNames), name);
        if (isSectionImplemented)
        {
          SectionNames sectionName =
            (SectionNames)Enum.Parse(typeof(SectionNames), name);

          currentSection.Name = sectionName;
          currentSection.Size = inputReader.ReadInt32();
          currentSection.Offset = inputReader.ReadInt32();

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

        if (!ParseSection(inputReader, Sections[i]))
          Console.WriteLine(" Error.");
        else
          Console.WriteLine(" Done!");
      }

      Console.WriteLine("\tDone!");

      return true;
    }

    private static string[] ParseStrings(BinaryReader inputReader)
    {
      Console.Write("\tParsing strings...");

      Int32 dataSize = inputReader.ReadInt32();
      byte[] rawData = inputReader.ReadBytes(dataSize);
      Int32 stringsCount = inputReader.ReadInt32();

      string[] names = StringUtils.ConvertNullTerminatedSequence(rawData);
      Debug.Assert(names.Length == stringsCount);

      Console.WriteLine(" Done!");

      return names;
    }

    protected enum SectionNames
    {
      Unknown,
      INFO,
      DATA,
      NUMO,
      OFFS,
    }

    private struct FilesTableInfo
    {
      public Int32 Offset;
      public Int32 Count;
    }

    protected struct Asset
    {
      public Int16 FolderIndex;
      public Int16 ExtensionIndex;
      public Int32 Offset;
      public Int32 Size;
      public string Name;
    }

    protected struct Section
    {
      public SectionNames Name;
      public Int32 Offset;
      public Int32 Size;

      public Section(SectionNames name, int offset, int size)
      {
        Name = name;
        Offset = offset;
        Size = size;
      }

      public bool IsNull()
      {
        return ((Name == SectionNames.Unknown) && (Offset == 0) && (Size == 0));
      }
    }
  }
}
