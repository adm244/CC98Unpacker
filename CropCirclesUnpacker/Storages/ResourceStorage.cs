﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CropCirclesUnpacker.Extensions;

namespace CropCirclesUnpacker.Storages
{
  public abstract class ResourceStorage : BaseStorage
  {
    private List<Section> Sections;
    
    protected int Width;
    protected int Height;
    protected byte[] Pixels;
    protected ResourceType Type;

    protected ResourceStorage()
      : this(string.Empty)
    {
    }

    protected ResourceStorage(string filePath)
      : base(filePath)
    {
      Sections = new List<Section>(0);

      Width = 0;
      Height = 0;
      Pixels = new byte[0];
      Type = ResourceType.Unknown;
    }

    protected abstract bool ParseSection(BinaryReader inputReader, Section section);

    protected bool ParseFile()
    {
      bool result = false;

      using (FileStream inputStream = new FileStream(LibraryPath, FileMode.Open))
      {
        using (BinaryReader inputReader = new BinaryReader(inputStream, Encoding))
        {
          result = Parse(inputReader);
        }
      }

      return result;
    }

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

    private bool ParseSharedSection(BinaryReader inputReader, SectionType type)
    {
      Section section = GetSection(type);
      if (section.IsNull())
        return false;

      inputReader.BaseStream.Seek(section.Offset, SeekOrigin.Begin);

      Console.Write("\t\tParsing {0} section...", section.Type);

      bool result = false;
      switch (type)
      {
        case SectionType.INFO:
          result = ParseINFOSection(inputReader);
          break;
        case SectionType.DATA:
          result = ParseDATASection(inputReader, section);
          break;

        default:
          Debug.Assert(false, "Attempting to parse a non-shared section in a base class.");
          break;
      }

      if (!result)
        Console.WriteLine(" Error.");
      else
        Console.WriteLine(" Done!");

      Sections.Remove(section);

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

    private bool ParseSections(BinaryReader inputReader)
    {
      Console.WriteLine("\tParsing sections...");

      ParseSharedSection(inputReader, SectionType.INFO);
      ParseSharedSection(inputReader, SectionType.DATA);

      for (int i = 0; i < Sections.Count; ++i)
      {
        if (Sections[i].IsNull())
        {
          Console.WriteLine("\t\tSkipping empty section...");
          continue;
        }

        inputReader.BaseStream.Seek(Sections[i].Offset, SeekOrigin.Begin);

        Console.Write("\t\tParsing {0} section...", Sections[i].Type);

        if (!ParseSection(inputReader, Sections[i]))
          Console.WriteLine(" Error.");
        else
          Console.WriteLine(" Done!");
      }

      Console.WriteLine("\tDone!");

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

    protected enum ResourceType
    {
      Unknown = -1,
      Background = 0,
      Sprite = 1,
      Font = 2,
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
    }
  }
}
