using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages.Resources
{
  public class FontStorage : ResourceStorage
  {
    private Font.GlythOffset[] GlythOffsets;

    private FontStorage()
      : base()
    {
    }

    private FontStorage(string libraryPath)
      : base(libraryPath)
    {
      GlythOffsets = new Font.GlythOffset[0];
    }

    public static Font ReadFromStream(BinaryReader inputReader, string name)
    {
      FontStorage storage = new FontStorage();
      if (!storage.ParseFile(inputReader))
        return null;

      Sprite texture = new Sprite(name, storage.Pixels, storage.Width, storage.Height, true);
      return new Font(name, texture, storage.GlythOffsets);
    }

    public static Font ReadFromFile(string filePath)
    {
      FontStorage storage = new FontStorage(filePath);
      if (!storage.ParseFile())
        return null;

      string name = Path.GetFileNameWithoutExtension(filePath);
      Sprite texture = new Sprite(name, storage.Pixels, storage.Width, storage.Height, true);
      return new Font(name, texture, storage.GlythOffsets);
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Type)
      {
        case SectionType.NUMO:
          result = ParseNUMOSection(inputReader);
          break;
        case SectionType.OFFS:
          result = ParseOFFSSection(inputReader);
          break;

        default:
          Debug.Assert(false, "Section is not implemented");
          break;
      }

      return result;
    }

    private bool ParseNUMOSection(BinaryReader inputReader)
    {
      Int32 offsetsCount = inputReader.ReadInt32();
      GlythOffsets = new Font.GlythOffset[offsetsCount];

      return true;
    }

    private bool ParseOFFSSection(BinaryReader inputReader)
    {
      for (int i = 0; i < GlythOffsets.Length; ++i)
      {
        GlythOffsets[i].Left = inputReader.ReadInt32();
        GlythOffsets[i].Right = inputReader.ReadInt32();
      }

      return true;
    }
  }
}
