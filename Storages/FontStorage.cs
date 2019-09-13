﻿using System;
using System.IO;
using CropCirclesUnpacker.Assets;

namespace CropCirclesUnpacker.Storages
{
  public class FontStorage : BaseStorage
  {
    public static readonly string FolderName = "fonts";

    private UInt16[] Dimensions;
    private Int32[] Sizes;
    private byte[] Pixels;
    private GlythOffset[] GlythOffsets;

    private FontStorage(string libraryPath)
      : base(libraryPath)
    {
      Dimensions = new UInt16[0];
      Sizes = new Int32[0];
      Pixels = new byte[0];
      GlythOffsets = new GlythOffset[0];
    }

    public static Font ReadFromFile(string filePath)
    {
      FontStorage storage = new FontStorage(filePath);
      storage.ParseFile();

      //TODO(adm244): convert parsed data into a Font object

      return null;
    }

    protected override bool ParseSection(BinaryReader inputReader, Section section)
    {
      bool result = false;

      switch (section.Name)
      {
        case SectionNames.INFO:
          result = ParseINFOSection(inputReader);
          break;
        case SectionNames.DATA:
          result = ParseDATASection(inputReader);
          break;
        case SectionNames.NUMO:
          result = ParseNUMOSection(inputReader);
          break;
        case SectionNames.OFFS:
          result = ParseOFFSSection(inputReader);
          break;

        default:
          throw new NotImplementedException();
      }

      return result;
    }

    private bool ParseINFOSection(BinaryReader inputReader)
    {
      Int32 dimensionsCount = inputReader.ReadInt32();
      Dimensions = new UInt16[dimensionsCount];
      for (int i = 0; i < Dimensions.Length; ++i)
      {
        Dimensions[i] = inputReader.ReadUInt16();
      }

      Int32 sizesCount = inputReader.ReadInt32();
      Sizes = new Int32[sizesCount];
      for (int i = 0; i < Sizes.Length; ++i)
      {
        Sizes[i] = inputReader.ReadInt32();
      }

      return true;
    }

    private bool ParseDATASection(BinaryReader inputReader)
    {
      Int32 width = Dimensions[0];
      Int32 height = Dimensions[1];
      Int32 size = (width * height);

      Pixels = new byte[size];
      for (int y = 0; y < height; ++y)
      {
        byte[] row = inputReader.ReadBytes(width);
        row.CopyTo(Pixels, y * width);

        int padding = (int)(inputReader.BaseStream.Position % 4);
        if (padding > 0)
          inputReader.BaseStream.Seek(4 - padding, SeekOrigin.Current);
      }

      return true;
    }

    private bool ParseNUMOSection(BinaryReader inputReader)
    {
      Int32 offsetsCount = inputReader.ReadInt32();
      GlythOffsets = new GlythOffset[offsetsCount];

      return true;
    }

    private bool ParseOFFSSection(BinaryReader inputReader)
    {
      for (int i = 0; i < GlythOffsets.Length; ++i)
      {
        GlythOffsets[i].X = inputReader.ReadInt32();
        GlythOffsets[i].Y = inputReader.ReadInt32();
      }

      return true;
    }

    private struct GlythOffset
    {
      public Int32 X;
      public Int32 Y;
    }
  }
}
