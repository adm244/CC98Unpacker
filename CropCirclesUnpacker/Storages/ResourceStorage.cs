using System;
using System.Collections.Generic;
using System.IO;
using CropCirclesUnpacker.Extensions;
using System.Diagnostics;

namespace CropCirclesUnpacker.Storages
{
  public abstract class ResourceStorage : BaseStorage
  {
    protected List<Section> Sections;

    protected ResourceStorage()
      : this(string.Empty)
    {
    }

    protected ResourceStorage(string filePath)
      : base(filePath)
    {
      Sections = new List<Section>();
    }

    protected abstract bool ParseSection(BinaryReader inputReader, Section section);
    protected abstract bool WriteSection(BinaryWriter outputWriter, SectionType type);

    protected override bool Parse(BinaryReader inputReader)
    {
      bool result = false;

      Console.WriteLine("Parsing {0}...", Path.GetFileName(LibraryPath));

      if (!base.Parse(inputReader))
      {
        Console.WriteLine("Failed. Invalid or corrupt file detected!");
        return false;
      }

      result = ParseSectionsTable(inputReader);
      result = ParseSections(inputReader);

      Console.WriteLine("Done!");

      return result;
    }

    protected virtual bool Write(BinaryWriter outputWriter, SectionType[] types)
    {
      if (!base.Write(outputWriter))
        return false;

      if (!WriteSections(outputWriter, types))
        return false;

      if (!WriteSectionsTable(outputWriter))
        return false;

      return true;
    }

    private bool ParseSectionsTable(BinaryReader inputReader)
    {
      Int32 tableOffset = GetSectionsTableOffset(inputReader);
      inputReader.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

      Console.WriteLine("\tReading sections table...");

      Sections = new List<Section>();
      Section currentSection;
      do
      {
        currentSection = new Section(SectionType.Unknown, 0, 0);

        string name = inputReader.ReadFixedString(4);
        bool isSectionImplemented = Enum.IsDefined(typeof(SectionType), name);
        if (isSectionImplemented)
        {
          SectionType sectionName =
            (SectionType)Enum.Parse(typeof(SectionType), name);

          currentSection.Type = sectionName;
          currentSection.Size = inputReader.ReadInt32();
          currentSection.Offset = inputReader.ReadInt32();

          Sections.Add(currentSection);
          Console.WriteLine("\t\tFound {0} section", name);
        }
        else
        {
          if (!string.IsNullOrEmpty(name))
            Console.WriteLine("\t\tSkipping {0} section", name);
        }
      }
      while (!currentSection.IsNull());

      Console.WriteLine("\tDone!");

      return true;
    }

    private bool WriteSectionsTable(BinaryWriter outputWriter)
    {
      int offset = (int)outputWriter.BaseStream.Position;
      for (int i = 0; i < Sections.Count; ++i)
      {
        Sections[i].Write(outputWriter);
      }
      outputWriter.Write((Int32)offset);

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

    private Section GetSection(SectionType type)
    {
      for (int i = 0; i < Sections.Count; ++i)
      {
        if (Sections[i].Type == type)
          return Sections[i];
      }

      return new Section();
    }

    protected bool ParseSectionType(BinaryReader inputReader, SectionType type)
    {
      Section section = GetSection(type);
      if (section.IsNull())
        return false;

      inputReader.BaseStream.Seek(section.Offset, SeekOrigin.Begin);

      Console.Write("\t\tParsing {0} section...", section.Type);

      bool result = ParseSection(inputReader, section);
      if (!result)
        Console.WriteLine(" Error.");
      else
        Console.WriteLine(" Done!");

      return result;
    }

    protected virtual bool ParseSections(BinaryReader inputReader)
    {
      bool parseResult = true;

      Console.WriteLine("\tParsing sections...");

      for (int i = 0; i < Sections.Count; ++i)
      {
        if (Sections[i].IsNull())
        {
          Console.WriteLine("\t\tSkipping empty section...");
          continue;
        }

        inputReader.BaseStream.Seek(Sections[i].Offset, SeekOrigin.Begin);

        Console.Write("\t\tParsing {0} section...", Sections[i].Type);

        bool result = ParseSection(inputReader, Sections[i]);
        if (!result)
          Console.WriteLine(" Error.");
        else
          Console.WriteLine(" Done!");
        
        parseResult &= result;
      }

      Console.WriteLine("\tDone!");

      return parseResult;
    }

    protected virtual bool WriteSections(BinaryWriter outputWriter, SectionType[] types)
    {
      for (int i = 0; i < types.Length; ++i)
      {
        long start = outputWriter.BaseStream.Position;

        if (!WriteSection(outputWriter, types[i]))
          return false;

        long end = outputWriter.BaseStream.Position;

        int offset = (int)start;
        int size = (int)(end - start);

        Debug.Assert(offset > 0);
        Debug.Assert(size > 0);

        Section section = new Section(types[i], offset, size);
        Sections.Add(section);
      }

      Section emptySection = new Section();
      Sections.Add(emptySection);

      return true;
    }

    protected enum SectionType
    {
      Unknown = 0,
      INFO,
      DATA,
      NUMO,
      OFFS,
      OFFI,
      ZPAL,
      LKUP,
    }

    protected struct Section
    {
      public SectionType Type;
      public int Offset;
      public int Size;

      public Section(SectionType type, int offset, int size)
      {
        Type = type;
        Offset = offset;
        Size = size;
      }

      public bool IsNull()
      {
        return ((Type == SectionType.Unknown) && (Offset == 0) && (Size == 0));
      }

      public bool Write(BinaryWriter outputWriter)
      {
        if (IsNull())
        {
          outputWriter.Write((Int32)0);
        }
        else
        {
          string name = Type.ToString();
          Debug.Assert(name.Length == 4, "Section type name MUST be 4 characters long!");

          outputWriter.WriteFixedString(name, System.Text.Encoding.ASCII);
        }

        outputWriter.Write((Int32)Size);
        outputWriter.Write((Int32)Offset);

        return true;
      }
    }
  }
}
